using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.Egain.Messages
{
	[DataContract]
	public class AuthChatCoBrowseRequest : MessageBase
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
		public string CoBrowseSessionId
		{
			get;
			set;
		}

		[DataMember]
		public string CoBrowseSessionLog
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
		public string VeteranId
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

		public AuthChatCoBrowseRequest()
		{
		}
	}
}