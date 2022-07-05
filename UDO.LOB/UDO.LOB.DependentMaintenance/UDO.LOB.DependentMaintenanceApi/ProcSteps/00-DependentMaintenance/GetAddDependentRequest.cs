using CuttingEdge.Conditions;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
//using VRM.CRME.Plugin.DependentMaintenance;
using VRM.Integration.Servicebus.AddDependent;
using VRM.Integration.Servicebus.Core;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class GetAddDependentRequest: FilterBase<IAddDependentMaintenanceRequestState>
    {
        public override void Execute(IAddDependentMaintenanceRequestState msg)
        {
			//CSDev Rem 
			//Logger.Instance.Debug("Calling GetAddDependentRequest");

			Condition.Requires(msg.DependentMaintenance, "msg.Context").IsNotNull();

            msg.AddDependentRequest = msg.DependentMaintenance.MapAddDependentRequest(msg.Context);
			//msg.AddDependentRequest = DependentMaintenanceMappingExtensions.MapAddDependentRequest(msg.Context);

            Condition.Requires(msg.AddDependentRequest, "msg.AddDependentRequest").IsNotNull();

			LogHelper.LogDebug(msg.OrganizationName, msg.Debug, msg.SystemUserId, $"{ this.GetType().FullName}"
							, $"| VV End w/ Request {this.GetType().FullName} | RequestBody: {JsonHelper.Serialize(msg.AddDependentRequest, msg.AddDependentRequest.GetType())}");
		}
    } 
}
