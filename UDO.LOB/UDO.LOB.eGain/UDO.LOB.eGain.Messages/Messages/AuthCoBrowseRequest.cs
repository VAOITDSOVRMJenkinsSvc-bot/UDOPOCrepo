using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.Egain.Messages.Messages
{
	[DataContract]
	public class AuthCoBrowseRequest : MessageBase
	{
		[DataMember]
		public string CallAgentId
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

		public AuthCoBrowseRequest()
		{
		}
	}
}