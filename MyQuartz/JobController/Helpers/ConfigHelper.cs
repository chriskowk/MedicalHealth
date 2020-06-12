using JobController.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobController
{
    public class ConfigHelper
    {
        private static readonly IntegrationSection _integrationSection = ConfigurationManager.GetSection("integration.config") as IntegrationSection;

        public static SchedulerCollection SchedulerCollection
        {
            get { return _integrationSection.SchedulerCollection; }
        }

        public static string GetJobNameByExecutablePath(string executablePath)
        {
            SchedulerElement item = SchedulerCollection.FirstOrDefault(a => executablePath.StartsWith(a.BasePath, StringComparison.OrdinalIgnoreCase));
            return item?.JobName;
        }

        public static string GetBasePath(string jobname)
        {
            SchedulerElement item = SchedulerCollection.FirstOrDefault(a => a.JobName == jobname);
            return item?.BasePath;
        }

        public static string GetCustomerName(string jobname)
        {
            SchedulerElement item = SchedulerCollection.FirstOrDefault(a => a.JobName == jobname);
            return item?.CustomerName;
        }
        
        public static string GetCronExpression(string jobname)
        {
            SchedulerElement item = SchedulerCollection.FirstOrDefault(a => a.JobName == jobname);
            return item?.CronExpression;
        }

        public static string GetTypeName(string jobname)
        {
            SchedulerElement item = SchedulerCollection.FirstOrDefault(a => a.JobName == jobname);
            return item?.TypeName;
        }

        /// <summary>
        /// 获取配置文件键值对（appSettings）
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string GetAppConfig(string strKey)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == strKey)
                {
                    return config.AppSettings.Settings[strKey].Value.ToString();
                }
            }
            return null;
        }
    }
}
