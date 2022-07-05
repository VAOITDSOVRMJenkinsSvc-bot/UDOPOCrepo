using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Runtime.CompilerServices;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.State;

namespace VRM.Integration.Servicebus.Egain
{
	public class AuthChatRequestState : PipeState, IAuthChatRequestState
	{
		public string CallAgentId
		{
			get;
			set;
		}

		public string Category
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

		public string Edipi
		{
			get;
			set;
		}

		public bool IsTargetUserUdoChatUser
		{
			get;
			set;
		}

		public string OrgName
		{
			get;
			set;
		}

		public string ParticipantId
		{
			get;
			set;
		}

		public string Resolution
		{
			get;
			set;
		}

		public string Ssn
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

		public OrganizationServiceProxy TargetOrganizationServiceProxy
		{
			get;
			set;
		}

		public string VsoOrgId
		{
			get;
			set;
		}

		public AuthChatRequestState(string callAgentId, string category, string chatSessionId, string chatSessionLog, string edipi, string orgName, string resolution, string ssn, string vsoOrgId, string participantId)
		{
			this.CallAgentId = callAgentId;
			this.Category = category;
			this.ChatSessionId = chatSessionId;
			this.ChatSessionLog = chatSessionLog;
			this.Edipi = edipi;
			this.OrgName = orgName;
			this.Resolution = resolution;
			this.Ssn = ssn;
			this.VsoOrgId = vsoOrgId;
			this.ParticipantId = participantId;
		}
	}
}