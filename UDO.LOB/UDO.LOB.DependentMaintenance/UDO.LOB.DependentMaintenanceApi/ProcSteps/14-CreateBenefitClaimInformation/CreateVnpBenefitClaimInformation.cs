using System;
using CuttingEdge.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Bgs.Services.VnpBnftClaimServiceReference;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Bgs.Services.VnpBnftClaimServiceReference;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreateVnpBenefitClaimInformation : FilterBase<IAddDependentRequestState>
    {
        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDEv REM 
			//Logger.Instance.Debug("Calling CreateVnpBenefitClaimInformation");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreateVnpBenefitClaimInformation.Execute", "Calling CreateVnpBenefitClaimInformation");

			DateTime methodStartTime, wsStartTime;
            string method = "CreateVnpBenefitClaimInformation", webService = "vnpBnftClaimCreate";

			Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);


			var vnpBnftClaimDto = CreateVnpBnftClaimDto(msg);

            var request = new vnpBnftClaimCreateRequest(vnpBnftClaimDto);

            Condition.Requires(msg.AddDependentMaintenanceRequestState,
                "msg.AddDependentMaintenanceRequestState").IsNotNull();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName,
                "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService,
                "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

            var service = BgsServiceFactory.GetVnpBnftClaimService(msg.AddDependentMaintenanceRequestState.OrganizationName,
                msg.AddDependentMaintenanceRequestState.OrganizationService,
                msg.AddDependentMaintenanceRequestState.SystemUserId);

			Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

			var response = service.vnpBnftClaimCreate(request);

			LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

			Condition.Ensures(response, "response").IsNotNull();

			Condition.Ensures(response.@return, "response.@return").IsNotNull();

			Condition.Ensures(response.@return.vnpBnftClaimId, "response.@return.vnpBnftClaimId").IsGreaterThan(0);

			msg.VnpBenefitClaimId = response.@return.vnpBnftClaimId;

			LogHelper.EndTiming(methodLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTime);

			var beautifiedRequest = JToken.Parse(JsonHelper.Serialize(request, request.GetType())).ToString(Formatting.Indented);
			var beautifiedResponse = JToken.Parse(JsonHelper.Serialize(response, response.GetType())).ToString(Formatting.Indented);

			LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
				, msg.AddDependentMaintenanceRequestState.SystemUserId, $"{ this.GetType().FullName}"
							, $"| RRR End w/ Request / Response {this.GetType().FullName} \r\n\r\n " +
							$"|| RequestBody: \r\n { beautifiedRequest } \r\n\r\n" +
							$"|| ResponseBody: \r\n { beautifiedResponse }");
		}

        private static VnpBnftClaimDTO CreateVnpBnftClaimDto(IAddDependentRequestState msg)
        {
            Condition.Requires(msg, "msg")
                .IsNotNull();

            Condition.Requires(msg.ProcRequestState, "msg.ProcRequestState").IsNotNull();
            Condition.Requires(msg.ProcRequestState.VnpProcId, "msg.ProcRequestState.VnpProcId").IsGreaterThan(0);
            Condition.Requires(msg.StationOfJurisdiction, "msg.StationOfJurisdiction").IsNotEmpty();
            Condition.Requires(msg.LocationId, "msg.LocationId").IsGreaterThan(0);
            Condition.Requires(BgsSecurityConfiguration.Current, "BgsSecurityConfiguration.Current").IsNotNull();
            Condition.Requires(BgsSecurityConfiguration.Current.ApplicationId,
                "BgsSecurityConfiguration.Current.ApplicationId").IsNotEmpty();
            Condition.Requires(BgsSecurityConfiguration.Current.UserName,
                "BgsSecurityConfiguration.Current.UserName").IsNotEmpty();

            var vnpBnftClaimDto = new VnpBnftClaimDTO
            {
                vnpProcID = msg.ProcRequestState.VnpProcId,
                bnftClaimTypeCd = BenefitClaimConstants.BnftClaimTypeCd,
                claimRcvdDt = msg.ProcRequestState.CreatedDate.Date.AddHours(12),
                claimJrsdtnLctnId = msg.LocationId,
                claimJrsdtnLctnIdSpecified = msg.LocationId > 0,
                jrnLctnId = msg.StationOfJurisdiction,
                jrnObjId = BgsSecurityConfiguration.Current.ApplicationId,
                intakeJrsdtnLctnId = msg.LocationId,
                intakeJrsdtnLctnIdSpecified = msg.LocationId > 0,

                //  DEFECT:  115035
                //  incorrect, should not be setting the default value
                //  endPrdctTypeCd = BenefitClaimConstants.EndPrdctTypeCd,

                jrnDt = DateTimeExtensions.TodayNoon,
                jrnStatusTypeCd = BenefitClaimConstants.JrnInsertStatusTypeCd,
                jrnUserId = msg.AddDependentMaintenanceRequestState.BgsHeaderInfo.LoginName,
                pgmTypeCd = BenefitClaimConstants.PgmTypeCd,
                statusTypeCd = BenefitClaimConstants.StatusTypeCd,
                svcTypeCd = BenefitClaimConstants.SvcTypeCd,
                ptcpntClmantId = msg.VeteranRequestState.VeteranParticipantId,
                ptcpntMailAddrsId = msg.ParticipantMailAddressId,
                ptcpntMailAddrsIdSpecified = msg.ParticipantMailAddressId > 0,
                vnpPtcpntVetId = msg.VeteranRequestState.VeteranParticipantId,
                vnpPtcpntVetIdSpecified = msg.VeteranRequestState.VeteranParticipantId > 0
            };

            return vnpBnftClaimDto;
        }
    }
}