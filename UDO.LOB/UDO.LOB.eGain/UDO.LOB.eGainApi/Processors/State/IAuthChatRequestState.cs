using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace VRM.Integration.Servicebus.Egain.State
{
	public interface IAuthChatRequestState
	{
		string CallAgentId
		{
			get;
			set;
		}

		string Category
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

		string Edipi
		{
			get;
			set;
		}

		bool IsTargetUserUdoChatUser
		{
			get;
			set;
		}

		string OrgName
		{
			get;
			set;
		}

		string ParticipantId
		{
			get;
			set;
		}

		string Resolution
		{
			get;
			set;
		}

		string Ssn
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

		string VsoOrgId
		{
			get;
			set;
		}
	}
}