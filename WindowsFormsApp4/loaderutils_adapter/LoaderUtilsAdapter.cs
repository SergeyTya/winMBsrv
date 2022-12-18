using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoaderUtilsAdapter
{
    static class LoaderUtilsAdapter
    {

        static Process console = null;
        static StreamWriter consoleStreamWriter = null;
        delegate void UpdateConsoleWindowDelegate(String msg);
        private static System.Windows.Forms.TextBox textBox;

        private static void ConsoleOutputHandler(object sendingProcess, DataReceivedEventArgs recieved)
        {
            if (!String.IsNullOrEmpty(recieved.Data))
            {
                UpdateConsoleWindow(recieved.Data + "\r\n");
            }
        }

        private static void UpdateConsoleWindow(String message)
        {
            if (textBox.InvokeRequired)
            {
                UpdateConsoleWindowDelegate update = new UpdateConsoleWindowDelegate(UpdateConsoleWindow);
                textBox.Invoke(update, message);
            }
            else
            {
                textBox.AppendText(message);

            }
        }

        private static bool IsRunning(this Process process)
        {
            if (process == null)
                return false;

            try
            {
                Process.GetProcessById(process.Id);
            }
            catch (ArgumentException)
            {
                return false;
            }
            Debug.WriteLine("Loader is running");
            return true;
        }

        private static void exec_cmd(string name, string arg) {
            if (IsRunning(console)) return;
            UpdateConsoleWindow(name+"\r\n");
            console = new Process();
            console.StartInfo.FileName = "loader_util.exe";
            console.StartInfo.UseShellExecute = false;
            console.StartInfo.CreateNoWindow = true;
            console.StartInfo.RedirectStandardInput = true;
            console.StartInfo.RedirectStandardOutput = true;
            console.OutputDataReceived += new DataReceivedEventHandler(ConsoleOutputHandler);
            console.StartInfo.Arguments = arg;
            console.Start();
            consoleStreamWriter = console.StandardInput;
            console.BeginOutputReadLine();
        }

        public static void SetOutput(System.Windows.Forms.TextBox tb) {
            textBox = tb;
        }

        public static void Version() {
            exec_cmd("Loader_utils", "-v");
        }

        public static void Help() {
            exec_cmd("Help", "-?");
        }

        public static void Write(string src, string dst)
        {
            string arg = "-w " + src + " " + dst;
            exec_cmd("Прошиваю:", arg);
        }

        public static void Compare(string src, string dst)
        {
            string arg = "-c " + src + " " + dst;
            exec_cmd("Сравниваю:", arg);
        }

        public static void Reset(string dst)
        {
            string arg = "-r " + dst;
            exec_cmd("Сброс.", "-?");
        }
    }
}
