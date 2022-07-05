using System;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.Messages.Messages;
using VRM.Integration.Servicebus.Egain.Processor.AuthChatCoBrowseRequestStep.Steps;
using VRM.Integration.Servicebus.Egain.State;

namespace VRM.Integration.Servicebus.Egain
{
	public class AuthChatCoBrowseRequestProcessor
	{
		public AuthChatCoBrowseRequestProcessor()
		{
		}

		public void Execute(AuthChatCoBrowseRequest request)
		{
			AuthChatCoBrowseRequestState msg = new AuthChatCoBrowseRequestState(request.CallAgentId, request.ChatSessionId, request.ChatSessionLog, request.CoBrowseSessionId, request.CoBrowseSessionLog, request.OrgName, request.VeteranId, request.VsoOrgId);
			try
			{
				(new Pipeline<IAuthChatCoBrowseRequestState>()).Register(new ConnectToTargetCrmStep()).Register(new CreateSessionStep()).Execute(msg);
			}
			finally
			{
				if (msg != null)
				{
					((IDisposable)msg).Dispose();
				}
			}
		}
	}
}