using System;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Linq;
using VRMRest;
using System.Net.Http;
using Microsoft.Xrm.Sdk.Query;
using UDO.Model;
using System.ServiceModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.PersonSearch.Messages;
using UDO.LOB.Core;
//using UDO.LOB.PersonSearch.Models;
using UDO.LOB.PersonSearch.Messages;
using CustomActions.Plugins.Messages.PersonSearch;

namespace CustomActions.Plugins.Entities.PersonSearch
{
    public class UDOPersonSearchRunner : UDOActionRunner
    {
        protected Entity thisUser = new Entity();
        protected crme_person crmePerson;
        protected Guid _ownerId = new Guid();
        protected EntityCollection resultCollection = new EntityCollection();
        protected string _interactionId = string.Empty;

        public UDOPersonSearchRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_searchlogtiming";
            _logSoapField = "udo_searchlogsoap";
            _debugField = "udo_searchdebug";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_searchtimeout";
            _validEntities = new string[] { "crme_person" };
        }

        public override void DoAction()
        {
            _method = "DoAction";
            crmePerson = EntityParameter.ToEntity<crme_person>();

            if (crmePerson == null)
            {
                DataIssue = true;
                _responseMessage = "Please supply a valid crme_person entity object for searching";
                return;
            }

            GetSettingValues();

            if (crmePerson.crme_SearchType.ToUpper() == "CombinedSelectedPerson")
                SelectPerson();
            else
                FindPerson();

            PluginExecutionContext.OutputParameters["ResultCollection"] = (EntityCollection)resultCollection;
        }

        private void FindPerson()
        {
            var personSearchHeaderInfo = PersonSearchHeaderInfo.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            var FindPersonSearchRequest = new UDOpsFindPersonRequest();
            FindPersonSearchRequest.MessageId = PluginExecutionContext.CorrelationId.ToString();
            FindPersonSearchRequest.OrganizationName = PluginExecutionContext.OrganizationName;
            FindPersonSearchRequest.UserId = PluginExecutionContext.InitiatingUserId;
            FindPersonSearchRequest.Debug = _debug;
            FindPersonSearchRequest.LogSoap = _logSoap;
            FindPersonSearchRequest.LogTiming = _logTimer;
            FindPersonSearchRequest.LegacyServiceHeaderInfo = personSearchHeaderInfo.HeaderInfo;
            FindPersonSearchRequest.noAddPerson = _addperson;
            FindPersonSearchRequest.MVICheck = _MVICheck;
            FindPersonSearchRequest.BypassMvi = _bypassMvi;
            FindPersonSearchRequest.UserSL = personSearchHeaderInfo.UserSL;
            FindPersonSearchRequest.UDOSearchType = crmePerson.crme_SearchType;

            FindPersonSearchRequest.SSIdString = crmePerson.crme_SSN;
            FindPersonSearchRequest.FirstName = crmePerson.crme_FirstName;
            FindPersonSearchRequest.MiddleName = crmePerson.crme_MiddleName;
            FindPersonSearchRequest.FamilyName = crmePerson.crme_LastName;
            FindPersonSearchRequest.BranchOfService = crmePerson.crme_BranchOfService;
            FindPersonSearchRequest.BirthDate = crmePerson.crme_DOBString;
            FindPersonSearchRequest.PhoneNumber = crmePerson.crme_PrimaryPhone;
            FindPersonSearchRequest.IdentifierClassCode = crmePerson.crme_ClassCode;
            FindPersonSearchRequest.IsAttended = crmePerson.crme_IsAttended.HasValue ? crmePerson.crme_IsAttended.Value : false;
            FindPersonSearchRequest.OrganizationName = PluginExecutionContext.OrganizationName;
            //FindPersonSearchRequest.FetchMessageProcessType = "remote";
            FindPersonSearchRequest.UserId = PluginExecutionContext.UserId;
            FindPersonSearchRequest.UserFirstName = personSearchHeaderInfo.FirstName;
            FindPersonSearchRequest.UserLastName = personSearchHeaderInfo.LastName;

            if (!string.IsNullOrEmpty(crmePerson.crme_EDIPI) && !crmePerson.crme_EDIPI.ToUpper().Contains("NOT"))
                FindPersonSearchRequest.Edipi = crmePerson.crme_EDIPI;
            else
                FindPersonSearchRequest.IsAttended = true;

            if (!string.IsNullOrEmpty(crmePerson.crme_ParticipantID))
                FindPersonSearchRequest.ParticipantID = Convert.ToInt64(crmePerson.crme_ParticipantID);

            if (crmePerson.crme_udointeractionid != null)
            {
                FindPersonSearchRequest.InteractionId = Guid.Parse(crmePerson.crme_udointeractionid);
            }

            if (!string.IsNullOrEmpty(FindPersonSearchRequest.Edipi))
            {
                FindPersonSearchRequest.IdentifierClassCode = "MIL";
            }

            LogSettings _logSettings = new LogSettings()
            {
               Org = PluginExecutionContext.OrganizationName,
               ConfigFieldName = "RESTCALL",
               UserId = PluginExecutionContext.InitiatingUserId,
               callingMethod = "UDOPersonSearch"
            };

            var response = Utility.SendReceive<UDOpsFindPersonResponse>(_uri, "UDOpsFindPersonRequest", FindPersonSearchRequest, _logSettings, _timeOutSetting,_crmAuthTokenConfig, tracer);

            if(response.Person == null || (response.Person != null && response.Person.FirstOrDefault() == null))
            {
                var newPerson = new crme_person();
                newPerson.Id = Guid.NewGuid();

                newPerson.crme_ReturnMessage = response.MVIMessage + response.CORPDbMessage;

                resultCollection.Entities.Add(newPerson.ToEntity<Entity>());

                return;
            }

            foreach (var person in response.Person)
            {
                var newPerson = new crme_person();

                #region map the person with data
                if (!string.IsNullOrEmpty(person.SSIdString))
                {
                    var ssnGuid = new Guid(Regex.Replace(person.SSIdString, @"[\-]", "").PadRight(32, 'F'));
                    newPerson.Id = ssnGuid;
                }
                else
                {
                    newPerson.Id = Guid.NewGuid();
                }

                newPerson.Id = response.contactId;

                    if (person.NameList != null)
                    {
                        var legalName = person.NameList.FirstOrDefault(v => v.NameType.Equals("Legal", StringComparison.OrdinalIgnoreCase));
                        var alias = person.NameList.FirstOrDefault(v => v.NameType.Equals("Alias", StringComparison.OrdinalIgnoreCase));

                        if (legalName != null)
                        {
                            newPerson.crme_FirstName = legalName.GivenName;
                            newPerson.crme_LastName = legalName.FamilyName;
                            newPerson.crme_MiddleName = legalName.MiddleName;
                            newPerson.crme_Suffix = legalName.NameSuffix;
                        }
                        else
                        {
                            legalName = person.NameList.FirstOrDefault();

                            if (legalName != null)
                            {
                                newPerson.crme_FirstName = legalName.GivenName;
                                newPerson.crme_LastName = legalName.FamilyName;
                                newPerson.crme_MiddleName = legalName.MiddleName;
                                newPerson.crme_Suffix = legalName.NameSuffix;
                            }
                        }

                        newPerson.crme_Alias = TryGetAlias(alias);
                    }

                    newPerson.crme_DOBString = person.BirthDate;
                    newPerson.crme_BranchOfService = person.BranchOfService;
                    newPerson.crme_Rank = person.Rank;
                    newPerson.crme_FullName = person.FullName;
                    newPerson.crme_FullAddress = person.FullAddress;

                    newPerson.crme_PrimaryPhone = person.PhoneNumber;
                    newPerson.crme_RecordSource = person.RecordSource;
                    newPerson.crme_Gender = person.GenderCode;
                    newPerson.crme_DeceasedDate = person.DeceasedDate;
                    newPerson.crme_IdentityTheft = person.IdentifyTheft;
                    newPerson.crme_url = person.Url;
                    newPerson.crme_ReturnMessage = response.MVIMessage + " " + response.CORPDbMessage;
                    newPerson.crme_EDIPI = person.EdiPi;
                    newPerson.crme_SSN = person.SSIdString;
                    newPerson.crme_FileNumber = person.FileNumber;
                    newPerson.crme_ParticipantID = person.ParticipantId;
                    newPerson.crme_VeteranSensitivityLevel = person.VeteranSensitivityLevel;
                    TryGetMviQueryParams(person, newPerson);
                    newPerson.crme_ServiceNumber = person.ServiceNumber;
                    newPerson.crme_InsuranceNumber = person.InsuranceNumber;
                    newPerson.crme_EnteredOnDutyDate = person.EnteredOnDutyDate;
                    newPerson.crme_ReleasedActiveDutyDate = person.ReleasedActiveDutyDate;
                    newPerson.crme_PayeeNumber = person.PayeeNumber;
                    newPerson.crme_FolderLocation = person.FolderLocation;
                    if (_interactionId != null)
                    {
                        newPerson.crme_udointeractionid = _interactionId;
                    }
                   
                    newPerson.crme_udoidproofid = response.idProofId.ToString();


                    resultCollection.Entities.Add(newPerson.ToEntity<Entity>());

                #endregion
            
            //SetQueryString((UDOCombinedPersonSearchRequest)CombinedSearchRequest, qe);
            }
        }

        private void SelectPerson()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aliasName"></param>
        /// <returns></returns>
        private static string TryGetAlias(Name aliasName)
        {
            try
            {
                if (aliasName == null)
                {
                    return string.Empty;
                }

                const string nameFormat = "{0} {1} {2}";
                var alias = string.Format(nameFormat, aliasName.GivenName, aliasName.MiddleName, aliasName.FamilyName);

                return alias;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        private static void TryGetMviQueryParams(PatientPerson person, crme_person newPerson)
        {
            if (person.CorrespondingIdList == null || !person.CorrespondingIdList.Any())
            {
                newPerson.crme_PatientMviIdentifier = string.Empty;
                newPerson.crme_ICN = string.Empty;

            }
            else
            {

                var patientNo = person.CorrespondingIdList.FirstOrDefault((v =>
                    v.AssigningAuthority != null &&
                    v.AssigningAuthority.Equals("USVHA", StringComparison.InvariantCultureIgnoreCase) &&
                    v.AssigningFacility != null && v.AssigningFacility == "200M" &&
                    v.IdentifierType != null &&
                    v.IdentifierType.Equals("NI", StringComparison.InvariantCultureIgnoreCase))) ??
                                person.CorrespondingIdList.FirstOrDefault((v =>
                                    v.AssigningAuthority != null &&
                                    v.AssigningAuthority.Equals("USVHA", StringComparison.InvariantCultureIgnoreCase) &&
                                    v.IdentifierType != null &&
                                    v.IdentifierType.Equals("PI", StringComparison.InvariantCultureIgnoreCase)));

                newPerson.crme_PatientMviIdentifier = patientNo != null ? patientNo.RawValueFromMvi : string.Empty;
                newPerson.crme_ICN = patientNo != null ? patientNo.PatientIdentifier : string.Empty;
            }
        }
    }
}
