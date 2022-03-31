using Profisee.Common.EventProcessing.DataContracts;
using Profisee.Common.EventProcessing.Enums;
using Profisee.Common.EventProcessing.Interfaces;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace Profisee.EventSubscribers.SubscriberTemplate
{
    [Export(typeof(ISubscribe))]
    public class SubscriberTemplate : SubscriberBase
    {
        #region constants
        //TODO: declare configuration property names
        public const string StringParameterPropertyName = "String Parameter Property Name";
        public const string IntegerParameterPropertyName = "Integer Parameter Property Name";
        #endregion

        #region private variables
        //TODO: declare variables to hold configuration property values
        private string _stringParameter = string.Empty;
        private int _integerParameter = 0;
        #endregion

        #region constructor
        public SubscriberTemplate() : base("SubscriberTemplate")
        {
            //TODO: initiate configuration properties
            this.AddConfigurationProperty(new ConfigurationProperty
            {
                Name = StringParameterPropertyName,
                Description = "String Parameter Property Description",
                Type = EventSubscriberConfigurationPropertyType.Text,
                IsRequired = true,
            });

            this.AddConfigurationProperty(new ConfigurationProperty
            {
                Name = IntegerParameterPropertyName,
                Description = "Integer Parameter Property Description",
                Type = EventSubscriberConfigurationPropertyType.Int32,
                IsRequired = true,
            });
        }
        #endregion

        #region overrides
        public override ValidationResult ValidateSubscriber()
        {
            var result = new ValidationResult();

            //TODO: set configuration property values
            _stringParameter = this.ConfigurationPropertyValues.GetEventSubscriberConfigurationPropertyValueAsString(StringParameterPropertyName);
            _integerParameter = this.ConfigurationPropertyValues.GetEventSubscriberConfigurationPropertyValueAsInteger(IntegerParameterPropertyName);

            //TODO: add custom validation logic here
            //NOTE: The underlying framework has already verfiied that a value is provided to any required configuration property

            return result;
        }

        public override ValidationResult ExecuteSubscriber(SubscriberContext context)
        {
            var result = new ValidationResult();

            //TODO: add custom logic here
            //NOTE: context.MdmSource, context.MdmModel, context.MdmEntity are object instances that are ready to be used
            //      configureation property values are hold in the instance variables such as _stringParameter and _integerParameter
            // use Logging.LogMessage(message, EventLogEntryType.Error); for logging to the SQL logging.tSystemLog table in Profisee.  Event log should not be used.

            var model = context.MdmModel;
            var entity = context.MdmEntity;

            return result;
        }
        #endregion
    }
}
