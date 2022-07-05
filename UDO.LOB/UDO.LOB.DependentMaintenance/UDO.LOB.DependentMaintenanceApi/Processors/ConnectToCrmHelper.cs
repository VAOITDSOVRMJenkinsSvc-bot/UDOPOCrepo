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

// THIS MAY NOT BE NEEDED - UDO.LOB.Core has CrmConnector replacement and any helper should go under there


namespace UDO.LOB.DependentMaintenance
{
    public static class ConnectToCrmHelper
    {

		public static IOrganizationService ConnectToCrm(string organizationName)
        {
			/*
			//Get Connection for OrganizationName else use the Default Connection

			var crmConnectionParmsByOrganizationId = CrmConnectionConfiguration.Current
                .GetCrmConnectionParmsByOrganizationName(organizationName);

            var defaultCrmConnectionParms = CrmConnectionConfiguration.Current
                .DefaultCrmConnectionParms;

            var connectionParms = crmConnectionParmsByOrganizationId ??
                                  defaultCrmConnectionParms;

            //Connect to CRM
            var connection = CrmConnection.Connect(connectionParms);

            //Enable Early Binding - Sepcifiy Assembly containing early bound types
            connection.EnableProxyTypes(typeof(crme_dependentmaintenance).Assembly);

            //Save Connection for use in DocGen
            //return connection;
			*/

			#region connect to CRM

			try
			{
				//CSDev The code taken from th eold processors folder has this, do we need it? 
				//connection.EnableProxyTypes(typeof(VRM.Integration.Servicebus.BGS.CrmModel.SystemUser).Assembly);

				return ConnectionCache.GetProxy() as IOrganizationService;
				//CrmConnection connection = new CrmConnection();
				//if (msg.SystemUserId != Guid.Empty)
				//{
				//	webProxyClient.CallerId = msg.SystemUserId;
				//}

			}
			catch (Exception connectException)
			{
				LogHelper.LogError(organizationName, Guid.Empty, "ConnectToCrmHelper Proc Step, Connection Error", connectException.Message);
				throw connectException;
			}
			#endregion

		}
	}
}