using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Linq;
using VIMT.ClaimManagementService.Messages;
using VRM.Integration.Common;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Claims.Messages;
using VRM.Integration.UDO.Common;

/// <summary>
/// VIMT LOB Component for UDOcreateUDOStatus,createUDOStatus method, Processor.
/// Code Generated by IMS on: 9/3/2015 2:11:24 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.Claims.Processors
{
    class UDOcreateUDOStatusProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateUDOStatusProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateUDOStatusRequest request)
        {
            //var request = message as createUDOStatusRequest;
            UDOcreateUDOStatusResponse response = new UDOcreateUDOStatusResponse();
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            OrganizationServiceProxy OrgServiceProxy;

            #region connect to CRM
            try
            {
                var CommonFunctions = new CRMCommonFunctions();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, string.Format("{0} Processor, Connection Error", method), connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                // prefix = fndClmStatfindClaimStatusRequest();
                var findClaimStatusRequest = new VIMTfndClmStatfindClaimStatusRequest();
                findClaimStatusRequest.LogTiming = request.LogTiming;
                findClaimStatusRequest.LogSoap = request.LogSoap;
                findClaimStatusRequest.Debug = request.Debug;
                findClaimStatusRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findClaimStatusRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findClaimStatusRequest.RelatedParentId = request.RelatedParentId;
                findClaimStatusRequest.UserId = request.UserId;
                findClaimStatusRequest.OrganizationName = request.OrganizationName;

                findClaimStatusRequest.mcs_claimid = request.claimId;

                var findClaimStatusResponse = findClaimStatusRequest.SendReceive<VIMTfndClmStatfindClaimStatusResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = findClaimStatusResponse.ExceptionMessage;
                response.ExceptionOccured = findClaimStatusResponse.ExceptionOccured;
                if (findClaimStatusResponse.VIMTfndClmStatClaimStatusclmMngmInfo!=null)
                {
                    if (findClaimStatusResponse.VIMTfndClmStatClaimStatusclmMngmInfo.VIMTfndClmStatclaimLifecycleStatusListclmMngmInfo != null)
                    {
                        var requestCollection = new OrganizationRequestCollection();
                        var count = 0;
                        var claimLifecycleStatus = findClaimStatusResponse.VIMTfndClmStatClaimStatusclmMngmInfo.VIMTfndClmStatclaimLifecycleStatusListclmMngmInfo;
                        foreach (var claimLifecycleStatusItem in claimLifecycleStatus)
                        {
                            count += 1;
                            var responseIds = new UDOcreateUDOStatusMultipleResponse();
                            //instantiate the new Entity
                            Entity thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_claimstatus";
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            if (!string.IsNullOrEmpty(claimLifecycleStatusItem.mcs_jrnSttTc))
                            {
                                thisNewEntity["udo_status"] = claimLifecycleStatusItem.mcs_bnftClmLcSttTn;
                            }
                            if (!string.IsNullOrEmpty(claimLifecycleStatusItem.mcs_clmId))
                            {
                                thisNewEntity["udo_claimidstring"] = claimLifecycleStatusItem.mcs_clmId;
                            }
                            if (claimLifecycleStatusItem.mcs_chngdDtSpecified)
                            {
                                thisNewEntity["udo_changedate"] = claimLifecycleStatusItem.mcs_chngdDt;
                            }
                            if (!string.IsNullOrEmpty(claimLifecycleStatusItem.mcs_actionLctnId))
                            {
                                thisNewEntity["udo_actionlocation"] = claimLifecycleStatusItem.mcs_actionLctnId;
                            }
                            //not mapped thisNewEntity["udo_dayinstatus"]=??
                            if (request.UDOcreateUDOStatusRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedItem in request.UDOcreateUDOStatusRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }
                            CreateRequest createSuspenseRecord = new CreateRequest
                            {
                                Target = thisNewEntity
                            };
                            requestCollection.Add(createSuspenseRecord); 
                            
                        }
                        #region Create records

                        if (requestCollection.Count() > 0)
                        {
                            var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, request.UserId, request.Debug);

                            if (_debug)
                            {
                                LogBuffer += result.LogDetail;
                                LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                            }

                            if (result.IsFaulted)
                            {
                                LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                                response.ExceptionMessage = result.FriendlyDetail;
                                response.ExceptionOccured = true;
                                return response;
                            }
                        }

                        string logInfo = string.Format("Claim Status Records Created: {0}", requestCollection.Count());
                        LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Claim Status Records Created", logInfo);
                        #endregion

                    }
                }
                //added to generated code
                if (request.udo_claimId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_claimId;
                    parent.LogicalName = "udo_claim";
                    parent["udo_statuscomplete"] = true;
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Claim Status Data"; 
                response.ExceptionOccured = true;
                return response;
            }
        }
    }
}