using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Runtime.CompilerServices;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.State;

namespace VRM.Integration.Servicebus.Egain
{
	public class AuthCoBrowseRequestState : PipeState, IAuthCoBrowseRequestState
	{
		public string CallAgentId
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

		public AuthCoBrowseRequestState(string callAgentId, string coBrowseSessionId, string coBrowseSessionLog, string orgName, string veteranId)
		{
			this.CallAgentId = callAgentId;
			this.CoBrowseSessionId = coBrowseSessionId;
			this.CoBrowseSessionLog = coBrowseSessionLog;
			this.OrgName = orgName;
			this.VeteranId = veteranId;
		}
	}
}