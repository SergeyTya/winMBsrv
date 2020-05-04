#define my_DEBUG


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;


namespace WindowsFormsApp4
{

    public partial class Form1 : Form
    {

        MODBUS_srv Server = new MODBUS_srv();
        Bootloader bloader = null;
        List<TextBox> lstIndIR = new List<TextBox>();
        Thread Updater;
        string[] saRazmForInd = new string[] { " Гц", " В", " В", " В", " A", " o" };
        int[][] iaLevelForInd = new int[][] {
            new int[] {   0,4000}, // freq_out
            new int[] {1000,3900}, // Volt_out
            new int[] {2900,5800}, // BUS
            new int[] {3100,4200}, // GRID
            new int[] {  10,1000}, // CURRENT
            new int[] {  20,  60}, // Temp
        };

        public enum eDev_cmd
        {
            RUN   = 0x1,
            STOP   = 0x2,
            RESET  = 0x4,
            REBOOT = 0x1603,
            SAVEPRM = 0x8,
            LOADPRM = 0x10,
            DEFPRM = 0x12,
            CHKDSBL = 0x2801,
            BOOT = 0x7777,
            FANTST = 0x300,
            ENCTST = 0x1303,
            RDOTST = 0x0016,
            SET1310 = 0x1304
        }

        eDev_cmd[] uaCMD =
        { 00, eDev_cmd.STOP,     eDev_cmd.RUN,     eDev_cmd.RESET,    eDev_cmd.REBOOT,
              eDev_cmd.CHKDSBL,  eDev_cmd.SAVEPRM, eDev_cmd.LOADPRM,  eDev_cmd.DEFPRM,
              eDev_cmd.FANTST,   eDev_cmd.ENCTST,  eDev_cmd.BOOT, eDev_cmd.RDOTST, eDev_cmd.SET1310
        };

        List<string> slErrMes = new List<string>();
        List<string> slPrmNam = new List<string>();
        List<UInt16> slPrmToSaveRead = new List<UInt16>();

        public FormScope ScopeForm = null;
        SetupForm SetupForm1 = null;
   

        private string sTempForCell = null;
        UInt16 chart_cnt = 0;
        UInt16 chart_max = 1000;

        UInt16 uiServerDelay=50;

        public delegate void MyDelegate();



        public Form1()
        {



            InitializeComponent();

            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = "МПЧ Сервер " + version;

            lstIndIR.AddRange(new TextBox[] { tbFREQ_out, tbVOLT_out, tbDC, tbGRID, tbCUR_out, tbTemp });

            cmbBoxBaudRate.Items.AddRange(new string[] { "9600", "38400", "115200", "128000", "230400" , "406000"});
            cmbBoxBaudRate.SelectedIndex = 2;
            cmbBoxPortList.Items.Clear();
            cmbBoxPortList.Items.Add("COM7");
            cmbBoxPortList.SelectedIndex = 0;
            string[] ports = SerialPort.GetPortNames();

            cmbBoxPortList.Items.AddRange(ports);
            vIndi_Clear();
            // читаю файл с именами ошибок
            try{
                FileStream fs = new FileStream("ERR.mpch", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    slErrMes.Add(str);
                }
                sr.Close();
                fs.Close();
            } catch (Exception ex) { };
            // читаю файл с именами параметров
            try{
                string str = "";
                FileStream fs = new FileStream("PRM.mpch", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                while ((str = sr.ReadLine()) != null)
                {
                    slPrmNam.Add(str);
                }
                sr.Close();
                fs.Close();
            }catch (Exception ex) { };
            //Заполняю таблицу RIO
            for (int i = 0; i < 8; i++)
            {

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(gridRelayIO);
                row.SetValues(new object[] { "Выход "+i.ToString(), 0, "Вход "+ i.ToString(), 0 });
                this.gridRelayIO.Rows.Add(row);

            }

            Updater = new Thread(updater);
            Updater.IsBackground = true;

            btnCnct.Text = "Соединить";

//Контекстное меню графика

            ToolStripMenuItem clearMenuItem = new ToolStripMenuItem("Очистить");
            contextMenuForChart.Items.AddRange(new[] { clearMenuItem });
            chart1.ContextMenuStrip = contextMenuForChart;
            clearMenuItem.Click += clearChartItem_Click;
            txtBoxModbusAdr.Text = "1";

//Контекстное меню 

            // Графики
            cmbBoxChart1Series.SelectedIndex = 4;
            cmbBoxChart2Series.SelectedIndex = 1;
            chart1.ChartAreas[0].AxisX.Maximum = chart_max;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Interval = chart_max/10;
            chart1.ChartAreas[0].AxisX.LineColor = Color.Black;
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray;
            chart1.ChartAreas[0].AxisY.LineColor = Color.Black;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gray;
            chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;

            chart2.ChartAreas[0].AxisX.Maximum = chart_max;
            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.Interval = chart_max/10;
            chart2.ChartAreas[0].AxisX.LineColor = Color.Black;
            chart2.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray;
            chart2.ChartAreas[0].AxisY.LineColor = Color.Black;
            chart2.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gray;
            chart2.ChartAreas[0].AxisY.IsStartedFromZero = false;

 

            foreach (Series el in chart1.Series)
            {
                el.BorderWidth = 3;
                el.BorderColor = Color.DarkBlue;
            }

            foreach (Series el in chart2.Series)
            {
                el.BorderWidth = 3;
                el.BorderColor = Color.DarkBlue;
            }

            cmbBoxServerDelay.SelectedIndex = 3;

        }
        //основной поток
        private void updater()
        {

            while (true)
            {
                
                if (Server.spPort.IsOpen)
                {
                    try
                    {
                        BeginInvoke(new MyDelegate(vIndi_Update));
                    }
                    catch (System.InvalidOperationException e) { };
               //     Application.DoEvents();

                    if (Server.suspend) { Thread.Sleep(1000);  continue; }

                    if (bloader != null) bloader = null;

                    if (Server.iFail > 10) btn_Cnct_Click(this, null);
                    if (Server.iFail < 10) Server.blReadIRreq = true;

                    if (!Server.blDevCnctd) { Server.vConnectToDev(); Server.iFail++; continue; }

                    Server.vPoll();
                    
                }else
                {
                    BeginInvoke(new MyDelegate(vIndi_Clear));
                    Server.vReset();
                    bloader = null;
                }

                Thread.Sleep(uiServerDelay);

            }
        }

        private void vSetComandForDev(eDev_cmd cmd)
        {
            if (Server.blDevCnctd)
            {
                Server.uialHRForWrite.Add(new UInt16[2] { 0, (UInt16)cmd });
                gridHRTable.Rows[0].Cells[2].Value = (UInt16)cmd;
                gridHRTable.Rows[0].Cells[2].Style.BackColor = Color.Red;
            }
        }
        // Обновление цвета индикатора
        private void vIndi_SetColor(TextBox tb, int mes, int[] Level)
        {
            tb.ForeColor = Color.Cyan;
            if (mes < Level[0]) tb.ForeColor = Color.LightGreen;
            if (mes > Level[1]) tb.ForeColor = Color.LightCoral;
        }
        // Обновление индикаторов
        private void vIndi_Update()
        {
            int i = 0;


            // пишу сообщения в лог 
            if (Server.logger.Count != 0) // Print Log 
            {
                txtBoxLog.AppendText(Environment.NewLine + "SV: " + Server.logger[0]);
                Server.logger.RemoveAt(0);
            };

            if (bloader != null) while (bloader.logger.Count != 0) // Print Log 
                {
                    
                    if (bloader.logger[0].Length>1)
                    {
                        var lines = txtBoxLog.Lines.ToList();
                        lines.RemoveAt(lines.Count - 1);
                        txtBoxLog.Lines = lines.ToArray();
                    }
                    txtBoxLog.AppendText(Environment.NewLine + "BL: " + bloader.logger[0][0]);
                    bloader.logger.RemoveAt(0);
                };

            if (txtBoxLog.Lines.Count() > 20) txtBoxLog.Clear();

            if (Server.suspend) return;

            //  Debug.WriteLine(Server.uiInputReg[1].ToString());
            //кнопка коннект
            if (Server.spPort.IsOpen) { btnCnct.Text = "Отключить"; } else { btnCnct.Text = "Соединить"; };
            //проверяем актуальность размера таблиц
            if (gridHRTable.Rows.Count < Server.uiInputReg[1]) vIndi_HRGrid_init(Server.uiInputReg[1]);

            //обновляю индикаторы

            //  foreach (TextBox element in lstIndIR) //update indicators
            i = 0; while(i<100)
            {
                if ((i+3) == Server.uiInputReg.Length) break;
                if (i == 12) break;

                Int16 mes = (Int16)Server.uiInputReg[i + 3];
                float temp = ((float)mes * 10 / 100);
                if (i < lstIndIR.Count) {
                    lstIndIR[i].Text = temp.ToString("#####0.0") + saRazmForInd[i];
                    vIndi_SetColor(lstIndIR[i], mes, iaLevelForInd[i]);
                }

                chart1.Series[i].Points.Add(temp);
                chart2.Series[i].Points.Add(temp);

                if (chart1.Series[i].Points.Count > chart_max) chart1.Series[i].Points.Clear();
                if (chart2.Series[i].Points.Count > chart_max) chart2.Series[i].Points.Clear();

                i++;
            }

            //Осциллограф
            if (ScopeForm!= null)
            {
                if (Server.blnScpDataRdy)
                {
                    ScopeForm.UpdateCharts(Server.uialScope, Server.iScpChNum);
                    Server.blnScpDataRdy = false;
                  
                };
                if (!Server.blnScpEnbl) ScopeForm.Close();
            }

            //обновляю таблицу RIO
            if (tabForm.SelectedTab.Name == "tabPage3")
            {

                i = 0;
                gridRelayIO.ClearSelection();
                foreach (DataGridViewRow row in gridRelayIO.Rows)
                {
                    row.Cells[1].Value = 0;
                    row.Cells[3].Value = 0;
                    row.Cells[1].Style.BackColor = Color.White;
                    row.Cells[3].Style.BackColor = Color.White;
                    if ((Server.uiInputReg[9] & (1 << i + 8)) > 0)
                    {
                        row.Cells[1].Value = 1;
                        row.Cells[1].Style.BackColor = Color.LightGreen;
                    };
                    if ((Server.uiInputReg[9] & (1 << i + 0)) > 0)
                    {
                        row.Cells[3].Value = 1;
                        row.Cells[3].Style.BackColor = Color.LightCoral;
                    };

                    i++;
                }
            }

            //обновляю таблицу параметров
            i = 0;
            if (Server.blUpdGridHR)
            {
                foreach (DataGridViewRow row in gridHRTable.Rows)
                {

                    if (row.Cells[2].Value != null)
                    {
                        //  if (Convert.ToUInt16(row.Cells[2].Value.ToString()) != Server.uiHoldingReg[i]) //--------------------------------------------------------
                        if (Convert.ToInt32(row.Cells[2].Value.ToString()) != Server.uiHoldingReg[i])
                        {
                            row.Cells[2].Style.BackColor = Color.Yellow;
                        }
                        else
                        {
                            if (row.Cells[2].Style.BackColor == Color.Red)
                            {
                                row.Cells[2].Style.BackColor = Color.LightGreen;
                                
                            }
                            else
                            {
                                row.Cells[2].Style.BackColor = Color.White;
                            };

                            

                        }

                        // предел по частоте
                        trackBar1.Maximum = Server.uiHoldingReg[5];
                        if (i == 3)
                        {
                            if (Server.uiHoldingReg[i] > trackBar1.Maximum) { trackBar1.Value = trackBar1.Maximum; }
                            else
                            if (Server.uiHoldingReg[i] < trackBar1.Minimum) { trackBar1.Value = trackBar1.Minimum; } else
                            { trackBar1.Value = Server.uiHoldingReg[i]; }

                           
                        };
                    }
                    // задание отрицательных значений
                    if ( (Convert.ToInt32(row.Cells[2].Value.ToString()) < 0) && (Server.uiHoldingReg[i]> 0X7FFF) )
                    {
                        if (Convert.ToInt32(row.Cells[2].Value.ToString()) == ((Int32) Server.uiHoldingReg[i] - 65536))
                            {
                            row.Cells[2].Style.BackColor = Color.LightGreen;
                        }
                        else {
                            row.Cells[2].Value = Server.uiHoldingReg[i];
                        } 
                    }
                    else
                    {

                        row.Cells[2].Value = Server.uiHoldingReg[i];
                    }
               
                   
                    if ((row.Cells[3] as DataGridViewComboBoxCell) != null)
                    {
                        if (Server.uiHoldingReg[i] < (row.Cells[3] as DataGridViewComboBoxCell).Items.Count)
                            if (i != 0)
                                row.Cells[3].Value = (row.Cells[3] as DataGridViewComboBoxCell).Items[Server.uiHoldingReg[i]];
                    }

                    

                    i++;
                }
                Server.blUpdGridHR = false;
            }

            // пределы по току
            iaLevelForInd[4][0] = (int)((float)Server.uiHoldingReg[8] * 0.10); 
            iaLevelForInd[4][1] = (int)((float)Server.uiHoldingReg[8] * 0.95);

            // обновляю поле статуса
            // tbState.Text = "0x0" + Convert.ToString(Server.uiInputReg[2], 16) + Environment.NewLine;

            switch (Server.uiInputReg[2] & 0xFF00)
            {
                case 0x0:
                    tbState.ForeColor = Color.Blue; tbState.Text = "Нет связи ";
                    break;
                case 0x100:
                    tbState.ForeColor = Color.YellowGreen; tbState.Text = "Ожидание - ";
                    break;
                case 0x200:
                    tbState.ForeColor = Color.Blue; tbState.Text = "Готов - ";
                    break;
                case 0x300:
                    tbState.ForeColor = Color.Green; tbState.Text = "Работа - ";
                    break;
                case 0x400:
                    tbState.ForeColor = Color.Red; tbState.Text = "Авария - ";
                    break;
                default:
                    tbState.Text = " ";
                    break;

            };

            // сообщение берется из предварительно загруженного списка 
            if (slErrMes.Count > (Server.uiInputReg[2] & 0xFF)) tbState.Text += slErrMes[Server.uiInputReg[2] & 0xFF];

            // обновляю статус бар
            tsStatus.Text = "Соединен с [" + Server.strDevID + "] статус [0x0" + Convert.ToString(Server.uiInputReg[2],16)+ "]. Ошибок связи " + Server.iFail.ToString() ;


            }
        //Процедура сброса интерфейса при обрыве связи
        private void vIndi_Clear()
        {
            gridHRTable.Rows.Clear();
     
            foreach (TextBox element in lstIndIR) 
            {
                element.Text = "0";
                element.ForeColor = Color.White;
            }

            tbState.Text = "порт не открыт";
            tbState.ForeColor = Color.Gray;
            Server.logger.Clear();
            if(bloader!=null) bloader.logger.Clear();

            btnCnct.Text = "Соединить";
            Server.uiHoldingReg = new ushort[256];
            Server.uiInputReg   = new ushort[256];
            txtBoxLog.Clear();
        }
        // Создание таблицы параметров
        private void vIndi_HRGrid_init(UInt32 size)
        {

            string str;
            int i = 0;

            gridHRTable.Rows.Clear();
            if (size > 255) size = 255;
            while (i < size)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.HeaderCell.Value = "Row";
                row.CreateCells(gridHRTable);
                var listSet = new DataGridViewComboBoxCell();
                var btnSend = new DataGridViewButtonCell();
                btnSend.Value = "Задать";


                switch (i)
                {
                    case 0:
                        listSet.Items.AddRange(new string[]
                        {
                            " ",
                            "0x0002 Стоп","0x0001 Старт","0x0004 Сброс", "0x1603 Перезагрузка",
                            "0x2801 Откл. Авар.", "0x0008 Сохранить настройки", "0x0010 Загрузить настройки", "0x0012 Настройки по умолчанию",
                            "0x0300 Тест вентилятора", "0x1303 Тест энкодера", "0x7777 Bootloader", "0x0016 Тест релейных выходов", "0x1304 Конфигурация 1310HM"
                        });
                        row.Cells[3] = listSet;
                        row.Cells[4] = btnSend;
                        if (Server.uiHoldingReg[i] < (row.Cells[3] as DataGridViewComboBoxCell).Items.Count)
                            row.Cells[3].Value = (row.Cells[3] as DataGridViewComboBoxCell).Items[Server.uiHoldingReg[i]];
                        break;
                    case 2:
                        listSet.Items.AddRange(new string[] { "9600", "38400", "115200", "128000", "230400", "406000" });
                        Application.DoEvents();
                        row.Cells[3] = listSet;
                        row.Cells[4] = btnSend;
                        if(Server.uiHoldingReg[i]< (row.Cells[3] as DataGridViewComboBoxCell).Items.Count)
                            row.Cells[3].Value = (row.Cells[3] as DataGridViewComboBoxCell).Items[Server.uiHoldingReg[i]];
                        break;
                    case 4:
                        listSet.Items.AddRange(new string[] { "Вперед", "Назад" });
                        row.Cells[3] = listSet;
                        row.Cells[4] = btnSend;
                        if (Server.uiHoldingReg[i] < (row.Cells[3] as DataGridViewComboBoxCell).Items.Count)
                            row.Cells[3].Value = (row.Cells[3] as DataGridViewComboBoxCell).Items[Server.uiHoldingReg[i]];
                        break;

                    case 10:
                        listSet.Items.AddRange(new string[] { "Modbus", "Панель", "Релейные сигналы","Аналоговый сигнал","Modbus регистр 45"});
                        row.Cells[3] = listSet;
                        row.Cells[4] = btnSend;
                        if (Server.uiHoldingReg[i] < (row.Cells[3] as DataGridViewComboBoxCell).Items.Count)
                            row.Cells[3].Value = (row.Cells[3] as DataGridViewComboBoxCell).Items[Server.uiHoldingReg[i]];
                        break;
                    case 11:
                        listSet.Items.AddRange(new string[] { "АД Скалярное", "СД по датчику", "СД без датчика", "АД без датчика" });
                        row.Cells[3] = listSet;
                        row.Cells[4] = btnSend;
                        if (Server.uiHoldingReg[i] < (row.Cells[3] as DataGridViewComboBoxCell).Items.Count)
                            row.Cells[3].Value = (row.Cells[3] as DataGridViewComboBoxCell).Items[Server.uiHoldingReg[i]];
                        break;
                    case 39:
                        listSet.Items.AddRange(new string[] { "Выбег", "Снижение частоты", "Постоянный ток" });
                        row.Cells[3] = listSet;
                        row.Cells[4] = btnSend;
                        if (Server.uiHoldingReg[i] < (row.Cells[3] as DataGridViewComboBoxCell).Items.Count)
                            row.Cells[3].Value = (row.Cells[3] as DataGridViewComboBoxCell).Items[Server.uiHoldingReg[i]];
                        break;

                    case 43:
                        listSet.Items.AddRange(new string[] { "Замкнутый", "Разомкнутый", "Всегда 0", "Всегда 1" });
                        row.Cells[3] = listSet;
                        row.Cells[4] = btnSend;
                        if (Server.uiHoldingReg[i] < (row.Cells[3] as DataGridViewComboBoxCell).Items.Count)
                            row.Cells[3].Value = (row.Cells[3] as DataGridViewComboBoxCell).Items[Server.uiHoldingReg[i]];
                        break;
                    case 56:
                        listSet.Items.AddRange(new string[] { "Нет действия", "Предупреждение", "Авария" });
                        row.Cells[3] = listSet;
                        row.Cells[4] = btnSend;
                        if (Server.uiHoldingReg[i] < (row.Cells[3] as DataGridViewComboBoxCell).Items.Count)
                            row.Cells[3].Value = (row.Cells[3] as DataGridViewComboBoxCell).Items[Server.uiHoldingReg[i]];
                        break;
                }
                str = "";
                if (i < slPrmNam.Count) str = slPrmNam[i];
                row.SetValues(new object[] { i, str, Server.uiHoldingReg[i] });
                gridHRTable.Rows.Add(row);

                i++;
            }
        }

        private void txtBox_ModbusAdr_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtBoxModbusAdr.Text))
            {
                if (Convert.ToDouble(txtBoxModbusAdr.Text) > 255) txtBoxModbusAdr.Text = "1";
            }
        }
        private void txtBox_ModbusAdr_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8)
            {
                e.Handled = true;
            }
        }

        private void cmbBox_PortList_DropDown(object sender, EventArgs e)
        {
            cmbBoxPortList.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length == 0) return;
            cmbBoxPortList.Items.AddRange(ports);
            if (cmbBoxPortList.Items.Count == 0) cmbBoxPortList.Text = "";
        }
        private void cmbBoxChart1Series_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < chart1.Series.Count; i++)
            {
                chart1.Series[i].Enabled = false;
                if (i == cmbBoxChart1Series.SelectedIndex) chart1.Series[i].Enabled = true;
            }

        }
        private void cmbBoxChart2Series_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < chart1.Series.Count; i++)
            {
               chart2.Series[i].Enabled = false;
               if (i == cmbBoxChart2Series.SelectedIndex) chart2.Series[i].Enabled = true;
            }

        }
        private void cmbBoxServerDelay_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.uiServerDelay = Convert.ToUInt16(cmbBoxServerDelay.Items[cmbBoxServerDelay.SelectedIndex]);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (Server.blDevCnctd)
            {
                gridHRTable.Rows[3].Cells[2].Value = trackBar1.Value;
                tbFREQ_ref.Text = (Convert.ToDouble(trackBar1.Value.ToString())/10).ToString()+" Гц";
            }
        }
        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            if (Server.blDevCnctd)
            {
                Server.uialHRForWrite.Add(new UInt16[2] { 3,(UInt16) trackBar1.Value });
                gridHRTable.Rows[3].Cells[2].Style.BackColor = Color.Red;
            }
        }
        private void trackBar1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                if (Server.blDevCnctd)
                {
                    Server.uialHRForWrite.Add(new UInt16[2] { 3, (UInt16)trackBar1.Value });
                    gridHRTable.Rows[3].Cells[2].Style.BackColor = Color.Red;
                }
            }
        }

        private void GridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.CellStyle.BackColor = Color.Aquamarine;
        }
        private void GridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

            if (e.ColumnIndex < 2) { e.Cancel = true; return; };
            if (e.ColumnIndex > 2 && (gridHRTable.SelectedCells[0].Value == null)) { e.Cancel = true; return; };
            if (gridHRTable.SelectedCells[0].Value != null) sTempForCell = gridHRTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

        }
        private void GridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
               int num = 0;
                if (gridHRTable.SelectedCells[0].ColumnIndex == 2)
                  if(gridHRTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value!=null)
                    if (int.TryParse(gridHRTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out num))
                    {

                        Server.uialHRForWrite.Add(new UInt16[2] {(UInt16) e.RowIndex, (UInt16)num });
                            gridHRTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                        return;

                    }
                        

                gridHRTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = sTempForCell;
                return;
            }

        }
        private void GridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == 4) if (gridHRTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {

                    UInt16 index = (UInt16)(gridHRTable.Rows[e.RowIndex].Cells[3] as DataGridViewComboBoxCell).Items.IndexOf
                    (
                        gridHRTable.Rows[e.RowIndex].Cells[3].Value
                    );


                    if (e.RowIndex == 0)
                    {
                        vSetComandForDev(uaCMD[index]);
                        return;
                    };

                    Server.uialHRForWrite.Add(new UInt16[2] {(UInt16) e.RowIndex, index });
                    gridHRTable.Rows[e.RowIndex].Cells[2].Value = index;
                    gridHRTable.Rows[e.RowIndex].Cells[2].Style.BackColor = Color.Red;

                    //  Debug.WriteLine(gridHRTable.Rows[e.RowIndex].Cells[3].Value.ToString()+data);
            }
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            vSetComandForDev(eDev_cmd.RUN);
        }
        private void btn_Stop_Click(object sender, EventArgs e)
        {
            vSetComandForDev(eDev_cmd.STOP);
        }
        private void btn_Reset_Click(object sender, EventArgs e)
        {
            vSetComandForDev(eDev_cmd.RESET);
        }
        private void btn_Cnct_Click(object sender, EventArgs e)
        {

            if (Server.spPort.IsOpen)
            {
                try { Server.spPort.Close(); } catch (Exception ex) { };
              //  btnCnct.Text = "Connect";
                Server.vReset();
                if (bloader != null) bloader.Reset();
               // vIndi_Clear();
                return;
            };

            if (String.IsNullOrEmpty(cmbBoxPortList.SelectedItem?.ToString())) return;
            if (String.IsNullOrEmpty(cmbBoxBaudRate.SelectedItem?.ToString())) return;

            Server.spPort.BaudRate = Convert.ToInt32(cmbBoxBaudRate.SelectedItem?.ToString());
            Server.spPort.PortName = cmbBoxPortList.SelectedItem?.ToString();
            Server.spPort.Parity = Parity.None;
            Server.spPort.DataBits = 8;
            Server.spPort.ReadTimeout = 500;
            Server.btDevAdr = Convert.ToByte(txtBoxModbusAdr.Text);

            try
            { Server.spPort.Open(); }
            catch (Exception ex) { Server.logger.Add(ex.Message.ToString()); Server.vReset(); btnCnct.Text = "Отключить"; return; };

           // txtBoxLog.Text= "Открытие порта";

            if (!Updater.IsAlive) Updater.Start();

        }

     
        private void clearChartItem_Click(object sender, EventArgs e)
        {
            foreach (Series el in chart1.Series)  el.Points.Clear();
            foreach (Series el in chart2.Series)  el.Points.Clear();
        }


        private void tbFREQ_ref_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8 && number != 44 )
            {
                e.Handled = true;
                return;
            }

           if (((TextBox)sender).Text.IndexOf("Г") != -1) ((TextBox)sender).Text = "";

        }


        private void tbFREQ_ref_KeyUp(object sender, KeyEventArgs e)
        {
           switch (e.KeyCode )
            {
                case Keys.Enter:
                if (Server.blDevCnctd)
                {
                        UInt16 temp;
                        
                        try
                        {
                            temp = (UInt16) (Convert.ToDouble(tbFREQ_ref.Text) * 10);
                            trackBar1.Value = temp;
                        }
                        catch (Exception e2) { return; };

                        if (Server.blDevCnctd)
                        {
                            gridHRTable.Rows[3].Cells[2].Value = temp;
                            gridHRTable.Rows[3].Cells[2].Style.BackColor = Color.Red;

                            Server.uialHRForWrite.Add(new UInt16[2] { 3, temp });

                        }
                      


                    }
                return;

                case Keys.Escape:
                        tbFREQ_ref.Text = (Convert.ToDouble(trackBar1.Value.ToString()) / 10).ToString() + " Гц";
                return;
            }



        }


        private void MenuItem_Param_Save_ToFile_Click(object sender, EventArgs e)
        {

            if (!Server.blDevCnctd) return;
            DateTime thisDay = DateTime.Today;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Файл параметров|*.txt";
            saveFileDialog1.FileName = thisDay.ToShortDateString() + "_prm";
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel) return;
            string fname = saveFileDialog1.FileName;

            if (File.Exists(fname)) File.Delete(fname);
            // Create the file.
            using (FileStream fs = File.Create(fname))
            {
                fs.Write(new UTF8Encoding(true).GetBytes(Server.strDevID), 0, Server.strDevID.Length);

                string str = null;
                for (int i = 1; i < Server.uiInputReg[1]; i++)
                {
                    byte[] line = new UTF8Encoding(true).GetBytes("\n" + Server.uiHoldingReg[i].ToString());
                    fs.Write(line, 0, line.Length);
                }

            }


        }

        private void MenuItem_Param_Save_ToDev_Click(object sender, EventArgs e)
        {
            if (!Server.blDevCnctd) return;
            vSetComandForDev(eDev_cmd.SAVEPRM);
        }

        private void MenuItem_Param_Load_FromFile_Click(object sender, EventArgs e)
        {

            if (!Server.blDevCnctd) return;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Файл параметров|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                string str;
                slPrmToSaveRead.Clear();
                slPrmToSaveRead.Add(0);
                sr.ReadLine();

                while ((str = sr.ReadLine()) != null)
                {

                    try
                    {
                        slPrmToSaveRead.Add((UInt16)Convert.ToDouble(str));
                    }
                    catch (FormatException) { slPrmToSaveRead.Add(0); };

                }
                sr.Close();
                fs.Close();
            }
            catch (Exception) { return; }

            foreach (DataGridViewRow row in gridHRTable.Rows)
            {
                row.Cells[2].Value = 0;
                row.Cells[2].Style.BackColor = Color.Pink;
            }

            UInt16 i = 0;
            Server.uialHRForWrite.Clear();
            foreach (UInt16 data in slPrmToSaveRead)
            {
                if (i > Server.uiInputReg[1] - 1) break;

                Server.uialHRForWrite.Add(new UInt16[1] { data });

                gridHRTable.Rows[0].Cells[2].Value = data;
                gridHRTable.Rows[0].Cells[2].Style.BackColor = Color.Red;

                i++;

            }

#if DEBUG
            Debug.WriteLine("OPEN FILE FOR WRITE " + filename);
            string tstr = null;
            foreach (ushort el in slPrmToSaveRead) tstr += el.ToString() + ".";
            Debug.WriteLine(tstr);
#endif

            slPrmToSaveRead.Clear();
            Server.blnWriteAllHR = true;

        }

        private void MenuItem_Param_Load_FromDev_Click(object sender, EventArgs e)
        {
            if (!Server.blDevCnctd) return;

                vSetComandForDev(eDev_cmd.LOADPRM);
                foreach (DataGridViewRow row in gridHRTable.Rows)
                {
                    row.Cells[2].Style.BackColor = Color.Pink;
                }
                Application.DoEvents();
                Thread.Sleep(500);
                Server.uilHRadrForRead.Add(0);
                Server.uilHRadrForRead.Add(256);
        }

        private void MenuItem_About_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ООО НПФ НПТ 2018 г.", "ModBus Сервер МПЧ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MenuItem_Refresh_State_Click(object sender, EventArgs e)
        {           
            if (!Server.blDevCnctd) return;
            tabForm.SelectTab(0);
            Server.blReadIRreq = true;
            Server.iFail = 0;
        }

        private void MenuItem_Refresh_Dev_Click(object sender, EventArgs e)
        {
            if (!Server.blDevCnctd) return;
            Server.blReadIRreq = true;
            tabForm.SelectTab(0);
            int i = 0;
            foreach (UInt16 data in Server.uiHoldingReg)
            {
                if (i > Server.uiInputReg[1] - 1) break;

                Server.uialHRForWrite.Add(new UInt16[1] { data });

                gridHRTable.Rows[i].Cells[2].Value = data;
                gridHRTable.Rows[i].Cells[2].Style.BackColor = Color.Red;

                i++;

            }

            Server.blnWriteAllHR = true;
            Server.iFail = 0;
        }

        private void MenuItem_Refresh_Prog_Click(object sender, EventArgs e)
        {
            if (!Server.blDevCnctd) return;
            tabForm.SelectTab(0);
            Server.uilHRadrForRead.Add(0);
            Server.uilHRadrForRead.Add(256);
            foreach (DataGridViewRow row in gridHRTable.Rows)
            {
                row.Cells[2].Style.BackColor = Color.Pink;
            }
            Server.iFail = 0;
        }


        private void MenuItem_Loader_Flash_Click(object sender, EventArgs e)
        {
            if (!Server.spPort.IsOpen) return;
            if ((Server.uiInputReg[2] & 0xFF00) == 0x0300) return;
            bloader = new Bootloader(Server.spPort);

            if (bloader.ProcBisy) return;

            tabForm.SelectTab(3);

            Server.suspend = true;
            Server.blUpdGridHR = false;
            if (bloader.GetLoader())
            {
                tbState.Text = "Режим загрузчика";


                bloader.FOpen();

                Thread Tloader = new Thread(bloader.threadProgram);
                Tloader.Start();
                return;
            }

            Server.suspend = false;
            Server.blUpdGridHR = true;
        }

        private void MenuItem_Loader_Verify_Click(object sender, EventArgs e)
        {

            if (!Server.spPort.IsOpen) return;
            if ((Server.uiInputReg[2] & 0xFF00) == 0x0300) return;
            bloader = new Bootloader(Server.spPort);

            if (bloader.ProcBisy) return;

            tabForm.SelectTab(3);

            Server.suspend = true;
            Server.blUpdGridHR = false;
            if (bloader.GetLoader())
            {
                tbState.Text = "Режим загрузчика";

                bloader.FOpen();

                Thread Tloader = new Thread(bloader.threadVeryfi);
                Tloader.Start();
                return;
            }

            Server.suspend = false;
            Server.blUpdGridHR = true;

        }

        private void MenuItem_Loader_Reset_Click(object sender, EventArgs e)
        {
            if (bloader == null) return;
            if (!bloader.devconected) return;

            if (!bloader.port.IsOpen) { bloader.logger.Add(new string[] { "Ошибка открытия порта" }); return; }
            bloader.port.Write("R");
            System.Threading.Thread.Sleep(100);
            if (bloader.port.ReadExisting() == "R") { bloader.logger.Add(new string[] { "Перезагрузка ... ок" }); return; }
            bloader.logger.Add(new string[] { "нет ответа" });
        }

        private void MenuItem_Loader_Stop_Click(object sender, EventArgs e)
        {
            if (bloader == null) return;
            if (bloader.ProcBisy) bloader.ChnlReq = true;
        }

        private void MenuItem_Scope_Start_Click(object sender, EventArgs e)
        {
            if (ScopeForm != null) if (ScopeForm.Created) { ScopeForm.BringToFront(); return; };

            if (!Server.blnScpEnbl && Server.blDevCnctd)
            {
              
                ScopeForm = new FormScope(Server);
                ScopeForm.Show();
            };
        }

        private void MenuItem_Scope_Stop_Click(object sender, EventArgs e)
        {
            if (ScopeForm != null) if (ScopeForm.Created) ScopeForm.Close();
        }

        private void MenuItem_Param_MotorSel_Click(object sender, EventArgs e)
        {
            if (!Server.blDevCnctd) return;
            if (SetupForm1 != null) if (SetupForm1.Created) { SetupForm1.BringToFront(); return; };
            SetupForm1 = new SetupForm(Server);
            SetupForm1.Show();
        }
    }
}
