using System;
using CuttingEdge.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Bgs.Services.VnpChildSchoolServiceReference;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreateVnpChildSchool : FilterBase<IAddDependentRequestState>
    {
        public override void Execute(IAddDependentRequestState msg)
        {
            //CSDEv Rem
            //Logger.Instance.Debug("Calling CreateVnpChildSchool");
            //LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.,debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreateVnpChildSchool.Execute", "Calling CreateVnpChildSchool");

            DateTime methodStartTime, wsStartTime;
            string method = "CreateVnpChildSchool", webService = "vnpChildSchoolCreate";

            Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);
                       
            Condition.Requires(msg, "msg")
                .IsNotNull();
           
            Condition.Requires(msg.ProcRequestState, "msg.ProcRequestState").IsNotNull();
            Condition.Requires(msg.ProcRequestState.VnpProcId, "msg.ProcRequestState.VnpProcId").IsGreaterThan(0);

            Condition.Requires(msg.VeteranRequestState, "msg.VeteranRequestState").IsNotNull();
            Condition.Requires(msg.VeteranRequestState.VeteranParticipantId,
                "msg.VeteranRequestState.VeteranParticipantId").IsGreaterThan(0);
            Condition.Requires(msg.VeteranRequestState.VeteranParticipant,
                "msg.VeteranRequestState.VeteranParticipant").IsNotNull();

            Condition.Requires(msg.DependentRequestState, "msg.DependentRequestState").IsNotNull();
            Condition.Requires(msg.DependentRequestState.DependentParticipantId,
                "msg.DependentRequestState.DependentParticipantId").IsGreaterThan(0);
            Condition.Requires(msg.DependentRequestState.DependentParticipant,
                "msg.DependentRequestState.DependentParticipant").IsNotNull();

            Condition.Requires(msg.DependentRequestState.DependentParticipant.DependentRelationship,
                "msg.DependentRequestState.DependentParticipant.DependentRelationship").IsNotNull();
            
            var dependent = msg.DependentRequestState.DependentParticipant;


            if (dependent.IsScholdChild == true)
            {

                var VnpChildSchoolDto = new VnpChildSchoolDTO
                {
                    jrnDt = DateTimeExtensions.TodayNoon,
                    jrnLctnId = msg.StationOfJurisdiction,
                    jrnObjId = BgsSecurityConfiguration.Current.ApplicationId,
                    jrnStatusTypeCd = BenefitClaimConstants.JrnInsertStatusTypeCd,
                    jrnUserId = msg.AddDependentMaintenanceRequestState.BgsHeaderInfo.LoginName,
                    vnpProcId = msg.ProcRequestState.VnpProcId,
                    vnpPtcpntId = msg.DependentRequestState.DependentParticipantId,
                    courseNameTxt = dependent.CourseName,
                    curntSchoolAddrsOneTxt = dependent.SchoolAddressLine1,
                    curntSchoolAddrsZipNbr = dependent.SchoolAddressZip,
                    curntSchoolCityNm = dependent.SchoolAddressCity,
                    curntSchoolNm = dependent.SchoolName,
                    curntSchoolPostalCd = dependent.SchoolAddressState,
                    fullTimeStudntTypeCd = dependent.FullTimeStudentTypeCode,
                    gradtnDt = dependent.ExpectedGradDate,
                    gradtnDtSpecified = true,
                    lastTermEnrlmtInd = dependent.IsAttendedLastTerm ? "Y" : "N",
                    schoolActualExpctdStartDt = dependent.ExpectedStartDate,
                    schoolActualExpctdStartDtSpecified = true,
                    schoolTermStartDt = dependent.CourseBeginDate,
                    schoolTermStartDtSpecified = true
                   // vnpChildSchoolId = long.Parse(dependent.SchoolCode)
                };

                if (dependent.SchoolCode != null)
                {
                    VnpChildSchoolDto.currentEduInstnPtcpntIdSpecified = true;
                    VnpChildSchoolDto.currentEduInstnPtcpntId = long.Parse(dependent.SchoolCode);
                }
               

                if (dependent.IsAttendedLastTerm == true)
                {
                    if(dependent.PrevSchoolCode != null)
                    {
                        VnpChildSchoolDto.prevEduInstnPtcpntIdSpecified = true;
                        VnpChildSchoolDto.prevEduInstnPtcpntId = long.Parse(dependent.PrevSchoolCode);
                    }
                    VnpChildSchoolDto.lastTermEndDtSpecified = true;
                    VnpChildSchoolDto.lastTermEndDt = dependent.AttendedEndDate;
                    VnpChildSchoolDto.lastTermStartDtSpecified = true;
                    VnpChildSchoolDto.lastTermStartDt = dependent.AttendedBeginDate;
                    VnpChildSchoolDto.prevHoursPerWkNumSpecified = true;
                    VnpChildSchoolDto.prevHoursPerWkNum = dependent.AttendedHoursPerWeek;
                    VnpChildSchoolDto.prevSchoolAddrsOneTxt = dependent.AttendedSchoolAddressLine1;
                    VnpChildSchoolDto.prevSchoolAddrsZipNbr = dependent.AttendedSchoolAddressZip;
                    VnpChildSchoolDto.prevSchoolCityNm = dependent.AttendedSchoolAddressCity;
                    VnpChildSchoolDto.prevSchoolNm = dependent.AttendedSchool;
                    VnpChildSchoolDto.prevSchoolPostalCd = dependent.AttendedSchoolAddressState;
                    VnpChildSchoolDto.prevSessnsPerWkNumSpecified = true;
                    VnpChildSchoolDto.prevSessnsPerWkNum = dependent.AttendedSessionsPerWeek;

                }

                var request = new vnpChildSchoolCreateRequest(VnpChildSchoolDto);
                Condition.Requires(msg.AddDependentMaintenanceRequestState,
                "msg.AddDependentMaintenanceRequestState").IsNotNull();
                Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName,
                        "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
                Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService,
                    "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

                var service = BgsServiceFactory.GetVnpChildSchoolService(msg.AddDependentMaintenanceRequestState.OrganizationName,
                msg.AddDependentMaintenanceRequestState.OrganizationService,
                msg.AddDependentMaintenanceRequestState.SystemUserId);


                Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                          msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

                var response = service.vnpChildSchoolCreate(request);

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

          //  Condition.Requires(dependent.IsScholdChild).IsEqualTo(true);

          //  Condition.Requires(dependent.SchoolCode).IsNotNull();// this means school is not entered manually. If school is entered manually then we will not submit the information to End point.
               

        }
    }
}