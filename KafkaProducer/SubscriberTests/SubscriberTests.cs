using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Profisee.EventSubscribers.KafkaProducerSubscriber
{
    [TestClass]
    public class SubscriberTests
    {
        private static SubscriberTestContext _subscriberTestContext = null;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            //TODO: set Profisee connection
            _subscriberTestContext = new SubscriberTestContext
            {
                ProfiseeUri = "https://[Profisee App URL]/Profisee/service.svc",
                ClientId = "Client ID"
                //DomainName = "",
                //UserName = "",
                //Password = ""
            };

            //TODO: set configuration property values
            _subscriberTestContext
                .AddConfigurationProperty(KafkaProducerSubscriber.CertDirPropertyName, "A")
                .AddConfigurationProperty(KafkaProducerSubscriber.ConfigFilePropertyName, "A")
                .AddConfigurationProperty(KafkaProducerSubscriber.TopicPropertyName, "A");
        }

        [TestMethod]
        [DataRow("ABC Company", "100000")] //TODO: set entity name and member code
        public void SubscriberTemplateExecute(string entityName, string memberCode)
        {
            var subscriber = new KafkaProducerSubscriber();
            var ValidationResult = subscriber.RunSubscriberTest(_subscriberTestContext, entityName, memberCode);
            Assert.IsFalse(ValidationResult.HasError());
        }
    }
}

