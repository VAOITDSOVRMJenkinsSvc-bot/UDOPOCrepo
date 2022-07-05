using System;
using CuttingEdge.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Bgs.Services.VnpChildStudentServiceReference;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreateVnpChildStudent : FilterBase<IAddDependentRequestState>
    {
        public override void Execute(IAddDependentRequestState msg)
        {
            //CSDEv Rem 
            //Logger.Instance.Debug("Calling CreateVnpChildStudent");
            //LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreateVnpChildStudent.Execute", "Calling CreateVnpChildStudent");

            DateTime methodStartTime, wsStartTime;
            string method = "CreateVnpChildStudent", webService = "vnpChildStudentCreate";

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
            // Condition.Requires(dependent.IsScholdChild).IsEqualTo(true);

            // Condition.Requires(dependent.SchoolCode).IsNotNull();

            if (dependent.IsScholdChild == true)
            {
                var VnpChildStudntDto = new VnpChildStudntDTO
                {
                    govtPaidTuitnInd = dependent.IsPaidByDEA ? "Y" : "N",
                    jrnDt = DateTimeExtensions.TodayNoon,
                    jrnLctnId = msg.StationOfJurisdiction,
                    jrnObjId = BgsSecurityConfiguration.Current.ApplicationId,
                    jrnStatusTypeCd = BenefitClaimConstants.JrnInsertStatusTypeCd,
                    jrnUserId = msg.AddDependentMaintenanceRequestState.BgsHeaderInfo.LoginName,
                    otherAssetAmt = dependent.OtherAssests,
                    otherAssetAmtSpecified = true,
                    realEstateAmt = dependent.RealEstate,
                    realEstateAmtSpecified = true,
                    savingAmt = dependent.Savings,
                    savingAmtSpecified = true,
                    stockBondAmt = dependent.Securities,
                    stockBondAmtSpecified = true,
                    vnpProcId = msg.ProcRequestState.VnpProcId,
                    vnpPtcpntId = msg.DependentRequestState.DependentParticipantId
                };

                if (dependent.IsPaidByDEA == true)
                {
                    VnpChildStudntDto.agencyPayingTuitnNm = dependent.AgencyName;
                    VnpChildStudntDto.govtPaidTuitnStartDtSpecified = true;
                    VnpChildStudntDto.govtPaidTuitnStartDt = dependent.PaidTuitionStartDate;
                }

                var request = new vnpChildStudentCreateRequest(VnpChildStudntDto);
                Condition.Requires(msg.AddDependentMaintenanceRequestState,
                "msg.AddDependentMaintenanceRequestState").IsNotNull();
                Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName,
                        "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
                Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService,
                    "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

                var service = BgsServiceFactory.GetVnpChildStudentService(msg.AddDependentMaintenanceRequestState.OrganizationName,
               msg.AddDependentMaintenanceRequestState.OrganizationService,
               msg.AddDependentMaintenanceRequestState.SystemUserId);

                Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                          msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

                var response = service.vnpChildStudentCreate(request);

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
            //Place Holder Service
        }
    }
}