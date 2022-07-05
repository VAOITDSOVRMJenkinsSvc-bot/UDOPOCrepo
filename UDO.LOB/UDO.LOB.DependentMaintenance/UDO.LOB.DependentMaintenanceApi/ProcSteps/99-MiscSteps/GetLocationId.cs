using System;
using System.Linq;
using CuttingEdge.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Bgs.Services.StandardDataWebServiceReference;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Bgs.Services.StandardDataWebServiceReference;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class GetLocationId : FilterBase<IAddDependentRequestState>
    {
        public override void Execute(IAddDependentRequestState msg)
        {
			//CSdev Rem 
			//Logger.Instance.Debug("Calling GetLocationId");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "GetLocationId.Execute", "Calling GetLocationId");

			DateTime methodStartTime, wsStartTime;
            string method = "GetLocationId", webService = "findRegionalOffices";

			
			Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

			Condition.Requires(msg, "msg")
				.IsNotNull();

			Condition.Requires(msg.AddDependentMaintenanceRequestState, "msg.AddDependentMaintenanceRequestState").IsNotNull();
			Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName, "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
			Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService, "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();
			Condition.Requires(msg.StationOfJurisdiction, "msg.StationOfJurisdiction").IsNotEmpty();

			var service = BgsServiceFactory.GetStandardDataWebService(msg.AddDependentMaintenanceRequestState.OrganizationName,
				msg.AddDependentMaintenanceRequestState.OrganizationService,
				msg.AddDependentMaintenanceRequestState.SystemUserId);

			var request = new findRegionalOfficesRequest();

			
			Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

			var response = service.findRegionalOffices(request);

			
			LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

			Condition.Ensures(response, "response").IsNotNull();

			var firstOrDefault = response.@return.FirstOrDefault(c => String.CompareOrdinal(c.stationNumber,
				msg.StationOfJurisdiction) == 0);

			Condition.Ensures(firstOrDefault, "firstOrDefault").IsNotNull();

			if (firstOrDefault != null)
				msg.LocationId = firstOrDefault.lctnId;

			
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