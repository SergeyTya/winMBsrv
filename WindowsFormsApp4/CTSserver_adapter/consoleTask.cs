using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace consoleTask
{
    internal class ConsoleTask
    {
        private Process console = null;
        private StreamWriter consoleStreamWriter = null;

        private delegate void UpdateConsoleWindowDelegate(String msg);
        string name;

        public ConsoleTask(string exec_name, string arg, StreamReader consoleStreamWReader, bool create_windows=true)
        {
            name = exec_name;
            console = new Process();
            console.StartInfo.FileName = exec_name;

            if (create_windows == false)
            {
                console.StartInfo.UseShellExecute = false;
                console.StartInfo.CreateNoWindow = true;
                //console.StartInfo.RedirectStandardInput = true;
                //console.StartInfo.RedirectStandardOutput = true;
                //console.StartInfo.RedirectStandardError = true;
                //console.OutputDataReceived += new DataReceivedEventHandler(ConsoleOutputHandler);
                //console.ErrorDataReceived += new DataReceivedEventHandler(ConsoleErrorHandler);
               
            }
            else {
                console.StartInfo.UseShellExecute = true;
                console.StartInfo.CreateNoWindow = false;
            }

            try
            {
                console.StartInfo.Arguments = arg;
                console.Start();
                //if (redirect_output) {
                //    consoleStreamWriter = console.StandardInput;
                //    consoleStreamWReader = console.StandardOutput;
                //    console.BeginOutputReadLine();
                //    console.BeginErrorReadLine();
                //}
            }
            catch (System.ComponentModel.Win32Exception)
            {
                UpdateConsoleWindow(String.Format("Файл {0} отсутсвует!", exec_name));
            }
        }

        private void ConsoleOutputHandler(object sendingProcess, DataReceivedEventArgs recieved)
        {
            if (!String.IsNullOrEmpty(recieved.Data))
            {
                UpdateConsoleWindow("   " + recieved.Data + Environment.NewLine);
            }
        }

        private void ConsoleErrorHandler(object sendingProcess, DataReceivedEventArgs recieved)
        {
            if (!String.IsNullOrEmpty(recieved.Data))
            {
                UpdateConsoleWindow(Environment.NewLine + recieved.Data + Environment.NewLine + Environment.NewLine);
            }
        }

        private void UpdateConsoleWindow(String message)
        {
            Debug.WriteLine(message);
        }

        public bool IsRunning()
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
            Debug.WriteLine("Process is running");
            return true;
        }

        public static void Abort(ConsoleTask task)
        {
            //task.console.Kill();
            task.consoleStreamWriter.Close();
            foreach (var process in Process.GetProcessesByName(task.name))
            {
                process.Kill();
            }

            task.console = null;
        }
    }
}
