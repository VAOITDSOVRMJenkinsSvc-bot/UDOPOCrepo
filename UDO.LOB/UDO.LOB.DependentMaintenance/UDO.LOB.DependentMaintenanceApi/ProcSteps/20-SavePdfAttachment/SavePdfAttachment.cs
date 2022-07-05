using CuttingEdge.Conditions;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Core;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class SavePdfAttachment : FilterBase<IAddDependentRequestState>
    {
        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDev 
            //Logger.Instance.Debug("Calling SavePdfAttachment");
			LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
				, msg.AddDependentMaintenanceRequestState.SystemUserId, "SavePdfAttachment.Execute", "Calling SavePdfAttachment");

			Condition.Requires(msg, "msg")
               .IsNotNull();
        }
    }
}