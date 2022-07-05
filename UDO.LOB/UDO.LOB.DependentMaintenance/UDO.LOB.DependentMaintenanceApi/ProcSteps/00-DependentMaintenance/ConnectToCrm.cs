//CSDev
//using CRM007.CRM.SDK.Core;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Core;
using VRM.IntegrationServicebus.AddDependent.CrmModel;
using Logger = VRM.Integration.Servicebus.Core.Logger;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class ConnectToCrm : FilterBase<IAddDependentMaintenanceRequestState>
    {
		private IOrganizationService OrgServiceProxy;
		private OrganizationWebProxyClient webProxyClient;

		public override void Execute(IAddDependentMaintenanceRequestState msg)
        {
			/*
            //Get Connection for OrganizationName else use the Default Connection

            var crmConnectionParmsByOrganizationId = CrmConnectionConfiguration.Current
                .GetCrmConnectionParmsByOrganizationName(msg.OrganizationName);

            var defaultCrmConnectionParms = CrmConnectionConfiguration.Current
                .DefaultCrmConnectionParms;

            var connectionParms = crmConnectionParmsByOrganizationId ??
                                  defaultCrmConnectionParms;

            //Connect to CRM
            var connection = CrmConnection.Connect(connectionParms);

            //Enable Early Binding - Sepcifiy Assembly containing early bound types
            connection.EnableProxyTypes(typeof(crme_dependentmaintenance).Assembly);

            //Save Connection for use in DocGen
            msg.OrganizationService = connection;

            //Create a context - used for rest of orchestration
            var context = new OrganizationServiceContext(connection);

            msg.Context = context;
			*/

			#region connect to CRM
			try
			{
				//CSDEV START HERE ######
				//webProxyClient = CrmConnection.Connect<OrganizationWebProxyClient>();
				//webProxyClient = CrmConnection.Connect<OrganizationWebProxyClient>(true);
				//CSdev Forced Enable Proxy Types Here and it works 
				webProxyClient = ConnectionCache.GetProxy().OrganizationWebProxyClient;

				//CSDEv REm 
				//Logger.Instance.Debug("Calling ConnectToCrm");
				//CDev This Must Follow the .Connect Call in order to enable proxy types on the first non cached call to crm 
				LogHelper.LogDebug(msg.OrganizationName, msg.Debug, msg.SystemUserId, $"{ this.GetType().FullName}.Execute", $"| VVV Start {this.GetType().FullName}.Execute");


				//CrmConnection connection = new CrmConnection();
				if (msg.SystemUserId != Guid.Empty)
				{
					webProxyClient.CallerId = msg.SystemUserId;
				}
				OrgServiceProxy = webProxyClient as IOrganizationService;

				//SEt the Org Service 
				msg.OrganizationService = OrgServiceProxy;
				var context = new OrganizationServiceContext(OrgServiceProxy);

				msg.Context = context;

			}
			catch (Exception connectException)
			{
				LogHelper.LogError(msg.OrganizationName,msg.SystemUserId, "ConnectToCrm Proc Step, Connection Error", connectException.Message);
				msg.Exception = connectException;
			}
			#endregion


		}
	}
}
