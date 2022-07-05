using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace VRM.Integration.Servicebus.Egain.State
{
	public interface IAuthCoBrowseRequestState
	{
		string CallAgentId
		{
			get;
			set;
		}

		string CoBrowseSessionId
		{
			get;
			set;
		}

		string CoBrowseSessionLog
		{
			get;
			set;
		}

		string OrgName
		{
			get;
			set;
		}

		IOrganizationService TargetOrganizationService
		{
			get;
			set;
		}

		OrganizationServiceContext TargetOrganizationServiceContext
		{
			get;
			set;
		}

		OrganizationServiceProxy TargetOrganizationServiceProxy
		{
			get;
			set;
		}

		string VeteranId
		{
			get;
			set;
		}
	}
}