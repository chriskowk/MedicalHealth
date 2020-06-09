using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.Win32;
using MyJob;
using Quartz;
using Quartz.Impl;
using Quartz.Job;

namespace JobController
{
    public static class GlobalEventManager
    {
        public static event EventHandler JobWasExecuted;
        public static void FireJobWasExecutedEvent(object sender, EventArgs args)
        {
            JobWasExecuted?.Invoke(sender, args);
        }

        public static string GetRegFilePath(string executePath)
        {
            string ret = string.Empty;

            if (executePath.Contains(@"\MedicalHealth\"))
                ret = Path.Combine(TaskJob.GetBatchFilePath(), @"注册表\眼科注册表.reg");
            else if (executePath.Contains(@"\MedicalHealthSY\"))
                ret = Path.Combine(TaskJobA.GetBatchFilePath(), @"注册表\省医注册表.reg");
            else if (executePath.Contains(@"\MedicalHealthBasicRC\"))
                ret = Path.Combine(TaskJobB.GetBatchFilePath(), @"注册表\市十二注册表.reg");
            else if (executePath.Contains(@"\MedicalHealthGH\"))
                ret = Path.Combine(TaskJobC.GetBatchFilePath(), @"注册表\光华注册表.reg");
            else if (executePath.Contains(@"\MedicalHealthS1\"))
                ret = Path.Combine(TaskJobD.GetBatchFilePath(), @"注册表\市一注册表.reg");
            else if (executePath.Contains(@"\MedicalHealthSGS1\"))
                ret = Path.Combine(TaskJobE.GetBatchFilePath(), @"注册表\韶关市一注册表.reg");

            return ret;
        }
    }

    public class JobListener : IJobListener
    {
        private static ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public virtual string Name
        {
            get { return "JobListener"; }
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                //执行前执行
                object path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\JetSun\3.0\", "ExecutablePath", "");
                string executePath = path == null ? "" : path.ToString();
                string regFilePath = GlobalEventManager.GetRegFilePath(executePath);
                IJobDetail job = context.JobDetail;
                job.JobDataMap.Put(TaskJobBase.RegisterFilePath, regFilePath);
                
                _logger.Info($"JobToBeExecuted: {regFilePath}");
            });
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                //否决时执行
                _logger.Info("JobExecutionVetoed");
            });
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                IJobDetail job = context.JobDetail;
                JobKey jobKey = job.Key;
                // 获取传递过来的参数
                JobDataMap data = job.JobDataMap;
                //获取回传的数据库表条目数
                int rowCount = data.GetInt(TaskJobBase.RowCount);
                try
                {
                    if (rowCount > 9)
                    {
                        context.Scheduler.PauseAll();
                        System.Windows.Forms.MessageBox.Show("预警已超9条");
                        context.Scheduler.ResumeAll();

                    }
                    _logger.Info(rowCount.ToString());
                    GlobalEventManager.FireJobWasExecutedEvent(job.JobType.FullName, new EventArgs());
                }
                catch (SchedulerException e)
                {
                    _logger.Error(e.StackTrace);
                }
            });
        }
    }
}