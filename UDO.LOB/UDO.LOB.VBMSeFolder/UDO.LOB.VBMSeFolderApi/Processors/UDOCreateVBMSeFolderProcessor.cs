using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.VBMSeFolder.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.VBMSWebService;

/// <summary>
/// UDOCreateVBMSeFolder,CreateVBMSeFolder method, Processor.
/// </summary>
namespace UDO.LOB.VBMSeFolder.Processors
{
    class UDOCreateVBMSeFolderProcessor
    {
        private CrmServiceClient OrgServiceProxy;

        private const string method = "UDOCreateVBMSeFolderProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOCreateVBMSeFolderRequest request)
        {
            UDOCreateVBMSeFolderResponse response = new UDOCreateVBMSeFolderResponse { MessageId = request.MessageId };
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
                return response;
            }
            if (request.DiagnosticsContext == null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    IdProof=request.udo_idProofId,
                     VeteranId=request.VeteranId
                };
            }

            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, "UDOCreateVBMSeFolderProcessor , Progress:" + progressString + WebApiUtility.StackTraceToString(ex: connectException));
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion

            var requestCollection = new OrganizationRequestCollection();

            progressString = "After Connection, starting on mapping";
            requestCollection.Clear();
            try
            {
                progressString = "Updating IDProof record for VBMS Load State = In Progress(752280000)";

                var updatedIdProof = new Entity("udo_idproof");
                updatedIdProof.Id = request.RelatedParentId;
                updatedIdProof["udo_vbmsloadstate"] = new OptionSetValue(752280000);

                OrgServiceProxy.Update(updatedIdProof);

                progressString = "ID Proof Updated, starting on mapping";
                var findDocumentVersionReferenceRequest = new VEISfdvrfindDocumentVersionReferenceRequest();
                findDocumentVersionReferenceRequest.MessageId = request.MessageId;
                findDocumentVersionReferenceRequest.LogTiming = request.LogTiming;
                findDocumentVersionReferenceRequest.LogSoap = request.LogSoap;
                findDocumentVersionReferenceRequest.Debug = request.Debug;
                findDocumentVersionReferenceRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findDocumentVersionReferenceRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findDocumentVersionReferenceRequest.RelatedParentId = request.RelatedParentId;
                findDocumentVersionReferenceRequest.UserId = request.UserId;
                findDocumentVersionReferenceRequest.OrganizationName = request.OrganizationName;
                findDocumentVersionReferenceRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber,
                };

                findDocumentVersionReferenceRequest.documentversionreferencesearchcriteriaInfo = new VEISfdvrdocumentversionreferencesearchcriteria
                {
                    VeteranIdentifierInfo = new VEISfdvrVeteranIdentifier
                    {
                        mcs_fileNumber = "",
                        mcs_edipi = "",
                        mcs_ssn = request.udo_ssn,
                    }
                };

                var findDocumentVersionReferenceResponse = WebApiUtility.SendReceive<VEISfdvrfindDocumentVersionReferenceResponse>(findDocumentVersionReferenceRequest, WebApiType.VEIS);
                progressString = "After VEIS EC Call (VEISfdvrfindDocumentVersionReferenceRequest) ";

                if (request.LogSoap || findDocumentVersionReferenceResponse.ExceptionOccurred)
                {
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"SOAP Request: {findDocumentVersionReferenceResponse.SerializedSOAPRequest} SOAP Response: {findDocumentVersionReferenceResponse.SerializedSOAPResponse}", true);
                }

                if (findDocumentVersionReferenceResponse.ExceptionOccurred)
                {
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, $"Exception Message: {findDocumentVersionReferenceResponse.ExceptionMessage} EC Trace : {findDocumentVersionReferenceResponse.EcTraceLog}");
                }

                response.ExceptionMessage = findDocumentVersionReferenceResponse.ExceptionMessage;
                response.ExceptionOccurred = findDocumentVersionReferenceResponse.ExceptionOccurred;

                if (findDocumentVersionReferenceResponse.VEISfdvrsourceInfo != null)
                {
                    var DocumentVersionReference = findDocumentVersionReferenceResponse.VEISfdvrsourceInfo;
                    foreach (var DocumentVersionReferenceItem in DocumentVersionReference)
                    {
                        var responseIds = new UDOCreateVBMSeFolderMultipleResponse();
                        
                        // Instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_vbmsefolder";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (!string.IsNullOrEmpty(DocumentVersionReferenceItem.mcs_subject))
                        {
                            thisNewEntity["udo_subject"] = DocumentVersionReferenceItem.mcs_subject;
                        }
                        if (!string.IsNullOrEmpty(DocumentVersionReferenceItem.mcs_mimeType))
                        {
                            thisNewEntity["udo_mimetype"] = DocumentVersionReferenceItem.mcs_mimeType;
                        }

                        if (!string.IsNullOrEmpty(DocumentVersionReferenceItem.mcs_vaReceiveDate) && DateTime.TryParse(DocumentVersionReferenceItem.mcs_vaReceiveDate, out DateTime result))
                        {
                            thisNewEntity["udo_receiptdate"] = result;
                        }

                        if (!string.IsNullOrEmpty(DocumentVersionReferenceItem.mcs_documentVersionRefId))
                        {
                            thisNewEntity["udo_documentversionrefid"] = DocumentVersionReferenceItem.mcs_documentVersionRefId;
                        }
                        if (DocumentVersionReferenceItem.VEISfdvrtypeCategoryInfo != null)
                        {
                            if (!string.IsNullOrEmpty(DocumentVersionReferenceItem.VEISfdvrtypeCategoryInfo.mcs_typeDescriptionText))
                            {
                                thisNewEntity["udo_documenttype"] = DocumentVersionReferenceItem.VEISfdvrtypeCategoryInfo.mcs_typeDescriptionText;
                                thisNewEntity["udo_name"] = DocumentVersionReferenceItem.VEISfdvrtypeCategoryInfo.mcs_typeDescriptionText;
                            }
                        }

                        if (request.UDOCreateVBMSeFolderRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOCreateVBMSeFolderRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createData);
                    }
                }

                #region Create records

                if (requestCollection.Count() > 0)
                {
                    var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);

                    if (request.Debug)
                    {
                        LogBuffer += result.LogDetail;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, LogBuffer, request.Debug);
                    }

                    if (result.IsFaulted)
                    {
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, result.ErrorDetail);
                        response.ExceptionMessage = result.FriendlyDetail;
                        response.ExceptionOccurred = true;
                        return response;
                    }
                }

                string logInfo = string.Format("eFolder Records Created: {0}", requestCollection.Count());
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, logInfo, request.Debug);
                #endregion

                progressString = "Updating ID Proof VBMS Load State = Complete  (752280001)";
                updatedIdProof["udo_vbmsloadstate"] = new OptionSetValue(752280001);
                OrgServiceProxy.Update(updatedIdProof);
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $"<<< Exiting {method} {Environment.NewLine} Progress: {progressString}", true);

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $"### Exiting {method} {Environment.NewLine} Progress: {progressString}", true);

                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOCreateVBMSeFolderProcessort Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process eFolder Data";
                response.ExceptionOccurred = true;
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
    }
}
