using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSideKicks.Configuration
{
    public class WcfServersSection : ConfigurationSection
    {
        // 定义 app.config 中此 section 所使用的 collection 标签为 
        [ConfigurationProperty("wcfservers", IsRequired = false)]
        public WcfServerCollection WcfServers
        {
            get
            {
                return (WcfServerCollection)this["wcfservers"];
            }
        }

    }
}
