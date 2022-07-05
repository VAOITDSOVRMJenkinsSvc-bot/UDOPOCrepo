using System;
using Microsoft.Xrm.Sdk.Client;
using VIMT.AddressWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.Contact.Messages;
using System.Text;

namespace VRM.Integration.UDO.Contact.Processors
{
    public class UDOvalidateAddressRecordProcessor
    {
        // dependent to contact udo_contact_udo_dependant
        // bank to contact udo_contact_va_bankaccount_Veteranid
        // intent to file to contact udo_contact_va_intenttofile_VeteranId

        private bool _debug { get; set; }
        private const string method = "UDOvalidateAddressRecordProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOValidateAddressRequest request)  // The LoB instance of UDOValidateAddressRequest
        {
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            UDOValidateAddressResponse Response = new UDOValidateAddressResponse(); // The LoB instance of UDOValidateAddressResponse

            if (request == null)
            {
                Response.ExceptionMessage = "Called with no message";
                Response.ExceptionOccured = true;
                return Response;
            }

            progressString = "After CRM Connection";

            try
            {
                var request2 = new VIMTvaladdvalidateAddressRequest
                {
                    OrganizationName = request.OrganizationName,
                    UserId = request.UserId,
                    mcs_addressline1 = request.mcs_addressLine1,
                    mcs_addressline2 = request.mcs_addressLine2,
                    mcs_addressline3 = "",
                    mcs_addressline4 = "",
                    mcs_city = request.mcs_city,
                    mcs_country = "",
                    mcs_state = request.mcs_stateProvince,
                    mcs_postalcode = request.mcs_postalCode,
                    RelatedParentEntityName = "",
                    RelatedParentFieldName = "",
                    //RelatedParentId = placeHolderGuid,
                    LogSoap = false,
                    LogTiming = true,
                    Debug = true
                };

                if (request.LegacyServiceHeaderInfo != null)
                {
                    request2.LegacyServiceHeaderInfo = new VIMT.AddressWebService.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                // TODO(TN): Comment to remediate
                var EC_Response = request2.SendReceive<VIMTvaladdvalidateAddressResponse>(MessageProcessType.Local);
                var test = EC_Response.VIMTvaladdreturnInfo;

                progressString = "After VIMT EC Call";

                Response.ExceptionMessage = EC_Response.ExceptionMessage;
                Response.ExceptionOccured = EC_Response.ExceptionOccured;

                // CADD has 3 address sets, all not tied to the address entity
                if (EC_Response.VIMTvaladdreturnInfo != null)
                {
                    string address1Concat = string.Empty;
                    var ptcpntAddrsDTO = EC_Response.VIMTvaladdreturnInfo;

                    foreach (var item in ptcpntAddrsDTO) 
                    {
                        // address line 1
                        if (!string.IsNullOrEmpty(item.mcs_houseNumber) && !string.IsNullOrEmpty(item.mcs_streetName)) {
                            address1Concat = item.mcs_houseNumber.Trim() + " " + item.mcs_streetName.Trim();
                            if (!string.IsNullOrEmpty(item.mcs_streetSuffix))
                            {
                                address1Concat += " " + item.mcs_streetSuffix.Trim();    
                                Response.mcs_addressLine1 = address1Concat;
                            }
                        }
                        else if (!string.IsNullOrEmpty(item.mcs_addressLine1)) { Response.mcs_addressLine1 = item.mcs_addressLine1.Trim(); }

                        // Users have been entering apartment and such in address line 2
                        if (!string.IsNullOrEmpty(item.mcs_apartmentLabel) && !string.IsNullOrEmpty(item.mcs_apartmentNumber))
                        {
                            Response.mcs_addressLine2 = item.mcs_apartmentLabel.Trim() + " " + item.mcs_apartmentNumber.Trim();
                        }
                        else if (!string.IsNullOrEmpty(item.mcs_addressLine2)) { Response.mcs_addressLine2 = item.mcs_addressLine2.Trim(); }

                        if (!string.IsNullOrEmpty(item.mcs_addressLine3)) { Response.mcs_addressLine3 = item.mcs_addressLine3; }
                        if (!string.IsNullOrEmpty(item.mcs_addressLine4)) { Response.mcs_addressLine4 = item.mcs_addressLine4; }
                        if (!string.IsNullOrEmpty(item.mcs_city)) { Response.mcs_city = item.mcs_city; }
                        if (!string.IsNullOrEmpty(item.mcs_country)) { Response.mcs_country = item.mcs_country; }
                        if (!string.IsNullOrEmpty(item.mcs_firmName)) { Response.mcs_firmName = item.mcs_firmName; }
                        if (!string.IsNullOrEmpty(item.mcs_POBox)) { Response.mcs_POBox = item.mcs_POBox; }
                        if (!string.IsNullOrEmpty(item.mcs_postalCode)) { Response.mcs_postalCode = item.mcs_postalCode; }
                        if (!string.IsNullOrEmpty(item.mcs_privateMailbox)) { Response.mcs_privateMailbox = item.mcs_privateMailbox; }
                        if (!string.IsNullOrEmpty(item.mcs_stateProvince)) { Response.mcs_stateProvince = item.mcs_stateProvince; }
                        if (!string.IsNullOrEmpty(item.mcs_streetName)) { Response.mcs_streetName = item.mcs_streetName; }
                        if (!string.IsNullOrEmpty(item.mcs_streetSuffix)) { Response.mcs_streetSuffix = item.mcs_streetSuffix; }
                        if (!string.IsNullOrEmpty(item.mcs_USAltAddr)) { Response.mcs_USAltAddr = item.mcs_USAltAddr; }
                        if (!string.IsNullOrEmpty(item.mcs_USCountyName)) { Response.mcs_USCountyName = item.mcs_USCountyName; }
                        if (!string.IsNullOrEmpty(item.mcs_country)) { Response.mcs_country = item.mcs_country; }
                        if (!string.IsNullOrEmpty(item.mcs_status)) { Response.mcs_status = item.mcs_status; }
                        if (!string.IsNullOrEmpty(item.mcs_statusCode)) { Response.mcs_statusCode = item.mcs_statusCode; }
                        if (!string.IsNullOrEmpty(item.mcs_postalCode)) { Response.mcs_postalCode = item.mcs_postalCode; }
                        if (!string.IsNullOrEmpty(item.mcs_postalCodeAddOn)) { Response.mcs_postalCodeAddOn = item.mcs_postalCodeAddOn; }
                        if (!string.IsNullOrEmpty(item.mcs_postalCodeBase)) { Response.mcs_postalCodeBase = item.mcs_postalCodeBase; }
                        if (!string.IsNullOrEmpty(item.mcs_confidence)) { Response.mcs_confidence = item.mcs_confidence; }

                        if (!string.IsNullOrEmpty(item.mcs_addressBlock1)) { Response.mcs_addressBlock1 = item.mcs_addressBlock1; }
                        if (!string.IsNullOrEmpty(item.mcs_addressBlock2)) { Response.mcs_addressBlock2 = item.mcs_addressBlock2; }
                        if (!string.IsNullOrEmpty(item.mcs_addressBlock3)) { Response.mcs_addressBlock3 = item.mcs_addressBlock3; }
                        if (!string.IsNullOrEmpty(item.mcs_stateProvinceResult)) { Response.mcs_stateProvinceResult = item.mcs_stateProvinceResult; }

                        //mcs_country      "UNITED STATES OF AMERICA"  string


                    // *****************************************************************************************************

                        // what comes back from the EC call
                        //mcs_abbreviatedAliasResult      null        string
                        //mcs_additionalInputData            ""            string
                        //mcs_addressBlock1        "1800 JONATHAN WAY APT 1115"             string
                        //mcs_addressBlock2        "RESTON VA  20190-3696"            string
                        //mcs_addressBlock3        "UNITED STATES OF AMERICA"  string
                        //mcs_addressBlock4        null        string
                        //mcs_addressBlock5        null        string
                        //mcs_addressBlock6        null        string
                        //mcs_addressBlock7        null        string
                        //mcs_addressBlock8        null        string
                        //mcs_addressBlock9        null        string
                        //mcs_addressFormat       null        string
                        //mcs_addressLine1           "1800 JONATHAN WAY APT 1115"             string
                        //mcs_addressLine2           ""            string
                        //mcs_addressLine3           null        string
                        //mcs_addressLine4           null        string
                        //mcs_apartmentLabel     "APT"    string
                        //mcs_apartmentLabel2   null        string
                        //mcs_apartmentLabel2Result      null        string
                        //mcs_apartmentLabelInput          "APT"    string
                        //mcs_apartmentLabelResult        "S"          string
                        //mcs_apartmentNumber               "1115"   string
                        //mcs_apartmentNumber2             null        string
                        //mcs_apartmentNumber2Result                null        string
                        //mcs_apartmentNumberInput    "1115"   string
                        //mcs_apartmentNumberResult  "V"         string
                        //mcs_canadianDeliveryInstallationAreaName     null        string
                        //mcs_canadianDeliveryInstallationAreaNameInput          null        string
                        //mcs_canadianDeliveryInstallationQualifierName            null        string
                        //mcs_canadianDeliveryInstallationQualifierNameInput null        string
                        //mcs_canadianDeliveryInstallationType null        string
                        //mcs_canadianDeliveryInstallationTypeInput      null        string
                        //mcs_canadianSERPCode               null        string
                        //mcs_city              "RESTON"            string
                        //mcs_cityInput   "RESTON"            string
                        //mcs_cityResult "V"         string
                        //mcs_CMRA         "N"         string
                        //mcs_confidence               "87"       string
                        //mcs_couldNotValidate null        string
                        //mcs_country      "UNITED STATES OF AMERICA"  string
                        //mcs_countryInput           ""            string
                        //mcs_countryLevel           "A"         string
                        //mcs_countryResult         null        string
                        //mcs_DPV             "Y"          string
                        //mcs_DPVFootnote          "AABB" string
                        //mcs_DPVNoStat               null        string
                        //mcs_DPVVacant               null        string
                        //mcs_firmName null        string
                        //mcs_firmNameInput      ""            string
                        //mcs_firmNameResult    null        string
                        //mcs_houseNumber        "1800"   string
                        //mcs_houseNumberInput             "1800"   string
                        //mcs_houseNumberResult           "V"         string
                        //mcs_leadingDirectional                ""            string
                        //mcs_leadingDirectionalInput     null        string
                        //mcs_leadingDirectionalResult   null        string
                        //mcs_matchScore              "0"          string
                        //mcs_multimatchCount  null        string
                        //mcs_multipleMatches   null        string
                        //mcs_POBox        ""            string
                        //mcs_POBoxInput             null        string
                        //mcs_POBoxResult           null        string
                        //mcs_postalBarCode        "90"       string
                        //mcs_postalCode              "20190-3696"     string
                        //mcs_postalCodeAddOn                "3696"   string
                        //mcs_postalCodeBase     "20190"                string
                        //mcs_postalCodeInput   "09001"                string
                        //mcs_postalCodeResult "C"         string
                        //mcs_postalCodeSource                "FinanceNumber"           string
                        //mcs_postalCodeType    null        string
                        //mcs_preferredAliasResult           null        string
                        //mcs_privateMailbox      ""            string
                        //mcs_privateMailboxInput           null        string
                        //mcs_privateMailboxType            null        string
                        //mcs_privateMailboxTypeInput null        string
                        //mcs_processedBy            "USA"    string
                        //mcs_RDI               null        string
                        //mcs_recordType              "HighRise"          string
                        //mcs_recordTypeDefault               null        string
                        //mcs_RRHC           ""            string
                        //mcs_RRHCInput                null        string
                        //mcs_RRHCResult              null        string
                        //mcs_RRHCType null        string
                        //mcs_stateProvince         "VA"      string
                        //mcs_stateProvinceInput              "VA"      string
                        //mcs_stateProvinceResult            "V"         string
                        //mcs_status         null        string
                        //mcs_statusCode               null        string
                        //mcs_statusDescription  null        string
                        //mcs_streetName             "JONATHAN"     string
                        //mcs_streetNameAbbreviatedAliasResult            ""            string
                        //mcs_streetNameAliasType         null        string
                        //mcs_streetNameInput  "JONATHAN"     string
                        //mcs_streetNamePreferredAliasResult  "N"         string
                        //mcs_streetNameResult                "V"         string
                        //mcs_streetSuffix             "WAY"  string
                        //mcs_streetSuffixInput  null        string
                        //mcs_streetSuffixResult                "A"         string
                        //mcs_suiteLinkFidelity   ""            string
                        //mcs_suiteLinkMatchCode            ""            string
                        //mcs_suiteLinkReturnCode          ""            string
                        //mcs_trailingDirectional                ""            string
                        //mcs_trailingDirectionalInput     null        string
                        //mcs_trailingDirectionalResult   null        string
                        //mcs_USAltAddr                ""            string
                        //mcs_USBCCheckDigit     "5"          string
                        //mcs_USCarrierRouteCode           "C014"  string
                        //mcs_USCongressionalDistrict     "11"       string
                        //mcs_USCountyName     "FAIRFAX"           string
                        //mcs_USFinanceNumber               "514212"              string
                        //mcs_USFIPSCountyNumber        "059"     string
                        //mcs_usImDestinationDATF         null        string
                        //mcs_usImDestinationRaw           null        string
                        //mcs_usImGenericDATF null        string
                        //mcs_usImGenericRaw   null        string
                        //mcs_usImOriginDATF    null        string
                        //mcs_usImOriginRaw      null        string
                        //mcs_USLACS      "N"         string
                        //mcs_USLACSReturnCode              ""            string
                        //mcs_USLastLineNumber               "X27512"              string
                        //mcs_USLOTCode              "0042A"                string
                        //mcs_USLOTHex "DF"       string
                        //mcs_USLOTSequence    "T0"       string
                        //mcs_USUrbanName        ""            string
                        //mcs_USUrbanNameInput             ""            string
                        //mcs_USUrbanNameResult           null        string
                        //mcs_veriMoveDataBlock              null        string

                    }
                }

                progressString = "none";

            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOgetAddressRecordsProcessor Processor, Progess:" + progressString, ExecutionException);
                Response.ExceptionMessage = "Failed to Map EC data to LOB";
                Response.ExceptionOccured = true;
                return Response;
            }

            return Response;

        }
    }
}
