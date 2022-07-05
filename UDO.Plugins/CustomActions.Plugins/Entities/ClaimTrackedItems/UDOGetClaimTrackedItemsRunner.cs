using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using UDO.LOB.Claims.Messages;
using UDO.LOB.Core;
using UDO.Model;
using VRMRest;

namespace CustomActions.Plugins.Entities.ClaimsTrackedItems
{
    public class UDOGetClaimTrackedItemsRunner : UDOActionRunner
    {
        #region Members
        protected Guid _veteranId = new Guid();
        protected Guid _claimIdGuid = new Guid();
        protected Int64 _claimId = 0;
        protected Guid _ownerId = new Guid();
        protected string _ownerType;
        #endregion

        public UDOGetClaimTrackedItemsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_claimlogtimer";
            _logSoapField = "udo_claimlogsoap";
            _debugField = "udo_claim";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_trackeditemsvimttimeout";
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

            var request = new UDOcreateUDOTrackedItemsRequest();
            var veteranReference = new UDOcreateUDOTrackedItemsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteran",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_claimReference = new UDOcreateUDOTrackedItemsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_claimid",
                RelatedEntityId = _claimIdGuid,
                RelatedEntityName = "udo_claim"
            };
            var references = new[] { veteranReference, udo_claimReference };
            request.UDOcreateUDOTrackedItemsRelatedEntitiesInfo = references;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.Debug = _debug;
            request.RelatedParentEntityName = "udo_claim";
            request.RelatedParentFieldName = "udo_claimid";
            request.RelatedParentId = _claimIdGuid;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            request.claimId = _claimId;
            request.udo_claimId = _claimIdGuid;
            // //Logger.WriteDebugMessage("Request Created");
            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            tracer.Trace("calling UDOcreateUDOTrackedItemsRequest");
            var response = Utility.SendReceive<UDOcreateUDOTrackedItemsResponse>(_uri, "UDOcreateUDOTrackedItemsRequest", request, _logSettings, _timeOutSetting,_crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateUDOTrackedItemsRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Tracked Items LOB.";

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


                var retrievedClaim = OrganizationService.Retrieve(udo_claim.EntityLogicalName, _claimIdGuid, new ColumnSet("udo_callcomplete", "udo_trackeditemmapdcomplete", "udo_veteranid", "udo_claimidentifier", "ownerid"));

                if (retrievedClaim != null)
                {
                    gotData = true;
                    var claim = retrievedClaim.ToEntity<udo_claim>();

                    if (claim.udo_CallComplete != null)
                    {
                        if (claim.udo_CallComplete.Value)
                        {
                            _responseMessage = "Call is complete. Cannot get Tracked Items.";
                            return false;
                        }
                    }
                    if (claim.udo_trackeditemmapdcomplete != null)
                    {
                        if (claim.udo_trackeditemmapdcomplete.Value)
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
                        _claimId = Convert.ToInt64(claim.udo_ClaimIdentifier);
                    }
                    else
                    {
                        _responseMessage = "No claimID found. Cannot get Tracked Items.";
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
