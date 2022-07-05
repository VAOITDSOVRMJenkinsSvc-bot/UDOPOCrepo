using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.EBenefitEducationService.Api.Messages
{ 
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.EBenefitEducationService.Api,GetEduRPOStates method, Response. 
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISgtedurpostGetEduRPOStatesResponse : VEISEcResponseBase
    {
		[DataMember]
		public VEISgtedurposteduRPOStatesMultipleResponse[] VEISgtedurposteduRPOStatesInfo { get; set; }
		[DataMember]
		public bool ExceptionOccured { get; set; }
		[DataMember]
		public string ExceptionMessage { get; set; }
	}
	[DataContract]
	public class VEISgtedurposteduRPOStatesMultipleResponse
	{
		[DataMember]
		public string mcs_roNumber { get; set; }
		[DataMember]
		public string mcs_roName { get; set; }
		[DataMember]
		public VEISgtedurposteduStateMultipleResponse[] VEISgtedurposteduStateInfo { get; set; }
	}
	[DataContract]
	public class VEISgtedurposteduStateMultipleResponse
	{
		[DataMember]
		public string mcs_state { get; set; }
		[DataMember]
		public string mcs_stateCodeORForeignCountry { get; set; }
		[DataMember]
		public string mcs_stateNumber { get; set; }
	}
}
