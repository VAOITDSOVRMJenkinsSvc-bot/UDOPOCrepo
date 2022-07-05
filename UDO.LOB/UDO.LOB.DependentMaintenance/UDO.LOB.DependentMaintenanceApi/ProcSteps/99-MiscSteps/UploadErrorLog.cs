using System;
using System.Linq;
using System.Text;
using System.Threading;
using CuttingEdge.Conditions;
using Microsoft.Xrm.Sdk;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Core;
using VRM.IntegrationServicebus.AddDependent.CrmModel;
//using Annotation = CRM007.CRM.SDK.Core.Annotation;
//using SystemUser = CRM007.CRM.SDK.Core.SystemUser;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class UploadErrorLog : FilterBase<IAddDependentMaintenanceRequestState>
    {
        public void Execute(IAddDependentMaintenanceRequestState msg, string noteText)
        {
			//CSDev rem 
			//Logger.Instance.Debug("Calling UploadErrorLog");
			LogHelper.LogDebug(msg.OrganizationName, msg.Debug, msg.SystemUserId, $"{ this.GetType().FullName}", $"| FFF Start {this.GetType().FullName}.Execute()");

			Condition.Requires(msg, "msg").IsNotNull();

            Condition.Requires(msg.Context, "msg.Context").IsNotNull();

            Condition.Requires(msg.Exception, "msg.Exception").IsNotNull();

            var annotation = new Annotation
            {
                IsDocument = true,
                Subject = noteText,
                ObjectId = new EntityReference(crme_dependentmaintenance.EntityLogicalName,
                    msg.DependentMaintenanceId),
                ObjectTypeCode = crme_dependentmaintenance.EntityLogicalName.ToLower(),
                OwnerId = new EntityReference(SystemUser.EntityLogicalName,
                    msg.SystemUserId)
            };

           // msg.Context.AddObject(annotation);

            annotation.MimeType = @"text/plain";
            annotation.NoteText = noteText;
            annotation.FileName = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), ".txt");

            var bytes = Encoding.UTF8.GetBytes(msg.Exception.ToString());

            annotation.DocumentBody = Convert.ToBase64String(bytes);

            //Update To Failed Trans Status
            UpdateTransactionStatus(msg, 935950002);

            //msg.Context.UpdateObject(annotation);

            //msg.Context.SaveChanges();

            var newAnnotation = msg.OrganizationService.Create(annotation);
           

            LogHelper.LogDebug(msg.OrganizationName, msg.Debug, msg.SystemUserId, $"{ this.GetType().FullName}", $"| FFF End {this.GetType().FullName}.Execute()");
		}

        private static void UpdateTransactionStatus(IAddDependentMaintenanceRequestState msg, int status)
        {

            Entity thisEntity = new Entity();
            thisEntity.Id = msg.DependentMaintenanceId;
            thisEntity.LogicalName = "crme_dependentmaintenance";
            thisEntity["crme_txnstatus"] = new OptionSetValue(status);
            msg.OrganizationService.Update(thisEntity);
            ////Update Status 
            //msg.DependentMaintenance.crme_txnStatus = new OptionSetValue(status);

            ////Attach object to Context
            //msg.Context.Attach(msg.DependentMaintenance);

            ////Update the Context
            //msg.Context.UpdateObject(msg.DependentMaintenance);

            //msg.Context.SaveChanges();
        }

        public override void Execute(IAddDependentMaintenanceRequestState msg)
        {
        }
    }
}