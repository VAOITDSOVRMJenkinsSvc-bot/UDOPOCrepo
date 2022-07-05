
namespace UDO.LOB.VirtualVA.Processors
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Tooling.Connector;
    using System;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions;
    using UDO.LOB.Extensions.Logging;
    using UDO.LOB.VirtualVA.Messages;
    using VEIS.Core.Messages;
    using VEIS.Messages.VVADocOperations;

    class UDOcreateUDOVirtualVAProcessor
    {
        private CrmServiceClient OrgServiceProxy = null;
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOcreateUDOVirtualVAProcessor";

        public IMessageBase Execute(UDOcreateUDOVirtualVARequest request)
        {
            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method,
                $">> Entered {this.GetType().FullName}.Execute", request.Debug);

            UDOcreateUDOVirtualVAResponse response = new UDOcreateUDOVirtualVAResponse { MessageId = request.MessageId };
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = $"{method}: Called with no message";
                response.ExceptionOccurred = true;
                return response;
            }
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = "UDOcreateUDOVirtualVAProcessor",
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            TraceLogger tLogger = new TraceLogger(method, request);

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOcreateUDOVirtualVARequest>(request)}");

            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, $"{method}:CRM Connection Error", connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                // prefix = vvado_GetDocumentListRequest();
                var GetDocumentListRequest = new VEISvvado_GetDocumentListRequest
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    documentlistInfo = new VEISvvado_GetDocumentListRequest.VEISvvado_documentlist { mcs_claimNbr = request.ClaimNbr },
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.HCValue1,
                        Password = request.LegacyServiceHeaderInfo.HCValue2,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    }
                };

                // REM: Invoke VEIS Endpoint
                //var GetDocumentListResponse = GetDocumentListRequest.SendReceive<VIMTvvado_GetDocumentListResponse>(MessageProcessType.Local);
                var GetDocumentListResponse = WebApiUtility.SendReceive<VEISvvado_GetDocumentListResponse>(GetDocumentListRequest, WebApiType.VEIS);

                if (request.LogSoap || GetDocumentListResponse.ExceptionOccurred)
                {
                    if (GetDocumentListResponse.SerializedSOAPRequest != null || GetDocumentListResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = GetDocumentListResponse.SerializedSOAPRequest + GetDocumentListResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISvvado_GetDocumentListRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VEIS EC Call";

                response.ExceptionMessage = GetDocumentListResponse.ExceptionMessage;
                response.ExceptionOccurred = GetDocumentListResponse.ExceptionOccurred;

                // An exception occured, so we can't load data, don't attempt it.
                if (response.ExceptionOccurred)
                {
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().ToString(), "Virtual VA VEIS Call Failed: " + response.ExceptionMessage);
                    return response;
                }

                //had to add something to compile
                if (GetDocumentListResponse.VEISvvado_dcmntRecordCollectionInfo != null)
                {
                    if (GetDocumentListResponse.VEISvvado_dcmntRecordCollectionInfo.VEISvvado_dcmntRecordInfo != null)
                    {

                        var requestCollection = new OrganizationRequestCollection();
                        var docList = GetDocumentListResponse.VEISvvado_dcmntRecordCollectionInfo.VEISvvado_dcmntRecordInfo;


                        var docCount = 0;
                        var totalDocCount = 0;
                        foreach (var doc in docList)
                        {
                            if (doc.mcs_rstrcdDcmntInd.Equals("N", StringComparison.InvariantCultureIgnoreCase))
                            {
                                #region create records
                                docCount += 1;
                                totalDocCount += 1;
                                //instantiate the new Entity
                                Entity thisNewEntity = new Entity();
                                thisNewEntity.LogicalName = "udo_virtualva";
                                thisNewEntity["udo_name"] = "Virtual VA Summary";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                if (!string.IsNullOrEmpty(doc.mcs_rcvdDt))
                                {
                                    thisNewEntity["udo_documentdate"] = doc.mcs_rcvdDt;
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(doc.mcs_rcvdDt, out newDateTime))
                                    {

                                        thisNewEntity["udo_docdate"] = newDateTime;
                                    }
                                }
                                if (!string.IsNullOrEmpty(doc.mcs_dcmntFormatCd))
                                {
                                    thisNewEntity["udo_documentformat"] = doc.mcs_dcmntFormatCd;
                                }
                                if (!string.IsNullOrEmpty(doc.mcs_fnDcmntId))
                                {
                                    thisNewEntity["udo_documentid"] = doc.mcs_fnDcmntId;
                                }
                                if (!string.IsNullOrEmpty(doc.mcs_fnDcmntSource))
                                {
                                    thisNewEntity["udo_documentsource"] = doc.mcs_fnDcmntSource;
                                }
                                if (!string.IsNullOrEmpty(doc.mcs_dcmntTypeDescpTxt))
                                {
                                    thisNewEntity["udo_documenttypedesc"] = doc.mcs_dcmntTypeDescpTxt;
                                }
                                if (!string.IsNullOrEmpty(doc.mcs_authorRoNbr.ToString()))
                                {
                                    thisNewEntity["udo_regionaloffice"] = doc.mcs_authorRoNbr.ToString();
                                }
                                if (!string.IsNullOrEmpty(doc.mcs_subjctTxt))
                                {
                                    thisNewEntity["udo_subject"] = doc.mcs_subjctTxt;
                                }
                                if (request.UDOcreateUDOVirtualVARelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOVirtualVARelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                }
                                CreateRequest createDocs = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createDocs);

                                if (docCount == 50)
                                {
                                    #region write to CRM
                                    if (requestCollection.Count > 0)
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
                                            response.ExceptionOccurred = true;
                                            return response;
                                        }
                                        string logInfo = string.Format("Number of Virtual VA Records Created: {0}", docCount);
                                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $"Virtual VA Records Created. {logInfo}", request.Debug);

                                        requestCollection.Clear();
                                        docCount = 0;
                                    }
                                    #endregion
                                }
                                #endregion

                            }

                        }
                        #region write to CRM
                        if (requestCollection.Count > 0)
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
                                response.ExceptionOccurred = true;
                                return response;
                            }

                            string logInfo = string.Format("Number of Virtual VA Records Created: {0}", totalDocCount);
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $"VirtualVA: {logInfo}", request.Debug);

                            #endregion
                        }
                    }
                }
                if (request.udo_idproofid != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_idproofid;
                    parent.LogicalName = "udo_idproof";
                    parent["udo_virtualvacomplete"] = true;
                    // OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                    OrgServiceProxy.Update(parent);
                }

                return response;
            }
            catch (Exception executionException)
            {
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Execution Progress:" + progressString, request.Debug);
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName,
                    request.MessageId, method, executionException);
                response.ExceptionMessage = "Failed to Process Virtual VA Data";
                response.ExceptionOccurred = true;
                return response;
            }
            finally
            {
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method,
                    $"<< Exited {this.GetType().FullName}.Execute ", request.Debug);
                if (OrgServiceProxy != null)
                    OrgServiceProxy.Dispose();
            }
        }

    }
}