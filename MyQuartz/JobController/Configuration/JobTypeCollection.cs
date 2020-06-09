using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobController.Configuration
{

    [ConfigurationCollection(typeof(JobTypeElement))]
    public class JobTypeCollection : ConfigurationElementCollection, IEnumerable<JobTypeElement>
    {
        // 设定 app.config 中 collection 的 element 标签为 
        internal const string _propertyName = "jobtype";
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
            return new JobTypeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((JobTypeElement)(element)).Name;
        }

        IEnumerator<JobTypeElement> IEnumerable<JobTypeElement>.GetEnumerator()
        {
            int count = Count;
            for (var i = 0; i < count; i++)
            {
                yield return BaseGet(i) as JobTypeElement;
            }
        }

    }
}
