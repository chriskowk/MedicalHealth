using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

using Quartz;
using Quartz.Impl;
using Quartz.Xml;

namespace JobController
{
    //public class SchedulerEventArgs : EventArgs
    //{
    //    public SchedulerEventArgs(IJob instance)
    //    {
    //        _instance = instance;
    //    }

    //    private IJob _instance;
    //    public IJob Instance
    //    {
    //        get { return _instance; }
    //    }
    //}

    public class HandleMask
    {
        //public event EventHandler<SchedulerEventArgs> ScheduleStarting;
        //public event EventHandler<SchedulerEventArgs> ScheduleFinished;

        private IList<IScheduler> _schedules;
        public void Start()
        {
            _schedules = new List<IScheduler>();
            JobSchedulingDataProcessor processor;
            processor = new JobSchedulingDataProcessor(true, true);
            _schedules.Add(NewScheduler(processor, "JobConfig.xml"));
            _schedules.Add(NewScheduler(processor, "JobAConfig.xml"));
            _schedules.Add(NewScheduler(processor, "JobBConfig.xml"));
            _schedules.Add(NewScheduler(processor, "JobCConfig.xml"));
            _schedules.Add(NewScheduler(processor, "JobDConfig.xml"));
            _schedules.Add(NewScheduler(processor, "JobEConfig.xml"));
            foreach (var sched in _schedules)
            {
                sched.Start();
            }
        }

        private static IScheduler NewScheduler(JobSchedulingDataProcessor processor, string configFile)
        {
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();
            string path = AppDomain.CurrentDomain.BaseDirectory + configFile;
            Stream s = new StreamReader(path, System.Text.Encoding.GetEncoding("UTF-8")).BaseStream;
            processor.ProcessStream(s, null);
            processor.ScheduleJobs(new Hashtable(), sched, false);

            return sched;
        }

        public void Stop()
        {
            foreach (var sched in _schedules)
            {
                sched.Shutdown(true);
            }
        }
    }
}
