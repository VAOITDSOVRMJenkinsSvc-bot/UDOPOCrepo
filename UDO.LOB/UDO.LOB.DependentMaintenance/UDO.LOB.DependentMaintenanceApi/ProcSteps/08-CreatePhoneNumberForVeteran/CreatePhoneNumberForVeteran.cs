using System;
using System.Globalization;
using CuttingEdge.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntPhoneServiceReference;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntPhoneServiceReference;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreatePhoneNumberForVeteran : FilterBase<IAddDependentRequestState>
    {
        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDEV REm 
            //Logger.Instance.Debug("Calling CreatePhoneNumberForVeteran");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreatePhoneNumberForVeteran.Execute", "Calling CreatePhoneNumberForVeteran");

			DateTime methodStartTime, wsStartTime;
            string method = "CreatePhoneNumberForVeteran", webService = "vnpPtcpntPhoneCreate";
			
			Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

			Condition.Requires(msg, "msg").IsNotNull();

            Condition.Requires(msg.ProcRequestState, "msg.ProcRequestState").IsNotNull();
            Condition.Requires(msg.ProcRequestState.VnpProcId, "msg.ProcRequestState.VnpProcId").IsGreaterThan(0);

            Condition.Requires(msg.VeteranRequestState, "msg.VeteranRequestState").IsNotNull();
            Condition.Requires(msg.VeteranRequestState.VeteranParticipantId,
                "msg.VeteranRequestState.VeteranParticipantId").IsGreaterThan(0);
            Condition.Requires(msg.VeteranRequestState.VeteranParticipant,
                "msg.VeteranRequestState.VeteranParticipant")
                .IsNotNull();

            Condition.Requires(msg.VeteranRequestState.VeteranParticipant.PhoneNumbers,
                "msg.VeteranRequestState.VeteranParticipant.PhoneNumbers").IsNotNull();

            foreach (var phoneNumber in msg.VeteranRequestState.VeteranParticipant.PhoneNumbers)
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
                    vnpPtcpntId = msg.VeteranRequestState.VeteranParticipantId.ToString(CultureInfo.InvariantCulture),
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