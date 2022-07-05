using System;
using CuttingEdge.Conditions;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreatePdfDocument : FilterBase<IAddDependentPdfState>
    {
        public override void Execute(IAddDependentPdfState msg)
        {
			//CSDEv REm 
			//Logger.Instance.Debug("Calling CreatePdfDocument");
			LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
				, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreatePdfDocument.Execute", "Calling CreatePdfDocument");

			DateTime methodStartTime, wsStartTime;
            string method = "CreatePdfDocument", webService = "ConvertWordToPdf";

			
			Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

			Condition.Requires(msg, "msg")
			 .IsNotNull();

			Condition.Requires(msg.WordDocBytes, "msg.WordDocBytes").IsNotNull().IsNotEmpty();

			msg.PdfFileName = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), ".pdf");

			var service = BgsServiceFactory.GetPdfService(msg.AddDependentMaintenanceRequestState.OrganizationName);

			
			Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

			var response = service.ConvertWordToPdf(msg.WordDocBytes, msg.PdfFileName);

			
			LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

			Condition.Requires(response, "response").IsNotNull();

			//CSdev TODO Remove Me
			if(response.Message != null)
			{
				LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
											, msg.AddDependentMaintenanceRequestState.SystemUserId, "MMM CreatePdfDocument.Execute Message: ", response.Message.ToString());

			}
			

			Condition.Requires(response.OutputFileBytes, "response.OutputFileBytes").IsNotEmpty();

			msg.PdfFileBytes = response.OutputFileBytes;

			
			LogHelper.EndTiming(methodLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTime);
		}
    }
}