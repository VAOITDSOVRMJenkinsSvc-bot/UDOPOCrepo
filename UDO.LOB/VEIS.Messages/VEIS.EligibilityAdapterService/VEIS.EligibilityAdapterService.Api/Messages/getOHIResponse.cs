using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.EligibilityAdapterService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.EligibilityAdapterService,getOHI method, Response.
	/// Code Generated by IMS on: 1/11/2019 10:34:01 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISgOhigetOHIResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISgOhiGetOHIResponseMessage_v1 VEISgOhiGetOHIResponseMessage_v1Info { get; set; }
	}
	[DataContract]
	public class VEISgOhiGetOHIResponseMessage_v1
	{
		[DataMember]
		public VEISgOhiohisMultipleResponse[] VEISgOhiohisInfo { get; set; }
		[DataMember]
		public VEISgOhicommentsMultipleResponse[] VEISgOhicommentsInfo { get; set; }
		[DataMember]
		public VEISgOhifault VEISgOhifaultInfo { get; set; }
	}
	[DataContract]
	public class VEISgOhiohisMultipleResponse
	{
		[DataMember]
		public DateTime mcs_beginDate { get; set; }
		[DataMember]
		public bool mcs_beginDateSpecified { get; set; }
		[DataMember]
		public DateTime mcs_endDate { get; set; }
		[DataMember]
		public bool mcs_endDateSpecified { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public DateTime mcs_dateEdited { get; set; }
		[DataMember]
		public bool mcs_dateEditedSpecified { get; set; }
		[DataMember]
		public VEISgOhiuserName VEISgOhiuserNameInfo { get; set; }
		[DataMember]
		public VEISgOhiohiHistoryMultipleResponse[] VEISgOhiohiHistoryInfo { get; set; }
	}
	[DataContract]
	public class VEISgOhiuserName
	{
		[DataMember]
		public string mcs_prefix { get; set; }
		[DataMember]
		public string mcs_firstName { get; set; }
		[DataMember]
		public string mcs_middleName { get; set; }
		[DataMember]
		public string mcs_lastName { get; set; }
		[DataMember]
		public string mcs_suffix { get; set; }
	}
	[DataContract]
	public class VEISgOhiohiHistoryMultipleResponse
	{
		[DataMember]
		public DateTime mcs_beginDate { get; set; }
		[DataMember]
		public bool mcs_beginDateSpecified { get; set; }
		[DataMember]
		public DateTime mcs_endDate { get; set; }
		[DataMember]
		public bool mcs_endDateSpecified { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public DateTime mcs_dateEdited { get; set; }
		[DataMember]
		public bool mcs_dateEditedSpecified { get; set; }
		[DataMember]
		public VEISgOhiuserName1 VEISgOhiuserName1Info { get; set; }
	}
	[DataContract]
	public class VEISgOhiuserName1
	{
		[DataMember]
		public string mcs_prefix { get; set; }
		[DataMember]
		public string mcs_firstName { get; set; }
		[DataMember]
		public string mcs_middleName { get; set; }
		[DataMember]
		public string mcs_lastName { get; set; }
		[DataMember]
		public string mcs_suffix { get; set; }
	}
	[DataContract]
	public class VEISgOhicommentsMultipleResponse
	{
		[DataMember]
		public DateTime mcs_dateEntered { get; set; }
		[DataMember]
		public bool mcs_dateEnteredSpecified { get; set; }
		[DataMember]
		public string mcs_comment { get; set; }
		[DataMember]
		public VEISgOhienteredBy VEISgOhienteredByInfo { get; set; }
	}
	[DataContract]
	public class VEISgOhienteredBy
	{
		[DataMember]
		public string mcs_prefix { get; set; }
		[DataMember]
		public string mcs_firstName { get; set; }
		[DataMember]
		public string mcs_middleName { get; set; }
		[DataMember]
		public string mcs_lastName { get; set; }
		[DataMember]
		public string mcs_suffix { get; set; }
	}
	[DataContract]
	public class VEISgOhifault
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
		public VEISgOhiessCode VEISgOhiessCodeInfo { get; set; }
		[DataMember]
		public VEISgOhinestedFault VEISgOhinestedFaultInfo { get; set; }
	}
	[DataContract]
	public enum VEISgOhiessCode
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
	public class VEISgOhinestedFault
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
		public VEISgOhiessCode1 VEISgOhiessCode1Info { get; set; }
	}
	[DataContract]
	public enum VEISgOhiessCode1
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
