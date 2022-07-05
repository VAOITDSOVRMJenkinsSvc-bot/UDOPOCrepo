using System;
using CuttingEdge.Conditions;
//using DocumentFormat.OpenXml.Presentation;
using Microsoft.Xrm.Sdk;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Core;
using VRM.IntegrationServicebus.AddDependent.CrmModel;
//using VRM.IntegrationServicebus.AddDependent.CrmModel;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class UploadMsWordDocument : FilterBase<IAddDependentPdfState>
    {
        private const string _CWordDoc = "686C Word Document";
        private const string _MimeType = "application/doc";

        public override void Execute(IAddDependentPdfState msg)
        {
            DateTime methodStartTime;
            string method = "UploadMsWordDocument";

			
            Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

            Condition.Requires(msg.AddDependentMaintenanceRequestState, 
                "msg.AddDependentMaintenanceRequestState").IsNotNull();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.Context, 
                "msg.AddDependentMaintenanceRequestState.Context").IsNotNull();

            var addDependentConfiguration = AddDependentConfiguration.GetCurrent(msg.AddDependentMaintenanceRequestState.Context);

            if (!addDependentConfiguration.AttachWordDocToAdRecord || 
                (msg.HasOrchestrationError && !addDependentConfiguration.AttachWordDocToAdRecordError))
                return;

			//CSDev REm 
            //Logger.Instance.Debug("Calling UploadMsWordDocument");
			LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
				, msg.AddDependentMaintenanceRequestState.SystemUserId, "UploadMsWordDocument.Execute", "Calling UploadMsWordDocuments");

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

            Condition.Requires(msg.WordDocBytes,
                "msg.PdfFileBytes").IsNotNull().IsNotEmpty();

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

            //msg.AddDependentMaintenanceRequestState.Context.AddObject(annotation);

            annotation.MimeType = _MimeType;
            annotation.NoteText = _CWordDoc;
            annotation.FileName = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), ".doc");
            annotation.DocumentBody = Convert.ToBase64String(msg.WordDocBytes);

           // msg.AddDependentMaintenanceRequestState.Context.UpdateObject(annotation);

           // msg.AddDependentMaintenanceRequestState.Context.SaveChanges();

            msg.AddDependentMaintenanceRequestState.OrganizationService.Create(annotation);

			
			LogHelper.EndTiming(methodLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTime);

		}
    }
}