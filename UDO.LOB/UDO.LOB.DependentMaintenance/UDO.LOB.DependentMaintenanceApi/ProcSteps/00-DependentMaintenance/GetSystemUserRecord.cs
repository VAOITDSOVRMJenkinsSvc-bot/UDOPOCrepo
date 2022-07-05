using System.Linq;
using CuttingEdge.Conditions;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Core;
using VRM.IntegrationServicebus.AddDependent.CrmModel;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class GetSystemUserRecord : FilterBase<IAddDependentMaintenanceRequestState>
    {
        public override void Execute(IAddDependentMaintenanceRequestState msg)
        {
			//CSDEv 
			//Logger.Instance.Debug("Calling GetSystemUserRecord");
			LogHelper.LogDebug(msg.OrganizationName, msg.Debug, msg.SystemUserId, "GetSystemUserRecord.Execute", "Calling GetSystemUserRecord");

			Condition.Requires(msg.Context, "msg.Context").IsNotNull();

            msg.SystemUser = (from d in msg.Context.CreateQuery<SystemUser>()
                                        where d.Id == msg.SystemUserId
                                        select d).FirstOrDefault();

            Condition.Requires(msg.SystemUser, "").IsNotNull();
        }
    }
}