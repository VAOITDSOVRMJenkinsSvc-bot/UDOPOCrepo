using System;
using CuttingEdge.Conditions;
using Microsoft.Xrm.Sdk;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Core;
using VRM.IntegrationServicebus.AddDependent.CrmModel;
//using VRM.IntegrationServicebus.AddDependent.CrmModel;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class UploadPdfDocument : FilterBase<IAddDependentPdfState>
    {
        private const string _CPdfDocument = "686C PDF Document";
        private const string _CPdfErrDocument = "686C PDF (Error) Document";
        private const string _MimeType = "application/pdf";

        public override void Execute(IAddDependentPdfState msg)
        {
            DateTime methodStartTime;
            string method = "UploadPdfDocument";

			
			Guid methodLoggingId  = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName,  msg.AddDependentMaintenanceRequestState.SystemUserId,
                msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

            Condition.Requires(msg.AddDependentMaintenanceRequestState,
                "msg.AddDependentMaintenanceRequestState").IsNotNull();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.Context,
                "msg.AddDependentMaintenanceRequestState.Context").IsNotNull();

            var addDependentConfiguration = AddDependentConfiguration.GetCurrent(msg.AddDependentMaintenanceRequestState.Context);

            if (!addDependentConfiguration.AttachPdfToAdRecord || 
                (msg.HasOrchestrationError && !addDependentConfiguration.AttachPdfToAdRecordError))
                return;

			//CSDev Rem 
            //Logger.Instance.Debug("Calling UploadPdfDocument");
			LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
				, msg.AddDependentMaintenanceRequestState.SystemUserId, "UploadPdfDocument.Execute", "Calling UploadPdfDocument");

			Condition.Requires(msg, "msg")
               .IsNotNull();

            Condition.Requires(msg.AddDependentMaintenanceRequestState, 
                "msg.AddDependentMaintenanceRequestState").IsNotNull();

            Condition.Requires(msg.AddDependentMaintenanceRequestState.DependentMaintenance,
                "msg.AddDependentMaintenanceRequestState.DependentMaintenance").IsNotNull();

            Condition.Requires(msg.AddDependentMaintenanceRequestState.Context,
                "msg.AddDependentMaintenanceRequestState.Context").IsNotNull();

            Condition.Requires(msg.AddDependentMaintenanceRequestState.SystemUser,
                "msg.AddDependentMaintenanceRequestState.SystemUser").IsNotNull();

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

            var annotation = new Annotation
            {
                IsDocument = true, 
                Subject = documentLabel,
                ObjectId = new EntityReference(crme_dependentmaintenance.EntityLogicalName, 
                    msg.AddDependentMaintenanceRequestState.DependentMaintenance.Id),
                ObjectTypeCode = crme_dependentmaintenance.EntityLogicalName.ToLower(),
                OwnerId = new EntityReference(SystemUser.EntityLogicalName, 
                    msg.AddDependentMaintenanceRequestState.SystemUser.Id)
            };

          //  msg.AddDependentMaintenanceRequestState.Context.AddObject(annotation);

            annotation.MimeType = _MimeType;
            annotation.NoteText = documentLabel;
            annotation.FileName = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), ".pdf");
            annotation.DocumentBody = Convert.ToBase64String(msg.PdfFileBytes);

        //    msg.AddDependentMaintenanceRequestState.Context.UpdateObject(annotation);

          //  msg.AddDependentMaintenanceRequestState.Context.SaveChanges();

            msg.AddDependentMaintenanceRequestState.OrganizationService.Create(annotation);

			
			LogHelper.EndTiming(methodLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTime);

		}
    }
}