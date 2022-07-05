using System;
using CuttingEdge.Conditions;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntServiceReference;

//using VRM.Integration.Servicebus.Logging.CRM.Util;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreateDependentParticipant : FilterBase<IAddDependentRequestState>
    {
        private const string _PtcpntTypeNm = "Person";

        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDev Rem 
			//Logger.Instance.Debug("Calling CreateDependentParticipant");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreateDependentParticipant.Execute"
				//, "Calling CreateDependentParticipant");

			DateTime methodStartTime, wsStartTime;
            string method = "CreateDependentParticipant", webService = "vnpPtcpntCreate";

			Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

			Condition.Requires(msg, "msg").IsNotNull();

            Condition.Requires(msg.ProcRequestState, "msg.ProcRequestState").IsNotNull();

            Condition.Requires(msg.ProcRequestState.VnpProcId, "msg.ProcRequestState.VnpProcId").IsGreaterThan(0);

            Condition.Requires(msg.DependentRequestState, "msg.DependentRequestState").IsNotNull();
            Condition.Requires(msg.DependentRequestState.DependentParticipant,
                "msg.DependentRequestState.DependentParticipant").IsNotNull();

            var vnpPtcpntDto = new VnpPtcpntDTO
            {
                jrnDt = DateTimeExtensions.TodayNoon,
                vnpProcId = msg.ProcRequestState.VnpProcId,
                ptcpntTypeNm = _PtcpntTypeNm
            };

            var request = new vnpPtcpntCreateRequest(vnpPtcpntDto);

            Condition.Requires(msg.AddDependentMaintenanceRequestState, 
                "msg.AddDependentMaintenanceRequestState").IsNotNull();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName,
               "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService, 
                "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

            var service = BgsServiceFactory.GetVnpPtcpntService(msg.AddDependentMaintenanceRequestState.OrganizationName,
                msg.AddDependentMaintenanceRequestState.OrganizationService, 
                msg.AddDependentMaintenanceRequestState.SystemUserId);

			Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

			var response = service.vnpPtcpntCreate(request);

			LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

			Condition.Ensures(response, "response").IsNotNull();

            msg.DependentRequestState.DependentParticipantId = response.@return.vnpPtcpntId;

            Condition.Ensures(msg.DependentRequestState.DependentParticipantId,
                "msg.DependentRequestState.DependentParticipantId").IsGreaterThan(0);

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