using System;
using System.Text;
using CuttingEdge.Conditions;
using Microsoft.Xrm.Sdk;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Core;
using VRM.IntegrationServicebus.AddDependent.CrmModel;
//using Annotation = CRM007.CRM.SDK.Core.Annotation;
//using SystemUser = CRM007.CRM.SDK.Core.SystemUser;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class UploadSoapLog : FilterBase<IAddDependentRequestState> 
    {
        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDEv rem 
			//Logger.Instance.Debug("Calling UploadSoapLog");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId
			//	, $"{ this.GetType().FullName}", $"| FFF Start {this.GetType().FullName}.Execute()");

			Condition.Requires(msg.AddDependentMaintenanceRequestState, 
                "msg.AddDependentMaintenanceRequestState").IsNotNull();

            Condition.Requires(msg.AddDependentMaintenanceRequestState.Context,
                "msg.AddDependentMaintenanceRequestState.Context").IsNotNull();

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
            
            var annotation = new Annotation
            {
                IsDocument = true,
                Subject = "Orchestration Soap Log",
                ObjectId = new EntityReference(crme_dependentmaintenance.EntityLogicalName,
                    msg.AddDependentMaintenanceRequestState.DependentMaintenance.Id),
                ObjectTypeCode = crme_dependentmaintenance.EntityLogicalName.ToLower(),
                OwnerId = new EntityReference(SystemUser.EntityLogicalName,
                    msg.AddDependentMaintenanceRequestState.SystemUser.Id)
            };

           // msg.AddDependentMaintenanceRequestState.Context.AddObject(annotation);

            annotation.MimeType = @"text/xml";
            annotation.NoteText = "Orchestration Soap Log";
            annotation.FileName = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), ".xml");

            var bytes = Encoding.UTF8.GetBytes(VEIS.Core.Wcf.SoapLog.Current.Log);

            annotation.DocumentBody = Convert.ToBase64String(bytes);

            //msg.AddDependentMaintenanceRequestState.Context.UpdateObject(annotation);

            //msg.AddDependentMaintenanceRequestState.Context.SaveChanges();
            msg.AddDependentMaintenanceRequestState.OrganizationService.Create(annotation);

            LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
				, msg.AddDependentMaintenanceRequestState.SystemUserId
				, $"{ this.GetType().FullName}", $"| FFF END {this.GetType().FullName}.Execute()");
		}
    }
}