using Profisee.Common.EventProcessing.DataContracts;
using Profisee.Common.EventProcessing.Enums;
using Profisee.Common.EventProcessing.Interfaces;
using Profisee.Services.Sdk.AcceleratorFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;

/// <summary>
/// This solution extends the functionality of the SDK for Subscribers.
/// It includes Unit testing support and friendly getMemberValue methods
/// </summary>
namespace Profisee.EventSubscribers
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    abstract public class SubscriberBase : ISubscribe
    {
        #region Private variables
        private readonly List<EventSubscriberConfigurationProperty> _configurationProperties = null;
        private const string ProfiseeUriPropertyName = "Profisee URI";
        private ConnectionOverride _maestroUriConnectionOverride;
        private const string ClientIdPropertyName = "ClientId";
        private string _ClientId;

        private MdmSource _mdmSource = null;
        private MdmModel _mdmModel = null;
        private MdmEntity _mdmEntity = null;
        #endregion

        #region Constructor and protected methods
        protected SubscriberBase(string name)
        {
            Name = name;

            _configurationProperties = new List<EventSubscriberConfigurationProperty>
            {
                new EventSubscriberConfigurationProperty
                {
                    Name = ProfiseeUriPropertyName,
                    PropertyType = EventSubscriberConfigurationPropertyType.ConnectionOverride,
                    Category = "Configuration",
                    IsRequired = false,
                    Description = "Maestro URI",
                    IsVisible = false,
                    DefaultValue = "net.tcp://localhost/Profisee"
                },
                new EventSubscriberConfigurationProperty
                {
                    Name = ClientIdPropertyName,
                    PropertyType = EventSubscriberConfigurationPropertyType.Text,
                    Category = "Configuration",
                    IsRequired = true,
                    Description = "Client Id of the Subscriber's \"Run as\" user.",
                    IsVisible = true
                }
            };
        }

        protected void AddConfigurationProperty(ConfigurationProperty configurationProperty)
        {
            _configurationProperties.Add(
                new EventSubscriberConfigurationProperty
                {
                    Name = configurationProperty.Name,
                    Description = configurationProperty.Description,
                    PropertyType = configurationProperty.Type,
                    IsRequired = configurationProperty.IsRequired,
                    IsVisible = true,
                    Category = "Configuration",
                    PicklistValues = configurationProperty.PicklistValues,
                });
        }

        protected List<EventSubscriberConfigurationPropertyValue> ConfigurationPropertyValues { get; private set; } = null;
        #endregion

        #region ISubscriber template
        public string Name
        {
            get;  private set;
        }

        public bool SupportsExternalScenarios
        {
            get
            {
                return true;
            }
        }

        public List<SupportedSystem> GetSupportedSystems()
        {
            return new List<SupportedSystem>
            {
                new SupportedSystem
                {
                    System = "Profisee",
                    Versions = new List<string>
                    {
                        ""
                    }
                }
            };
        }

        public List<EventSubscriberConfigurationProperty> GetConfigurationProperties()
        {
            return _configurationProperties;
        }

        public ContextInfo GetExternalConfigurationInfo(List<EventSubscriberConfigurationPropertyValue> propertyValues)
        {
            var mapInfo = new ContextInfo();

            mapInfo.SourceContext = new MapContext
            {
                Type = "Profisee",
                SystemName = "Profisee"
            };

            mapInfo.TargetContext = new MapContext
            {
                Type = "Profisee",
                SystemName = "Profisee"
            };

            return mapInfo;
        }

        public ValidationResult Initialize(List<EventSubscriberConfigurationPropertyValue> propertyValues)
        {
            ValidationResult result = new ValidationResult();

            _maestroUriConnectionOverride = propertyValues.GetEventSubscriberConfigurationPropertyValue(ProfiseeUriPropertyName) as ConnectionOverride;
            _ClientId = propertyValues.GetEventSubscriberConfigurationPropertyValue(ClientIdPropertyName) as string;

            if (_maestroUriConnectionOverride != null)
            {
                _mdmSource = new MdmSource();
                _mdmSource.Connect(_maestroUriConnectionOverride.Url, _ClientId);
            }

            result = Validate(propertyValues);

            return result;
        }
        #endregion

        #region ISubscriber calling override
        public ValidationResult Validate(List<EventSubscriberConfigurationPropertyValue> propertyValues)
        {
            ValidationResult result = new ValidationResult();

            ConfigurationPropertyValues = propertyValues;

            try
            {
                if (propertyValues == null)
                {
                    result.AddValidationMessage("PropertyValues is not supplied for validation.");
                    return result;
                }

                foreach (var property in _configurationProperties)
                {
                    var propertyValue = propertyValues.FirstOrDefault(p => p.PropertyName == property.Name);
                    if (property.IsRequired &&
                        string.IsNullOrWhiteSpace(propertyValue?.Value?.ToString()))
                    {
                        result.AddValidationMessage($"The {property.Name} property is required but was not supplied.");
                    }

                    if (propertyValue != null)
                    {
                        try
                        {
                            switch (property.PropertyType)
                            {
                                case EventSubscriberConfigurationPropertyType.Bool:
                                    Convert.ToBoolean(propertyValue.Value);
                                    break;
                                case EventSubscriberConfigurationPropertyType.DateTime:
                                    Convert.ToDateTime(propertyValue.Value);
                                    break;
                                case EventSubscriberConfigurationPropertyType.Double:
                                    Convert.ToDouble(propertyValue.Value);
                                    break;
                                case EventSubscriberConfigurationPropertyType.Int32:
                                    Convert.ToInt32(propertyValue.Value);
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            result.AddValidationMessage($"The {property.Name} property value supplied is not the correct data type.");
                        }
                    }
                }

                if (result.ResultMessages.Count > 0)
                {
                    return result;
                }

                // Plugin specific validations
                result = ValidateSubscriber();
            }
            catch (Exception exception)
            {
                result.AddValidationMessage(exception.Message);
                return result;
            }

            return result;
        }

        public ValidationResult Execute(IExecuteContext context)
        {
            ValidationResult result = new ValidationResult();

            try
            {
                if (context == null)
                {
                    result.AddValidationMessage("No execution context supplied.");
                    return result;
                }

                // Initialize Source, Model, Entity
                try
                {
                    if (_mdmSource == null)
                    {
                        _mdmSource = new MdmSource();
                        _mdmSource.Connect(_maestroUriConnectionOverride.Url, _ClientId);
                        _mdmModel = _mdmSource.GetModel();
                        _mdmEntity = _mdmModel.GetEntity(context.Member.EntityId.Name);
                    }
                    else if (_mdmModel == null)
                    {
                        _mdmModel = _mdmSource.GetModel();
                        _mdmEntity = _mdmModel.GetEntity(context.Member.EntityId.Name);
                    }
                    else if (_mdmEntity == null)
                    {
                        _mdmEntity = _mdmModel.GetEntity(context.Member.EntityId.Name);
                    }
                }
                catch (FaultException<Exception> fee)
                {
                    result.AddValidationMessage($" Error: {fee.Message}, Profisee Uri: {_maestroUriConnectionOverride.Url}");
                    return result;
                }

                var subscriberContext = new SubscriberContext
                {
                    MdmSource = _mdmSource,
                    MdmModel = _mdmModel,
                    MdmEntity = _mdmEntity,
                    ExecuteContext = context,
                };

                result = ExecuteSubscriber(subscriberContext);
            }
            catch (Exception exception)
            {
                result.AddValidationMessage(exception.Message);
                return result;
            }

            return result;
        }
        #endregion

        #region abstract for override
        abstract public ValidationResult ValidateSubscriber();

        abstract public ValidationResult ExecuteSubscriber(SubscriberContext context);
        #endregion
    }
}
