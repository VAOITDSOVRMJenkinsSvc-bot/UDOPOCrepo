using System;
using System.Linq;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

using VRMRest;
using UDO.Model;
using UDO.LOB.Core;
using UDO.LOB.Awards.Messages;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.Awards.Messages;

namespace CustomActions.Plugins.Entities.IncomSummary
{
    public class UDOGetIncomeSummaryRunner : UDOActionRunner
    {
        protected Guid _awardId = new Guid();
        protected Guid _ownerId = new Guid();
        protected Guid _veteranId = new Guid();
        protected string _ownerType = string.Empty;
        protected string _awardTypeCode = string.Empty;
        protected string _beneId = string.Empty;
        protected string _recipId = string.Empty;
        protected string _PID = string.Empty;

        public UDOGetIncomeSummaryRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_awardlogtimer";
            _logSoapField = "udo_awardlogsoap";
            _debugField = "udo_award";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_evrsvimttimeout";
            _validEntities = new string[] { "udo_award" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            _method = "DoAction";
            _awardId = Parent.Id;

            if (!DidWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();
            UDOHeaderInfo _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            var request = new UDOcreateIncomeSummaryRequest();
            request.AwardId = _awardId;

            var veteranReference = new UDOcreateIncomeSummaryRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_awardReference = new UDOcreateIncomeSummaryRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_awardid",
                RelatedEntityId = _awardId,
                RelatedEntityName = "udo_award"
            };
            var references = new[] { veteranReference, udo_awardReference };
            request.UDOcreateIncomeSummaryRelatedEntitiesInfo = references;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.RelatedParentEntityName = "udo_award";
            request.RelatedParentFieldName = "udo_awardid";
            request.RelatedParentId = _awardId;
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.ptcpntBeneId = _beneId;
            request.ptcpntVetId = _PID;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            tracer.Trace("calling UDOcreateIncomeSummaryRequest");
            var response = Utility.SendReceive<UDOcreateIncomeSummaryResponse>(_uri, "UDOcreateIncomeSummaryRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateIncomeSummaryRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Income Summary LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
            }
        }

        private bool DidWeNeedData()
        {
            try
            {
                tracer.Trace("DidWeNeedData started");
                Logger.setMethod = "DidWeNeedData";
                var gotData = false;


                using (var xrm = new UDOContext(OrganizationService))
                {
                    gotData = true;

                    var getParent = from awd in xrm.udo_awardSet
                                    join vet in xrm.ContactSet on awd.udo_VeteranId.Id equals vet.ContactId.Value
                                    where awd.udo_awardId.Value == _awardId
                                    select new
                                    {
                                        awd.udo_incomesummarycomplete,
                                        awd.udo_CallComplete,
                                        awd.udo_VeteranId,
                                        awd.udo_PtcpntBeneID,
                                        awd.udo_PtcpntRecipID,
                                        vet.udo_ParticipantId,
                                        vet.OwnerId

                                    };
                    foreach (var awd in getParent)
                    {
                        if (awd.OwnerId != null)
                        {
                            _ownerType = awd.OwnerId.LogicalName;
                            _ownerId = awd.OwnerId.Id;
                        }
                        else
                        {

                            return false;
                        }
                        if (awd.udo_CallComplete != null)
                        {
                            if (awd.udo_CallComplete.Value)
                            {
                                _responseMessage = "Call complete. Cannot retrieve Income Summary.";
                                return false;
                            }
                        }
                        if (awd.udo_incomesummarycomplete != null)
                        {
                            if (awd.udo_incomesummarycomplete.Value)
                            {
                                Complete = true;
                                return false;
                            }
                        }
                        if (awd.udo_VeteranId != null)
                        {
                            _veteranId = awd.udo_VeteranId.Id;
                        }

                        else
                        {
                            _responseMessage = "No Veteran ID. Cannot retrieve Income Summary.";
                            return false;
                        }
                        if (awd.udo_PtcpntBeneID != null)
                        {
                            _beneId = awd.udo_PtcpntBeneID;
                        }
                        else
                        {
                            _responseMessage = "Participant Benefit Id not found. Cannot retrieve Income Summary.";
                            return false;
                        }
                        if (awd.udo_ParticipantId != null)
                        {
                            _PID = awd.udo_ParticipantId;
                        }
                        else
                        {
                            _responseMessage = "Participant Id not found. Cannot retrieve Income Summary.";
                            return false;
                        }

                    }
                }

                Logger.setMethod = "Execute";

                return gotData;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
    }
}
