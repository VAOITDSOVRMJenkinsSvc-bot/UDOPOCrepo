using CuttingEdge.Conditions;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Bgs.Services.VnpBnftClaimServiceReference;
using System;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services.VnpBnftClaimServiceReference;
using VRM.Integration.Servicebus.Bgs.Services;
using UDO.LOB.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class UpdateBenefitClaimInformation : FilterBase<IAddDependentRequestState>
    {
        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDEv REm 
			//Logger.Instance.Debug("Calling CreateVnpBenefitClaimInformation");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "UpdateBenefitClaimInformation.Execute", "Calling UpdateBenefitClaimInformation");

			DateTime methodStartTime, wsStartTime;
            string method = "UpdateBenefitClaimInformation", webService = "vnpBnftClaimUpdate";

			Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

			Condition.Requires(msg, "msg")
                .IsNotNull();

            Condition.Requires(msg.AddDependentMaintenanceRequestState,
                "msg.AddDependentMaintenanceRequestState").IsNotNull();

            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName,
               "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();

            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService,
                "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

            var vnpBnftClaimDto = CreateVnpBnftClaimDto(msg);

            var request = new vnpBnftClaimUpdateRequest(vnpBnftClaimDto);

            var service = BgsServiceFactory.GetVnpBnftClaimService(msg.AddDependentMaintenanceRequestState.OrganizationName,
                msg.AddDependentMaintenanceRequestState.OrganizationService,
                msg.AddDependentMaintenanceRequestState.SystemUserId);

			Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

			service.vnpBnftClaimUpdate(request);

			LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

			LogHelper.EndTiming(methodLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTime);

			var beautifiedRequest = JToken.Parse(JsonHelper.Serialize(request, request.GetType())).ToString(Formatting.Indented);

			LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
				, msg.AddDependentMaintenanceRequestState.SystemUserId, $"{ this.GetType().FullName}"
							, $"| RRR End w/ Request / No Response {this.GetType().FullName} \r\n\r\n " +
							$"|| RequestBody: { beautifiedRequest }");

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

            Condition.Requires(msg.VnpBenefitClaimId, "msg.VnpBenefitClaimId").IsGreaterThan(0);
            Condition.Requires(msg.BenefitClaimId, "msg.BenefitClaimId").IsGreaterThan(0);

            var vnpBnftClaimDto = new VnpBnftClaimDTO
            {
                vnpBnftClaimId = msg.VnpBenefitClaimId,
                bnftClaimId = msg.BenefitClaimId,
                bnftClaimIdSpecified = true,
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
                //  incorrect, should be using the next available increment value previously retrieved
                //  endPrdctTypeCd = BenefitClaimConstants.EndPrdctTypeCd,
                endPrdctTypeCd = msg.NextAvailBnftClaimIncrement,

                jrnDt = DateTimeExtensions.TodayNoon,
                jrnStatusTypeCd = BenefitClaimConstants.JrnUpdateStatusTypeCd,
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