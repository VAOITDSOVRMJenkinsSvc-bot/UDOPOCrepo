using System;
using System.ComponentModel.Composition;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.Messages;

namespace VRM.Integration.Servicebus.Egain
{
	[Export(typeof(IMessageHandler))]
	[ExportMetadata("MessageType", "Egain#AnonChatRequest")]
	public class AnonChatRequestHandler : RequestHandler
	{
		public AnonChatRequestHandler()
		{
		}

		public override void Handle(object message)
		{
			base.LogMessageReceipt(message);
			AnonChatRequest request = message as AnonChatRequest;
			AnonChatRequestProcessor processor = new AnonChatRequestProcessor();
			if (request != null)
			{
				processor.Execute(request);
			}
		}
	}
}