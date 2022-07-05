using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using VRMRest;
using System.ServiceModel;
using System.Diagnostics;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.Awards.Messages;
using UDO.Model;
using UDO.LOB.Core;
using UDO.LOB.Awards.Messages;

namespace CustomActions.Plugins.Entities.Proceeds
{
    class UDOGetProceedsRunner : UDOActionRunner
    {

        Guid _ownerId = new Guid();
        string _ownerType;
        Guid _veteranId = new Guid();
        Guid _awardId = new Guid();
        string _awardTypeCode = "";
        string _beneId = "";
        string _recipId = "";
        string _PID = "";


        public UDOGetProceedsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_awardlogtimer";
            _logSoapField = "udo_awardlogsoap";
            _debugField = "udo_award";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_awardvimttimeout";
            _validEntities = new string[] { "udo_award" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            _method = "DoAction";
            _awardId = Parent.Id;
            if (!didWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();

            var request = new UDOcreateProceedsRequest();

            var veteranReference = new UDOcreateProceedsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_awardReference = new UDOcreateProceedsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_awardid",
                RelatedEntityId = _awardId,
                RelatedEntityName = "udo_award"
            };
            var references = new[] { veteranReference, udo_awardReference };

            request.UDOcreateProceedsRelatedEntitiesInfo = references;

            UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.RelatedParentEntityName = "udo_award";
            request.RelatedParentFieldName = "udo_awardid";
            request.RelatedParentId = _awardId;
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.AwardId = _awardId;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            request.awardTypeCd = _awardTypeCode;
            request.ptcpntBeneId = _beneId;
            request.ptcpntRecipId = _recipId;
            request.ptcpntVetId = _PID;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            LogSettings _logSettings = new LogSettings() { Org = PluginExecutionContext.OrganizationName, ConfigFieldName = "RESTCALL", UserId = PluginExecutionContext.InitiatingUserId };

            tracer.Trace("calling UDOcreateProceedsRequest");
            var response = Utility.SendReceive<UDOcreateProceedsResponse>(_uri, "UDOcreateProceedsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateProceedsRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Proceeds LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
            }
        }

        internal bool didWeNeedData()
        {
            try
            {
                Logger.setMethod = "didWeNeedData";

                using (var xrm = new UDOContext(OrganizationService))
                {
                    var getParent = (from awd in xrm.udo_awardSet
                                     join vet in xrm.ContactSet on awd.udo_VeteranId.Id equals vet.ContactId.Value
                                     where awd.udo_awardId.Value == _awardId
                                     select new
                                     {
                                         awd.udo_proceedscomplete,
                                         awd.udo_CallComplete,
                                         awd.udo_VeteranId,
                                         awd.udo_AwardTypeCode,
                                         awd.udo_PtcpntBeneID,
                                         awd.udo_PtcpntRecipID,
                                         vet.udo_ParticipantId,
                                         vet.OwnerId

                                     }).FirstOrDefault();
                    if (getParent != null)
                    {
                        if (getParent.udo_CallComplete != null)
                        {
                            if (getParent.udo_CallComplete.Value)
                            {
                                _responseMessage = "Call already completed. Cannot retrieve Proceeds.";
                                return false;
                            }
                        }
                        if (getParent.udo_proceedscomplete != null)
                        {
                            if (getParent.udo_proceedscomplete.Value)
                            {
                                Complete = true;
                                return false;
                            }
                        }
                        if (getParent.udo_VeteranId != null)
                        {
                            _veteranId = getParent.udo_VeteranId.Id;
                        }
                        if (getParent.udo_AwardTypeCode != null)
                        {
                            _awardTypeCode = getParent.udo_AwardTypeCode;
                        }
                        else
                        {
                            _responseMessage = "Award Type Code is missing. Cannot retrieve Proceeds.";
                            return false;
                        }
                        if (getParent.udo_PtcpntBeneID != null)
                        {
                            _beneId = getParent.udo_PtcpntBeneID;
                        }
                        else
                        {
                            _responseMessage = "Beneficiary is missing. Cannot retrieve Proceeds.";
                            return false;
                        }
                        if (getParent.udo_PtcpntRecipID != null)
                        {
                            _recipId = getParent.udo_PtcpntRecipID;
                        }
                        else
                        {
                            _responseMessage = "Recipient is missing. Cannot retrieve Proceeds.";
                            return false;
                        }
                        if (getParent.udo_ParticipantId != null)
                        {
                            _PID = getParent.udo_ParticipantId;
                        }
                        else
                        {
                            _responseMessage = "Participant ID is missing. Cannot retrieve Proceeds.";
                            return false;
                        }
                        if (getParent.OwnerId != null)
                        {
                            _ownerType = getParent.OwnerId.LogicalName;
                            _ownerId = getParent.OwnerId.Id;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                tracer.Trace("didWeNeedData have been retrieved and set");
                Logger.setMethod = "Execute";

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
    }
}