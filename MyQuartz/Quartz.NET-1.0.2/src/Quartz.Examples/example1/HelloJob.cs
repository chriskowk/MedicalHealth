/* 
* Copyright 2007 the original author or authors. 
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
using Common.Logging;

namespace Quartz.Examples.Example1
{
	
	/// <summary>
	/// This is just a simple job that says "Hello" to the world.
	/// </summary>
	/// <author>Bill Kratzer</author>
	public class HelloJob : IJob
	{
		
		private static ILog _log = LogManager.GetLogger(typeof(HelloJob));
		
		/// <summary> 
		/// Empty constructor for job initilization
		/// <p>
		/// Quartz requires a public empty constructor so that the
		/// scheduler can instantiate the class whenever it needs.
		/// </p>
		/// </summary>
		public HelloJob()
		{
		}
		
		/// <summary> 
		/// Called by the <see cref="IScheduler" /> when a
		/// <see cref="Trigger" /> fires that is associated with
		/// the <see cref="IJob" />.
		/// </summary>
		public virtual void  Execute(JobExecutionContext context)
		{
			
			// Say Hello to the World and display the date/time
			_log.Info(string.Format("Hello World! - {0}", System.DateTime.Now.ToString("r")));
		}

	}
}