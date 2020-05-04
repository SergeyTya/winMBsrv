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
        List<CheckBox[]>  lsaRbEnbl= new List<CheckBox[]>();
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
            chart1.ChartAreas[0].AxisX.Interval = 0.2;
            chart2.ChartAreas[0].AxisX.LabelStyle.Format = "0.##";
            chart2.ChartAreas[0].AxisX.Interval = 0.2;
            //создание значений по оси х
            iaAxisX = new float[Server.scp_cntmax];
            for (int i = 0; i < Server.scp_cntmax; i++) iaAxisX[i] =(float)(  i*((cmBoxScpFreq.SelectedIndex + 1) * 0.001) );
            // настройка параметров зума
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].CursorX.Interval = 0.005;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            chart1.ChartAreas[0].CursorY.AutoScroll = true;

            chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].CursorX.Interval = 0.005;
            chart2.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart2.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart2.ChartAreas[0].CursorX.AutoScroll = true;
            chart2.ChartAreas[0].CursorY.AutoScroll = true;
            //сброс графиков
            ResetAxis();
            //
            tbaGain    = new TextBox[] { textBoxGainCh1,textBoxGainCh2,textBoxGainCh3,textBoxGainCh4 };
            tbaOffset  = new TextBox[] { textBoxOffCh1, textBoxOffCh2, textBoxOffCh3, textBoxOffCh4 };
            tbaAdr     = new TextBox[] { textBoxAdrCh1, textBoxAdrCh2, textBoxAdrCh3, textBoxAdrCh4 };
            tbaAdrSh   = new TextBox[] { textBoxAdrShCh1, textBoxAdrShCh2, textBoxAdrShCh3, textBoxAdrShCh4 };
            btaAdrSend = new Button[]  { btnSendAdrCh1, btnSendAdrCh2, btnSendAdrCh3, btnSendAdrCh4 };
     
            //массив чекбоксов для отображения каналов
            lsaRbEnbl.Add(new CheckBox[] { checkBox1, checkBox2, checkBox3, checkBox4 });
            lsaRbEnbl.Add(new CheckBox[] { checkBox5, checkBox6, checkBox7, checkBox8 });
            //все включены
            foreach (CheckBox[] el in lsaRbEnbl) { for (int i = 0; i < el.Length; i++) el[i].Checked = true; };

            for (int i = 0; i < 4; i++) { tbaGain[i].Text = "1"; tbaOffset[i].Text = "0"; };


            chart1.MouseWheel += new MouseEventHandler(chData_MouseWheel);
            chart2.MouseWheel += new MouseEventHandler(chData_MouseWheel);
            Server.blnScpEnbl = true;
        }



        public void UpdateCharts(double [] point, int num)
        {

            if (blnFrmOpen) chart1.Series[0].Points.AddXY(point[0], point[1]);
        }

        //Обновление графиков. Функция выполняется в основном потоке вызванном из  главной формы
        public void UpdateCharts(List<double[]> value, int num)
        {
            
            for (int i = 0; i < 4; i++)
            {

                if (blnFrmOpen) // если форма отображается
                {
                    if (i < num) // количество доступных каналов
                    {
                        if (iaAxisX.Length == value[i].Length) //проверяем что количество значений по осям равно
                        {

                            if (!checkBoxPause1.Checked) chart1.Series[i].Points.DataBindXY(iaAxisX, value[i]);
                            if (lsaRbEnbl[0][i].Checked == true) { chart1.Series[i].Enabled = true; } else { chart1.Series[i].Enabled = false; };
                            if (!checkBoxPause2.Checked) chart2.Series[i].Points.DataBindXY(iaAxisX, value[i]);
                            if (lsaRbEnbl[1][i].Checked == true) { chart2.Series[i].Enabled = true; } else { chart2.Series[i].Enabled = false; };


                        }
                        else
                        {
                            Debug.WriteLine("Length x="+iaAxisX.Length + " Length y=" + value[i].Length);
                            return;
                        }
                    }
                    else
                    {
                        //недоступные каналы не отображаются
                        chart1.Series[i].Enabled = false; 
                        chart2.Series[i].Enabled = false;
                    }

                }
               
            }
            

            //double[] tempD = new double[256];
            //double[] temp = new double[256];

            //if (value[0].Length==360)
            //{
            //    for (int i = 0; i < 256; i++) tempD[i] = value[0][i]; 
            //    FFTLibrary.Complex.FFT(1, 8, tempD, temp);
            //    chart2.ChartAreas[0].AxisX.Maximum = 128;
            //    chart2.ChartAreas[0].AxisX.Interval = 10;
            //    chart2.Series[0].Points.DataBindY(tempD);
            //}



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

        //Сброс графиков
        private void ResetAxis()
        {
            //новое количество точек в графике для сервера 
            Server.scp_cntmax = 120 * (comboBoxPageNum.SelectedIndex + 1);
            //запрос на сброс точек на сервере
            Server.blnScpRstreq = true;
            int i=0;
            while (Server.blnScpRstreq)
            {
                Thread.Sleep(1);
                i++;
                if (i > 1000) return;
                if (!Server.blnScpEnbl) return;
            }

            //обновляем пределы по оси х
           // chart1.ChartAreas[0].AxisX.Maximum = Server.scp_cntmax;

           chart2.ChartAreas[0].AxisX.Maximum = 120 * (comboBoxPageNum.SelectedIndex + 1) * (cmBoxScpFreq.SelectedIndex + 1) * 0.001*Server.scope_ADC_div;
           chart1.ChartAreas[0].AxisX.Maximum = 120 * (comboBoxPageNum.SelectedIndex + 1) * (cmBoxScpFreq.SelectedIndex + 1) * 0.001*Server.scope_ADC_div;
          //обновляем значения по оси х
             iaAxisX = new float[Server.scp_cntmax];
             if (iaAxisX != null) for (i = 0; i < Server.scp_cntmax; i++) iaAxisX[i] = (float)(i * ((cmBoxScpFreq.SelectedIndex + 1) * 0.001) * Server.scope_ADC_div);

            //обновляем шаг сетки по х
            chart1.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            chart1.ChartAreas[0].AxisX.Interval = chart1.ChartAreas[0].AxisX.Maximum / 10;
            chart2.ChartAreas[0].AxisX.Interval = chart2.ChartAreas[0].AxisX.Maximum / 10;

            //  Debug.WriteLine("Новый размер " + Server.scp_cntmax);

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

            ResetAxis();

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
            ResetAxis();
        }
        private void btnClear1_Click(object sender, EventArgs e)
        {
            ResetAxis();
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
               Server.daGain[Array.IndexOf(tbaGain, temp)] = Convert.ToDouble(temp.Text);
            }
            catch (Exception e2) { Debug.WriteLine(e2); return; };

        }

        private void textBoxOffCh1_TextChanged(object sender, EventArgs e)
        {
            TextBox temp = (TextBox)sender;
            try
            {
                Server.daOffset[Array.IndexOf(tbaOffset, temp)] = Convert.ToDouble(temp.Text);
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
            ResetAxis();


        }
    }
}
