using JobController.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobController
{
    public class ConfigHelper
    {
        private static readonly IntegrationSection _integrationSection = ConfigurationManager.GetSection("integration.config") as IntegrationSection;

        public static JobTypeCollection SchedulerCollection
        {
            get { return _integrationSection.SchedulerCollection; }
        }

        public static string GetBasePath(string name, bool byFullName = false)
        {
            JobTypeElement item = byFullName ? SchedulerCollection.FirstOrDefault(a => a.FullName == name) : SchedulerCollection.FirstOrDefault(a => a.Name == name);
            return item?.BasePath;
        }

        public static string GetCustomerName(string name, bool byFullName = false)
        {
            JobTypeElement item = byFullName ? SchedulerCollection.FirstOrDefault(a => a.FullName == name) : SchedulerCollection.FirstOrDefault(a => a.Name == name);
            return item?.CustomerName;
        }

        public static string GetSchedulerFile(string name, bool byFullName = false)
        {
            JobTypeElement item = byFullName ? SchedulerCollection.FirstOrDefault(a => a.FullName == name) : SchedulerCollection.FirstOrDefault(a => a.Name == name);
            return item?.SchedulerFile;
        }

        public static string GetName(string fullname)
        {
            JobTypeElement item = SchedulerCollection.FirstOrDefault(a => a.FullName == fullname);
            return item?.Name;
        }

        public static string GetFullName(string name)
        {
            JobTypeElement item = SchedulerCollection.FirstOrDefault(a => a.Name == name);
            return item?.FullName;
        }
    }
}
