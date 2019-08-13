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

namespace Quartz.Examples.Example5
{
	
	/// <summary>
	/// A dumb implementation of Job, for unittesting purposes.
	/// </summary>
	/// <author>James House</author>
	public class MisfireJob : IStatefulJob
	{
		
		// Logging
		private static ILog _log = LogManager.GetLogger(typeof(MisfireJob));
		
		// Constants
		public const string NUM_EXECUTIONS = "NumExecutions";
		public const string EXECUTION_DELAY = "ExecutionDelay";
		
	
		/// <summary> <p>
		/// Called by the <see cref="IScheduler" /> when a <see cref="Trigger" />
		/// fires that is associated with the <see cref="IJob" />.
		/// </p>
		/// 
		/// </summary>
		/// <throws>  JobExecutionException </throws>
		/// <summary>           if there is an exception while executing the job.
		/// </summary>
		public virtual void  Execute(JobExecutionContext context)
		{
			string jobName = context.JobDetail.FullName;
			_log.Info(string.Format("---{0} executing at {1}", jobName, DateTime.Now.ToString("r")));
			
			// default delay to five seconds
			int delay = 5;
			
			// use the delay passed in as a job parameter (if it exists)
			JobDataMap map = context.JobDetail.JobDataMap;
			if (map.Contains(EXECUTION_DELAY))
			{
				delay = map.GetInt(EXECUTION_DELAY);
			}
			
			try
			{
				Thread.Sleep(1000 * delay);
			}
            catch (ThreadInterruptedException)
			{
			}
			
			_log.Info(string.Format("---{0} completed at {1}", jobName, DateTime.Now.ToString("r")));
		}

	}
}
