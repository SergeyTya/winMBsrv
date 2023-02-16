using ctsServerAdapter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsFormsApp4.FormMain;

namespace WindowsFormsApp4
{

    public partial class FormConnectionSetups : Form
    {

        

        public FormConnectionSetups()
        {
            InitializeComponent();
            read_data();
            this.ShowDialog();
        }

        ConnectionSetups setups = new ConnectionSetups();


        void read_data() {

            setups = ConnectionSetups.read();

            checkBox_Attach.Checked=setups.Attach;
            checkBox_autoconnect.Checked = setups.Aconnect;
            checkBox_devSearch.Checked = setups.DevSearch;
            checkBox_ServerConsole.Checked = setups.RunServerConsole;

            textBox_serverName.Text = setups.ServerName;
            textBox_serverPort.Text = setups.ServerPort.ToString();
            textBox_ModbusAdr.Text = setups.SlaveAdr.ToString();
            comboBox_baud.SelectedIndex = comboBox_baud.Items.IndexOf(setups.BaudRate.ToString());

            comboBox_ComName_DropDown();
            comboBox_ComName.SelectedIndex = comboBox_ComName.Items.IndexOf(setups.ComPortName.ToString()); 

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button_apply_Click(object sender, EventArgs e)
        {
            try
            {
                setups.write();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            this.Close();
        }

        private void button_cancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox_autoconnect_CheckedChanged(object sender, EventArgs e)
        {
            setups.Aconnect =this.checkBox_autoconnect.Checked;
        }

        private void checkBox_devSearch_CheckedChanged(object sender, EventArgs e)
        {
            setups.DevSearch =this.checkBox_devSearch.Checked;
        }

        private void checkBox_Attach_CheckedChanged(object sender, EventArgs e)
        {
            setups.Attach =this.checkBox_Attach.Checked;
        }

        private void checkBox_ServerConsole_CheckedChanged(object sender, EventArgs e)
        {
            setups.RunServerConsole = this.checkBox_ServerConsole.Checked;
        }

        private void textBox_serverPort_TextChanged(object sender, EventArgs e)
        {
            string res = this.textBox_serverPort.Text;
            if (!String.IsNullOrEmpty(res))
            {
                try
                {

                    UInt16 port = Convert.ToUInt16(res);
                    if(port == 0) port = 0;
                    setups.ServerPort = port;
                }
                catch (Exception ex) {
                    Debug.WriteLine(ex);
                    this.textBox_serverPort.Text = "";

                }
            }
        }

        private void textBox_serverName_TextChanged(object sender, EventArgs e)
        {
            setups.ServerName =this.textBox_serverName.Text;
        }

        private void textBox_ModbusAdr_TextChanged(object sender, EventArgs e)
        {
            string res = this.textBox_ModbusAdr.Text;
            if (!String.IsNullOrEmpty(res))
            {
                try
                {

                    UInt16 adr = Convert.ToUInt16(res);
                    if (adr == 0) adr = 1;
                    if (adr > 255) adr = 255;
                    setups.SlaveAdr = adr;
                    this.textBox_ModbusAdr.Text = adr.ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    this.textBox_ModbusAdr.Text = "";

                }
            }

        }

        private void comboBox_baud_SelectedIndexChanged(object sender, EventArgs e)
        {
            string data = comboBox_baud.Items[comboBox_baud.SelectedIndex].ToString();
            int res = (int) Convert.ToInt64(data);
            setups.BaudRate = res;
        }

        private void comboBox_ComName_DropDown(object sender = null, EventArgs e = null)
        {
            comboBox_ComName.Items.Clear();
            List<String> tempPortsList = SerialPort.GetPortNames().ToList();
            if (tempPortsList is null) return;
            foreach (String el in tempPortsList) comboBox_ComName.Items.Add(el);
        }

        private void comboBox_ComName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = this.comboBox_ComName.Text;
            if(name == null) return;
            setups.ComPortName = name;
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            CtsServerAdapter.Close();
        }
    }


    class ConnectionSetups
    {
        public ConnectionSetups(
            bool aconnect = true,
            bool devSearch = true,
            string port = "COM6",
            int baudRate = 9600,
            int slaveAdr = 1,
            string serverName = "localhost",
            int serverPort = 8888,
            bool attach = false,
            bool runServerConsole = false
        )
        {
            Aconnect = aconnect;
            DevSearch = devSearch;
            ComPortName = port;
            BaudRate = baudRate;
            SlaveAdr = slaveAdr;
            ServerName = serverName;
            ServerPort = serverPort;
            Attach = attach;
            RunServerConsole = runServerConsole;
        }

        [JsonProperty("Autoconnect")]
        public bool Aconnect { get; set; }

        [JsonProperty("DevSearch")]
        public bool DevSearch { get; set; }

        [JsonProperty("ComPortName")]
        public string ComPortName { get; set; }

        [JsonProperty("BaudRate")]
        public int BaudRate { get; set; }

        [JsonProperty("SlaveAdr")]
        public int SlaveAdr { get; set; }

        [JsonProperty("ServerName")]
        public string ServerName { get; set; }

        [JsonProperty("ServerPort")]
        public int ServerPort { get; set; }

        [JsonProperty("AttachToRunningServer")]
        public bool Attach { get; set; }

        [JsonProperty("RunServerConsole")]
        public bool RunServerConsole { get; set; }

        public void write()
        {
            string jsonString = JsonConvert.SerializeObject(this);
            File.WriteAllText("connection_setups.json", jsonString);
        }

        public static ConnectionSetups read()
        {
            try
            {
                string jsonString = File.ReadAllText("connection_setups.json", Encoding.Default);
                return JsonConvert.DeserializeObject<ConnectionSetups>(jsonString);
            }
            catch (Exception e)
            {
                ConnectionSetups inst = new ConnectionSetups();
                inst.write();
                Debug.WriteLine(e);
                return inst;
            }
        }
    }

}
