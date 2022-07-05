using System;
using System.Linq;
using CuttingEdge.Conditions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Core;
using VRM.IntegrationServicebus.AddDependent.CrmModel;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class GetDependentMaintenanceRecord : FilterBase<IAddDependentMaintenanceRequestState>
    {
        public override void Execute(IAddDependentMaintenanceRequestState msg)
        {
			//CSDEv 
			//Logger.Instance.Debug("Calling GetDependentMaintenanceRecord");
			LogHelper.LogDebug(msg.OrganizationName, msg.Debug, msg.SystemUserId, $"{ this.GetType().FullName}", $"| VV Start {this.GetType().FullName}.Execute Calling GetDependentMaintenanceRecord");

			Condition.Requires(msg.Context, "msg.Context").IsNotNull();

			msg.DependentMaintenance = (from d in msg.Context.CreateQuery<crme_dependentmaintenance>()
                where d.Id == msg.DependentMaintenanceId
                select d).FirstOrDefault();
			
			Condition.Requires(msg.DependentMaintenance, "msg.DependentMaintenance").IsNotNull();
			LogHelper.LogDebug(msg.OrganizationName, msg.Debug, msg.SystemUserId, $"{ this.GetType().FullName}", $"| VV End {this.GetType().FullName}");
		}

		/// <summary>
		/// DO NOT USE; DELETE IF BREAKS
		/// 
		/// </summary>
		/// <returns></returns>
		public crme_dependentmaintenance TryGetDependentMaintenanceRecord(IOrganizationService conn, Guid dependentMaintenanceId)
		{
			//Set up our Target
			crme_dependentmaintenance targetDependentMaintenanceRecord = new crme_dependentmaintenance();
			targetDependentMaintenanceRecord.OwnerId = new EntityReference();

			//Setup Fetch Columns for Source
			var fetchColumns =
				"<attribute name='ownerid'/>" +
				"<attribute name='activityid'/>";

			//Setup Where clause for Source
			if (dependentMaintenanceId != Guid.Empty)
			{
				var fetch = "<fetch><entity name='crme_dependentmaintenance'>" + fetchColumns +
					 "<filter>" +
					 "<condition attribute='activityid' operator='eq' value='" +
					dependentMaintenanceId + "'/>" +
					 "</filter></entity></fetch>";

				var result = conn.RetrieveMultiple(new FetchExpression(fetch));

				//Map Source to Target
				if (result != null & result.Entities.Count > 0)
				{
					var sourceDependentMaintenanceRecord = result.Entities[0];
					targetDependentMaintenanceRecord.OwnerId = sourceDependentMaintenanceRecord.GetAttributeValue<EntityReference>("ownerid");
					targetDependentMaintenanceRecord.ActivityId = sourceDependentMaintenanceRecord.GetAttributeValue<Guid>("activityid");


					//Template for Strings 
					//firstName = sourceDependentMaintenanceRecord.GetAttributeValue<string>("firstname");

					//Template for Entity Refs 
					//ownerId = sourceDependentMaintenanceRecord.GetAttributeValue<EntityReference>("ownerid").Id;

					//Template for Optionsets 
					//if (sourceDependentMaintenanceRecord.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel") != null)
					//{
					//	sensitivityLevel = sourceDependentMaintenanceRecord.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel").Value.ToString();
					//}
				}


			}

			return targetDependentMaintenanceRecord;
		}
	}
}