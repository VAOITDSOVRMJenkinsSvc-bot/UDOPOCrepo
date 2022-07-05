
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Configuration;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Core;
using UDO.LOB.CADD.Messages;
using VEIS.Messages.AppealService;
using VEIS.Messages.BenefitClaimService;
using VEIS.Messages.ClaimantService;
using VEIS.Core.Messages;
using VEIS.Messages.DdeftWebService;
using VEIS.Messages.PaymentInformationService;
using VEIS.Messages.VeteranWebService;

//using Microsoft.Crm.Sdk.Messages;
//using CRM007.CRM.SDK.Core;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.CADD.Messages;
//using VRM.Integration.UDO.CADD.Processors;
//using VIMT.AppealService.Messages;
//using VIMT.ClaimantWebService.Messages;
//using VIMT.PaymentInformationService.Messages;
//using VIMT.VeteranWebService.Messages;
//using VIMT.BenefitClaimService.Messages;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//using VRM.Integration.UDO.Common;
//using VRM.Integration.UDO.Common.Messages;
//using Logger = VRM.Integration.Servicebus.Core.Logger;

namespace UDO.LOB.CADD.Processors
{
    class UDOupdateCADDAddressProcessor
    {
        private bool _debug { get; set; }

        private const string method = "UDOupdateCADDAddressProcessor";

        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOupdateCADDAddressRequest request)
        {
            return null;
            // NOTHING EDITED already commented out.
            //            //var request = message as InitiateCADDRequest;
            //            UDOupdateCADDAddressResponse response = new UDOupdateCADDAddressResponse();
            //            var progressString = "Top of Processor";

            //            if (request == null)
            //            {
            //                response.ExceptionMessage = "Called with no message";
            //                response.ExceptionOccured = true;
            //                return response;
            //            }

            //            Logger.Instance.Info(string.Format("Message Id:{0}, Type={2}, Recieved diagnostics message: {1}",
            //            request.MessageId,
            //            request.MessageId,
            //            GetType().FullName));

            //            OrganizationServiceProxy OrgServiceProxy;

            //            #region connect to CRM
            //            try
            //            {
            //                var CommonFunctions = new CRMCommonFunctions();

            //                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            //            }
            //            catch (Exception connectException)
            //            {
            //                LogHelper.LogError(request.OrganizationName, "mcs_updateCADD", request.UserId, "UDOupdateCADDAddressProcessor Processor, Progess:" + progressString, connectException.Message);
            //                response.ExceptionMessage = "Failed to get CRMConnection";
            //                response.ExceptionOccured = true;
            //                return response;
            //            }
            //            #endregion

            //            progressString = "After Connection";

            //            try
            //            {
            //                Entity thisNewEntity = new Entity();
            //                thisNewEntity.LogicalName = "va_bankaccount";
            //                var responseIds = new UDOupdateCADDAddressMultipleResponse();
            //                System.Collections.Generic.List<UDOupdateCADDAddressMultipleResponse> UDOcreateUDOCADDArray = new System.Collections.Generic.List<UDOupdateCADDAddressMultipleResponse>();

            //                #region updateAddressRequest

            //                var updateAddressLegacyServiceHeaderInfo = new VIMT.BenefitClaimService.Messages.HeaderInfo
            //                {
            //                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
            //                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
            //                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
            //                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
            //                };
            //                var updateAddressRequestFields = new VIMTupdBenClmAddr_updatebenefitclaimaddresscinput();


            //                var updateAddressRequest = new VIMTupdBenClmAddr_updateBenefitClaimAddressRequest
            //                {
            //                    LogTiming = request.LogTiming,
            //                    LogSoap = request.LogSoap,
            //                    Debug = request.Debug,
            //                    RelatedParentEntityName = request.RelatedParentEntityName,
            //                    RelatedParentFieldName = request.RelatedParentFieldName,
            //                    RelatedParentId = request.RelatedParentId,
            //                    UserId = request.UserId,
            //                    OrganizationName = request.OrganizationName,
            //                    LegacyServiceHeaderInfo = updateAddressLegacyServiceHeaderInfo

            //                    updatebenefitclaimaddresscinputInfo = updateAddressRequestFields
            //                };


            //                if (request.UDOupdateCADDMailingAddress != null || request.UDOupdateCADDPaymentAddress != null || request.UDOupdateCADDInfo != null)
            //                {
            //                    //Set FileNumber for Input
            //                    updateAddressRequestFields.mcs_fileNumber = request.fileNumber;

            //                    #region updateMailingAddress
            //                    if (request.UDOupdateCADDMailingAddress != null)
            //                    {
            //                        //These fields are to be updated regardless of null/blank status
            //                        updateAddressRequestFields.mcs_corpAddressLine2 = request.UDOupdateCADDMailingAddress.MailingAddressLine2;
            //                        updateAddressRequestFields.mcs_corpAddressLine3 = request.UDOupdateCADDMailingAddress.MailingAddressLine3;

            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDMailingAddress.MailingAddressLine1))
            //                        {
            //                            updateAddressRequestFields.mcs_corpAddressLine1 = request.UDOupdateCADDMailingAddress.MailingAddressLine1;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDMailingAddress.MailingAddressCity))
            //                        {
            //                            updateAddressRequestFields.mcs_corpCity = request.UDOupdateCADDMailingAddress.MailingAddressCity;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDMailingAddress.MailingAddressState))
            //                        {
            //                            updateAddressRequestFields.mcs_corpState = request.UDOupdateCADDMailingAddress.MailingAddressState;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDMailingAddress.MailingAddressZipCode))
            //                        {
            //                            updateAddressRequestFields.mcs_corpZipCode = request.UDOupdateCADDMailingAddress.MailingAddressZipCode;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDMailingAddress.MailingAddressCountry))
            //                        {
            //                            updateAddressRequestFields.mcs_corpCountry = request.UDOupdateCADDMailingAddress.MailingAddressCountry;
            //                        }
            //                    }
            //                    #endregion

            //                    #region updatePaymentAddress
            //                    if (request.UDOupdateCADDPaymentAddress != null)
            //                    {
            //                        //Fields will be updated regardless of null/blank status
            //                        updateAddressRequestFields.mcs_cpPaymentAddressLine2 = request.UDOupdateCADDPaymentAddress.PaymentAddressLine2;
            //                        updateAddressRequestFields.mcs_cpPaymentAddressLine3 = request.UDOupdateCADDPaymentAddress.PaymentAddressLine3;

            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDPaymentAddress.PaymentAddressLine1))
            //                        {
            //                            updateAddressRequestFields.mcs_cpPaymentAddressLine1 = request.UDOupdateCADDPaymentAddress.PaymentAddressLine1;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDPaymentAddress.PaymentAddressCity))
            //                        {
            //                            updateAddressRequestFields.mcs_cpPaymentCity = request.UDOupdateCADDPaymentAddress.PaymentAddressCity;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDPaymentAddress.PaymentAddressState))
            //                        {
            //                            updateAddressRequestFields.mcs_cpPaymentState = request.UDOupdateCADDPaymentAddress.PaymentAddressState;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDPaymentAddress.PaymentAddressZipCode))
            //                        {
            //                            updateAddressRequestFields.mcs_cpPaymentZipCode = request.UDOupdateCADDPaymentAddress.PaymentAddressZipCode;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDPaymentAddress.PaymentForeignZipCode))
            //                        {
            //                            updateAddressRequestFields.mcs_cpPaymentForeignZipCode = request.UDOupdateCADDPaymentAddress.PaymentForeignZipCode;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDPaymentAddress.PaymentPostalTypeCode))
            //                        {
            //                            updateAddressRequestFields.mcs_cpPaymentPostalTypeCode = request.UDOupdateCADDPaymentAddress.PaymentPostalTypeCode;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDPaymentAddress.PaymentPoTypeCode))
            //                        {
            //                            updateAddressRequestFields.mcs_cpPaymentPoTypeCode = request.UDOupdateCADDPaymentAddress.PaymentPoTypeCode;
            //                        }
            //                    }
            //                    #endregion

            //                    #region updateInfo
            //                    if (request.UDOupdateCADDInfo != null)
            //                    {
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.Phone1))
            //                        {
            //                            updateAddressRequestFields.mcs_corpPhoneNumber1 = request.UDOupdateCADDInfo.Phone1;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.Phone2))
            //                        {
            //                            updateAddressRequestFields.mcs_corpPhoneNumber2 = request.UDOupdateCADDInfo.Phone2;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.Phone1Type))
            //                        {
            //                            updateAddressRequestFields.mcs_corpPhoneTypeName1 = request.UDOupdateCADDInfo.Phone1Type;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.Phone2Type))
            //                        {
            //                            updateAddressRequestFields.mcs_corpPhoneTypeName2 = request.UDOupdateCADDInfo.Phone2Type;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.AreaNumber1))
            //                        {
            //                            updateAddressRequestFields.mcs_corpAreaNumber1 = request.UDOupdateCADDInfo.AreaNumber1;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.AreaNumber2))
            //                        {
            //                            updateAddressRequestFields.mcs_corpAreaNumber2 = request.UDOupdateCADDInfo.AreaNumber2;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.awardTypeCd))
            //                        {
            //                            updateAddressRequestFields.mcs_awardTypeCd = request.UDOupdateCADDInfo.awardTypeCd;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.EFTAccountNumber))
            //                        {
            //                            updateAddressRequestFields.mcs_eftAccountNumber = request.UDOupdateCADDInfo.EFTAccountNumber;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.EFTAccountType))
            //                        {
            //                            updateAddressRequestFields.mcs_eftAccountType = request.UDOupdateCADDInfo.EFTAccountType;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.EFTRoutingNumber))
            //                        {
            //                            updateAddressRequestFields.mcs_eftRoutingNumber = request.UDOupdateCADDInfo.EFTRoutingNumber;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.emailAddress))
            //                        {
            //                            updateAddressRequestFields.mcs_emailAddress = request.UDOupdateCADDInfo.emailAddress;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.MilitaryPostalCode))
            //                        {
            //                            updateAddressRequestFields.mcs_mltyPostalTypeCd = request.UDOupdateCADDInfo.MilitaryPostalCode;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.MilitaryPostOfficeTypeCode))
            //                        {
            //                            updateAddressRequestFields.mcs_mltyPostOfficeTypeCd = request.UDOupdateCADDInfo.MilitaryPostOfficeTypeCode;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.PayeeFirstName))
            //                        {
            //                            updateAddressRequestFields.mcs_payeeFirstName = request.UDOupdateCADDInfo.PayeeFirstName;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.PayeeLastName))
            //                        {
            //                            updateAddressRequestFields.mcs_payeeLastName = request.UDOupdateCADDInfo.PayeeLastName;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.PayeeMiddleName))
            //                        {
            //                            updateAddressRequestFields.mcs_payeeMiddleName = request.UDOupdateCADDInfo.PayeeMiddleName;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.PayeeSuffix))
            //                        {
            //                            updateAddressRequestFields.mcs_payeeSuffixName = request.UDOupdateCADDInfo.PayeeSuffix;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.PayeeCode))
            //                        {
            //                            updateAddressRequestFields.mcs_payeeCode = request.PayeeCode;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.ProvinceName))
            //                        {
            //                            updateAddressRequestFields.mcs_provinceName = request.UDOupdateCADDInfo.ProvinceName;
            //                        }
            //                        if (!string.IsNullOrEmpty(request.UDOupdateCADDInfo.TerritoryName))
            //                        {
            //                            updateAddressRequestFields.mcs_territoryName = request.UDOupdateCADDInfo.TerritoryName;
            //                        }
            //                    }
            //                    #endregion
            // (TN): Orginal code had commented out
            //                    var UDOupdateCADDAddressRequest = updateAddressRequest.SendReceive<VIMTupdBenClmAddr_updateBenefitClaimAddressResponse>(MessageProcessType.Local);
            //                }
            //                if (request.UDOupdateCADDAppellantAddress != null)
            //                {
            //                    #region updateAppellantAddress
            //                    var updateAppellantAddressRequest = new VIMT.AppealService.Messages.VIMTupdAppAddr_updateAppellantAddressRequest
            //                    {
            //                        LogTiming = request.LogTiming,
            //                        LogSoap = request.LogSoap,
            //                        Debug = request.Debug,
            //                        RelatedParentEntityName = request.RelatedParentEntityName,
            //                        RelatedParentFieldName = request.RelatedParentFieldName,
            //                        RelatedParentId = request.RelatedParentId,
            //                        UserId = request.UserId,
            //                        OrganizationName = request.OrganizationName,


            //                        LegacyServiceHeaderInfo = new VIMT.AppealService.Messages.HeaderInfo
            //                        {
            //                            ApplicationName = updateAddressLegacyServiceHeaderInfo.ApplicationName,
            //                            ClientMachine = updateAddressLegacyServiceHeaderInfo.ClientMachine,
            //                            LoginName = updateAddressLegacyServiceHeaderInfo.LoginName,
            //                            Password = updateAddressLegacyServiceHeaderInfo.Password,
            //                            StationNumber = updateAddressLegacyServiceHeaderInfo.StationNumber
            //                        }
            //                    };
            //                    var AppellantAddress = new VIMT.AppealService.Messages.VIMTupdAppAddr_appellantaddress();

            //                    //Do not check if blank/empty
            //                    AppellantAddress.mcs_AppellantAddressLine2 = request.UDOupdateCADDAppellantAddress.AppellantAddressLine2;

            //                    if (!string.IsNullOrEmpty(request.UDOupdateCADDAppellantAddress.AppellantAddressLine1))
            //                    {
            //                        AppellantAddress.mcs_AppellantAddressLine1 = request.UDOupdateCADDAppellantAddress.AppellantAddressLine1;
            //                    }
            //                    if (!string.IsNullOrEmpty(request.UDOupdateCADDAppellantAddress.AppellantAddressCity))
            //                    {
            //                        AppellantAddress.mcs_AppellantAddressCityName = request.UDOupdateCADDAppellantAddress.AppellantAddressCity;
            //                    }
            //                    if (!string.IsNullOrEmpty(request.UDOupdateCADDAppellantAddress.AppellantAddressCountry))
            //                    {
            //                        AppellantAddress.mcs_AppellantAddressCountryName = request.UDOupdateCADDAppellantAddress.AppellantAddressCountry;
            //                    }
            //                    if (!string.IsNullOrEmpty(request.UDOupdateCADDAppellantAddress.AppellantAddressPhone1))
            //                    {
            //                        AppellantAddress.mcs_AppellantAddressZipCode = request.UDOupdateCADDAppellantAddress.AppellantAddressZipCode;
            //                    }
            //                    if (!string.IsNullOrEmpty(request.UDOupdateCADDAppellantAddress.AppellantAddressPhone2))
            //                    {
            //                        AppellantAddress.mcs_AppellantHomePhoneNumber = request.UDOupdateCADDAppellantAddress.AppellantAddressPhone2;
            //                    }
            //                    if (!string.IsNullOrEmpty(request.UDOupdateCADDAppellantAddress.AppellantAddressPhone1))
            //                    {
            //                        AppellantAddress.mcs_AppellantWorkPhoneNumber = request.UDOupdateCADDAppellantAddress.AppellantAddressPhone1;
            //                    }
            //                    if (!string.IsNullOrEmpty(request.UDOupdateCADDAppellantAddress.AppellantAddressState))
            //                    {
            //                        AppellantAddress.mcs_AppellantAddressStateCode = request.UDOupdateCADDAppellantAddress.AppellantAddressState;
            //                    }
            //                    if (!string.IsNullOrEmpty(request.UDOupdateCADDAppellantAddress.LastUpdatedDate))
            //                    {
            //                        AppellantAddress.mcs_AppellantAddressLastModifiedDate = DateTime.Today.ToString();
            //                    }
            //                    if (!string.IsNullOrEmpty(request.UDOupdateCADDAppellantAddress.AddressKey))
            //                    { 
            //                        AppellantAddress.mcs_AddressKey = request.UDOupdateCADDAppellantAddress.AddressKey;                    
            //                    }


            //                    updateAppellantAddressRequest.updateappellantaddressrequestInfo = new VIMT.AppealService.Messages.VIMTupdAppAddr_updateappellantaddressrequest
            //                    {
            //                        appellantaddressInfo = AppellantAddress
            //                    };
                                  // (TN): Orginal code had commented out
            //                    var UDOupdateCADDAddressRequest = updateAppellantAddressRequest.SendReceive<VIMT.AppealService.Messages.VIMTupdAppAddr_updateAppellantAddressResponse>(MessageProcessType.Local);
            //                    #endregion
            //                }

            //                #endregion

            //                //added to generated code
            //                if (request.va_bankaccountId != System.Guid.Empty)
            //                {
            //                    var parent = new Entity();
            //                    parent.Id = request.va_bankaccountId;
            //                    parent.LogicalName = "va_bankaccount";
            //                    parent["va_bankaccountcomplete"] = true;
            //                    //parent["va_bankaccountmessage"] = "";
            //                    OrgServiceProxy.Update(parent);
            //                }
            //                return response;
            //            }
            //            catch (Exception connectException)
            //            {
            //                LogHelper.LogError(request.OrganizationName, "va_bankaccount", request.UserId, "UDOupdateCADDAddress Processor, Progess:" + progressString, connectException.Message);
            //                response.ExceptionMessage = "Failed to Map EC data to LOB";
            //                response.ExceptionOccured = true;
            //                if (request.va_bankaccountId != System.Guid.Empty)
            //                {
            //                    var parent = new Entity();
            //                    parent.Id = request.va_bankaccountId;
            //                    parent.LogicalName = "va_bankaccount";
            //                    //parent["va_bankaccountmessage"] = "";
            //                    OrgServiceProxy.Update(parent);
            //                }
            //                return response;
            //            }
        }
    }
}