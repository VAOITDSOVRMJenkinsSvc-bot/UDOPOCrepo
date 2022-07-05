using System;
using System.Globalization;
using CuttingEdge.Conditions;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntPhoneServiceReference;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntPhoneServiceReference;
using UDO.LOB.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreatePhoneNumberForDependent : FilterBase<IAddDependentRequestState>
    {
        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDEV Rem 
			//Logger.Instance.Debug("Calling CreatePhoneNumberForDependent");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreatePhoneNumberForDependent.Execute", "Calling CreatePhoneNumberForDependent");

			DateTime methodStartTime, wsStartTime;
            string method = "CreatePhoneNumberForDependent", webService = "vnpPtcpntPhoneCreate";

			Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

			Condition.Requires(msg, "msg").IsNotNull();

            Condition.Requires(msg.ProcRequestState, "msg.ProcRequestState").IsNotNull();
            Condition.Requires(msg.ProcRequestState.VnpProcId, "msg.ProcRequestState.VnpProcId").IsGreaterThan(0);

            Condition.Requires(msg.DependentRequestState, "msg.DependentRequestState").IsNotNull();
            Condition.Requires(msg.DependentRequestState.DependentParticipantId,
                "msg.DependentRequestState.DependentParticipantId").IsGreaterThan(0);
            Condition.Requires(msg.DependentRequestState.DependentParticipant,
                "msg.DependentRequestState.DependentParticipant")
                .IsNotNull();

            Condition.Requires(msg.DependentRequestState.DependentParticipant.PhoneNumbers,
                "msg.DependentRequestState.DependentParticipant.PhoneNumbers").IsNotNull();

            foreach (var phoneNumber in msg.DependentRequestState.DependentParticipant.PhoneNumbers)
            {
                Condition.Requires(phoneNumber.Number, "phoneNumber.Number").IsNotNullOrEmpty();
                Condition.Requires(phoneNumber.PhoneTypeName, "phoneNumber.PhoneTypeName").IsNotNullOrEmpty();
                Condition.Requires(phoneNumber.EffectiveDate, "phoneNumber.EffectiveDate")
                    .IsGreaterThan(DateTime.MinValue)
                    .IsLessThan(DateTime.MaxValue);

                var vnpPtcpntPhoneDto = new VnpPtcpntPhoneDTO
                {
                    jrnDt = DateTimeExtensions.TodayNoon,
                    vnpProcId = msg.ProcRequestState.VnpProcId,
                    vnpPtcpntId =
                        msg.DependentRequestState.DependentParticipantId.ToString(CultureInfo.InvariantCulture),
                    phoneNbr = phoneNumber.Number,
                    phoneTypeNm = phoneNumber.PhoneTypeName,
                    areaNbr = phoneNumber.AreaCode,
                    efctvDt = DateTimeExtensions.TodayNoon
                };

                var request = new vnpPtcpntPhoneCreateRequest(vnpPtcpntPhoneDto);

                Condition.Requires(msg.AddDependentMaintenanceRequestState, 
                    "msg.AddDependentMaintenanceRequestState").IsNotNull();
                Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName,
                    "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
                Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService, 
                    "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

                var service = BgsServiceFactory.GetVnpPtcpntPhoneService(msg.AddDependentMaintenanceRequestState.OrganizationName,
                    msg.AddDependentMaintenanceRequestState.OrganizationService, 
                    msg.AddDependentMaintenanceRequestState.SystemUserId);

				
				Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
					msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

				var response = service.vnpPtcpntPhoneCreate(request);

				
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