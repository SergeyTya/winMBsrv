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


namespace WindowsFormsApp4
{

    public partial class FormChart : Form
    {

        private string[] lables = {  "Частота, Гц", "Напряжение, В", "Шина DC, В", "Сеть, В",  "Ток, А", "Температура, С" , "Релейные сигналы" , 
       "ДПР Мотор" ,"Температура мотора, С",  "ДПР Виртуальный" , "ДПР Физический" ,  "Регистр 14", "Регистр 15",
       "Регистр 16", "Регистр 17", " ", " ", "", ""};

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
             chart1.Series[0].YValueType = ChartValueType.Auto;
            //chart1.Series[0].XValueType = ChartValueType.Auto;
            //chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Minutes;
            //chart1.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            //chart1.ChartAreas[0].AxisX.MaximumAutoSize = 100;
          //  chart1.ChartAreas[0].AxisX.Minimum = 0;
          //  chart1.ChartAreas[0].AxisX.Interval = 0.01;
            //art1.ChartAreas[0].AxisX.LabelStyle.Format = "mm:ss";
            chart1.ChartAreas[0].AxisX.Maximum = 1000;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "{0:0.0}";



            ToolStripMenuItem clearMenuItem = new ToolStripMenuItem("Очистить");
            ToolStripMenuItem zoomMenuItem = new ToolStripMenuItem("Масштаб");
            contextMenuForChart.Items.AddRange(new[] { zoomMenuItem, clearMenuItem });
            chart1.ContextMenuStrip = contextMenuForChart;

            clearMenuItem.Click += new System.EventHandler((s, e) => {
                foreach (Series el in chart1.Series) el.Points.Clear();
            }); 
            zoomMenuItem.Click += new System.EventHandler((s, e) => {
                chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            }); 

            foreach (Series el in chart1.Series)
            {
                el.BorderWidth = 3;
                el.BorderColor = Color.DarkBlue;
            }

            if (ind < lables.Length) {
                label1.Text = lables[ind];
                this.Text = lables[ind];
            }

        }

        public string Label {

            set {label1.Text = value;}
        }

        
        private double time = 0;
        private Stopwatch startTime = Stopwatch.StartNew();

        public void AddPoint(float point) {

            // if (chart1 == null) return;
            // DateTime baseDate = DateTime.Today;
            //// var x = baseDate.AddSeconds((double)cnt);
            // var x = DateTime.Now;
            // try
            // {
            //     //this.chart1.Series[0].Points.AddXY(cnt, point);
            //     this.chart1.Series[0].Points.Add(point);
            //     cnt++;
            // }
            // catch (System.NullReferenceException) { Debug.WriteLine("oops"); };

            // if (cnt >= chart1.ChartAreas[0].AxisX.Maximum)
            // {
            //     cnt = 0;
            //     foreach (var sr in chart1.Series) sr.Points.Clear();
            // }

            double scale = 30;
            if (chart1 == null) return;
            startTime.Stop();
            double timeStep = (double) startTime.ElapsedMilliseconds / 1000;
            startTime.Restart();

            if (timeStep == 0) timeStep = 0.1;
            try
            {
                
                double xAxisMaximum = Math.Round(time, 1);
                this.chart1.Series[0].Points.AddXY(time, point);

                if (xAxisMaximum < scale) xAxisMaximum = scale;

                if (this.chart1.Series[0].Points.Count > scale / timeStep) chart1.Series[0].Points.RemoveAt(0);

                chart1.ChartAreas[0].AxisX.Maximum = xAxisMaximum;

                if (chart1.Series.Count > 0 && chart1.Series[0].Points.Count > 0)
                    chart1.ChartAreas[0].AxisX.Minimum = Math.Round(chart1.Series[0].Points[0].XValue, 1);

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
                    ((Chart)sender).ChartAreas[0].AxisX.ScaleView.ZoomReset();
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
    }
}
