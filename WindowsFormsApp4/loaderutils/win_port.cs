using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp4.loaderutils
{
    class Win_port : ISerialPort
    {
        public SerialPort sport = new SerialPort();

        public Win_port(SerialPort serialport) {
            sport = serialport;
        }

        public int BytesToRead()
        {
            if (!sport.IsOpen)
            {
                return 0;
            }
            return sport.BytesToRead;
        }

        public void Close()
        {
            sport.Close();
        }

        public string GetName()
        {
            return sport.PortName;
        }

        public int GetBaudrate()
        {
            return sport.BaudRate;
        }

        public bool IsOpen()
        {
            return sport.IsOpen;
        }

        public bool OpenPort(string portname, int baudRate)
        {
            sport.Open();
            return sport.IsOpen; 
        }

        public int Read(byte[] data, int maxNumBytesRead)
        {
            if (sport.IsOpen)
            {
                sport.ReadTimeout = 1;
                return sport.Read(data, 0, maxNumBytesRead);
            }else {
                return 0;
            }
            
        }

        public byte[] ReadAll()
        {
            if (sport.IsOpen)
            {

                int cnt = sport.BytesToRead;
                byte[] res = new byte[cnt];
                sport.Read(res, 0, cnt);
                return res;
            }
            else {
                return null;
            }
        }

        public string ReadAlltoStr() {

            //return System.Text.Encoding.UTF8.GetString(ReadAll());
            return sport.ReadExisting();
        }


        public string ClearRxBuf()
        {

            //return System.Text.Encoding.UTF8.GetString(ReadAll());
            return sport.ReadExisting();
        }

        public byte[] ReadAll(int timeout)
        {
            System.Threading.Thread.Sleep(timeout);
            return ReadAll();
        }

        public bool WaitForReadyRead(int timeout)
        {
            if (sport.IsOpen)
            {
                System.Threading.Thread.Sleep(timeout);
                if (sport.BytesToRead > 0) {
                    return true;
                }
                throw new TimeoutException();
            }
            return false;
        }

        public bool WaitForReadyRead(int timeout, int size)
        {
            if (!sport.IsOpen) {
                return false;
            }

            for (int i = 0; i < timeout; i++) {
                System.Threading.Thread.Sleep(1);
                if (sport.BytesToRead == size)
                {
                    return true;
                }
            }

            string res = sport.ReadExisting();
            Debug.WriteLine("WaitForReadyRead Timeout " +res);
            throw new TimeoutException();
        }

        public bool Write(byte[] data)
        {
            if (!sport.IsOpen)
            {
                return false;
            }

            sport.Write(data, 0 , data.Length);
           
           // Debug.WriteLine("Write to port " + crc.sBtoS(data));

            return true;
        }

        public bool Write(char[] data) {

            if (!sport.IsOpen)
            {
                return false;
            }

            sport.Write(data, 0, data.Length);

            // Debug.WriteLine("Write to port " + crc.sBtoS(data));

            return true;

        }

        public bool Write(char data)
        {
            if (!sport.IsOpen)
            {
                return false;
            }

            sport.Write(data.ToString());
            return true;
        }
    }

}
