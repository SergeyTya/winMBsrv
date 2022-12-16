using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp4.loaderutils
{
    interface ISerialPort
    {
        bool OpenPort(string portname, int baudRate);
        bool Write(char[] data);
        bool Write(char data);
        int Read(byte[] data, int maxNumBytesRead);
        int BytesToRead();
        bool IsOpen();
        bool WaitForReadyRead(int timeout);
        bool WaitForReadyRead(int timeout, int size);
        byte[] ReadAll();
        byte[] ReadAll(int timeout);
	    string GetName();
        void Close();
    }
}
