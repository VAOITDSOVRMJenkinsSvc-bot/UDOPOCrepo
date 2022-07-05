using CuttingEdge.Conditions;
using MCSUtilities2011;
using System;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.CRM.SDK;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreateMsWordDocument : FilterBase<IAddDependentPdfState>
    {
        private const string _AddDependentFormTemplate = "686 Add Dependent Form";

        public override void Execute(IAddDependentPdfState msg)
        {
			//CSDEv REM 
			//Logger.Instance.Debug("Calling CreateMsWordDocument");
			LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
				, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreateMsWordDocument.Execute", "Calling CreateMsWordDocument");

			DateTime methodStartTime;
            string method = "CreateMsWordDocument";

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

            //msg.WordDocBytes = ReadFile.GetFileData(@"D:\\projects\\sorr\\686 Add Dependent Form.docx");

            var logger = new MCSLogger
            {
                setService = msg.AddDependentMaintenanceRequestState.OrganizationService
            };
            
            msg.WordDocBytes = DocGen.CreateDocumentFromMaster(msg.AddDependentMaintenanceRequestState.OrganizationService,
                //IOrganizationService service
                msg.AddDependentMaintenanceRequestState.DependentMaintenance, //Entity thisResource
                _AddDependentFormTemplate, //string template
                logger); //IMCSLogger logger

			LogHelper.EndTiming(methodLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTime);

        }
    }
}