using System;
using System.ComponentModel.Composition;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.Messages.Messages;

namespace VRM.Integration.Servicebus.Egain
{
	[Export(typeof(IMessageHandler))]
	[ExportMetadata("MessageType", "Egain#AuthChatCoBrowseRequest")]
	public class AuthChatCoBrowseRequestHandler : RequestHandler
	{
		public AuthChatCoBrowseRequestHandler()
		{
		}

		public override void Handle(object message)
		{
			base.LogMessageReceipt(message);
			AuthChatCoBrowseRequest request = message as AuthChatCoBrowseRequest;
			AuthChatCoBrowseRequestProcessor processor = new AuthChatCoBrowseRequestProcessor();
			if (request != null)
			{
				processor.Execute(request);
			}
		}
	}
}