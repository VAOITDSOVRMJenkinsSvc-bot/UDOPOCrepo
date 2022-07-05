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

namespace CustomActions.Plugins.Entities.Contentions
{
    public class UDOGetContentionsRunner : UDOActionRunner
    {
        #region Members
        protected Guid _veteranId = new Guid();
        protected Guid _claimIdGuid = new Guid();
        protected Int64 _claimId = 0;
        protected Guid _ownerId = new Guid();
        protected string _ownerType;
        #endregion


        public UDOGetContentionsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_claimlogtimer";
            _logSoapField = "udo_claimlogsoap";
            _debugField = "udo_claim";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_contentionvimttimeout";
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

            var request = new UDOcreateUdoContentionsRequest();
            var veteranReference = new UDOcreateUdoContentionsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteran",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_claimReference = new UDOcreateUdoContentionsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_claimid",
                RelatedEntityId = _claimIdGuid,
                RelatedEntityName = "udo_claim"
            };
            var references = new[] { veteranReference, udo_claimReference };
            request.UDOcreateUdoContentionsRelatedEntitiesInfo = references;

            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.Debug = _debug;
            request.RelatedParentEntityName = "udo_claim";
            request.RelatedParentFieldName = "udo_claimid";
            request.RelatedParentId = _claimIdGuid;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
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

            tracer.Trace("calling UDOcreateUdoContentionsRequest");
            var response = Utility.SendReceive<UDOcreateUdoContentionsResponse>(_uri, "UDOcreateUdoContentionsRequest", request, _logSettings, _timeOutSetting,_crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateUdoContentionsRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Contentions LOB.";

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

                var columns = new ColumnSet("udo_contentioncomplete", "udo_callcomplete", "udo_veteranid", "udo_claimidentifier", "ownerid");

                var retrievedClaim = OrganizationService.Retrieve(udo_claim.EntityLogicalName, _claimIdGuid, columns);

                if (retrievedClaim != null)
                {
                    gotData = true;
                    var claimEntity = retrievedClaim.ToEntity<udo_claim>();
                    if (claimEntity.udo_CallComplete != null)
                    {
                        if (claimEntity.udo_CallComplete.Value)
                        {
                            _responseMessage = "Call is already complete. Cannot get Contentions.";
                            return false;
                        }
                    }
                    if (claimEntity.udo_contentioncomplete != null)
                    {
                        if (claimEntity.udo_contentioncomplete.Value)
                        {
                            Complete = true;
                            return false;
                        }
                    }
                    if (claimEntity.udo_VeteranId != null)
                    {
                        _veteranId = claimEntity.udo_VeteranId.Id;
                    }
                    if (claimEntity.udo_ClaimIdentifier != null)
                    {
                        _claimId = Convert.ToInt64(claimEntity.udo_ClaimIdentifier);
                    }
                    else
                    {
                        _responseMessage = "Claim Identifier is blank. Cannot get Contentions.";
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

                ////Logger.WriteDebugMessage("Ending didWeNeedData Method");
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
