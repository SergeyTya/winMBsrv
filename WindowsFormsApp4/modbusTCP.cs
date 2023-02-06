using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WindowsFormsApp9
{
    class ModbusTCPserver
    {
        Socket socket;

        public ModbusTCPserver()
        {

           


        }

        public async void Connect() {

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                var port = 8888;
                var url = "localhodt";
                // пытаемся подключиться используя URL-адрес и 
                await socket.ConnectAsync(url, port);
                Console.WriteLine($"Подключение к {url} установлено");
            }
            catch (SocketException)
            {
                Console.WriteLine($"Не удалось установить подключение");
            }
        }

        public async void SlavePollAsync(int delay)
        {
            Debug.WriteLine("-->");
            while (true)
            {
                byte[] data = { 0x01, 0x06, 0, 0, 0 , 9 };
                int bytesSent = await socket.SendT;

                await Task.Delay(delay);
            }
        }
    }
}
