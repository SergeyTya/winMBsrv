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
    public class CtsServerAdapter
    {
        ConsoleTask task;
        string host = "localhost";
        int port = 8888;
        System.Windows.Forms.TextBox tb = new TextBox();


        public CtsServerAdapter(TextBox output) {
          //  tb = output;
            Start();
        }

        public void Start()
        {
            if (isAlaive() == false)
            {
                task = new ConsoleTask("CTS_server.exe", "--serial COM6", tb, false);
            }
        }

        public void Close() {
            var buf = Encoding.UTF8.GetBytes("close");
            try
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(host, port);
                NetworkStream stream = tcpClient.GetStream();
                stream.Write(buf, 0, buf.Length);
            }
            catch (System.Net.Sockets.SocketException ex) { 
            }
        }

        public bool isAlaive() {

            try {

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
            catch(System.Net.Sockets.SocketException ex) {

                return false;
            }
           
        }


    }

}
