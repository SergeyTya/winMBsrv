using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Threading;

namespace WindowsFormsApp4
{
    public partial class SetupForm : Form
    {
        public MODBUS_srv Server = null;

        public SetupForm(MODBUS_srv value)
        {
            this.Text = "Выбор мотора" ;
            // Server = value;
            InitializeComponent();
            // initiate DB connection
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source = motors.mdb";
            try
            {
                OleDbConnection database = new OleDbConnection(connectionString);
                database.Open();
                //SQL query to list movies
                // string sqlQueryString = "SELECT Имя, Тип, Мощность, Частота, Ток, КПД FROM Motors";
                string sqlQueryString = "SELECT * FROM Motors";

                OleDbCommand SQLQuery = new OleDbCommand();
                DataTable data = null;
                dataGridView1.DataSource = null;
                SQLQuery.Connection = null;
                OleDbDataAdapter dataAdapter = null;
                dataGridView1.Columns.Clear(); // <-- clear 

                SQLQuery.CommandText = sqlQueryString;
                SQLQuery.Connection = database;
                data = new DataTable();
                dataAdapter = new OleDbDataAdapter(SQLQuery);
                dataAdapter.Fill(data);
                dataGridView1.DataSource = data;

                Server = value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }



        }


        private void SetupForm_Load(object sender, EventArgs e)
        {
            
            
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Selected = false;
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    if (dataGridView1.Rows[i].Cells[j].Value != null)
                        if (dataGridView1.Rows[i].Cells[0].Value.ToString().Contains(textBox1.Text))
                        {
                            dataGridView1.Rows[i].Selected = true;
                            break;
                        }
            }
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                index_selected();
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            index_selected();

        }

        void index_selected()
        {
            if (dataGridView1.CurrentRow.Cells[1].Value == null) { Close(); return; };

           List<UInt16[]> temp = new List<ushort[]>();
            int adr = 0;
            for (int i = 0; i < 15; i++)
            {
                double value = 0;
                try { value  = Convert.ToDouble(dataGridView1.CurrentRow.Cells[i+9].Value.ToString()); }
                catch (Exception ex) { return; };



                
                adr = i  + 29;
                if (i < 5) value *= 10000;
                if (i > 5) adr = i + 25 - 6;
                if(i>10) adr = i + 48 - 11;

                if (i == 10) adr = 24;

                temp.Add(new UInt16[] { (UInt16) adr, (UInt16)value });
            }

            if (dataGridView1.CurrentRow.Cells[2].Value.ToString() == "ASI") { Server.uialHRForWrite.Add(new UInt16[2] { 11, 3 }); };
            if (dataGridView1.CurrentRow.Cells[2].Value.ToString() == "SYN") { Server.uialHRForWrite.Add(new UInt16[2] { 11, 2 }); };


            foreach(UInt16[] el in temp) Server.uialHRForWrite.Add(el);

            Form mes = new Form();
            Label lb = new Label();
            mes.ClientSize = new System.Drawing.Size(100, 50);
            mes.FormBorderStyle = FormBorderStyle.None;
            lb.Size = new System.Drawing.Size(100, 50);
            lb.TabIndex = 5;
            lb.Text = "Пишу параметры";
            lb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            mes.Controls.Add(lb);
            mes.Show();
            Application.DoEvents();

            int k = 0;
            while (Server.uialHRForWrite.Count != 0) { Thread.Sleep(1); k++; if (k > 5000) break; };

            mes.Close();
            this.Close();


        }


    }
}
