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
using JobController.Configuration;

namespace JobController
{
    public class HandleMask
    {
        private ConcurrentDictionary<JobKey, IScheduler> _schedules;
        public void Start()
        {
            Task.Run(async () =>
            {
                _schedules = new ConcurrentDictionary<JobKey, IScheduler>();
                foreach (SchedulerElement item in ConfigHelper.SchedulerCollection)
                {
                    _schedules.TryAdd(new JobKey(item.JobName, item.JobGroup), await NewScheduler(item.SchedulerFile));
                }

                foreach (var item in _schedules)
                {
                    IScheduler sched = item.Value;
                    // …Ë÷√º‡Ã˝∆˜
                    JobListener listener = new JobListener();
                    //IMatcher<JobKey> matcher = KeyMatcher<JobKey>.KeyEquals(key);
                    //sched.ListenerManager.AddJobListener(listener, matcher);
                    sched.ListenerManager.AddJobListener(listener);

                    await sched.Start();
                }
            });
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

        public void Stop()
        {
            Task.Run(() =>
            {
                foreach (var sched in _schedules.Values)
                {
                    sched.Shutdown(true);
                }
            });
        }
    }
}
