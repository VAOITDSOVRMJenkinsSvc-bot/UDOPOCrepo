using CuttingEdge.Conditions;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Bgs.Services.DocOperationsReference;
using System;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Bgs.Services.DocOperationsReference;
//using DocumentFormat.OpenXml.Presentation;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class LoadPdfToVva : FilterBase<IAddDependentPdfState>
    {
        private const string _InsertedByRo = "RO";
        private const string _ApplicationPdf = "application/pdf";
        private const int _DocumentCategory = 62;
        private const int _DocumentType = 148;

        public override void Execute(IAddDependentPdfState msg)
        {
            Condition.Requires(msg.AddDependentMaintenanceRequestState, 
                "msg.AddDependentMaintenanceRequestState").IsNotNull();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.Context, 
                "msg.AddDependentMaintenanceRequestState.Context").IsNotNull();

            var addDependentConfiguration = AddDependentConfiguration.GetCurrent(msg.AddDependentMaintenanceRequestState.Context);

            if (!addDependentConfiguration.LoadPdfToVva || 
                (msg.HasOrchestrationError && !addDependentConfiguration.LoadPdfToVvaError)) 
                return;

			//CSdev REm 
            //Logger.Instance.Debug("Calling LoadPdfToVva");
			LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
				, msg.AddDependentMaintenanceRequestState.SystemUserId, "UploadPdfDocument.Execute", "Calling UploadPdfDocument");


			DateTime methodStartTime, wsStartTime;
            string method = "LoadPdfToVva", webService = "AddDocument";

			
			Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

			Condition.Requires(msg, "msg")
			   .IsNotNull();

			Condition.Requires(msg.PdfFileBytes, "msg.PdfFileBytes").IsNotNull().IsNotEmpty();
			Condition.Requires(msg.PdfFileName, "msg.PdfFileName").IsNotNull().IsNotEmpty();

			var applicationName = addDependentConfiguration.VvaCallerApplicationName;

			var vaDocument = new vaDocument
			{
				claimNumber = msg.VeteranRequestState.VeteranParticipant.Ssn,
				content = msg.PdfFileBytes,
				documentCategory = _DocumentCategory,
				documentSubject = applicationName,
				fileName = msg.PdfFileName,
				insertedByRO = _InsertedByRo,
				insertedByUserId = msg.AddDependentMaintenanceRequestState.BgsHeaderInfo.LoginName,
				mimeType = _ApplicationPdf,
				source = applicationName,
				sourceComment = applicationName,
				documentType = _DocumentType
			};

			var request = new AddDocument(vaDocument);

			var service = BgsServiceFactory.GetDocOperationsService(msg.AddDependentMaintenanceRequestState.OrganizationName);

			
			Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

			var response = service.AddDocument(request);

			
			LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

			Condition.Requires(response, "response").IsNotNull();

			Condition.Requires(response.AddDocumentResponse1, "response.AddDocumentResponse1").IsGreaterThan(0);

			
			LogHelper.EndTiming(methodLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTime);

		}
    }
}