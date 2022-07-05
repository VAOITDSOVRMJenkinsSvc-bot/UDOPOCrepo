using CRM007.CRM.SDK.Core;
using CuttingEdge.Conditions;
using log4net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.CrmModel;
using VRM.Integration.Servicebus.Egain.State;

namespace VRM.Integration.Servicebus.Egain.Processor.AuthChatCoBrowseRequestStep.Steps
{
	internal class CreateSessionStep : FilterBase<IAuthChatCoBrowseRequestState>
	{
		public CreateSessionStep()
		{
		}

		public override void Execute(IAuthChatCoBrowseRequestState msg)
		{
			Logger.get_Instance().Debug("Calling CreateSessionStep");
			ValidatorExtensions.IsNotNull<IAuthChatCoBrowseRequestState>(Condition.Requires<IAuthChatCoBrowseRequestState>(msg, "msg"));
			CrmConnectionParms connectionParms = CrmConnectionConfiguration.get_Current().GetCrmConnectionParmsByName(msg.OrgName.ToUpper());
			if (connectionParms == null)
			{
				throw new Exception(string.Format("ConnectionParms was not found for OrganizationProxy {0}", msg.OrgName));
			}
			OrganizationServiceProxy connection = CrmConnection.Connect(connectionParms, typeof(crme_chatcobrowsesessionlog).Assembly);
			ChatXrmServiceContext xrm = new ChatXrmServiceContext(connection);
			WhoAmIRequest userRequest = new WhoAmIRequest();
			WhoAmIResponse response = (WhoAmIResponse)msg.TargetOrganizationServiceProxy.Execute(userRequest);
			crme_chatcobrowsesessionlog session = xrm.CreateQuery<crme_chatcobrowsesessionlog>().FirstOrDefault<crme_chatcobrowsesessionlog>((crme_chatcobrowsesessionlog x) => (x.crme_ChatSessionId == msg.ChatSessionId) && (x.crme_CoBrowseSessionId == msg.CoBrowseSessionId));
			if (session != null)
			{
				session.crme_CallAgentId = (msg.CallAgentId != null ? msg.CallAgentId : session.crme_CallAgentId);
				session.crme_ChatSessionId = (msg.ChatSessionId != null ? msg.ChatSessionId : session.crme_ChatSessionId);
				session.crme_CoBrowseSessionId = (msg.CoBrowseSessionId != null ? msg.CoBrowseSessionId : session.crme_CoBrowseSessionId);
				xrm.UpdateObject(session);
			}
			else
			{
				session = new crme_chatcobrowsesessionlog()
				{
					OwnerId = new EntityReference("systemuser", response.get_UserId()),
					crme_SessionType = new OptionSetValue()
				};
				session.crme_SessionType.set_Value(935950003);
				session.crme_CallAgentId = msg.CallAgentId;
				session.crme_ChatSessionId = msg.ChatSessionId;
				session.crme_CoBrowseSessionId = msg.CoBrowseSessionId;
				xrm.AddObject(session);
			}
			xrm.SaveChanges();

            
		}
	}
}