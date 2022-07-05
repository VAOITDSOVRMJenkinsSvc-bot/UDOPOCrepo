using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using VRM.Integration.Servicebus.Egain.Messages.Messages;

namespace VRM.Integration.Servicebus.Egain.State
{
	public interface ICrmLaunchUrlRequestState
	{
		IOrganizationService CrmeOrganizationService
		{
			get;
			set;
		}

		OrganizationServiceContext CrmeOrganizationServiceContext
		{
			get;
			set;
		}

		OrganizationServiceProxy CrmeOrganizationServiceProxy
		{
			get;
			set;
		}

		string CrmLaunchUrl
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

		CrmLaunchUrlRequest Request
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
	}
}