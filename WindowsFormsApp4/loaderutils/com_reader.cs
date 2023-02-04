using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp4.loaderutils;

namespace UnitTestProject1
{
    class Com_Reader: Reader , Iimagereader 
    {
        

        public new void Write(char[] image) {

        }

        public new void Read()
        {
            image.Clear();
            int size = getRequstedSize();
            if (!_isBootloaderReady) {
                cout("Device is not ready");
                return;
            }
            akn();
            cout("Reading " + getRequstedSize() + " byte image from device ");
            int tmp_cnt = size / 256;
            if (size % 256 != 0) {
                tmp_cnt += 1; // if reference image size not multiple 256
            }

            int temp_pr = 0;
            byte[] buf = new byte[256];

            port.ClearRxBuf();
            for (int i = 0; i < tmp_cnt; i++)
            {
                port.Write('V');
                port.WaitForReadyRead(500, 256);
                port.Read(buf, 256);
                image.AddRange(buf);
                int progres = 100 * (i + 1) / (int)(size / 256);
                if ((temp_pr / 10) != (progres / 10))
                {
                    cout(". ");
                    temp_pr = progres;
                }
            }
            cout(image.Count + " byte image read");
        }


        private Win_port port;
        private bool _isBootloaderReady = false;
        private bool _isModbusDevReady = false;
        private string mcu_name;
        private int modbusAdr;

        public Com_Reader(SerialPort serialport, int adr)
        {
            port = new Win_port(serialport);
            modbusAdr = adr;

            string logline = "Searching device " + port.GetName() + " " + port.GetBaudrate() + " " + adr;
            Debug.WriteLine(logline);
            if (port.IsOpen())
            {
                get_loaderID();
                if (!_isBootloaderReady)
                {
                    check_modbus_device();
                    if (_isModbusDevReady)
                    {
                        setup_loader();
                        get_loaderID();
                    }
                }
                if (!_isBootloaderReady)
                {
                    return;
                }
                check_mcu();
                get_devBaseAddr();
                akn();
            }
        }

        private void akn()
        {
            try
            {
                char[] buf = { '0', '0', '0', '0' };
                port.Write('A');
                port.WaitForReadyRead(5);
                //port.Write(buf);
                port.WaitForReadyRead(1);
                port.ReadAll();
            }
            catch (TimeoutException) {
                cout("Akn timeot");
            }
        }

        private bool get_devBaseAddr()
        {
            try
            {
                akn();
                cout("Get MCU flash start addr... ");
                port.WaitForReadyRead(3);
                port.ReadAll();
                port.Write('B');
                string str = port.ReadAlltoStr();
                if (str.Length != 2) {
                    throw new TimeoutException();
                }
                if (str[0] != 'B') {
                    throw new TimeoutException();
                }
                setImageBaseAdr(0);
                return true;
            }
            catch (TimeoutException) {
                cout("0x8000000 default");
                setImageBaseAdr(0x8000000);
                return false;
            }
        }

        private void check_mcu()
        {
            try
            {
                port.ReadAlltoStr();
                cout("Check MCU ... ");
                port.Write('M');
                port.WaitForReadyRead(100, 9);
                mcu_name = port.ReadAlltoStr();
                cout(mcu_name);
            }
            catch (TimeoutException) {
                cout("n/a");
            }
        }

        private void setup_loader()
        {
            if (!_isModbusDevReady)
            {
                return;
            }

            try {
                byte[] req = { (byte)modbusAdr, 0x6, 0x0, 0x0, 0x77, 0x77, 0, 0 };
                crc.Write(req);
                cout("Go to loader ... ");
                port.Write(req);
                port.ReadAll(50);
                cout("ok");
            }
            catch (TimeoutException)
            {
                cout("Timeout");
            }
        }

        private void check_modbus_device()
        {
            if (!port.IsOpen())
            {
                cout("Port is close");
                return;
            }
            if (modbusAdr < 1 || modbusAdr > 255) {
                cout("Wrong modbus address");
                return;
            }
            try {
                port.ReadAll();
                byte[] req = { (byte)modbusAdr, 0x2b, 0xe, 0x1, 0x1, 0, 0 };
                crc.Write(req);
                cout("Searching ModBus device at address " + modbusAdr + " ... ");
                port.Write(req);
                byte[] res = port.ReadAll(100);
                bool crc_chk = crc.Check(res);
                if (res.Length < 4 || crc_chk == false)
                {
                    cout("CRC error !");
                    return;
                }
                cout("ok");
                string str = System.Text.Encoding.UTF8.GetString(res);
                string dev_id = str.Substring(10, 8) + str.Substring(32, 10) + str.Substring(44, 9);
                cout("Device: " + dev_id);
                _isModbusDevReady = true;
            } catch(TimeoutException)
            {
                cout("Timeout");
            }
        }

        private void get_loaderID()
        {
            cout("Setup loader ... ");
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    port.ReadAlltoStr();
                    port.Write('I');
                    port.WaitForReadyRead(500, 12);
                    string str = port.ReadAlltoStr();
                    cout(str);
                    _isBootloaderReady = true;
                    return;
                }
                throw new TimeoutException();
            }
            catch (TimeoutException)
            {
                cout("Get loader ID timeout");
                _isBootloaderReady = false;
            }
        }

    }
}
