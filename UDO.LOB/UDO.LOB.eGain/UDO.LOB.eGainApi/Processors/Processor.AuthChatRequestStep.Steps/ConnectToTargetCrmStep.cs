using CRM007.CRM.SDK.Core;
using CuttingEdge.Conditions;
using log4net;
using System;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.State;

namespace VRM.Integration.Servicebus.Egain.Processor.AuthChatRequestStep.Steps
{
	public class ConnectToTargetCrmStep : FilterBase<IAuthChatRequestState>
	{
		public ConnectToTargetCrmStep()
		{
		}

		public override void Execute(IAuthChatRequestState msg)
		{
			try
			{
				try
				{
					Logger.get_Instance().Debug("AuthChatRequest::Calling ConnectToTargetCrmStep");
					Logger.get_Instance().Debug("Using enhanced log code");
					ILog instance = Logger.get_Instance();
					object[] callAgentId = new object[] { msg.CallAgentId, msg.Category, msg.ChatSessionId, msg.Edipi, msg.OrgName, msg.ParticipantId, msg.Resolution, msg.VsoOrgId };
					instance.Debug(string.Format("Received values -> CallAgentId: {0}; Category: {1}; ChatSessionId: {2}; Edipi: {3}; OrgName: {4}; ParticipantId: {5}; Resolution: {6}; VsoOrgId {7};", callAgentId));
					ValidatorExtensions.IsNotNull<IAuthChatRequestState>(Condition.Requires<IAuthChatRequestState>(msg, "msg"));
					if (string.IsNullOrEmpty(msg.OrgName))
					{
						throw new NullReferenceException("msg.OrgName cannot be null.");
					}
					if (CrmConnectionConfiguration.get_Current().GetCrmConnectionParmsByName(msg.OrgName) == null)
					{
						throw new NullReferenceException(string.Concat("Could not find the OrgName ", msg.OrgName, ". Ensure the OrgName exists in the VIMT.config CrmConnectionParms section."));
					}
				}
				catch (Exception exception)
				{
					Exception ex = exception;
					Logger.get_Instance().Debug(ex.ToString());
					throw ex;
				}
			}
			finally
			{
				Logger.get_Instance().Debug("AuthChatRequest::Exiting ConnectToTargetCrmStep");
			}
		}
	}
}