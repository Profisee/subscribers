using Confluent.Kafka;
using Newtonsoft.Json;
using Profisee.Common.EventProcessing.DataContracts;
using Profisee.Common.EventProcessing.Enums;
using Profisee.Common.EventProcessing.Interfaces;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;

/// <summary>
/// This Subscriber will put a message on the Kafka Event Streaming Platform, then,
/// based on the Event Trigger, the Entity Name and the Member for the record
/// </summary>
namespace Profisee.EventSubscribers.KafkaProducerSubscriber
{
    [Export(typeof(ISubscribe))]
    public class KafkaProducerSubscriber : SubscriberBase
    {
        public const string ConfigFilePropertyName = "Kafka Configuration File";
        public const string CertDirPropertyName = "CA Cert Directory";
        public const string TopicPropertyName = "Topic Name";

        private string configFilePath = string.Empty;
        private string certDir = string.Empty;
        private string topicName = string.Empty;


        private ClientConfig kafkaClientConfig;

        public KafkaProducerSubscriber() : base(nameof(KafkaProducerSubscriber))
        {
            this.AddConfigurationProperty(new ConfigurationProperty
            {
                Name = ConfigFilePropertyName,
                Type = EventSubscriberConfigurationPropertyType.Text,
                IsRequired = true,
            });

            this.AddConfigurationProperty(new ConfigurationProperty
            {
                Name = CertDirPropertyName,
                Type = EventSubscriberConfigurationPropertyType.Text,
                IsRequired = false,
            });

        }

        public override ValidationResult ExecuteSubscriber(SubscriberContext context)
        {
            var result = new ValidationResult();

            try
            {
                using (var producer = new ProducerBuilder<string, string>(kafkaClientConfig).Build())
                {
                    var member = context.ExecuteContext.Member;
                    var jsonMember = JsonConvert.SerializeObject(member);
                    producer.Produce(topicName, new Message<string, string> { Key = member.EntityId.Name, Value = jsonMember }, DeliveryHandler);
                    producer.Flush(TimeSpan.FromSeconds(2));
                }
            }
            catch (Exception exception)
            {
                //Logging.LogException(exception, System.Diagnostics.EventLogEntryType.Error);
                result.ResultMessages.Add(new ValidationMessage() { Description = exception.Message, Type = ErrorType.Error });
            }

            return result;
        }

        public static void DeliveryHandler(DeliveryReport<string, string> report)
        {
            if (report.Error == null)
            {
                var infoMsg = $"Posted {report.Key}/{report.Value} on topic {report.Topic}";
                //Logging.LogMessage(infoMsg, System.Diagnostics.EventLogEntryType.Information);
            }
            else
            {
                var errMsg = $"An error occured for {report.Key}/{report.Value} on topic {report.Topic}: {report.Error}";
                //Logging.LogMessage(errMsg, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        public override ValidationResult ValidateSubscriber()
        {
            var result = new ValidationResult();

            configFilePath = this.ConfigurationPropertyValues.GetEventSubscriberConfigurationPropertyValueAsString(ConfigFilePropertyName);
            certDir = this.ConfigurationPropertyValues.GetEventSubscriberConfigurationPropertyValueAsString(CertDirPropertyName);
            topicName = this.ConfigurationPropertyValues.GetEventSubscriberConfigurationPropertyValueAsString(TopicPropertyName);


            if (File.Exists(this.configFilePath))
            {
                var configDict = File.ReadAllLines(this.configFilePath)
                    .Where(line => !line.StartsWith("#"))
                    .ToDictionary(
                        line => line.Substring(0, line.IndexOf('=')),
                        line => line.Substring(line.IndexOf('=') + 1));

                kafkaClientConfig = new ClientConfig(configDict);

                if (!string.IsNullOrEmpty(certDir))
                {
                    kafkaClientConfig.SslCaLocation = certDir;
                }
            }
            else
            {
                result.AddValidationMessage($"Config file does not exist at location: {configFilePath}");
            }

            return result;
        }
    }
}
