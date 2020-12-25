namespace WindowsFormsApp4
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.contextMenuForChart = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tbState = new System.Windows.Forms.TextBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MenuItem_Refresh = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Refresh_State = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Refresh_Dev = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Refresh_Prog = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Connect = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Connect = new System.Windows.Forms.ToolStripMenuItem();
            this.адресToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox_adr = new System.Windows.Forms.ToolStripTextBox();
            this.MenuItem_RefrTime = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox_RefTime = new System.Windows.Forms.ToolStripComboBox();
            this.MenuItem_About = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Scope = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Scope_Start = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Scope_Stop = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Param = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Param_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Param_Save_ToFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Param_Save_ToDev = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Param_Load = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Param_Load_FromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Param_Load_FromDev = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Param_MotorSel = new System.Windows.Forms.ToolStripMenuItem();
            this.выбратьФайлОписанияНастроекToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Loader = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Loader_Flash = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Loader_Verify = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Loader_Reset = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Loader_Stop = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel_Controls = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_Button = new System.Windows.Forms.TableLayoutPanel();
            this.btn_Reset = new System.Windows.Forms.Button();
            this.tabForm = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.gridHRTable = new System.Windows.Forms.DataGridView();
            this.adr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.set = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.read = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel_customControl = new System.Windows.Forms.TableLayoutPanel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.gridRelayIO = new System.Windows.Forms.DataGridView();
            this.input = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Ivalue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Out = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Ovalue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtBoxLog = new System.Windows.Forms.TextBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel_main = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_IR = new System.Windows.Forms.TableLayoutPanel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel_Controls.SuspendLayout();
            this.tableLayoutPanel_Button.SuspendLayout();
            this.tabForm.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridHRTable)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridRelayIO)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel_main.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuForChart
            // 
            this.contextMenuForChart.Name = "contextMenuForChart";
            this.contextMenuForChart.Size = new System.Drawing.Size(61, 4);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 584);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(952, 22);
            this.statusStrip1.TabIndex = 26;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsStatus
            // 
            this.tsStatus.Name = "tsStatus";
            this.tsStatus.Size = new System.Drawing.Size(89, 17);
            this.tsStatus.Text = "нет устройства";
            // 
            // tbState
            // 
            this.tbState.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbState.BackColor = System.Drawing.SystemColors.Menu;
            this.tbState.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbState.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbState.Location = new System.Drawing.Point(3, 3);
            this.tbState.Name = "tbState";
            this.tbState.Size = new System.Drawing.Size(690, 31);
            this.tbState.TabIndex = 27;
            this.tbState.Text = "\nНет соединения";
            this.tbState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnRun.ForeColor = System.Drawing.Color.Green;
            this.btnRun.Location = new System.Drawing.Point(3, 3);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(134, 46);
            this.btnRun.TabIndex = 23;
            this.btnRun.Text = "Пуск";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btn_Run_Click);
            // 
            // btn_Stop
            // 
            this.btn_Stop.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Stop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_Stop.ForeColor = System.Drawing.Color.Red;
            this.btn_Stop.Location = new System.Drawing.Point(143, 3);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(122, 46);
            this.btn_Stop.TabIndex = 23;
            this.btn_Stop.Text = "Стоп";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.menuStrip1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_Refresh,
            this.MenuItem_Connect,
            this.MenuItem_About,
            this.MenuItem_Scope,
            this.MenuItem_Param,
            this.MenuItem_Loader});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.menuStrip1.Size = new System.Drawing.Size(952, 29);
            this.menuStrip1.TabIndex = 29;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // MenuItem_Refresh
            // 
            this.MenuItem_Refresh.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_Refresh_State,
            this.MenuItem_Refresh_Dev,
            this.MenuItem_Refresh_Prog});
            this.MenuItem_Refresh.Image = global::WindowsFormsApp4.Properties.Resources.available_updates_32;
            this.MenuItem_Refresh.Name = "MenuItem_Refresh";
            this.MenuItem_Refresh.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.MenuItem_Refresh.Size = new System.Drawing.Size(109, 25);
            this.MenuItem_Refresh.Text = "Обновить";
            // 
            // MenuItem_Refresh_State
            // 
            this.MenuItem_Refresh_State.Name = "MenuItem_Refresh_State";
            this.MenuItem_Refresh_State.Size = new System.Drawing.Size(325, 26);
            this.MenuItem_Refresh_State.Text = "Обновить статус";
            this.MenuItem_Refresh_State.Click += new System.EventHandler(this.MenuItem_Refresh_State_Click);
            // 
            // MenuItem_Refresh_Dev
            // 
            this.MenuItem_Refresh_Dev.Name = "MenuItem_Refresh_Dev";
            this.MenuItem_Refresh_Dev.Size = new System.Drawing.Size(325, 26);
            this.MenuItem_Refresh_Dev.Text = "Обновить регистры на устройстве";
            this.MenuItem_Refresh_Dev.Click += new System.EventHandler(this.MenuItem_Refresh_Dev_Click);
            // 
            // MenuItem_Refresh_Prog
            // 
            this.MenuItem_Refresh_Prog.Name = "MenuItem_Refresh_Prog";
            this.MenuItem_Refresh_Prog.Size = new System.Drawing.Size(325, 26);
            this.MenuItem_Refresh_Prog.Text = "Обновить регистры в программе";
            this.MenuItem_Refresh_Prog.Click += new System.EventHandler(this.MenuItem_Refresh_Prog_Click);
            // 
            // MenuItem_Connect
            // 
            this.MenuItem_Connect.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.MenuItem_Connect.CheckOnClick = true;
            this.MenuItem_Connect.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Connect,
            this.адресToolStripMenuItem,
            this.MenuItem_RefrTime});
            this.MenuItem_Connect.Image = global::WindowsFormsApp4.Properties.Resources.connect;
            this.MenuItem_Connect.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.MenuItem_Connect.Name = "MenuItem_Connect";
            this.MenuItem_Connect.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.MenuItem_Connect.Size = new System.Drawing.Size(126, 25);
            this.MenuItem_Connect.Text = "Соединение";
            // 
            // ToolStripMenuItem_Connect
            // 
            this.ToolStripMenuItem_Connect.Name = "ToolStripMenuItem_Connect";
            this.ToolStripMenuItem_Connect.Size = new System.Drawing.Size(190, 26);
            this.ToolStripMenuItem_Connect.Text = "Соединить";
            this.ToolStripMenuItem_Connect.Click += new System.EventHandler(this.btn_Cnct_Click);
            // 
            // адресToolStripMenuItem
            // 
            this.адресToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox_adr});
            this.адресToolStripMenuItem.Name = "адресToolStripMenuItem";
            this.адресToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.адресToolStripMenuItem.Text = "ModBus адрес";
            // 
            // toolStripTextBox_adr
            // 
            this.toolStripTextBox_adr.Name = "toolStripTextBox_adr";
            this.toolStripTextBox_adr.Size = new System.Drawing.Size(100, 23);
            this.toolStripTextBox_adr.Text = "1";
            this.toolStripTextBox_adr.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolStripTextBox_adr.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.toolStripTextBox_adr_KeyPress);
            this.toolStripTextBox_adr.TextChanged += new System.EventHandler(this.txtBox_ModbusAdr_TextChanged);
            // 
            // MenuItem_RefrTime
            // 
            this.MenuItem_RefrTime.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox_RefTime});
            this.MenuItem_RefrTime.Name = "MenuItem_RefrTime";
            this.MenuItem_RefrTime.Size = new System.Drawing.Size(190, 26);
            this.MenuItem_RefrTime.Text = "Период опроса";
            // 
            // toolStripComboBox_RefTime
            // 
            this.toolStripComboBox_RefTime.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripComboBox_RefTime.Items.AddRange(new object[] {
            "5",
            "10",
            "50",
            "100",
            "150",
            "200",
            "500",
            "1000"});
            this.toolStripComboBox_RefTime.Name = "toolStripComboBox_RefTime";
            this.toolStripComboBox_RefTime.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripComboBox_RefTime.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBox_RefTime.SelectedIndexChanged += new System.EventHandler(this.cmbBoxServerDelay_SelectedIndexChanged);
            // 
            // MenuItem_About
            // 
            this.MenuItem_About.Enabled = false;
            this.MenuItem_About.Image = global::WindowsFormsApp4.Properties.Resources.inf;
            this.MenuItem_About.Name = "MenuItem_About";
            this.MenuItem_About.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.MenuItem_About.Size = new System.Drawing.Size(134, 25);
            this.MenuItem_About.Text = "О программе";
            this.MenuItem_About.Visible = false;
            this.MenuItem_About.Click += new System.EventHandler(this.MenuItem_About_Click);
            // 
            // MenuItem_Scope
            // 
            this.MenuItem_Scope.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_Scope_Start,
            this.MenuItem_Scope_Stop});
            this.MenuItem_Scope.Image = global::WindowsFormsApp4.Properties.Resources.scope;
            this.MenuItem_Scope.Name = "MenuItem_Scope";
            this.MenuItem_Scope.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.MenuItem_Scope.Size = new System.Drawing.Size(135, 25);
            this.MenuItem_Scope.Text = "Осциллограф";
            // 
            // MenuItem_Scope_Start
            // 
            this.MenuItem_Scope_Start.Image = global::WindowsFormsApp4.Properties.Resources.play;
            this.MenuItem_Scope_Start.Name = "MenuItem_Scope_Start";
            this.MenuItem_Scope_Start.Size = new System.Drawing.Size(121, 26);
            this.MenuItem_Scope_Start.Text = "Старт";
            this.MenuItem_Scope_Start.Click += new System.EventHandler(this.MenuItem_Scope_Start_Click);
            // 
            // MenuItem_Scope_Stop
            // 
            this.MenuItem_Scope_Stop.Image = global::WindowsFormsApp4.Properties.Resources.stop;
            this.MenuItem_Scope_Stop.Name = "MenuItem_Scope_Stop";
            this.MenuItem_Scope_Stop.Size = new System.Drawing.Size(121, 26);
            this.MenuItem_Scope_Stop.Text = "Стоп";
            this.MenuItem_Scope_Stop.Click += new System.EventHandler(this.MenuItem_Scope_Stop_Click);
            // 
            // MenuItem_Param
            // 
            this.MenuItem_Param.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_Param_Save,
            this.MenuItem_Param_Load,
            this.MenuItem_Param_MotorSel,
            this.выбратьФайлОписанияНастроекToolStripMenuItem});
            this.MenuItem_Param.Image = global::WindowsFormsApp4.Properties.Resources.no_translate_detected_318_46670;
            this.MenuItem_Param.Name = "MenuItem_Param";
            this.MenuItem_Param.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.MenuItem_Param.Size = new System.Drawing.Size(120, 25);
            this.MenuItem_Param.Text = "Параметры";
            // 
            // MenuItem_Param_Save
            // 
            this.MenuItem_Param_Save.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_Param_Save_ToFile,
            this.MenuItem_Param_Save_ToDev});
            this.MenuItem_Param_Save.Image = global::WindowsFormsApp4.Properties.Resources.save;
            this.MenuItem_Param_Save.Name = "MenuItem_Param_Save";
            this.MenuItem_Param_Save.Size = new System.Drawing.Size(322, 26);
            this.MenuItem_Param_Save.Text = "Сохранить";
            // 
            // MenuItem_Param_Save_ToFile
            // 
            this.MenuItem_Param_Save_ToFile.Name = "MenuItem_Param_Save_ToFile";
            this.MenuItem_Param_Save_ToFile.Size = new System.Drawing.Size(184, 26);
            this.MenuItem_Param_Save_ToFile.Text = "В файл";
            this.MenuItem_Param_Save_ToFile.Click += new System.EventHandler(this.MenuItem_Param_Save_ToFile_Click);
            // 
            // MenuItem_Param_Save_ToDev
            // 
            this.MenuItem_Param_Save_ToDev.Name = "MenuItem_Param_Save_ToDev";
            this.MenuItem_Param_Save_ToDev.Size = new System.Drawing.Size(184, 26);
            this.MenuItem_Param_Save_ToDev.Text = "В память МПЧ";
            this.MenuItem_Param_Save_ToDev.Click += new System.EventHandler(this.MenuItem_Param_Save_ToDev_Click);
            // 
            // MenuItem_Param_Load
            // 
            this.MenuItem_Param_Load.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_Param_Load_FromFile,
            this.MenuItem_Param_Load_FromDev});
            this.MenuItem_Param_Load.Image = global::WindowsFormsApp4.Properties.Resources.load;
            this.MenuItem_Param_Load.Name = "MenuItem_Param_Load";
            this.MenuItem_Param_Load.Size = new System.Drawing.Size(322, 26);
            this.MenuItem_Param_Load.Text = "Загрузить";
            // 
            // MenuItem_Param_Load_FromFile
            // 
            this.MenuItem_Param_Load_FromFile.Name = "MenuItem_Param_Load_FromFile";
            this.MenuItem_Param_Load_FromFile.Size = new System.Drawing.Size(195, 26);
            this.MenuItem_Param_Load_FromFile.Text = "Из файла";
            this.MenuItem_Param_Load_FromFile.Click += new System.EventHandler(this.MenuItem_Param_Load_FromFile_Click);
            // 
            // MenuItem_Param_Load_FromDev
            // 
            this.MenuItem_Param_Load_FromDev.Name = "MenuItem_Param_Load_FromDev";
            this.MenuItem_Param_Load_FromDev.Size = new System.Drawing.Size(195, 26);
            this.MenuItem_Param_Load_FromDev.Text = "Из памяти МПЧ";
            this.MenuItem_Param_Load_FromDev.Click += new System.EventHandler(this.MenuItem_Param_Load_FromDev_Click);
            // 
            // MenuItem_Param_MotorSel
            // 
            this.MenuItem_Param_MotorSel.Name = "MenuItem_Param_MotorSel";
            this.MenuItem_Param_MotorSel.Size = new System.Drawing.Size(322, 26);
            this.MenuItem_Param_MotorSel.Text = "Выбрать мотор";
            this.MenuItem_Param_MotorSel.Click += new System.EventHandler(this.MenuItem_Param_MotorSel_Click);
            // 
            // выбратьФайлОписанияНастроекToolStripMenuItem
            // 
            this.выбратьФайлОписанияНастроекToolStripMenuItem.Name = "выбратьФайлОписанияНастроекToolStripMenuItem";
            this.выбратьФайлОписанияНастроекToolStripMenuItem.Size = new System.Drawing.Size(322, 26);
            this.выбратьФайлОписанияНастроекToolStripMenuItem.Text = "Выбрать файл описания настроек";
            this.выбратьФайлОписанияНастроекToolStripMenuItem.Click += new System.EventHandler(this.MenuItem_SetParamDescriptFile_Click);
            // 
            // MenuItem_Loader
            // 
            this.MenuItem_Loader.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_Loader_Flash,
            this.MenuItem_Loader_Verify,
            this.MenuItem_Loader_Reset,
            this.MenuItem_Loader_Stop});
            this.MenuItem_Loader.Image = global::WindowsFormsApp4.Properties.Resources.Loader;
            this.MenuItem_Loader.Name = "MenuItem_Loader";
            this.MenuItem_Loader.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.MenuItem_Loader.Size = new System.Drawing.Size(111, 25);
            this.MenuItem_Loader.Text = "Загрузчик";
            // 
            // MenuItem_Loader_Flash
            // 
            this.MenuItem_Loader_Flash.Name = "MenuItem_Loader_Flash";
            this.MenuItem_Loader_Flash.Size = new System.Drawing.Size(151, 26);
            this.MenuItem_Loader_Flash.Text = "Прошить";
            this.MenuItem_Loader_Flash.Click += new System.EventHandler(this.MenuItem_Loader_Flash_Click);
            // 
            // MenuItem_Loader_Verify
            // 
            this.MenuItem_Loader_Verify.Name = "MenuItem_Loader_Verify";
            this.MenuItem_Loader_Verify.Size = new System.Drawing.Size(151, 26);
            this.MenuItem_Loader_Verify.Text = "Сравнить";
            this.MenuItem_Loader_Verify.Click += new System.EventHandler(this.MenuItem_Loader_Verify_Click);
            // 
            // MenuItem_Loader_Reset
            // 
            this.MenuItem_Loader_Reset.Name = "MenuItem_Loader_Reset";
            this.MenuItem_Loader_Reset.Size = new System.Drawing.Size(151, 26);
            this.MenuItem_Loader_Reset.Text = "Сброс МК";
            this.MenuItem_Loader_Reset.Click += new System.EventHandler(this.MenuItem_Loader_Reset_Click);
            // 
            // MenuItem_Loader_Stop
            // 
            this.MenuItem_Loader_Stop.Name = "MenuItem_Loader_Stop";
            this.MenuItem_Loader_Stop.Size = new System.Drawing.Size(151, 26);
            this.MenuItem_Loader_Stop.Text = "Отмена";
            this.MenuItem_Loader_Stop.Click += new System.EventHandler(this.MenuItem_Loader_Stop_Click);
            // 
            // tableLayoutPanel_Controls
            // 
            this.tableLayoutPanel_Controls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel_Controls.ColumnCount = 1;
            this.tableLayoutPanel_Controls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Controls.Controls.Add(this.tableLayoutPanel_Button, 0, 2);
            this.tableLayoutPanel_Controls.Controls.Add(this.tabForm, 0, 1);
            this.tableLayoutPanel_Controls.Controls.Add(this.tbState, 0, 0);
            this.tableLayoutPanel_Controls.Location = new System.Drawing.Point(253, 3);
            this.tableLayoutPanel_Controls.Name = "tableLayoutPanel_Controls";
            this.tableLayoutPanel_Controls.RowCount = 3;
            this.tableLayoutPanel_Controls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel_Controls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Controls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.tableLayoutPanel_Controls.Size = new System.Drawing.Size(696, 549);
            this.tableLayoutPanel_Controls.TabIndex = 31;
            // 
            // tableLayoutPanel_Button
            // 
            this.tableLayoutPanel_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel_Button.ColumnCount = 5;
            this.tableLayoutPanel_Button.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_Button.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_Button.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_Button.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel_Button.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_Button.Controls.Add(this.btn_Reset, 3, 0);
            this.tableLayoutPanel_Button.Controls.Add(this.btn_Stop, 1, 0);
            this.tableLayoutPanel_Button.Controls.Add(this.btnRun, 0, 0);
            this.tableLayoutPanel_Button.Location = new System.Drawing.Point(3, 494);
            this.tableLayoutPanel_Button.Name = "tableLayoutPanel_Button";
            this.tableLayoutPanel_Button.RowCount = 1;
            this.tableLayoutPanel_Button.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Button.Size = new System.Drawing.Size(690, 52);
            this.tableLayoutPanel_Button.TabIndex = 24;
            // 
            // btn_Reset
            // 
            this.btn_Reset.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Reset.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_Reset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btn_Reset.Location = new System.Drawing.Point(291, 3);
            this.btn_Reset.Name = "btn_Reset";
            this.btn_Reset.Size = new System.Drawing.Size(194, 46);
            this.btn_Reset.TabIndex = 23;
            this.btn_Reset.Text = "Сброс";
            this.btn_Reset.UseVisualStyleBackColor = true;
            this.btn_Reset.Click += new System.EventHandler(this.btn_Reset_Click);
            // 
            // tabForm
            // 
            this.tabForm.Controls.Add(this.tabPage1);
            this.tabForm.Controls.Add(this.tabPage4);
            this.tabForm.Controls.Add(this.tabPage3);
            this.tabForm.Controls.Add(this.tabPage2);
            this.tabForm.Controls.Add(this.tabPage5);
            this.tabForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabForm.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.tabForm.Location = new System.Drawing.Point(3, 53);
            this.tabForm.Name = "tabForm";
            this.tabForm.SelectedIndex = 0;
            this.tabForm.Size = new System.Drawing.Size(690, 435);
            this.tabForm.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabForm.TabIndex = 23;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.gridHRTable);
            this.tabPage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(682, 406);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Параметры";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // gridHRTable
            // 
            this.gridHRTable.AllowUserToAddRows = false;
            this.gridHRTable.AllowUserToDeleteRows = false;
            this.gridHRTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridHRTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.adr,
            this.name,
            this.value,
            this.set,
            this.read});
            this.gridHRTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridHRTable.Location = new System.Drawing.Point(3, 3);
            this.gridHRTable.Name = "gridHRTable";
            this.gridHRTable.RowHeadersVisible = false;
            this.gridHRTable.Size = new System.Drawing.Size(676, 400);
            this.gridHRTable.TabIndex = 22;
            this.gridHRTable.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.GridView_CellBeginEdit);
            this.gridHRTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridView_CellContentClick);
            this.gridHRTable.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridView_CellEndEdit);
            this.gridHRTable.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.GridView_EditingControlShowing);
            // 
            // adr
            // 
            this.adr.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.adr.DefaultCellStyle = dataGridViewCellStyle1;
            this.adr.FillWeight = 60F;
            this.adr.HeaderText = "Адрес";
            this.adr.Name = "adr";
            this.adr.ReadOnly = true;
            this.adr.Width = 62;
            // 
            // name
            // 
            this.name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.name.DefaultCellStyle = dataGridViewCellStyle2;
            this.name.HeaderText = "Параметр";
            this.name.Name = "name";
            this.name.Width = 99;
            // 
            // value
            // 
            this.value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.value.DefaultCellStyle = dataGridViewCellStyle3;
            this.value.HeaderText = "Значение";
            this.value.Name = "value";
            this.value.Width = 98;
            // 
            // set
            // 
            this.set.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.set.HeaderText = "";
            this.set.Name = "set";
            this.set.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // read
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.read.DefaultCellStyle = dataGridViewCellStyle4;
            this.read.HeaderText = "";
            this.read.Name = "read";
            this.read.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.read.Width = 70;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tableLayoutPanel_customControl);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(682, 406);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Управление";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel_customControl
            // 
            this.tableLayoutPanel_customControl.AutoScroll = true;
            this.tableLayoutPanel_customControl.ColumnCount = 2;
            this.tableLayoutPanel_customControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.41679F));
            this.tableLayoutPanel_customControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.58321F));
            this.tableLayoutPanel_customControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_customControl.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_customControl.Margin = new System.Windows.Forms.Padding(10);
            this.tableLayoutPanel_customControl.Name = "tableLayoutPanel_customControl";
            this.tableLayoutPanel_customControl.RowCount = 1;
            this.tableLayoutPanel_customControl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_customControl.Size = new System.Drawing.Size(682, 406);
            this.tableLayoutPanel_customControl.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.gridRelayIO);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(682, 406);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Релейные сигналы";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // gridRelayIO
            // 
            this.gridRelayIO.AllowUserToAddRows = false;
            this.gridRelayIO.AllowUserToDeleteRows = false;
            this.gridRelayIO.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridRelayIO.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.input,
            this.Ivalue,
            this.Out,
            this.Ovalue});
            this.gridRelayIO.Location = new System.Drawing.Point(3, 12);
            this.gridRelayIO.Name = "gridRelayIO";
            this.gridRelayIO.ReadOnly = true;
            this.gridRelayIO.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridRelayIO.Size = new System.Drawing.Size(638, 421);
            this.gridRelayIO.TabIndex = 0;
            // 
            // input
            // 
            this.input.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.input.HeaderText = "Выходы";
            this.input.Name = "input";
            this.input.ReadOnly = true;
            this.input.Width = 84;
            // 
            // Ivalue
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Ivalue.DefaultCellStyle = dataGridViewCellStyle5;
            this.Ivalue.DividerWidth = 10;
            this.Ivalue.HeaderText = "";
            this.Ivalue.Name = "Ivalue";
            this.Ivalue.ReadOnly = true;
            // 
            // Out
            // 
            this.Out.HeaderText = "Входы";
            this.Out.Name = "Out";
            this.Out.ReadOnly = true;
            // 
            // Ovalue
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Ovalue.DefaultCellStyle = dataGridViewCellStyle6;
            this.Ovalue.HeaderText = "";
            this.Ovalue.Name = "Ovalue";
            this.Ovalue.ReadOnly = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtBoxLog);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(682, 406);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Лог";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtBoxLog
            // 
            this.txtBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBoxLog.Location = new System.Drawing.Point(3, 3);
            this.txtBoxLog.Multiline = true;
            this.txtBoxLog.Name = "txtBoxLog";
            this.txtBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBoxLog.Size = new System.Drawing.Size(676, 400);
            this.txtBoxLog.TabIndex = 19;
            // 
            // tabPage5
            // 
            this.tabPage5.Location = new System.Drawing.Point(4, 25);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(682, 406);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Debug";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel_main
            // 
            this.tableLayoutPanel_main.ColumnCount = 2;
            this.tableLayoutPanel_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_main.Controls.Add(this.tableLayoutPanel_Controls, 1, 0);
            this.tableLayoutPanel_main.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_main.Location = new System.Drawing.Point(0, 29);
            this.tableLayoutPanel_main.Name = "tableLayoutPanel_main";
            this.tableLayoutPanel_main.RowCount = 1;
            this.tableLayoutPanel_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_main.Size = new System.Drawing.Size(952, 555);
            this.tableLayoutPanel_main.TabIndex = 32;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel_IR, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1.639344F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 98.36066F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(244, 549);
            this.tableLayoutPanel4.TabIndex = 32;
            this.tableLayoutPanel4.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel4_Paint);
            // 
            // tableLayoutPanel_IR
            // 
            this.tableLayoutPanel_IR.AutoScroll = true;
            this.tableLayoutPanel_IR.ColumnCount = 1;
            this.tableLayoutPanel_IR.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_IR.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_IR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_IR.Location = new System.Drawing.Point(10, 18);
            this.tableLayoutPanel_IR.Margin = new System.Windows.Forms.Padding(10);
            this.tableLayoutPanel_IR.Name = "tableLayoutPanel_IR";
            this.tableLayoutPanel_IR.RowCount = 1;
            this.tableLayoutPanel_IR.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_IR.Size = new System.Drawing.Size(224, 521);
            this.tableLayoutPanel_IR.TabIndex = 34;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(952, 606);
            this.Controls.Add(this.tableLayoutPanel_main);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "Form1";
            this.Text = "Form1";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel_Controls.ResumeLayout(false);
            this.tableLayoutPanel_Controls.PerformLayout();
            this.tableLayoutPanel_Button.ResumeLayout(false);
            this.tabForm.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridHRTable)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridRelayIO)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tableLayoutPanel_main.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsStatus;
        private System.Windows.Forms.TextBox tbState;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.ContextMenuStrip contextMenuForChart;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Refresh;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Refresh_State;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Refresh_Dev;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Refresh_Prog;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Param;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Param_Save;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Param_Save_ToFile;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Param_Save_ToDev;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Param_Load;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Param_Load_FromFile;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Param_Load_FromDev;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_About;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Scope;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Loader;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Loader_Flash;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Loader_Verify;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Loader_Reset;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Loader_Stop;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Scope_Start;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Scope_Stop;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Param_MotorSel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Controls;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_main;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Button;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Connect;
        private System.Windows.Forms.ToolStripMenuItem адресToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_adr;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_RefrTime;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox_RefTime;
        protected System.Windows.Forms.ToolStripMenuItem MenuItem_Connect;
        private System.Windows.Forms.Button btn_Reset;
        private System.Windows.Forms.ToolStripMenuItem выбратьФайлОписанияНастроекToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_IR;
        private System.Windows.Forms.TabControl tabForm;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView gridHRTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn adr;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn value;
        private System.Windows.Forms.DataGridViewTextBoxColumn set;
        private System.Windows.Forms.DataGridViewTextBoxColumn read;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_customControl;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView gridRelayIO;
        private System.Windows.Forms.DataGridViewTextBoxColumn input;
        private System.Windows.Forms.DataGridViewTextBoxColumn Ivalue;
        private System.Windows.Forms.DataGridViewTextBoxColumn Out;
        private System.Windows.Forms.DataGridViewTextBoxColumn Ovalue;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtBoxLog;
        private System.Windows.Forms.TabPage tabPage5;
    }
}

