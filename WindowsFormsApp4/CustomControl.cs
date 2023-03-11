using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestGen
{

    internal class CustomControl
    {

        public delegate void MethodContainer();
        public event MethodContainer OnControlEvetn;

        internal void ControlEvetn()
        {
            OnControlEvetn();
        }

        public string Name { set; get; }
        public string[] Range { set; get; }

        private double max;
        public double Max { set; get; }
        private double min;
        public double Min { set; get; }
        public bool Checkedbox;

        private double val;
        public double Value
        {
            set
            {


                if (value > Max) { val = Max; }
                else
                if (value < Min) { val = Min; }
                else
                { val = value; };
            }
            get
            {
                return val;
            }

        }

        public CustomControl(
            string name, 
            string[] range = null, 
            double max = 32768, 
            double min = -32768, 
            double def = 0, 
            bool checkedbox = false
        )
        {
            Name = name;
            Range = range;
            Max = max;
            Min = min;
            Value = def;
            Checkedbox = checkedbox;
        }



    }



    internal class ControlsTable
    {

        private Dictionary<String, CustomControl> controls;
        private DataGridView table;

        public ControlsTable(DataGridView table)
        {

            this.table = table;
            controls = new Dictionary<String, CustomControl>();

        }


        public void addControl(CustomControl control) {

            controls.Add(control.Name, control);
        }

        public void RenderTable() {

            foreach (String name in controls.Keys)
            {
                AddGridRow(controls[name]);
            }

            table.CellValidating += DataGridView1_CellValidating;
            table.CellEndEdit += dataGridView1_CellEndEdit;
            table.ColumnHeadersVisible = false;
            table.RowHeadersVisible = false;

            table.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            table.Dock = System.Windows.Forms.DockStyle.Fill;
            table.Location = new System.Drawing.Point(3, 3);
        }

        public double getControlValue(string name) { 
            return controls[name].Value;
        }

        private void AddGridRow(CustomControl control)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(this.table);
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

            if (control.Checkedbox)
            {

                var checkedBox = new DataGridViewCheckBoxCell();
                checkedBox.Value = false;
                row.Cells[1] = checkedBox;

            }
            this.table.Rows.Add(row);
        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            if (e.ColumnIndex != 1) return;
            if (table.Rows[e.RowIndex].Cells[1].GetType() == typeof(DataGridViewTextBoxCell))
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
            string name = table.Rows[e.RowIndex].Cells[0].Value.ToString();

            if (table.Rows[e.RowIndex].Cells[1].GetType() == typeof(DataGridViewComboBoxCell))
            {

                UInt16 index = (UInt16)(table.Rows[e.RowIndex].Cells[1] as DataGridViewComboBoxCell).Items.IndexOf
                                   (
                                       table.Rows[e.RowIndex].Cells[1].Value
                                   );
                this.controls[name].Value = index;
               

            } else

            if (table.Rows[e.RowIndex].Cells[1].GetType() == typeof(DataGridViewTextBoxCell))
            {
                double num = 0;
                if (double.TryParse(table.Rows[e.RowIndex].Cells[1].Value.ToString(), out num))
                {
                    this.controls[name].Value = num;
                    if (this.controls[name].Value != num)
                        table.Rows[e.RowIndex].Cells[1].Value = this.controls[name].Value;

                }
                
            } else

            if (table.Rows[e.RowIndex].Cells[1].GetType() == typeof(DataGridViewCheckBoxCell))
            {

                bool res = (bool)(table.Rows[e.RowIndex].Cells[1] as DataGridViewCheckBoxCell).Value;

                this.controls[name].Value = 0;
                if (res) this.controls[name].Value = 1;

                
            }

            this.controls[name].ControlEvetn();

        }
    }




}
