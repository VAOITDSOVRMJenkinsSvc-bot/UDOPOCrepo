using System;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using System.Diagnostics;
// using CRM007.CRM.SDK.Core;
using Microsoft.Xrm.Sdk;
using VIMT.EBenefitsAccountActivity.Messages;
//using VRM.Integration.UDO.MVI.Messages;
using VRM.Integration.UDO.Contact.Messages;
using VRM.Integration.UDO.Common;
using Microsoft.Xrm.Sdk.Client;

namespace VRM.Integration.UDO.Contact.Processors
{
    class UDOupdateHasBenefitsProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOupdateHasBenefitsProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOupdateHasBenefitsRequest request)
        {
            UDOupdateHasBenefitsResponse response = new UDOupdateHasBenefitsResponse();

            try
            {
                Logger.Instance.Debug("In UDOupdateBenefitsProcessor");
                Entity contact = new Entity("contact");
                contact.Id = new Guid(request.ContactId);
                if (!string.IsNullOrEmpty(request.Edipi) && request.Edipi != "UNK")
                {
                    var getEbenefitsStatus = new VIMTebenAccgetRegistrationStatusRequest();  //using VIMT.EBenefitsAccountActivity.Messages;

                    getEbenefitsStatus.LogTiming = request.LogTiming;
                    getEbenefitsStatus.LogSoap = request.LogSoap;
                    getEbenefitsStatus.Debug = request.Debug;
                    getEbenefitsStatus.RelatedParentEntityName = request.RelatedParentEntityName;
                    getEbenefitsStatus.RelatedParentFieldName = request.RelatedParentFieldName;
                    getEbenefitsStatus.RelatedParentId = request.RelatedParentId;
                    getEbenefitsStatus.UserId = request.UserId;
                    getEbenefitsStatus.OrganizationName = request.OrganizationName;

                    if (request.LegacyServiceHeaderInfo != null)
                    {
                        getEbenefitsStatus.LegacyServiceHeaderInfo = new VIMT.EBenefitsAccountActivity.Messages.HeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                    }

                    //non standard fields
                    getEbenefitsStatus.mcs_edipi = request.Edipi;
                    // TODO(TN): Comment to remediate
                    var benefitsresponse = getEbenefitsStatus.SendReceive<VIMTebenAccgetRegistrationStatusResponse>(MessageProcessType.Local);

                    //response.ExceptionMessage = benefitsresponse.ExceptionMessage;
                    if (benefitsresponse.ExceptionOccured)
                    {
                        Logger.Instance.Warn(string.Format("Exception during VIMTebenAccgetRegistrationStatus EC: {0}", benefitsresponse.ExceptionMessage));
                        response.ExceptionOccurred = benefitsresponse.ExceptionOccured;
                        response.ExceptionMessage = benefitsresponse.ExceptionMessage;
                        return response;
                    }
                    
                    // response.UDOgetContactRecordsInfo = new UDOgetContactRecords();
                    if (benefitsresponse.VIMTebenAccgetRegistrationResponseRecordInfo != null)
                    {
                        var credLevel = benefitsresponse.VIMTebenAccgetRegistrationResponseRecordInfo.mcs_credLevelAtLastLogin;
                        if (credLevel != null)
                        {
                            var credLevelString = "Basic";
                            switch (credLevel)
                            {
                                case 0:
                                    credLevelString = "None";
                                    break;
                                case 1:
                                    credLevelString = "Basic";
                                    break;
                                default:
                                    credLevelString = "Premium";
                                    break;
                            }

                            // Update Contact Record
                            contact["udo_ebenefitstatus"] = "Registered: " + benefitsresponse.VIMTebenAccgetRegistrationResponseRecordInfo.mcs_isRegistered + "; Credlevel:" + credLevelString + "; Status:" + benefitsresponse.VIMTebenAccgetRegistrationResponseRecordInfo.mcs_status;
                            if (benefitsresponse.VIMTebenAccgetRegistrationResponseRecordInfo.mcs_isRegistered)
                            {
                                contact["udo_hasebenefitsaccount"] = new OptionSetValue(752280001);
                            }
                            else
                            {
                                contact["udo_hasebenefitsaccount"] = new OptionSetValue(752280000);
                            }
                        }
                        else
                        {
                            contact["udo_ebenefitstatus"] = "Not Applicable";
                            contact["udo_hasebenefitsaccount"] = new OptionSetValue(752280000);
                        }
                        
                    }
                    else
                    {
                        contact["udo_ebenefitstatus"] = "Not Applicable";
                        contact["udo_hasebenefitsaccount"] = new OptionSetValue(752280000);
                    }
                }
                else
                {
                    contact["udo_ebenefitstatus"] = "Not Applicable";
                    contact["udo_hasebenefitsaccount"] = new OptionSetValue(752280000);
                }

                //TODO: Review this:: var connectionParms = CrmConnectionConfiguration.Current.GetCrmConnectionParmsByName(request.OrganizationName.ToUpper());
                // IOrganizationService OrgServiceProxy = CrmConnection.Connect(connectionParms);
                OrganizationServiceProxy OrgServiceProxy; 

                #region connect to CRM
                try
                {
                    var CommonFunctions = new CRMConnect();
                    OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

                }
                catch (Exception connectException)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateAwardLinesProcessor Processor, Connection Error", connectException.Message);
                    response.ExceptionMessage = "Failed to get CRMConnection";
                    response.ExceptionOccurred = true;
                    return response;
                }
                #endregion

                OrgServiceProxy.Update(contact);
                return response;
            }
            catch (Exception ExecutionException)
            {
                response.ExceptionOccurred = true;
                response.ExceptionMessage = ExecutionException.Message;
                Logger.Instance.Warn(string.Format("Exception during UpdateHasBenefitsProcessor: {0}", response.ExceptionMessage));
                return response;
            }
        }

    }
}
