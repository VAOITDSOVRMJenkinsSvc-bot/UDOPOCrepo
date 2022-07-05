using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Runtime.CompilerServices;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.State;

namespace VRM.Integration.Servicebus.Egain
{
	public class AuthChatCoBrowseRequestState : PipeState, IAuthChatCoBrowseRequestState
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

		public string CoBrowseSessionId
		{
			get;
			set;
		}

		public string CoBrowseSessionLog
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

		public OrganizationServiceProxy TargetOrganizationServiceProxy
		{
			get;
			set;
		}

		public string VeteranId
		{
			get;
			set;
		}

		public string VsoOrgId
		{
			get;
			set;
		}

		public AuthChatCoBrowseRequestState(string callAgentId, string chatSessionId, string chatSessionLog, string coBrowseSessionId, string coBrowseSessionLog, string orgName, string veteranId, string vsoOrgId)
		{
			this.CallAgentId = callAgentId.Trim();
			this.ChatSessionId = chatSessionId.Trim();
			this.ChatSessionLog = chatSessionLog.Trim();
			this.CoBrowseSessionId = coBrowseSessionId.Trim();
			this.CoBrowseSessionLog = coBrowseSessionLog.Trim();
			this.OrgName = orgName.Trim();
			this.VeteranId = veteranId.Trim();
			this.VsoOrgId = vsoOrgId.Trim();
		}
	}
}