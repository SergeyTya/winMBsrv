using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading.Tasks;



namespace WindowsFormsApp4
{

    public partial class FormChart : Form
    {

        private string[] lables = {  "Частота, Гц", "Напряжение, В", "Шина DC, В", "Сеть, В",  "Ток, А", "Температура, С" , "Релейные сигналы" , 
       "ДПР Мотор" ,"Температура мотора, С",  "ДПР Виртуальный" , "ДПР Физический" ,  "Регистр 14", "Регистр 15",
       "Регистр 16", "Регистр 17", " ", " ", "", ""};

        private List<Points> ChartData = new List<Points>();
        private double maxScan = 10;

        private class Points
        {

            public double Time { get; set; }
            public double Values { get; set; }

            public Points(double time, double values)
            { 
                Time = time;
                Values = values;
            }
        }

        private bool resresh_enable = true;
        private int index = 0;
        public FormChart(int ind)
        {
          
            index = ind;
            InitializeComponent();
            ContextMenuStrip contextMenuForChart = new ContextMenuStrip();


            chart1.ChartAreas[0].AxisX.LineColor = Color.Black;
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray;
            chart1.ChartAreas[0].AxisY.LineColor = Color.Black;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gray;
            chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;
            chart1.MouseWheel += new MouseEventHandler(chData_MouseWheel);
            // настройка параметров зума
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].CursorX.Interval = 0.005;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            chart1.ChartAreas[0].CursorY.AutoScroll = true;

            chart1.Series[0].XValueType = ChartValueType.Auto;
            // chart1.Series[0].YValueType = ChartValueType.Auto;
            //chart1.Series[0].XValueType = ChartValueType.Auto;
            //chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Minutes;
            //chart1.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            //chart1.ChartAreas[0].AxisX.MaximumAutoSize = 100;
          //  chart1.ChartAreas[0].AxisX.Minimum = 0;
          //  chart1.ChartAreas[0].AxisX.Interval = 0.01;
            //art1.ChartAreas[0].AxisX.LabelStyle.Format = "mm:ss";
           // chart1.ChartAreas[0].AxisX.Maximum = 1000;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "{0:0.0}";



            ToolStripMenuItem clearMenuItem = new ToolStripMenuItem("Очистить");
            ToolStripMenuItem zoomMenuItem = new ToolStripMenuItem("Масштаб");
            ToolStripMenuItem PoinStyle = new ToolStripMenuItem("Тип графика");
            ToolStripMenuItem ScaneMenuItem = new ToolStripMenuItem("Развертка");

            ToolStripMenuItem PoinStyle_Point = new ToolStripMenuItem("Точки");
            ToolStripMenuItem PoinStyle_Line = new ToolStripMenuItem("Линия");

            ToolStripTextBox ScanToolStripTextBox = new ToolStripTextBox();


            ScaneMenuItem.DropDownItems.Add(ScanToolStripTextBox);
            ScanToolStripTextBox.KeyPress += new KeyPressEventHandler((s, e) =>
            {
                char num = e.KeyChar;
                if (!Char.IsDigit(num) && num != 8) e.Handled = true;
            });

            ScanToolStripTextBox.TextChanged += new System.EventHandler((s, e) => {

                double val = 10;
                try
                {
                    val = Convert.ToDouble(ScanToolStripTextBox.Text);

                    if (val < 0) return; 
                    if (val < maxScan)
                    {
                        ChartData.Clear();
                        time = 0;
                    }
                    maxScan = val;
                }
                catch (System.FormatException) { return; };
                

            });

            ScaneMenuItem.DropDownOpening += new System.EventHandler((s, e) => {
                ScanToolStripTextBox.Text = maxScan.ToString();
            });


           
            PoinStyle.DropDownItems.Add(PoinStyle_Point);
            PoinStyle.DropDownItems.Add(PoinStyle_Line);

            clearMenuItem.Click += new System.EventHandler((s, e) => {
               ChartData.Clear();
               time = 0;
            }); 

            zoomMenuItem.Click += new System.EventHandler((s, e) => 
            {
                chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            });

            PoinStyle_Point.Click += new System.EventHandler((s, e) =>
            {
                chart1.Series[0].ChartType = SeriesChartType.Point;
            });

            PoinStyle_Line.Click += new System.EventHandler((s, e) =>
            {
                chart1.Series[0].ChartType = SeriesChartType.FastLine;
            });

            contextMenuForChart.Items.AddRange(new[] { zoomMenuItem, clearMenuItem, PoinStyle, ScaneMenuItem });
            chart1.ContextMenuStrip = contextMenuForChart;

            foreach (Series el in chart1.Series)
            {
                el.BorderWidth = 3;
                el.BorderColor = Color.DarkBlue;
            }

            if (ind < lables.Length) {
                label1.Text = lables[ind];
                this.Text = lables[ind];
            }

            chart1.Series[0].XValueMember = "Time";
            chart1.Series[0].YValueMembers = "Values";

            chart1.DataSource = ChartData;

            Task_FormRefreshAsync();


        }

        public string Label {

            set {label1.Text = value;}
        }


        private async void Task_FormRefreshAsync()
        {

            while (true)
            {

               if(resresh_enable) chart1.DataBind();

                await Task.Delay(300);
            }
        }

        private double time = 0;
        private Stopwatch startTime = Stopwatch.StartNew();

        public void AddPoint(double point) {

            
            if (chart1 == null) return;
            startTime.Stop();
            double timeStep = (double) startTime.ElapsedMilliseconds / 1000;
            startTime.Restart();

           // if (timeStep == 0) timeStep = 0.1;
            try
            {
                
                double xAxisMaximum = Math.Round(time, 1);
                ChartData.Add(new Points(time, point));
                if (ChartData.Count > (int)(maxScan / timeStep)) ChartData.RemoveAt(0);
                time += timeStep;
            }
            catch (System.NullReferenceException) { Debug.WriteLine("oops"); };


        }

        private void chart_form_Click(object sender, EventArgs e)
        {

        }

        private void FormChart_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();


        }

        private void clearChartItem_Click(object sender, EventArgs e)
        {
            
        }

        //Прокрутка колесиком
        private void chData_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0)
                {
                    ((Chart)sender).ChartAreas[0].AxisY.ScaleView.ZoomReset();
                }

                if (e.Delta > 0)
                {
                    double xMin = ((Chart)sender).ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    double xMax = ((Chart)sender).ChartAreas[0].AxisX.ScaleView.ViewMaximum;

                    double posXStart = ((Chart)sender).ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    double posXFinish = ((Chart)sender).ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;

                    ((Chart)sender).ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                }
            }
            catch { }
        }

        private void chart1_DoubleClick(object sender, EventArgs e)
        {
            
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void btn_pause_Click(object sender, EventArgs e)
        {
            resresh_enable = !resresh_enable;
            if (!resresh_enable) { btn_pause.BackColor = Color.Gray; } else { btn_pause.BackColor = Color.White; }
        }

        private void btn_rec_Click(object sender, EventArgs e)
        {
            List <Points> temp = ChartData;
            List <double[]> data = new List<double[]>();

            int key = 1;
            foreach (Points el in temp)
            {

                data.Add(new double[] { el.Time,el.Values});
            }

            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Add();
            Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1];


            //int key = 1;
            //foreach (Points el in temp) {

            //    ws.Cells[key, 1] = el.Time;
            //    ws.Cells[key, 2] = el.Values;
            //    ws.Cells.bi
            //    key++;

            //}

            Microsoft.Office.Interop.Excel.Range rng = (Microsoft.Office.Interop.Excel.Range)ws.Range[ws.Cells[1, 1], ws.Cells[2, data.Count + 2]];
            rng.Value = data.ToArray();


            app.Visible = true;

        }
    }
}
