using System;
using System.ComponentModel.Composition;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.Messages.Messages;

namespace VRM.Integration.Servicebus.Egain
{
	[Export(typeof(IMessageHandler))]
	[ExportMetadata("MessageType", "Egain#CrmLaunchUrlRequest")]
	public class CrmUrlRequestHandler : RequestResponseHandler
	{
		public CrmUrlRequestHandler()
		{
		}

		public override IMessageBase HandleRequestResponse(object message)
		{
			return (new CrmLaunchUrlRequestProcessor()).Execute((CrmLaunchUrlRequest)message);
		}
	}
}