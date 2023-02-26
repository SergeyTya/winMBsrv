using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static TestGen.FormGensig2;
using TestGen;

namespace WindowsFormsApp4
{
    public partial class FormScope2 : Form
    {

        private ControlsTable controlTable1;
        private ControlsTable controlTable2;

        private static class ScopeParamNames
        {
            public static string SampleRate = "Дискретность";
            public static string Oversampling = "Делитель";
            public static string ChanelsNum = "Кол-во каналов";
            public static string TimeScale = "Разветка";
            public static string Ch1_gain = "Множитель K1";
            public static string Ch1_shift = "Смещение K1";
            public static string Ch2_gain = "Множитель K2";
            public static string Ch2_shift = "Смещение K2";
            public static string Ch3_gain = "Множитель K3";
            public static string Ch3_shift = "Смещение K3";
            public static string Ch4_gain = "Множитель K4";
            public static string Ch4_shift = "Смещение K4";

        }

        public FormScope2()
        {
            InitializeComponent();
            CreateControlTable1();
            controlTable2 = new ControlsTable(this.dataGridView2);
        }

        private void CreateControlTable1() {


            controlTable1 = new ControlsTable(this.dataGridView1);
            controlTable1.addControl(new CustomControl(
                ScopeParamNames.SampleRate,
                range: new string[] { "100", "200", "300", "500", "700", "1000" },
                max: 5,
                min: 0
            ));

            controlTable1.addControl(new CustomControl(
                ScopeParamNames.Oversampling,
                range: new string[] { "1", "2", "4", "8", "10" },
                max: 4,
                min: 0
            ));

            controlTable1.addControl(new CustomControl(
                ScopeParamNames.ChanelsNum,
                def: 4,
                max: 4,
                min: 1
            ));

            controlTable1.RenderTable();


        }

        private void FormScope2_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
