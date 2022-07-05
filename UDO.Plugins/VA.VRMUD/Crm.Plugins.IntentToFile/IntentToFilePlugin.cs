using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Va.CrmUD.Utility;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using System.Timers;
using System.Diagnostics;
using Microsoft.Xrm.Sdk.Query;
using System.Net;
using System.Security.Cryptography;
using Microsoft.Crm.Sdk.Messages;
using Crm.WebServices;
using Microsoft.Xrm.Sdk.Client;
using System.IO;

namespace Crm.Plugins.IntentToFile
{
    public class IntentToFilePlugin : IPlugin
    {
        /// <summary>
        /// A plug-in that auto generates an account number when an
        /// account is created.
        /// </summary>
        /// <remarks>Register this plug-in on the Create message, account entity,
        /// and pre-operation stage.
        /// </remarks>
        //<snippetAccountNumberPlugin2>
        public void Execute(IServiceProvider serviceProvider)
        {
            //Obtain the execution context from the service provider.
            Microsoft.Xrm.Sdk.IPluginExecutionContext context = (Microsoft.Xrm.Sdk.IPluginExecutionContext)
                serviceProvider.GetService(typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            ITracingService tracingService =
    (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace("Starting here right now.");
            tracingService.Trace("Entering beginning of plugin logic.");
            // The InputParameters collection contains all the data passed in the message request.
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity && context.Depth == 1)
            {
                // Obtain the target entity from the input parameters.
                Entity entity = (Entity)context.InputParameters["Target"];
                Va.CrmUD.Utility.va_intenttofile itf = null;
                // Verify that the target entity represents an account.
                // If not, this plug-in was not registered correctly.
                if (entity.LogicalName == Va.CrmUD.Utility.va_intenttofile.EntityLogicalName)
                {
                    tracingService.Trace("This is an Intent to File record.");
                    itf = entity.ToEntity<Va.CrmUD.Utility.va_intenttofile>();
                    OrganizationServiceProxy serviceProxy = (OrganizationServiceProxy)service;
                    XrmServiceContext xrmServiceContext = new XrmServiceContext(service);
                    tracingService.Trace("Impersonating System Admin User.");
                    var systemAdminGuid = GetSystemAdministratorGuid(xrmServiceContext);
                    serviceProxy.CallerId = systemAdminGuid;
                    xrmServiceContext = null;
                    xrmServiceContext = new XrmServiceContext(serviceProxy);
                    tracingService.Trace("Succesfully impersonated the user.");

                    tracingService.Trace("Created the xrmServiceContext.");
                    //tracingService.Trace(string.Format("Entity Name = {0}", itf.ObjectTypeCode));
                    tracingService.Trace(string.Format("Intent To File Record Id = {0}.", itf.Id.ToString()));
                    try
                    {
                        if (context.MessageName.ToUpper() == "CREATE")
                        {
                            tracingService.Trace("Retrieving User Setttings.");
                            var bgsConfig = GetUserSettings(xrmServiceContext, context.UserId);
                            bgsConfig.ApplicationName = "CRMUD";
                            //bgsConfig.BepServiceUrl = "http://linktestbepbenefits.vba.va.gov:80/IntentToFileWebServiceBean/IntentToFileWebService"

                            string request = string.Empty;
                            string response = string.Empty;
                            var compensationType = "";

                            tracingService.Trace("Setting Benefit Type Values.");
                            switch (itf.va_GeneralBenefitType.Value)
                            {
                                case (int)va_intenttofileva_GeneralBenefitType.Compensation:
                                    compensationType = "C";
                                    break;
                                case (int)va_intenttofileva_GeneralBenefitType.Pension:
                                    compensationType = "P";
                                    break;
                                case (int)va_intenttofileva_GeneralBenefitType.Survivor:
                                    compensationType = "S";
                                    break;
                                default:
                                    break;
                            }

                            var gender = string.Empty;
                            if (itf.va_VeteranGender.HasValue)
                            {
                                if (itf.va_VeteranGender == true)
                                    gender = "F";
                                else
                                    gender = "M";
                            }

                            string phoneDigits = string.Empty;
                            string phoneNumber = string.Empty;
                            string areaCode = string.Empty;
                            if (!String.IsNullOrEmpty(itf.va_VeteranPhone))
                            {
                                phoneDigits = new string(itf.va_VeteranPhone.Where(c => char.IsDigit(c)).ToArray());
                                if (phoneDigits.Length == 10)
                                {
                                    phoneNumber = phoneDigits.Substring(3);
                                    areaCode = phoneDigits.Substring(0, 3);
                                }
                                else if (phoneDigits.Length == 11)
                                {
                                    phoneNumber = phoneDigits.Substring(4);
                                    areaCode = phoneDigits.Substring(1, 3);
                                }
                            }

                            string state = string.Empty;
                            if (!String.IsNullOrEmpty(itf.va_VeteranState) && itf.va_VeteranState.Length == 2)
                                state = itf.va_VeteranState;

                            tracingService.Trace("Setting Intent To File Web Service Fields.");
                            Claimant claimant = new Claimant()
                            {
                                ClaimantParticipantId = itf.va_ClaimantParticipantId,
                                VeteranParticipantId = itf.va_ParticipantId,
                                CompensationType = compensationType,
                                VeteranFirstName = itf.va_VeteranFirstName,
                                VeteranLastName = itf.va_VeteranLastName,
                                VeteranMiddleInitial = itf.va_VeteranMiddleInitial,
                                VeteranSsn = !String.IsNullOrEmpty(itf.va_VeteranSsn) ? InsertIntentToFile.ConvertToSecureString(itf.va_VeteranSsn) : null,
                                VeteranBirthDate = itf.va_VeteranDateofBirth,
                                VeteranGender = gender,
                                VeteranFileNumber = itf.va_VeteranFileNumber,
                                ClaimantFirstName = itf.va_ClaimantFirstName,
                                ClaimantLastName = itf.va_ClaimantLastName,
                                ClaimantMiddleInitial = itf.va_ClaimantMiddleInitial,
                                ClaimantSsn = !String.IsNullOrEmpty(itf.va_ClaimantSsn) ? InsertIntentToFile.ConvertToSecureString(itf.va_ClaimantSsn) : null,
                                Phone = phoneNumber,
                                PhoneAreaCode = areaCode,
                                Email = itf.va_VeteranEmail,
                                AddressLine1 = itf.va_VeteranAddressLine1,
                                AddressLine2 = itf.va_VeteranAddressLine2,
                                AddressLine3 = itf.va_VeteranUnitNumber,
                                City = itf.va_VeteranCity,
                                State = state,
                                Zip = itf.va_VeteranZip,
                                Country = itf.va_VeteranCountry,
                                UserId = bgsConfig.Username,
                                StationLocation = bgsConfig.StationId,
                            };
                            tracingService.Trace("Sending record to Corp Web Service.");

                            var itfDto = new InsertIntentToFile().InsertItf(bgsConfig, claimant, out request, out response); //itf.va_ParticipantId, compensationType, out request, out response);
                            tracingService.Trace("Succesfully Saved record.");
                            //// CompensationType
                            //// ParicipantId

                            itf.va_IntentToFileDate = DateTime.Now;
                            itf.va_CorpDbResponse = response;
                            itf.va_CorpDBRequest = request;
                            itf.va_userid = itfDto.jrnUserId;
                            itf.va_stationlocation = itfDto.jrnLctnId;
                            itf.va_CreatedDate = itfDto.createDt;
                            itf.va_SourceApplicationName = itfDto.submtrApplcnTypeCd;
                        };
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException(ex.Message);
                    }
                }
            }
        }

        private static BgsConfig GetUserSettings(XrmServiceContext xrmServiceContext, Guid userId)
        {
            BgsConfig bgsConfig = new BgsConfig();

            var user = xrmServiceContext.SystemUserSet.Where(s => s.Id == userId).FirstOrDefault();
            var itfUrl = xrmServiceContext.va_systemsettingsSet.Where(s => s.va_name == "ITF WebService").FirstOrDefault();
            var itfCert = xrmServiceContext.va_systemsettingsSet.Where(s => s.va_name == "ITF Cert").FirstOrDefault();
            if (user != null)
            {
                if (itfUrl != null)
                {
                    bgsConfig.StationId = user.va_StationNumber;
                    bgsConfig.Username = user.va_WSLoginName;
                    bgsConfig.BepServiceUrl = itfUrl.va_Description;
                    if (itfCert != null && !String.IsNullOrEmpty(itfCert.va_Description))
                        bgsConfig.ThumbPrint = itfCert.va_Description.Replace(" ", "").ToUpper();
                }
                else
                {
                    throw new InvalidPluginExecutionException("The Intent To File WebService Url has not been configured.");
                }
            }
            else
            {
                throw new InvalidPluginExecutionException("Unable to get User credentials.");
            }

            return bgsConfig;
        }

        private static Guid GetSystemAdministratorGuid(XrmServiceContext xrmServiceContext)
        {
            Guid systemUser = Guid.Empty;
            var sysUsername = xrmServiceContext.va_systemsettingsSet.Where(s => s.va_name == "System User").FirstOrDefault();
            if (sysUsername != null)
            {
                var admin = xrmServiceContext.SystemUserSet.Where(u => u.DomainName == sysUsername.va_Description).FirstOrDefault();
                if (admin != null)
                    systemUser = admin.Id;
                else
                    throw new InvalidPluginExecutionException("The system administrator account is not setup for Intent To File.");
            }
            return systemUser;
        }

        //private static bool CheckIntentToFile(BgsConfig bgsConfig, Va.CrmUD.Utility.va_intenttofile itf, string response)
        //{
        //    bool canIntentToFile = true;
        //    var intentToFileRecords = FindIntentToFile.FindIntentToFileByPtcpntId(bgsConfig, itf.va_ParticipantId, out response);
        //    foreach (var intentTofFileRecord in intentToFileRecords)
        //    {
        //        if (intentTofFileRecord.itfStatusTypeCd.ToUpper() == "ACTIVE")
        //        {
        //            canIntentToFile = false;
        //            break;
        //        }
        //    }
        //    return canIntentToFile;
        //}

    }
}
d