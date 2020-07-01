using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSideKicks.Configuration;

namespace TFSideKicks.Helpers
{
    public class ConfigHelper
    {
        private static readonly IntegrationSection _integrationSection = ConfigurationManager.GetSection("integration.config") as IntegrationSection;

        public static TfsUrlCollection ProjectCollection
        {
            get { return _integrationSection.ProjectCollection; }
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
    }
}
