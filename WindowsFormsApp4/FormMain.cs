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
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text.RegularExpressions;
using consoleTask;
using ctsServerAdapter;
using System.Net.NetworkInformation;

namespace WindowsFormsApp4
{

    public partial class FormMain : Form
    {

        private enum eDev_cmd
        {
            RUN = 0x1, STOP = 0x2, RESET = 0x4, REBOOT = 0x1603, SAVEPRM = 0x8, LOADPRM = 0x10, DEFPRM = 0x12,
            CHKDSBL = 0x2801, BOOT = 0x7777, FANTST = 0x300, ENCTST = 0x1303, RDOTST = 0x0016, SET1310 = 0x1304
        }

        private ConnectionSetups connection_setups = new ConnectionSetups();

        private MODBUS_srv Server =  new MODBUS_srv();
        private Bootloader bloader = null;
        private SetupForm SetupForm1 = null;
        private FormGensig FormGenSig;

        private string sTempForCell = null;
        private UInt16 uiServerDelay = 10;
       
        private InputRegisterIndicator timeStepInd;
        
        private List<UInt16> slPrmToSaveRead = new List<UInt16>();
        private List<TextBox> lstIndIR = new List<TextBox>();
        private List<string> slErrMes = new List<string>();
        private List<CustomControlsReader> paramNames;
        private List<CustomControlTyple> customControlsList = new List<CustomControlTyple>();
        private List<InputRegisterIndicator> IrIndicList = new List<InputRegisterIndicator>();

        public FormScope ScopeForm = null;
        public delegate void MyDelegate();

        public FormMain()
        {

            InitializeComponent();
            connection_setups = ConnectionSetups.read();

            try
            {
                string jsonString = File.ReadAllText("prm.json", Encoding.Default);
                paramNames = JsonConvert.DeserializeObject<List<CustomControlsReader>>(jsonString);
            }
            catch (Newtonsoft.Json.JsonReaderException e)
            {
                Debug.WriteLine(e);
            }


            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = "Сервер МПЧ " + version;

            var dsdf = new FormChart(1);

            string[] ports = SerialPort.GetPortNames();

            vIndi_Clear();
            // читаю файл с именами ошибок
            try
            {
                FileStream fs = new FileStream("ERR.mpch", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    slErrMes.Add(str);
                }
                sr.Close();
                fs.Close();
            }
            catch (Exception ex) { };

            //Заполняю таблицу RIO
            for (int i = 0; i < 8; i++)
            {

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(gridRelayIO);
                row.SetValues(new object[] { "Выход " + i.ToString(), 0, "Вход " + i.ToString(), 0 });
                this.gridRelayIO.Rows.Add(row);

            }

            ToolStripMenuItem_Connect.Text = "Соединить";

            //debug TimeStep
            timeStepInd = new InputRegisterIndicator(100);
            timeStepInd.RegSingEnable = false;
            timeStepInd.Name = "TimeStep, msec";
            this.tableLayoutPanel_debug.Controls.Add(timeStepInd.label, 0, 0);
            this.tableLayoutPanel_debug.RowCount++;
            this.tableLayoutPanel_debug.Controls.Add(timeStepInd.indicator, 0, 1);
            this.tableLayoutPanel_debug.RowCount++;

            Server.SlavePollAsync(300);
            Task_FormRefreshAsync();

            if (connection_setups.Aconnect) {
                btn_Cnct_Click(new object(), new EventArgs());
            }
        }

        private async void TStart_Scope()
        {

            while (true)
            {
                MenuItem_Scope_Start_Click(new object(), new EventArgs());
                await Task.Delay(200);
                if (ScopeForm != null) if (ScopeForm.Created) return;
            }

        }


        //основной поток
        private async void Task_FormRefreshAsync()
        {
            double counter = 0.0;
            int pos = 0;

            while (true)
            {
               

                    try { 
                        BeginInvoke(new MyDelegate(vLog_Update)); }
                    catch (Exception e) { Debug.WriteLine(e.Message); }

                    try
                    {
                        if (Server.isDeviceConnected)
                        {
                            BeginInvoke(new MyDelegate(vIndi_Update));
                        }
                        else
                        {

                            BeginInvoke(new MyDelegate(vIndi_Clear));
                        }
                    }
                    catch (System.InvalidOperationException e) { await Task.Delay(1000); };


                    if (Server.isDeviceConnected)
                    {
                        //if (FormGenSig.GetState())
                        //{
                        //    //UInt16 point = FormGenSig.GetReference();
                        //    //UInt16 target = FormGenSig.GetTargetHR();
                        //    //FormGenSig.SetTargetRef(Server.uiHoldingReg[target]);
                        //    //FormGenSig.SetResponce(Server.uiInputReg[FormGenSig.GetResponceIR()]);
                        //    //if (Server.uiHoldingReg[target] != point)
                        //    //    Server.uialHRForWrite.Add(new UInt16[2] { target, point });
                        //};

                        //try { BeginInvoke(new MyDelegate(() => { FormGenSig.proc(timeStep); })); }
                        //catch (Exception) { }

                        Server.blReadIRreq = true;
                    }

                await Task.Delay(100);
            }
        }

        private void vSearchDeviceDescriptionFile()
        {
            // поиск файлов настройки
            string[] allFoundFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.json", SearchOption.AllDirectories);

            foreach (var file in allFoundFiles)
            {

                try
                {
                    string jsonString = File.ReadAllText(file, Encoding.Default);
                    List<CustomControlsReader> temp = JsonConvert.DeserializeObject<List<CustomControlsReader>>(jsonString);

                    if (Server.strDevID.IndexOf(temp[0].Device) >= 0)
                    {


                        DialogResult result = MessageBox.Show(
                            "Найден файл описания настроек для устройства \n [" + temp[0].Device + " ]",
                            "Внимание",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1
                            );

                        if (result == DialogResult.OK)
                        {

                        }

                        paramNames = temp;

                        customControlsList.Clear();
                        tableLayoutPanel_IR.RowCount = 0;
                        tableLayoutPanel_IR.Controls.Clear();

                        tableLayoutPanel_customControl.Controls.Clear();
                        vIndi_HRGrid_init(Server.uiInputReg[1]);
                    }
                }
                catch (Newtonsoft.Json.JsonReaderException ex)
                {

                }


            }

        }

        private void vSetComandForDev(eDev_cmd cmd)
        {
            if (Server.isDeviceConnected)
            {
                Server.uialHRForWrite.Add(new UInt16[2] { 0, (UInt16)cmd });
                gridHRTable.Rows[0].Cells[2].Value = (UInt16)cmd;
                gridHRTable.Rows[0].Cells[2].Style.BackColor = Color.Red;
            }
        }

        private void vLog_Update()
        {
            // пишу сообщения в лог 
            while (Server.logger.Count != 0) // Print Log 
            {
                txtBoxLog.AppendText(Environment.NewLine + DateTime.Now.ToLongTimeString() + "  " + Server.logger[0]);
                Server.logger.RemoveAt(0);
            };
        }

        // Обновление индикаторов
        private void vIndi_Update()
        {
            int i = 0;

            if (bloader != null) while (bloader.logger.Count != 0) // Print Log 
                {

                    if (bloader.logger[0].Length > 1)
                    {
                        var lines = txtBoxLog.Lines.ToList();
                        lines.RemoveAt(lines.Count - 1);
                        txtBoxLog.Lines = lines.ToArray();
                    }
                    txtBoxLog.AppendText(Environment.NewLine + "BL: " + bloader.logger[0][0]);
                    bloader.logger.RemoveAt(0);
                };

            //if (txtBoxLog.Lines.Count() > 20) txtBoxLog.Clear();

            if (Server.suspend) return;

            //  Debug.WriteLine(Server.uiInputReg[1].ToString());
            ////кнопка коннект
            //if (Server.spPort.IsOpen) { ToolStripMenuItem_Connect.Text = "Отключить"; } else { ToolStripMenuItem_Connect.Text = "Соединить"; };
            ////проверяем актуальность размера таблиц
            if (gridHRTable.Rows.Count < Server.uiInputReg[1]) vIndi_HRGrid_init(Server.uiInputReg[1]);

            //Input Registers table
            if (tableLayoutPanel_IR.RowCount < Server.uiInputReg[0])
            {

                tableLayoutPanel_IR.Controls.Clear();
                IrIndicList.Clear();
                tableLayoutPanel_IR.RowCount = 1;
                tableLayoutPanel_IR.ColumnCount = 1;

                i = 2; while (i++ < Server.uiInputReg[0])
                {

                    InputRegisterIndicator ir = new InputRegisterIndicator(i);

                    foreach (var el in paramNames) if (el.Adr == 1000 + i)
                        {

                            ir.Name = el.Name;
                            ir.Scale = el.Scl;
                            ir.Max = el.Max;
                            ir.Min = el.Min;
                            ir.Dimension = el.Dim;
                        }

                    tableLayoutPanel_IR.Controls.Add(ir.label, 0, tableLayoutPanel_IR.RowCount - 1);
                    tableLayoutPanel_IR.RowCount++;

                    tableLayoutPanel_IR.Controls.Add(ir.indicator, 0, tableLayoutPanel_IR.RowCount - 1);
                    tableLayoutPanel_IR.RowCount++;

                    IrIndicList.Add(ir);

                };

            }

            foreach (var el in IrIndicList)
            {
                el.value = ((Int16)Server.uiInputReg[IrIndicList.IndexOf(el) + 3]);
            }



            //обновляю таблицу RIO
            BeginInvoke(new MyDelegate(() =>
            {

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
            }));

            //обновляю таблицу параметров
            i = 0;
            if (Server.blUpdGridHR)
            {
                foreach (DataGridViewRow row in gridHRTable.Rows)
                {

                    if (row.Cells[2].Value != null)
                    {
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
                    }
                    // задание отрицательных значений
                    if ((Convert.ToInt32(row.Cells[2].Value.ToString()) < 0) && (Server.uiHoldingReg[i] > 0X7FFF))
                    {
                        if (Convert.ToInt32(row.Cells[2].Value.ToString()) == ((Int32)Server.uiHoldingReg[i] - 65536))
                        {
                            row.Cells[2].Style.BackColor = Color.LightGreen;
                        }
                        else
                        {
                            row.Cells[2].Value = Server.uiHoldingReg[i];
                        }
                    }
                    else
                    {

                        row.Cells[2].Value = Server.uiHoldingReg[i];
                    }


                    // обновление listbox соответсвенно значению
                    if ((row.Cells[3] as DataGridViewComboBoxCell) != null)
                    {
                        foreach (var opt in paramNames[i].Options)
                        {
                            if (Convert.ToUInt16(opt[1]) == Server.uiHoldingReg[i])
                            {
                                row.Cells[3].Value = (row.Cells[3] as DataGridViewComboBoxCell).Items[paramNames[i].Options.IndexOf(opt)];
                            };
                        };
                    };


                    // обновляю customControlsTextBox
                    foreach (var el in customControlsList)
                    {
                        if (el.index == i)
                        {
                            int tmp_trackBarValue = 0;

                            if (el.trackBarController.Minimum < 0)
                            {
                                if (Convert.ToInt32(row.Cells[2].Value) > 65536 / 2)
                                {
                                    tmp_trackBarValue = Convert.ToInt32(row.Cells[2].Value) - 65536;
                                }
                                else
                                {
                                    tmp_trackBarValue = Convert.ToInt32(row.Cells[2].Value);
                                }
                            }
                            else
                            {

                                tmp_trackBarValue = Convert.ToUInt16(row.Cells[2].Value);
                            }

                            if (tmp_trackBarValue > el.trackBarController.Maximum)
                            {
                                el.trackBarController.BackColor = Color.LightPink;
                                el.trackBarController.Value = el.trackBarController.Maximum;
                            }
                            else if (tmp_trackBarValue < el.trackBarController.Minimum)
                            {
                                el.trackBarController.BackColor = Color.LightPink;
                                el.trackBarController.Value = el.trackBarController.Minimum;
                            }
                            else
                            {
                                el.trackBarController.Value = tmp_trackBarValue;
                                el.trackBarController.BackColor = Color.White;
                            };
                            el.textboxIndicator.Text = tmp_trackBarValue.ToString();
                        };
                    };
                    i++;
                }
                Server.blUpdGridHR = false;
            }


            // обновляю поле статуса
            // tbState.Text = "0x0" + Convert.ToString(Server.uiInputReg[2], 16) + Environment.NewLine;

            switch (Server.uiInputReg[2] & 0xFF00)
            {
                case 0x0:
                    tbState.ForeColor = Color.Blue; tbState.Text = "Нет связи ";
                    break;
                case 0x100:
                    tbState.ForeColor = Color.YellowGreen; tbState.Text = "Инициализация - ";
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
            string info = String.Format("{0} [{1}] статус 0x0{2}  ошибок связи {3}", Server.info, Server.strDevID, Convert.ToString(Server.uiInputReg[2], 16), Server.iFail.ToString());
            tsStatus.Text = info;

            //кнопка соединить
            if (Server.isDeviceConnected)
            {
                ToolStripMenuItem_Connect.Text = "Отключить";
            }
            else {
                ToolStripMenuItem_Connect.Text = "Соединить";
            }
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
            //Server.logger.Clear();
            if (bloader != null) bloader.logger.Clear();

            ToolStripMenuItem_Connect.Text = "Соединить";
            Server.uiHoldingReg = new ushort[256];
            Server.uiInputReg = new ushort[256];
            //txtBoxLog.Clear();
            // 
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

                row.CreateCells(gridHRTable);
                var listSet = new DataGridViewComboBoxCell();
                var btnSend = new DataGridViewButtonCell();
                btnSend.Value = "Задать";

                if (i < (paramNames.Count - 1) && paramNames[i].Options != null)
                {

                    listSet.Items.Clear();
                    foreach (var itm in paramNames[i].Options)
                    {
                        listSet.Items.Add(itm[0]);
                    };
                    row.Cells[3] = listSet;
                    row.Cells[4] = btnSend;
                    if (Server.uiHoldingReg[i] < (row.Cells[3] as DataGridViewComboBoxCell).Items.Count)
                        row.Cells[3].Value = (row.Cells[3] as DataGridViewComboBoxCell).Items[Server.uiHoldingReg[i]];
                }
                else { /*paramNames.Add(new ParamNames());*/ }


                // custom controls
                if (paramNames[i].Control != false)
                {

                    bool elementExist = false;
                    //foreach (var el in this.customControlsList)
                    //    if (el.textboxIndicator.Name == "CustomTexBox_" + i.ToString()) elementExist = true;
                    foreach (var el in this.customControlsList)
                        if (el.index == (UInt16)i) elementExist = true;
                    if (elementExist == false)
                    {
                        if (tableLayoutPanel_customControl.RowCount == 1) tableLayoutPanel_customControl.RowCount++;
                        Label lb = new Label();
                        lb.Text = paramNames[i].Name;
                        lb.Dock = DockStyle.Fill;
                        lb.Margin = new System.Windows.Forms.Padding(1, 10, 1, 0); ;
                        tableLayoutPanel_customControl.Controls.Add(lb, 0, tableLayoutPanel_customControl.RowCount - 1);
                        tableLayoutPanel_customControl.RowCount++;
                        TrackBar bar = new TrackBar();
                        bar.Name = "CustomTrackBar_" + i.ToString("D2");
                        bar.Dock = DockStyle.Fill;
                        bar.Maximum = paramNames[i].Max;
                        bar.Minimum = paramNames[i].Min;
                        if (Server.uiHoldingReg[i] < bar.Maximum) bar.Value = Server.uiHoldingReg[i];
                        //Контекстное меню для регуляторов
                        ToolStripMenuItem TrackBarlimitMenu_Hi = new ToolStripMenuItem("Максимум");
                        ToolStripTextBox toolStripText_Hi = new ToolStripTextBox();
                        toolStripText_Hi.Text = bar.Maximum.ToString();
                        toolStripText_Hi.TextBoxTextAlign = HorizontalAlignment.Center;
                        toolStripText_Hi.KeyUp += new KeyEventHandler((s, e) =>
                        {
                            if (e.KeyCode != Keys.Enter) return;
                            try
                            {
                                bar.Maximum = Convert.ToInt32(toolStripText_Hi.Text);
                                if (bar.Minimum < 0 && bar.Maximum > 32767)
                                {
                                    bar.Maximum = 32767;
                                    toolStripText_Hi.Text = bar.Maximum.ToString();
                                }
                            }
                            catch (Exception) { toolStripText_Hi.Text = bar.Maximum.ToString(); };
                        });
                        TrackBarlimitMenu_Hi.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripText_Hi });
                        ToolStripMenuItem TrackBarlimitMenu_Lo = new ToolStripMenuItem("Минимум");
                        ToolStripTextBox toolStripText_Lo = new ToolStripTextBox();
                        toolStripText_Lo.Text = bar.Minimum.ToString();
                        toolStripText_Lo.TextBoxTextAlign = HorizontalAlignment.Center;
                        toolStripText_Lo.KeyUp += new KeyEventHandler((s, e) =>
                        {
                            if (e.KeyCode != Keys.Enter) return;
                            try
                            {
                                bar.Minimum = Convert.ToInt32(toolStripText_Lo.Text);
                                if (bar.Minimum < -32767)
                                {
                                    bar.Minimum = -32767;
                                    toolStripText_Lo.Text = bar.Minimum.ToString();
                                }
                            }
                            catch (Exception) { toolStripText_Lo.Text = bar.Minimum.ToString(); };
                        });
                        TrackBarlimitMenu_Lo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripText_Lo });
                        ContextMenuStrip contextMenuForTrackBar = new ContextMenuStrip();
                        contextMenuForTrackBar.Items.AddRange(new[] { TrackBarlimitMenu_Hi, TrackBarlimitMenu_Lo });
                        bar.ContextMenuStrip = contextMenuForTrackBar;
                        //Changed handler for Custom Bar
                        bar.MouseUp += new MouseEventHandler((s, e) =>
                        {
                            if (!Server.isDeviceConnected) return;
                            UInt16 tmp = Convert.ToUInt16(bar.Name.Substring(bar.Name.IndexOf('_') + 1, 2));
                            Server.uialHRForWrite.Add(new UInt16[2] { (UInt16)tmp, (UInt16)bar.Value });
                            gridHRTable.Rows[tmp].Cells[2].Style.BackColor = Color.Red;
                        });
                        bar.ValueChanged += new EventHandler((s, e) =>
                        {
                            if (!Server.isDeviceConnected) return;
                            UInt16 tmp = Convert.ToUInt16(bar.Name.Substring(bar.Name.IndexOf('_') + 1, 2));
                            foreach (var el in customControlsList)
                                if (tmp == el.index) el.textboxIndicator.Text = bar.Value.ToString();
                        });
                        bar.KeyUp += new KeyEventHandler((s, e) =>
                        {
                            if (!(e.KeyCode == Keys.Up | e.KeyCode == Keys.Down | e.KeyCode == Keys.Right | e.KeyCode == Keys.Left)) return;
                            if (!Server.isDeviceConnected) return;
                            UInt16 tmp = Convert.ToUInt16(bar.Name.Substring(bar.Name.IndexOf('_') + 1, 2));
                            Server.uialHRForWrite.Add(new UInt16[2] { (UInt16)tmp, (UInt16)bar.Value });
                            gridHRTable.Rows[tmp].Cells[2].Style.BackColor = Color.Red;
                        });
                        tableLayoutPanel_customControl.Controls.Add(bar, 0, tableLayoutPanel_customControl.RowCount - 1);
                        TextBox tb = new TextBox();
                        tb.Name = "CustomTextBox_" + i.ToString("D2");
                        tb.KeyUp += new KeyEventHandler((s, e) =>
                        {
                            if (e.KeyCode != Keys.Enter) return;
                            if (!Server.isDeviceConnected) return;

                            UInt16 tmp_index = Convert.ToUInt16(tb.Name.Substring(tb.Name.IndexOf('_') + 1, 2));
                            UInt16 tmp_val = 0;
                            try
                            {
                                tmp_val = (UInt16)Convert.ToInt32(tb.Text);
                            }
                            catch (Exception) { return; };
                            gridHRTable.Rows[3].Cells[2].Value = tmp_val;
                            gridHRTable.Rows[3].Cells[2].Style.BackColor = Color.Red;
                            Server.uialHRForWrite.Add(new UInt16[2] { (UInt16)tmp_index, (UInt16)tmp_val });
                        });
                        tb.Dock = DockStyle.Fill;
                        tb.BackColor = System.Drawing.SystemColors.MenuText;
                        tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                        tb.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                        tb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
                        tb.HideSelection = false;
                        tb.Location = new System.Drawing.Point(15, 203);
                        tb.Margin = new System.Windows.Forms.Padding(15, 3, 10, 3);
                        tb.MaximumSize = new System.Drawing.Size(1000, 1000);
                        tb.ReadOnly = false;
                        tb.Size = new System.Drawing.Size(150, 33);
                        tb.TabIndex = 6;
                        tb.Text = Server.uiHoldingReg[i].ToString();
                        tb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

                        tableLayoutPanel_customControl.Controls.Add(tb, 1, tableLayoutPanel_customControl.RowCount - 1);
                        tableLayoutPanel_customControl.RowCount++;

                        // добавляю в customControlsList
                        CustomControlTyple tmpcntrl = new CustomControlTyple();
                        tmpcntrl.textboxIndicator = tb;
                        tmpcntrl.trackBarController = bar;
                        tmpcntrl.index = (UInt16)i;
                        this.customControlsList.Add(tmpcntrl);
                    }

                }

                // if (i == 0) foreach (var itm in listSet.Items) Debug.WriteLine(itm.ToString());

                str = "";
                if (i < paramNames.Count) str = paramNames[i].Name;
                row.SetValues(new object[] { i, str, Server.uiHoldingReg[i] });
                if (paramNames[i].Descript != null) row.Cells[1].ToolTipText = paramNames[i].Descript;
                gridHRTable.Rows.Add(row);

                i++;
            }

            if (paramNames[0].Device != null) { this.tabForm.TabPages[0].Text = "Параметры [ " + paramNames[0].Device + " ]"; } else { this.tabForm.TabPages[0].Text = "Параметры [ ]"; };
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
                    if (gridHRTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                        if (int.TryParse(gridHRTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out num))
                        {
                            Server.uialHRForWrite.Add(new UInt16[2] { (UInt16)e.RowIndex, (UInt16)num });
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

                    UInt16 data = Convert.ToUInt16(paramNames[e.RowIndex].Options[index][1]);
                    Server.uialHRForWrite.Add(new UInt16[2] { (UInt16)e.RowIndex, data });
                    gridHRTable.Rows[e.RowIndex].Cells[2].Value = data;
                    gridHRTable.Rows[e.RowIndex].Cells[2].Style.BackColor = Color.Red;
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

        static int spdpos = 0;
        static int portpos = 0;
        static bool bs_flg = false;



        private void btn_Cnct_Click(object sender, EventArgs e)
        {

            if (Server.isDeviceConnected)
            {
                Server.logger.Add("Отключаю соединение");
                Server.Close();
                
            }
            else {

                Server.logger.Add("Установка соединения");
                Server.OpenConsole = connection_setups.RunServerConsole;

                if (connection_setups.DevSearch) {
                    Task.Run(() => { 
                        Server.SearchPort(
                            connection_setups.ServerName,
                            connection_setups.ServerPort); 
                    });
                    return;
                }

                if (connection_setups.Attach) {
                    Task.Run(() => { 
                        Server.ConncetToRunningServer(
                            connection_setups.ServerName, 
                            connection_setups.ServerPort); 
                    });
                    return;
                }

                Task.Run(async () =>
                {
                    await Server.StartPort(
                    connection_setups.ComPortName,
                    connection_setups.BaudRate,
                    connection_setups.ServerName,
                    connection_setups.ServerPort
                    );
                });
            }
        }



        private void MenuItem_Param_Save_ToFile_Click(object sender, EventArgs e)
        {

            if (!Server.isDeviceConnected) return;
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
            if (!Server.isDeviceConnected) return;
            vSetComandForDev(eDev_cmd.SAVEPRM);
        }

        private void MenuItem_Param_Load_FromFile_Click(object sender, EventArgs e)
        {

            if (!Server.isDeviceConnected) return;

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
            if (!Server.isDeviceConnected) return;

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
            if (!Server.isDeviceConnected) return;
            tabForm.SelectTab(0);
            Server.blReadIRreq = true;
            Server.iFail = 0;
        }

        private void MenuItem_Refresh_Dev_Click(object sender, EventArgs e)
        {
            if (!Server.isDeviceConnected) return;
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
            if (!Server.isDeviceConnected) return;
            tabForm.SelectTab(0);
            Server.uilHRadrForRead.Add(0);
            Server.uilHRadrForRead.Add(256);
            foreach (DataGridViewRow row in gridHRTable.Rows)
            {
                row.Cells[2].Style.BackColor = Color.Pink;
            }
            Server.iFail = 0;
        }


        private async void MenuItem_Loader_Flash_Click(object sender, EventArgs e)
        {
            //     if (toolStripMenuItemConSpd.SelectedItem == null) return;

            


            string pname = connection_setups.ComPortName;
            int pspeed = connection_setups.BaudRate;
            int dadr = connection_setups.SlaveAdr;

            // получаем выбранный файл
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Filter = "Файл прошивки|*.hex";
            if (openFileDialog2.ShowDialog() == DialogResult.Cancel)
                return;

           string filename = "\"" + openFileDialog2.FileName + "\"";

            if (Server.isDeviceConnected)
            {
                var res = await Server.GetComInfoAsync();
                Server.logger.Add(String.Format("{0}:{1}", res.Item1, res.Item2));
                pname = res.Item1;
                pspeed = res.Item2;
                dadr = 1;
                btn_Cnct_Click(sender, e);
            }

            string dst = pname + "," + pspeed + "," + dadr;

            tabForm.SelectTab(3);
            if (Server.logger.Count != 0)
            {
                Server.logger.Clear();
            }

            LoaderUtilsAdapter.LoaderUtilsAdapter.SetOutput(txtBoxLog);
            LoaderUtilsAdapter.LoaderUtilsAdapter.Version();
            while (LoaderUtilsAdapter.LoaderUtilsAdapter.IsRunning()) ;
            LoaderUtilsAdapter.LoaderUtilsAdapter.Write(filename, dst);
        }

        private async void MenuItem_Loader_Verify_Click(object sender, EventArgs e)
        {
            if (toolStripMenuItemConSpd.SelectedItem == null) return;

            string pname = connection_setups.ComPortName;
            int pspeed = connection_setups.BaudRate;
            int dadr = connection_setups.SlaveAdr;

            // получаем выбранный файл
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Filter = "Файл прошивки|*.hex";
            if (openFileDialog2.ShowDialog() == DialogResult.Cancel)
                return;

            string filename = "\"" + openFileDialog2.FileName + "\"";

            if (Server.isDeviceConnected)
            {
                var res = await Server.GetComInfoAsync();
                Server.logger.Add(String.Format("{0}:{1}", res.Item1, res.Item2));
                pname = res.Item1;
                pspeed = res.Item2;
                dadr = 1;
                btn_Cnct_Click(sender, e);
            }

            string dst = pname + "," + pspeed + "," + dadr;

            tabForm.SelectTab(3);
            if (Server.logger.Count != 0) {
                Server.logger.Clear();
            }

            LoaderUtilsAdapter.LoaderUtilsAdapter.SetOutput(txtBoxLog);
            LoaderUtilsAdapter.LoaderUtilsAdapter.Version();
            while (LoaderUtilsAdapter.LoaderUtilsAdapter.IsRunning());
            LoaderUtilsAdapter.LoaderUtilsAdapter.Compare(filename, dst);

        }

        private void MenuItem_Loader_Reset_Click(object sender, EventArgs e)
        {
            if (toolStripMenuItemConSpd.SelectedItem == null) return;
            //if (Server.spPort.IsOpen) { return; }
            string pname = connection_setups.ComPortName;
            int pspeed = Convert.ToInt32(toolStripMenuItemConSpd.SelectedItem.ToString());
            int dadr = connection_setups.BaudRate;
            string dst = pname + "," + pspeed + "," + dadr;
            tabForm.SelectTab(3);
            LoaderUtilsAdapter.LoaderUtilsAdapter.SetOutput(txtBoxLog);
            LoaderUtilsAdapter.LoaderUtilsAdapter.Version();
            while (LoaderUtilsAdapter.LoaderUtilsAdapter.IsRunning()) ;
            LoaderUtilsAdapter.LoaderUtilsAdapter.ResetMCU(dst);
        }

        private void MenuItem_Loader_Stop_Click(object sender, EventArgs e)
        {
            if (Server.isDeviceConnected) { return; }
            LoaderUtilsAdapter.LoaderUtilsAdapter.Abort();

        }

        private void MenuItem_Scope_Start_Click(object sender, EventArgs e)
        {
            if (ScopeForm != null) if (ScopeForm.Created) { ScopeForm.BringToFront(); return; };

            if (!Server.blnScpEnbl && Server.isDeviceConnected)
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
            if (!Server.isDeviceConnected) return;
            if (SetupForm1 != null) if (SetupForm1.Created) { SetupForm1.BringToFront(); return; };
            SetupForm1 = new SetupForm(Server);
            SetupForm1.Show();
        }

        private void MenuItem_SetParamDescriptFile_Click(object sender, EventArgs e)
        {
            if (!Server.isDeviceConnected) return;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Файл описания параметров|*.json";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = "\"" + openFileDialog1.FileName+ "\"";

            try
            {
                string jsonString = File.ReadAllText(filename, Encoding.Default);
                paramNames = JsonConvert.DeserializeObject<List<CustomControlsReader>>(jsonString);
                customControlsList.Clear();
                tableLayoutPanel_IR.RowCount = 0;
                tableLayoutPanel_IR.Controls.Clear();

                tableLayoutPanel_customControl.Controls.Clear();
                vIndi_HRGrid_init(Server.uiInputReg[1]);

            }
            catch (Newtonsoft.Json.JsonReaderException ex)
            {
                Debug.WriteLine(ex);

                MessageBox.Show(
                    "Файл не содержит описания настроек",
                    "Ошибка чтения файла описания настроек ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);

            }


        }

        private void toolStripTextBox_adr_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {

            }
        }

        private void ToolStripMenuItem_ToolGen_Click(object sender, EventArgs e)
        {
            if (FormGenSig != null) if (FormGenSig.Created)
                {
                    FormGenSig.Show();
                    FormGenSig.BringToFront();
                    return;
                };

            FormGenSig = new FormGensig(Server);
            FormGenSig.Show();
        }

        private void toolStripComboBox_RefTime_Click(object sender, EventArgs e)
        {

        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(CtsServerAdapter.isAlaive()) CtsServerAdapter.Close();
        }

        private void tableLayoutPanel_IR_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripMenuItemConPort_Click(object sender, EventArgs e)
        {

        }

        private void ParamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConnectionSetups connectionSetups = new FormConnectionSetups();
            connection_setups = ConnectionSetups.read();
            if (Server.isDeviceConnected) {
                btn_Cnct_Click(sender, e);
                Thread.Sleep(300);
                btn_Cnct_Click(sender, e);
            }
        }
    }
}