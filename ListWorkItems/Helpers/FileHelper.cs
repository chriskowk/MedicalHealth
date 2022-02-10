using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListWorkItems
{
    public static class FileHelper
    {
        public static void SetFilesWritable(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: SFW.exe <FolderName>");
                Console.WriteLine("提示：文件夹路径如有空格需用双引号括起来！");
                return;
            }
            SetFilesWritable(args[0], SearchOption.AllDirectories);
        }

        private static void SetFilesWritable(string foldername, SearchOption searchOption)
        {
            if (!Directory.Exists(foldername))
            {
                Console.WriteLine($"路径 {foldername} 不存在！");
                return;
            }

            DirectoryInfo folder = new DirectoryInfo(foldername);
            FileInfo[] fileInfos = folder.GetFiles("*", searchOption);
            foreach (FileInfo fi in fileInfos)
            {
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                {
                    fi.Attributes = FileAttributes.Normal;
                    Console.WriteLine($"文件 {fi.FullName} 已改可写");
                }
            }
        }

    }
}
