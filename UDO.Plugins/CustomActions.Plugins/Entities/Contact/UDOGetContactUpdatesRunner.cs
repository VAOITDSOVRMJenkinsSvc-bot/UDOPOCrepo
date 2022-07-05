using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using UDO.LOB.Contact.Messages;
using UDO.LOB.Core;
using UDO.Model;
using VRMRest;

namespace CustomActions.Plugins.Entities.Contact
{
    public class UDOGetContactUpdatesRunner : UDOActionRunner
    {
        protected Entity thisUser = new Entity();
        protected Guid _contactId = new Guid();
        protected Guid _ownerId = new Guid();
        protected string _ownerType = string.Empty;
        protected string _pid = string.Empty;
        protected string _edipi = string.Empty;
        protected string _fileNumber = string.Empty;
        protected string _sojName = string.Empty;
        protected UDOHeaderInfo _headerInfo;

        public UDOGetContactUpdatesRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_contactlogtimer";
            _logSoapField = "udo_contactlogsoap";
            _debugField = "udo_contact";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_contacttimeout";
            _validEntities = new string[] { "contact" };
        }

        public override void DoAction()
        {
            try
            {
                _method = "DoAction";
                _contactId = Parent.Id;

                var targetContact = GetTargetContact();

                if (!DidWeNeedData(targetContact))
                {
                    DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                    return;
                }

                GetSettingValues();


                var sourceContact = GetSourceContact(targetContact);

                if (sourceContact.Attributes.Count == 0)
                    return;


                if (targetContact.Contains("udo_ssn"))
                    targetContact.Attributes.Remove("udo_ssn");

                if (targetContact.Contains("udo_edipi"))
                    targetContact.Attributes.Remove("udo_edipi");

                if (targetContact.Contains("udo_filenumber"))
                    targetContact.Attributes.Remove("udo_filenumber");

                UpdateTargetContact(targetContact, sourceContact);
            }
            finally
            {
                Trace("Entered Finally");
                SetupLogger();
                Trace("Set up logger done.");
                ExecuteFinally();
                Trace("Exit Finally");
            }
        }

        private void UpdateTargetContact(Entity targetContact, Entity sourceContact)
        {
            var updated = false;
            var updatedContact = new Entity(UDO.Model.Contact.EntityLogicalName);

            try
            {
                foreach (var attribute in sourceContact.Attributes)
                {
                    var attName = attribute.Key;
                    var sourceValue = attribute.Value;

                    if (targetContact.Contains(attName))
                    {
                        var targetAttValue = targetContact[attName];

                        if (sourceValue != null && sourceValue.GetType().Equals(typeof(DateTime)))
                        {
                            var sourceDate = Convert.ToDateTime(sourceValue);
                            var targetDate = Convert.ToDateTime(targetAttValue);

                            if (sourceDate == null || targetDate == null)
                            {
                                updatedContact.Attributes.Add(attName, sourceValue);
                                continue;
                            }

                            var result = DateTime.Compare(sourceDate.Date, targetDate.Date);

                            if (result != 0)
                                updatedContact.Attributes.Add(attName, sourceValue);
                        }
                        else if (!targetAttValue.Equals(sourceValue))
                            updatedContact.Attributes.Add(attName, sourceValue);
                    }
                    else if (sourceValue != null)
                        updatedContact.Attributes.Add(attName, sourceValue);
                }

                if (updatedContact.Attributes.Count > 0)
                {
                    updatedContact.Id = targetContact.Id;
                    ElevatedOrganizationService.Update(updatedContact);
                    updated = true;
                }

            }
            catch (Exception ex)
            {
                PluginError = true;
                tracer.Trace(string.Format("Error message - {0}", ex.Message + " || " + ex.StackTrace));
                Trace(string.Format("Error message - {0}", ex.Message + " || " + ex.StackTrace));
                //Logger.WriteException(ex);
                throw ex;

            }



            EntityUpdate = updated;
        }

        private Entity GetSourceContact(Entity targetEntity)
        {
            UDO.Model.Contact sourceEntity = new UDO.Model.Contact();

            _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            var performBIRLSCall = false;

            if (targetEntity.Attributes.Contains("udo_dateofdeath"))
            {
                if (!targetEntity.Attributes.Contains("udo_causeofdeath"))
                {
                    performBIRLSCall = true;
                }
            }
            if (!targetEntity.Attributes.Contains("udo_releasedactivedutydate"))
            {
                performBIRLSCall = true;
            }

            var request = new UDOgetContactRecordsRequest()
            {
                MessageId = PluginExecutionContext.CorrelationId.ToString(),
                fileNumber = _fileNumber,
                ptcpntVetId = _pid,
                ptcpntBeneId = _pid,
                ptpcntRecipId = _pid,
                edipi = _edipi,
                Debug = _debug,
                RelatedParentEntityName = "contact",
                RelatedParentFieldName = "udo_contactid",
                RelatedParentId = _contactId,
                LogSoap = _logSoap,
                LogTiming = _logTimer,
                ownerId = _ownerId,
                ownerType = _ownerType,
                UserId = PluginExecutionContext.InitiatingUserId,
                OrganizationName = PluginExecutionContext.OrganizationName,
                VeteranId = PluginExecutionContext.PrimaryEntityId,
                performBIRLSCall = performBIRLSCall

            };

            if (targetEntity.Attributes.Contains("udo_ssn"))
            {
                request.vetsoc = targetEntity["udo_ssn"].ToString();
            }

            request.LegacyServiceHeaderInfo = _headerInfo;

            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = "Contact.Plugins.PostContactRetrieveRunner"
            };

            tracer.Trace("calling UDOgetContactRecordsRequest");
            Trace("calling UDOgetContactRecordsRequest");
            Logger.setDebug = request.Debug;

            var response = Utility.SendReceive<UDOgetContactRecordsResponse>(_uri, "UDOgetContactRecordsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Retrieve Contacts LOB. Skipped update to the Veteran record.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage += response.ExceptionMessage;
                }

                string errorMessageformat = "Error Message - {0}. CorrelationId: {1}";
                Logger.WriteToFile(string.Format(errorMessageformat, _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
                tracer.Trace(string.Format(errorMessageformat, _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
                Trace(string.Format(errorMessageformat, _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
                return sourceEntity;
            }

            #region Map Response
            if (response != null)
            {
                if (response.UDOgetContactRecordsInfo != null)
                {

                    //BIRLS FIELDS

                    //from BIRLS only if veteran is deceased
                    if (performBIRLSCall)
                    {
                        sourceEntity["udo_causeofdeath"] = response.UDOgetContactRecordsInfo.udo_CauseofDeath;
                        //this won't change, so only need if absent
                        sourceEntity["udo_releasedactivedutydate"] = response.UDOgetContactRecordsInfo.udo_activeReleaseDate.ToDateTime();
                    }

                    //this field is updated on the veteransnapshot load,  so use it.
                    if (targetEntity.Attributes.Contains("udo_folderlocation"))
                    {
                        var sojId = getSOJId(targetEntity["udo_folderlocation"].ToString());
                        Logger.setMethod = "Execute";
                        if (sojId != Guid.Empty)
                        {

                            EntityReference thisEntRef = new EntityReference();
                            thisEntRef.Name = _sojName;
                            thisEntRef.Id = sojId;
                            thisEntRef.LogicalName = "va_regionaloffice";

                            sourceEntity["va_stationofjurisdictionid"] = thisEntRef;
                        }
                    }


                    //ony fields needed on corp call
                    sourceEntity["emailaddress1"] = response.UDOgetContactRecordsInfo.EMailAddress1;
                    sourceEntity["udo_fidfolderloc"] = response.UDOgetContactRecordsInfo.udo_fidfolderloc ?? "";
                    sourceEntity["udo_phonenumber2"] = response.UDOgetContactRecordsInfo.udo_PhoneNumber2;
                    sourceEntity["udo_phonenumber1"] = response.UDOgetContactRecordsInfo.udo_PhoneNumber1;





                    //these fields are updated during search - TryGetCrmPerson - or TryNewCrmPerson
                    //sourceEntity["firstname"] = response.UDOgetContactRecordsInfo.FirstName;
                    //sourceEntity["middlename"] = response.UDOgetContactRecordsInfo.MiddleName;
                    //sourceEntity["lastname"] = response.UDOgetContactRecordsInfo.LastName;
                    //sourceEntity["udo_birthdatestring"] = response.UDOgetContactRecordsInfo.udo_BirthDateString;
                    //sourceEntity["udo_branchofservice"] = response.UDOgetContactRecordsInfo.udo_BranchOfService;
                    //sourceEntity["udo_gender"] = response.UDOgetContactRecordsInfo.udo_gender;


                    //Not used on form
                    // sourceEntity["udo_phone1type"] = response.UDOgetContactRecordsInfo.udo_Phone1Type;
                    //sourceEntity["udo_phone2type"] = response.UDOgetContactRecordsInfo.udo_Phone2Type;
                    //sourceEntity["telephone1"] = response.UDOgetContactRecordsInfo.Telephone1;
                    // sourceEntity["birthdate"] = response.UDOgetContactRecordsInfo.udo_DateofBirth.ToDateTime();








                    //fields updated in idproofcreatepoststaterunner
                    //sourceEntity["udo_ebenefitstatus"] = response.UDOgetContactRecordsInfo.udo_ebenefitsStatus;
                    //if (response.UDOgetContactRecordsInfo.udo_hasebenefits > 0)
                    //{
                    //    sourceEntity["udo_hasebenefitsaccount"] = new OptionSetValue(response.UDOgetContactRecordsInfo.udo_hasebenefits);
                    //}

                    //flashes separateg long ago
                    //sourceEntity["udo_flashes"] = string.IsNullOrWhiteSpace(vetFlashes) ? null : vetFlashes;






                    //updated snapshot code
                    //sourceEntity["udo_fiduciaryappointed"] = response.UDOgetContactRecordsInfo.udo_FiduciaryAppointed;
                    //sourceEntity["udo_poa"] = response.UDOgetContactRecordsInfo.udo_POA;


                    //updated during snapshot

                    //sourceEntity["udo_dateofdeath"] = response.UDOgetContactRecordsInfo.udo_DateofDeath;
                    //sourceEntity["udo_charactorofdischarge"] = response.UDOgetContactRecordsInfo.udo_charofdisccode;
                    //sourceEntity["udo_folderlocation"] = response.UDOgetContactRecordsInfo.udo_FolderLocation;


                    //fid data - moved to get with POA and FID tab
                    //sourceEntity["udo_cfid2personorgname"] = response.UDOgetContactRecordsInfo.udo_cfid2PersonOrgName;
                    //sourceEntity["udo_cfid2personorganizationname"] = response.UDOgetContactRecordsInfo.udo_personOrganizationName;
                    //if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cfidBeginDate))
                    //{
                    //    sourceEntity["udo_cfidbegindate"] = response.UDOgetContactRecordsInfo.udo_cfidBeginDate.ToDateTime();
                    //}
                    //if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cfidEndDate))
                    //{
                    //    sourceEntity["udo_cfidenddate"] = response.UDOgetContactRecordsInfo.udo_cfidEndDate.ToDateTime();
                    //}
                    //if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cfidEventDate))
                    //{
                    //    sourceEntity["udo_cfideventdate"] = response.UDOgetContactRecordsInfo.udo_cfidEventDate.ToDateTime();
                    //}
                    //if (response.UDOgetContactRecordsInfo.udo_cfidHCProviderReleaseSpecified)
                    //{
                    //    if (response.UDOgetContactRecordsInfo.udo_cfidHCProviderRelease)
                    //    {
                    //        sourceEntity["udo_cfidhcproviderreleasedrp"] = new OptionSetValue(752280000);
                    //    }
                    //    else
                    //    {
                    //        sourceEntity["udo_cfidhcproviderreleasedrp"] = new OptionSetValue(752280001);
                    //    }
                    //}
                    //if (response.UDOgetContactRecordsInfo.udo_cfidPersonorOrgSpecified)
                    //{
                    //    if (response.UDOgetContactRecordsInfo.udo_cfidPersonorOrg)
                    //    {
                    //        sourceEntity["udo_cfidpersonororgdrp"] = new OptionSetValue(752280000);
                    //    }
                    //    else
                    //    {
                    //        sourceEntity["udo_cfidpersonororgdrp"] = new OptionSetValue(752280001);
                    //    }
                    //}
                    //if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cfidJrnDate))
                    //{
                    //    sourceEntity["udo_cfidjrndate"] = response.UDOgetContactRecordsInfo.udo_cfidJrnDate.ToDateTime();
                    //}

                    ///sourceEntity["udo_cfidjrnlocid"] = response.UDOgetContactRecordsInfo.udo_cfidJrnLocID;
                    //sourceEntity["udo_cfidjrnobjid"] = response.UDOgetContactRecordsInfo.udo_cfidJrnObjID;
                    //sourceEntity["udo_cfidjrnstatustype"] = response.UDOgetContactRecordsInfo.udo_cfidJrnStatusType;
                    //sourceEntity["udo_cfidpersonorgattn"] = response.UDOgetContactRecordsInfo.udo_cfidPersonOrgAttn;
                    //sourceEntity["udo_cfidpersonorgcode"] = response.UDOgetContactRecordsInfo.udo_cfidPersonOrgCode;
                    //sourceEntity["udo_cfidpersonorgname"] = response.UDOgetContactRecordsInfo.udo_cfidPersonOrgName;
                    // sourceEntity["udo_cfidpersonorgparticipantid"] = response.UDOgetContactRecordsInfo.udo_cfidPersonOrgParticipantID;

                    //sourceEntity["udo_cfidprepositionalphrase"] = response.UDOgetContactRecordsInfo.udo_cfidPrepositionalPhrase;
                    //sourceEntity["udo_cfidratename"] = response.UDOgetContactRecordsInfo.udo_cfidRateName;
                    //sourceEntity["udo_cfidrelationship"] = response.UDOgetContactRecordsInfo.udo_cfidRelationship;
                    //sourceEntity["udo_cfidstatus"] = response.UDOgetContactRecordsInfo.udo_cfidStatus;
                    //sourceEntity["udo_cfidtempcustodian"] = response.UDOgetContactRecordsInfo.udo_cfidTempCustodian;
                    //sourceEntity["udo_cfidvetptcpntid"] = response.UDOgetContactRecordsInfo.udo_cfidVetPtcpntID;

                    //current POA data

                    //tracer.Trace("Current POA End Date: " + response.UDOgetContactRecordsInfo.udo_cpoaEndDate);
                    //Trace("Current POA End Date: " + response.UDOgetContactRecordsInfo.udo_cpoaEndDate);

                    //if (string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cpoaEndDate))
                    //{
                    //sourceEntity["udo_cpoaenddate"] = null;

                    //if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cpoaBeginDate))
                    //{
                    //    sourceEntity["udo_cpoabegindate"] = response.UDOgetContactRecordsInfo.udo_cpoaBeginDate.ToDateTime();
                    //}
                    //if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cpoaEventDate))
                    //{
                    //    //  //Logger.WriteDebugMessage("udo_cpoaEventDate:" + response.UDOgetContactRecordsInfo.udo_cpoaEventDate); 
                    //    sourceEntity["udo_cpoaeventdate"] = response.UDOgetContactRecordsInfo.udo_cpoaEventDate.ToDateTime();
                    //}
                    //if (response.UDOgetContactRecordsInfo.udo_cpoaHCProviderReleaseSpecified)
                    //{
                    //    if (response.UDOgetContactRecordsInfo.udo_cpoaHCProviderRelease)
                    //    {
                    //        sourceEntity["udo_cpoahcproviderreleasedrp"] = new OptionSetValue(752280000);
                    //    }
                    //    else
                    //    {
                    //        sourceEntity["udo_cpoahcproviderreleasedrp"] = new OptionSetValue(752280001);
                    //    }
                    //}
                    //if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cpoaJrnDate))
                    //{
                    //    sourceEntity["udo_cpoajrndate"] = response.UDOgetContactRecordsInfo.udo_cpoaJrnDate.ToDateTime();
                    //}
                    //  //Logger.WriteDebugMessage("udo_cpoaPersonOrOrgSpecified: " + response.UDOgetContactRecordsInfo.udo_cpoaPersonOrOrgSpecified);
                    //if (response.UDOgetContactRecordsInfo.udo_cpoaPersonOrOrgSpecified)
                    //{
                    //
                    // //Logger.WriteDebugMessage("udo_cpoaPersonOrOrg" + response.UDOgetContactRecordsInfo.udo_cpoaPersonOrOrg);
                    //    if (response.UDOgetContactRecordsInfo.udo_cpoaPersonOrOrg)
                    //    {
                    //        sourceEntity["udo_cpoapersonororgdrp"] = new OptionSetValue(752280000);
                    //    }
                    //    else
                    //    {
                    //        sourceEntity["udo_cpoapersonororgdrp"] = new OptionSetValue(752280001);
                    //    }
                    // }

                    //sourceEntity["udo_cpoajrnlocid"] = response.UDOgetContactRecordsInfo.udo_cpoaJrnLocID;
                    //sourceEntity["udo_cpoajrnobjid"] = response.UDOgetContactRecordsInfo.udo_cpoaJrnObjID;
                    //sourceEntity["udo_cpoajrnstatustype"] = response.UDOgetContactRecordsInfo.udo_cpoaJrnStatusType;
                    //sourceEntity["udo_cpoajrnuserid"] = response.UDOgetContactRecordsInfo.udo_cpoaJrnUserID;
                    //sourceEntity["udo_cpoaorgpersonname"] = response.UDOgetContactRecordsInfo.udo_cpoaOrgPersonName;
                    //sourceEntity["udo_cpoapersonorgattn"] = response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgAttn;
                    //sourceEntity["udo_cpoapersonorgcode"] = response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgCode;
                    //sourceEntity["udo_cpoapersonorgname"] = response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgName;
                    //sourceEntity["udo_cpoapersonorgparticipantid"] = response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgParticipantID;

                    //sourceEntity["udo_cpoaprepositionalphrase"] = response.UDOgetContactRecordsInfo.udo_cpoaPrepositionalPhrase;
                    //sourceEntity["udo_cpoaratename"] = response.UDOgetContactRecordsInfo.udo_cpoaRateName;
                    //sourceEntity["udo_cpoarelationship"] = response.UDOgetContactRecordsInfo.udo_cpoaRelationship;
                    //sourceEntity["udo_cpoastatus"] = response.UDOgetContactRecordsInfo.udo_cpoaStatus;
                    //sourceEntity["udo_cpoatempcustodian"] = response.UDOgetContactRecordsInfo.udo_cpoaTempCustodian;
                    //sourceEntity["udo_cpoavetptcptid"] = response.UDOgetContactRecordsInfo.udo_cpoaVetPtcptID;
                    //}
                    //else
                    //{
                    //    sourceEntity["udo_cpoaenddate"] = response.UDOgetContactRecordsInfo.udo_cpoaEndDate.ToDateTime();
                    //    #region clean-up poa fields
                    //sourceEntity["udo_poa"] = null;
                    //sourceEntity["udo_cpoabegindate"] = null;
                    //sourceEntity["udo_cpoaeventdate"] = null;
                    //sourceEntity["udo_cpoahcproviderreleasedrp"] = null;
                    //sourceEntity["udo_cpoajrndate"] = null;
                    //sourceEntity["udo_cpoapersonororgdrp"] = null;
                    //sourceEntity["udo_cpoajrnlocid"] = null;
                    //sourceEntity["udo_cpoajrnobjid"] = null;
                    //sourceEntity["udo_cpoajrnstatustype"] = null;
                    //sourceEntity["udo_cpoajrnuserid"] = null;
                    //sourceEntity["udo_cpoaorgpersonname"] = null;
                    //sourceEntity["udo_cpoapersonorgattn"] = null;
                    //sourceEntity["udo_cpoapersonorgcode"] = null;
                    //sourceEntity["udo_cpoapersonorgname"] = null;
                    //sourceEntity["udo_cpoapersonorgparticipantid"] = null;
                    //sourceEntity["udo_cpoaprepositionalphrase"] = null;
                    //sourceEntity["udo_cpoaratename"] = null;
                    //sourceEntity["udo_cpoarelationship"] = null;
                    //sourceEntity["udo_cpoastatus"] = null;
                    //sourceEntity["udo_cpoatempcustodian"] = null;
                    //sourceEntity["udo_cpoavetptcptid"] = null;
                    //#endregion
                    //
                    //CreatePastPOA(request, response);
                    //}


                    //ColumnSet userCols = new ColumnSet("crme_ihfrom", "crme_ihto", "crme_ihsource");
                    //thisUser = OrganizationService.Retrieve("systemuser", PluginExecutionContext.InitiatingUserId, userCols);

                    //if (thisUser.Attributes.Contains("crme_ihfrom"))
                    //{
                    //    sourceEntity["crme_ihfrom"] = (DateTime)thisUser["crme_ihfrom"];
                    //}
                    //if (thisUser.Attributes.Contains("crme_ihto"))
                    //{
                    //    sourceEntity["crme_ihto"] = (DateTime)thisUser["crme_ihto"];
                    //}
                    //if (thisUser.Attributes.Contains("crme_ihsource"))
                    //{
                    //    sourceEntity["crme_ihlobs"] = thisUser["crme_ihsource"].ToString();
                    //}
                }
            }
            #endregion MapResponse

            return sourceEntity;
        }

        private void CreatePastPOA(UDOgetContactRecordsRequest request, UDOgetContactRecordsResponse response)
        {
            try
            {
                var poaInfo = response.UDOgetContactRecordsInfo;
                var isAlreadyCreatedFlag = isAlreadyCreated(request.RelatedParentId, poaInfo.udo_cpoaEndDate);
                tracer.Trace("isAlreadyCreated: " + isAlreadyCreatedFlag);
                Trace("isAlreadyCreated: " + isAlreadyCreatedFlag);
                //Checked the record is existing or not by end date.
                if (!isAlreadyCreatedFlag)
                {
                    Entity thisNewEntity = new Entity();
                    thisNewEntity.LogicalName = "udo_pastpoa";

                    #region field mapping
                    if (request.ownerId != System.Guid.Empty)
                    {
                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                    }

                    if (poaInfo.udo_cpoaVetPtcptID != string.Empty)
                    {
                        thisNewEntity["udo_vetptcpntid"] = poaInfo.udo_cpoaVetPtcptID;
                    }
                    if (poaInfo.udo_cpoaTempCustodian != string.Empty)
                    {
                        thisNewEntity["udo_tempcustodian"] = poaInfo.udo_cpoaTempCustodian;
                    }
                    if (poaInfo.udo_cpoaRelationship != string.Empty)
                    {
                        thisNewEntity["udo_relationship"] = poaInfo.udo_cpoaRelationship;
                    }
                    if (poaInfo.udo_cpoaRateName != string.Empty)
                    {
                        thisNewEntity["udo_rate"] = poaInfo.udo_cpoaRateName;
                    }
                    if (poaInfo.udo_cpoaPrepositionalPhrase != string.Empty)
                    {
                        thisNewEntity["udo_phase"] = poaInfo.udo_cpoaPrepositionalPhrase;
                    }
                    if (poaInfo.udo_cpoaPersonOrgParticipantID != string.Empty)
                    {
                        thisNewEntity["udo_personorgptcpnt"] = poaInfo.udo_cpoaPersonOrgParticipantID;
                    }
                    if (poaInfo.udo_POA != string.Empty)
                    {
                        thisNewEntity["udo_name"] = poaInfo.udo_POA;
                    }
                    if (poaInfo.udo_cpoaPersonOrgName != string.Empty)
                    {
                        thisNewEntity["udo_personorgname"] = poaInfo.udo_cpoaPersonOrgName;
                    }
                    if (poaInfo.udo_cpoaPersonOrgCode != string.Empty)
                    {
                        thisNewEntity["udo_personorg"] = poaInfo.udo_cpoaPersonOrgCode;
                    }
                    if (poaInfo.udo_cpoaJrnUserID != string.Empty)
                    {
                        thisNewEntity["udo_jrnuser"] = poaInfo.udo_cpoaJrnUserID;
                    }
                    if (poaInfo.udo_cpoaJrnStatusType != string.Empty)
                    {
                        thisNewEntity["udo_jrnstatus"] = poaInfo.udo_cpoaJrnStatusType;
                    }
                    if (poaInfo.udo_cpoaJrnObjID != string.Empty)
                    {
                        thisNewEntity["udo_jrnobj"] = poaInfo.udo_cpoaJrnObjID;
                    }
                    if (poaInfo.udo_cpoaJrnLocID != string.Empty)
                    {
                        thisNewEntity["udo_jrnloc"] = poaInfo.udo_cpoaJrnLocID;
                    }
                    if (poaInfo.udo_cpoaJrnDate != string.Empty)
                    {
                        DateTime newDateTime;
                        if (DateTime.TryParse(poaInfo.udo_cpoaJrnDate, out newDateTime))
                        {
                            thisNewEntity["udo_jrndate"] = newDateTime;
                        }
                    }
                    if (poaInfo.udo_cpoaHCProviderReleaseSpecified)
                    {
                        thisNewEntity["udo_hcproviderrelease"] = poaInfo.udo_cpoaHCProviderRelease;
                    }
                    if (poaInfo.udo_cpoaEventDate != string.Empty)
                    {
                        DateTime newDateTime;
                        if (DateTime.TryParse(poaInfo.udo_cpoaEventDate, out newDateTime))
                        {
                            thisNewEntity["udo_eventdate"] = newDateTime;
                        }
                    }
                    if (poaInfo.udo_cpoaEndDate != string.Empty)
                    {
                        DateTime newDateTime;
                        if (DateTime.TryParse(poaInfo.udo_cpoaEndDate, out newDateTime))
                        {
                            thisNewEntity["udo_enddate"] = newDateTime;
                        }
                    }
                    if (poaInfo.udo_cpoaBeginDate != string.Empty)
                    {
                        DateTime newDateTime;
                        if (DateTime.TryParse(poaInfo.udo_cpoaBeginDate, out newDateTime))
                        {
                            thisNewEntity["udo_begindate"] = newDateTime;
                        }
                    }
                    if (poaInfo.udo_cpoaPersonOrgAttn != string.Empty)
                    {
                        thisNewEntity["udo_attn"] = poaInfo.udo_cpoaPersonOrgAttn;
                    }
                    if (request.RelatedParentId != Guid.Empty)
                    {
                        thisNewEntity["udo_veteranid"] = new EntityReference("contact", request.RelatedParentId);
                    }

                    thisNewEntity["udo_currentpoasrecord"] = true;

                    #endregion

                    var pastPoaGuid = OrganizationService.Create(thisNewEntity);

                    tracer.Trace("Past POA is created with current POA informations. Created record ID: " + pastPoaGuid.ToString());
                    Trace("Past POA is created with current POA informations. Created record ID: " + pastPoaGuid.ToString());
                }
            }
            catch (Exception ex)
            {
                PluginError = true;
                throw new InvalidPluginExecutionException("Unable to create Past POA data due to: {0}".Replace("{0}", ex.Message));
            }
        }

        private bool isAlreadyCreated(Guid veteranId, string endDate)
        {
            string fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='udo_pastpoa'>
                                    <attribute name='udo_pastpoaid' />
                                    <filter type='and'>
                                      <condition attribute='udo_veteranid' operator='eq' value='{0}' />
                                      <condition attribute='udo_enddate' operator='on' value='{1}' />
                                    </filter>
                                  </entity>
                                </fetch>";

            fetchXml = string.Format(fetchXml, veteranId.ToString(), endDate);

            tracer.Trace("isAlready Created - FetchXml: " + fetchXml);
            Trace("isAlready Created - FetchXml: " + fetchXml);

            EntityCollection entityCollection = OrganizationService.RetrieveMultiple(new FetchExpression(fetchXml));

            if (entityCollection.Entities != null && entityCollection.Entities.Count > 0)
                return true;

            return false;
        }

        private bool DidWeNeedData(Entity contact)
        {
            tracer.Trace("DidWeNeedData started");
            Trace("DidWeNeedData started");
            Logger.setMethod = "DidWeNeedData";

            var currentEntity = contact.ToEntity<UDO.Model.Contact>();

            if (string.IsNullOrEmpty(currentEntity.udo_FileNumber))
            {
                _responseMessage = "File Number not found. Cannot retrieve Veteran updates.";
                return false;
            }
            else
                _fileNumber = currentEntity.udo_FileNumber;

            //if (string.IsNullOrEmpty(currentEntity.udo_ParticipantId))
            //{
            //    _responseMessage = "Participant ID not found. Cannot retrieve Veteran updates.";
            //    return false;
            //}
            //else
            _pid = currentEntity.udo_ParticipantId;

            if (!string.IsNullOrEmpty(currentEntity.udo_EDIPI))
                _edipi = currentEntity.udo_EDIPI;


            if (currentEntity.Attributes.Contains("ownerid"))
            {
                EntityReference owner = (EntityReference)currentEntity["ownerid"];
                _ownerId = owner.Id;
                _ownerType = owner.LogicalName;
            }

            return true;
        }

        private Entity GetTargetContact()
        {
            var columns = new ColumnSet("udo_filenumber",
                                        "udo_participantid",
                                        "udo_edipi",
                                        "ownerid",
                                        "udo_ssn",
                                        "udo_gender",
                                        "firstname",
                                        "middlename",
                                        "lastname",
                                        "udo_folderlocation",
                                        "udo_ebenefitstatus",
                                        "udo_hasebenefitsaccount",
                                        "va_stationofjurisdictionid",
                                        "udo_flashes",
                                        "emailaddress1",
                                        "telephone1",
                                        "udo_birthdatestring",
                                        "birthdate",
                                        "udo_dateofdeath",
                                        "udo_causeofdeath",
                                        "udo_releasedactivedutydate",
                                        "udo_charactorofdischarge",
                                        "udo_branchofservice",
                                        "udo_fiduciaryappointed",
                                        "udo_poa",
                                        "udo_folderlocation",
                                        "udo_cfid2personorgname",
                                        "udo_cfid2personorganizationname",
                                        "udo_cfidbegindate",
                                        "udo_cfidenddate",
                                        "udo_cfideventdate",
                                        "udo_cfidhcproviderreleasedrp",
                                        "udo_cfidpersonororgdrp",
                                        "udo_cfidjrndate",
                                        "udo_cfidjrnlocid",
                                        "udo_cfidjrnobjid",
                                        "udo_cfidjrnstatustype",
                                        "udo_cfidpersonorgattn",
                                        "udo_cfidpersonorgcode",
                                        "udo_cfidpersonorgname",
                                        "udo_cfidpersonorgparticipantid",
                                        "udo_cfidprepositionalphrase",
                                        "udo_cfidratename",
                                        "udo_cfidrelationship",
                                        "udo_cfidstatus",
                                        "udo_cfidtempcustodian",
                                        "udo_cfidvetptcpntid",
                                        "udo_cpoabegindate",
                                        "udo_cpoaenddate",
                                        "udo_cpoaeventdate",
                                        "udo_cpoahcproviderreleasedrp",
                                        "udo_cpoajrndate",
                                        "udo_cpoapersonororgdrp",
                                        "udo_cpoajrnlocid",
                                        "udo_cpoajrnobjid",
                                        "udo_cpoajrnstatustype",
                                        "udo_cpoajrnuserid",
                                        "udo_cpoaorgpersonname",
                                        "udo_cpoapersonorgattn",
                                        "udo_cpoapersonorgcode",
                                        "udo_cpoapersonorgname",
                                        "udo_cpoapersonorgparticipantid",
                                        "udo_cpoaprepositionalphrase",
                                        "udo_cpoaratename",
                                        "udo_cpoarelationship",
                                        "udo_cpoastatus",
                                        "udo_cpoatempcustodian",
                                        "udo_cpoavetptcptid",
                                        "udo_phone1type",
                                        "udo_phonenumber1",
                                        "udo_phone2type",
                                        "udo_phonenumber2",
                                        "crme_ihfrom",
                                        "crme_ihto",
                                        "crme_ihlobs");

            return OrganizationService.Retrieve(UDO.Model.Contact.EntityLogicalName, _contactId, columns);
        }

        private Guid getSOJId(string stationCode)
        {
            try
            {
                Logger.setMethod = "getSOJId";

                using (var xrm = new UDOContext(OrganizationService))
                {
                    var getParent = from awd in xrm.va_regionalofficeSet
                                    where awd.va_Code == stationCode
                                    select new
                                    {
                                        awd.va_regionalofficeId,
                                        awd.va_name

                                    };
                    foreach (var awd in getParent)
                    {
                        ////Logger.WriteDebugMessage("Name:" + awd.va_name);
                        _sojName = awd.va_name;

                        if (awd.va_regionalofficeId != null)
                        {
                            return awd.va_regionalofficeId.Value;
                        }

                    }
                }
                ////Logger.WriteDebugMessage("Ending didWeNeedData Method");
                Logger.setMethod = "Execute";

                return Guid.Empty;
            }
            catch (Exception ex)
            {
                PluginError = true;
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
    }

    static class DateTimeHelper
    {
        public static DateTime? ToDateTime(this string value)
        {
            DateTime? result = null;
            if (value != null)
            {
                DateTime.TryParse(value, out DateTime res);
                result = res;
            }
            return result;
        }
    }
}
