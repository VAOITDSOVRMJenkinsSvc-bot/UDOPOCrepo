using System;
using System.ComponentModel.Composition;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.Messages.Messages;

namespace VRM.Integration.Servicebus.Egain
{
	[Export(typeof(IMessageHandler))]
	[ExportMetadata("MessageType", "Egain#AuthChatRequest")]
	public class AuthChatRequestHandler : RequestResponseHandler
	{
		public AuthChatRequestHandler()
		{
		}

		public override IMessageBase HandleRequestResponse(object message)
		{
			base.LogMessageReceipt(message);
			return (new AuthChatRequestProcessor()).Execute(message as AuthChatRequest);
		}
	}
}