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

/// <summary>
/// VIMT LOB Component for UDOcreateAwardLines,createAwardLines method, Processor.
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace UDO.LOB.Awards.Processors
{
    class UDOcreateAwardLinesProcessor
    {
        private bool _debug { get; set; }

        private const string method = "UDOcreateAwardsLinesProcessor";

        private string LogBuffer { get; set; }
        
        public IMessageBase Execute(UDOcreateAwardLinesRequest request)
        {
            UDOcreateAwardLinesResponse response = new UDOcreateAwardLinesResponse();
            response.MessageId = request.MessageId;
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = "UDOCreateAwardsLinesProcessor",
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
            tLogger.LogEvent("Connect to CRM", "001");
            progressString = "After Connection";

            try
            {
                // prefix = foawdinfofindOtherAwardInformationRequest();
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

                // REM: Invoke VEIS Endpoint
                // var findOtherAwardInformationResponse = findOtherAwardInformationRequest.SendReceive<VIMTfoawdinfofindOtherAwardInformationResponse>(MessageProcessType.Local);
                var findOtherAwardInformationResponse = WebApiUtility.SendReceive<VEISfoawdinfofindOtherAwardInformationResponse>(findOtherAwardInformationRequest, WebApiType.VEIS);
                if (request.LogSoap || findOtherAwardInformationResponse.ExceptionOccurred)
                {
                    if (findOtherAwardInformationResponse.SerializedSOAPRequest != null || findOtherAwardInformationResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findOtherAwardInformationResponse.SerializedSOAPRequest + findOtherAwardInformationResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfoawdinfofindOtherAwardInformationRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VIMTfoawdinfofindOtherAwardInformationResponse EC Call";
                tLogger.LogEvent("Web Service Call VIMTfoawdinfofindOtherAwardInformation", "002");
                response.ExceptionMessage = findOtherAwardInformationResponse.ExceptionMessage;
                response.ExceptionOccured = findOtherAwardInformationResponse.ExceptionOccurred;
                var awardLinesCount = 0;
                var requestCollection = new OrganizationRequestCollection();
                // Replaced? VIMTfoawdinforeturnclmsInfo.VIMTfoawdinfoawardLinesclmsInfo = VEISfoawdinforeturnInfo
                //          VEIS missing something. See error for awardLine, the VEISfoawdinforeturnInfo is not enumerated type
                if (findOtherAwardInformationResponse.VEISfoawdinforeturnInfo != null)
                {
                    // Replaced? VIMTfoawdinforeturnclmsInfo.VIMTfoawdinfoawardLinesclmsInfo = VEISfoawdinforeturnInfo
                    var awardLine = findOtherAwardInformationResponse.VEISfoawdinforeturnInfo.VEISfoawdinfoawardLinesInfo;
                    System.Collections.Generic.List<UDOcreateAwardLinesMultipleResponse> UDOcreateAwardLinesArray = new System.Collections.Generic.List<UDOcreateAwardLinesMultipleResponse>();

                    if (awardLine != null)
                    {
                        // REM: VEIS need update for the VEISfoawdinforeturnclmsInfo.VEISfoawdinfoawardLinesclmsInfo
                        foreach (var awardLineItem in awardLine)
                        {
                            var responseIds = new UDOcreateAwardLinesMultipleResponse();
                            //instantiate the new Entity
                            Entity thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_awardline";
                            thisNewEntity["udo_name"] = "Award Line Summary";
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            thisNewEntity["udo_withholdtotal"] = moneyStringFormat(awardLineItem.mcs_witholdingAmt);
                            if (awardLineItem.mcs_instznWthldg != string.Empty)
                            {
                                thisNewEntity["udo_withholdinst"] = awardLineItem.mcs_instznWthldg;
                            }
                            thisNewEntity["udo_withholddrill"] = moneyStringFormat(awardLineItem.mcs_drillWitholding);

                            if (awardLineItem.mcs_totalAward != string.Empty)
                            {
                                thisNewEntity["udo_total"] = moneyStringFormat(awardLineItem.mcs_totalAward);
                            }
                            if (awardLineItem.mcs_spouse != string.Empty)
                            {
                                thisNewEntity["udo_spouse"] = awardLineItem.mcs_spouse;
                            }
                            if (awardLineItem.mcs_schoolChild != string.Empty)
                            {
                                thisNewEntity["udo_schoolchild"] = awardLineItem.mcs_schoolChild;
                            }

                            thisNewEntity["udo_recoup"] = moneyStringFormat(awardLineItem.mcs_recoupTotal);

                            if (awardLineItem.mcs_parentNbr != string.Empty)
                            {
                                thisNewEntity["udo_parentnum"] = awardLineItem.mcs_parentNbr;
                            }

                            thisNewEntity["udo_otheradjustments"] = moneyStringFormat(awardLineItem.mcs_otherAdjustments);

                            thisNewEntity["udo_netaward"] = moneyStringFormat(awardLineItem.mcs_netAward);
                            if (awardLineItem.mcs_minorChild != string.Empty)
                            {
                                thisNewEntity["udo_minorchild"] = awardLineItem.mcs_minorChild;
                            }

                            thisNewEntity["udo_income"] = moneyStringFormat(awardLineItem.mcs_income);

                            if (awardLineItem.mcs_helplessChild != string.Empty)
                            {
                                thisNewEntity["udo_helplesschild"] = awardLineItem.mcs_helplessChild;
                            }
                            if (awardLineItem.mcs_entitlementNm != string.Empty)
                            {
                                thisNewEntity["udo_entitlement"] = awardLineItem.mcs_entitlementNm;
                            }
                            if (awardLineItem.mcs_effectiveDate != string.Empty)
                            {
                                DateTime newDateTime;
                                var newDate = dateStringFormat(awardLineItem.mcs_effectiveDate);
                                if (DateTime.TryParse(newDate, out newDateTime))
                                {
                                    thisNewEntity["udo_effectivedate"] = newDateTime;
                                }
                            }

                            thisNewEntity["udo_crsc"] = moneyStringFormat(awardLineItem.mcs_crscAmt);

                            thisNewEntity["udo_crdp"] = moneyStringFormat(awardLineItem.mcs_crdpAmt);

                            thisNewEntity["udo_amount"] = moneyStringFormat(awardLineItem.mcs_altmnt);

                            if (awardLineItem.mcs_aaHbInd != string.Empty)
                            {
                                thisNewEntity["udo_aahb"] = awardLineItem.mcs_aaHbInd;
                            }
                            var awardLineReasons = awardLineItem.VEISfoawdinfoawardReasonsInfo;
                            var finalReason = "";
                            if (awardLineItem.VEISfoawdinfoawardReasonsInfo != null)
                            {
                                foreach (var awardLineReasonsItem in awardLineReasons)
                                {
                                    if (finalReason != "") finalReason += ", ";
                                    finalReason += awardLineReasonsItem.mcs_name;
                                }
                                thisNewEntity["udo_reasons"] = finalReason;
                            }
                            if (request.UDOcreateAwardLinesRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedItem in request.UDOcreateAwardLinesRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }

                            CreateRequest createExamData = new CreateRequest
                            {
                                Target = thisNewEntity
                            };
                            requestCollection.Add(createExamData);
                            awardLinesCount += 1;
                        }
                    }
                }
                if (awardLinesCount > 0)
                {
                    #region Execute Multiple

                    var result = new ExecuteMultipleHelperResponse { IsFaulted = false };
                    result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);

                    if (_debug)
                    {
                        LogBuffer += result.LogDetail;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, LogBuffer, _debug);
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
                tLogger.LogEvent("Create award lines", "003");
                string logInfo = string.Format("Number of Award Lines Created: {0}", awardLinesCount);
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, logInfo, request.Debug);

                //added to generated code
                if (request.AwardId != Guid.Empty)
                {
                    var award = new Entity();
                    award.Id = request.AwardId;
                    award.LogicalName = "udo_award";
                    award["udo_awardlinescomplete"] = true;
                    OrgServiceProxy.Update(award);
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, award.Id.ToString(), request.Debug);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateAwardLinesProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Award Line Data";
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