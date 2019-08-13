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
using Common.Logging;

namespace Quartz.Examples.Example9
{
	
	/// <summary>
	/// This is just a simple job that gets fired off many times by example 1
	/// </summary>
	/// <author>Bill Kratzer</author>
	public class SimpleJob1 : IJob
	{
		
		private static ILog _log = LogManager.GetLogger(typeof(SimpleJob1));
		

		/// <summary>
		/// Called by the <see cref="IScheduler" /> when a <see cref="Trigger" />
		/// fires that is associated with the <see cref="IJob" />.
		/// <p>
		/// The implementation may wish to set a  result object on the
		/// JobExecutionContext before this method exits.  The result itself
		/// is meaningless to Quartz, but may be informative to
		/// <see cref="JobListeners" /> or
		/// <see cref="TriggerListeners" /> that are watching the job's
		/// execution.
		/// </p>
		/// </summary>
		/// <param name="context"></param>
		public virtual void Execute(JobExecutionContext context)
		{
			
			// This job simply prints out its job name and the
			// date and time that it is running
			String jobName = context.JobDetail.FullName;
			_log.Info(string.Format("SimpleJob1 says: {0} executing at {1}", jobName, DateTime.Now.ToString("r")));
		}
	}
}
