using System;
using System.Runtime.Serialization;

namespace UDO.LOB.DependentMaintenance.Messages
{
    [DataContract]
    public class UdoEcGetEduInstitueResponse : UdoEcResponseBase
    {
        [DataMember]
        public UdoEcgteduinstdtleduInstitute VEISgteduinstdtleduInstituteInfo { get; set; }
        [DataMember]
        public bool ExceptionOccured { get; set; }
    }
    [DataContract]
    public class UdoEcgteduinstdtleduInstitute
    {
        [DataMember]
        public Int64 mcs_participantID { get; set; }
        [DataMember]
        public string mcs_instituteName { get; set; }
        [DataMember]
        public string mcs_facilityCode { get; set; }
        [DataMember]
        public UdoEcgteduinstdtlStatusTypeCode mcs_status { get; set; }
        [DataMember]
        public string mcs_statusDate { get; set; }
        [DataMember]
        public UdoEcgteduinstdtladdress VEISgteduinstdtladdressInfo { get; set; }
        [DataMember]
        public UdoEcgteduinstdtlcertifyingOfficalMultipleResponse[] VEISgteduinstdtlcertifyingOfficalInfo { get; set; }
        [DataMember]
        public UdoEcgteduinstdtlfacilityPhoneMultipleResponse[] VEISgteduinstdtlfacilityPhoneInfo { get; set; }
    }
    [DataContract]
    public class UdoEcgteduinstdtladdress
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
        public UdoEcgteduinstdtlMilitaryPostOfficeTypeCode mcs_militaryPostOfficeTypeCode { get; set; }
        [DataMember]
        public bool mcs_militaryPostOfficeTypeCodeSpecified { get; set; }
        [DataMember]
        public UdoEcgteduinstdtlMilitaryPostalTypeCode mcs_militaryPostalTypeCode { get; set; }
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
    public class UdoEcgteduinstdtlcertifyingOfficalMultipleResponse
    {
        [DataMember]
        public string mcs_nameTitle { get; set; }
        [DataMember]
        public UdoEcgteduinstdtlphone VEISgteduinstdtlphoneInfo { get; set; }
    }
    [DataContract]
    public class UdoEcgteduinstdtlphone
    {
        [DataMember]
        public UdoEcgteduinstdtlPhoneType mcs_phoneType { get; set; }
        [DataMember]
        public bool mcs_phoneTypeSpecified { get; set; }
        [DataMember]
        public string mcs_phoneNumber { get; set; }
        [DataMember]
        public string mcs_phoneExtension { get; set; }
    }
    [DataContract]
    public class UdoEcgteduinstdtlfacilityPhoneMultipleResponse
    {
        [DataMember]
        public UdoEcgteduinstdtlPhoneType mcs_phoneType { get; set; }
        [DataMember]
        public bool mcs_phoneTypeSpecified { get; set; }
        [DataMember]
        public string mcs_phoneNumber { get; set; }
        [DataMember]
        public string mcs_phoneExtension { get; set; }
    }

    public enum UdoEcgteduinstdtlPhoneType
    {
        None,
        Fax,
        Phone,
        International,
    }

    public enum UdoEcgteduinstdtlMilitaryPostOfficeTypeCode
    {
        APO,
        DPO,
        FPO,
    }

    public enum UdoEcgteduinstdtlMilitaryPostalTypeCode
    {
        AA,
        AE,
    }

    public enum UdoEcgteduinstdtlStatusTypeCode
    {
        Approved,
        Created,
        Withdrawn,
        Suspended,
    }
}
