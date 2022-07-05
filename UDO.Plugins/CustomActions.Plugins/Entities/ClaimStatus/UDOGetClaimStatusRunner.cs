using System;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
//using VRM.Integration.UDO.Claims.Messages;
using VRMRest;
using Microsoft.Xrm.Sdk.Query;
using UDO.Model;
using UDO.LOB.Core;
using UDO.LOB.Claims.Messages;

//using VRM.Integration.UDO.Common.Messages;

namespace CustomActions.Plugins.Entities.ClaimStatus
{
    public class UDOGetClaimStatusRunner : UDOActionRunner
    {
        #region Members
        protected Guid _veteranId = new Guid();
        protected Guid _claimIdGuid = new Guid();
        protected Int64 _claimId = 0;
        protected Guid _ownerId = new Guid();
        protected string _ownerType; 

        #endregion

        public UDOGetClaimStatusRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_claimlogtimer";
            _logSoapField = "udo_claimlogsoap";
            _debugField = "udo_claim";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_claimstatusvimttimeout";
            _validEntities = new string[] { "udo_claim" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            _method = "DoAction";
            _claimIdGuid = Parent.Id;

            if (!DidWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();
            UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            var veteranReference = new UDOcreateUDOStatusRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_claimReference = new UDOcreateUDOStatusRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_claimid",
                RelatedEntityId = _claimIdGuid,
                RelatedEntityName = "udo_claim"
            };
            var references = new[] { veteranReference, udo_claimReference };

            var request = new UDOcreateUDOStatusRequest();
            request.UDOcreateUDOStatusRelatedEntitiesInfo = references;
            request.udo_claimId = _claimIdGuid;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.Debug = _debug;
            request.RelatedParentEntityName = "udo_claim";
            request.RelatedParentFieldName = "udo_claimid";
            request.RelatedParentId = request.udo_claimId;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            request.claimId = _claimId;
            //  //Logger.WriteDebugMessage("Request Created");
            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            tracer.Trace("calling UDOcreateUDOStatusRequest");
            var response = Utility.SendReceive<UDOcreateUDOStatusResponse>(_uri, "UDOcreateUDOStatusRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateUDOStatusRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Claim Status LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
            }       
        }

        private bool DidWeNeedData()
        {
            try
            {
                tracer.Trace("DidWeNeedData started");
                Logger.setMethod = "DidWeNeedData";
                var gotData = false;


                var retrievedClaim = OrganizationService.Retrieve(udo_claim.EntityLogicalName, _claimIdGuid, new ColumnSet("udo_callcomplete", "udo_statuscomplete", "udo_veteranid", "udo_claimidentifier","ownerid"));

                if (retrievedClaim != null)
                {
                    gotData = true;
                    var claim = retrievedClaim.ToEntity<udo_claim>();

                    if (claim.udo_CallComplete != null)
                    {
                        if (claim.udo_CallComplete.Value)
                        {
                            _responseMessage = "Call complete. Cannot get Claim statuses.";
                            return false;
                        }
                    }
                    if (claim.udo_statuscomplete != null)
                    {
                        if (claim.udo_statuscomplete.Value)
                        {
                            Complete = true;
                            return false;
                        }
                    }
                    if (claim.udo_VeteranId != null)
                    {
                        _veteranId = claim.udo_VeteranId.Id;
                    }
                    if (claim.udo_ClaimIdentifier != null)
                    {
                        _claimId = claim.udo_ClaimIdentifier.Value;
                    }
                    else
                    {
                        _responseMessage = "No claimID. Cannot get Claim statuses.";
                        ////Logger.WriteDebugMessage("No claimID, can't get status");
                        tracer.Trace("No claimID, can't get status");
                        return false;
                    }
                    if (claim.OwnerId != null)
                    {
                        _ownerType = claim.OwnerId.LogicalName;
                        _ownerId = claim.OwnerId.Id;
                    }
                    else
                    {
                        return false;
                    }
                }
                // //Logger.WriteDebugMessage("Ending didWeNeedData Method");
                Logger.setMethod = "Execute";

                return gotData;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}. CorrelationId: {1}".Replace("{0}", ex.Message).Replace("{1}",PluginExecutionContext.CorrelationId.ToString()));
            }
        }
    }
}
