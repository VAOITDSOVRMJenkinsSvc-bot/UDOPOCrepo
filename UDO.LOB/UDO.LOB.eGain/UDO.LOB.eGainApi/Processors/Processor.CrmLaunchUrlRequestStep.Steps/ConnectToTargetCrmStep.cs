using CRM007.CRM.SDK.Core;
using CuttingEdge.Conditions;
using log4net;
using System;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.Messages.Messages;
using VRM.Integration.Servicebus.Egain.State;

namespace VRM.Integration.Servicebus.Egain.Processor.CrmLaunchUrlRequestStep.Steps
{
	public class ConnectToTargetCrmStep : FilterBase<ICrmLaunchUrlRequestState>
	{
		public ConnectToTargetCrmStep()
		{
		}

		public override void Execute(ICrmLaunchUrlRequestState msg)
		{
			try
			{
				try
				{
					Logger.get_Instance().Debug("CrmLaunchUrlRequest::Calling ConnectToTargetCrmStep");
					Logger.get_Instance().Debug("Using enhanced log code");
					ILog instance = Logger.get_Instance();
					object[] chatSessionId = new object[] { msg.Request.ChatSessionId, msg.Request.Edipi, msg.Request.OrgName, msg.Request.ParticipantId };
					instance.Debug(string.Format("Received values -> ChatSessionId: {0}; Edipi: {1}; OrgName: {2}; ParticipantId: {3}", chatSessionId));
					ValidatorExtensions.IsNotNull<ICrmLaunchUrlRequestState>(Condition.Requires<ICrmLaunchUrlRequestState>(msg, "msg"));
					if (string.IsNullOrEmpty(msg.Request.OrgName))
					{
						throw new NullReferenceException("msg.OrgName cannot be null.");
					}
					if (CrmConnectionConfiguration.get_Current().GetCrmConnectionParmsByName(msg.Request.OrgName) == null)
					{
						throw new NullReferenceException(string.Concat("Could not find the OrgName ", msg.Request.OrgName, ". Ensure the OrgName exists in the VIMT.config CrmConnectionParms section."));
					}
				}
				catch (Exception exception)
				{
					Logger.get_Instance().Debug(exception.ToString());
				}
			}
			finally
			{
				Logger.get_Instance().Debug("CrmLaunchUrlRequest::Exiting ConnectToTargetCrmStep");
			}
		}
	}
}