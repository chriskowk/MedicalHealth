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
using System.Diagnostics;

namespace JobController
{
    public class HandleMask
    {
        private static IScheduler _scheduler;
        public void Start()
        {
            if (!File.Exists(ConfigHelper.QuartzSchedulerFile)) return;

            Task.Run(async () =>
            {
                ISchedulerFactory sf = new StdSchedulerFactory();
                _scheduler = await sf.GetScheduler();
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigHelper.QuartzSchedulerFile);

                XMLSchedulingDataProcessor processor = new XMLSchedulingDataProcessor(new SimpleTypeLoadHelper());
                await processor.ProcessFileAndScheduleJobs(path, _scheduler);

                // …Ë÷√º‡Ã˝∆˜
                JobListener listener = new JobListener();
                //IMatcher<JobKey> matcher = KeyMatcher<JobKey>.KeyEquals(key);
                //sched.ListenerManager.AddJobListener(listener, matcher);
                _scheduler.ListenerManager.AddJobListener(listener);
                await _scheduler.Start();
            });
        }

        public void Stop()
        {
            if (_scheduler == null) return;

            Task.Run(() =>
            {
                Debug.Print(_scheduler.InStandbyMode.ToString());
                _scheduler.Standby();
            });
        }
    }
}
