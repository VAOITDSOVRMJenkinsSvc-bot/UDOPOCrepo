using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;

namespace UDO.LOB.Egain.Processor
{
	public interface IAnonChatRequestState
	{
		string CallAgentId
		{
			get;
			set;
		}

		string ChatSessionId
		{
			get;
			set;
		}

		string ChatSessionLog
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

        OrganizationWebProxyClient TargetOrganizationServiceProxy
		{
			get;
			set;
		}
	}
}