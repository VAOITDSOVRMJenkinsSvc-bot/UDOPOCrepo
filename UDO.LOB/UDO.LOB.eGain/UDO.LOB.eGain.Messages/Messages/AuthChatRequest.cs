using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.Egain.Messages
{
	[DataContract]
	public class AuthChatRequest : MessageBase
	{
		[DataMember]
		public string CallAgentId
		{
			get;
			set;
		}

		[DataMember]
		public string Category
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
		public string Edipi
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

		[DataMember]
		public string OrgName
		{
			get;
			set;
		}

		[DataMember]
		public string ParticipantId
		{
			get;
			set;
		}

		[DataMember]
		public string Resolution
		{
			get;
			set;
		}

		[DataMember]
		public string Ssn
		{
			get;
			set;
		}

		[DataMember]
		public string VsoOrgId
		{
			get;
			set;
		}

		public AuthChatRequest()
		{
		}
	}
}