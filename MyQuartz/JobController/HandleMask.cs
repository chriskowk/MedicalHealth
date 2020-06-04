using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using Quartz.Job;
using Quartz;
using Quartz.Impl;
using Quartz.Xml;
using System.Threading.Tasks;
using Quartz.Simpl;
using Quartz.Impl.Matchers;
using MyJob;
using System.Collections.Concurrent;

namespace JobController
{
    public static class SchedulerManager
    {
        private static IScheduler _scheduler;
        private static readonly object _lockObj = new object();
        private static ISchedulerFactory _schedulerFactory;

        public static ISchedulerFactory GetSchedulerFactory()
        {
            if (_schedulerFactory == null)
            {
                lock (_lockObj)
                {
                    _schedulerFactory = new StdSchedulerFactory();
                }
            }
            return _schedulerFactory;
        }

        public static async Task<IScheduler> GetScheduler()
        {
            if (_scheduler == null)
            {
                _scheduler = await GetSchedulerFactory().GetScheduler();
            }
            return _scheduler;
        }
    }

    public class HandleMask
    {
        private ConcurrentDictionary<JobKey, IScheduler> _schedules = new ConcurrentDictionary<JobKey, IScheduler>();
        public async Task Start()
        {
            _schedules.Values.ForEach((IScheduler a) => { a.Shutdown(true); });
            _schedules = new ConcurrentDictionary<JobKey, IScheduler>();
            _schedules.TryAdd(new JobKey("TaskJob", "TaskJobGroup"), await NewScheduler("JobConfig.xml"));
            _schedules.TryAdd(new JobKey("TaskJobA", "TaskJobAGroup"), await NewScheduler("JobAConfig.xml"));
            _schedules.TryAdd(new JobKey("TaskJobB", "TaskJobBGroup"), await NewScheduler("JobBConfig.xml"));
            _schedules.TryAdd(new JobKey("TaskJobC", "TaskJobCGroup"), await NewScheduler("JobCConfig.xml"));
            _schedules.TryAdd(new JobKey("TaskJobD", "TaskJobDGroup"), await NewScheduler("JobDConfig.xml"));
            _schedules.TryAdd(new JobKey("TaskJobE", "TaskJobEGroup"), await NewScheduler("JobEConfig.xml"));

            foreach (var item in _schedules)
            {
                JobKey key = item.Key;
                IScheduler sched = item.Value;
                // 设置初始参数
                IJobDetail job = await sched.GetJobDetail(key);
                job.JobDataMap.Put(TaskJobBase.SQL, "SELECT * FROM [ACT_ID_USER]");
                job.JobDataMap.Put(TaskJobBase.ExecutionCount, 1);

                // 设置监听器
                JobListener listener = new JobListener();
                //IMatcher<JobKey> matcher = KeyMatcher<JobKey>.KeyEquals(job.Key);
                //sched.ListenerManager.AddJobListener(listener, matcher);
                sched.ListenerManager.AddJobListener(listener);
                await sched.Start();
            }
        }

        private static async Task<IScheduler> NewScheduler(string configFile)
        {
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = await sf.GetScheduler();
            string path = AppDomain.CurrentDomain.BaseDirectory + configFile;
            XMLSchedulingDataProcessor processor = new XMLSchedulingDataProcessor(new SimpleTypeLoadHelper());
            await processor.ProcessFileAndScheduleJobs(path, sched);

            return sched;
        }

        public async Task Stop()
        {
            foreach (var sched in _schedules.Values)
            {
                await sched.Shutdown(true);
            }
        }
    }
}
