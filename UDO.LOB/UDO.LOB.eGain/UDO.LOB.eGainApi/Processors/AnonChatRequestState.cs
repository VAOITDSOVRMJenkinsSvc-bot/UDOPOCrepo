using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;
using System.Runtime.CompilerServices;
using VRM.Integration.Servicebus.Core;

namespace UDO.LOB.Egain.Processor
{
	public class AnonChatRequestState : PipeState, IAnonChatRequestState
	{
		public string CallAgentId
		{
			get;
			set;
		}

		public string ChatSessionId
		{
			get;
			set;
		}

		public string ChatSessionLog
		{
			get;
			set;
		}

		public string OrgName
		{
			get;
			set;
		}

		public IOrganizationService TargetOrganizationService
		{
			get;
			set;
		}

		public OrganizationServiceContext TargetOrganizationServiceContext
		{
			get;
			set;
		}

		public OrganizationWebProxyClient TargetOrganizationServiceProxy
		{
			get;
			set;
		}

		public AnonChatRequestState(string callAgentId, string chatSessionId, string chatSessionLog, string orgName)
		{
			this.CallAgentId = callAgentId;
			this.ChatSessionId = chatSessionId;
			this.ChatSessionLog = chatSessionLog;
			this.OrgName = orgName;
		}
	}
}