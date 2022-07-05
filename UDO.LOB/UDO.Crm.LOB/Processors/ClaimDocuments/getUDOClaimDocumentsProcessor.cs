using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk;
using System.Text;
using System.ServiceModel;
using System.IO;
using System.IO.Compression;
using System;
using System.Collections.Generic;
using VIMT.DocumentWebService.Messages;
using VRM.Integration.UDO.ClaimDocuments.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;

namespace VRM.Integration.UDO.ClaimDocuments.Processors
{
    class getUDOClaimDocumentsProcessor
    {
        string _progressString = "Top of Processor";
        private bool _debug { get; set; }
        private const string method = "getUDOClaimDocumentsProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(getUDOClaimDocumentsRequest request)
        {

            getUDOClaimDocumentsResponse response = new getUDOClaimDocumentsResponse();            
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            #region Handle Debug & Log Soap

            if (request.Debug)
            {
                
                var requestMessage = string.Format("Document Id: {0} \r\n\r\nRequest: \r\n\r\n{1}", request.documentId.ToString(), request.SerializeToString());
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, GetMethod(MethodInfo.GetThisMethod().ToString(true)), requestMessage);
            }

            if (request.LogSoap)
            {
                var requestMessage = "Request: \r\n\r\n" + request.SerializeToString();
                LogHelper.LogInfo(request.OrganizationName, request.LogSoap, request.UserId, GetMethod(MethodInfo.GetThisMethod().ToString(true)), requestMessage);
            }

            #endregion

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

            _progressString = "After Connection";

            try
            {

                var findClaimantLettersRequest = new VIMTfclmlettersfindClaimantLettersRequest();
                findClaimantLettersRequest.LogTiming = request.LogTiming;
                findClaimantLettersRequest.LogSoap = request.LogSoap;
                findClaimantLettersRequest.Debug = request.Debug;
                findClaimantLettersRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findClaimantLettersRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findClaimantLettersRequest.RelatedParentId = request.RelatedParentId;
                findClaimantLettersRequest.UserId = request.UserId;
                findClaimantLettersRequest.OrganizationName = request.OrganizationName;
                findClaimantLettersRequest.LegacyServiceHeaderInfo = new VIMT.DocumentWebService.Messages.HeaderInfo()
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,

                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };

                findClaimantLettersRequest.mcs_documentid = request.documentId;

                _progressString = "Before VIMT EC Call for VIMTfclmlettersfindClaimantLetters...";
                // TODO(TN): Comment to Remediate
                var findClaimantLettersResponse = new VIMTfclmlettersfindClaimantLettersResponse();
                // var findClaimantLettersResponse = findClaimantLettersRequest.SendReceive<VIMTfclmlettersfindClaimantLettersResponse>(MessageProcessType.Local);
                _progressString = "After VIMT EC Call for VIMTfclmlettersfindClaimantLetters...";

                response.ExceptionMessage = findClaimantLettersResponse.ExceptionMessage;
                response.ExceptionOccured = findClaimantLettersResponse.ExceptionOccured;
                if (findClaimantLettersResponse.VIMTfclmlettersletterContainerInfo != null)
                {
                    var responseDetails = new getUDOClaimDocumentsResponseData();
                    System.Collections.Generic.List<getUDOClaimDocumentsResponseData> UDODocsArray = new System.Collections.Generic.List<getUDOClaimDocumentsResponseData>();

                    if (findClaimantLettersResponse.VIMTfclmlettersletterContainerInfo.VIMTfclmletterslettersInfo != null)
                    {
                        var cols = new ColumnSet("ownerid");
                        _progressString = "Before Retrieve mapdletter Snapshot";
                        var mapDletter = OrgServiceProxy.Retrieve("udo_mapdletter", request.MAPDLetterId, cols);
                        _progressString = "After Retrieve mapdletter Snapshot";

                        var letterInfo = findClaimantLettersResponse.VIMTfclmlettersletterContainerInfo.VIMTfclmletterslettersInfo;
                        //try
                        //{

                        Entity newAnnotation = new Entity();
                        newAnnotation["ownerid"] = mapDletter["ownerid"];
                        newAnnotation.LogicalName = "annotation";
                        newAnnotation["objecttypecode"] = "udo_mapdletter";
                        newAnnotation["objectid"] = new EntityReference("udo_mapdletter", request.MAPDLetterId);
                        newAnnotation["subject"] = "Claim Letter";
                        newAnnotation["subject"] = "Claim Letter";
                        newAnnotation["notetext"] = "Claim Letter";
                        newAnnotation["filename"] = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), "ClaimLetter.rtf");

                        var hadText = false;
                        if (letterInfo.Length > 0)
                        {
                            //if there is nothing in the letter text, then why bother?
                            if (letterInfo[0].mcs_lttrTxt != null)
                            {
                                byte[] docText = UnZip2(letterInfo[0].mcs_lttrTxt);
                                newAnnotation["mimetype"] = @"application/msword";

                                newAnnotation["documentbody"] = Convert.ToBase64String(docText);
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
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + _progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Claim Document Data";
                response.ExceptionOccured = true;
                return response;
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
            System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArray);
            System.IO.Compression.GZipStream sr = new System.IO.Compression.GZipStream(ms,
                System.IO.Compression.CompressionMode.Decompress);

            //Reset variable to collect uncompressed result
            byteArray = new byte[byteArray.Length];

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
                const int size = 4096;
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