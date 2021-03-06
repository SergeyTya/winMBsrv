﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp4
{

    
    public partial class FormGensig : Form
    {
 
        private List<Control> myControl;
        private ControlsTable controlTable;

        private static class ParamNames {
           public static string Enable = "Выход Разрешен";
            public static string Type = "Тип сигнала";
            public static string Target = "РХ задание";
            public static string ResEnable = "Выводить отклик";
            public static string Responce = "РВ отклик";
            public static string Scale = "Развертка, с";
            public static string Period =  "Период, с";
            public static string Amplitude = "Aмплитуда";
            public static string Offset = "Смещение";

        }

        private class Control {
            
            public string   Name { set; get; }
            public string[] Range { set; get; }

            private double max;
            public double Max { set; get; }
            private double min;
            public double Min { set; get; }
            public bool Checkedbox;

            private double val;
            public double Value {
                set {

                    
                    if (value > Max) { val = Max; }
                    else
                    if (value < Min) { val = Min; }
                    else
                    { val = value; };
                }
                get {  
                    return val;
                }

            } 

            public Control(string name, string[] range = null, double max = 32768, double min =-32768, double def = 0, bool checkedbox = false)
            {
                Name = name;
                Range = range;
                Max = max;
                Min = min;
                Value = def;
                Checkedbox = checkedbox;
            }

        }

        private class ControlsTable
        {

            public Dictionary<String , Control> Controls;
            private DataGridView Table;

            public ControlsTable(DataGridView table) {

                Table = table;
                Controls = new Dictionary<String, Control>();

                Controls.Add(ParamNames.Enable,      new Control( ParamNames.Enable, checkedbox: true, max: 1, min: 0));
                Controls.Add(ParamNames.ResEnable, new Control(ParamNames.ResEnable, checkedbox: true, max: 1, min: 0));
                Controls.Add(ParamNames.Target, new Control(ParamNames.Target, def: 20));
                Controls.Add(ParamNames.Responce, new Control(ParamNames.Responce, def: 12));
                Controls.Add(ParamNames.Scale, new Control(ParamNames.Scale, min: 0.1, def: 10));
                Controls.Add(ParamNames.Type,        new Control( ParamNames.Type, range: new string[3] { "Синус", "Меандр", "Треугольник" }, max: 2, min: 0));
                Controls.Add(ParamNames.Period,      new Control(ParamNames.Period, min: 0.1, def: 15));
                Controls.Add(ParamNames.Amplitude,   new Control(ParamNames.Amplitude, min: 1, def: 3000));
                Controls.Add(ParamNames.Offset,      new Control(ParamNames.Offset, min: 0));

                foreach (String name in Controls.Keys)
                {
                    AddGridRow(Controls[name]);
                }

            }

            private void AddGridRow(Control control)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(Table);
                row.SetValues(new object[] { control.Name, 0 });
                row.Cells[0].ReadOnly = true;
                row.Cells[1].Value = control.Value;

                if (control.Range != null)
                {
                    var combo = new DataGridViewComboBoxCell();
                    combo.Items.AddRange(control.Range);
                    combo.Value = combo.Items[0];
                    row.Cells[1] = combo;
                }

                if (control.Checkedbox) {

                    var checkedBox = new DataGridViewCheckBoxCell();
                    checkedBox.Value = false;
                    row.Cells[1] = checkedBox;

                }

                
                Table.Rows.Add(row);
            }
        }


        public FormGensig()
        {
            InitializeComponent();
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "{0:0.00}";
            chart1.Series[0].Color = Color.Gray;
            chart1.Series[1].Color = Color.DarkBlue;
            chart1.Series[2].Color = Color.Red;
            chart1.MouseWheel += new MouseEventHandler(chData_MouseWheel);
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].CursorX.Interval = 0.005;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            chart1.ChartAreas[0].CursorY.AutoScroll = true;

            chart2.ChartAreas[0].AxisX.LabelStyle.Format = "{0:0.00}";
            chart2.Series[0].Color = Color.Red;
            chart2.MouseWheel += new MouseEventHandler(chData_MouseWheel);
            chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].CursorX.Interval = 0.005;
            chart2.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart2.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart2.ChartAreas[0].CursorX.AutoScroll = true;
            chart2.ChartAreas[0].CursorY.AutoScroll = true;

            chart2.GetToolTipText += chart_GetToolTipText;
            chart2.Series[0].ToolTip = "X = #VALX, Y = #VALY";

            ToolStripMenuItem clearMenuItem = new ToolStripMenuItem("Очистить");
            ToolStripMenuItem zoomMenuItem = new ToolStripMenuItem("Масштаб");
            ContextMenuStrip contextMenuForChart = new ContextMenuStrip();
            contextMenuForChart.Items.AddRange(new[] { zoomMenuItem, clearMenuItem });
            chart1.ContextMenuStrip = contextMenuForChart;

            clearMenuItem.Click += new System.EventHandler((s, e) => {
                foreach (System.Windows.Forms.DataVisualization.Charting.Series el in chart1.Series) el.Points.Clear();
                foreach (System.Windows.Forms.DataVisualization.Charting.Series el in chart2.Series) el.Points.Clear();
                time = 0;
            });
            zoomMenuItem.Click += new System.EventHandler((s, e) => {
                chart1.ChartAreas[0].AxisY.Minimum = -1;
                chart1.ChartAreas[0].AxisY.Maximum =  1;
                chart2.ChartAreas[0].AxisY.Minimum = -1;
                chart2.ChartAreas[0].AxisY.Maximum = 1;
            });

            // chart1.ChartAreas[0].CursorY.AutoScroll = true;

            controlTable = new ControlsTable(this.dataGridView1);
            dataGridView1.DataSource = myControl;


        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
          
            if (e.ColumnIndex != 1) return;
            if (dataGridView1.Rows[e.RowIndex].Cells[1].GetType() == typeof(DataGridViewTextBoxCell))
            {
                double num = 0;
                if (!double.TryParse(e.FormattedValue.ToString(), out num))
                {
                    e.Cancel = true;
                    return;
                }
            }

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string name = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            if (dataGridView1.Rows[e.RowIndex].Cells[1].GetType() == typeof(DataGridViewComboBoxCell)) {

                UInt16 index = (UInt16)(dataGridView1.Rows[e.RowIndex].Cells[1] as DataGridViewComboBoxCell).Items.IndexOf
                                   (
                                       dataGridView1.Rows[e.RowIndex].Cells[1].Value
                                   );
                this.controlTable.Controls[name].Value = index;
                return;

            };

            if (dataGridView1.Rows[e.RowIndex].Cells[1].GetType() == typeof(DataGridViewTextBoxCell))
            {
                double num = 0;
                if (double.TryParse(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(), out num))
                {
                    this.controlTable.Controls[name].Value = num;
                    if (this.controlTable.Controls[name].Value != num)
                        dataGridView1.Rows[e.RowIndex].Cells[1].Value = this.controlTable.Controls[name].Value;

                }
                return;
            };

            if (dataGridView1.Rows[e.RowIndex].Cells[1].GetType() == typeof(DataGridViewCheckBoxCell))
            {

                bool res = (bool)(dataGridView1.Rows[e.RowIndex].Cells[1] as DataGridViewCheckBoxCell).Value;

                this.controlTable.Controls[name].Value = 0;
                if (res) this.controlTable.Controls[name].Value = 1;

                return;
            }



        }

        private double time = 0;
        private double gen_dir = 1;
        private double point = 0;
        private double scale = 1;
        private double responce = 0;
        private double targetRef = 0;

        public void proc(double Ts) {
            if (chart1 == null || chart2 == null) return;
            if (!this.Visible) return;

            double time_step = Ts;
            double xAxisMaximum = Math.Round(time, 3);
            double amp = controlTable.Controls[ParamNames.Amplitude].Value;
            double period = controlTable.Controls[ParamNames.Period].Value;
            double offset = controlTable.Controls[ParamNames.Offset].Value;


            switch (controlTable.Controls[ParamNames.Type].Value)
            {
                case 0:
                    point = amp * Math.Sin(2 * Math.PI * time / period) + offset;
                    break;
                case 1:
                    point = amp * Math.Sin(2 * Math.PI * time / period);
                    if (point > 0) point = amp + offset;
                    if (point < 0) point = -amp + offset;
                    break;
                case 2:
                    double delta = 4 * amp * time_step / period;
                    point += delta * gen_dir;
                    if (point - offset >= amp) gen_dir = -1;
                    if (point - offset <= -amp) gen_dir = 1;

                    break;
                default:

                    break;
            };

            try
            {
                if (scale != controlTable.Controls[ParamNames.Scale].Value)
                {
                    scale = controlTable.Controls[ParamNames.Scale].Value;
                    foreach (System.Windows.Forms.DataVisualization.Charting.Series el in chart1.Series) el.Points.Clear();
                    foreach (System.Windows.Forms.DataVisualization.Charting.Series el in chart2.Series) el.Points.Clear();
                }


                if (chart1.ChartAreas[0].AxisY.Minimum > point) chart1.ChartAreas[0].AxisY.Minimum = Math.Round(point - 5, 0);
                if (chart1.ChartAreas[0].AxisY.Maximum < point) chart1.ChartAreas[0].AxisY.Maximum = Math.Round(point + 5, 0);

                if (chart2.ChartAreas[0].AxisY.Minimum > targetRef - responce) chart2.ChartAreas[0].AxisY.Minimum = Math.Round(targetRef - responce - 5, 0);
                if (chart2.ChartAreas[0].AxisY.Maximum < targetRef - responce) chart2.ChartAreas[0].AxisY.Maximum = Math.Round(targetRef - responce + 5, 0);
                if (chart2.ChartAreas[0].AxisY.Maximum > 100) chart2.ChartAreas[0].AxisY.Maximum = 100;
                if (chart2.ChartAreas[0].AxisY.Minimum < -100) chart2.ChartAreas[0].AxisY.Minimum = -100;

                if (xAxisMaximum < scale) xAxisMaximum = scale;

                this.chart1.Series[0].Points.AddXY(time, point);
                this.chart1.Series[1].Points.AddXY(time, targetRef);
                this.chart1.Series[2].Points.AddXY(time, responce);

                this.chart2.Series[0].Points.AddXY(time, targetRef - responce);


                this.chart1.Series[2].Enabled = controlTable.Controls[ParamNames.ResEnable].Value == 1;

                if (this.chart1.Series[0].Points.Count > scale / time_step)
                {
                    foreach (Series el in chart1.Series) el.Points.RemoveAt(0);
                    foreach (Series el in chart2.Series) el.Points.RemoveAt(0);
                }


                chart1.ChartAreas[0].AxisX.Maximum = xAxisMaximum;
                chart1.ChartAreas[0].AxisX.Minimum = Math.Round(chart1.Series[0].Points[0].XValue, 3);

                chart2.ChartAreas[0].AxisX.Maximum = xAxisMaximum;
                chart2.ChartAreas[0].AxisX.Minimum = Math.Round(chart2.Series[0].Points[0].XValue, 3);
            }
            catch (NullReferenceException) { };

            this.time += time_step;

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

        public UInt16 GetReference() {

            Int16 reference = (Int16) Math.Round(point, 0);
            return (UInt16) reference;
        }

        public UInt16 GetTargetHR()
        {
            UInt16 target_HR = (UInt16) Math.Round(controlTable.Controls[ParamNames.Target].Value, 0);
            return target_HR;
        }

        public UInt16 GetResponceIR()
        {
            UInt16 target_IR = (UInt16)Math.Round(controlTable.Controls[ParamNames.Responce].Value, 0);
            return target_IR;
        }

        public void SetResponce(UInt16 value)
        {
            responce =(Int16) value;
        }

        public void SetTargetRef(UInt16 value)
        {
            targetRef = (Int16)value;
        }

        public bool GetState() {

            double enable = controlTable.Controls[ParamNames.Enable].Value;
            if (enable == 1) return true;

            return false;
        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }

        private void FormGensig_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            (dataGridView1.Rows[0].Cells[1] as DataGridViewCheckBoxCell).Value = 0;
            double enable = controlTable.Controls[ParamNames.Enable].Value = 0;
            enable = 0;
            foreach (System.Windows.Forms.DataVisualization.Charting.Series el in chart1.Series) el.Points.Clear();
            foreach (System.Windows.Forms.DataVisualization.Charting.Series el in chart2.Series) el.Points.Clear();
            time = 0;


        }


        private void chart_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            // Если текст в подсказке уже есть, то ничего не меняем.
            if (!String.IsNullOrWhiteSpace(e.Text))
                return;

            Console.WriteLine(e.HitTestResult.ChartElementType);

            switch (e.HitTestResult.ChartElementType)
            {
                case ChartElementType.DataPoint:
                case ChartElementType.DataPointLabel:
                case ChartElementType.Gridlines:
                case ChartElementType.Axis:
                case ChartElementType.TickMarks:
                case ChartElementType.PlottingArea:
                    // Первый ChartArea
                    var area = chart2.ChartAreas[0];

                    // Его относительные координаты (в процентах от размеров Chart)
                    var areaPosition = area.Position;

                    // Переводим в абсолютные
                    var areaRect = new RectangleF(areaPosition.X * chart2.Width / 100, areaPosition.Y * chart2.Height / 100,
                        areaPosition.Width * chart2.Width / 100, areaPosition.Height * chart2.Height / 100);

                    // Область построения (в процентах от размеров area)
                    var innerPlot = area.InnerPlotPosition;

                    double x = area.AxisX.Minimum +
                                (area.AxisX.Maximum - area.AxisX.Minimum) * (e.X - areaRect.Left - innerPlot.X * areaRect.Width / 100) /
                                (innerPlot.Width * areaRect.Width / 100);
                    double y = area.AxisY.Maximum -
                                (area.AxisY.Maximum - area.AxisY.Minimum) * (e.Y - areaRect.Top - innerPlot.Y * areaRect.Height / 100) /
                                (innerPlot.Height * areaRect.Height / 100);

                    Console.WriteLine("{0:F2} {1:F2}", x, y);
                    e.Text = String.Format("{0:F2} {1:F2}", x, y);
                    break;
            }
        }
    }
}
