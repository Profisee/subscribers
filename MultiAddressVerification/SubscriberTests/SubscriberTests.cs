using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Profisee.EventSubscribers.MultiAddressVerificationSubscriber
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
            };

            //TODO: set configuration property values
            _subscriberTestContext
                .AddConfigurationProperty(MultiAddressVerificationSubscriber.StrategiesCsvPropertyDescription, "A")
                .AddConfigurationProperty(MultiAddressVerificationSubscriber.StrategiesCsvPropertyName, "A");
        }

        [TestMethod]
        [DataRow("ABC Company", "100000")] //TODO: set entity name and member code
        public void SubscriberTemplateExecute(string entityName, string memberCode)
        {
            var subscriber = new MultiAddressVerificationSubscriber();
            var ValidationResult = subscriber.RunSubscriberTest(_subscriberTestContext, entityName, memberCode);
            Assert.IsFalse(ValidationResult.HasError());
        }
    }
}

