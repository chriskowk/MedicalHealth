using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSideKicks
{
    public static class WinRARHelper
    {
        /// <summary>
        /// 压缩sourceFilePath文件夹及其下文件；默认压缩方式.rar
        /// </summary>
        /// <param name="sourceFullPath"></param>
        /// <param name="destFileFullName"></param>
        /// <param name="isFile"></param>
        public static void Compress(string sourceFullPath, string destFileFullName, bool isFile = false)
        {
            if (File.Exists(destFileFullName)) File.Delete(destFileFullName);

            string opt = isFile ? "-ep" : "-r -ep1";  //开关：-r 含子文件夹；-ep 忽略路径信息；-ep1 表示忽略被压缩的根文件夹（还要配合根文件夹名称附上\*.*）；-ed 忽略空文件夹
            string filename = Path.Combine("C:\\Program Files\\WinRAR", "WinRAR.exe");
            using (Process pro = new Process())
            {
                FileInfo file = new FileInfo(filename);
                pro.StartInfo.Arguments = $" a {opt} {destFileFullName} {sourceFullPath}\\*.*";  //加上\*.*的目的是压缩文件里不希望出现根文件夹（且启用开关-ep1）
                pro.StartInfo.WorkingDirectory = file.Directory.FullName;
                pro.StartInfo.FileName = filename;
                pro.StartInfo.CreateNoWindow = false;
                pro.StartInfo.UseShellExecute = false;

                pro.Start();
                pro.WaitForExit();
            }
        }
        /// <summary>
        /// 将压缩文件sourceFileFullName解压到destFileFullPath文件夹下
        /// </summary>
        /// <param name="sourceFileFullName"></param>
        /// <param name="destFullPath"></param>
        public static void Decompress(string sourceFileFullName, string destFullPath)
        {
            if (!File.Exists(sourceFileFullName)) return;
            if (!Directory.Exists(destFullPath)) Directory.CreateDirectory(destFullPath);

            string filename = Path.Combine("C:\\Program Files\\WinRAR", "WinRAR.exe");
            using (Process pro = new Process())
            {
                FileInfo file = new FileInfo(filename);
                pro.StartInfo.Arguments = $" x {sourceFileFullName} {destFullPath}";
                pro.StartInfo.WorkingDirectory = file.Directory.FullName;
                pro.StartInfo.FileName = filename;
                pro.StartInfo.CreateNoWindow = false;
                pro.StartInfo.UseShellExecute = false;

                pro.Start();
                pro.WaitForExit();
            }
        }
    }
}
