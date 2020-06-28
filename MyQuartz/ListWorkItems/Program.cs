using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListWorkItems
{
    class Program
    {
        static void Main(string[] args)
        {
            //WorkItemHelper.List(new string[] { "http://172.18.128.166:8080/medicalhealthsy", "$/ClinicalManagement" });
            //Console.WriteLine("Press any key...");
            //Console.ReadKey();

            if (args.Length < 1)
            {
                Console.WriteLine("Usage: SFW.exe <FolderName>");
            }
            FileHelper.SetFilesWritable(args[0], System.IO.SearchOption.AllDirectories);
        }
    }
}
