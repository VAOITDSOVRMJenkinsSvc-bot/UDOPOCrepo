using System;
using CuttingEdge.Conditions;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Bgs.Services.VnpPersonServiceReference;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreateVeteran : FilterBase<IAddDependentRequestState>
    {
        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDev Rem 
			//Logger.Instance.Debug("Calling CreateVeteran");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreateVeteran.Execute", "Calling CreateVeteran");

			//Sleep for the configured number of seconds 
			//"AddDependentOrchestration".SleepSeconds();

            DateTime methodStartTime, wsStartTime;
            string method = "CreateVeteran", webService = "vnpPersonCreate";
			
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

            Condition.Requires(msg.VeteranRequestState.VeteranParticipant.BirthDate,
                "msg.VeteranRequestState.VeteranParticipant.BirthDate")
                .IsGreaterThan(DateTime.MinValue)
                .IsLessThan(DateTime.MaxValue);

            Condition.Requires(msg.VeteranRequestState.VeteranParticipant.FileNumber,
                "msg.VeteranRequestState.VeteranParticipant.FileNumber").IsNotNullOrEmpty();
            Condition.Requires(msg.VeteranRequestState.VeteranParticipant.FirstName,
                "msg.VeteranRequestState.VeteranParticipant.FirstName").IsNotNullOrEmpty();
            Condition.Requires(msg.VeteranRequestState.VeteranParticipant.LastName,
                "msg.VeteranRequestState.VeteranParticipant.LastName").IsNotNullOrEmpty();
            Condition.Requires(msg.VeteranRequestState.VeteranParticipant.Ssn,
                "msg.VeteranRequestState.VeteranParticipant.Ssn").IsNotNullOrEmpty();

            //if (msg.VeteranRequestState.VeteranParticipant.FileNumber !=
            //    msg.VeteranRequestState.VeteranParticipant.Ssn)
            //    throw new Exception("Veteran FileNumber does not equal Veteran SSN.");
   


            var vnpPersonDto = new VnpPersonDTO
            {
                jrnDt = DateTimeExtensions.TodayNoon,
                vnpProcId = msg.ProcRequestState.VnpProcId,
                vnpPtcpntId = msg.VeteranRequestState.VeteranParticipantId,
                firstNm = msg.VeteranRequestState.VeteranParticipant.FirstName,
                lastNm = msg.VeteranRequestState.VeteranParticipant.LastName,
                suffixNm = msg.VeteranRequestState.VeteranParticipant.SuffixName,
                slttnTypeNm = msg.VeteranRequestState.VeteranParticipant.TitleName,
                //titleTxt = msg.VeteranRequestState.VeteranParticipant.TitleName,
                brthdyDt = msg.VeteranRequestState.VeteranParticipant.BirthDate.Date.AddHours(12),
                brthdyDtSpecified = true,
                fileNbr = msg.VeteranRequestState.VeteranParticipant.FileNumber,
                ssnNbr = msg.VeteranRequestState.VeteranParticipant.Ssn,
                middleNm = msg.VeteranRequestState.VeteranParticipant.MiddleName,
                vetInd = msg.VeteranRequestState.VeteranParticipant.IsVetInd ? "Y" : "N",

                everMariedInd = msg.VeteranRequestState.VeteranParticipant.EverMarriedInd,
                martlStatusTypeCd = msg.VeteranRequestState.VeteranParticipant.MaritalStatus,
               
                vnpSruslyDsabldInd = msg.VeteranRequestState.VeteranParticipant.IsSeriouslyDisabled ? "Y" : "N",
                vnpSchoolChildInd = msg.VeteranRequestState.VeteranParticipant.IsScholdChild ? "Y" : "N"
            };

            if (!string.IsNullOrEmpty(msg.VeteranRequestState.VeteranParticipant.BirthCityName))
                vnpPersonDto.birthCityNm = msg.VeteranRequestState.VeteranParticipant.BirthCityName;

            if (!string.IsNullOrEmpty(msg.VeteranRequestState.VeteranParticipant.BirthStateCode))
                vnpPersonDto.birthStateCd = msg.VeteranRequestState.VeteranParticipant.BirthStateCode;

            if (!string.IsNullOrEmpty(msg.VeteranRequestState.VeteranParticipant.BirthCountryName))
                vnpPersonDto.birthCntryNm = msg.VeteranRequestState.VeteranParticipant.BirthCountryName;

            if (!string.IsNullOrEmpty(msg.VeteranRequestState.VeteranParticipant.FileNumber))
            {
                string fileNumber = msg.VeteranRequestState.VeteranParticipant.FileNumber;
                if (fileNumber.Length > 2)
                {
                    vnpPersonDto.termnlDigitNbr = fileNumber.Substring(fileNumber.Length - 2);
                }
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