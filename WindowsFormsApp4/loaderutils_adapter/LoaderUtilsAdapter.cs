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
                UpdateConsoleWindow("   "+recieved.Data + Environment.NewLine);
            }
        }

        private static void ConsoleErrorHandler(object sendingProcess, DataReceivedEventArgs recieved)
        {
            if (!String.IsNullOrEmpty(recieved.Data))
            {
                UpdateConsoleWindow(Environment.NewLine+ recieved.Data + Environment.NewLine + Environment.NewLine);
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

        public static bool IsRunning()
        {
            if (console == null)
                return false;

            try
            {
                Process.GetProcessById(console.Id);
            }
            catch (Exception)
            {
                return false;
            }
            Debug.WriteLine("Loader is running");
            return true;
        }

        private static void exec_cmd( string arg) {
            if (IsRunning()) return;
            UpdateConsoleWindow(Environment.NewLine);
            console = new Process();
            console.StartInfo.FileName = "loader_util.exe";
            console.StartInfo.UseShellExecute = false;
            console.StartInfo.CreateNoWindow = true;
            console.StartInfo.RedirectStandardInput = true;
            console.StartInfo.RedirectStandardOutput = true;
            console.StartInfo.RedirectStandardError = true;
            console.OutputDataReceived += new DataReceivedEventHandler(ConsoleOutputHandler);
            console.ErrorDataReceived += new DataReceivedEventHandler(ConsoleErrorHandler);
            console.StartInfo.Arguments = arg;
            try {
                console.Start();
                consoleStreamWriter = console.StandardInput;
                console.BeginOutputReadLine();
                console.BeginErrorReadLine();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                UpdateConsoleWindow("Файл loaderutil.exe отсутсвует!");
            }
        }

        public static void SetOutput(System.Windows.Forms.TextBox tb) {
            textBox = tb;
        }

        public static void Version() {
            exec_cmd( "-v");
        }

        public static void Help() {
            exec_cmd( "-?");
        }

        public static void Write(string src, string dst)
        {
            string arg = "-w " + src + " " + dst;
            exec_cmd( arg);
        }

        public static void Compare(string src, string dst)
        {
            string arg = "-c " + src + " " + dst;
            exec_cmd( arg);
        }

        public static void ResetMCU(string dst)
        {
            string arg = "-r " + dst;
            exec_cmd(arg);
        }

        public static void Abort()
        {
            if (IsRunning()) {
                console.Kill();
                UpdateConsoleWindow("Отмена!");
            }

        }
    }
}

