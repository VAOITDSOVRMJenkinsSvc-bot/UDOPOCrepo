using System;
using VRM.Integration.Servicebus.Core;
using UDO.LOB.Egain.Messages;
using UDO.LOB.Egain.Processor;

namespace UDO.LOB.Egain.Processor
{
	public class AnonChatRequestProcessor
	{
		public AnonChatRequestProcessor()
		{
		}

		public void Execute(AnonChatRequest request)
		{
			AnonChatRequestState msg = new AnonChatRequestState(request.CallAgentId, request.ChatSessionId, request.ChatSessionLog, request.OrgName);
			try
			{
				(new Pipeline<IAnonChatRequestState>()).Register(new ConnectToTargetCrmStep()).Register(new CreateSessionStep()).Execute(msg);
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