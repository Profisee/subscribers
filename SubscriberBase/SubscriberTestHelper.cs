using Profisee.Common.EventProcessing.DataContracts;
using Profisee.Common.EventProcessing.Interfaces;
using System;
using System.Collections.Generic;

namespace Profisee.EventSubscribers
{
    public static class SubscriberTest
    {

        public static ValidationResult RunSubscriberTest(this SubscriberBase subscriber, SubscriberTestContext subscriberTestContext, string entityName, string memberCode)
        {
            var result = new ValidationResult();

            subscriberTestContext.EntityName = entityName;
            subscriberTestContext.MemberCode = memberCode;
            var properties = CreateConfigurationPropertyValueList(subscriberTestContext);

            // initialize
            result = (subscriber as ISubscribe).Initialize(properties);
            if (result.HasError())
                return result;

            // excute
            result.AppendValidationResult(subscriber.Execute(CreateExecuteContext(subscriberTestContext)));
            return result;
        }

        private static List<EventSubscriberConfigurationPropertyValue> CreateConfigurationPropertyValueList(SubscriberTestContext subscriberTestContext)
        {
            ConnectionOverride connectionOverride;
            connectionOverride = new ConnectionOverride() { Url = subscriberTestContext.ProfiseeUri };

            var result = new List<EventSubscriberConfigurationPropertyValue>();
            var propertyValue1 = new EventSubscriberConfigurationPropertyValue() { PropertyName =  "Profisee URI", Value = connectionOverride };
            result.Add(propertyValue1);
            var propertyValue2 = new EventSubscriberConfigurationPropertyValue() { PropertyName = "ClientId", Value = subscriberTestContext.ClientId };
            result.Add(propertyValue2);

            result.AddRange(subscriberTestContext.GetProperties());

            return result;
        }

        private static ExecuteContext CreateExecuteContext(SubscriberTestContext subscriberTestContext)
        {
            ExecuteContext executeContext = new ExecuteContext();

            executeContext.Member = new EntityMember();
            executeContext.Member.MemberCode = subscriberTestContext.MemberCode;
            executeContext.Member.EntityId = new Identifier { Name = subscriberTestContext.EntityName, Id = Guid.Empty };

            return executeContext;
        }
    }
}
