using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using VIMT.BenefitClaimService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Claims.Messages;
using VRM.Integration.UDO.Common;

namespace VRM.Integration.UDO.Claims.Processors
{
    class UDOcreateUDOSuspenseProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateUDOSuspenseProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateUDOSuspenseRequest request)
        {
            //var request = message as createUDOSuspenseRequest;
            UDOcreateUDOSuspenseResponse response = new UDOcreateUDOSuspenseResponse();
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
                var CommonFunctions = new CRMConnect();
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
                //not mapped thisNewEntity["udo_updateby"]=??
                // prefix = findBenefitClaimDetailRequest();
                var findBenefitClaimDetailRequest = new VIMTfbendtlfindBenefitClaimDetailRequest();
                findBenefitClaimDetailRequest.LogTiming = request.LogTiming;
                findBenefitClaimDetailRequest.LogSoap = request.LogSoap;
                findBenefitClaimDetailRequest.Debug = request.Debug;
                findBenefitClaimDetailRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findBenefitClaimDetailRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findBenefitClaimDetailRequest.RelatedParentId = request.RelatedParentId;
                findBenefitClaimDetailRequest.UserId = request.UserId;
                findBenefitClaimDetailRequest.OrganizationName = request.OrganizationName;

                findBenefitClaimDetailRequest.mcs_benefitclaimid = request.benefitClaimId;

                findBenefitClaimDetailRequest.LegacyServiceHeaderInfo = new VIMT.BenefitClaimService.Messages.HeaderInfo
                {
                    ///Header Info
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber,
                };

                // TODO (TN): Comment to remediate
                var findBenefitClaimDetailResponse = new VIMTfbendtlfindBenefitClaimDetailResponse();
                // var findBenefitClaimDetailResponse = findBenefitClaimDetailRequest.SendReceive<VIMTfbendtlfindBenefitClaimDetailResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = findBenefitClaimDetailResponse.ExceptionMessage;
                response.ExceptionOccured = findBenefitClaimDetailResponse.ExceptionOccured;
                if (findBenefitClaimDetailResponse.VIMTfbendtlbenefitClaimRecordbclmInfo != null)
                {
                    if (findBenefitClaimDetailResponse.VIMTfbendtlbenefitClaimRecordbclmInfo.VIMTfbendtlsuspenceRecordbclmInfo != null)
                    {
                        if (findBenefitClaimDetailResponse.VIMTfbendtlbenefitClaimRecordbclmInfo.VIMTfbendtlsuspenceRecordbclmInfo.VIMTfbendtlsuspenceRecordsbclmInfo != null)
                        {
                            var suspenseRecords = findBenefitClaimDetailResponse.VIMTfbendtlbenefitClaimRecordbclmInfo.VIMTfbendtlsuspenceRecordbclmInfo.VIMTfbendtlsuspenceRecordsbclmInfo;
                            var requestCollection = new OrganizationRequestCollection();
                            var count = 0;
                            foreach (var suspenseRecord in suspenseRecords)
                            {
                                count = ++count;
                                //instantiate the new Entity
                                Entity thisNewEntity = new Entity();
                                thisNewEntity.LogicalName = "udo_claimsuspense";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                if (!string.IsNullOrEmpty(suspenseRecord.mcs_suspenceReasonText))
                                {
                                    thisNewEntity["udo_suspensereason"] = suspenseRecord.mcs_suspenceReasonText;
                                }
                                if (!string.IsNullOrEmpty(suspenseRecord.mcs_claimSuspenceDate))
                                {
                                    var newDate = new System.DateTime();
                                    if (DateTime.TryParse(suspenseRecord.mcs_claimSuspenceDate, out newDate))
                                    {
                                        thisNewEntity["udo_suspensedate"] = newDate;
                                    }
                                }
                                if (!string.IsNullOrEmpty(suspenseRecord.mcs_suspenceActionDate))
                                {
                                    var newDate = new System.DateTime();
                                    if (DateTime.TryParse(suspenseRecord.mcs_suspenceActionDate, out newDate))
                                    {
                                        thisNewEntity["udo_actioncompletedon"] = newDate;
                                    }
                                }
                                if (!string.IsNullOrEmpty(suspenseRecord.mcs_firstName) && !string.IsNullOrEmpty(suspenseRecord.mcs_lastName) && !string.IsNullOrEmpty(suspenseRecord.mcs_journalDate))
                                {
                                    thisNewEntity["udo_updatedby"] = suspenseRecord.mcs_lastName + ", " + suspenseRecord.mcs_firstName + " - " + suspenseRecord.mcs_journalDate;
                                }
                                if (request.UDOcreateUDOSuspenseRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOSuspenseRelatedEntitiesInfo)
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

                            if (requestCollection.Count > 0)
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

                            string logInfo = string.Format("Claim Suspense Records Created: {0}", requestCollection.Count);
                            LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Claim Suspense Records Created", logInfo);
                            #endregion
                        }
                    }
                }

                //added to generated code
                if (request.udo_claimId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_claimId;
                    parent.LogicalName = "udo_claim";
                    parent["udo_suspensecomplete"] = true;
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Claim Suspense Data"; 
                response.ExceptionOccured = true;
                return response;
            }
        }
    }
}