using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobController.Configuration
{
    public class SchedulerElement : ConfigurationElement
    {
        // JOB NAME
        [ConfigurationProperty("jobname", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string JobName
        {
            get { return (string)this["jobname"]; }
            set { this["jobname"] = value; }
        }

        // JOB Group
        [ConfigurationProperty("jobgroup", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string JobGroup
        {
            get { return (string)this["jobgroup"]; }
            set { this["jobgroup"] = value; }
        }

        // 作业类型名
        [ConfigurationProperty("name", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string TypeName
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        // 类型全称（FullName）
        [ConfigurationProperty("fullname", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string TypeFullName
        {
            get { return (string)this["fullname"]; }
            set { this["fullname"] = value; }
        }

        // 根路径（BASE_PATH）
        [ConfigurationProperty("basepath", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string BasePath
        {
            get { return (string)this["basepath"]; }
            set { this["basepath"] = value; }
        }

        // Scheduler File
        [ConfigurationProperty("schedulerfile", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string SchedulerFile
        {
            get { return (string)this["schedulerfile"]; }
            set { this["schedulerfile"] = value; }
        }

        // 客户名称
        [ConfigurationProperty("customer", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string CustomerName
        {
            get { return (string)this["customer"]; }
            set { this["customer"] = value; }
        }
    }
}
