using System;
using CuttingEdge.Conditions;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Bgs.Services.VnpPersonServiceReference;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Bgs.Services.VnpPersonServiceReference;
using UDO.LOB.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
//using VRM.Integration.Servicebus.AddDependent;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreateDependent : FilterBase<IAddDependentRequestState>
    {
        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDev Rem 
			//Logger.Instance.Debug("Calling CreateDependent");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreateDependent.Execute", "Calling CreateDependent");

			DateTime methodStartTime, wsStartTime;
            string method = "CreateDependent", webService = "vnpPersonCreate";

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

            Condition.Requires(msg.DependentRequestState.DependentParticipant.BirthDate,
                "msg.DependentRequestState.DependentParticipant.BirthDate")
                .IsGreaterThan(DateTime.MinValue)
                .IsLessThan(DateTime.MaxValue);

            Condition.Requires(msg.DependentRequestState.DependentParticipant.FirstName,
                "msg.DependentRequestState.DependentParticipant.FirstName").IsNotNullOrEmpty();

            Condition.Requires(msg.DependentRequestState.DependentParticipant.LastName,
                "msg.DependentRequestState.DependentParticipant.LastName").IsNotNullOrEmpty();
            
            var vnpPersonDto = new VnpPersonDTO
            {
                jrnDt = DateTimeExtensions.TodayNoon,
                vnpProcId = msg.ProcRequestState.VnpProcId,
                vnpPtcpntId = msg.DependentRequestState.DependentParticipantId,
                firstNm = msg.DependentRequestState.DependentParticipant.FirstName,
                lastNm = msg.DependentRequestState.DependentParticipant.LastName,
                suffixNm = msg.DependentRequestState.DependentParticipant.SuffixName,
                //slttnTypeNm = msg.DependentRequestState.DependentParticipant.TitleName, 
                //titleTxt = msg.DependentRequestState.DependentParticipant.TitleName,
                brthdyDt = msg.DependentRequestState.DependentParticipant.BirthDate.Date.AddHours(12),
                brthdyDtSpecified = true,
                //fileNbr = msg.DependentRequestState.DependentParticipant.FileNumber,
                ssnNbr = msg.DependentRequestState.DependentParticipant.Ssn,

                middleNm = msg.DependentRequestState.DependentParticipant.MiddleName,
                vetInd = msg.DependentRequestState.DependentParticipant.IsVetInd ? "Y" : "N",
                birthCityNm = msg.DependentRequestState.DependentParticipant.BirthCityName,
                birthStateCd = msg.DependentRequestState.DependentParticipant.BirthStateCode,
                birthCntryNm = msg.DependentRequestState.DependentParticipant.BirthCountryName,
                vnpSruslyDsabldInd = msg.DependentRequestState.DependentParticipant.IsSeriouslyDisabled ? "Y" : "N",
                vnpSchoolChildInd = msg.DependentRequestState.DependentParticipant.IsScholdChild ? "Y" : "N",
                noSsnReasonTypeCd = msg.DependentRequestState.DependentParticipant.NoSssnReasonTypeCd,
            };


            Condition.Requires(msg.DependentRequestState.DependentParticipant.DependentRelationship,
               "msg.DependentRequestState.DependentParticipant.DependentRelationship").IsNotNull();

            var relationship = msg.DependentRequestState.DependentParticipant.DependentRelationship;

            Condition.Requires(relationship.FamilyRelationshipTypeName, "relationship.FamilyRelationshipTypeName")
                .IsNotNullOrEmpty();

            if (relationship.FamilyRelationshipTypeName.Equals("Spouse"))
            {
                //make sure spouse is veteran.
                vnpPersonDto.everMariedInd = "Y";
                vnpPersonDto.martlStatusTypeCd = "Married";

            }
            
            var request = new vnpPersonCreateRequest(vnpPersonDto);

            Condition.Requires(msg.AddDependentMaintenanceRequestState, 
                "msg.AddDependentMaintenanceRequestState").IsNotNull();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName,
                    "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService, 
                "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

            var service = BgsServiceFactory.GetVnpPersonService(msg.AddDependentMaintenanceRequestState.OrganizationName,
                msg.AddDependentMaintenanceRequestState.OrganizationService, 
                msg.AddDependentMaintenanceRequestState.SystemUserId);

			Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

			var response = service.vnpPersonCreate(request);

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