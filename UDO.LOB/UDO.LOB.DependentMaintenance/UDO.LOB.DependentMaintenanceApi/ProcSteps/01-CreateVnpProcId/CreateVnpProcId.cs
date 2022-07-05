using CuttingEdge.Conditions;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Bgs.Services.VnpProcServiceReference;
//using VRM.Integration.Servicebus.Logging.CRM.Messages;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
using System;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services.VnpProcServiceReference;
using VRM.Integration.Servicebus.Bgs.Services;
using UDO.LOB.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreateVnpProcId : FilterBase<IAddDependentRequestState>
    {
        private const string _VnpProcTypeCd = "DEPCHG";
        private const string _VnpProcStateTypeCd = "Started";



        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDev Rem 
            //Logger.Instance.Debug("Calling CreateVnpProcId");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.Debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreateVnpProcId.Execute", "Calling CreateVnpProcId");

			DateTime methodStartTime, wsStartTime;
            string method = "CreateVnpProcId", webService = "vnpProcCreate";

			Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

			Condition.Requires(msg, "msg")
                .IsNotNull();

            var vnpPropDto = new VnpProcDTO
            {
                jrnDt = DateTimeExtensions.TodayNoon,
                vnpProcTypeCd = _VnpProcTypeCd,
                vnpProcStateTypeCd = _VnpProcStateTypeCd,
                creatdDt = msg.ProcRequestState.CreatedDate,
                creatdDtSpecified = true,
                lastModifdDt = msg.ProcRequestState.LastModifiedDate,
                lastModifdDtSpecified = true
            };

            var request = new vnpProcCreateRequest(vnpPropDto);

            Condition.Requires(msg.AddDependentMaintenanceRequestState, 
                "msg.AddDependentMaintenanceRequestState").IsNotNull();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName,
               "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService, 
                "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();


			//CSDEv Rem
			
            var service = BgsServiceFactory.GetVnpProcService(msg.AddDependentMaintenanceRequestState.OrganizationName,
                msg.AddDependentMaintenanceRequestState.OrganizationService, 
                msg.AddDependentMaintenanceRequestState.SystemUserId);

			Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

			var response = service.vnpProcCreate(request);

			LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

			Condition.Requires(response, "response").IsNotNull();
			
			msg.ProcRequestState.VnpProcId = response.@return.vnpProcId;

			Condition.Ensures(msg.ProcRequestState.VnpProcId, "msg.ProcRequestState.VnpProcId").IsGreaterThan(0);
			
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