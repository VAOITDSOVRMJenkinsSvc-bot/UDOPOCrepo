using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
//using VRM.Integration.UDO.Claims.Messages;
using VRMRest;
using UDO.Model;
using UDO.LOB.Core;
using UDO.LOB.Claims.Messages;
using System.ServiceModel;
using System.Diagnostics;

//using VRM.Integration.UDO.Common.Messages;

namespace CustomActions.Plugins.Entities.LifeCycles
{
    public class UDOGetLifeCyclesRunner : UDOActionRunner
    {
        #region Members

        Guid _veteranId = new Guid();
        Guid _claimIdGuid = new Guid();
        Int64 _claimId = 0;
        Guid _ownerId = new Guid();
        string _ownerType;
        #endregion

        public UDOGetLifeCyclesRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_claimlogtimer";
            _logSoapField = "udo_claimlogsoap";
            _debugField = "udo_claim";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_lifecyclesvimttimeout";
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

            var request = new UDOcreateUDOLifecyclesRequest();
            request.udo_claimId = _claimIdGuid;

            var veteranReference = new UDOcreateUDOLifecyclesRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteran",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_claimReference = new UDOcreateUDOLifecyclesRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_claimid",
                RelatedEntityId = request.udo_claimId,
                RelatedEntityName = "udo_claim"
            };
            var references = new[] { veteranReference, udo_claimReference };
            request.UDOcreateUDOLifecyclesRelatedEntitiesInfo = references;

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

            tracer.Trace("calling UDOcreateUDOLifecyclesRequest");
            var response = Utility.SendReceive<UDOcreateUDOLifecyclesResponse>(_uri, "UDOcreateUDOLifecyclesRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateUDOLifecyclesRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Life Cycles Request LOB.";

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

                var columns = new ColumnSet("udo_lifecyclecomplete", "udo_callcomplete", "udo_veteranid", "udo_claimidentifier", "ownerid");

                var retrievedClaim = OrganizationService.Retrieve(udo_claim.EntityLogicalName, _claimIdGuid, columns);

                if (retrievedClaim != null)
                {
                    gotData = true;
                    var claimEntity = retrievedClaim.ToEntity<udo_claim>();

                    if (claimEntity.udo_CallComplete != null)
                    {
                        if (claimEntity.udo_CallComplete.Value)
                        {
                            _responseMessage = "Call is complete. Cannot get Life Cycles.";
                            return false;
                        }
                    }
                    if (claimEntity.udo_lifecyclecomplete.HasValue)
                    {
                        if (claimEntity.udo_lifecyclecomplete.Value)
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
                        _claimId = claimEntity.udo_ClaimIdentifier.Value;
                    }
                    else
                    {
                        _responseMessage = "No claimID, can't get lifecycles";
                        ////Logger.WriteDebugMessage("No claimID, can't get lifecycles");
                        tracer.Trace("No claimID, can't get lifecycles");
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
                // //Logger.WriteDebugMessage("Ending didWeNeedData Method");
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
