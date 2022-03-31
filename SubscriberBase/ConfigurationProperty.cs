using Profisee.Common.EventProcessing.Enums;
using System.Collections.Generic;

namespace Profisee.EventSubscribers
{
    public class ConfigurationProperty
    {
        public string Name { set; get; }
        public string Description { set; get; }
        public EventSubscriberConfigurationPropertyType Type { set; get; }
        public bool IsRequired { get; set; }

        public List<string> PicklistValues { get; set; }
    }
}
