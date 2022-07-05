using System;
using CuttingEdge.Conditions;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Bgs.Services.VnpProcServiceReference;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Bgs.Services.VnpProcServiceReference;
using UDO.LOB.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class SetVnpProcStateToReady : FilterBase<IAddDependentRequestState>
    {
        private const string _VnpProcTypeCd = "DEPCHG";
        private const string _VnpProcStateTypeCd = "Ready";

        public override void Execute(IAddDependentRequestState msg)
        {

            DateTime methodStartTime, wsStartTime;
            string method = "SetVnpProcStateToReady", webService = "vnpProcUpdate";

			
			Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

			Condition.Requires(msg.AddDependentMaintenanceRequestState,
                "msg.AddDependentMaintenanceRequestState").IsNotNull();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.Context,
                "msg.AddDependentMaintenanceRequestState.Context").IsNotNull();

            if (!AddDependentConfiguration.GetCurrent(msg.AddDependentMaintenanceRequestState.Context).OrchestrationSetReadyState)
                return;

			//CSdev Rem 
			//Logger.Instance.Debug("Calling SetVnpProcStateToReady");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "SetVnpProcStateToReady.Execute", "Calling SetVnpProcStateToReady", true);

			Condition.Requires(msg, "msg")
                .IsNotNull();

            Condition.Requires(msg.ProcRequestState, "msg.ProcRequestState").IsNotNull();
            Condition.Requires(msg.ProcRequestState.VnpProcId, "msg.ProcRequestState.VnpProcId").IsGreaterThan(0);
            
            var vnpProcDto = new VnpProcDTO
            {
                jrnDt = DateTimeExtensions.TodayNoon,
                vnpProcId = msg.ProcRequestState.VnpProcId,
                vnpProcTypeCd = _VnpProcTypeCd,
                vnpProcStateTypeCd = _VnpProcStateTypeCd,
                creatdDt = msg.ProcRequestState.CreatedDate.AddHours(12),
                creatdDtSpecified = true,
                lastModifdDt = msg.ProcRequestState.LastModifiedDate.AddHours(12),
                lastModifdDtSpecified = true
            };

            var request = new vnpProcUpdateRequest(vnpProcDto);

            Condition.Requires(msg.AddDependentMaintenanceRequestState, 
                "msg.AddDependentMaintenanceRequestState").IsNotNull();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName,
               "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService, 
                "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

            var service = BgsServiceFactory.GetVnpProcService(msg.AddDependentMaintenanceRequestState.OrganizationName,
                msg.AddDependentMaintenanceRequestState.OrganizationService, 
                msg.AddDependentMaintenanceRequestState.SystemUserId);

			
			Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

			var response = service.vnpProcUpdate(request);

			
			LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

			Condition.Requires(response, "response").IsNotNull();

			
			LogHelper.EndTiming(methodLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTime);

			var beautifiedRequest = JToken.Parse(JsonHelper.Serialize(request, request.GetType())).ToString(Formatting.Indented);
			var beautifiedResponse = JToken.Parse(JsonHelper.Serialize(response, response.GetType())).ToString(Formatting.Indented);

			LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
				, msg.AddDependentMaintenanceRequestState.SystemUserId, $"{ this.GetType().FullName}"
							, $"| RRR End w/ Request / Response {this.GetType().FullName} \r\n\r\n " +
							$"|| RequestBody: \r\n { beautifiedRequest } \r\n\r\n" +
							$"|| ResponseBody: \r\n { beautifiedResponse }");
		}
    }
}