//using CRM007.CRM.SDK.Core;
using CuttingEdge.Conditions;
//using log4net;
using Microsoft.Xrm.Sdk.Client;
using System;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain;
using UDO.LOB.Extensions;
using Microsoft.Xrm.Sdk.WebServiceClient;

namespace UDO.LOB.Egain.Processor
{
	public class ConnectToTargetCrmStep : FilterBase<IAnonChatRequestState>
	{
		public ConnectToTargetCrmStep()
		{
		}

		public override void Execute(IAnonChatRequestState msg)
		{
            //TODO: Remediate for Migration: Add Log Messages
			//Logger.get_Instance().Debug("AnonChatRequest::Calling ConnectToTargetCrmStep");
			//Logger.get_Instance().Debug("Using enhanced log code");
			//ILog instance = Logger.get_Instance();
			object[] callAgentId = new object[] { msg.CallAgentId, msg.ChatSessionId, msg.OrgName, msg.ChatSessionLog };
			// instance.Debug(string.Format("Received values -> CallAgentId: {0}; ChatSessionId: {1}; OrgName: {2};  ChatSessionLog: {3}", callAgentId));
			ValidatorExtensions.IsNotNull<IAnonChatRequestState>(Condition.Requires<IAnonChatRequestState>(msg, "msg"));
			if (string.IsNullOrEmpty(msg.OrgName))
			{
				throw new NullReferenceException("msg.OrgName cannot be null.");
			}

            //CrmConnectionParms parms = CrmConnectionConfiguration.get_Current().GetCrmConnectionParmsByName(msg.OrgName);
            //if (parms == null)
            //{
            //	throw new NullReferenceException(string.Concat("Could not find the OrgName ", msg.OrgName, ". Ensure the OrgName exists in the VIMT.config CrmConnectionParms section."));
            //}

            OrganizationWebProxyClient connection = CrmConnection.Connect<OrganizationWebProxyClient>();
			msg.TargetOrganizationService = connection;
			msg.TargetOrganizationServiceProxy = connection;
			msg.TargetOrganizationServiceContext = new OrganizationServiceContext(connection);
		}
	}
}