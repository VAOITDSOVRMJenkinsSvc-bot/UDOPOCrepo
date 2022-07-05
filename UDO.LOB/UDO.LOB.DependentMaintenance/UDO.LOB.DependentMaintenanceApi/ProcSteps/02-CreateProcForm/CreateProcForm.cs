using CuttingEdge.Conditions;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Bgs.Services.VnpProcFormServiceReference;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
using UDO.LOB.Extensions.Logging;
using System;
using UDO.LOB.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreateProcForm : FilterBase<IAddDependentRequestState>
    {
        private const string _FormTypeCd = "21-686c";
        private const string _FromType674 = "21-674";

        public override void Execute(IAddDependentRequestState msg)
        {
            bool is686 = false;
            bool is674 = false;
			//CSDEv REm 
			//Logger.Instance.Debug("Calling CreateProcForm");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreateProcForm.Execute", "Calling CreateProcForm");

			DateTime methodStartTime, wsStartTime;
            string method = "CreateProcForm", webService = "vnpProcFormCreate";

			Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

			Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, "time just for 2 logging", out wsStartTime);
			LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

			wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, "code to chanel", out wsStartTime);

			Condition.Requires(msg, "msg")
                .IsNotNull();

            Condition.Requires(msg.ProcRequestState, "_State.ProcRequestState")
                .IsNotNull();

            Condition.Requires(msg.ProcRequestState.VnpProcId, "_State.ProcRequestState.VnpProcId").IsGreaterThan(0);

            foreach (var dependent in msg.AddDependentMaintenanceRequestState.AddDependentRequest.Dependents)
            {
                if ((dependent.MaintenanceType == "Add") || (dependent.MaintenanceType == "Remove"))
                {
                    is686 = true;
                    break;
                }
            }
            foreach (var dependent in msg.AddDependentMaintenanceRequestState.AddDependentRequest.Dependents)
            {
                if ((dependent.MaintenanceType == "Edit" && dependent.IsScholdChild == true) || (dependent.MaintenanceType == "Add" && dependent.IsScholdChild == true))
                {
                    is674 = true;
                    break;
                }
            }
            if(is686 == true)
            {
                var vnpProcFormDto = new VnpProcFormDTO
                {
                    jrnDt = DateTimeExtensions.TodayNoon,
                    compId = new VnpProcFormPKDTO
                    {
                        vnpProcId = msg.ProcRequestState.VnpProcId,
                        formTypeCd = _FormTypeCd
                    }
                };
                var request = new vnpProcFormCreateRequest(vnpProcFormDto);

                Condition.Requires(msg.AddDependentMaintenanceRequestState, "msg.AddDependentMaintenanceRequestState").IsNotNull();
                Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName, "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
                Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService, "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

                //LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

                wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                    msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, "CreateChannel", out wsStartTime);

                var service = BgsServiceFactory.GetVnpProcFormService(msg.AddDependentMaintenanceRequestState.OrganizationName,
                    msg.AddDependentMaintenanceRequestState.OrganizationService,
                    msg.AddDependentMaintenanceRequestState.SystemUserId);

                LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

                /*Guid */
                wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
           msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

                var response = service.vnpProcFormCreate(request);

                LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);


                Condition.Ensures(response, "response").IsNotNull();

                LogHelper.EndTiming(methodLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTime);

                var beautifiedRequest = JToken.Parse(JsonHelper.Serialize(request, request.GetType())).ToString(Formatting.Indented);
                var beautifiedResponse = JToken.Parse(JsonHelper.Serialize(response, response.GetType())).ToString(Formatting.Indented);

                LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
                    , msg.AddDependentMaintenanceRequestState.SystemUserId, $"{ this.GetType().FullName}"
                                , $"| RRR End w/ Request / Response {this.GetType().FullName} \r\n\r\n " +
                                $"|| RequestBody: \r\n { beautifiedRequest } \r\n\r\n" +
                                $"|| ResponseBody: \r\n { beautifiedResponse }");

            }

            if(is674 == true)
            {
                var vnpProcFormDto = new VnpProcFormDTO
                {
                    jrnDt = DateTimeExtensions.TodayNoon,
                    compId = new VnpProcFormPKDTO
                    {
                        vnpProcId = msg.ProcRequestState.VnpProcId,
                        formTypeCd = _FromType674
                    }
                };
                var request = new vnpProcFormCreateRequest(vnpProcFormDto);

                Condition.Requires(msg.AddDependentMaintenanceRequestState, "msg.AddDependentMaintenanceRequestState").IsNotNull();
                Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName, "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
                Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService, "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

                //LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

                wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                    msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, "CreateChannel", out wsStartTime);

                var service = BgsServiceFactory.GetVnpProcFormService(msg.AddDependentMaintenanceRequestState.OrganizationName,
                    msg.AddDependentMaintenanceRequestState.OrganizationService,
                    msg.AddDependentMaintenanceRequestState.SystemUserId);

                LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

                /*Guid */
                wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
           msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

                var response = service.vnpProcFormCreate(request);

                LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);


                Condition.Ensures(response, "response").IsNotNull();

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
}