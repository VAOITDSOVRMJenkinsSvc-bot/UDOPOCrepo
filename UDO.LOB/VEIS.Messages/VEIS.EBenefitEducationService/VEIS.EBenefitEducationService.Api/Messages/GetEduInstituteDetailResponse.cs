using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.EBenefitEducationService.Api.Messages
{ 
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.EBenefitEducationService.Api,GetEduInstituteDetail method, Response. 
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISgteduinstdtlGetEduInstituteDetailResponse : VEISEcResponseBase
    {
		[DataMember]
		public VEISgteduinstdtleduInstitute VEISgteduinstdtleduInstituteInfo { get; set; }
		[DataMember]
		public bool ExceptionOccured { get; set; }
		[DataMember]
		public string ExceptionMessage { get; set; }
	}
	[DataContract]
	public class VEISgteduinstdtleduInstitute
	{
		[DataMember]
		public Int64 mcs_participantID { get; set; }
		[DataMember]
		public string mcs_instituteName { get; set; }
		[DataMember]
		public string mcs_facilityCode { get; set; }
		[DataMember]
		public VEISgteduinstdtlStatusTypeCode mcs_status { get; set; }
		[DataMember]
		public DateTime mcs_statusDate { get; set; }
		[DataMember]
		public VEISgteduinstdtladdress VEISgteduinstdtladdressInfo { get; set; }
		[DataMember]
		public VEISgteduinstdtlcertifyingOfficalMultipleResponse[] VEISgteduinstdtlcertifyingOfficalInfo { get; set; }
		[DataMember]
		public VEISgteduinstdtlfacilityPhoneMultipleResponse[] VEISgteduinstdtlfacilityPhoneInfo { get; set; }
	}
	[DataContract]
	public class VEISgteduinstdtladdress
	{
		[DataMember]
		public string mcs_addressType { get; set; }
		[DataMember]
		public string mcs_addressLine1 { get; set; }
		[DataMember]
		public string mcs_addressLine2 { get; set; }
		[DataMember]
		public string mcs_addressLine3 { get; set; }
		[DataMember]
		public string mcs_city { get; set; }
		[DataMember]
		public string mcs_county { get; set; }
		[DataMember]
		public string mcs_state { get; set; }
		[DataMember]
		public string mcs_zipcode { get; set; }
		[DataMember]
		public string mcs_zipcodeSuffix { get; set; }
		[DataMember]
		public string mcs_emailAddress { get; set; }
		[DataMember]
		public VEISgteduinstdtlMilitaryPostOfficeTypeCode mcs_militaryPostOfficeTypeCode { get; set; }
		[DataMember]
		public bool mcs_militaryPostOfficeTypeCodeSpecified { get; set; }
		[DataMember]
		public VEISgteduinstdtlMilitaryPostalTypeCode mcs_militaryPostalTypeCode { get; set; }
		[DataMember]
		public bool mcs_militaryPostalTypeCodeSpecified { get; set; }
		[DataMember]
		public string mcs_foreignPostalCode { get; set; }
		[DataMember]
		public string mcs_province { get; set; }
		[DataMember]
		public string mcs_country { get; set; }
		[DataMember]
		public string mcs_effectiveDate { get; set; }
		[DataMember]
		public string mcs_endDate { get; set; }
	}
	[DataContract]
	public class VEISgteduinstdtlcertifyingOfficalMultipleResponse
	{
		[DataMember]
		public string mcs_nameTitle { get; set; }
		[DataMember]
		public VEISgteduinstdtlphone VEISgteduinstdtlphoneInfo { get; set; }
	}
	[DataContract]
	public class VEISgteduinstdtlphone
	{
		[DataMember]
		public VEISgteduinstdtlPhoneType mcs_phoneType { get; set; }
		[DataMember]
		public bool mcs_phoneTypeSpecified { get; set; }
		[DataMember]
		public string mcs_phoneNumber { get; set; }
		[DataMember]
		public string mcs_phoneExtension { get; set; }
	}
	[DataContract]
	public class VEISgteduinstdtlfacilityPhoneMultipleResponse
	{
        [DataMember]
		public VEISgteduinstdtlPhoneType mcs_phoneType { get; set; }
		[DataMember]
		public bool mcs_phoneTypeSpecified { get; set; }
		[DataMember]
		public string mcs_phoneNumber { get; set; }
		[DataMember]
		public string mcs_phoneExtension { get; set; }
	}
	
	public enum VEISgteduinstdtlPhoneType
	{
        None,
		Fax,
		Phone,
		International,
	}
	
	public enum VEISgteduinstdtlMilitaryPostOfficeTypeCode
	{
		APO,
		DPO,
		FPO,
	}
	
	public enum VEISgteduinstdtlMilitaryPostalTypeCode
	{
		AA,
		AE,
	}
	
	public enum VEISgteduinstdtlStatusTypeCode
	{
		Approved,
		Created,
		Withdrawn,
		Suspended,
	}
}
