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
        // �����������
        public const string RegisterFilePath = "RegisterFilePath";
        public const string BASE_PATH = "BASE_PATH";
        public const string SQL = "SQL";
        public const string ExecutionCount = "ExecutionCount";
        public const string RowCount = "RowCount";
        // Quartz ÿ��ִ��ʱ��������ʵ����һ����, ���Job���еķǾ�̬�������ܴ洢״̬��Ϣ
        private static int _counter = 1;//���Ա���״̬

        public string BatchFilesPath { get; private set; }
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                JobKey jobKey = context.JobDetail.Key;
                // ��ȡ���ݹ����Ĳ���            
                JobDataMap data = context.JobDetail.JobDataMap;
                string regFilePath = data.GetString(RegisterFilePath);
                string sql = data.GetString(SQL);
                int count = data.GetInt(ExecutionCount);
                string basepath = data.GetString(BASE_PATH);
                BatchFilesPath = Path.Combine(basepath, "BatchFiles");

                //Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\JetSun\3.0\Quartz\Job\" + this.GetType().ToString(), "State", "STATE_START");
                // �˴�Ϊִ�е�����
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
            factory.HostName = "localhost";//RabbitMQ�����ڱ�������
            factory.UserName = "guest";//�û���
            factory.Password = "guest";//���� 
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
            string path = Path.Combine(BatchFilesPath, "ȫ����Upload.bat");
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
            string output = ""; //����ַ���  
            if (filefullname != null && !filefullname.Equals(""))
            {
                FileInfo file = new FileInfo(filefullname);
                Process process = new Process();//�������̶���  
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";//�趨��Ҫִ�е�����  
                startInfo.WorkingDirectory = file.Directory.FullName;
                startInfo.Arguments = $"/C {filefullname} {arguments}"; //��/C����ʾִ��������������˳�  
                startInfo.UseShellExecute = false;//ʹ��ϵͳ��ǳ�������  
                startInfo.RedirectStandardInput = false;//���ض�������  
                startInfo.RedirectStandardOutput = redirectStandardOutput; //�ض������  
                startInfo.RedirectStandardError = redirectStandardError;
                startInfo.CreateNoWindow = false;//��������  
                process.StartInfo = startInfo;
                try
                {
                    if (process.Start())//��ʼ����  
                    {
                        if (seconds == 0)
                        {
                            process.WaitForExit();//�������޵ȴ����̽���  
                        }
                        else
                        {
                            process.WaitForExit(seconds * 1000); //�ȴ����̽������ȴ�ʱ��Ϊָ���ĺ���  
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

