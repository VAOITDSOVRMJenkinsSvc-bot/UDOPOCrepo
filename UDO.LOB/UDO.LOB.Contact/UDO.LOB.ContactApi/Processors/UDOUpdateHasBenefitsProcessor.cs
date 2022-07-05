using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using UDO.LOB.Contact.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Core.Messages;
using VEIS.Messages.EBenefitsAccountActivity;

namespace UDO.LOB.Contact.Processors
{
    class UDOupdateHasBenefitsProcessor
    {
        private const string method = "UDOupdateHasBenefitsProcessor";

        public IMessageBase Execute(UDOupdateHasBenefitsRequest request)
        {
            UDOupdateHasBenefitsResponse response = new UDOupdateHasBenefitsResponse { MessageId = request?.MessageId };

            try
            {
                LogHelper.LogInfo("In UDOupdateBenefitsProcessor");
                Entity contact = new Entity("contact");
                contact.Id = new Guid(request.ContactId);
                if (!string.IsNullOrEmpty(request.Edipi) && request.Edipi != "UNK")
                {
                    var getEbenefitsStatus = new VEISebenAccgetRegistrationStatusRequest();
                    getEbenefitsStatus.MessageId = request.MessageId;
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
                        getEbenefitsStatus.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                    }

                    //non standard fields
                    getEbenefitsStatus.mcs_edipi = request.Edipi;
                    // REM: Invoke VEIS Endpoint
                    var benefitsresponse = WebApiUtility.SendReceive<VEISebenAccgetRegistrationStatusResponse>(getEbenefitsStatus, WebApiType.VEIS);
                    if (request.LogSoap || benefitsresponse.ExceptionOccurred)
                    {
                        if (benefitsresponse.SerializedSOAPRequest != null || benefitsresponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = benefitsresponse.SerializedSOAPRequest + benefitsresponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISebenAccgetRegistrationStatusRequest Request/Response {requestResponse}", true);
                        }
                    }

                    if (benefitsresponse.ExceptionOccured)
                    {
                        LogHelper.LogInfo(string.Format("Exception during VEISebenAccgetRegistrationStatus EC: {0}", benefitsresponse.ExceptionMessage));
                        response.ExceptionOccurred = benefitsresponse.ExceptionOccured;
                        response.ExceptionMessage = benefitsresponse.ExceptionMessage;
                        return response;
                    }

                    // Replaced: VIMTebenAccgetRegistrationResponseRecordInfo = VEISebenAccgetRegistrationStatusResponseDataInfo
                    if (benefitsresponse.VEISebenAccgetRegistrationStatusResponseDataInfo != null)
                    {
                        if (benefitsresponse.VEISebenAccgetRegistrationStatusResponseDataInfo != null)
                        {
                            var credLevel = benefitsresponse.VEISebenAccgetRegistrationStatusResponseDataInfo.mcs_credLevelAtLastLogin;
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
                                contact["udo_ebenefitstatus"] = "Registered: " + benefitsresponse.VEISebenAccgetRegistrationStatusResponseDataInfo.mcs_isRegistered + "; Credlevel:" + credLevelString + "; Status:" + benefitsresponse.VEISebenAccgetRegistrationStatusResponseDataInfo.mcs_status;
                                if (benefitsresponse.VEISebenAccgetRegistrationStatusResponseDataInfo.mcs_isRegistered)
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

                if (request.DiagnosticsContext == null && request != null)
                {
                    request.DiagnosticsContext = new DiagnosticsContext()
                    {
                        AgentId = request.UserId,
                        MessageTrigger = method,
                        OrganizationName = request.OrganizationName,
                        StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                    };

                    response.DiagnosticsContext = request.DiagnosticsContext;
                }

                #region connect to CRM
                CrmServiceClient OrgServiceProxy = null; 

                try
                {
                    OrgServiceProxy = ConnectionCache.GetProxy();
                }
                catch (Exception connectException)
                {
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                    response.ExceptionMessage = "Failed to get CRMConnection";
                    response.ExceptionOccurred = true;
                    return response;
                }
                #endregion

                using (OrgServiceProxy)
                {
                    OrgServiceProxy.Update(contact);
                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                response.ExceptionOccurred = true;
                response.ExceptionMessage = ExecutionException.Message;
                LogHelper.LogInfo(string.Format("Exception during UpdateHasBenefitsProcessor: {0}", response.ExceptionMessage));
                return response;
            }
        }
    }
}