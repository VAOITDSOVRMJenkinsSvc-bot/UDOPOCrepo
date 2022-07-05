/// <summary>
/// LOB Component for UDOgetVBMSDocumentContent,getVBMSDocumentContent method, Processor.
/// </summary>
namespace UDO.LOB.VBMSeFolder.Processors
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Tooling.Connector;
    using System;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions;
    using UDO.LOB.Extensions.Logging;
    using UDO.LOB.VBMSeFolder.Messages;
    using VEIS.Core.Messages;
    using VEIS.Messages.VBMSWebService;

    public class getVBMSDocumentContentProcessor 
	{
        private CrmServiceClient OrgServiceProxy;
        private string LogBuffer { get; set; }
        private const string method = "getVBMSDocumentContentProcessor";
        
        public IMessageBase Execute(UDOgetVBMSDocumentContentRequest request)
		{
			UDOgetVBMSDocumentContentResponse response = new UDOgetVBMSDocumentContentResponse { MessageId = (request == null)? Guid.NewGuid().ToString(): request.MessageId };
			var progressString = "Top of Processor";
            
            if (request == null)
			{
				response.ExceptionMessage = "Called with no message";
				response.ExceptionOccurred = true;
				return response;
			}
            // request.Debug = true; // todo remove this after testing
            //_debug = request.Debug;
            if (request.DiagnosticsContext == null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    OrganizationName = request.OrganizationName,
                };
            }
            TraceLogger aiLogger = new TraceLogger(method, request);
            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                // Build the VEISVBMSgetDocgetDocumentContentRequest
                var getDocumentContent = new VEISVBMSgetDocgetDocumentContentRequest();
                getDocumentContent.LogTiming = request.LogTiming;
                getDocumentContent.LogSoap = request.LogSoap;
                getDocumentContent.Debug = request.Debug;
                getDocumentContent.RelatedParentEntityName = request.RelatedParentEntityName;
                getDocumentContent.RelatedParentFieldName = request.RelatedParentFieldName;
                getDocumentContent.RelatedParentId = request.RelatedParentId;
                getDocumentContent.UserId = request.UserId;
                getDocumentContent.OrganizationName = request.OrganizationName;
                getDocumentContent.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber,
                };

                getDocumentContent.mcs_documentversionrefid = request.udo_DocumentVersionRefId;

                var getDocumentContentResponse = WebApiUtility.SendReceive<VEISVBMSgetDocgetDocumentContentResponse>(getDocumentContent, WebApiType.VEIS);
                progressString = "After VEIS EC Call (VEISVBMSgetDocgetDocumentContentRequest)...";

                if (request.LogSoap || getDocumentContentResponse.ExceptionOccurred)
                {
                    if (getDocumentContentResponse.SerializedSOAPRequest != null || getDocumentContentResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = getDocumentContentResponse.SerializedSOAPRequest + getDocumentContentResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISVBMSgetDocgetDocumentContentRequest Request/Response {requestResponse}", true);
                    }
                }

                response.ExceptionMessage = getDocumentContentResponse.ExceptionMessage;
                response.ExceptionOccurred = getDocumentContentResponse.ExceptionOccurred;
                if (getDocumentContentResponse.VEISVBMSgetDocDocumentContentInfo != null)
                {
                    var cols = new ColumnSet("ownerid", "udo_mimetype");
                    var vbmseFolder = OrgServiceProxy.Retrieve("udo_vbmsefolder", request.udo_VBMSeFolderId, cols);
                    progressString = $" > After Retrieve udo_vbmsefolder. udo_VBMSeFolderId: {request.udo_VBMSeFolderId.ToString()} ";

                    var letterInfo = getDocumentContentResponse.VEISVBMSgetDocDocumentContentInfo;

                    Entity newAnnotation = new Entity();
                    newAnnotation.LogicalName = "annotation";
                    newAnnotation["ownerid"] = vbmseFolder["ownerid"];
                    newAnnotation["objecttypecode"] = "udo_vbmsefolder";
                    newAnnotation["objectid"] = new EntityReference("udo_vbmsefolder", request.udo_VBMSeFolderId);
                    newAnnotation["subject"] = "eFolder Document";
                    // newAnnotation["subject"] = "eFolder Document";
                    if (vbmseFolder.Contains("udo_mimetype"))
                    {
                        newAnnotation["mimetype"] = vbmseFolder["udo_mimetype"];
                    }

                    newAnnotation["notetext"] = "eFolder Document";
                    newAnnotation["filename"] = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), "eFolderDocument.pdf");
                    newAnnotation["documentbody"] = letterInfo.mcs_bytes;

                    var newID = OrgServiceProxy.Create(newAnnotation);
                        
                    response.udo_attachmentId = newID;
                    progressString = $" > Created new Note: Id: {newID}: udo_vbmsefolder: {request.udo_VBMSeFolderId}";
                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $"Error in {method} Processor, Progress: {progressString}", request.Debug);
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, ExecutionException);
                response.ExceptionMessage = "Failed to Process Document Attachment to the Note on VBMS eFolder Record";
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