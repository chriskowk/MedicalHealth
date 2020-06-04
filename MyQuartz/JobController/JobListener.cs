using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
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
    }

    public class JobListener : IJobListener
    {
        readonly ILog _log = LogManager.GetLogger(typeof(TaskJobBase));
        public virtual string Name
        {
            get { return "JobListener"; }
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                //执行前执行
                _log.Info("JobToBeExecuted");
            });
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                //否决时执行
                _log.Info("JobExecutionVetoed");
            });
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                JobKey jobKey = context.JobDetail.Key;
                // 获取传递过来的参数
                JobDataMap data = context.JobDetail.JobDataMap;
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
                    _log.Info(rowCount.ToString());
                    GlobalEventManager.FireJobWasExecutedEvent($"MyJob.{jobKey.Name}", new EventArgs());
                }
                catch (SchedulerException e)
                {
                    _log.Error(e.StackTrace);
                }
            });
        }
    }
}