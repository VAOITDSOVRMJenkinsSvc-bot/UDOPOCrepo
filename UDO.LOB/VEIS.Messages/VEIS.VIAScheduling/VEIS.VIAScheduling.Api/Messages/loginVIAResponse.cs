using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VIAScheduling.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.VIAScheduling,loginVIA method, Response.
	/// Code Generated by IMS on: 1/24/2019 5:42:03 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISVIAScheLIloginVIAResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISVIAScheLIuserTO VEISVIAScheLIuserTOInfo { get; set; }
	}
	[DataContract]
	public class VEISVIAScheLIuserTO
	{
		[DataMember]
		public string mcs_id { get; set; }
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public string mcs_SSN { get; set; }
		[DataMember]
		public string mcs_DUZ { get; set; }
		[DataMember]
		public string mcs_siteId { get; set; }
		[DataMember]
		public string mcs_office { get; set; }
		[DataMember]
		public string mcs_phone { get; set; }
		[DataMember]
		public string mcs_pager { get; set; }
		[DataMember]
		public string mcs_service { get; set; }
		[DataMember]
		public string mcs_title { get; set; }
		[DataMember]
		public string mcs_orderRole { get; set; }
		[DataMember]
		public string mcs_userClass { get; set; }
		[DataMember]
		public string mcs_greeting { get; set; }
		[DataMember]
		public string mcs_siteMessage { get; set; }
		[DataMember]
		public string mcs_emailAddress { get; set; }
		[DataMember]
		public string mcs_username { get; set; }
		[DataMember]
		public string mcs_vistaDUZ { get; set; }
		[DataMember]
		public VEISVIAScheLIids VEISVIAScheLIidsInfo { get; set; }
		[DataMember]
		public VEISVIAScheLIdivisionsMultipleResponse[] VEISVIAScheLIdivisionsInfo { get; set; }
		[DataMember]
		public VEISVIAScheLIfault2 VEISVIAScheLIfault2Info { get; set; }
	}
	[DataContract]
	public class VEISVIAScheLIids
	{
		[DataMember]
		public bool mcs_textOnly { get; set; }
		[DataMember]
		public bool mcs_textOnlySpecified { get; set; }
		[DataMember]
		public Int32 mcs_count { get; set; }
		[DataMember]
		public bool mcs_countSpecified { get; set; }
		[DataMember]
		public VEISVIAScheLIresultsMultipleResponse[] VEISVIAScheLIresultsInfo { get; set; }
		[DataMember]
		public VEISVIAScheLIfault1 VEISVIAScheLIfault1Info { get; set; }
	}
	[DataContract]
	public class VEISVIAScheLIresultsMultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIAScheLItaggedResultsMultipleResponse[] VEISVIAScheLItaggedResultsInfo { get; set; }
		[DataMember]
		public VEISVIAScheLIfault VEISVIAScheLIfaultInfo { get; set; }
	}
	[DataContract]
	public class VEISVIAScheLItaggedResultsMultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIAScheLIfault
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIAScheLIfault1
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIAScheLIdivisionsMultipleResponse
	{
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public string mcs_siteId { get; set; }
	}
	[DataContract]
	public class VEISVIAScheLIfault2
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
}