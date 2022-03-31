using Profisee.Common.EventProcessing.DataContracts;
using Profisee.Common.EventProcessing.Enums;
using Profisee.MasterDataMaestro.Services.DataContracts.MasterDataServices;
using Profisee.Services.Sdk.AcceleratorFramework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Script.Serialization;

namespace Profisee.EventSubscribers
{
    public static class SubscriberHelper
    {
        public static object GetEventSubscriberConfigurationPropertyValue(this List<EventSubscriberConfigurationPropertyValue> propertyValues, string propertyName)
        {
            var propertyValue = propertyValues.FirstOrDefault(p => p.PropertyName == propertyName);

            return propertyValue == null || propertyValue.Value == null ? null : propertyValue.Value;
        }

        public static string GetEventSubscriberConfigurationPropertyValueAsString (this List<EventSubscriberConfigurationPropertyValue> propertyValues, string propertyName)
        {
            var result = propertyValues.GetEventSubscriberConfigurationPropertyValue(propertyName) as string;

            return result ?? string.Empty;
        }

        public static int GetEventSubscriberConfigurationPropertyValueAsInteger(this List<EventSubscriberConfigurationPropertyValue> propertyValues, string propertyName)
        {
            var result = propertyValues.GetEventSubscriberConfigurationPropertyValue(propertyName) as int?;

            return result == null || !result.HasValue ? 0 : result.Value;
        }

        public static void AddValidationMessage(this ValidationResult validationResult, string description, ErrorType errorType = ErrorType.Error)
        {
            validationResult.ResultMessages.Add(new ValidationMessage
            {
                Code = "21000",
                Description = description,
                Type = errorType
            });
        }

        public static ValidationResult AppendErrors(this ValidationResult validationResult, Collection<MasterDataMaestro.Services.DataContracts.Error> errors)
        {
            foreach (var error in errors)
            {
                validationResult.AddValidationMessage(error.Description);
            }

            return validationResult;
        }

        public static void AppendValidationResult(this ValidationResult validationResult, ValidationResult anotherValidationResult)
        {
            foreach(ValidationMessage validationMessage in anotherValidationResult.ResultMessages)
            {
                validationResult.ResultMessages.Add(validationMessage);
            }
        }

        public static bool HasError(this ValidationResult validationResult)
        {
            return validationResult.ResultMessages.Any(m => m.Type == ErrorType.Error | m.Type == ErrorType.SevereError);
        }

        public static string GetMemberValueAsString(this MdmMember mdmMember, string attributeName)
        {
            return mdmMember.GetMemberValue(attributeName) as string;
        }

        public static MemberIdentifier GetMemberValueAsMemberIdentifier(this MdmMember mdmMember, string attributeName)
        {
            return mdmMember.GetMemberValue(attributeName) as MemberIdentifier;
        }

        public static Dictionary<string, object> ToDictionary(this string json)
        {
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Deserialize<dynamic>(json) as Dictionary<string, object>;
        }

        public static string GetValueAsString(this Dictionary<string, object> parameters, string key, string defaultValue = null)
        {
            if (!parameters.ContainsKey(key))
            {
                return defaultValue;
            }

            return parameters[key] as string ?? defaultValue;
        }
    }
}
