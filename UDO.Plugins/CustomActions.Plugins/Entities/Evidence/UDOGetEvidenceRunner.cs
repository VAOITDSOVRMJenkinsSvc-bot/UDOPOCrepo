using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;

using VRMRest;
using UDO.Model;
using System.ServiceModel;
using System.Diagnostics;
using UDO.LOB.Core;
using UDO.LOB.Claims.Messages;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.Claims.Messages;

namespace CustomActions.Plugins.Entities.Evidence
{
    public class UDOGetEvidenceRunner: UDOActionRunner
    {
        #region Members
        protected Guid _veteranId = new Guid();
        protected string _PID = "";
        protected Guid _ownerId = new Guid();
        protected string _ownerType;
        protected Guid _claimId = new Guid();
        #endregion

        public UDOGetEvidenceRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_claimlogtimer";
            _logSoapField = "udo_claimlogsoap";
            _debugField = "udo_claim";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_evidencevimttimeout";
            _validEntities = new string[] { "udo_claim" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            _method = "DoAction";
            _claimId = Parent.Id;

            if (!DidWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();
            UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);


            var request = new UDOcreateUDOEvidenceRequest();

            //  //Logger.WriteDebugMessage("getDebug:" + _debug);

            var veteranReference = new UDOcreateUDOEvidenceRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteran",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_claimReference = new UDOcreateUDOEvidenceRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_claimid",
                RelatedEntityId = _claimId,
                RelatedEntityName = "udo_claim"
            };
            var references = new[] { veteranReference, udo_claimReference };

            request.udo_claimId = _claimId;
            request.UDOcreateUDOEvidenceRelatedEntitiesInfo = references;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.Debug = _debug;
            request.RelatedParentEntityName = "udo_claim";
            request.RelatedParentFieldName = "udo_claimid";
            request.RelatedParentId = _claimId;

            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            request.Claiment_ptpcpnt_id = _PID;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            //  //Logger.WriteDebugMessage("Request Created");

            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            tracer.Trace("calling UDOcreateUDOEvidenceRequest");
            var response = Utility.SendReceive<UDOcreateUDOEvidenceResponse>(_uri, "UDOcreateUDOEvidenceRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateUDOEvidenceRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Evidence LOB.";

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

                var columns = new ColumnSet("udo_evidencecomplete", "udo_callcomplete", "udo_veteranid", "udo_claimidentifier","udo_participantid", "ownerid");

                var retrievedClaim = OrganizationService.Retrieve(udo_claim.EntityLogicalName, _claimId, columns);

                if (retrievedClaim != null)
                {
                    gotData = true;
                    var claimEntity = retrievedClaim.ToEntity<udo_claim>();

                    if (claimEntity.udo_CallComplete != null)
                    {
                        if (claimEntity.udo_CallComplete.Value)
                        {
                            _responseMessage = "Call already complete! Cannot get Evidence data";
                            return false;
                        }
                    }
                    if (claimEntity.udo_evidencecomplete != null)
                    {
                        if (claimEntity.udo_evidencecomplete.Value)
                        {
                            Complete = true;
                            return false;
                        }
                    }
                    if (claimEntity.udo_VeteranId != null)
                    {
                        _veteranId = claimEntity.udo_VeteranId.Id;
                    }
                    if (claimEntity.udo_ParticipantID != null)
                    {
                        _PID = claimEntity.udo_ParticipantID;
                    }
                    else
                    {
                        _responseMessage = "No PID found. Cannot get Evidence data";
                        return false;
                    }
                    if (claimEntity.OwnerId != null)
                    {
                        _ownerType = claimEntity.OwnerId.LogicalName;
                        _ownerId = claimEntity.OwnerId.Id;
                    }
                    else
                    {

                        return false;
                    }
                }
                //  //Logger.WriteDebugMessage("Ending didWeNeedData Method");
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
