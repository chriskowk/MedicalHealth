using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using System.Collections.ObjectModel;
using LunarCalendar.Entities;

namespace LunarCalendar.JobScheduler
{
    /// <summary>
    /// 任务调度中心
    /// </summary>
    public static class JobCenter
    {
        /// <summary>
        /// 任务计划
        /// </summary>
        private static IScheduler _scheduler = null;
        public static async Task<IScheduler> GetSchedulerAsync()
        {
            if (_scheduler != null)
            {
                return _scheduler;
            }
            else
            {
                ISchedulerFactory schedf = new StdSchedulerFactory();
                IScheduler sched = await schedf.GetScheduler();
                return sched;
            }
        }
        /// <summary>
        /// 添加任务计划//或者进程终止后的开启
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> StartScheduleJobAsync(Diary info)
        {
            try
            {
                if (info != null)
                {
                    if (info.RunningStart == null)
                    {
                        info.RunningStart = DateTime.Now;
                    }
                    DateTimeOffset startRunTime = DateBuilder.NextGivenSecondDate(info.RunningStart, 1);
                    if (info.RunningEnd == null)
                    {
                        info.RunningEnd = DateTime.MaxValue.AddDays(-1);
                    }
                    DateTimeOffset endRunTime = DateBuilder.NextGivenSecondDate(info.RunningEnd, 1);
                    if (string.IsNullOrWhiteSpace(info.CronExpress))
                    {
                        DateTime dt = DateTime.Now.AddMinutes(1);
                        info.CronExpress = $"0 {dt.Minute} {dt.Hour} * * ?";
                    }
                    _scheduler = await GetSchedulerAsync();
                    Type type = typeof(RemindJob); // Type.GetType(info.JobName, true, true);
                    IJobDetail job = JobBuilder.Create(type)
                                                .WithIdentity(info.JobName, info.JobGroup)
                                                .Build();
                    job.JobDataMap.Put(Common.REMIND_CONTENT, $"【主题】{info.Title}\r\n【内容】{info.Content}");
                    ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                                .StartAt(startRunTime)
                                                .EndAt(endRunTime)
                                                .WithIdentity(info.JobName, info.JobGroup)
                                                .WithCronSchedule(info.CronExpress)
                                                .Build();
                    ((CronTriggerImpl)trigger).MisfireInstruction = MisfireInstruction.CronTrigger.DoNothing;
                    IList<ICronTrigger> triggers = new List<ICronTrigger> { trigger };
                    await _scheduler.ScheduleJob(job, new ReadOnlyCollection<ICronTrigger>(triggers), true);
                    if (!_scheduler.IsStarted) await _scheduler.Start();

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("because one already exists with this identification"))
                {
                    _ = await ResumeScheduleJobAsync(info);
                }
                return false;
            }
        }
        /// <summary>
        /// 恢复指定的任务计划**恢复的是暂停后的任务计划，如果是程序奔溃后 或者是进程杀死后的恢复，此方法无效
        /// </summary>
        /// <returns></returns>
        public static async Task<string> ResumeScheduleJobAsync(Diary info)
        {
            try
            {
                _scheduler = await GetSchedulerAsync();
                //resumejob 恢复
                await _scheduler.ResumeJob(new JobKey(info.JobName, info.JobGroup));
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
