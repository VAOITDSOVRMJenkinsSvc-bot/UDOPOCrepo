using System;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.Messages.Messages;
using VRM.Integration.Servicebus.Egain.Processor.AuthCoBrowseRequestStep.Steps;
using VRM.Integration.Servicebus.Egain.State;

namespace VRM.Integration.Servicebus.Egain
{
	public class AuthCoBrowseRequestProcessor
	{
		public AuthCoBrowseRequestProcessor()
		{
		}

		public void Execute(AuthCoBrowseRequest request)
		{
			AuthCoBrowseRequestState msg = new AuthCoBrowseRequestState(request.CallAgentId, request.CoBrowseSessionId, request.CoBrowseSessionLog, request.OrgName, request.VeteranId);
			try
			{
				(new Pipeline<IAuthCoBrowseRequestState>()).Register(new ConnectToTargetCrmStep()).Register(new CreateSessionStep()).Execute(msg);
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