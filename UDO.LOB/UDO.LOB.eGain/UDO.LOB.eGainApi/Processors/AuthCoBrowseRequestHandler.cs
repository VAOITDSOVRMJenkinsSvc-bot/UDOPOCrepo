using System;
using System.ComponentModel.Composition;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.Messages.Messages;

namespace VRM.Integration.Servicebus.Egain
{
	[Export(typeof(IMessageHandler))]
	[ExportMetadata("MessageType", "Egain#AuthCoBrowseRequest")]
	public class AuthCoBrowseRequestHandler : RequestHandler
	{
		public AuthCoBrowseRequestHandler()
		{
		}

		public override void Handle(object message)
		{
			base.LogMessageReceipt(message);
			AuthCoBrowseRequest request = message as AuthCoBrowseRequest;
			AuthCoBrowseRequestProcessor processor = new AuthCoBrowseRequestProcessor();
			if (request != null)
			{
				processor.Execute(request);
			}
		}
	}
}