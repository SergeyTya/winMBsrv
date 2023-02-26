using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestGen
{

    internal class myControl
    {

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

        public myControl(string name, string[] range = null, double max = 32768, double min = -32768, double def = 0, bool checkedbox = false)
        {
            Name = name;
            Range = range;
            Max = max;
            Min = min;
            Value = def;
            Checkedbox = checkedbox;
        }

    }

    internal static class ParamNames
    {
        public static string Enable = "Выход Разрешен";
        public static string Type = "Тип сигнала";
        public static string Target = "РХ задание";
        public static string ResEnable = "Выводить отклик";
        public static string Responce = "РВ отклик";
        public static string Scale = "Развертка, с";
        public static string Period = "Период, с";
        public static string Amplitude = "Aмплитуда";
        public static string Offset = "Смещение";
        public static string Timestep = "TimeStep, с";

    }

    internal class ControlsTable
    {

        public Dictionary<String, myControl> controls;
        private DataGridView table;

        public ControlsTable(DataGridView table)
        {

            this.table = table;
            controls = new Dictionary<String, myControl>();

            controls.Add(ParamNames.Enable, new myControl(ParamNames.Enable, checkedbox: true, max: 1, min: 0));
            controls.Add(ParamNames.ResEnable, new myControl(ParamNames.ResEnable, checkedbox: true, max: 1, min: 0));
            controls.Add(ParamNames.Target, new myControl(ParamNames.Target, def: 20));
            controls.Add(ParamNames.Responce, new myControl(ParamNames.Responce, def: 12));
            controls.Add(ParamNames.Scale, new myControl(ParamNames.Scale, min: 0.1, def: 10));
            controls.Add(ParamNames.Type, new myControl(ParamNames.Type, range: new string[3] { "Синус", "Меандр", "Треугольник" }, max: 2, min: 0));
            controls.Add(ParamNames.Period, new myControl(ParamNames.Period, min: 0.1, def: 15));
            controls.Add(ParamNames.Amplitude, new myControl(ParamNames.Amplitude, min: 1, def: 3000));
            controls.Add(ParamNames.Offset, new myControl(ParamNames.Offset, min: 0));
            controls.Add(ParamNames.Timestep, new myControl(ParamNames.Timestep, min: 0.01, max: 0.5, def: 0.100));

            foreach (String name in controls.Keys)
            {
                AddGridRow(controls[name]);
            }

        }

        private void AddGridRow(myControl control)
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
    }




}
