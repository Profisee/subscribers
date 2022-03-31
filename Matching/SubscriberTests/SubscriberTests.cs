using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profisee.EventSubscribers.SubscriberBase;

namespace Profisee.EventSubscribers.MatchingSubscriber.SubscriberTests
{
    [TestClass]
    public class SubscriberTemplateTests
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
                .AddConfigurationProperty(MatchingSubscriber.MatchingStrategyPropertyName, "A")
                .AddConfigurationProperty(MatchingSubscriber.IncludeSurvivorshipPropertyName, "A")
                .AddConfigurationProperty(MatchingSubscriber.RematchProposedMember, "A");
        }

        [TestMethod]
        [DataRow("ABC Company", "100000")] //TODO: set entity name and member code
        public void SubscriberTemplateExecute(string entityName, string memberCode)
        {
            var subscriber = new MatchingSubscriber();
            var ValidationResult = subscriber.RunSubscriberTest(_subscriberTestContext, entityName, memberCode);
            Assert.IsFalse(ValidationResult.HasError());
        }
    }
}

