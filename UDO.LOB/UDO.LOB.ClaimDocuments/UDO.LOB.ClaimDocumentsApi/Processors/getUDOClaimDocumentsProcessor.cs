/// <summary>
/// GetUDOClaimDocumentsProcessor
/// </summary>
namespace UDO.LOB.ClaimDocuments.Processors
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Tooling.Connector;
    using System;
    using System.IO;
    using System.IO.Compression;
    using UDO.LOB.ClaimDocuments.Messages;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions;
    using UDO.LOB.Extensions.Logging;
    using VEIS.Messages.DocumentService;

    class GetUDOClaimDocumentsProcessor
    {
        private bool _debug { get; set; }

        private const string method = "getUDOClaimDocumentsProcessor";

        private string LogBuffer { get; set; }

        private CrmServiceClient OrgServiceProxy;

        string _progressString = "Top of Processor";

        public IMessageBase Execute(getUDOClaimDocumentsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute");
            getUDOClaimDocumentsResponse response = new getUDOClaimDocumentsResponse { MessageId = request.MessageId };
            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = "getUDOClaimDocumentsProcessor",
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            TraceLogger tLogger = new TraceLogger(method, request);

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<getUDOClaimDocumentsRequest>(request)}");
            LogBuffer = string.Empty;
            _debug = request.Debug;

            #region Handle Debug & Log Soap

            if (request.Debug)
            {
                var requestMessage = string.Format("Document Id: {0} \r\n\r\nRequest: \r\n\r\n{1}", request.documentId.ToString(),
                    JsonHelper.Serialize<getUDOClaimDocumentsRequest>(request));
                LogHelper.LogDebug(request.OrganizationName, request.UserId, request.Debug, request.RelatedParentId,
                    request.RelatedParentEntityName, request.RelatedParentFieldName,
                    GetMethod(MethodInfo.GetThisMethod().ToString(true)), requestMessage, false);
            }

            if (request.LogSoap)
            {
                // var requestMessage = "Request: \r\n\r\n" + request.SerializeToString();
                var requestMessage = "Request: \r\n\r\n" + JsonHelper.Serialize<getUDOClaimDocumentsRequest>(request); ;
                LogHelper.LogInfo(request.OrganizationName, request.LogSoap, request.UserId, GetMethod(MethodInfo.GetThisMethod().ToString(true)), requestMessage);
            }

            #endregion

            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, string.Format("{0} Processor, Connection Error", method), connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            _progressString = "After CRM Connection";

            try
            {

                var findClaimantLettersRequest = new VEISfclmlettersfindClaimantLettersRequest();
                findClaimantLettersRequest.LogTiming = request.LogTiming;
                findClaimantLettersRequest.LogSoap = request.LogSoap;
                findClaimantLettersRequest.Debug = request.Debug;
                findClaimantLettersRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findClaimantLettersRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findClaimantLettersRequest.RelatedParentId = request.RelatedParentId;
                findClaimantLettersRequest.UserId = request.UserId;
                findClaimantLettersRequest.OrganizationName = request.OrganizationName;
                findClaimantLettersRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo()
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,

                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };

                findClaimantLettersRequest.mcs_documentid = request.documentId;

                _progressString = "Before VEIS EC Call for VEISfclmlettersfindClaimantLetters...";

                var findClaimantLettersResponse = WebApiUtility.SendReceive<VEISfclmlettersfindClaimantLettersResponse>(findClaimantLettersRequest, WebApiType.VEIS);

                _progressString = "After VEIS EC Call for VEISfclmlettersfindClaimantLetters...";

                response.ExceptionMessage = findClaimantLettersResponse.ExceptionMessage;
                response.ExceptionOccured = findClaimantLettersResponse.ExceptionOccurred;
                if (findClaimantLettersResponse.VEISfclmlettersletterContainerInfo != null)
                {
                    var responseDetails = new getUDOClaimDocumentsResponseData();
                    System.Collections.Generic.List<getUDOClaimDocumentsResponseData> UDODocsArray = new System.Collections.Generic.List<getUDOClaimDocumentsResponseData>();

                    if (findClaimantLettersResponse.VEISfclmlettersletterContainerInfo.VEISfclmletterslettersInfo != null)
                    {
                        var cols = new ColumnSet("ownerid");
                        _progressString = "Before Retrieve mapdletter Snapshot";
                        var mapDletter = OrgServiceProxy.Retrieve("udo_mapdletter", request.MAPDLetterId, cols);
                        _progressString = "After Retrieve mapdletter Snapshot";

                        var letterInfo = findClaimantLettersResponse.VEISfclmlettersletterContainerInfo.VEISfclmletterslettersInfo;
                        //try
                        //{

                        Entity newAnnotation = new Entity();
                        newAnnotation["ownerid"] = mapDletter["ownerid"];
                        newAnnotation.LogicalName = "annotation";
                        newAnnotation["objecttypecode"] = "udo_mapdletter";
                        newAnnotation["objectid"] = new EntityReference("udo_mapdletter", request.MAPDLetterId);
                        newAnnotation["subject"] = "Claim Letter";
                        //newAnnotation["subject"] = "Claim Letter";
                        newAnnotation["notetext"] = "Claim Letter";
                        newAnnotation["filename"] = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), "ClaimLetter.rtf");

                        var hadText = false;
                        if (letterInfo.Length > 0)
                        {
                            //if there is nothing in the letter text, then why bother?
                            if (letterInfo[0].mcs_lttrTxt != null)
                            {
                                //byte[] compressedDoc = Convert.FromBase64String(letterInfo[0].mcs_lttrTxt);
                                //byte[] compressedDoc = letterInfo[0].mcs_lttrTxt;
                                //byte[] docText = UnZip2(compressedDoc);
                                //string docText = Encoding.UTF8.GetString(compressedDoc);
                                newAnnotation["mimetype"] = @"application/msword";

                                newAnnotation["documentbody"] = WebApiUtility.ConvertByteStringToDocBody(letterInfo[0].mcs_lttrTxt);
                                //Convert.ToBase64String(docText);
                                //newAnnotation["documentbody"] = docText;
                                hadText = true;
                            }
                        }
                        if (!hadText)
                        {
                            newAnnotation["notetext"] = "Empty Claim Letter Found";
                        }

                        _progressString = "Before Create Annotation";
                        var newID = OrgServiceProxy.Create(newAnnotation);
                        _progressString = "After Create Annotation";
                        responseDetails.udo_attachmentId = newID;
                        UDODocsArray.Add(responseDetails);

                        response.getUDOClaimDocumentsResponseInfo = UDODocsArray.ToArray();
                        //}
                        //catch (Exception ExecutionException)
                        //{
                        //    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                        //}
                    }
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId,
                    request.RelatedParentEntityName, request.RelatedParentFieldName, method, ExecutionException);
                response.ExceptionMessage = "Failed to Process Claim Document Data";
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

        public static string UnZip(byte[] byteArray)
        {
            //Transform string into byte[]
            // byte[] byteArray = new byte[value.Length];
            //int indexBA = 0;
            //foreach (char item in value.ToCharArray())
            //{
            //    byteArray[indexBA++] = (byte)item;
            //}

            //Prepare for decompress
            MemoryStream ms = new MemoryStream(byteArray);
            GZipStream sr = new GZipStream(ms, CompressionMode.Decompress);

            //Reset variable to collect uncompressed result
            //byteArray = new byte[byteArray.Length];

            //Decompress
            int rByte = sr.Read(byteArray, 0, byteArray.Length);

            //Transform byte[] unzip data to string
            System.Text.StringBuilder sB = new System.Text.StringBuilder(rByte);
            //Read the number of bytes GZipStream red and do not a for each bytes in
            //resultByteArray;
            for (int i = 0; i < rByte; i++)
            {
                sB.Append((char)byteArray[i]);
            }
            sr.Close();
            ms.Close();
            sr.Dispose();
            ms.Dispose();
            return sB.ToString();
        }

        public static byte[] UnZip2(byte[] byteArray)
        {


            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.

            using (GZipStream stream = new GZipStream(new MemoryStream(byteArray), CompressionMode.Decompress))
            {
                int size = byteArray.Length;
                byte[] buffer = new byte[size];
                if (buffer != null)
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        return memory.ToArray();
                    }
                }
                return null;

            }

        }

        /// <summary>
        /// Get Method - Will return the Method Value that contains [Method Name]
        /// </summary>
        /// <returns></returns>
        public string GetMethod(string method)
        {
            var returnMethod = string.Format("{0}_ClaimDocuments_{1}", method, _progressString);
            return returnMethod;
        }
    }
}