using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.CpeForeignMedicalProgramAdapterServiceV3.CRMOL.Messages
{ 
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.CpeForeignMedicalProgramAdapterServiceV3.CRMOL,getFMPInfo method, Response.
	/// Code Generated by IMS on: 6/1/2018 11:14:57 AM
	/// Version: 2018.05.09.05
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISgFMPIgetFMPInfoResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISgFMPICPEForeignMedicalProgramAdapterv3getFMPInfoResponse VEISgFMPICPEForeignMedicalProgramAdapterv3getFMPInfoResponseInfo { get; set; }
		[DataMember]
		public bool ExceptionOccured { get; set; } 
	}
	[DataContract]
	public class VEISgFMPICPEForeignMedicalProgramAdapterv3getFMPInfoResponse
	{
		[DataMember]
		public VEISgFMPIcode VEISgFMPIcodeInfo { get; set; }
		[DataMember]
		public VEISgFMPIfmpBeneficiary VEISgFMPIfmpBeneficiaryInfo { get; set; }
		[DataMember]
		public VEISgFMPIfault VEISgFMPIfaultInfo { get; set; }
	}
	[DataContract]
	public enum VEISgFMPIcode
	{
		SUCCESS,
		ERROR,
	}
	[DataContract]
	public class VEISgFMPIfmpBeneficiary
	{
		[DataMember]
		public bool mcs_commentsFlag { get; set; }
		[DataMember]
		public bool mcs_commentsFlagSpecified { get; set; }
		[DataMember]
		public string mcs_fileLocation { get; set; }
		[DataMember]
		public string mcs_fileNumber { get; set; }
		[DataMember]
		public string mcs_gender { get; set; }
		[DataMember]
		public String[] mcs_phoneNumber { get; set; }
		[DataMember]
		public String[] mcs_vaRating { get; set; }
		[DataMember]
		public bool mcs_chapter31 { get; set; }
		[DataMember]
		public bool mcs_chapter31Specified { get; set; }
		[DataMember]
		public bool mcs_fiduciaryPOCSpecified { get; set; }
		[DataMember]
		public DateTime mcs_acceptLetterDate { get; set; }
		[DataMember]
		public bool mcs_acceptLetterDateSpecified { get; set; }
		[DataMember]
		public VEISgFMPIname VEISgFMPInameInfo { get; set; }
		[DataMember]
		public VEISgFMPIaddressMultipleResponse[] VEISgFMPIaddressInfo { get; set; }
		[DataMember]
		public VEISgFMPIstatusMultipleResponse[] VEISgFMPIstatusInfo { get; set; }
		[DataMember]
		public VEISgFMPIfiduciaryPOC VEISgFMPIfiduciaryPOCInfo { get; set; }
		[DataMember]
		public VEISgFMPIfmpConditionsMultipleResponse[] VEISgFMPIfmpConditionsInfo { get; set; }
	}
	[DataContract]
	public class VEISgFMPIname
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
	public class VEISgFMPIaddressMultipleResponse
	{
		[DataMember]
		public bool mcs_typeSpecified { get; set; }
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public string mcs_addressLine1 { get; set; }
		[DataMember]
		public string mcs_addressLine2 { get; set; }
		[DataMember]
		public string mcs_addressLine3 { get; set; }
		[DataMember]
		public string mcs_addressCityName { get; set; }
		[DataMember]
		public string mcs_addressUSStateCode { get; set; }
		[DataMember]
		public string mcs_addressZip5PostalCode { get; set; }
		[DataMember]
		public string mcs_country { get; set; }
		[DataMember]
		public string mcs_countryCode { get; set; }
		[DataMember]
		public string mcs_foreignAddressFlag { get; set; }
		[DataMember]
		public string mcs_phoneNumber { get; set; }
		[DataMember]
		public DateTime mcs_addressDocumentDate { get; set; }
		[DataMember]
		public bool mcs_addressDocumentDateSpecified { get; set; }
		[DataMember]
		public VEISgFMPItype VEISgFMPItypeInfo { get; set; }
		[DataMember]
		public VEISgFMPIaddressDocument VEISgFMPIaddressDocumentInfo { get; set; }
	}
	[DataContract]
	public enum VEISgFMPItype
	{
		SponsorRemitto,
		SponsorCorrespondence,
		SponsorPhysical,
		BeneficiaryRemitto,
		BeneficiaryCorrespondence,
		BeneficiaryGuardian,
		BeneficiaryPhysical,
		ProviderRemitto,
		ProviderBilling,
		ProviderProvider,
		ContactContact,
		FMPCorrespondence,
		FMPRemitto,
		FMPPhysical,
		DebtorCorrespondence,
		DebtorBilling,
		DebtorOrdering,
	}
	[DataContract]
	public class VEISgFMPIaddressDocument
	{
		[DataMember]
		public string mcs_documentTypeID { get; set; }
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public string mcs_abbreviation { get; set; }
	}
	[DataContract]
	public class VEISgFMPIstatusMultipleResponse
	{
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
	}
	[DataContract]
	public enum VEISgFMPIfiduciaryPOC
	{
		YES,
		NO,
		UNKNOWN,
	}
	[DataContract]
	public class VEISgFMPIfmpConditionsMultipleResponse
	{
		[DataMember]
		public string mcs_condition { get; set; }
		[DataMember]
		public DateTime mcs_effectiveDate { get; set; }
		[DataMember]
		public bool mcs_effectiveDateSpecified { get; set; }
		[DataMember]
		public DateTime mcs_endDate { get; set; }
		[DataMember]
		public bool mcs_endDateSpecified { get; set; }
		[DataMember]
		public string mcs_source { get; set; }
		[DataMember]
		public DateTime mcs_docDate { get; set; }
		[DataMember]
		public bool mcs_docDateSpecified { get; set; }
	}
	[DataContract]
	public class VEISgFMPIfault
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
		public VEISgFMPIessCode VEISgFMPIessCodeInfo { get; set; }
		[DataMember]
		public VEISgFMPInestedFault VEISgFMPInestedFaultInfo { get; set; }
	}
	[DataContract]
	public enum VEISgFMPIessCode
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
	public class VEISgFMPInestedFault
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
		public VEISgFMPIessCode1 VEISgFMPIessCode1Info { get; set; }
	}
	[DataContract]
	public enum VEISgFMPIessCode1
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
