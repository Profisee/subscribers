using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Profisee.Common.EventProcessing.Enums;
using Profisee.Common.EventProcessing.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

/// <summary>
/// This Subscriber will put a message on the Azure Service Bus that contains a custom Instance ID then,
/// based on the Event Trigger, the Entity Name and the Code for the record
/// </summary>
namespace Profisee.EventSubscribers.AzureServiceBusProducerSubscriber
{
    [Export(typeof(ISubscribe))]
    public class AzureServiceBusProducerSubscriber : SubscriberBase
    {
        public const string ConnectionStringPropertyName = "Connection String";
        public const string TopicOrQueuePropertyName = "Topic or Queue Name";
        public const string InstanceIdentifierPropertyName = "InstanceIdentifier";

        private string _connectionString;
        private string _topicOrQueue;
        private string _instanceId;


        public AzureServiceBusProducerSubscriber() : base(nameof(AzureServiceBusProducerSubscriber))
        {
            this.AddConfigurationProperty(new ConfigurationProperty
            {
                Name = ConnectionStringPropertyName,
                Description = "See readme",
                Type = EventSubscriberConfigurationPropertyType.Text,
                IsRequired = true,
            });

            this.AddConfigurationProperty(new ConfigurationProperty
            {
                Name = TopicOrQueuePropertyName,
                Description = "Topic or Queue to publish to",
                Type = EventSubscriberConfigurationPropertyType.Text,
                IsRequired = true,
            });

            this.AddConfigurationProperty(new ConfigurationProperty
            {
                Name = InstanceIdentifierPropertyName,
                Description = "A unique ID for the Profisee istance",
                Type = EventSubscriberConfigurationPropertyType.Text,
                IsRequired = false,
            });
        }


        public override Profisee.Common.EventProcessing.DataContracts.ValidationResult ExecuteSubscriber(SubscriberContext context)
        {
            var result = new Common.EventProcessing.DataContracts.ValidationResult();

            ServiceBusClient client = null;
            try
            {
                client = new ServiceBusClient(_connectionString);
                ServiceBusSender sender = client.CreateSender(_topicOrQueue);
                var msgBody = JsonConvert.SerializeObject(GetMessageDictionary(context));
                ServiceBusMessage message = new ServiceBusMessage(msgBody);
                message.ContentType = "application/json";
                var sendTask = sender.SendMessageAsync(message);
                sendTask.Wait();
            }
            catch (Exception ex)
            {
                var errors = new Collection<MasterDataMaestro.Services.DataContracts.Error>();
                errors.Add(new MasterDataMaestro.Services.DataContracts.Error() { Code = "5000", Description = ex.Message });
                result.AppendErrors(errors);
            }
            finally
            {
                client.DisposeAsync();
            }

            return result;
        }

        public override Profisee.Common.EventProcessing.DataContracts.ValidationResult ValidateSubscriber()
        {
            var result = new Common.EventProcessing.DataContracts.ValidationResult();

            _connectionString = this.ConfigurationPropertyValues.GetEventSubscriberConfigurationPropertyValueAsString(ConnectionStringPropertyName);
            _topicOrQueue = this.ConfigurationPropertyValues.GetEventSubscriberConfigurationPropertyValueAsString(TopicOrQueuePropertyName);
            _instanceId = this.ConfigurationPropertyValues.GetEventSubscriberConfigurationPropertyValueAsString(InstanceIdentifierPropertyName);

            return result;
        }

        private Dictionary<string, string> GetMessageDictionary(SubscriberContext context)
        {
            var msgDict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(_instanceId))
                msgDict.Add("InstanceId", _instanceId);
            msgDict.Add("Entity", context.ExecuteContext.Member.EntityId.Name);
            msgDict.Add("Code", context.ExecuteContext.Member.MemberCode);
            return msgDict;
        }
    }
}
