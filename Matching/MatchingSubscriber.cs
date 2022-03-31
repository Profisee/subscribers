using System;
using Profisee.Common.EventProcessing.DataContracts;
using Profisee.Common.EventProcessing.Enums;
using Profisee.Common.EventProcessing.Interfaces;
using System.ComponentModel.Composition;
using Profisee.MasterDataMaestro.Services.DataContracts;
using System.Collections.ObjectModel;
using Profisee.MasterDataMaestro.Services.DataContracts.MasterDataServices;
using Profisee.EventSubscribers.SubscriberBase;

namespace Profisee.EventSubscribers.MatchingSubscriber
{
    [Export(typeof(ISubscribe))]
    public class MatchingSubscriber : SubscriberBase.SubscriberBase
    {
        #region constants
        //DONE: declare configuration property names
        public const string MatchingStrategyPropertyName = "Matching Strategy";
        public const string IncludeSurvivorshipPropertyName = "Include Survivorship";
        public const string RematchProposedMember = "Rematch Proposed Member";
        #endregion

        #region private variables
        //DONE: declare variables to hold configuration property values
        private string _matchingStrategy = string.Empty;
        private bool _includeSurvivorship = false;
        private bool _rematchProposedMember = false;
        private MatchingStrategy _matchingStragety = null;
        #endregion

        #region constructor
        public MatchingSubscriber() : base("MatchingSubscriber")
        {
            //DONE: initiate configuration properties
            this.AddConfigurationProperty(new ConfigurationProperty
            {
                Name = MatchingStrategyPropertyName,
                Description = "The name of the matching strategy to be executed",
                Type = EventSubscriberConfigurationPropertyType.Text,
                IsRequired = true,
            });

            this.AddConfigurationProperty(new ConfigurationProperty
            {
                Name = IncludeSurvivorshipPropertyName,
                Description = "Should the survivroship, if exists, be executed?",
                Type = EventSubscriberConfigurationPropertyType.Bool,
                IsRequired = true,
            });

            this.AddConfigurationProperty(new ConfigurationProperty
            {
                Name = RematchProposedMember,
                Description = "Should the member, if it is propopsed, be rematched?",
                Type = EventSubscriberConfigurationPropertyType.Bool,
                IsRequired = true,
            });

        }
        #endregion

        #region overrides
        public override ValidationResult ValidateSubscriber()
        {
            var result = new ValidationResult();

            //DONE: set configuration property values
            _matchingStrategy = this.ConfigurationPropertyValues.GetEventSubscriberConfigurationPropertyValueAsString(MatchingStrategyPropertyName);
            _includeSurvivorship = Convert.ToBoolean(this.ConfigurationPropertyValues.GetEventSubscriberConfigurationPropertyValue(IncludeSurvivorshipPropertyName));
            _rematchProposedMember = Convert.ToBoolean(this.ConfigurationPropertyValues.GetEventSubscriberConfigurationPropertyValue(RematchProposedMember));

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
            if (_matchingStragety == null)
            {
                _matchingStragety = context.MdmModel.GetMatchingStrategy(_matchingStrategy);
                if (_matchingStragety == null)
                {
                    result.AddValidationMessage($"Cannot find the matching strategy {_matchingStragety}");
                    return result;
                }
            }

            var attributes = new Collection<string> { _matchingStragety.MatchGroupAttributeID.Name, _matchingStragety.MatchStatusAttributeID.Name };

            var member = context.MdmEntity.GetMdmMember(context.ExecuteContext.Member.MemberCode, attributes);

            // clear
            if (_rematchProposedMember)
            {
                var matchStatus = member.GetMemberValueAsMemberIdentifier(_matchingStragety.MatchStatusAttributeID.Name);
                var matchGroup = member.GetMemberValueAsString(_matchingStragety.MatchGroupAttributeID.Name);

                if (matchStatus != null && matchStatus.Code == "30")
                {
                    var unmatchResult = context.MdmModel.UnmatchMembers(_matchingStragety.Identifier, new Collection<MemberIdentifier> { member.MemberId });
                    if (result.AppendErrors(unmatchResult).HasError())
                    {
                        return result;
                    }

                    if (!string.IsNullOrWhiteSpace(matchGroup))
                    {
                        var survivorshpResult = context.MdmModel.SurviveMatchGroup(_matchingStragety.Identifier, matchGroup);
                        if (result.AppendErrors(survivorshpResult).HasError())
                        {
                            return result;
                        }
                    }
                }
            }

            // matching
            member = context.MdmEntity.GetMdmMember(context.ExecuteContext.Member.MemberCode, attributes);
            if (string.IsNullOrWhiteSpace(member.GetMemberValueAsString(_matchingStragety.MatchGroupAttributeID.Name)))
            {
                var matchingResult = context.MdmModel.MatchMember(_matchingStragety.Identifier, member.MemberId);
                if (result.AppendErrors(matchingResult).HasError())
                {
                    return result;
                }
            }

            // survivorship
            if (_includeSurvivorship && _matchingStragety.IncludeSurvivorship)
            {
                member = context.MdmEntity.GetMdmMember(context.ExecuteContext.Member.MemberCode, attributes);
                var matchGroup = member.GetMemberValueAsString(_matchingStragety.MatchGroupAttributeID.Name);
                if (!string.IsNullOrWhiteSpace(matchGroup))
                {
                    var survivorshpResult = context.MdmModel.SurviveMatchGroup(_matchingStragety.Identifier, matchGroup);
                    if (result.AppendErrors(survivorshpResult).HasError())
                    {
                        return result;
                    }
                }
            }

            return result;
        }
        #endregion
    }
}
