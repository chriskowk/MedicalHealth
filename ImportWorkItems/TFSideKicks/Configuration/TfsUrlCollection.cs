using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSideKicks.Configuration
{
    public class TfsUrlElement : ConfigurationElement
    {
        // VERSION NAME
        [ConfigurationProperty("version-name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string VersionName
        {
            get { return (string)this["version-name"]; }
            set { this["version-name"] = value; }
        }

        // URL
        [ConfigurationProperty("tfs-url", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string TfsUrl
        {
            get { return (string)this["tfs-url"]; }
            set { this["tfs-url"] = value; }
        }
    }

    [ConfigurationCollection(typeof(TfsUrlElement))]
    public class TfsUrlCollection : ConfigurationElementCollection, IEnumerable<TfsUrlElement>
    {
        // 设定 app.config 中 collection 的 element 标签为 
        internal const string _propertyName = "teamproject";
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
            return new TfsUrlElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TfsUrlElement)(element)).VersionName;
        }

        IEnumerator<TfsUrlElement> IEnumerable<TfsUrlElement>.GetEnumerator()
        {
            int count = Count;
            for (var i = 0; i < count; i++)
            {
                yield return BaseGet(i) as TfsUrlElement;
            }
        }

    }
}
