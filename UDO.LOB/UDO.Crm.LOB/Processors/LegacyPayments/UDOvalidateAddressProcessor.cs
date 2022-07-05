using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using VIMT.AddressWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.IntentToFile.Messages;
using Logger = VRM.Integration.Servicebus.Core.Logger;

namespace VRM.Integration.UDO.IntentToFile.Processors
{
    class UDOvalidateAddressProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOvalidateAddressProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOvalidateAddressRequest request)
        {
            //var request = message as createUDOLegacyPaymentDataRequest;
            UDOvalidateAddressResponse response = new UDOvalidateAddressResponse();
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            Logger.Instance.Info(string.Format("Message Id:{0}, Type={2}, Recieved diagnostics message: {1}",
            request.MessageId,
            request.MessageId,
            GetType().FullName));

            OrganizationServiceProxy OrgServiceProxy;

            #region connect to CRM
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOvalidateAddressProcessor Processor, Connection Error", connectException.Message);                
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                // prefix = payHistSSN_findPayHistoryBySSNRequest();
                var validateAddressRequest = new VIMTvaladdvalidateAddressRequest
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    LegacyServiceHeaderInfo = new VIMT.AddressWebService.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    mcs_addressline1 = request.AddressLine1,
                    mcs_addressline2 = request.AddressLine2,
                    mcs_addressline3 = request.AddressLine3,
                    mcs_addressline4 = request.AddressLine4,
                    mcs_city = request.City,
                    mcs_country = request.Country,
                    mcs_postalcode = request.Zip,
                    mcs_state = request.State
                };

                var validateAddressResponse = validateAddressRequest.SendReceive<VIMTvaladdvalidateAddressResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = validateAddressResponse.ExceptionMessage;
                response.ExceptionOccured = validateAddressResponse.ExceptionOccured;

                var requestCollection = new OrganizationRequestCollection();

                if (validateAddressResponse != null)
                {
                    if (validateAddressResponse.VIMTvaladdreturnInfo != null)
                    {
                        System.Collections.Generic.List<MultipleAddressResponse> VerifiedAddresses = new System.Collections.Generic.List<MultipleAddressResponse>();
                        foreach (var validatedAddress in validateAddressResponse.VIMTvaladdreturnInfo)
                        {
                            var thisAddress = new MultipleAddressResponse();
                            if (!!string.IsNullOrEmpty(validatedAddress.mcs_abbreviatedAliasResult))
                            {
                                thisAddress.abbreviatedAliasResult = validatedAddress.mcs_abbreviatedAliasResult;
                            }
                            if (!!string.IsNullOrEmpty(validatedAddress.mcs_additionalInputData))
                            {
                                thisAddress.additionalInputData = validatedAddress.mcs_additionalInputData;
                            }
                            if (!!string.IsNullOrEmpty(validatedAddress.mcs_addressBlock1))
                            {
                                thisAddress.addressBlock1 = validatedAddress.mcs_addressBlock1;
                            }
                            if (!!string.IsNullOrEmpty(validatedAddress.mcs_addressBlock2))
                            {
                                thisAddress.addressBlock2 = validatedAddress.mcs_addressBlock2;
                            }
                            if (!!string.IsNullOrEmpty(validatedAddress.mcs_addressBlock3))
                            {
                                thisAddress.addressBlock3 = validatedAddress.mcs_addressBlock3;
                            }
                            if (!!string.IsNullOrEmpty(validatedAddress.mcs_addressBlock4))
                            {
                                thisAddress.addressBlock4 = validatedAddress.mcs_addressBlock4;
                            }
                            if (!!string.IsNullOrEmpty(validatedAddress.mcs_addressBlock5))
                            {
                                thisAddress.addressBlock5 = validatedAddress.mcs_addressBlock5;
                            }
                            if (!!string.IsNullOrEmpty(validatedAddress.mcs_addressBlock6))
                            {
                                thisAddress.addressBlock6 = validatedAddress.mcs_addressBlock6;
                            }
                            if (!!string.IsNullOrEmpty(validatedAddress.mcs_addressBlock7))
                            {
                                thisAddress.addressBlock7 = validatedAddress.mcs_addressBlock7;
                            }
                            if (!!string.IsNullOrEmpty(validatedAddress.mcs_addressBlock8))
                            {
                                thisAddress.addressBlock8 = validatedAddress.mcs_addressBlock8;
                            }
                            if (!!string.IsNullOrEmpty(validatedAddress.mcs_addressBlock9))
                            {
                                thisAddress.addressBlock9 = validatedAddress.mcs_addressBlock9;
                            }
                            if (!!string.IsNullOrEmpty(validatedAddress.mcs_addressFormat))
                            {
                                thisAddress.addressFormat = validatedAddress.mcs_addressFormat;
                            }
                            if (!!string.IsNullOrEmpty(validatedAddress.mcs_addressLine1))
                            {
                                thisAddress.addressLine1 = validatedAddress.mcs_addressLine1;
                            }
                            if (!!string.IsNullOrEmpty(validatedAddress.mcs_addressLine2))
                            {
                                thisAddress.addressLine2 = validatedAddress.mcs_addressLine2;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_addressLine3))
                            {
                                thisAddress.addressLine3 = validatedAddress.mcs_addressLine3;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_addressLine4))
                            {
                                thisAddress.addressLine4 = validatedAddress.mcs_addressLine4;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_apartmentLabel))
                            {
                                thisAddress.apartmentLabel = validatedAddress.mcs_apartmentLabel;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_apartmentLabel2))
                            {
                                thisAddress.apartmentLabel2 = validatedAddress.mcs_apartmentLabel2;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_apartmentLabelInput))
                            {
                                thisAddress.apartmentLabelInput = validatedAddress.mcs_apartmentLabelInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_apartmentLabel2Result))
                            {
                                thisAddress.apartmentLabel2Result = validatedAddress.mcs_apartmentLabel2Result;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_apartmentLabelResult))
                            {
                                thisAddress.apartmentLabelResult = validatedAddress.mcs_apartmentLabelResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_apartmentNumber))
                            {
                                thisAddress.apartmentNumber = validatedAddress.mcs_apartmentNumber;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_apartmentNumber2))
                            {
                                thisAddress.apartmentNumber2 = validatedAddress.mcs_apartmentNumber2;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_apartmentNumber2Result))
                            {
                                thisAddress.apartmentNumber2Result = validatedAddress.mcs_apartmentNumber2Result;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_apartmentNumberInput))
                            {
                                thisAddress.apartmentNumberInput = validatedAddress.mcs_apartmentNumberInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_apartmentNumberResult))
                            {
                                thisAddress.apartmentNumberResult = validatedAddress.mcs_apartmentNumberResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_canadianDeliveryInstallationAreaName))
                            {
                                thisAddress.canadianDeliveryInstallationAreaName = validatedAddress.mcs_canadianDeliveryInstallationAreaName;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_canadianDeliveryInstallationAreaNameInput))
                            {
                                thisAddress.canadianDeliveryInstallationAreaNameInput = validatedAddress.mcs_canadianDeliveryInstallationAreaNameInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_canadianDeliveryInstallationQualifierName))
                            {
                                thisAddress.canadianDeliveryInstallationQualifierName = validatedAddress.mcs_canadianDeliveryInstallationQualifierName;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_canadianDeliveryInstallationQualifierNameInput))
                            {
                                thisAddress.canadianDeliveryInstallationQualifierNameInput = validatedAddress.mcs_canadianDeliveryInstallationQualifierNameInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_canadianDeliveryInstallationType))
                            {
                                thisAddress.canadianDeliveryInstallationType = validatedAddress.mcs_canadianDeliveryInstallationType;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_canadianDeliveryInstallationTypeInput))
                            {
                                thisAddress.canadianDeliveryInstallationTypeInput = validatedAddress.mcs_canadianDeliveryInstallationTypeInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_canadianSERPCode))
                            {
                                thisAddress.canadianDeliveryInstallationQualifierName = validatedAddress.mcs_canadianSERPCode;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_city))
                            {
                                thisAddress.city = validatedAddress.mcs_city;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_cityInput))
                            {
                                thisAddress.cityInput = validatedAddress.mcs_cityInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_cityResult))
                            {
                                thisAddress.cityResult = validatedAddress.mcs_cityResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_CMRA))
                            {
                                thisAddress.CMRA = validatedAddress.mcs_CMRA;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_confidence))
                            {
                                thisAddress.confidence = validatedAddress.mcs_confidence;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_couldNotValidate))
                            {
                                thisAddress.couldNotValidate = validatedAddress.mcs_couldNotValidate;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_country))
                            {
                                thisAddress.country = validatedAddress.mcs_country;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_countryInput))
                            {
                                thisAddress.countryInput = validatedAddress.mcs_countryInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_countryLevel))
                            {
                                thisAddress.countryLevel = validatedAddress.mcs_countryLevel;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_countryResult))
                            {
                                thisAddress.countryResult = validatedAddress.mcs_countryResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_DPV))
                            {
                                thisAddress.DPV = validatedAddress.mcs_DPV;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_DPVFootnote))
                            {
                                thisAddress.DPVFootnote = validatedAddress.mcs_DPVFootnote;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_DPVNoStat))
                            {
                                thisAddress.DPVNoStat = validatedAddress.mcs_DPVNoStat;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_DPVVacant))
                            {
                                thisAddress.DPVVacant = validatedAddress.mcs_DPVVacant;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_firmName))
                            {
                                thisAddress.firmName = validatedAddress.mcs_firmName;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_firmNameInput))
                            {
                                thisAddress.firmNameInput = validatedAddress.mcs_firmNameInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_firmNameResult))
                            {
                                thisAddress.firmNameResult = validatedAddress.mcs_firmNameResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_houseNumber))
                            {
                                thisAddress.houseNumber = validatedAddress.mcs_houseNumber;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_houseNumberInput))
                            {
                                thisAddress.houseNumberInput = validatedAddress.mcs_houseNumberInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_houseNumberResult))
                            {
                                thisAddress.houseNumberResult = validatedAddress.mcs_houseNumberResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_leadingDirectional))
                            {
                                thisAddress.leadingDirectional = validatedAddress.mcs_leadingDirectional;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_leadingDirectionalInput))
                            {
                                thisAddress.leadingDirectionalInput = validatedAddress.mcs_leadingDirectionalInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_leadingDirectionalResult))
                            {
                                thisAddress.leadingDirectionalResult = validatedAddress.mcs_leadingDirectionalResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_matchScore))
                            {
                                thisAddress.matchScore = validatedAddress.mcs_matchScore;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_multimatchCount))
                            {
                                thisAddress.multimatchCount = validatedAddress.mcs_multimatchCount;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_multipleMatches))
                            {
                                thisAddress.multipleMatches = validatedAddress.mcs_multipleMatches;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_POBox))
                            {
                                thisAddress.POBox = validatedAddress.mcs_POBox;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_POBoxInput))
                            {
                                thisAddress.POBoxInput = validatedAddress.mcs_POBoxInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_POBoxResult))
                            {
                                thisAddress.POBoxResult = validatedAddress.mcs_POBoxResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_postalBarCode))
                            {
                                thisAddress.postalBarCode = validatedAddress.mcs_postalBarCode;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_postalCode))
                            {
                                thisAddress.postalCode = validatedAddress.mcs_postalCode;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_postalCodeAddOn))
                            {
                                thisAddress.postalCodeAddOn = validatedAddress.mcs_postalCodeAddOn;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_postalCodeBase))
                            {
                                thisAddress.postalCodeBase = validatedAddress.mcs_postalCodeBase;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_postalCodeInput))
                            {
                                thisAddress.postalCodeInput = validatedAddress.mcs_postalCodeInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_postalCodeResult))
                            {
                                thisAddress.postalCodeResult = validatedAddress.mcs_postalCodeResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_postalCodeSource))
                            {
                                thisAddress.postalCodeSource = validatedAddress.mcs_postalCodeSource;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_postalCodeType))
                            {
                                thisAddress.postalCodeType = validatedAddress.mcs_postalCodeType;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_preferredAliasResult))
                            {
                                thisAddress.preferredAliasResult = validatedAddress.mcs_preferredAliasResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_privateMailbox))
                            {
                                thisAddress.privateMailbox = validatedAddress.mcs_privateMailbox;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_privateMailboxInput))
                            {
                                thisAddress.privateMailboxInput = validatedAddress.mcs_privateMailboxInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_privateMailboxType))
                            {
                                thisAddress.privateMailboxType = validatedAddress.mcs_privateMailboxType;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_privateMailboxTypeInput))
                            {
                                thisAddress.privateMailboxTypeInput = validatedAddress.mcs_privateMailboxTypeInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_processedBy))
                            {
                                thisAddress.processedBy = validatedAddress.mcs_processedBy;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_RDI))
                            {
                                thisAddress.RDI = validatedAddress.mcs_RDI;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_recordType))
                            {
                                thisAddress.recordType = validatedAddress.mcs_recordType;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_recordTypeDefault))
                            {
                                thisAddress.recordTypeDefault = validatedAddress.mcs_recordTypeDefault;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_RRHC))
                            {
                                thisAddress.RRHC = validatedAddress.mcs_RRHC;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_RRHCInput))
                            {
                                thisAddress.RRHCInput = validatedAddress.mcs_RRHCInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_RRHCResult))
                            {
                                thisAddress.RRHCResult = validatedAddress.mcs_RRHCResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_RRHCType))
                            {
                                thisAddress.RRHCType = validatedAddress.mcs_RRHCType;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_stateProvince))
                            {
                                thisAddress.stateProvince = validatedAddress.mcs_stateProvince;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_stateProvinceInput))
                            {
                                thisAddress.stateProvinceInput = validatedAddress.mcs_stateProvinceInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_stateProvinceResult))
                            {
                                thisAddress.stateProvinceResult = validatedAddress.mcs_stateProvinceResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_status))
                            {
                                thisAddress.status = validatedAddress.mcs_status;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_statusCode))
                            {
                                thisAddress.statusCode = validatedAddress.mcs_statusCode;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_statusDescription))
                            {
                                thisAddress.statusDescription = validatedAddress.mcs_statusDescription;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_streetName))
                            {
                                thisAddress.streetName = validatedAddress.mcs_streetName;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_streetNameAbbreviatedAliasResult))
                            {
                                thisAddress.streetNameAbbreviatedAliasResult = validatedAddress.mcs_streetNameAbbreviatedAliasResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_streetNameAliasType))
                            {
                                thisAddress.streetNameAliasType = validatedAddress.mcs_streetNameAliasType;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_streetNameInput))
                            {
                                thisAddress.streetNameInput = validatedAddress.mcs_streetNameInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_streetNamePreferredAliasResult))
                            {
                                thisAddress.streetNamePreferredAliasResult = validatedAddress.mcs_streetNamePreferredAliasResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_streetNameResult))
                            {
                                thisAddress.streetNameResult = validatedAddress.mcs_streetNameResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_streetSuffix))
                            {
                                thisAddress.streetSuffix = validatedAddress.mcs_streetSuffix;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_streetSuffixResult))
                            {
                                thisAddress.streetSuffixResult = validatedAddress.mcs_streetSuffixResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_streetSuffixInput))
                            {
                                thisAddress.streetSuffixResult = validatedAddress.mcs_streetSuffixInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_suiteLinkFidelity))
                            {
                                thisAddress.suiteLinkFidelity = validatedAddress.mcs_suiteLinkFidelity;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_suiteLinkMatchCode))
                            {
                                thisAddress.suiteLinkMatchCode = validatedAddress.mcs_suiteLinkMatchCode;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_suiteLinkReturnCode))
                            {
                                thisAddress.suiteLinkReturnCode = validatedAddress.mcs_suiteLinkReturnCode;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_trailingDirectional))
                            {
                                thisAddress.trailingDirectional = validatedAddress.mcs_trailingDirectional;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_trailingDirectionalInput))
                            {
                                thisAddress.trailingDirectionalInput = validatedAddress.mcs_trailingDirectionalInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_trailingDirectionalResult))
                            {
                                thisAddress.trailingDirectionalResult = validatedAddress.mcs_trailingDirectionalResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USAltAddr))
                            {
                                thisAddress.USAltAddr = validatedAddress.mcs_USAltAddr;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USBCCheckDigit))
                            {
                                thisAddress.USBCCheckDigit = validatedAddress.mcs_USBCCheckDigit;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USCarrierRouteCode))
                            {
                                thisAddress.USCarrierRouteCode = validatedAddress.mcs_USCarrierRouteCode;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USCongressionalDistrict))
                            {
                                thisAddress.USCongressionalDistrict = validatedAddress.mcs_USCongressionalDistrict;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USCountyName))
                            {
                                thisAddress.USCountyName = validatedAddress.mcs_USCountyName;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USFinanceNumber))
                            {
                                thisAddress.USFinanceNumber = validatedAddress.mcs_USFinanceNumber;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USFIPSCountyNumber))
                            {
                                thisAddress.USFIPSCountyNumber = validatedAddress.mcs_USFIPSCountyNumber;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_usImDestinationDATF))
                            {
                                thisAddress.usImDestinationDATF = validatedAddress.mcs_usImDestinationDATF;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_usImDestinationRaw))
                            {
                                thisAddress.usImDestinationRaw = validatedAddress.mcs_usImDestinationRaw;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_usImGenericDATF))
                            {
                                thisAddress.usImGenericDATF = validatedAddress.mcs_usImGenericDATF;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_usImGenericRaw))
                            {
                                thisAddress.usImGenericRaw = validatedAddress.mcs_usImGenericRaw;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_usImOriginDATF))
                            {
                                thisAddress.usImOriginDATF = validatedAddress.mcs_usImOriginDATF;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_usImOriginRaw))
                            {
                                thisAddress.usImOriginRaw = validatedAddress.mcs_usImOriginRaw;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USLACS))
                            {
                                thisAddress.USLACS = validatedAddress.mcs_USLACS;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USLACSReturnCode))
                            {
                                thisAddress.USLACSReturnCode = validatedAddress.mcs_USLACSReturnCode;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USLastLineNumber))
                            {
                                thisAddress.USLastLineNumber = validatedAddress.mcs_USLastLineNumber;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USLOTCode))
                            {
                                thisAddress.USLOTCode = validatedAddress.mcs_USLOTCode;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USLOTHex))
                            {
                                thisAddress.USLOTHex = validatedAddress.mcs_USLOTHex;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USLOTSequence))
                            {
                                thisAddress.USLOTSequence = validatedAddress.mcs_USLOTSequence;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USUrbanName))
                            {
                                thisAddress.USUrbanName = validatedAddress.mcs_USUrbanName;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USUrbanNameInput))
                            {
                                thisAddress.USUrbanNameInput = validatedAddress.mcs_USUrbanNameInput;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_USUrbanNameResult))
                            {
                                thisAddress.USUrbanNameResult = validatedAddress.mcs_USUrbanNameResult;
                            }
                            if (!string.IsNullOrEmpty(validatedAddress.mcs_veriMoveDataBlock))
                            {
                                thisAddress.veriMoveDataBlock = validatedAddress.mcs_veriMoveDataBlock;
                            }
                            VerifiedAddresses.Add(thisAddress);
                        }
                            response.Addresses = VerifiedAddresses.ToArray();
                        }
                    }
                

                //added to generated code
                if (request.udo_IntentToFileId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.RelatedParentId;
                    parent.LogicalName = request.RelatedParentEntityName;
                    parent["udo_legacypaymentdatacomplete"] = true;
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOvalidateAddress Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to process UDOValidateAddress data";
                response.ExceptionOccured = true;
                return response;
            }
        }
    }
}