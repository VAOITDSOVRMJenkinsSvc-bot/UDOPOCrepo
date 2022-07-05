using CuttingEdge.Conditions;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class GetBgsHeaderInfo : FilterBase<IAddDependentMaintenanceRequestState>
    {
        public override void Execute(IAddDependentMaintenanceRequestState msg)
        {
			//CSDEv REm 
			//Logger.Instance.Debug("Calling GetBgsHeaderInfo");
			LogHelper.LogDebug(msg.OrganizationName, msg.Debug, msg.SystemUserId, "GetBgsHeaderInfo.Execute", "Calling GetBgsHeaderInfo");

			Condition.Requires(msg.OrganizationService, "msg.OrganizationService").IsNotNull();

            msg.BgsHeaderInfo = ClientBaseExtensions.GetBgsHeaderInfo(msg.OrganizationService, msg.SystemUserId);

            Condition.Requires(msg.SystemUser, "").IsNotNull();
        }
    }
}