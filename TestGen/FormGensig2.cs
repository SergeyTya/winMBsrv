using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestGen
{
    public partial class FormGensig2 : Form
    {
        private List<myControl> controls;
        private ControlsTable   controlTable;
       
        public FormGensig2()
        {
            InitializeComponent();
            controlTable = new ControlsTable(this.dataGridView1);
            dataGridView1.DataSource = controls;
        }
    }
}
