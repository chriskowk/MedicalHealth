using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckProcService
{
    public static class ProcessHelper
    {
        public static string Execute(string filefullname, string arguments, bool redirectStandardOutput, bool redirectStandardError, ref string errMsg)
        {
            return Execute(filefullname, arguments, 0, redirectStandardOutput, redirectStandardError, ref errMsg);
        }

        public static string Execute(string filefullname, string arguments, int seconds, bool redirectStandardOutput, bool redirectStandardError, ref string errMsg)
        {
            string output = ""; //输出字符串  
            if (filefullname != null && !filefullname.Equals(""))
            {
                FileInfo file = new FileInfo(filefullname);
                Process process = new Process();//创建进程对象  
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";//设定需要执行的命令  
                startInfo.WorkingDirectory = file.Directory.FullName;
                startInfo.Arguments = $"/C \"{filefullname}\" {arguments}"; //“/C”表示执行完命令后马上退出  
                startInfo.UseShellExecute = false;//使用系统外壳程序启动  
                startInfo.RedirectStandardInput = false;//不重定向输入  
                startInfo.RedirectStandardOutput = redirectStandardOutput; //重定向输出  
                startInfo.RedirectStandardError = redirectStandardError;
                startInfo.CreateNoWindow = false;//创建窗口  
                process.StartInfo = startInfo;
                try
                {
                    if (process.Start())//开始进程  
                    {
                        if (seconds == 0)
                        {
                            process.WaitForExit();//这里无限等待进程结束  
                        }
                        else
                        {
                            process.WaitForExit(seconds * 1000); //等待进程结束，等待时间为指定的毫秒  
                        }
                        output = process.StartInfo.RedirectStandardOutput ? process.StandardOutput.ReadToEnd() : string.Empty;
                        errMsg = process.StartInfo.RedirectStandardError ? process.StandardError.ReadToEnd() : string.Empty;
                    }
                }
                catch
                {
                }
                finally
                {
                    if (process != null)
                        process.Close();
                }
            }
            return output;
        }

        public static string ExecBatch(string batPath, bool useShellExecute, bool redirectStandardOutput, bool redirectStandardError, string arguments, ref string errMsg)
        {
            string outputString = string.Empty;

            using (Process pro = new Process())
            {
                FileInfo file = new FileInfo(batPath);
                ProcessStartInfo psi = new ProcessStartInfo(batPath, arguments)
                {
                    WorkingDirectory = file.Directory.FullName,
                    CreateNoWindow = false,
                    RedirectStandardOutput = redirectStandardOutput,
                    RedirectStandardError = redirectStandardError,
                    UseShellExecute = useShellExecute
                };
                pro.StartInfo = psi;
                pro.Start();
                pro.WaitForExit();

                outputString = redirectStandardOutput ? pro.StandardOutput.ReadToEnd() : string.Empty;
                errMsg = redirectStandardError ? pro.StandardError.ReadToEnd() : string.Empty;
            }
            return outputString;
        }
    }
}
