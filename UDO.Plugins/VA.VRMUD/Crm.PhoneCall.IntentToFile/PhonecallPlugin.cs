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

namespace CRM.Phonecall
{
    public class PhonecallPlugin : IPlugin
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
            tracingService.Trace("Entering beginning of plugin logic");
            // The InputParameters collection contains all the data passed in the message request.
            //if (context.InputParameters.Contains("Target") &&
            //    context.InputParameters["Target"] is Entity && context.Depth == 1)
            //{
            // Obtain the target entity from the input parameters.
            Entity entity = (Entity)context.OutputParameters["BusinessEntity"];
            //Entity entity = (Entity)context.InputParameters["Target"];
            Va.CrmUD.Utility.PhoneCall phonecall = null;
            // Verify that the target entity represents an account.
            // If not, this plug-in was not registered correctly.
            if (entity.LogicalName == Va.CrmUD.Utility.PhoneCall.EntityLogicalName)
            {
                try
                {
                    tracingService.Trace("This is a phonecall");
                    phonecall = entity.ToEntity<Va.CrmUD.Utility.PhoneCall>();
                    OrganizationServiceProxy serviceProxy = (OrganizationServiceProxy)service;
                    XrmServiceContext xrmServiceContext = new XrmServiceContext(service);
                    bool hasItfRole = DoesUserHaveIntentToFileRole(xrmServiceContext, context.UserId);
                    bool isItf = IsIntentToFile(xrmServiceContext, phonecall);//IsIntentToFile(xrmServiceContext, phonecall.Id);
                    if (hasItfRole && isItf)
                    {
                        tracingService.Trace("Impersonating the System Admin user.");
                        //var systemAdminGuid = GetSystemAdministratorGuid(xrmServiceContext);
                        //serviceProxy.CallerId = systemAdminGuid;
                        //xrmServiceContext = null;
                        //xrmServiceContext = new XrmServiceContext(serviceProxy);
                        //tracingService.Trace("Succesfully impersonated the System Admin user.");
                        tracingService.Trace("Created the xrmServiceContext");
                        tracingService.Trace(string.Format("Record Id = {0}", phonecall.Id.ToString()));

                        // Populate Intent To File records
                        tracingService.Trace("Populating Intent To File records.");
                        //var phone = xrmServiceContext.PhoneCallSet.Where(p => p.Id == phonecall.Id).FirstOrDefault();
                        PopulateIntentToFileRecords(xrmServiceContext, phonecall, service, context.UserId, phonecall);
                        tracingService.Trace("Succesfully populated the Intent To File records.");
                    }
                }
                catch (Exception ex)
                {
                    phonecall.va_IntentToFileResponse = ex.Message;
                    //throw new InvalidPluginExecutionException(ex.Message);//"Error retrieving Intent To File Records");
                }
            }
            ////}

        }

        /// <summary>
        /// Populates the intent to file records.
        /// </summary>
        /// <param name="xrmServiceContext">The XRM service context.</param>
        /// <param name="phone">The phone.</param>
        /// <param name="service">The service.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="phoneCallPlugin">The phone call plugin.</param>
        private void PopulateIntentToFileRecords(XrmServiceContext xrmServiceContext, PhoneCall phone, IOrganizationService service, Guid userId, PhoneCall phoneCallPlugin)
        {
            //throw new NotImplementedException();
            // Delete previous records
            if (phone != null)
            {
                if (!String.IsNullOrEmpty(phone.va_ParticipantID))
                {
                    var bgsConfig = GetUserSettings(xrmServiceContext, userId);
                    bgsConfig.ApplicationName = "CRMUD";
                    DeleteItfRecords(xrmServiceContext, phone);
                    string response = string.Empty;
                    string request = string.Empty;

                    var intentToFileRecords = new FindIntentToFile().FindIntentToFileByPtcpntId(bgsConfig, phone.va_ParticipantID, out request, out response);

                    foreach (var intentToFileDto in intentToFileRecords)
                    {
                        string areaCode = !String.IsNullOrEmpty(intentToFileDto.clmantPhoneAreaNbr) ? intentToFileDto.clmantPhoneAreaNbr : string.Empty;
                        string phoneNumber = !String.IsNullOrEmpty(intentToFileDto.clmantPhoneNbr) ? intentToFileDto.clmantPhoneNbr : string.Empty;
                        string veteranPhone = areaCode + phoneNumber;

                        va_intenttofile intentToFileRecord = new va_intenttofile()
                        {
                            va_ClaimantLastName = intentToFileDto.clmantLastNm,//phone.va_LastName,
                            va_ClaimantMiddleInitial = intentToFileDto.clmantMiddleNm, // phone.va_MiddleInitial,
                            va_ClaimantFirstName = intentToFileDto.clmantFirstNm, //phone.va_FirstName,
                            va_ClaimantSsn = intentToFileDto.clmantSsn,
                            va_VeteranLastName = intentToFileDto.vetLastNm,
                            va_VeteranFirstName = intentToFileDto.vetFirstNm,
                            va_VeteranMiddleInitial = intentToFileDto.vetMiddleNm,
                            va_VeteranSsn = intentToFileDto.vetSsnNbr,
                            va_VeteranFileNumber = intentToFileDto.vetFileNbr,
                            va_VeteranAddressLine1 = intentToFileDto.clmantAddrsOneTxt,
                            va_VeteranAddressLine2 = intentToFileDto.clmantAddrsTwoTxt,
                            va_VeteranUnitNumber = intentToFileDto.clmantAddrsUnitNbr,
                            va_VeteranCity = intentToFileDto.clmantCityNm,
                            va_VeteranZip = intentToFileDto.clmantZipCd,
                            va_VeteranCountry = intentToFileDto.clmantCntryNm,
                            va_ParticipantId = string.Format("{0}", intentToFileDto.ptcpntVetId),
                            va_ClaimantParticipantId = string.Format("{0}", intentToFileDto.ptcpntClmantId),
                            va_PhoneCallId = new EntityReference(PhoneCall.EntityLogicalName, phone.Id),
                            va_stationlocation = intentToFileDto.jrnLctnId,
                            va_userid = intentToFileDto.jrnUserId,
                            va_VeteranPhone = veteranPhone,
                            va_VeteranState = intentToFileDto.clmantStateCd,
                            va_CorpDBRequest = request,
                            va_CorpDbResponse = response,
                            va_IntentToFileStatus = intentToFileDto.itfStatusTypeCd,
                            va_SourceApplicationName = intentToFileDto.submtrApplcnTypeCd
                        };

                        switch (intentToFileDto.itfTypeCd)
                        {
                            case "C":
                                intentToFileRecord.va_GeneralBenefitType = new OptionSetValue((int)va_intenttofileva_GeneralBenefitType.Compensation);
                                break;
                            case "P":
                                intentToFileRecord.va_GeneralBenefitType = new OptionSetValue((int)va_intenttofileva_GeneralBenefitType.Pension);
                                break;
                            case "S":
                                intentToFileRecord.va_GeneralBenefitType = new OptionSetValue((int)va_intenttofileva_GeneralBenefitType.Survivor);
                                break;
                            default:
                                break;
                        }

                        if (intentToFileDto.vetBrthdyDt != DateTime.MinValue)
                            intentToFileRecord.va_VeteranDateofBirth = intentToFileDto.vetBrthdyDt;

                        if (intentToFileDto.rcvdDt != DateTime.MinValue)
                            intentToFileRecord.va_IntentToFileDate = intentToFileDto.rcvdDt;

                        if (intentToFileDto.createDt != DateTime.MinValue)
                            intentToFileRecord.va_CreatedDate = intentToFileDto.createDt;

                        if (!String.IsNullOrEmpty(intentToFileDto.genderCd))
                        {
                            if (intentToFileDto.genderCd == "M")
                                intentToFileRecord.va_VeteranGender = false;
                            else
                                intentToFileRecord.va_VeteranGender = true;
                        }

                        xrmServiceContext.AddObject(intentToFileRecord);
                        xrmServiceContext.SaveChanges();

                        //if (intentToFileDto.itfStatusTypeCd != "Active")
                        //{
                        SetStateRequest setState = new SetStateRequest();
                        setState.EntityMoniker = new EntityReference()
                        {
                            Id = intentToFileRecord.Id,
                            LogicalName = va_intenttofile.EntityLogicalName
                        };
                        setState.State = new OptionSetValue((int)va_intenttofileState.Inactive);
                        setState.Status = new OptionSetValue((int)va_intenttofile_statuscode.Inactive);
                        SetStateResponse setStateResponse = (SetStateResponse)service.Execute(setState);
                        //}
                        phoneCallPlugin.va_IntentToFileResponse = response;
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the itf records.
        /// </summary>
        /// <param name="xrmServiceContext">The XRM service context.</param>
        /// <param name="phone">The phone.</param>
        private static void DeleteItfRecords(XrmServiceContext xrmServiceContext, PhoneCall phone)
        {
            var itfCollection = xrmServiceContext.va_intenttofileSet.Where(i => i.va_PhoneCallId.Id == phone.Id);
            if (itfCollection != null && itfCollection.ToList().Count > 0)
            {
                foreach (var itf in itfCollection)
                {
                    xrmServiceContext.DeleteObject(itf);
                }
                xrmServiceContext.SaveChanges();
            }
            //bool doWait = true;
            //while (doWait)
            //{
            //    itfCollection = xrmServiceContext.va_intenttofileSet.Where(i => i.va_PhoneCallId.Id == phone.Id);
            //    if (itfCollection.ToList().Count == 0)
            //        doWait = false;
            //}
        }

        /// <summary>
        /// Gets the system administrator unique identifier.
        /// </summary>
        /// <param name="xrmServiceContext">The XRM service context.</param>
        /// <returns></returns>
        /// <exception cref="InvalidPluginExecutionException">The system administrator account is not setup for Intent To File.</exception>
        private static Guid GetSystemAdministratorGuid(XrmServiceContext xrmServiceContext)
        {
            Guid systemUser = Guid.Empty;
            var sysUsername = xrmServiceContext.va_systemsettingsSet.Where(s => s.va_name == "System User").FirstOrDefault();
            if (sysUsername != null)
            {
                var admin = xrmServiceContext.SystemUserSet.Where(u => u.DomainName.Contains(sysUsername.va_Description)).FirstOrDefault();
                if (admin != null)
                    systemUser = admin.Id;
                else
                    throw new InvalidPluginExecutionException("The system administrator account is not setup for Intent To File.");
            }
            return systemUser;
        }

        /// <summary>
        /// Gets the user settings.
        /// </summary>
        /// <param name="xrmServiceContext">The XRM service context.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="InvalidPluginExecutionException">
        /// The Intent To File WebService Url has not been configured.
        /// or
        /// Unable to get User credentials.
        /// </exception>
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

        /// <summary>
        /// Doeses the user have intent to file role.
        /// </summary>
        /// <param name="xrmServiceContext">The XRM service context.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        private static bool DoesUserHaveIntentToFileRole(XrmServiceContext xrmServiceContext, Guid userId)
        {
            bool hasItfRole = false;
            var itfRoles = xrmServiceContext.RoleSet.Where(r => r.Name == "Intent To File");
            if (itfRoles.ToList().Count > 0)
            {
                foreach (var itfRole in itfRoles)
                {
                    var role = xrmServiceContext.SystemUserRolesSet.Where(ur => ur.SystemUserId == userId && ur.RoleId == itfRole.Id).FirstOrDefault();
                    if (role != null)
                    {
                        hasItfRole = true;
                        break;
                    }
                }
            }
            else
            {
                throw new InvalidPluginExecutionException("No ITF role");
            }
            return hasItfRole;
        }

        /// <summary>
        /// Determines whether [is intent to file] [the specified XRM service context].
        /// </summary>
        /// <param name="xrmServiceContext">The XRM service context.</param>
        /// <param name="phoneCallId">The phone call identifier.</param>
        /// <returns></returns>
        private static bool IsIntentToFile(XrmServiceContext xrmServiceContext, PhoneCall phoneCall) //Guid phoneCallId)
        {
            bool isIntentToFile = false;
            //var phoneCall = xrmServiceContext.PhoneCallSet.Where(p => p.Id == phoneCallId).FirstOrDefault();
            if (phoneCall != null && phoneCall.va_Disposition != null && phoneCall.va_DispositionSubtype != null)
            {
                if (phoneCall != null && phoneCall.va_Disposition.Value ==
                        (int)va_calldispositions.Claim && phoneCall.va_DispositionSubtype.Value == (int)va_dispositionsubtypes.ClaimIntentToFile)
                    isIntentToFile = true;
            }
            return isIntentToFile;
        }
    }
}
