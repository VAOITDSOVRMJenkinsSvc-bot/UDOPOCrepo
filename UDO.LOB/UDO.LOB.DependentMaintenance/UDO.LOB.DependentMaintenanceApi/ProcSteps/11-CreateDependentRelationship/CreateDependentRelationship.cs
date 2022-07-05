using System;
using CuttingEdge.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntRlnshpServiceReference;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntRlnshpServiceReference;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class CreateDependentRelationship : FilterBase<IAddDependentRequestState>
    {
        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDev Rem 
			//Logger.Instance.Debug("Calling CreateDependentRelationship");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "CreateDependentRelationship.Execute", "Calling CreateDependentRelationship");

			DateTime methodStartTime, wsStartTime;
            string method = "CreateDependentRelationship", webService = "vnpPtcpntRlnshpCreate";

			
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

            var relationship = msg.DependentRequestState.DependentParticipant.DependentRelationship;

            Condition.Requires(relationship.FamilyRelationshipTypeName, "relationship.FamilyRelationshipTypeName")
                .IsNotNullOrEmpty();
            Condition.Requires(relationship.RelationshipTypeName, "relationship.RelationshipTypeName")
                .IsNotNullOrEmpty();

            if (relationship.FamilyRelationshipTypeName.Equals("Spouse"))
            {
                Condition.Requires(relationship.MarriageCityName, "relationship.MarriageCityName").IsNotNullOrEmpty();
                Condition.Requires(relationship.MarriageCountryName, "relationship.MarriageCountryName")
                    .IsNotNullOrEmpty();
                Condition.Requires(relationship.MarriageStateCode, "relationship.MarriageStateCode").IsNotNullOrEmpty();
            }
            var dependent = msg.DependentRequestState.DependentParticipant;
           
            var vnpPtcpntRlnshpDto = new VnpPtcpntRlnshpDTO
            {
                jrnDt = DateTimeExtensions.TodayNoon,  
                vnpProcId = msg.ProcRequestState.VnpProcId,
                vnpPtcpntIdA = msg.VeteranRequestState.VeteranParticipantId,
                vnpPtcpntIdB = msg.DependentRequestState.DependentParticipantId,
                childPrevlyMarriedInd = relationship.ChildPreviouslyMarried ? "Y" : "N",
                familyRlnshpTypeNm = relationship.FamilyRelationshipTypeName,
                livesWithRelatdPersonInd = relationship.LivesWithRelatedPerson ? "Y" : "N",
                marageCityNm = relationship.MarriageCityName,
                marageCntryNm = relationship.MarriageCountryName,
                marageStateCd = relationship.MarriageStateCode,
                ptcpntRlnshpTypeNm = relationship.RelationshipTypeName,
                mthlySupportFromVetAmt = Convert.ToDouble(relationship.MonthlyContributionToSpouseSupport),
                mthlySupportFromVetAmtSpecified = true
            };
            
            if (relationship.RelationshipTypeName == "Spouse")
            {
                vnpPtcpntRlnshpDto.beginDt = relationship.BeginDate.Date.AddHours(12);
                vnpPtcpntRlnshpDto.beginDtSpecified = true;

                
                //If we have an enddate add termination info.
                if (relationship.FamilyRelationshipTypeName.Equals("Ex-Spouse"))
                {
                    vnpPtcpntRlnshpDto.endDt = relationship.EndDate.Value.Date.AddHours(12);
                    vnpPtcpntRlnshpDto.endDtSpecified = true;
                    vnpPtcpntRlnshpDto.marageTrmntnCityNm = relationship.MarriageTerminationCityName;
                    vnpPtcpntRlnshpDto.marageTrmntnCntryNm = relationship.MarriageTerminationCountryName;
                    vnpPtcpntRlnshpDto.marageTrmntnStateCd = relationship.MarriageTerminationStateCode;
                    vnpPtcpntRlnshpDto.marageTrmntnTypeCd = relationship.MarriageTerminationTypeCode;
                }
            }

            var request = new vnpPtcpntRlnshpCreateRequest(vnpPtcpntRlnshpDto);

            Condition.Requires(msg.AddDependentMaintenanceRequestState, 
                "msg.AddDependentMaintenanceRequestState").IsNotNull();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName,
                    "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService, 
                "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

            var service = BgsServiceFactory.GetVnpPtcpntRlnshpService(msg.AddDependentMaintenanceRequestState.OrganizationName,
                msg.AddDependentMaintenanceRequestState.OrganizationService, 
                msg.AddDependentMaintenanceRequestState.SystemUserId);

			
			Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

			var response = service.vnpPtcpntRlnshpCreate(request);

			
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