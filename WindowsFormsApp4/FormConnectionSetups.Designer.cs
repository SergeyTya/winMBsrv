namespace WindowsFormsApp4
{
    partial class FormConnectionSetups
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBox_autoconnect = new System.Windows.Forms.CheckBox();
            this.checkBox_devSearch = new System.Windows.Forms.CheckBox();
            this.checkBox_Attach = new System.Windows.Forms.CheckBox();
            this.textBox_ModbusAdr = new System.Windows.Forms.TextBox();
            this.textBox_serverName = new System.Windows.Forms.TextBox();
            this.textBox_serverPort = new System.Windows.Forms.TextBox();
            this.comboBox_baud = new System.Windows.Forms.ComboBox();
            this.comboBox_ComName = new System.Windows.Forms.ComboBox();
            this.button_cancle = new System.Windows.Forms.Button();
            this.button_apply = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBox_ServerConsole = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.button_stop = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.92698F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.07302F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.checkBox_autoconnect, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkBox_devSearch, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBox_Attach, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.textBox_ModbusAdr, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.textBox_serverName, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.textBox_serverPort, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.comboBox_baud, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.comboBox_ComName, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.checkBox_ServerConsole, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.button_apply, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.button_cancle, 1, 11);
            this.tableLayoutPanel1.Controls.Add(this.label10, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.button_stop, 1, 10);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 12;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(408, 284);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Автосоединение";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(3, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Поиск устройства";
            this.label2.Click += new System.EventHandler(this.label1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(3, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "Порт";
            this.label3.Click += new System.EventHandler(this.label1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(3, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "Скорость";
            this.label4.Click += new System.EventHandler(this.label1_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(3, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(118, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "Адрес Modbus";
            this.label5.Click += new System.EventHandler(this.label1_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(3, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 20);
            this.label6.TabIndex = 0;
            this.label6.Text = "Сервер";
            this.label6.Click += new System.EventHandler(this.label1_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(3, 146);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 20);
            this.label7.TabIndex = 0;
            this.label7.Text = "Порт";
            this.label7.Click += new System.EventHandler(this.label1_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(3, 172);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(167, 20);
            this.label8.TabIndex = 0;
            this.label8.Text = "Не запускать сервер";
            this.label8.Click += new System.EventHandler(this.label1_Click);
            // 
            // checkBox_autoconnect
            // 
            this.checkBox_autoconnect.AutoSize = true;
            this.checkBox_autoconnect.Location = new System.Drawing.Point(214, 3);
            this.checkBox_autoconnect.Name = "checkBox_autoconnect";
            this.checkBox_autoconnect.Size = new System.Drawing.Size(15, 14);
            this.checkBox_autoconnect.TabIndex = 2;
            this.checkBox_autoconnect.UseVisualStyleBackColor = true;
            this.checkBox_autoconnect.CheckedChanged += new System.EventHandler(this.checkBox_autoconnect_CheckedChanged);
            // 
            // checkBox_devSearch
            // 
            this.checkBox_devSearch.AutoSize = true;
            this.checkBox_devSearch.Location = new System.Drawing.Point(214, 23);
            this.checkBox_devSearch.Name = "checkBox_devSearch";
            this.checkBox_devSearch.Size = new System.Drawing.Size(15, 14);
            this.checkBox_devSearch.TabIndex = 2;
            this.checkBox_devSearch.UseVisualStyleBackColor = true;
            this.checkBox_devSearch.CheckedChanged += new System.EventHandler(this.checkBox_devSearch_CheckedChanged);
            // 
            // checkBox_Attach
            // 
            this.checkBox_Attach.AutoSize = true;
            this.checkBox_Attach.Cursor = System.Windows.Forms.Cursors.Cross;
            this.checkBox_Attach.Location = new System.Drawing.Point(214, 175);
            this.checkBox_Attach.Name = "checkBox_Attach";
            this.checkBox_Attach.Size = new System.Drawing.Size(15, 14);
            this.checkBox_Attach.TabIndex = 2;
            this.checkBox_Attach.UseVisualStyleBackColor = true;
            this.checkBox_Attach.CheckedChanged += new System.EventHandler(this.checkBox_Attach_CheckedChanged);
            // 
            // textBox_ModbusAdr
            // 
            this.textBox_ModbusAdr.Location = new System.Drawing.Point(214, 97);
            this.textBox_ModbusAdr.Name = "textBox_ModbusAdr";
            this.textBox_ModbusAdr.Size = new System.Drawing.Size(100, 20);
            this.textBox_ModbusAdr.TabIndex = 3;
            this.textBox_ModbusAdr.TextChanged += new System.EventHandler(this.textBox_ModbusAdr_TextChanged);
            // 
            // textBox_serverName
            // 
            this.textBox_serverName.Location = new System.Drawing.Point(214, 123);
            this.textBox_serverName.Name = "textBox_serverName";
            this.textBox_serverName.Size = new System.Drawing.Size(100, 20);
            this.textBox_serverName.TabIndex = 3;
            this.textBox_serverName.TextChanged += new System.EventHandler(this.textBox_serverName_TextChanged);
            // 
            // textBox_serverPort
            // 
            this.textBox_serverPort.Location = new System.Drawing.Point(214, 149);
            this.textBox_serverPort.Name = "textBox_serverPort";
            this.textBox_serverPort.Size = new System.Drawing.Size(100, 20);
            this.textBox_serverPort.TabIndex = 3;
            this.textBox_serverPort.TextChanged += new System.EventHandler(this.textBox_serverPort_TextChanged);
            // 
            // comboBox_baud
            // 
            this.comboBox_baud.FormattingEnabled = true;
            this.comboBox_baud.Items.AddRange(new object[] {
            "9600",
            "38400",
            "115200",
            "128000",
            "230400"});
            this.comboBox_baud.Location = new System.Drawing.Point(214, 70);
            this.comboBox_baud.Name = "comboBox_baud";
            this.comboBox_baud.Size = new System.Drawing.Size(121, 21);
            this.comboBox_baud.TabIndex = 4;
            this.comboBox_baud.SelectedIndexChanged += new System.EventHandler(this.comboBox_baud_SelectedIndexChanged);
            // 
            // comboBox_ComName
            // 
            this.comboBox_ComName.FormattingEnabled = true;
            this.comboBox_ComName.Location = new System.Drawing.Point(214, 43);
            this.comboBox_ComName.Name = "comboBox_ComName";
            this.comboBox_ComName.Size = new System.Drawing.Size(121, 21);
            this.comboBox_ComName.TabIndex = 4;
            this.comboBox_ComName.DropDown += new System.EventHandler(this.comboBox_ComName_DropDown);
            this.comboBox_ComName.SelectedIndexChanged += new System.EventHandler(this.comboBox_ComName_SelectedIndexChanged);
            // 
            // button_cancle
            // 
            this.button_cancle.Location = new System.Drawing.Point(214, 243);
            this.button_cancle.Name = "button_cancle";
            this.button_cancle.Size = new System.Drawing.Size(87, 27);
            this.button_cancle.TabIndex = 1;
            this.button_cancle.Text = "Отмена";
            this.button_cancle.UseVisualStyleBackColor = true;
            this.button_cancle.Click += new System.EventHandler(this.button_cancle_Click);
            // 
            // button_apply
            // 
            this.button_apply.Location = new System.Drawing.Point(3, 243);
            this.button_apply.Name = "button_apply";
            this.button_apply.Size = new System.Drawing.Size(91, 27);
            this.button_apply.TabIndex = 1;
            this.button_apply.Text = "Принять";
            this.button_apply.UseVisualStyleBackColor = true;
            this.button_apply.Click += new System.EventHandler(this.button_apply_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(3, 192);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(156, 20);
            this.label9.TabIndex = 0;
            this.label9.Text = "Включить консоль сервера";
            this.label9.Click += new System.EventHandler(this.label1_Click);
            // 
            // checkBox_ServerConsole
            // 
            this.checkBox_ServerConsole.AutoSize = true;
            this.checkBox_ServerConsole.Cursor = System.Windows.Forms.Cursors.Default;
            this.checkBox_ServerConsole.Location = new System.Drawing.Point(214, 195);
            this.checkBox_ServerConsole.Name = "checkBox_ServerConsole";
            this.checkBox_ServerConsole.Size = new System.Drawing.Size(15, 14);
            this.checkBox_ServerConsole.TabIndex = 2;
            this.checkBox_ServerConsole.UseVisualStyleBackColor = true;
            this.checkBox_ServerConsole.CheckedChanged += new System.EventHandler(this.checkBox_ServerConsole_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.Location = new System.Drawing.Point(3, 212);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(158, 20);
            this.label10.TabIndex = 0;
            this.label10.Text = "Остановить сервер";
            this.label10.Click += new System.EventHandler(this.label1_Click);
            // 
            // button_stop
            // 
            this.button_stop.Location = new System.Drawing.Point(214, 215);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(58, 22);
            this.button_stop.TabIndex = 1;
            this.button_stop.Text = "Стоп";
            this.button_stop.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // FormConnectionSetups
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 284);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormConnectionSetups";
            this.Text = "Настройки соединения";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button_apply;
        private System.Windows.Forms.Button button_cancle;
        private System.Windows.Forms.CheckBox checkBox_autoconnect;
        private System.Windows.Forms.CheckBox checkBox_devSearch;
        private System.Windows.Forms.CheckBox checkBox_Attach;
        private System.Windows.Forms.TextBox textBox_ModbusAdr;
        private System.Windows.Forms.TextBox textBox_serverName;
        private System.Windows.Forms.TextBox textBox_serverPort;
        private System.Windows.Forms.ComboBox comboBox_baud;
        private System.Windows.Forms.ComboBox comboBox_ComName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox checkBox_ServerConsole;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button_stop;
    }
}