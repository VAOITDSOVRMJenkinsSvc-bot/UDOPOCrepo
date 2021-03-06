using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.CpeCreateCorrespondenceAdapterServiceV3.CRMOL.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.CpeCreateCorrespondenceAdapterServiceV3.CRMOL,createCorrespondencePickList method, Response.
	/// Code Generated by IMS on: 6/1/2018 10:18:29 AM
	/// Version: 2018.05.09.05
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEIScCPLcreateCorrespondencePickListResponse : VEISEcResponseBase
    {
		[DataMember]
		public VEIScCPLCPECreateCorrespondenceAdapterv3createCorrespondencePickListResponse VEIScCPLCPECreateCorrespondenceAdapterv3createCorrespondencePickListResponseInfo { get; set; }
		[DataMember]
		public bool ExceptionOccured { get; set; } 
	}
	[DataContract]
	public class VEIScCPLCPECreateCorrespondenceAdapterv3createCorrespondencePickListResponse
	{
		[DataMember]
		public VEIScCPLcode VEIScCPLcodeInfo { get; set; }
		[DataMember]
		public VEIScCPLformMultipleResponse[] VEIScCPLformInfo { get; set; }
		[DataMember]
		public VEIScCPLfault VEIScCPLfaultInfo { get; set; }
	}
	[DataContract]
	public enum VEIScCPLcode
	{
		SUCCESS,
		ERROR,
	}
	[DataContract]
	public class VEIScCPLformMultipleResponse
	{
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public string mcs_number { get; set; }
	}
	[DataContract]
	public class VEIScCPLfault
	{
		[DataMember]
		public bool mcs_essCodeSpecified { get; set; }
		[DataMember]
		public string mcs_essText { get; set; }
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public VEIScCPLessCode VEIScCPLessCodeInfo { get; set; }
		[DataMember]
		public VEIScCPLnestedFault VEIScCPLnestedFaultInfo { get; set; }
	}
	[DataContract]
	public enum VEIScCPLessCode
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
	public class VEIScCPLnestedFault
	{
		[DataMember]
		public bool mcs_essCodeSpecified { get; set; }
		[DataMember]
		public string mcs_essText { get; set; }
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public VEIScCPLessCode1 VEIScCPLessCode1Info { get; set; }
	}
	[DataContract]
	public enum VEIScCPLessCode1
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
