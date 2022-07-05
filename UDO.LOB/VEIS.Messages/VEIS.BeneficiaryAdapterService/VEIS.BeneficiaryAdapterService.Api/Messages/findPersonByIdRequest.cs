using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.BeneficiaryAdapterService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.BeneficiaryAdapterService,findPersonById method, Request.
	/// Code Generated by IMS on: 1/11/2019 8:26:29 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISfperBIDfindPersonByIdRequest : VEISEcRequestBase
	{
		[DataMember]
		public Guid RelatedParentId { get; set; }
		[DataMember]
		public string RelatedParentEntityName { get; set; }
		[DataMember]
		public string RelatedParentFieldName { get; set; }
		[DataMember]
		public bool LogTiming { get; set; }
		[DataMember]
		public bool LogSoap { get; set; }
		[DataMember]
		public bool Debug { get; set; }
		[DataMember]
		public LegacyHeaderInfo LegacyServiceHeaderInfo { get; set; }
		[DataMember]
		public string mcs_transactionid { get; set; }
		[DataMember]
		public VEISfperBIDReqCPEBeneficiaryAdapterv2findPersonByIdRequest1 VEISfperBIDReqCPEBeneficiaryAdapterv2findPersonByIdRequest1Info { get; set; }
	}
	[DataContract]
	public class VEISfperBIDReqCPEBeneficiaryAdapterv2findPersonByIdRequest1
	{
		[DataMember]
		public VIMTfperBIDReqbeneficiaryIdentifier VIMTfperBIDReqbeneficiaryIdentifierInfo { get; set; }
	}
	[DataContract]
	public class VIMTfperBIDReqbeneficiaryIdentifier
	{
		[DataMember]
		public string mcs_bfn { get; set; }
		[DataMember]
		public string mcs_dfn { get; set; }
	}
}
