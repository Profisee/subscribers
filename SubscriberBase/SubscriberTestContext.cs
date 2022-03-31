using Profisee.Common.EventProcessing.DataContracts;
using System.Collections.Generic;

namespace Profisee.EventSubscribers
{
    public class SubscriberTestContext
    {
        private readonly List<EventSubscriberConfigurationPropertyValue> _properties = new List<EventSubscriberConfigurationPropertyValue>();

        internal IEnumerable<EventSubscriberConfigurationPropertyValue> GetProperties()
        {
            return _properties;
        }

        public string ProfiseeUri { get; set; }
        public string ClientId { get; set; }
        public string EntityName { get; set; }
        public string MemberCode { get; set; }

        public SubscriberTestContext AddConfigurationProperty(string propertyName, object propertyValue)
        {
            if (_properties.GetEventSubscriberConfigurationPropertyValue(propertyName) != null)
            {
                _properties.RemoveAll(prop => prop.PropertyName == propertyName);
            }

            _properties.Add(new EventSubscriberConfigurationPropertyValue { PropertyName = propertyName, Value = propertyValue });

            return this;
        }
    }
}