using consoleTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ctsServerAdapter
{
    public static class CtsServerAdapter
    {
        static ConsoleTask task;
        static string host = "localhost";
        static int port = 8888;
        static string serial_name = "com5";
        static int serial_speed = 9600;
        public static StreamReader consoleStreamReader;
        private static bool _isAttached=false;
        public static bool isAttached { get { return _isAttached; } }

        private static bool sendStringToServer(string mes) {
            var buf = Encoding.UTF8.GetBytes(mes);
            try
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(host, port);
                NetworkStream stream = tcpClient.GetStream();
                stream.Write(buf, 0, buf.Length);
                return true;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }

        }


        public static void Start(string host="localhost", int port=8888, string serial_name="com6", int serial_speed=9600, bool createWindow = false)
        {
            _isAttached = false;
            if (isAlaive() == false)
            {
                CtsServerAdapter.host = host;
                CtsServerAdapter.port = port;
                if (isAlaive() == false)
                {
                    CtsServerAdapter.serial_name = serial_name;
                    CtsServerAdapter.serial_speed = serial_speed;
                    string param = String.Format("--host {0} --port {1} --serial {2} --speed {3}", host, port, serial_name, serial_speed);
                    task = new ConsoleTask("CTS_server.exe", param, consoleStreamReader, createWindow);
                }
            }


        }

        public static void Close() {
            //sendStringToServer("close");
            var buf = Encoding.UTF8.GetBytes("close");
            try
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(host, port);
                NetworkStream stream = tcpClient.GetStream();
                stream.Write(buf, 0, buf.Length);

            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public static bool isAlaive() {
            if (task == null) {
                return false;
            }
            return task.IsRunning();

        }

        public static bool isAnyServer(string host= "localhost", int port = 8888) {
            try
            {

                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(host, port);
                NetworkStream stream = tcpClient.GetStream();
                var buf = Encoding.UTF8.GetBytes("info");
                stream.Write(buf, 0, buf.Length);

                byte[] tmp = new byte[1024];
                stream.Read(tmp, 0, 1024);
                Debug.WriteLine(System.Text.Encoding.UTF8.GetString(tmp));
                Debug.WriteLine("");
                stream.Close();
                tcpClient.Close();
                return true;

            }
            catch (System.Net.Sockets.SocketException ex)
            {

                return false;
            }
            catch (System.IO.IOException ex)
            {
                return false;
            }

        }

        public static void GetComInfo()
        {


        }


    }

}
