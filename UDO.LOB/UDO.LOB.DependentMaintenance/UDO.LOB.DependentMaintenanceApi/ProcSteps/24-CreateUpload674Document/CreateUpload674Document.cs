using CuttingEdge.Conditions;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
using VRM.IntegrationServicebus.AddDependent.CrmModel;
//using VRM.Integration.Servicebus.CRM.SDK;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreateUpload674Document : FilterBase<IAddDependentPdfState>
    {
        private const string _AddDependentFormTemplate = "674 School Aged Child";
        private string _CWordDoc = "674 Word Document";
        private const string _MimeType = "application/doc";

        private string _CPdfDocument = "674 PDF Document";
        private string _CPdfErrDocument = "674 PDF (Error) Document";
        private const string _MimeTypePdf = "application/pdf";

        IOrganizationService OrgService;

        public override void Execute(IAddDependentPdfState msg)
        {
            //CSDEv REM 
            //Logger.Instance.Debug("Calling CreateMsWordDocument");
            LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
                , msg.AddDependentMaintenanceRequestState.SystemUserId, "CreateUpload674Document.Execute", "Calling CreateUpload674Document");

            DateTime methodStartTime;
            string method = "CreateAndUploadMsWordDocument674";

            Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);


            Condition.Requires(msg, "msg")
               .IsNotNull();

            Condition.Requires(msg.AddDependentMaintenanceRequestState,
                "msg.AddDependentMaintenanceRequestState").IsNotNull();

            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService,
                "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

            Condition.Requires(msg.AddDependentMaintenanceRequestState.DependentMaintenance,
                "msg.AddDependentMaintenanceRequestState.DependentMaintenance").IsNotNull();

            var logger = new MCSLogger
            {
                setService = msg.AddDependentMaintenanceRequestState.OrganizationService
            };

            try
            {
                foreach (var dep in msg.AddDependentMaintenanceRequestState.AddDependentRequest.Dependents)
                {
                    if ((dep.MaintenanceType == "Add" && dep.IsScholdChild == true) || (dep.MaintenanceType == "Edit" && dep.IsScholdChild == true))
                    {
                        msg.WordDocBytes = null;
                        msg.PdfFileBytes = null;
                        _CWordDoc = "674 Word Document_" + dep.FirstName + "_" + dep.LastName;
                        _CPdfDocument = "674 PDF Document_" + dep.FirstName + "_" + dep.LastName;
                        _CPdfErrDocument = "674 PDF (Error) Document_" + dep.FirstName + "_" + dep.LastName;

                        #region create word doc
                        msg.WordDocBytes = DocGen.CreateDocument674FromMaster(msg.AddDependentMaintenanceRequestState.OrganizationService,
                   //IOrganizationService service
                   dep.DepID,
                   _AddDependentFormTemplate, //string template
                   logger);


                        //  LogHelper.EndTiming(methodLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTime);

                        Condition.Requires(msg.AddDependentMaintenanceRequestState.Context,
                    "msg.AddDependentMaintenanceRequestState.Context").IsNotNull();

                        var addDependentConfiguration = AddDependentConfiguration.GetCurrent(msg.AddDependentMaintenanceRequestState.Context);

                        if (!addDependentConfiguration.AttachWordDocToAdRecord ||
                            (msg.HasOrchestrationError && !addDependentConfiguration.AttachWordDocToAdRecordError))
                            return;

                        Condition.Requires(msg.AddDependentMaintenanceRequestState.SystemUser,
                   "msg.AddDependentMaintenanceRequestState.SystemUser").IsNotNull();

                        Condition.Requires(msg.WordDocBytes,
                            "msg.WordDocBytes").IsNotNull().IsNotEmpty();

                        var annotation = new Annotation
                        {
                            IsDocument = true,
                            Subject = _CWordDoc,
                            ObjectId = new EntityReference(crme_dependentmaintenance.EntityLogicalName,
                       msg.AddDependentMaintenanceRequestState.DependentMaintenance.Id),
                            ObjectTypeCode = crme_dependentmaintenance.EntityLogicalName.ToLower(),
                            OwnerId = new EntityReference(SystemUser.EntityLogicalName,
                       msg.AddDependentMaintenanceRequestState.SystemUser.Id)
                        };
                        annotation.MimeType = _MimeType;
                        annotation.NoteText = _CWordDoc;
                        annotation.FileName = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), ".doc");
                        annotation.DocumentBody = Convert.ToBase64String(msg.WordDocBytes);

                        // msg.AddDependentMaintenanceRequestState.Context.UpdateObject(annotation);

                        // msg.AddDependentMaintenanceRequestState.Context.SaveChanges();

                        msg.AddDependentMaintenanceRequestState.OrganizationService.Create(annotation);

                        LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
                    , msg.AddDependentMaintenanceRequestState.SystemUserId, "CreatePdfDocument", "CreatingPdfDocument");

                        #endregion

                        #region create PDF
                        DateTime methodStartTime1, wsStartTime;
                        string method1 = "CreatePdfDocument", webService = "ConvertWordToPdf";


                        Guid methodLoggingId1 = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                            msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method1, null, out methodStartTime1);


                        msg.PdfFileName = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), ".pdf");

                        var service = BgsServiceFactory.GetPdfService(msg.AddDependentMaintenanceRequestState.OrganizationName);


                        Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                            msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method1, webService, out wsStartTime);

                        var response = service.ConvertWordToPdf(msg.WordDocBytes, msg.PdfFileName);


                        LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

                        Condition.Requires(response, "response").IsNotNull();

                        //CSdev TODO Remove Me
                        if (response.Message != null)
                        {
                            LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
                                                        , msg.AddDependentMaintenanceRequestState.SystemUserId, "MMM CreatePdfDocument.Execute Message: ", response.Message.ToString());

                        }


                        Condition.Requires(response.OutputFileBytes, "response.OutputFileBytes").IsNotEmpty();

                        msg.PdfFileBytes = response.OutputFileBytes;
                        LogHelper.EndTiming(methodLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTime1);

                        DateTime methodStartTimePDF;
                        string methodPDF = "UploadMsPdfDocument";

                        Guid methodLoggingIdPDF = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                            msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", methodPDF, null, out methodStartTimePDF);

                        if (!addDependentConfiguration.AttachPdfToAdRecord || (msg.HasOrchestrationError && !addDependentConfiguration.AttachPdfToAdRecordError))
                            return;


                        Condition.Requires(msg.PdfFileBytes,
                            "msg.PdfFileBytes").IsNotNull().IsNotEmpty();

                        string documentLabel = null;
                        if (msg.HasOrchestrationError)
                        {
                            documentLabel = _CPdfErrDocument;
                        }
                        else
                        {
                            documentLabel = _CPdfDocument;
                        }

                        var annotationPdf = new Annotation
                        {
                            IsDocument = true,
                            Subject = documentLabel,
                            ObjectId = new EntityReference(crme_dependentmaintenance.EntityLogicalName,
                        msg.AddDependentMaintenanceRequestState.DependentMaintenance.Id),
                            ObjectTypeCode = crme_dependentmaintenance.EntityLogicalName.ToLower(),
                            OwnerId = new EntityReference(SystemUser.EntityLogicalName,
                        msg.AddDependentMaintenanceRequestState.SystemUser.Id)
                        };

                        annotationPdf.MimeType = _MimeTypePdf;
                        annotationPdf.NoteText = documentLabel;
                        annotationPdf.FileName = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), ".pdf");
                        annotationPdf.DocumentBody = Convert.ToBase64String(msg.PdfFileBytes);

                        //    msg.AddDependentMaintenanceRequestState.Context.UpdateObject(annotation);

                        //  msg.AddDependentMaintenanceRequestState.Context.SaveChanges();

                        msg.AddDependentMaintenanceRequestState.OrganizationService.Create(annotationPdf);

                        LogHelper.EndTiming(methodLoggingIdPDF, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTimePDF);
                        #endregion

                        #region LoadPDFtoVBMS
                        LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
                        , msg.AddDependentMaintenanceRequestState.SystemUserId, "LoadPdfToVbms", "LoadPdfToVbms - Init LoadPdfToVbms");

                        if (!addDependentConfiguration.LoadPdfToVbms || msg.HasOrchestrationError)
                        {
                            LogHelper.LogInfo(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, "LoadPdfToVBMS Execute", "Configuration not set or Orchestration Error encountered. Skipping VBMS Upload.");
                            // return;
                        }
                        else
                        {
                            LogHelper.LogInfo(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, "After condition check", "After the condition check");
                            Condition.Requires(msg.PdfFileBytes, "msg.PdfFileBytes").IsNotNull().IsNotEmpty();
                            Condition.Requires(msg.PdfFileName, "msg.PdfFileName").IsNotNull().IsNotEmpty();

                            OrgService = ConnectToCrmHelper.ConnectToCrm(msg.AddDependentMaintenanceRequestState.OrganizationName);

                            var doctypeFetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                           @"<entity name='udo_vbmsdoctype'>" +
                                           @"<filter type='and'>" +
                                           @"<condition attribute='udo_name' operator='eq' value='VA 21-674' />" +
                                           @"</filter>" +
                                           @"</entity>" +
                                           @"</fetch>";

                            var docTypes = OrgService.RetrieveMultiple(new FetchExpression(doctypeFetch));
                            var docType = new Entity();

                            LogHelper.LogInfo("Value of count is: " + docTypes.Entities.Count);

                            if (docTypes.Entities.Count > 0)
                            {
                                docType = docTypes.Entities[0];
                                var userUploadRole = new OptionSetValue();
                                var uploadRoleFetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                                   @"<entity name='systemuser'>" +
                                                   @"<attribute name='udo_vbmsuploadrole' />" +
                                                   @"<filter>" +
                                                   @"<condition attribute='systemuserid' operator='eq' value='" + msg.AddDependentMaintenanceRequestState.SystemUserId + "' />" +
                                                   @"</filter>" +
                                                   @"</entity>" +
                                                   @"</fetch>";


                                var users = OrgService.RetrieveMultiple(new FetchExpression(uploadRoleFetch));
                                if (users.Entities.Count > 0)
                                {
                                    userUploadRole = users.Entities[0].GetAttributeValue<OptionSetValue>("udo_vbmsuploadrole");
                                }
                                if (userUploadRole == null)
                                {
                                    //Logger.Instance.Debug("User Role Not Found");
                                    LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
                                        , msg.AddDependentMaintenanceRequestState.SystemUserId, "UploadPdfToVBMS", "User Upload Role not set. Defaulting to PCR - NCC");
                                    userUploadRole = new OptionSetValue(752280000);
                                }


                                Entity vbmsDoc = new Entity("udo_vbmsdocument");
                                vbmsDoc.Attributes["udo_vbmsuploaddate"] = DateTime.Now;
                                vbmsDoc.Attributes["udo_vbmsfilename"] = "Dependent Maintenance";
                                vbmsDoc.Attributes["udo_vbmsdocumenttype"] = new EntityReference(docType.LogicalName, docType.Id);
                                vbmsDoc.Attributes["udo_vbmsuploadrole"] = userUploadRole;
                                vbmsDoc.Attributes["udo_firstname"] = msg.VeteranRequestState.VeteranParticipant.FirstName;
                                vbmsDoc.Attributes["udo_lastname"] = msg.VeteranRequestState.VeteranParticipant.LastName;
                                vbmsDoc.Attributes["udo_middlename"] = msg.VeteranRequestState.VeteranParticipant.MiddleName;
                                vbmsDoc.Attributes["udo_filenumber"] = msg.VeteranRequestState.VeteranParticipant.FileNumber;

                                var createdDoc = OrgService.Create(vbmsDoc);

                                var annotationVBMS = new Annotation
                                {
                                    Subject = "674 Dependent Maintenance File Attachment",
                                    ObjectTypeCode = vbmsDoc.LogicalName,
                                    MimeType = _MimeTypePdf,
                                    NoteText = _CPdfDocument,
                                    FileName = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), ".pdf"),
                                    DocumentBody = Convert.ToBase64String(msg.PdfFileBytes),
                                    ObjectId = new EntityReference
                                    {
                                        Id = createdDoc,
                                        LogicalName = vbmsDoc.LogicalName
                                    }
                                };
                                OrgService.Create(annotationVBMS);
                            }
                            else
                            {
                                //Logger.Instance.Debug("No DocTypes Found");
                                LogHelper.LogError(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.SystemUserId, "LoadPdfToVBMS, Execute", "Failed to identify correct VBMS Doc Type for Dependent Maintenance (type 148)");
                               // return;
                            }
                            
                        }
                        #endregion

                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            LogHelper.EndTiming(methodLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTime);

            //end rajul test

        }
    }
}