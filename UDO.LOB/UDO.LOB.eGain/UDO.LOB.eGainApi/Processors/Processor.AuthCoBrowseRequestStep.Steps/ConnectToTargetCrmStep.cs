using CRM007.CRM.SDK.Core;
using CuttingEdge.Conditions;
using log4net;
using Microsoft.Xrm.Sdk.Client;
using System;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.State;

namespace VRM.Integration.Servicebus.Egain.Processor.AuthCoBrowseRequestStep.Steps
{
	public class ConnectToTargetCrmStep : FilterBase<IAuthCoBrowseRequestState>
	{
		public ConnectToTargetCrmStep()
		{
		}

		public override void Execute(IAuthCoBrowseRequestState msg)
		{
			Logger.get_Instance().Debug("AuthCoBrowseRequest::ConnectToTargetCrmStep");
			Logger.get_Instance().Debug("Using enhanced log code");
			ILog instance = Logger.get_Instance();
			object[] callAgentId = new object[] { msg.CallAgentId, msg.CoBrowseSessionId, msg.OrgName, msg.VeteranId, msg.CoBrowseSessionLog };
			instance.Debug(string.Format("Received values -> CallAgentId: {0}; CoBrowseSessionId: {1}; OrgName: {2};  VeteranId: {3}; CoBrowseSessionLog: {4}", callAgentId));
			ValidatorExtensions.IsNotNull<IAuthCoBrowseRequestState>(Condition.Requires<IAuthCoBrowseRequestState>(msg, "msg"));
			if (string.IsNullOrEmpty(msg.OrgName))
			{
				throw new NullReferenceException("msg.OrgName cannot be null.");
			}
			CrmConnectionParms parms = CrmConnectionConfiguration.get_Current().GetCrmConnectionParmsByName(msg.OrgName);
			if (parms == null)
			{
				throw new NullReferenceException(string.Concat("Could not find the OrgName ", msg.OrgName, ". Ensure the OrgName exists in the VIMT.config CrmConnectionParms section."));
			}
			OrganizationServiceProxy connection = CrmConnection.Connect(parms);
			msg.TargetOrganizationService = connection;
			msg.TargetOrganizationServiceProxy = connection;
			msg.TargetOrganizationServiceContext = new OrganizationServiceContext(connection);
		}
	}
}