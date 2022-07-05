using System;

namespace VRM.Integration.Servicebus.Egain.Messages
{
	public static class MessageRegistry
	{
		public const string AnonChatRequest = "Egain#AnonChatRequest";

		public const string AuthChatRequest = "Egain#AuthChatRequest";

		public const string AuthChatResponse = "Egain#AuthChatResponse";

		public const string AuthCoBrowseRequest = "Egain#AuthCoBrowseRequest";

		public const string AuthChatCoBrowseRequest = "Egain#AuthChatCoBrowseRequest";

		public const string CrmLaunchUrlRequest = "Egain#CrmLaunchUrlRequest";

		public const string CrmLaunchUrlResponse = "Egain#CrmLaunchUrlResponse";
	}
}