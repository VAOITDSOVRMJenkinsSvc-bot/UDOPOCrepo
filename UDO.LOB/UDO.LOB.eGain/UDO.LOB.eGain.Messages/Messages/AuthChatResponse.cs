using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.Egain.Messages.Messages
{
	[DataContract]
	public class AuthChatResponse : MessageBase
	{
		[DataMember]
		public int ErrorCode
		{
			get;
			set;
		}

		[DataMember]
		public string ErrorMessage
		{
			get;
			set;
		}

		[DataMember]
		public bool IsTargetUserUdoChatUser
		{
			get;
			set;
		}

		public AuthChatResponse()
		{
		}
	}
}