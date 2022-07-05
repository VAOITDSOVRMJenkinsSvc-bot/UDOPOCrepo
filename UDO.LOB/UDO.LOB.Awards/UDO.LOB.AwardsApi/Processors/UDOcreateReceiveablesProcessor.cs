using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using UDO.LOB.Awards.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Messages.ClaimantService;

namespace UDO.LOB.Awards.Processors
{
    class UDOcreateReceivablesProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateReceivablesProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateReceivablesRequest request)
        {
            //var request = message as createReceivablesRequest;
            UDOcreateReceivablesResponse response = new UDOcreateReceivablesResponse();
            request.MessageId = request.MessageId;
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

            progressString = "After CRM Connection";

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
                    findOtherAwardInformationRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                //REM: VEIS Web Api Call
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


                var receivablesCount = 0;
                var requestCollection = new OrganizationRequestCollection();

                if (findOtherAwardInformationResponse.VEISfoawdinforeturnInfo.VEISfoawdinfoawardInfoInfo.VEISfoawdinforeceivablesInfo != null)
                {
                    var receivable = findOtherAwardInformationResponse.VEISfoawdinforeturnInfo.VEISfoawdinfoawardInfoInfo.VEISfoawdinforeceivablesInfo;
                    foreach (var receivableItem in receivable)
                    {
                        var responseIds = new UDOcreateReceivablesMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        thisNewEntity.LogicalName = "udo_receivable";
                        if (receivableItem.mcs_discoveryDate != string.Empty)
                        {
                            DateTime newDateTime;
                            var newDate = dateStringFormat(receivableItem.mcs_discoveryDate);
                            if (DateTime.TryParse(newDate, out newDateTime))
                            {
                                thisNewEntity["udo_discoverydate"] = newDateTime;
                            }
                        }
                        if (receivableItem.mcs_code != string.Empty)
                        {
                            thisNewEntity["udo_type"] = receivableItem.mcs_code;
                        }
                        if (receivableItem.mcs_balance != string.Empty)
                        {
                            thisNewEntity["udo_balanceamount"] = moneyStringFormat(receivableItem.mcs_balance);
                        }
                        if (receivableItem.mcs_name != string.Empty)
                        {
                            thisNewEntity["udo_name"] = receivableItem.mcs_name;
                        }
                        if (request.UDOcreateReceivablesRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateReceivablesRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createData);
                        receivablesCount += 1;
                    }
                }
                #region Create records

                if (receivablesCount > 0)
                {
                    var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);

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

                string logInfo = string.Format("receivables Records Created: {0}", receivablesCount);
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "receivables Records Created", logInfo, request.Debug);
                #endregion

                //added to generated code
                if (request.AwardId != null)
                {
                    var parent = new Entity();
                    parent.Id = request.AwardId;
                    parent.LogicalName = "udo_award";
                    parent["udo_receivablescomplete"] = true;
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateReceivablesProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Award receievable Data";
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