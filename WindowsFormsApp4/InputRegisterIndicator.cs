using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    internal class InputRegisterIndicator
    {

        public Label label;
        public TextBox indicator;
        public FormChart chart;

        private string RegSing = " ";

        public string Name
        { // name of input register
            get { return label.Text; }
            set
            {
                label.Text = RegSing + " " + value;
                chart.Label = label.Text;
            }
        }

        public bool RegSingEnable
        {

            set
            {
                if (value) RegSing = "Рег. " + Adr.ToString();
                if (!value) RegSing = "";
            }
        }

        private int scale;
        public int Scale
        {
            get { return scale; }
            set { scale = value; if (scale <= 0) scale = 1; }
        }

        public int Adr { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public string Dimension { get; set; }

        public int value
        {
            get { return 0; }
            set
            {
                float temp = (float)value / Scale;
                if (Scale == 1) { indicator.Text = value.ToString(); }
                else
                {
                    indicator.Text = temp.ToString("#####0.0") + " " + Dimension;
                }

                chart.AddPoint(value);
                indicator.ForeColor = Color.LightGreen;
                if (temp < Min) indicator.ForeColor = Color.Cyan;
                if (temp > Max) indicator.ForeColor = Color.LightCoral;
            }
        }


        public InputRegisterIndicator(int pos)
        {

            Adr = pos;
            chart = new FormChart(Adr - 3);
            Scale = 1;
            Min = -1;
            Max = 1;
            RegSingEnable = true;


            label = new Label();
            label.Dock = DockStyle.Fill;
            label.Margin = new System.Windows.Forms.Padding(1, 10, 1, 0);
            label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            Name = "";

            indicator = new TextBox();
            indicator.Name = "IRTextBox_" + Adr.ToString("D2");
            indicator.Dock = DockStyle.Fill;
            indicator.BackColor = System.Drawing.SystemColors.MenuText;
            indicator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            indicator.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            indicator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            indicator.HideSelection = false;
            indicator.Location = new System.Drawing.Point(15, 203);
            indicator.Margin = new System.Windows.Forms.Padding(15, 3, 50, 3);
            indicator.MaximumSize = new System.Drawing.Size(1000, 1000);
            indicator.ReadOnly = false;
            indicator.Size = new System.Drawing.Size(100, 33);
            indicator.TabIndex = 6;
            indicator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            indicator.Click += new System.EventHandler((s, e) =>
            {
                this.chart.Show();
                this.chart.BringToFront();
            });
        }
    }


}
