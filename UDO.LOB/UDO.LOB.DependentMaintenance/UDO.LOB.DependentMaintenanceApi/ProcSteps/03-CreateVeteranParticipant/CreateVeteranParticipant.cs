using CuttingEdge.Conditions;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntServiceReference;
using System;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntServiceReference;
using VRM.Integration.Servicebus.Bgs.Services;
using UDO.LOB.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreateVeteranParticipant : FilterBase<IAddDependentRequestState>
    {
        private const string _PtcpntTypeNm = "Person";

        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDEv Rem 
			//Logger.Instance.Debug("Calling CreateVeteranParticipant");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreateVeteranParticipant.Execute", "Calling CreateVeteranParticipant");


			DateTime methodStartTime, wsStartTime;
            string method = "CreateVeteranParticipant", webService = "vnpPtcpntCreate";
            Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

            Condition.Requires(msg, "_State").IsNotNull();

            Condition.Requires(msg.ProcRequestState, "_State.ProcRequestState").IsNotNull();

            Condition.Requires(msg.ProcRequestState.VnpProcId, "_State.ProcRequestState.VnpProcId").IsGreaterThan(0);

            Condition.Requires(msg.VeteranRequestState, "_State.VeteranRequestState").IsNotNull();

            Condition.Requires(msg.VeteranRequestState.VeteranParticipant,"_State.VeteranRequestState.VeteranParticipant").IsNotNull();

            Condition.Requires(msg.VeteranRequestState.VeteranParticipant.CorpParticipantId,"_State.VeteranRequestState.VeteranParticipant.CorpParticipantId").IsGreaterThan(0);

            var vnpPtcpntDto = new VnpPtcpntDTO
            {
                jrnDt = DateTimeExtensions.TodayNoon,
                vnpProcId = msg.ProcRequestState.VnpProcId,
                ptcpntTypeNm = _PtcpntTypeNm,
                corpPtcpntId = msg.VeteranRequestState.VeteranParticipant.CorpParticipantId,
                corpPtcpntIdSpecified = true
            };

            var request = new vnpPtcpntCreateRequest(vnpPtcpntDto);

            Condition.Requires(msg.AddDependentMaintenanceRequestState, "msg.AddDependentMaintenanceRequestState").IsNotNull();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName, "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService, "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

            var service = BgsServiceFactory.GetVnpPtcpntService(msg.AddDependentMaintenanceRequestState.OrganizationName,
                msg.AddDependentMaintenanceRequestState.OrganizationService, 
                msg.AddDependentMaintenanceRequestState.SystemUserId);

            Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

            var response = service.vnpPtcpntCreate(request);

            LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

            Condition.Ensures(response, "response").IsNotNull();

            msg.VeteranRequestState.VeteranParticipantId = response.@return.vnpPtcpntId;

            Condition.Ensures(msg.VeteranRequestState.VeteranParticipantId,
                "_State.VeteranRequestState.VeteranParticipantId").IsGreaterThan(0);

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