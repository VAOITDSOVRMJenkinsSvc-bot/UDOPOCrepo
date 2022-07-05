using System;
using System.Linq;
using CuttingEdge.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntAddrsServiceReference;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntAddrsServiceReference;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreateAddressForDependent : FilterBase<IAddDependentRequestState>
    {
        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDEv Rem 
			// Logger.Instance.Debug("Calling CreateAddressForDependent");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreateAddressForDependent.Execute", "Calling CreateAddressForDependent");

			DateTime methodStartTime, wsStartTime;
            string method = "CreateAddressForDependent", webService = "vnpPtcpntAddrsCreate";

			
            Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

            Condition.Requires(msg, "msg").IsNotNull();

            Condition.Requires(msg.ProcRequestState, "msg.ProcRequestState").IsNotNull();
            Condition.Requires(msg.ProcRequestState.VnpProcId, "msg.ProcRequestState.VnpProcId").IsGreaterThan(0);

            Condition.Requires(msg.DependentRequestState, "msg.DependentRequestState").IsNotNull();
            Condition.Requires(msg.DependentRequestState.DependentParticipantId,
                "msg.DependentRequestState.DependentParticipantId").IsGreaterThan(0);
            Condition.Requires(msg.DependentRequestState.DependentParticipant,
                "msg.DependentRequestState.DependentParticipant").IsNotNull();

            Condition.Requires(msg.DependentRequestState.DependentParticipant.Addresses,
                "msg.DependentRequestState.DependentParticipant.Addresses").IsNotNull().DoesNotHaveLength(0);

            foreach (var address in msg.DependentRequestState.DependentParticipant.Addresses.Where(address => 
                !string.IsNullOrEmpty(address.AddressLine1) && 
                !string.IsNullOrEmpty(address.City) && 
                !string.IsNullOrEmpty(address.Country) && 
                !string.IsNullOrEmpty(address.State) && 
                !string.IsNullOrEmpty(address.ZipCode)))
            {
                Condition.Requires(address.AddressTypeName, "address.AddressTypeName").IsNotNullOrEmpty();
                Condition.Requires(address.AddressLine1, "address.AddressLine1").IsNotNullOrEmpty();
                Condition.Requires(address.City, "address.City").IsNotNullOrEmpty();
                Condition.Requires(address.Country, "address.Country").IsNotNullOrEmpty();
                Condition.Requires(address.State, "address.State").IsNotNullOrEmpty();
                Condition.Requires(address.ZipCode, "address.ZipCode").IsNotNullOrEmpty();
                Condition.Requires(address.AddressTypeName, "address.AddressTypeName").IsNotNullOrEmpty();
                Condition.Requires(address.EffectiveDate, "address.EffectiveDate")
                    .IsGreaterThan(DateTime.MinValue)
                    .IsLessThan(DateTime.MaxValue);

                var vnpPtcpntAddrsDto = new VnpPtcpntAddrsDTO
                {
                    jrnDt = DateTimeExtensions.TodayNoon,
                    vnpProcId = msg.ProcRequestState.VnpProcId,
                    vnpPtcpntId = msg.DependentRequestState.DependentParticipantId,
                    efctvDt = address.EffectiveDate,
                    addrsOneTxt = address.AddressLine1,
                    addrsTwoTxt = address.AddressLine2,
                    addrsThreeTxt = address.AddressLine3,
                    cityNm = address.City,
                    cntryNm = address.Country,
                    countyNm = address.County,
                    postalCd = address.State,
                    zipPrefixNbr = address.ZipCode,
                    zipFirstSuffixNbr = address.ZipPlus4,
                    ptcpntAddrsTypeNm = address.AddressTypeName,
                    sharedAddrsInd = address.SharedAddressIndicator ? "Y" : "N"
                };

                var request = new vnpPtcpntAddrsCreateRequest(vnpPtcpntAddrsDto);

                Condition.Requires(msg.AddDependentMaintenanceRequestState, 
                    "msg.AddDependentMaintenanceRequestState").IsNotNull();
                Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName,
                    "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
                Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService, 
                    "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

                var service = BgsServiceFactory.GetVnpPtcpntAddrsService(msg.AddDependentMaintenanceRequestState.OrganizationName,
                    msg.AddDependentMaintenanceRequestState.OrganizationService, 
                    msg.AddDependentMaintenanceRequestState.SystemUserId);

				
				Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
					msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

				var response = service.vnpPtcpntAddrsCreate(request);
				
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