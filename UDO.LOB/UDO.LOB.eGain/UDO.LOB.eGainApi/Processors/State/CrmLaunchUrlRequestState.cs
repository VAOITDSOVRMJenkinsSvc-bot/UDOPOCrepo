using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Runtime.CompilerServices;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.Messages.Messages;

namespace VRM.Integration.Servicebus.Egain.State
{
	public class CrmLaunchUrlRequestState : PipeState, ICrmLaunchUrlRequestState
	{
		public IOrganizationService CrmeOrganizationService
		{
			get;
			set;
		}

		public OrganizationServiceContext CrmeOrganizationServiceContext
		{
			get;
			set;
		}

		public OrganizationServiceProxy CrmeOrganizationServiceProxy
		{
			get;
			set;
		}

		public string CrmLaunchUrl
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

		public CrmLaunchUrlRequest Request
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

		public CrmLaunchUrlRequestState(CrmLaunchUrlRequest request)
		{
			this.Request = request;
		}
	}
}