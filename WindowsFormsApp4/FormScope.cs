using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp4
{
    public partial class FormScope : Form
    {

        private bool blnFrmOpen = false;
        public MODBUS_srv Server = null;
        List<CheckBox>  lsaRbEnbl2= new List<CheckBox>();
        List<CheckBox>  lsaRbEnbl1 = new List<CheckBox>();
        TextBox[] tbaGain = new TextBox[] {};
        TextBox[] tbaOffset = new TextBox[] { };
        TextBox[] tbaAdr = new TextBox[] { };
        TextBox[] tbaAdrSh = new TextBox[] { };
        Button[]  btaAdrSend = new Button[] { };
        public AdrSelForm AdrForm = null;

        float[] iaAxisX = null;
        string filename = null;


        public FormScope(MODBUS_srv value)
        {
            InitializeComponent();
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = "Осциллограф " + version;

            Server = value;
            //Индексы комбо бокс при загрузке
            cmbBoxScpChNum.SelectedIndex = 0;
            cmBoxScpFreq.SelectedIndex = 2;
            comboBoxPageNum.SelectedIndex = 0;
        
         
            //цвета каналов
            chart1.Series[0].Color = Color.Red;
            chart1.Series[1].Color = Color.Blue;
            chart1.Series[2].Color = Color.Black;
            chart1.Series[3].Color = Color.Green;
            chart2.Series[0].Color = Color.Red;
            chart2.Series[1].Color = Color.Blue;
            chart2.Series[2].Color = Color.Black;
            chart2.Series[3].Color = Color.Green;
            //формат подписей по оси х
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "0.##";
           // chart1.ChartAreas[0].AxisX.Interval = 100;
            chart2.ChartAreas[0].AxisX.LabelStyle.Format = "0.##";
           // chart2.ChartAreas[0].AxisX.Interval = 100;
            //создание значений по оси х
            //iaAxisX = new float[Server.scp_cntmax];
            //for (int i = 0; i < Server.scp_cntmax; i++) iaAxisX[i] =(float)(  i*((cmBoxScpFreq.SelectedIndex + 1) * 0.001) );
            // настройка параметров зума
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].CursorX.Interval = 100;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            chart1.ChartAreas[0].CursorY.AutoScroll = true;

            chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].CursorX.Interval = 1;
            chart2.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart2.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart2.ChartAreas[0].CursorX.AutoScroll = true;
            chart2.ChartAreas[0].CursorY.AutoScroll = true;
           
            //
            tbaGain    = new TextBox[] { textBoxGainCh1,textBoxGainCh2,textBoxGainCh3,textBoxGainCh4 };
            tbaOffset  = new TextBox[] { textBoxOffCh1, textBoxOffCh2, textBoxOffCh3, textBoxOffCh4 };
            tbaAdr     = new TextBox[] { textBoxAdrCh1, textBoxAdrCh2, textBoxAdrCh3, textBoxAdrCh4 };
            tbaAdrSh   = new TextBox[] { textBoxAdrShCh1, textBoxAdrShCh2, textBoxAdrShCh3, textBoxAdrShCh4 };
            btaAdrSend = new Button[]  { btnSendAdrCh1, btnSendAdrCh2, btnSendAdrCh3, btnSendAdrCh4 };
     
            //массив чекбоксов для отображения каналов
            lsaRbEnbl1.AddRange(new List<CheckBox> { checkBox1, checkBox2, checkBox3, checkBox4 });
            lsaRbEnbl2.AddRange(new List<CheckBox> { checkBox5, checkBox6, checkBox7, checkBox8 });
            //все включены
            foreach (CheckBox el in lsaRbEnbl1) {el.Checked = true; };
            foreach (CheckBox el in lsaRbEnbl2) { el.Checked = true; };

            for (int i = 0; i < 4; i++) { tbaGain[i].Text = "1"; tbaOffset[i].Text = "0"; };


            chart1.MouseWheel += new MouseEventHandler(chData_MouseWheel);
            chart2.MouseWheel += new MouseEventHandler(chData_MouseWheel);
            Server.blnScpEnbl = true;

            UpdateChartsAsync();


        }


        //Обновление графиков.
        private async void UpdateChartsAsync()
        {

            while (true)
            {
                if (blnFrmOpen)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (Server.circbuf.GetChlDataCount(i) > 0)
                        {
                            chart1.Series[i].Enabled = lsaRbEnbl1[i].Checked;
                            chart2.Series[i].Enabled = lsaRbEnbl2[i].Checked;

                            double offset = 0; double gain = 1;

                            try
                            {
                                offset = Convert.ToDouble(tbaOffset[i].Text);
                                gain = Convert.ToDouble(tbaGain[i].Text);
                            }
                            catch (Exception e) { };

                            if (!checkBoxPause1.Checked) chart1.Series[i].Points.DataBindXY(Server.circbuf.Time,
                               Server.circbuf.GetChnlData(i).Select(x=> x * gain).Select(x=> x + offset).ToList()
                               );
                           if(!checkBoxPause2.Checked) chart2.Series[i].Points.DataBindXY(Server.circbuf.Time,
                               Server.circbuf.GetChnlData(i).Select(x => x * gain).Select(x => x + offset).ToList()
                               ); 

                        }
                        else
                        {
                            chart1.Series[i].Points.Clear();
                            chart2.Series[i].Points.Clear();
                        };

                    }

                    //chart2.ChartAreas[0].AxisX.Interval = Server.circbuf.TimeStep * 100;
                    //chart2.ChartAreas[0].AxisX.Maximum = Server.circbuf.TimeStep * Server.circbuf.BufferSize;
                }

                await Task.Delay(500);

            }
            

        }

        private void FormScope_Shown(object sender, EventArgs e)
        {
            blnFrmOpen = true;
        }

        private void FormScope_FormClosing(object sender, FormClosingEventArgs e)
        {
            blnFrmOpen = false;
            Server.blnScpEnbl = false;
        }

     
        //Прокрутка колесиком
        private void chData_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0)
                {
                    ((Chart )sender).ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    ((Chart )sender).ChartAreas[0].AxisY.ScaleView.ZoomReset();
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


        //изменение дискретности и количества каналов на устройстве
        private void cmBoxScpFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ФЗначение для регистра управления 
            int temp = (cmbBoxScpChNum.SelectedIndex) + (cmBoxScpFreq.SelectedIndex << 2); 
            //добавляем в очередь
            Server.uialHRForWrite.Add(new UInt16[2] { 35, (UInt16)temp });

        }


        private void btnAutoScale1_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
        }
        private void btnAutoScale2_Click(object sender, EventArgs e)
        {
            chart2.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            chart2.ChartAreas[0].AxisY.ScaleView.ZoomReset();
        }
        private void comboBoxPageNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            double time = comboBoxPageNum.SelectedIndex + 1;
            Server.circbuf.SetSweepTime(time);

        }


        private void btnClear1_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
        
            char number = e.KeyChar;
            if(!Char.IsDigit(number) && number != 8 && number != 44 && number != 0x2d)
            {
                e.Handled = true;
            }
        }

        private void textBoxGainCh1_TextChanged(object sender, EventArgs e)
        {
            TextBox temp = (TextBox)sender;
            try
            {
              // Server.daGain[Array.IndexOf(tbaGain, temp)] = Convert.ToDouble(temp.Text);
            }
            catch (Exception e2) { Debug.WriteLine(e2); return; };

        }

        private void textBoxOffCh1_TextChanged(object sender, EventArgs e)
        {
            TextBox temp = (TextBox)sender;
            try
            {
               // Server.daOffset[Array.IndexOf(tbaOffset, temp)] = Convert.ToDouble(temp.Text);
            }
            catch (Exception e2) { Debug.WriteLine(e2); return; };

        }

        private void textBoxAdrCh1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ( (e.KeyChar < 65 || e.KeyChar >70 ) && !Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }


        private void btnSendAdrCh1_Click(object sender, EventArgs e)
        {
            Button temp = (Button)sender;
            int num = 0 ;
            int ch = 0;
            try
            {
                ch = Array.IndexOf(btaAdrSend, (Button)sender);
                num = Convert.ToInt32( tbaAdr[ch] .Text, 16  )+ Convert.ToInt32(tbaAdrSh[ch].Text, 16);
            }
            catch (Exception ex) { return; };

            Server.uialHRForWrite.Add(new UInt16[2] { 37, (UInt16)num });
            Server.uialHRForWrite.Add(new UInt16[2] { 38, (UInt16)(num >> 16) });
            Server.uialHRForWrite.Add(new UInt16[2] { 35, (UInt16)(((ch + 1) << 10) + Server.uiHoldingReg[35]) });

        }

        private void textBoxAdrCh1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (filename == null)
            {
                OpenFileDialog openFileDialog2 = new OpenFileDialog();
                openFileDialog2.Filter = "|*.map";
                if (openFileDialog2.ShowDialog() == DialogResult.Cancel)
                    return;
                // получаем выбранный файл
                filename = openFileDialog2.FileName;
            }

            AdrForm = new AdrSelForm((TextBox) sender, filename);
            AdrForm.Show();

        }


        private void textBoxADCdiv_TextChanged(object sender, EventArgs e)
        {

            int temp = 0;
            Server.scope_ADC_div = 1;
            try{ temp = Convert.ToInt32(textBoxADCdiv.Text, 10);}catch (Exception ex) { return; };
            if (temp == 0) { textBoxADCdiv.Text = "1"; } else { Server.scope_ADC_div = temp; };

        }

        private async void butGetPreChnl_Click(object sender, EventArgs e)
        {
            dataGridPreChnls.Rows.Clear();

            Server.blnScpGetPreChRequest = true;
            while (Server.blnScpGetPreChRequest) await Task.Delay(100);
           
            foreach (var item in Server.circbuf.target_chnl)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.HeaderCell.Value = "Row";
                row.CreateCells(dataGridPreChnls);
                row.SetValues(new object[] { "", item._info, "0x"+Convert.ToString(item._adr,16), item._type_name});

                DataGridViewCheckBoxCell checkedBox = new DataGridViewCheckBoxCell();
                

                checkedBox.Value = false;
                row.Cells[0] = checkedBox;
                dataGridPreChnls.Rows.Add(row);
            };

        }


        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == 0) if (dataGridPreChnls.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {

          
                    while (Server.ScopeChnToRead.Count() != 4) Server.ScopeChnToRead.Add(0);

                    bool status = (bool) (dataGridPreChnls.Rows[e.RowIndex].Cells[0] as DataGridViewCheckBoxCell).Value;
                    UInt32 adr = Convert.ToUInt32( ( (string) dataGridPreChnls.Rows[e.RowIndex].Cells[2].Value).Substring(2), 16);
                    int adr_index = Server.ScopeChnToRead.IndexOf(adr);
                    int empty_adr = Server.ScopeChnToRead.IndexOf(0);

                    Action setcolor = () =>
                    {

                        foreach (DataGridViewRow row in dataGridPreChnls.Rows)
                        {
                            string tsadr = (string)row.Cells[2].Value;
                            if (tsadr != null)
                            {
                                UInt32 tadr = Convert.ToUInt32((tsadr).Substring(2), 16);
                                int ind = Server.ScopeChnToRead.IndexOf(tadr);
                                switch (ind)
                                {
                                    case 0: row.Cells[0].Style.BackColor = Color.Red; break;
                                    case 1: row.Cells[0].Style.BackColor = Color.Blue; break;
                                    case 2: row.Cells[0].Style.BackColor = Color.Black; break;
                                    case 3: row.Cells[0].Style.BackColor = Color.Green; break;

                                    default:
                                        row.Cells[0].Style.BackColor = Color.White;
                                        break;
                                }

                                 (row.Cells[0] as DataGridViewCheckBoxCell).Value = ind !=-1;

                            }

                               
                        }
                           
                    };


                    if (status & adr_index == -1 & empty_adr != -1) // Add to list
                    {
                        Server.ScopeChnToRead[empty_adr] = adr;
                        Debug.WriteLine("Add at " + empty_adr + " value " + adr);
                    }
                    else 

                    if (!status & adr_index != -1) //remove from list
                    {
                        Server.ScopeChnToRead.RemoveAt(adr_index);
                        Debug.WriteLine("Remove at " + adr_index + " value " + adr);   
                    }

                    setcolor();

                    while (Server.ScopeChnToRead.Count() != 4) Server.ScopeChnToRead.Add(0);
                    Server.blnScpSetChRequest = true;

                    textBoxAdrCh1.Text = Convert.ToString(Server.ScopeChnToRead[0], 16);
                    textBoxAdrCh2.Text = Convert.ToString(Server.ScopeChnToRead[1], 16);
                    textBoxAdrCh3.Text = Convert.ToString(Server.ScopeChnToRead[2], 16);
                    textBoxAdrCh4.Text = Convert.ToString(Server.ScopeChnToRead[3], 16);


                    int num = Server.ScopeChnToRead.GroupBy(_ => _ != 0).ToList()[0].Count();
                    if(num<=4 && num >= 1)cmbBoxScpChNum.SelectedIndex = num - 1;



                }
        }

        private void FormScope_Load(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
