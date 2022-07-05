using System;
using System.Linq;
using CuttingEdge.Conditions;
//using DocumentFormat.OpenXml.Presentation;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntAddrsServiceReference;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
using UDO.LOB.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreateAddressForVeteran : FilterBase<IAddDependentRequestState>
    {
        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDev REm 
			//Logger.Instance.Debug("Calling CreateAddressForVeteran");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreateAddressForVeteran.Execute", "Calling CreateAddressForVeteran");

			DateTime methodStartTime, wsStartTime;
            string method = "CreateAddressForVeteran", webService = "vnpPtcpntAddrsCreate";

			
            Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

    
            Condition.Requires(msg, "msg").IsNotNull();

            Condition.Requires(msg.ProcRequestState, "msg.ProcRequestState").IsNotNull();
            Condition.Requires(msg.ProcRequestState.VnpProcId, "msg.ProcRequestState.VnpProcId").IsGreaterThan(0);

            Condition.Requires(msg.VeteranRequestState, "msg.VeteranRequestState").IsNotNull();
            Condition.Requires(msg.VeteranRequestState.VeteranParticipantId,
                "msg.VeteranRequestState.VeteranParticipantId").IsGreaterThan(0);
            Condition.Requires(msg.VeteranRequestState.VeteranParticipant,
                "msg.VeteranRequestState.VeteranParticipant").IsNotNull();

            Condition.Requires(msg.VeteranRequestState.VeteranParticipant.Addresses,
                "msg.VeteranRequestState.VeteranParticipant.Addresses").IsNotNull().DoesNotHaveLength(0);

            foreach (var address in msg.VeteranRequestState.VeteranParticipant.Addresses.Where(address => 
                !string.IsNullOrEmpty(address.AddressLine1) && 
                !string.IsNullOrEmpty(address.City) && 
                !string.IsNullOrEmpty(address.Country) && 
                !string.IsNullOrEmpty(address.State) && 
                !string.IsNullOrEmpty(address.ZipCode)))
            {
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
                    vnpPtcpntId = msg.VeteranRequestState.VeteranParticipantId,
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
                    sharedAddrsInd = address.SharedAddressIndicator ? "Y" : "N",
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

				msg.ParticipantMailAddressId = response.@return.vnpPtcpntAddrsId;
				
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