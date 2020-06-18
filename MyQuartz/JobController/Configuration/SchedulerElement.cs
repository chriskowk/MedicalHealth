using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobController.Configuration
{
    public class SchedulerElement : ConfigurationElement
    {
        // JOB NAME
        [ConfigurationProperty("job-name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string JobName
        {
            get { return (string)this["job-name"]; }
            set { this["job-name"] = value; }
        }

        // JOB Group
        [ConfigurationProperty("job-group", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string JobGroup
        {
            get { return (string)this["job-group"]; }
            set { this["job-group"] = value; }
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
        [ConfigurationProperty("base-path", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string BasePath
        {
            get { return (string)this["base-path"]; }
            set { this["base-path"] = value; }
        }

        // cron-expression
        [ConfigurationProperty("cron-expression", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string CronExpression
        {
            get { return (string)this["cron-expression"]; }
            set { this["cron-expression"] = value; }
        }

        // 客户名称
        [ConfigurationProperty("customer", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string CustomerName
        {
            get { return (string)this["customer"]; }
            set { this["customer"] = value; }
        }

        /// <summary>
        /// 修改后的Cron表达式
        /// </summary>
        public string CronModified { get; set; }

        /// <summary>
        /// 获取批命令路径
        /// </summary>
        public string BatchFilesPath
        {
            get { return Path.Combine(BasePath, "BatchFiles"); }
        }

        /// <summary>
        /// 获取注册表文件
        /// </summary>
        public string RegistryFile
        {
            get { return Path.Combine(BatchFilesPath, $"注册表\\{CustomerName}注册表.reg"); }
        }

        /// <summary>
        /// 获取注册表文件 
        /// </summary>
        public string Copy2SvcBinBatFile
        {
            get { return Path.Combine(BatchFilesPath, "__copy2svcbin.bat"); }
        }
    }
}
