using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.RetrieveAccountActivity.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.RetrieveAccountActivity,getRegistrationStatus method, Response.
	/// Code Generated by IMS on: 1/11/2019 10:57:17 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISgetRegStatgetRegistrationStatusResponse : VEISEcResponseBase
	{
		[DataMember]
		public bool mcs_isRegistered { get; set; }
		[DataMember]
		public Int32 mcs_credLevelAtLastLogin { get; set; }
		[DataMember]
		public Int32 mcs_status { get; set; }
		[DataMember]
		public String[] mcs_errorMessage { get; set; }
	}
}
