using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckVersion
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: CheckVersion.exe [/D]");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Desktop desktop = new Desktop();
            if (args.Length == 1 && string.Equals(args[0], "/D", StringComparison.OrdinalIgnoreCase))
                desktop.DevelopMode = true;

            Application.Run(desktop);
        }
    }
}
