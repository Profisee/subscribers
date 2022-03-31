using Profisee.Common.EventProcessing.DataContracts;
using Profisee.Common.EventProcessing.Enums;
using Profisee.Common.EventProcessing.Interfaces;
using Profisee.MasterDataMaestro.Services.DataContracts;
using Profisee.MasterDataMaestro.Services.DataContracts.MasterDataServices;
using Profisee.MasterDataMaestro.Services.MessageContracts;
using Profisee.Services.Sdk.AcceleratorFramework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace Profisee.EventSubscribers.MultiAddressVerificationSubscriber
{
    [Export(typeof(ISubscribe))]
    public class MultiAddressVerificationSubscriber : SubscriberBase
    {
        public const string StrategiesCsvPropertyName = "Address Strategies";
        public const string StrategiesCsvPropertyDescription = "CSV List of Address Strategies";
        
        private string _addressStrategiesCsv;
        private List<AddressStrategy> _addressStrategies;

        public MultiAddressVerificationSubscriber() :  base(nameof(MultiAddressVerificationSubscriber))
        {
            this.AddConfigurationProperty(new ConfigurationProperty
            {
                Name = StrategiesCsvPropertyName,
                Description = StrategiesCsvPropertyDescription,
                Type = EventSubscriberConfigurationPropertyType.Text,
                IsRequired = true,
            });
        }

        private List<AddressStrategy> GetAddressStrategies(List<string> addressStrategyNames, MdmModel model)
        {
            var returnVal = new List<AddressStrategy>();
            var getAddressStrategiesResponse = model.GetAddressStrategies(new MasterDataMaestro.Services.MessageContracts.GetAddressStrategiesRequest());
            var allAddressStrategies = getAddressStrategiesResponse.Strategies;
            return allAddressStrategies.Where(x => addressStrategyNames.Contains(x.Identifier.Name)).ToList();
        }

        private GetAddressResponse ProcessAddressVerification(MdmModel model, AddressStrategy strategy, Member member)
        {
            var extSvcTypeColl = new Collection<ExternalServiceType>();
            if (strategy.MailingVerificationGroup != FieldMappingGroupType.Unknown)
                extSvcTypeColl.Add(ExternalServiceType.MailingAddressVerification);
            if (strategy.NameParsingGroup != FieldMappingGroupType.Unknown)
                extSvcTypeColl.Add(ExternalServiceType.NameParsing);
            if (strategy.PhoneVerificationGroup != FieldMappingGroupType.Unknown)
                extSvcTypeColl.Add(ExternalServiceType.PhoneVerification);
            if (strategy.EmailVerificationGroup != FieldMappingGroupType.Unknown)
                extSvcTypeColl.Add(ExternalServiceType.EmailVerification);
            return model.GetAddress(strategy.Identifier, member, extSvcTypeColl);
        }

        public override ValidationResult ExecuteSubscriber(SubscriberContext context)
        {
            var result = new ValidationResult();

            if (_addressStrategies == null)
            {
                var addressStratgiesList = _addressStrategiesCsv.Split(',').ToList();
                _addressStrategies = GetAddressStrategies(addressStratgiesList, context.MdmModel);
            }

            var member = new Member(context.ExecuteContext.Member.MemberCode);
            var attributeSet = new HashSet<Attribute>();
            Parallel.ForEach(_addressStrategies, addressStrategy =>
            {
                var getAddressResponse = ProcessAddressVerification(context.MdmModel, addressStrategy, member);
                if (getAddressResponse.OperationResult.HasErrors)
                    result.AppendErrors(getAddressResponse.OperationResult.Errors);
                foreach (var attribute in getAddressResponse.Members[0].Attributes) { attributeSet.Add(attribute); }
            });

            foreach (var attribute in attributeSet) { member.Attributes.Add(attribute); }

            var errors = context.MdmEntity.MergeMember(member);
            if (errors.Count > 0)
                result.AppendErrors(errors);

            return result;
        }


        public override ValidationResult ValidateSubscriber()
        {
            _addressStrategiesCsv = this.ConfigurationPropertyValues.GetEventSubscriberConfigurationPropertyValueAsString(StrategiesCsvPropertyName);
            return new ValidationResult();
        }
    }
}
