using global::UDO.LOB.Contact.Messages;
using global::UDO.LOB.Core;
using global::UDO.LOB.Extensions.Logging;
using System;
using UDO.LOB.Extensions;
using VEIS.Messages.AddressWebService;

namespace UDO.LOB.Contact.Processors
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
            Response.MessageId = request.MessageId;

            if (request == null)
            {
                Response.ExceptionMessage = "Called with no message";
                Response.ExceptionOccured = true;
                return Response;
            }

            progressString = "After CRM Connection";

            try
            {
                var request2 = new VEISvaladdvalidateAddressRequest
                {
                    MessageId = request.MessageId,
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
                    LogSoap = false,
                    LogTiming = true,
                    Debug = true
                };

                if (request.LegacyServiceHeaderInfo != null)
                {
                    request2.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                // TODO(TN): Comment to remediate
                var EC_Response = WebApiUtility.SendReceive<VEISvaladdvalidateAddressResponse>(request2, WebApiType.VEIS);
                if (request.LogSoap || EC_Response.ExceptionOccurred)
                {
                    if (EC_Response.SerializedSOAPRequest != null || EC_Response.SerializedSOAPResponse != null)
                    {
                        var requestResponse = EC_Response.SerializedSOAPRequest + EC_Response.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISvaladdvalidateAddressRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VIMT EC Call";

                Response.ExceptionMessage = EC_Response.ExceptionMessage;
                Response.ExceptionOccured = EC_Response.ExceptionOccurred;

                // CADD has 3 address sets, all not tied to the address entity
                if (EC_Response.VEISvaladdreturnInfo != null)
                {
                    string address1Concat = string.Empty;
                    var ptcpntAddrsDTO = EC_Response.VEISvaladdreturnInfo;

                    foreach (var item in ptcpntAddrsDTO)
                    {
                        // address line 1
                        if (!string.IsNullOrEmpty(item.mcs_houseNumber) && !string.IsNullOrEmpty(item.mcs_streetName))
                        {
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
                    }
                }

                progressString = "none";

            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOgetAddressRecordsProcessor Processor, Progess:" + progressString, ExecutionException);
                Response.ExceptionMessage = "Failed to Map EC data to LOB";
                Response.ExceptionOccured = true;
                return Response;
            }

            return Response;
        }
    }
}
