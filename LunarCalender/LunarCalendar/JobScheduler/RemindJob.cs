using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarCalendar.JobScheduler
{
    public class RemindJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                //using (StreamWriter sw = new StreamWriter(@"E:\httpjob.log", true, Encoding.UTF8))
                //{
                //    sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                //}
                string errMsg = string.Empty;
                string path = Path.Combine(Path.GetDirectoryName(Common.CurrentAssembly.Location), "MessageBox.exe");
                string param = context.JobDetail.JobDataMap[Common.REMIND_CONTENT].ToString();

                if (!string.IsNullOrEmpty(param))
                    Common.ExecBatch(path, false, false, false, param.Replace(" ", "{SPACE}"), ref errMsg);
            });
        }
    }
}
