using System;
using System.Linq;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.Awards.Messages;
using VRMRest;
using UDO.Model;
using UDO.LOB.Core;
using UDO.LOB.Awards.Messages;

namespace CustomActions.Plugins.Entities.Receivables
{
    public class UDOGetReceivablesRunner : UDOActionRunner
    {
        Guid _ownerId = new Guid();
        string _ownerType;
        Guid _awardId = new Guid();
        Guid _veteranId = new Guid();
        string _awardTypeCode = "";
        string _beneId = "";
        string _recipId = "";
        string _PID = "";

        public UDOGetReceivablesRunner(IServiceProvider serviceProvider)
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
            UDOHeaderInfo _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            var request = new UDOcreateReceivablesRequest();

            var veteranReference = new UDOcreateReceivablesRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_awardReference = new UDOcreateReceivablesRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_awardid",
                RelatedEntityId = _awardId,
                RelatedEntityName = "udo_award"
            };
            var references = new[] { veteranReference, udo_awardReference };
            request.AwardId = _awardId;
            request.UDOcreateReceivablesRelatedEntitiesInfo = references;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.RelatedParentEntityName = "udo_award";
            request.RelatedParentFieldName = "udo_awardid";
            request.RelatedParentId = _awardId;
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            request.awardTypeCd = _awardTypeCode;
            request.ptcpntBeneId = _beneId;
            request.ptcpntRecipId = _recipId;
            request.ptcpntVetId = _PID;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;

            LogSettings _logSettings = new LogSettings() { Org = PluginExecutionContext.OrganizationName, ConfigFieldName = "RESTCALL", UserId = PluginExecutionContext.InitiatingUserId };

            tracer.Trace("calling UDOcreateReceivablesRequest");
            var response = Utility.SendReceive<UDOcreateReceivablesResponse>(_uri, "UDOcreateReceivablesRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateReceivablesRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Receivables LOB.";

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
                                        awd.udo_receivablescomplete,
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
                                _responseMessage = "Call already completed. Cannot retrieve Receivables.";
                                return false;
                            }
                        }
                        if (getParent.udo_receivablescomplete != null)
                        {
                            if (getParent.udo_receivablescomplete.Value)
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
                            _responseMessage = "Award Type Code is missing. Cannot retrieve Receivables.";
                            return false;
                        }
                        if (getParent.udo_PtcpntBeneID != null)
                        {
                            _beneId = getParent.udo_PtcpntBeneID;
                        }
                        else
                        {
                            _responseMessage = "Beneficiary is missing. Cannot retrieve Receivables.";
                            return false;
                        }
                        if (getParent.udo_PtcpntRecipID != null)
                        {
                            _recipId = getParent.udo_PtcpntRecipID;
                        }
                        else
                        {
                            _responseMessage = "Recipient is missing. Cannot retrieve Receivables.";
                            return false;
                        }
                        if (getParent.udo_ParticipantId != null)
                        {
                            _PID = getParent.udo_ParticipantId;
                        }
                        else
                        {
                            _responseMessage = "Participant ID is missing. Cannot retrieve Receivables.";
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
