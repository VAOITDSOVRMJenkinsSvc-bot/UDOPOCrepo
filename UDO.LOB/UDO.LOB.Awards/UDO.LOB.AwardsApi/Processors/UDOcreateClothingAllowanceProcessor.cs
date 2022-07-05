using global::UDO.LOB.Awards.Messages;
using global::UDO.LOB.Core;
using global::UDO.LOB.Extensions;
using global::UDO.LOB.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using VEIS.Messages.ClaimantService;

namespace UDO.LOB.Awards.Processors
{
    class UDOcreateClothingAllowanceProcessor
    {
        private bool _debug { get; set; }

        private const string method = "UDOcreateClothingAllowanceProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateClothingAllowanceRequest request)
        {
            UDOcreateClothingAllowanceResponse response = new UDOcreateClothingAllowanceResponse { MessageId = request.MessageId };
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                { 
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            TraceLogger tLogger = new TraceLogger(method, request);
            LogBuffer = string.Empty;
            _debug = request.Debug;
            
            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            #region connect to CRM
            CrmServiceClient OrgServiceProxy;
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion
            tLogger.LogEvent("Connected to CRM", "001");

            try
            {                
                //REM: Update VIMTfoawdinfofindOtherAwardInformationRequest to VEISfoawdinfofindOtherAwardInformationRequest
                var findOtherAwardInformationRequest = new VEISfoawdinfofindOtherAwardInformationRequest();

                findOtherAwardInformationRequest.LogTiming = request.LogTiming;
                findOtherAwardInformationRequest.LogSoap = request.LogSoap;
                findOtherAwardInformationRequest.Debug = request.Debug;
                findOtherAwardInformationRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findOtherAwardInformationRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findOtherAwardInformationRequest.RelatedParentId = request.RelatedParentId;
                findOtherAwardInformationRequest.UserId = request.UserId;
                findOtherAwardInformationRequest.OrganizationName = request.OrganizationName;

                findOtherAwardInformationRequest.mcs_ptcpntvetid = request.ptcpntVetId;
                findOtherAwardInformationRequest.mcs_ptcpntbeneid = String.IsNullOrEmpty(request.ptcpntBeneId) ? request.ptcpntVetId : request.ptcpntBeneId;
                findOtherAwardInformationRequest.mcs_ptcpntrecipid = String.IsNullOrEmpty(request.ptcpntRecipId) ? request.ptcpntVetId : request.ptcpntRecipId;
                findOtherAwardInformationRequest.mcs_awardtypecd = request.awardTypeCd;

                if (request.LegacyServiceHeaderInfo != null)
                {
                    findOtherAwardInformationRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }
                
                // REM: Invoke VEIS WebApi
                var findOtherAwardInformationResponse = WebApiUtility.SendReceive<VEISfoawdinfofindOtherAwardInformationResponse>(findOtherAwardInformationRequest, WebApiType.VEIS);
                if (request.LogSoap || findOtherAwardInformationResponse.ExceptionOccurred)
                {
                    if (findOtherAwardInformationResponse.SerializedSOAPRequest != null || findOtherAwardInformationResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findOtherAwardInformationResponse.SerializedSOAPRequest + findOtherAwardInformationResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfoawdinfofindOtherAwardInformationRequest Request/Response {requestResponse}", true);
                    }
                }

                tLogger.LogEvent("WebService Call VEISfoawdinfofindOtherAwardInformation", "002");

                response.ExceptionMessage = findOtherAwardInformationResponse.ExceptionMessage;
                response.ExceptionOccured = findOtherAwardInformationResponse.ExceptionOccurred;

                var clothingCount = 0;
                var requestCollection = new OrganizationRequestCollection();
                if (findOtherAwardInformationResponse.VEISfoawdinforeturnInfo.VEISfoawdinfoclothingAllowanceInfoInfo != null)
                {
                    var clothingAllowanceInfo = findOtherAwardInformationResponse.VEISfoawdinforeturnInfo.VEISfoawdinfoclothingAllowanceInfoInfo;
                    System.Collections.Generic.List<UDOcreateClothingAllowanceMultipleResponse> UDOcreateClothingAllowanceArray = new System.Collections.Generic.List<UDOcreateClothingAllowanceMultipleResponse>();
                    foreach (var clothingAllowanceInfoItem in clothingAllowanceInfo)
                    {
                        var responseIds = new UDOcreateClothingAllowanceMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity("udo_clothingallowance");
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (clothingAllowanceInfoItem.mcs_eligibilityYear != string.Empty)
                        {
                            thisNewEntity["udo_eligibilityyear"] = clothingAllowanceInfoItem.mcs_eligibilityYear;
                        }
                        if (clothingAllowanceInfoItem.mcs_grossAward != string.Empty)
                        {
                            thisNewEntity["udo_grossaward"] = moneyStringFormat(clothingAllowanceInfoItem.mcs_grossAward);
                        }
                        if (clothingAllowanceInfoItem.mcs_incarcerationAdjustment != string.Empty)
                        {
                            thisNewEntity["udo_incarcerationadjustment"] = moneyStringFormat(clothingAllowanceInfoItem.mcs_incarcerationAdjustment);
                        }
                        if (clothingAllowanceInfoItem.mcs_netAward != string.Empty)
                        {
                            thisNewEntity["udo_netaward"] = moneyStringFormat(clothingAllowanceInfoItem.mcs_netAward);
                        }
                        if (request.UDOcreateClothingAllowanceRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateClothingAllowanceRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createExamData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createExamData);
                        clothingCount += 1;
                    }
                }

                if (clothingCount > 0)
                {
                    #region Execute Multiple

                    var result = new ExecuteMultipleHelperResponse();
                    result.IsFaulted = false;
                    result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);

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

                    #endregion
                }
                tLogger.LogEvent("Execute Multiple for Clothing Allowance", "002");
                string logInfo = string.Format("Number of Clothing Allowances Created: {0}", clothingCount);
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "Clothing Allowances Records Created", logInfo, request.Debug);
                //added to generated code
                if (request.AwardId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.AwardId;
                    parent.LogicalName = "udo_award";
                    parent["udo_clothingallowancecomplete"] = true;

                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, ExecutionException);
                response.ExceptionMessage = "Failed to Process clothingallowance Data";
                response.ExceptionOccured = true;
                return response;
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }
        }
        private static string dateStringFormat(string date)
        {
            if (date == null) return null;
            if (date.Length == 10) return date;
            if (date.Length < 8) return date;

            date = date.Insert(2, "/");
            date = date.Insert(5, "/");
            return date;
        }
        private static string moneyStringFormat(string thisField)
        {
            var returnField = "";
            try
            {
                Double newValue = 0;
                if (Double.TryParse(thisField, out newValue))
                {
                    returnField = string.Format("{0:C}", newValue);
                }
                else
                {
                    returnField = "$0.00";
                }
            }
            catch (Exception ex)
            {
                returnField = ex.Message;
            }
            return returnField;

        }
    }
}
