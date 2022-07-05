using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using UDO.LOB.Awards.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Core.Messages;
using VEIS.Messages.ClaimantService;

namespace UDO.LOB.Awards.Processors
{
    class UDOcreateDeductionsProcessor
    {
        private bool _debug { get; set; }

        private const string method = "UDOcreateDeductionsProcessor";

        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateDeductionsRequest request)
        {
            UDOcreateDeductionsResponse response = new UDOcreateDeductionsResponse();
            response.MessageId = request.MessageId;
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
            var progressString = "Top of Processor";
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

            progressString = "After Connection";

            try
            {
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
                findOtherAwardInformationRequest.mcs_ptcpntbeneid = request.ptcpntBeneId;
                findOtherAwardInformationRequest.mcs_ptcpntrecipid = request.ptcpntRecipId;
                findOtherAwardInformationRequest.mcs_awardtypecd = request.awardTypeCd;

                if (request.LegacyServiceHeaderInfo != null)
                {
                    findOtherAwardInformationRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                var findOtherAwardInformationResponse = WebApiUtility.SendReceive<VEISfoawdinfofindOtherAwardInformationResponse>(findOtherAwardInformationRequest, WebApiType.VEIS);
                if (request.LogSoap || findOtherAwardInformationResponse.ExceptionOccurred)
                {
                    if (findOtherAwardInformationResponse.SerializedSOAPRequest != null || findOtherAwardInformationResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findOtherAwardInformationResponse.SerializedSOAPRequest + findOtherAwardInformationResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfoawdinfofindOtherAwardInformationRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VIMT EC Call";

                response.ExceptionMessage = findOtherAwardInformationResponse.ExceptionMessage;
                response.ExceptionOccured = findOtherAwardInformationResponse.ExceptionOccurred;

                var requestCollection = new OrganizationRequestCollection();
                var deductionCount = 0;

                if (findOtherAwardInformationResponse.VEISfoawdinforeturnInfo.VEISfoawdinfoawardInfoInfo.VEISfoawdinfodeductionsInfo != null)
                {

                    var deduction = findOtherAwardInformationResponse.VEISfoawdinforeturnInfo.VEISfoawdinfoawardInfoInfo.VEISfoawdinfodeductionsInfo;
                    System.Collections.Generic.List<UDOcreateDeductionsMultipleResponse> UDOcreateDeductionsArray = new System.Collections.Generic.List<UDOcreateDeductionsMultipleResponse>();
                    foreach (var deductionItem in deduction)
                    {
                        var responseIds = new UDOcreateDeductionsMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_deduction";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (deductionItem.mcs_code != string.Empty)
                        {
                            thisNewEntity["udo_type"] = moneyStringFormat(deductionItem.mcs_code);
                        }
                        if (deductionItem.mcs_balance != string.Empty)
                        {
                            thisNewEntity["udo_balanceamount"] = moneyStringFormat(deductionItem.mcs_balance);
                        }
                        if (deductionItem.mcs_amount != string.Empty)
                        {
                            thisNewEntity["udo_deductionamount"] = moneyStringFormat(deductionItem.mcs_amount);
                        }
                        if (deductionItem.mcs_name != string.Empty)
                        {
                            thisNewEntity["udo_description"] = deductionItem.mcs_name;
                        }
                        if (request.UDOcreateDeductionsRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateDeductionsRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createData);
                        deductionCount += 1;
                    }


                }
                if (deductionCount > 0)
                {
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
                }

                #region Log Results
                
                string logInfo = string.Format("Deductions Records Created: {0}", deductionCount);
                LogHelper.LogDebug(request.MessageId, request.OrganizationName,  request.UserId, "Deductions Records Created", logInfo, request.Debug);
                #endregion


                //added to generated code
                if (request.AwardId != null)
                {
                    var parent = new Entity();
                    parent.Id = request.AwardId;
                    parent.LogicalName = "udo_award";
                    parent["udo_deductioncomplete"] = true;
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateDeductionsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Award Deductions Data";
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
