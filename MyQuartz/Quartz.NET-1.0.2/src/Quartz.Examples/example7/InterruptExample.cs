/* 
* Copyright 2007 OpenSymphony 
* 
* Licensed under the Apache License, Version 2.0 (the "License"); you may not 
* use this file except in compliance with the License. You may obtain a copy 
* of the License at 
* 
*   http://www.apache.org/licenses/LICENSE-2.0 
*   
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
* WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
* License for the specific language governing permissions and limitations 
* under the License.
* 
*/
using System;
using System.Threading;
using Common.Logging;
using Quartz;
using Quartz.Examples;
using Quartz.Impl;

namespace Quartz.Examples.Example7
{
	
	/// <summary>
	/// Demonstrates the behavior of <see cref="IStatefulJob" />s, as well as how
	/// misfire instructions affect the firings of triggers of <see cref="IStatefulJob" />
	/// s - when the jobs take longer to execute that the frequency of the trigger's
	/// repitition.
	/// 
	/// <p>
	/// While the example is running, you should note that there are two triggers
	/// with identical schedules, firing identical jobs. The triggers "want" to fire
	/// every 3 seconds, but the jobs take 10 seconds to execute. Therefore, by the
	/// time the jobs complete their execution, the triggers have already "misfired"
	/// (unless the scheduler's "misfire threshold" has been set to more than 7
	/// seconds). You should see that one of the jobs has its misfire instruction
	/// set to <see cref="MisfireInstruction.SimpleTrigger.RescheduleNowWithExistingRepeatCount" />-
	/// which causes it to fire immediately, when the misfire is detected. The other
	/// trigger uses the default "smart policy" misfire instruction, which causes
	/// the trigger to advance to its next fire time (skipping those that it has
	/// missed) - so that it does not refire immediately, but rather at the next
	/// scheduled time.
	/// </p>
	/// </summary>
	/// <author><a href="mailto:bonhamcm@thirdeyeconsulting.com">Chris Bonham</a></author>
	public class InterruptExample : IExample
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(InterruptExample));
		public string Name
		{
			get 
			{ 
				return GetType().Name;
			}
		}

		public virtual void Run()
		{
			log.Info("------- Initializing ----------------------");
			
			// First we must get a reference to a scheduler
			ISchedulerFactory sf = new StdSchedulerFactory();
			IScheduler sched = sf.GetScheduler();
			
			log.Info("------- Initialization Complete -----------");
			
			log.Info("------- Scheduling Jobs -------------------");
			
			// get a "nice round" time a few seconds in the future...
			DateTime ts = TriggerUtils.GetNextGivenSecondDate(null, 15);
			
			JobDetail job = new JobDetail("interruptableJob1", "group1", typeof(DumbInterruptableJob));
			SimpleTrigger trigger = new SimpleTrigger("trigger1", "group1", ts, null, SimpleTrigger.RepeatIndefinitely, TimeSpan.FromSeconds(5));
			DateTime ft = sched.ScheduleJob(job, trigger);
            log.Info(string.Format("{0} will run at: {1} and repeat: {2} times, every {3} seconds", job.FullName, ft.ToString("r"), trigger.RepeatCount, trigger.RepeatInterval.TotalSeconds));
			
			// start up the scheduler (jobs do not start to fire until
			// the scheduler has been started)
			sched.Start();
			log.Info("------- Started Scheduler -----------------");
			
			
			log.Info("------- Starting loop to interrupt job every 7 seconds ----------");
			for (int i = 0; i < 50; i++)
			{
				try
				{
					Thread.Sleep(7 * 1000);
					// tell the scheduler to interrupt our job
					sched.Interrupt(job.Name, job.Group);
				}
				catch (Exception ex)
				{
                    Console.WriteLine(ex);
				}
			}
			
			log.Info("------- Shutting Down ---------------------");
			
			sched.Shutdown(true);
			
			log.Info("------- Shutdown Complete -----------------");
			SchedulerMetaData metaData = sched.GetMetaData();
			log.Info(string.Format("Executed {0} jobs.", metaData.NumJobsExecuted));
		}

	}
}
