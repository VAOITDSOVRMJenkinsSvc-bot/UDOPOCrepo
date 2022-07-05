using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.Crm.LOB.Messages.Address
{
    [DataContract]
    public class UDOValidateAddressResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public string mcs_abbreviatedAliasResult { get; set; }
        [DataMember]
        public string mcs_additionalInputData { get; set; }
        [DataMember]
        public string mcs_addressBlock1 { get; set; }
        [DataMember]
        public string mcs_addressBlock2 { get; set; }
        [DataMember]
        public string mcs_addressBlock3 { get; set; }
        [DataMember]
        public string mcs_addressBlock4 { get; set; }
        [DataMember]
        public string mcs_addressBlock5 { get; set; }
        [DataMember]
        public string mcs_addressBlock6 { get; set; }
        [DataMember]
        public string mcs_addressBlock7 { get; set; }
        [DataMember]
        public string mcs_addressBlock8 { get; set; }
        [DataMember]
        public string mcs_addressBlock9 { get; set; }
        [DataMember]
        public string mcs_addressFormat { get; set; }
        [DataMember]
        public string mcs_addressLine1 { get; set; }
        [DataMember]
        public string mcs_addressLine2 { get; set; }
        [DataMember]
        public string mcs_addressLine3 { get; set; }
        [DataMember]
        public string mcs_addressLine4 { get; set; }
        [DataMember]
        public string mcs_apartmentLabel { get; set; }
        [DataMember]
        public string mcs_apartmentLabel2 { get; set; }
        [DataMember]
        public string mcs_apartmentLabel2Result { get; set; }
        [DataMember]
        public string mcs_apartmentLabelInput { get; set; }
        [DataMember]
        public string mcs_apartmentLabelResult { get; set; }
        [DataMember]
        public string mcs_apartmentNumber { get; set; }
        [DataMember]
        public string mcs_apartmentNumber2 { get; set; }
        [DataMember]
        public string mcs_apartmentNumber2Result { get; set; }
        [DataMember]
        public string mcs_apartmentNumberInput { get; set; }
        [DataMember]
        public string mcs_apartmentNumberResult { get; set; }
        [DataMember]
        public string mcs_canadianDeliveryInstallationAreaName { get; set; }
        [DataMember]
        public string mcs_canadianDeliveryInstallationAreaNameInput { get; set; }
        [DataMember]
        public string mcs_canadianDeliveryInstallationQualifierName { get; set; }
        [DataMember]
        public string mcs_canadianDeliveryInstallationQualifierNameInput { get; set; }
        [DataMember]
        public string mcs_canadianDeliveryInstallationType { get; set; }
        [DataMember]
        public string mcs_canadianDeliveryInstallationTypeInput { get; set; }
        [DataMember]
        public string mcs_canadianSERPCode { get; set; }
        [DataMember]
        public string mcs_city { get; set; }
        [DataMember]
        public string mcs_cityInput { get; set; }
        [DataMember]
        public string mcs_cityResult { get; set; }
        [DataMember]
        public string mcs_CMRA { get; set; }
        [DataMember]
        public string mcs_confidence { get; set; }
        [DataMember]
        public string mcs_couldNotValidate { get; set; }
        [DataMember]
        public string mcs_country { get; set; }
        [DataMember]
        public string mcs_countryInput { get; set; }
        [DataMember]
        public string mcs_countryLevel { get; set; }
        [DataMember]
        public string mcs_countryResult { get; set; }
        [DataMember]
        public string mcs_DPV { get; set; }
        [DataMember]
        public string mcs_DPVFootnote { get; set; }
        [DataMember]
        public string mcs_DPVNoStat { get; set; }
        [DataMember]
        public string mcs_DPVVacant { get; set; }
        [DataMember]
        public string mcs_firmName { get; set; }
        [DataMember]
        public string mcs_firmNameInput { get; set; }
        [DataMember]
        public string mcs_firmNameResult { get; set; }
        [DataMember]
        public string mcs_houseNumber { get; set; }
        [DataMember]
        public string mcs_houseNumberInput { get; set; }
        [DataMember]
        public string mcs_houseNumberResult { get; set; }
        [DataMember]
        public string mcs_leadingDirectional { get; set; }
        [DataMember]
        public string mcs_leadingDirectionalInput { get; set; }
        [DataMember]
        public string mcs_leadingDirectionalResult { get; set; }
        [DataMember]
        public string mcs_matchScore { get; set; }
        [DataMember]
        public string mcs_multimatchCount { get; set; }
        [DataMember]
        public string mcs_multipleMatches { get; set; }
        [DataMember]
        public string mcs_POBox { get; set; }
        [DataMember]
        public string mcs_POBoxInput { get; set; }
        [DataMember]
        public string mcs_POBoxResult { get; set; }
        [DataMember]
        public string mcs_postalBarCode { get; set; }
        [DataMember]
        public string mcs_postalCode { get; set; }
        [DataMember]
        public string mcs_postalCodeAddOn { get; set; }
        [DataMember]
        public string mcs_postalCodeBase { get; set; }
        [DataMember]
        public string mcs_postalCodeInput { get; set; }
        [DataMember]
        public string mcs_postalCodeResult { get; set; }
        [DataMember]
        public string mcs_postalCodeSource { get; set; }
        [DataMember]
        public string mcs_postalCodeType { get; set; }
        [DataMember]
        public string mcs_preferredAliasResult { get; set; }
        [DataMember]
        public string mcs_privateMailbox { get; set; }
        [DataMember]
        public string mcs_privateMailboxInput { get; set; }
        [DataMember]
        public string mcs_privateMailboxType { get; set; }
        [DataMember]
        public string mcs_privateMailboxTypeInput { get; set; }
        [DataMember]
        public string mcs_processedBy { get; set; }
        [DataMember]
        public string mcs_RDI { get; set; }
        [DataMember]
        public string mcs_recordType { get; set; }
        [DataMember]
        public string mcs_recordTypeDefault { get; set; }
        [DataMember]
        public string mcs_RRHC { get; set; }
        [DataMember]
        public string mcs_RRHCInput { get; set; }
        [DataMember]
        public string mcs_RRHCResult { get; set; }
        [DataMember]
        public string mcs_RRHCType { get; set; }
        [DataMember]
        public string mcs_stateProvince { get; set; }
        [DataMember]
        public string mcs_stateProvinceInput { get; set; }
        [DataMember]
        public string mcs_stateProvinceResult { get; set; }
        [DataMember]
        public string mcs_status { get; set; }
        [DataMember]
        public string mcs_statusCode { get; set; }
        [DataMember]
        public string mcs_statusDescription { get; set; }
        [DataMember]
        public string mcs_streetName { get; set; }
        [DataMember]
        public string mcs_streetNameAbbreviatedAliasResult { get; set; }
        [DataMember]
        public string mcs_streetNameAliasType { get; set; }
        [DataMember]
        public string mcs_streetNameInput { get; set; }
        [DataMember]
        public string mcs_streetNamePreferredAliasResult { get; set; }
        [DataMember]
        public string mcs_streetNameResult { get; set; }
        [DataMember]
        public string mcs_streetSuffix { get; set; }
        [DataMember]
        public string mcs_streetSuffixInput { get; set; }
        [DataMember]
        public string mcs_streetSuffixResult { get; set; }
        [DataMember]
        public string mcs_suiteLinkFidelity { get; set; }
        [DataMember]
        public string mcs_suiteLinkMatchCode { get; set; }
        [DataMember]
        public string mcs_suiteLinkReturnCode { get; set; }
        [DataMember]
        public string mcs_trailingDirectional { get; set; }
        [DataMember]
        public string mcs_trailingDirectionalInput { get; set; }
        [DataMember]
        public string mcs_trailingDirectionalResult { get; set; }
        [DataMember]
        public string mcs_USAltAddr { get; set; }
        [DataMember]
        public string mcs_USBCCheckDigit { get; set; }
        [DataMember]
        public string mcs_USCarrierRouteCode { get; set; }
        [DataMember]
        public string mcs_USCongressionalDistrict { get; set; }
        [DataMember]
        public string mcs_USCountyName { get; set; }
        [DataMember]
        public string mcs_USFinanceNumber { get; set; }
        [DataMember]
        public string mcs_USFIPSCountyNumber { get; set; }
        [DataMember]
        public string mcs_usImDestinationDATF { get; set; }
        [DataMember]
        public string mcs_usImDestinationRaw { get; set; }
        [DataMember]
        public string mcs_usImGenericDATF { get; set; }
        [DataMember]
        public string mcs_usImGenericRaw { get; set; }
        [DataMember]
        public string mcs_usImOriginDATF { get; set; }
        [DataMember]
        public string mcs_usImOriginRaw { get; set; }
        [DataMember]
        public string mcs_USLACS { get; set; }
        [DataMember]
        public string mcs_USLACSReturnCode { get; set; }
        [DataMember]
        public string mcs_USLastLineNumber { get; set; }
        [DataMember]
        public string mcs_USLOTCode { get; set; }
        [DataMember]
        public string mcs_USLOTHex { get; set; }
        [DataMember]
        public string mcs_USLOTSequence { get; set; }
        [DataMember]
        public string mcs_USUrbanName { get; set; }
        [DataMember]
        public string mcs_USUrbanNameInput { get; set; }
        [DataMember]
        public string mcs_USUrbanNameResult { get; set; }
        [DataMember]
        public string mcs_veriMoveDataBlock { get; set; }
        [DataMember]
        public UDOgetAddressRecordsMultipleResponse[] UDOgetAddressRecordsInfo { get; set; }
    }

    [DataContract]
    public class UDOgetAddressRecordsMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateAddressRecordsId { get; set; }
    }
}
