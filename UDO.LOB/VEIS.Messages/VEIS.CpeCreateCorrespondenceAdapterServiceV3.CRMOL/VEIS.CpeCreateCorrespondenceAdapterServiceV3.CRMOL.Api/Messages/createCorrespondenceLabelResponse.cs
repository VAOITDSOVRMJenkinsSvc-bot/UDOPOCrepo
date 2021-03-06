using System; 
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.CpeCreateCorrespondenceAdapterServiceV3.CRMOL.Messages
{ 
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.CpeCreateCorrespondenceAdapterServiceV3.CRMOL,createCorrespondenceLabel method, Response.
	/// Code Generated by IMS on: 6/1/2018 10:04:14 AM
	/// Version: 2018.05.09.05
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEIScCLcreateCorrespondenceLabelResponse : VEISEcResponseBase
    {
		[DataMember]
		public VEIScCLCPECreateCorrespondenceAdapterv3createCorrespondenceLabelResponse VEIScCLCPECreateCorrespondenceAdapterv3createCorrespondenceLabelResponseInfo { get; set; }
		[DataMember]
		public bool ExceptionOccured { get; set; } 
	}
	[DataContract]
	public class VEIScCLCPECreateCorrespondenceAdapterv3createCorrespondenceLabelResponse
	{
		[DataMember]
		public VEIScCLcode VEIScCLcodeInfo { get; set; }
		[DataMember]
		public VEIScCLfault VEIScCLfaultInfo { get; set; }
	}
	[DataContract]
	public enum VEIScCLcode
	{
		SUCCESS,
		ERROR,
	}
	[DataContract]
	public class VEIScCLfault
	{
		[DataMember]
		public VEIScCLessCode VEIScCLessCodeInfo { get; set; }
		[DataMember]
		public VEIScCLnestedFault VEIScCLnestedFaultInfo { get; set; }
	}
	[DataContract]
	public enum VEIScCLessCode
	{
		SUCCESS,
		ERROR,
		WARNING,
		REFUSED,
		ACCEPT,
		REJECT,
		FAIL,
	}
	[DataContract]
	public class VEIScCLnestedFault
	{
		[DataMember]
		public VEIScCLessCode1 VEIScCLessCode1Info { get; set; }
	}
	[DataContract]
	public enum VEIScCLessCode1
	{
		SUCCESS,
		ERROR,
		WARNING,
		REFUSED,
		ACCEPT,
		REJECT,
		FAIL,
	}
}
