using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSideKicks.Configuration
{
    public class SiteElement : ConfigurationElement
    {
        // SITE NAME
        [ConfigurationProperty("site-name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string SiteName
        {
            get { return (string)this["site-name"]; }
            set { this["site-name"] = value; }
        }

        // SHARED PATH
        [ConfigurationProperty("shared-path", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string SharedPath
        {
            get { return (string)this["shared-path"]; }
            set { this["shared-path"] = value; }
        }
    }

    [ConfigurationCollection(typeof(SiteElement))]
    public class SiteCollection : ConfigurationElementCollection, IEnumerable<SiteElement>
    {
        // 设定 app.config 中 collection 的 element 标签为 
        internal const string _propertyName = "site";
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }

        protected override bool IsElementName(string elementName)
        {
            return elementName.Equals(_propertyName, StringComparison.InvariantCultureIgnoreCase);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SiteElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SiteElement)(element)).SiteName;
        }

        IEnumerator<SiteElement> IEnumerable<SiteElement>.GetEnumerator()
        {
            int count = Count;
            for (var i = 0; i < count; i++)
            {
                yield return BaseGet(i) as SiteElement;
            }
        }
    }

    public class WcfServerElement : ConfigurationElement
    {
        // SERVER NAME
        [ConfigurationProperty("server-name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string ServerName
        {
            get { return (string)this["server-name"]; }
            set { this["server-name"] = value; }
        }
        
        // DISTRIBUTE PATH
        [ConfigurationProperty("distribute-path", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string DistributePath
        {
            get { return (string)this["distribute-path"]; }
            set { this["distribute-path"] = value; }
        }

        // DOWNLOADS BIN PATH
        [ConfigurationProperty("downloads-bin-path", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string DownloadsBinPath
        {
            get { return (string)this["downloads-bin-path"]; }
            set { this["downloads-bin-path"] = value; }
        }

        // DOWNLOADS RESOURCES PATH
        [ConfigurationProperty("downloads-resources-path", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string DownloadsResourcesPath
        {
            get { return (string)this["downloads-resources-path"]; }
            set { this["downloads-resources-path"] = value; }
        }

        // SITES
        [ConfigurationProperty("sites", DefaultValue = null, IsKey = false, IsRequired = true)]
        public SiteCollection Sites
        {
            get { return (SiteCollection)this["sites"]; }
        }

        public override string ToString()
        {
            return this.ServerName;
        }
    }

    [ConfigurationCollection(typeof(WcfServerElement))]
    public class WcfServerCollection : ConfigurationElementCollection, IEnumerable<WcfServerElement>
    {
        // 设定 app.config 中 collection 的 element 标签为 
        internal const string _propertyName = "wcfserver";
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }

        protected override bool IsElementName(string elementName)
        {
            return elementName.Equals(_propertyName, StringComparison.InvariantCultureIgnoreCase);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new WcfServerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WcfServerElement)(element)).ServerName;
        }

        IEnumerator<WcfServerElement> IEnumerable<WcfServerElement>.GetEnumerator()
        {
            int count = Count;
            for (var i = 0; i < count; i++)
            {
                yield return BaseGet(i) as WcfServerElement;
            }
        }
    }
}
