using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PriorityChatV2
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            string version = "2.5.0";
            if (args.Length == 1){
                if(args[0] == "--version" || args[0] == "-v"){
                    Console.WriteLine(version);
                    return 0;
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormChat(version));
            return 0;
        }
    }
}
