using System;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Linq;
using VRMRest;
using System.Net.Http;
using Microsoft.Xrm.Sdk.Query;
using UDO.Model;
using System.ServiceModel;
using System.Diagnostics;
using System.Threading;
using UDO.LOB.Core;
using UDO.LOB.Awards.Messages;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.Contact.Messages;

namespace CustomActions.Plugins.Entities.Dependents
{
    public class UDOGetDependentAwardsRunner : UDOActionRunner
    {
        protected Guid _dependentId = new Guid();
        protected Guid _veteranId = new Guid();
        protected Guid _ownerId = new Guid();
        protected string _ownerType = string.Empty;
        protected string _pid = string.Empty;
        protected string _fileNumber = string.Empty;
        protected UDOHeaderInfo _headerInfo;

        public UDOGetDependentAwardsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_dependentlogtimer";
            _logSoapField = "udo_dependentlogsoap";
            _debugField = "udo_dependent";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_contacttimeout";
            _validEntities = new string[] { "udo_dependant" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            _method = "DoAction";
            _dependentId = Parent.Id;

            if (!DidWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();
            _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);
            GetDependentAwards();

        }

        private bool DidWeNeedData()
        {
            tracer.Trace("DidWeNeedData started");
            Logger.setMethod = "DidWeNeedData";
            var gotData = false;

            var aliasGuid = String.Format("a_{0}", Guid.NewGuid().ToString("N"));
            var depFetch = String.Format(@"<fetch>
                                          <entity name='udo_dependant' >
                                            <attribute name='udo_awardcomplete' />
                                            <attribute name='udo_dependantid' />
                                            <attribute name='udo_veteranid' />
                                            <filter>
                                              <condition attribute='udo_dependantid' operator='eq' value='{0}' />
                                            </filter>
                                            <link-entity name='contact' from='contactid' to='udo_veteranid' alias='{1}'>
                                              <attribute name='udo_ssn' />
                                              <attribute name='udo_participantid' />
                                              <attribute name='ownerid' />
                                            </link-entity>
                                          </entity>
                                        </fetch>", _dependentId, aliasGuid);
            
            
            var response = OrganizationService.RetrieveMultiple(new FetchExpression(depFetch));
            if (response != null && response.Entities.Count() > 0)
            {
                gotData = true;
                var targetDependent = response.Entities[0];

                if (targetDependent.GetAttributeValue<AliasedValue>(string.Format("{0}.udo_participantid", aliasGuid)) != null)
                {
                    _pid = targetDependent.GetAttributeValue<AliasedValue>(string.Format("{0}.udo_participantid", aliasGuid)).Value as string;
                }
                else
                {
                    _responseMessage = "No valid PID found on the associated veteran record. Unable to retrieve awards.";
                    return false;
                }

                if (targetDependent.GetAttributeValue<AliasedValue>(string.Format("{0}.udo_ssn", aliasGuid)) != null)
                {
                    _fileNumber = targetDependent.GetAttributeValue<AliasedValue>(string.Format("{0}.udo_ssn", aliasGuid)).Value as string;
                }
                else
                {
                    _responseMessage = "No valid SSN found on the associated veteran record. Unable to retrieve awards.";
                    return false;
                }

                if (targetDependent.GetAttributeValue<EntityReference>("udo_veteranid") != null)
                {
                    _veteranId = targetDependent.GetAttributeValue<EntityReference>("udo_veteranid").Id;
                }
                else
                {
                    _responseMessage = "No valid Veteran found on this dependent record. Please close this tab and try opening the dependent record again. If this message persists, please contact your application support team.";
                    return false;
                }

                //if (targetDependent.GetAttributeValue<bool>("udo_awardcomplete"))
                //{
                //    Complete = true;
                //    return false;
                //}

                var owner = targetDependent.GetAttributeValue<AliasedValue>(string.Format("{0}.ownerid", aliasGuid)).Value as EntityReference;

                if (owner != null)
                {
                    _ownerId = owner.Id;
                    _ownerType = owner.LogicalName;
                }
            }
            else
            {
                _responseMessage = "Dependent record not correctly identified. Please close this form and reopen to try again.";
            }
            return gotData;
        }

        private void GetDependentAwards()
        {
            
            tracer.Trace("GetAwards started");
            Logger.setMethod = "GetDependentAwards";

            var awardRequest = new UDOcreateAwardsSyncRequest();
            awardRequest.udo_dependentId = _dependentId;
            var dependentReference = new UDOcreateAwardsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_dependentid",
                RelatedEntityId = _dependentId,
                RelatedEntityName = "udo_dependant"
            };

            var veteranReference = new UDOcreateAwardsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            ////Logger.WriteDebugMessage("_vet_contactid:" + _vet_contactid);
            var references = new[] { veteranReference, dependentReference };
            awardRequest.UDOcreateAwardsRelatedEntitiesInfo = references;

            awardRequest.LegacyServiceHeaderInfo = _headerInfo;
            awardRequest.MessageId = PluginExecutionContext.CorrelationId.ToString();

            awardRequest.fileNumber = _fileNumber;
            ////Logger.WriteDebugMessage("_fileNumber:" + _fileNumber);
            awardRequest.Debug = _debug;
            awardRequest.LogSoap = _logSoap;
            awardRequest.LogTiming = _logTimer;
            awardRequest.UserId = PluginExecutionContext.InitiatingUserId;
            awardRequest.OrganizationName = PluginExecutionContext.OrganizationName;
            awardRequest.ownerId = _ownerId;
            awardRequest.ownerType = _ownerType;

            awardRequest.ptcpntVetId = _pid;
            ////Logger.WriteDebugMessage("_PID:" + _PID);

            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = "CustomActions.Plugins.Entities.Dependents.UDOGetDependentAwardsRunner"
            };


            tracer.Trace("Returned from UDOcreateAwardsSyncRequest");
            var response = Utility.SendReceive<UDOcreateAwardsResponse>(_uri, "UDOcreateAwardsSyncRequest", awardRequest, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateAwardsSyncRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Awards Sync Request LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
            }     
        }
    }
}
