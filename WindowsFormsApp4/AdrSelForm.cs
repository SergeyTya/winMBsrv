using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class AdrSelForm : Form
    {
        TextBox TextBox;

        public AdrSelForm(TextBox tbContent, string filename)
        {
            InitializeComponent();
            int i = 0;
            try
            {
                bool GetSection = false;
                string str = "";

                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                while ((str = sr.ReadLine()) != null)
                {
                    if (str == "Entry                      Address    Size  Type      Object") GetSection = true;

                    if (GetSection)
                    {
                        if (str == "") break;
                        String[] words = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                      
                        if (words.Length>2) if (words[3] == "Data") dataGridView1.Rows.Add(new String[] { words[0], words[1], words[2], words[5] });
                        dataGridView1.ReadOnly = true;


                        //  Debug.WriteLine(str);
                    };

                    i++;

                }
                sr.Close();
                fs.Close();
                TextBox = tbContent;
                this.Text = filename;
            }
            catch (Exception ex) { Debug.WriteLine("Eror in line " + i); };

        }

        private void GridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
            this.Close();
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

            string str  = dataGridView1.CurrentRow.Cells[1].Value.ToString() ;
            if (str == null) return;
            try
            {
                Convert.ToInt32(str.Substring(2),16);
            } catch (Exception ex) {
                Debug.WriteLine("convert error");
                return;
            };
            TextBox.Text = str.Substring(2);
            Close();
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
