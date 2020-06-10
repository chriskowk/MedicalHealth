using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using RabbitMQ.Client;
using System.Threading.Tasks;

namespace MyJob
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public abstract class TaskJobBase : IJob
    {
        // 定义参数常量
        public const string RegisterFilePath = "RegisterFilePath";
        public const string BASE_PATH = "BASE_PATH";
        public const string SQL = "SQL";
        public const string ExecutionCount = "ExecutionCount";
        public const string RowCount = "RowCount";
        // Quartz 每次执行时都会重新实例化一个类, 因此Job类中的非静态变量不能存储状态信息
        private static int _counter = 1;//可以保存状态

        public string BatchFilesPath { get; private set; }
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                JobKey jobKey = context.JobDetail.Key;
                // 获取传递过来的参数            
                JobDataMap data = context.JobDetail.JobDataMap;
                string regFilePath = data.GetString(RegisterFilePath);
                string sql = data.GetString(SQL);
                int count = data.GetInt(ExecutionCount);
                string basepath = data.GetString(BASE_PATH);
                BatchFilesPath = Path.Combine(basepath, "BatchFiles");

                //Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\JetSun\3.0\Quartz\Job\" + this.GetType().ToString(), "State", "STATE_START");
                // 此处为执行的任务
                TFGetLatestVersion();
                BuildAll(regFilePath);
                RebuildDataModels();

                //Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\JetSun\3.0\Quartz\Job\" + this.GetType().ToString(), "State", "STATE_COMPLETE");
                SendMessageQueue(jobKey.Name);

                data.Put(RowCount, 5);
                data.Put(ExecutionCount, ++count);
                _counter++;
            });
        }

        private void SendMessageQueue(string message)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";//RabbitMQ服务在本地运行
            factory.UserName = "guest";//用户名
            factory.Password = "guest";//密码 
            using (var connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "TaskFinishedMessage", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.ExchangeDeclare(exchange: "TaskFinishedMessageExchange", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: "TaskFinishedMessage", exchange: "TaskFinishedMessageExchange", routingKey: string.Empty, arguments: null);

                    //string message = string.Format("STATE_COMPLETE ON {0:yyyy-MM-dd HH:mm:ss.fff}", DateTime.Now);
                    byte[] body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "TaskFinishedMessageExchange",
                                    routingKey: string.Empty,
                                    basicProperties: null,
                                    body: body);
                }
            }
        }

        private void TFGetLatestVersion()
        {
            string path = Path.Combine(BatchFilesPath, "TF_GET_MedicalHealth.bat");
            if (!File.Exists(path)) return;

            string errMsg = string.Empty;
            string output = JobHelper.Execute(path, string.Empty, 60, true, true, ref errMsg);

            FileStream fs = new FileStream(string.Format(@"{0}\Log\TFGetLog{1}.txt", BatchFilesPath, DateTime.Now.ToString("yyyyMMddHHmmss")), FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(string.Format("Output: {0}\r\nErrorMsg:\r\n{1}", output, errMsg));
            sw.Close();
            fs.Close();
        }

        private void BuildAll(string regFilePath)
        {
            string path = Path.Combine(BatchFilesPath, "全编译Upload.bat");
            if (!File.Exists(path)) return;

            string errMsg = string.Empty;
            JobHelper.Execute(path, regFilePath, false, false, ref errMsg);
        }

        public virtual void RebuildDataModels()
        {
        }
    }

    public class TaskJob : TaskJobBase
    {
    }

    public class TaskJobAdvanced : TaskJobBase
    {
        public override void RebuildDataModels()
        {
            base.RebuildDataModels();
            string path = Path.Combine(BatchFilesPath, "tempcmd.bat");
            if (!File.Exists(path)) return;

            string errMsg = string.Empty;
            JobHelper.Execute(path, string.Empty, false, false, ref errMsg);
        }
    }

    public static class JobHelper
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
                startInfo.Arguments = $"/C {filefullname} {arguments}"; //“/C”表示执行完命令后马上退出  
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

