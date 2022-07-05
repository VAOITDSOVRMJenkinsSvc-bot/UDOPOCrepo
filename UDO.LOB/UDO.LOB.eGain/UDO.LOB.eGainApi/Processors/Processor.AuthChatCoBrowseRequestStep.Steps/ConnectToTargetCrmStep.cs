using CRM007.CRM.SDK.Core;
using CuttingEdge.Conditions;
using log4net;
using System;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.State;

namespace VRM.Integration.Servicebus.Egain.Processor.AuthChatCoBrowseRequestStep.Steps
{
	internal class ConnectToTargetCrmStep : FilterBase<IAuthChatCoBrowseRequestState>
	{
		public ConnectToTargetCrmStep()
		{
		}

		public override void Execute(IAuthChatCoBrowseRequestState msg)
		{
			Logger.get_Instance().Debug("AuthChatCoBrowseRequest::ConnectToTargetCrmStep");
			Logger.get_Instance().Debug("Using enhanced log code");
			ILog instance = Logger.get_Instance();
			object[] callAgentId = new object[] { msg.CallAgentId, msg.ChatSessionId, msg.CoBrowseSessionId, msg.OrgName, msg.VeteranId, msg.VsoOrgId, msg.ChatSessionLog, msg.CoBrowseSessionLog };
			instance.Debug(string.Format("Received values -> CallAgentId: {0}; ChatSessionId: {1}; CoBrowseSessionId: {2}; OrgName: {3};  VeteranId: {4}; VsoOrgId: {5}; ChatSessionLog: {6}; CoBrowseSessionLog: {7}", callAgentId));
			ValidatorExtensions.IsNotNull<IAuthChatCoBrowseRequestState>(Condition.Requires<IAuthChatCoBrowseRequestState>(msg, "msg"));
			if (string.IsNullOrEmpty(msg.OrgName))
			{
				throw new NullReferenceException("msg.OrgName cannot be null.");
			}
			if (CrmConnectionConfiguration.get_Current().GetCrmConnectionParmsByName(msg.OrgName) == null)
			{
				throw new NullReferenceException(string.Concat("Could not find the OrgName ", msg.OrgName, ". Ensure the OrgName exists in the VIMT.config CrmConnectionParms section."));
			}
		}
	}
}