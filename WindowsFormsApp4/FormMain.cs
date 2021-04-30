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

namespace WindowsFormsApp4
{

    public partial class FormMain : Form
    {

        MODBUS_srv Server = new MODBUS_srv();
        Bootloader bloader = null;
        List<TextBox> lstIndIR = new List<TextBox>();
       
        public enum eDev_cmd
        {
            RUN = 0x1,
            STOP = 0x2,
            RESET = 0x4,
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
        private UInt16 uiServerDelay = 10;
        private List<ParamNames> paramNames;
        private InputRegisterIndicator timeStepInd;
        FormGensig FormGenSig = new FormGensig();

        private class CustomControlTyple
        {
            public TextBox textboxIndicator { get; set; }
            public TrackBar trackBarController { get; set; }
            public UInt16 index { get; set; }

        }

        private class InputRegisterIndicator
        {

            public Label label;
            public TextBox indicator;
            public FormChart chart;

            private string RegSing = " ";

            public string Name { // name of input register
                get { return label.Text; }
                set {
                    label.Text = RegSing + " " + value;
                    chart.Label = label.Text;
                }
            }

            public bool RegSingEnable {

                set {
                    if (value) RegSing = "Рег. " + Adr.ToString();
                    if (!value) RegSing = "";
                }
            }

            private int scale;
            public int Scale
            {
                get { return scale; }
                set { scale = value; if (scale <= 0) scale = 1; }
            }

            public int Adr { get; set; }
            public int Min { get; set; }
            public int Max { get; set; }
            public string Dimension { get; set; }

            public int value {
                get { return 0; }
                set {
                    float temp = (float)value / Scale;
                    if (Scale == 1) { indicator.Text = value.ToString(); } else {
                        indicator.Text = temp.ToString("#####0.0") + " " + Dimension;
                    }

                    chart.AddPoint(value);
                    indicator.ForeColor = Color.LightGreen;
                    if (temp < Min) indicator.ForeColor = Color.Cyan;
                    if (temp > Max) indicator.ForeColor = Color.LightCoral;
                }
            }


            public InputRegisterIndicator(int pos) {

                Adr = pos;
                chart = new FormChart(Adr - 3);
                Scale = 1;
                Min = -1;
                Max = 1;
                RegSingEnable = true;


                label = new Label();
                label.Dock = DockStyle.Fill;
                label.Margin = new System.Windows.Forms.Padding(1, 10, 1, 0);
                label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                Name = "";

                indicator = new TextBox();
                indicator.Name = "IRTextBox_" + Adr.ToString("D2");
                indicator.Dock = DockStyle.Fill;
                indicator.BackColor = System.Drawing.SystemColors.MenuText;
                indicator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                indicator.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                indicator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
                indicator.HideSelection = false;
                indicator.Location = new System.Drawing.Point(15, 203);
                indicator.Margin = new System.Windows.Forms.Padding(15, 3, 50, 3);
                indicator.MaximumSize = new System.Drawing.Size(1000, 1000);
                indicator.ReadOnly = false;
                indicator.Size = new System.Drawing.Size(100, 33);
                indicator.TabIndex = 6;
                indicator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;


                indicator.Click += new System.EventHandler((s, e) =>
                {
                    this.chart.Show();
                    this.chart.BringToFront();
                });

            }

        }

        private List<CustomControlTyple> customControlsList = new List<CustomControlTyple>();
        private List<InputRegisterIndicator> IrIndicList = new List<InputRegisterIndicator>();

        public delegate void MyDelegate();

        public class ParamNames
        {

            [JsonProperty("Device")]
            public string Device { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Options")]
            public List<List<string>> Options { get; set; }

            [JsonProperty("Control")]
            public bool Control { get; set; }

            [JsonProperty("Max")]
            public int Max { get; set; }

            [JsonProperty("Min")]
            public int Min { get; set; }

            [JsonProperty("Scl")]
            public int Scl { get; set; }

            [JsonProperty("Adr")]
            public int Adr { get; set; }

            [JsonProperty("Descript")]
            public string Descript { get; set; }

            [JsonProperty("Dim")]
            public string Dim { get; set; }
        }




        public FormMain()
        {

            InitializeComponent();

            try
            {
                string jsonString = File.ReadAllText("prm.json", Encoding.Default);
                paramNames = JsonConvert.DeserializeObject<List<ParamNames>>(jsonString);
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
            this.toolStripComboBox_RefTime.SelectedIndex = 2;


            //debug TimeStep
            timeStepInd = new InputRegisterIndicator(100);
            timeStepInd.RegSingEnable = false;
            timeStepInd.Name = "TimeStep, msec";
            this.tableLayoutPanel_debug.Controls.Add(timeStepInd.label, 0, 0);
            this.tableLayoutPanel_debug.RowCount++;
            this.tableLayoutPanel_debug.Controls.Add(timeStepInd.indicator, 0, 1);
            this.tableLayoutPanel_debug.RowCount++;

            Task_SlavePoll();
            Task_IndiRefresh();
            Server.vPoll();


           // List<int> a = new List<int> {1,2,3,0,0 };
           // var b = a.GroupBy( _ => _ != 0).ToList()[0].Count();
           // Debug.WriteLine(b.Count());

            btn_Cnct_Click(new object(), new EventArgs());
            TStart_Scope();



        }

        private async void TStart_Scope() {

            while (true) { 
            MenuItem_Scope_Start_Click(new object(), new EventArgs());
            await Task.Delay(200);
            if (ScopeForm != null) if (ScopeForm.Created) return;
            }

        }
        

        //основной поток

        private Stopwatch startTime = Stopwatch.StartNew();
        private double timeStep;

        private async void Task_SlavePoll() {
            while (true)
            {
                await Task.Run(() => {
                    if (Server.spPort.IsOpen)
                    {


                        if (Server.suspend) { return; }

                        if (bloader != null) bloader = null;

                        if (!Server.blDevCnctd)
                        {
                            Server.vConnectToDevAsync();
                            if (!Server.blDevCnctd)
                            {
                                Server.iFail++;
                                Server.logger.Add("Нет ответа");
                                btn_Cnct_Click(this, null);
                                return;
                            }

                            BeginInvoke(new MyDelegate(vSearchDeviceDescriptionFile));

                        }

                       
                    }
                    else {
                        Server.vReset();
                        bloader = null;
                    }

                    startTime.Stop();
                    timeStep = (double)startTime.ElapsedMilliseconds / 1000;
                    startTime.Restart();

                });
                
                await Task.Delay(1);
               
            }
        }

        private async void Task_IndiRefresh()
        {
            double counter = 0.0;
            int pos = 0;

            while (true)
            {
                await Task.Run(() =>
                {

                    try { BeginInvoke(new MyDelegate(vLog_Update)); }
                    catch (Exception) { }

                    try
                    {
                        if (Server.spPort.IsOpen)
                        {
                            BeginInvoke(new MyDelegate(vIndi_Update));
                        }
                        else
                        {

                            BeginInvoke(new MyDelegate(vIndi_Clear));
                        }
                    }
                    catch (System.InvalidOperationException e) { Task.Delay(1000); };


                    if (Server.spPort.IsOpen)
                    {
                        if (FormGenSig.GetState())
                        {
                            UInt16 point = FormGenSig.GetReference();
                            UInt16 target = FormGenSig.GetTargetHR();
                            FormGenSig.SetTargetRef(Server.uiHoldingReg[target]);
                            FormGenSig.SetResponce(Server.uiInputReg[FormGenSig.GetResponceIR()]);
                            if (Server.uiHoldingReg[target] != point)
                                Server.uialHRForWrite.Add(new UInt16[2] { target, point });
                        }

                        try { BeginInvoke(new MyDelegate(() => { FormGenSig.proc(timeStep); })); }
                        catch (Exception) { }

                        Server.blReadIRreq = true;
                    }


                });

                await Task.Delay(uiServerDelay);
            }
        }

        private void vSearchDeviceDescriptionFile() {
            // поиск файлов настройки
            string[] allFoundFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.json", SearchOption.AllDirectories);

            foreach (var file in allFoundFiles)
            {
 
                try
                {
                    string jsonString = File.ReadAllText(file, Encoding.Default);
                    List<ParamNames> temp = JsonConvert.DeserializeObject<List<ParamNames>>(jsonString);

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
                catch (Newtonsoft.Json.JsonReaderException ex) {

                }


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
                    {                      var lines = txtBoxLog.Lines.ToList();
                        lines.RemoveAt(lines.Count - 1);
                        txtBoxLog.Lines = lines.ToArray();
                    }
                    txtBoxLog.AppendText(Environment.NewLine + "BL: " + bloader.logger[0][0]);
                    bloader.logger.RemoveAt(0);
                };

            //if (txtBoxLog.Lines.Count() > 20) txtBoxLog.Clear();

            if (Server.suspend) return;

            //  Debug.WriteLine(Server.uiInputReg[1].ToString());
            //кнопка коннект
            if (Server.spPort.IsOpen) { ToolStripMenuItem_Connect.Text = "Отключить"; } else { ToolStripMenuItem_Connect.Text = "Соединить"; };
            //проверяем актуальность размера таблиц
            if (gridHRTable.Rows.Count < Server.uiInputReg[1]) vIndi_HRGrid_init(Server.uiInputReg[1]);

            //Input Registers table
            if (tableLayoutPanel_IR.RowCount < Server.uiInputReg[0]) {

                tableLayoutPanel_IR.Controls.Clear();
                IrIndicList.Clear();
                tableLayoutPanel_IR.RowCount = 1;
                tableLayoutPanel_IR.ColumnCount = 1;

                i = 2; while (i++ < Server.uiInputReg[0]) {

                    InputRegisterIndicator ir = new InputRegisterIndicator(i);

                    foreach (var el in paramNames) if (el.Adr == 1000 + i) {

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
                    foreach (var el in customControlsList) {
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
                            else {

                                tmp_trackBarValue = Convert.ToUInt16(row.Cells[2].Value);
                            }

                            if  (tmp_trackBarValue > el.trackBarController.Maximum)
                            {
                                el.trackBarController.BackColor = Color.LightPink;
                                el.trackBarController.Value = el.trackBarController.Maximum;
                            }else if(tmp_trackBarValue < el.trackBarController.Minimum)
                            {
                                el.trackBarController.BackColor = Color.LightPink;
                                el.trackBarController.Value = el.trackBarController.Minimum;
                            } else {
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
            tsStatus.Text =Server.spPort.PortName + " "+ Server.spPort.BaudRate + " устройство [" + Server.strDevID + "] статус [0x0" + Convert.ToString(Server.uiInputReg[2], 16) + "]. Ошибок связи " + Server.iFail.ToString();
            timeStepInd.value =(int) (timeStep*1000);


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
                else { paramNames.Add(new ParamNames()); }


                // custom controls
                if (paramNames[i].Control != false) {

                    bool elementExist = false;
                    //foreach (var el in this.customControlsList)
                    //    if (el.textboxIndicator.Name == "CustomTexBox_" + i.ToString()) elementExist = true;
                    foreach (var el in this.customControlsList)
                        if (el.index ==  (UInt16)i) elementExist = true;
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
                        if(Server.uiHoldingReg[i] < bar.Maximum) bar.Value = Server.uiHoldingReg[i];
                        //Контекстное меню для регуляторов
                        ToolStripMenuItem TrackBarlimitMenu_Hi = new ToolStripMenuItem("Максимум");
                        ToolStripTextBox toolStripText_Hi = new ToolStripTextBox();
                        toolStripText_Hi.Text = bar.Maximum.ToString();
                        toolStripText_Hi.TextBoxTextAlign = HorizontalAlignment.Center;
                        toolStripText_Hi.KeyUp += new KeyEventHandler((s, e) => {
                            if (e.KeyCode != Keys.Enter) return;
                            try
                            {
                                bar.Maximum = Convert.ToInt32(toolStripText_Hi.Text);
                                if (bar.Minimum < 0 && bar.Maximum > 32767) {
                                    bar.Maximum = 32767;
                                    toolStripText_Hi.Text = bar.Maximum.ToString();
                                }
                            }
                            catch (Exception) { toolStripText_Hi.Text = bar.Maximum.ToString(); };
                        });
                        TrackBarlimitMenu_Hi.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {toolStripText_Hi});
                        ToolStripMenuItem TrackBarlimitMenu_Lo = new ToolStripMenuItem("Минимум");
                        ToolStripTextBox toolStripText_Lo = new ToolStripTextBox();
                        toolStripText_Lo.Text = bar.Minimum.ToString();
                        toolStripText_Lo.TextBoxTextAlign = HorizontalAlignment.Center;
                        toolStripText_Lo.KeyUp += new KeyEventHandler((s, e) => {
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
                        bar.MouseUp += new MouseEventHandler((s, e) => {
                            if (!Server.blDevCnctd) return;
                            UInt16 tmp = Convert.ToUInt16(bar.Name.Substring(bar.Name.IndexOf('_') + 1, 2));
                            Server.uialHRForWrite.Add(new UInt16[2] { (UInt16)tmp, (UInt16)bar.Value });
                            gridHRTable.Rows[tmp].Cells[2].Style.BackColor = Color.Red;
                        });
                        bar.ValueChanged += new EventHandler((s, e) => {
                            if (!Server.blDevCnctd) return;
                            UInt16 tmp = Convert.ToUInt16(bar.Name.Substring(bar.Name.IndexOf('_') + 1, 2));
                            foreach (var el in customControlsList)
                                if (tmp == el.index) el.textboxIndicator.Text = bar.Value.ToString();
                        });
                        bar.KeyUp += new KeyEventHandler((s, e) => {
                            if (!(e.KeyCode == Keys.Up | e.KeyCode == Keys.Down | e.KeyCode == Keys.Right | e.KeyCode == Keys.Left)) return;
                            if (!Server.blDevCnctd) return;
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
                            if (!Server.blDevCnctd) return;

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
                if(paramNames[i].Descript != null) row.Cells[1].ToolTipText = paramNames[i].Descript;
                gridHRTable.Rows.Add(row);

                i++;
            }

            if (paramNames[0].Device != null) { this.tabForm.TabPages[0].Text = "Параметры [ " + paramNames[0].Device + " ]"; } else { this.tabForm.TabPages[0].Text = "Параметры [ ]"; };
        }

        private void txtBox_ModbusAdr_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(toolStripTextBox_adr.Text))
            {
                if (Convert.ToDouble(toolStripTextBox_adr.Text) > 255) toolStripTextBox_adr.Text = "1";
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


        private void cmbBoxServerDelay_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.uiServerDelay = Convert.ToUInt16(this.toolStripComboBox_RefTime.Items[this.toolStripComboBox_RefTime.SelectedIndex]);
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
                    Server.uialHRForWrite.Add(new UInt16[2] { (UInt16)e.RowIndex, data});
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
            List<String> broken_ports = new List<string>();
            if (bs_flg) return;
            con_start:
            bs_flg = true;
            Int32[] bds = new Int32[] { 9600, 38400, 115200, 128000, 230400, 406000 };
            List<String> ports = SerialPort.GetPortNames().ToList();

            foreach (var item in broken_ports)
            {
                int ind = ports.IndexOf(item);
                if (ind >= 0) ports.RemoveAt(ind);
            }

            if (ports.Count == 0) { Debug.WriteLine("No port found"); return; }

            if (Server.spPort.IsOpen)
            {
                try { Server.spPort.Close(); } catch (Exception ex) { };
                bool tmp = Server.blDevCnctd;
                Server.vReset();
                if (bloader != null) bloader.Reset();
                if (spdpos == bds.Length - 1) { spdpos = 0; portpos++; } else { spdpos++; }
                if (portpos == ports.Count || tmp)
                {
                    spdpos = 0;
                    portpos = 0;
                    Server.logger.Add("Сброс соединения ");
                    bs_flg = false;
                    return;
                };
            };


            Server.spPort.BaudRate = bds[spdpos];
            Server.spPort.PortName = ports[portpos];
            Server.spPort.Parity = Parity.None;
            Server.spPort.DataBits = 8;
            Server.spPort.ReadTimeout = 500;
            Server.btDevAdr = Convert.ToByte(toolStripTextBox_adr.Text);



            try
            {
                Server.spPort.Open();
            }
            catch (System.UnauthorizedAccessException ex)
            {
                Server.logger.Add(ports[portpos].ToString() + " порт занят");
                portpos++;
                spdpos = 0;
                goto con_start;


            }
            catch (Exception ex)
            {
                Server.logger.Add(ex.Message.ToString());
                Server.vReset();
                ToolStripMenuItem_Connect.Text = "Отключить";
                bs_flg = false;
                Server.logger.Add(ports[portpos].ToString() + " ошибка доступа xxx");
                broken_ports.Add(ports[portpos].ToString());
                goto con_start;
            };

        
             bs_flg = false;

        }


        private void tbFREQ_ref_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8 && number != 44)
            {
                e.Handled = true;
                return;
            }

            if (((TextBox)sender).Text.IndexOf("Г") != -1) ((TextBox)sender).Text = "";

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
            if (bloader.port.ReadExisting() == "R") {
                bloader.logger.Add(new string[] { "Перезагрузка ... ок" });
                this.btn_Cnct_Click(this,null);
                this.btn_Cnct_Click(this, null);
                return; }
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

        private void MenuItem_SetParamDescriptFile_Click(object sender, EventArgs e)
        {
            if (!Server.blDevCnctd) return;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Файл описания параметров|*.json";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;

            try
            {
                string jsonString = File.ReadAllText(filename, Encoding.Default);
                paramNames = JsonConvert.DeserializeObject<List<ParamNames>>(jsonString);
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
            if (FormGenSig != null) if (FormGenSig.Created) {
                    FormGenSig.Show();
                    FormGenSig.BringToFront();
                    return;
            };

            FormGenSig = new FormGensig();
            FormGenSig.Show();
        }

        private void toolStripComboBox_RefTime_Click(object sender, EventArgs e)
        {

        }
    }
}