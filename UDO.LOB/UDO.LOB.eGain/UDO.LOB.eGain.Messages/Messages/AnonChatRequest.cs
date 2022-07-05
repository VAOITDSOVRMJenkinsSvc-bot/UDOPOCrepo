using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.Egain.Messages
{
	[DataContract]
	public class AnonChatRequest : MessageBase
	{
		[DataMember]
		public string CallAgentId
		{
			get;
			set;
		}

		[DataMember]
		public string ChatSessionId
		{
			get;
			set;
		}

		[DataMember]
		public string ChatSessionLog
		{
			get;
			set;
		}

		[DataMember]
		public string OrgName
		{
			get;
			set;
		}

		public AnonChatRequest()
		{
		}
	}
}