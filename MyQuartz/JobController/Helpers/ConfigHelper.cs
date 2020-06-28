using JobController.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
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

        public static SchedulerElement GetElementByExecutablePath(string executablePath)
        {
            SchedulerElement item = SchedulerCollection.FirstOrDefault(a => executablePath.StartsWith(a.BasePath, StringComparison.OrdinalIgnoreCase));
            return item;
        }

        public static string GetBasePath(string jobname)
        {
            SchedulerElement item = SchedulerCollection.FirstOrDefault(a => a.JobName == jobname);
            return item?.BasePath;
        }

        public static string GetBatchFilesPath(string jobname)
        {
            SchedulerElement item = SchedulerCollection.FirstOrDefault(a => a.JobName == jobname);
            return item?.BatchFilesPath;
        }

        public static string GetCustomerName(string jobname)
        {
            SchedulerElement item = SchedulerCollection.FirstOrDefault(a => a.JobName == jobname);
            return item?.CustomerName;
        }

        /// <summary>
        /// 获取配置文件键值对（appSettings）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppConfig(string key)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(file);

            foreach (KeyValueConfigurationElement item in config.AppSettings.Settings)
            {
                if (item.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) return item.Value;
            }

            return null;
        }

        /// <summary>
        /// 获取 TeamFoundationServerURL
        /// </summary>
        public static string TeamFoundationServerURL
        {
            get { return ConfigHelper.GetAppConfig("TeamFoundationServerURL"); }
        }

        /// <summary>
        /// 获取全局调度配置文件名
        /// </summary>
        public static string QuartzSchedulerFile
        {
            get { return ConfigHelper.GetAppConfig("QuartzSchedulerFile"); }
        }

        /// <summary>
        /// 重写quartz_jobs.xml文件
        /// </summary>
        public static void WriteQuartzJobsConfig()
        {
            FileStream fs = new FileStream(QuartzSchedulerFile, FileMode.Create);
            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.WriteLine("<job-scheduling-data xmlns=\"http://quartznet.sourceforge.net/JobSchedulingData\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" version=\"2.0\">");
                sw.WriteLine("  <processing-directives>");
                sw.WriteLine("    <overwrite-existing-data>true</overwrite-existing-data>");
                sw.WriteLine("  </processing-directives>");
                sw.WriteLine("  <schedule>");
                foreach (SchedulerElement item in ConfigHelper.SchedulerCollection)
                {
                    string cronexpr = item.CronModified ?? item.CronExpression;
                    sw.WriteLine(BuildOneJobTrigger(item.JobName, item.JobGroup, item.TypeFullName, item.CustomerName, cronexpr));
                }
                sw.WriteLine("  </schedule>");
                sw.WriteLine("</job-scheduling-data>");
            }
            fs.Close();
        }

        private static string BuildOneJobTrigger(string jobname, string groupname, string fulltypename, string customer, string cornexpr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("    <job>");
            sb.AppendLine("      <!--name(必填)同一个group中多个job的name不能相同，若未设置group则所有未设置group的job为同一个分组-->");
            sb.AppendLine($"      <name>{jobname}</name>");
            sb.AppendLine("      <!--group(选填) 任务所属分组，用于标识任务所属分组-->");
            sb.AppendLine($"      <group>{groupname}</group>");
            sb.AppendLine($"      <description>{customer}版本编译任务</description>");
            sb.AppendLine("      <!--job-type(必填)任务的具体类型及所属程序集，格式：实现了IJob接口的包含完整命名空间的类名,程序集名称-->");
            sb.AppendLine($"      <job-type>{fulltypename}</job-type>");
            sb.AppendLine("      <durable>true</durable>");
            sb.AppendLine("      <recover>false</recover>");
            sb.AppendLine("    </job>");
            sb.AppendLine("    <trigger>");
            sb.AppendLine("      <cron>");
            sb.AppendLine("        <!--name(必填) 触发器名称，同一个分组中的名称必须不同-->");
            sb.AppendLine($"        <name>{jobname}Trigger</name>");
            sb.AppendLine("        <!--group(选填) 触发器组-->");
            sb.AppendLine($"        <group>{jobname}TriggerGroup</group>");
            sb.AppendLine("        <!--job-name(必填) 要调度的任务名称，该job-name必须和对应job节点中的name完全相同-->");
            sb.AppendLine($"        <job-name>{jobname}</job-name>");
            sb.AppendLine("        <!--job-group(选填) 调度任务(job)所属分组，该值必须和job中的group完全相同-->");
            sb.AppendLine($"        <job-group>{groupname}</job-group>");
            sb.AppendLine("        <!--misfire-instruction(选填：默认FireOnceNow会在程序启动立马执行一次），应设置为DoNothing-->");
            sb.AppendLine("        <misfire-instruction>DoNothing</misfire-instruction>");
            sb.AppendLine("        <!--秒 分 小时 月内日期 月 周内日期 年（可选字段）-->");
            sb.AppendLine($"        <cron-expression>{cornexpr}</cron-expression>");
            sb.AppendLine("      </cron>");
            sb.AppendLine("    </trigger>");

            return sb.ToString();
        }
    }
}
