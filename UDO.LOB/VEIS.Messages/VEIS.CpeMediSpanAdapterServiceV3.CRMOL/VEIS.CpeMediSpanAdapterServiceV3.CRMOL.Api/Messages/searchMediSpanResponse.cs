using System; 
using System.Runtime.Serialization;
using VEIS.Core.Messages; 

namespace VEIS.CpeMediSpanAdapterServiceV3.CRMOL.Messages
{ 
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.CpeMediSpanAdapterServiceV3.CRMOL,searchMediSpan method, Response.
	/// Code Generated by IMS on: 5/29/2018 2:55:29 PM
	/// Version: 2018.05.09.05
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISsMSsearchMediSpanResponse : VEISEcResponseBase
    {
		[DataMember]
		public VEISsMSCPEMediSpanAdapterv3searchMediSpanResponse VEISsMSCPEMediSpanAdapterv3searchMediSpanResponseInfo { get; set; }
		[DataMember]
		public bool ExceptionOccured { get; set; } 
	}
	[DataContract]
	public class VEISsMSCPEMediSpanAdapterv3searchMediSpanResponse
	{
		[DataMember]
		public VEISsMScode VEISsMScodeInfo { get; set; }
		[DataMember]
		public VEISsMSmedispanDrugMultipleResponse[] VEISsMSmedispanDrugInfo { get; set; }
		[DataMember]
		public VEISsMSfault VEISsMSfaultInfo { get; set; }
	}
	[DataContract]
	public enum VEISsMScode
	{
		SUCCESS,
		ERROR,
	}
	[DataContract]
	public class VEISsMSmedispanDrugMultipleResponse
	{
		[DataMember]
		public bool mcs_validForDate { get; set; }
		[DataMember]
		public bool mcs_validForDateSpecified { get; set; }
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public string mcs_companyName { get; set; }
		[DataMember]
		public string mcs_ndcNumber { get; set; }
		[DataMember]
		public string mcs_gpiNumber { get; set; }
		[DataMember]
		public string mcs_packageSize { get; set; }
		[DataMember]
		public string mcs_dosage { get; set; }
		[DataMember]
		public string mcs_ien { get; set; }
	}
	[DataContract]
	public class VEISsMSfault
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
		public VEISsMSessCode VEISsMSessCodeInfo { get; set; }
		[DataMember]
		public VEISsMSnestedFault VEISsMSnestedFaultInfo { get; set; }
	}
	[DataContract]
	public enum VEISsMSessCode
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
	public class VEISsMSnestedFault
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
		public VEISsMSessCode1 VEISsMSessCode1Info { get; set; }
	}
	[DataContract]
	public enum VEISsMSessCode1
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
