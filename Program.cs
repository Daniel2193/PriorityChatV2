using System;
using System.IO;
using System.Windows.Forms;

namespace PriorityChatV2
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        /// 

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;
        [STAThread]
        static void Main(string[] args)
        {
            string version = "2.4.3";
            if (args.Length == 1){
                if(args[0] == "--version" || args[0].Equals("-v"))
                {
                    var stdout = Console.OpenStandardOutput();
                    StreamWriter sw = new StreamWriter(stdout);
                    sw.AutoFlush = true;
                    AttachConsole(ATTACH_PARENT_PROCESS);
                    sw.WriteLine(version);
                    Console.WriteLine(version);
                    return;
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormChat(version));
        }
        
    }
}
