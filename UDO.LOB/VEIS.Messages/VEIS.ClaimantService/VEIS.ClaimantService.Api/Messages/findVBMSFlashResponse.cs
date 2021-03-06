using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.ClaimantService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.ClaimantService,findVBMSFlash method, Response.
	/// Code Generated by IMS on: 12/19/2018 9:54:57 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISfvbmsflfindVBMSFlashResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISfvbmsflreturn VEISfvbmsflreturnInfo { get; set; }
	}
	[DataContract]
	public class VEISfvbmsflreturn
	{
		[DataMember]
		public string mcs_assignedIndicator { get; set; }
		[DataMember]
		public string mcs_flashName { get; set; }
		[DataMember]
		public string mcs_flashType { get; set; }
	}
}
