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
using System.Threading;
using UDO.LOB.Core;
using UDO.LOB.Contact.Messages;
using CustomActions.Plugins.Entities.Contact;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.Contact.Messages;

namespace CustomActions.Plugins.Entities.Dependents
{
    public class UDOGetDependentUpdatesRunner : UDOActionRunner
    {
        protected Guid _dependentId = new Guid();
        protected Guid _veteranId = new Guid();
        protected Guid _ownerId = new Guid();
        protected string _ownerType = string.Empty;
        protected string _pid = string.Empty;
        protected string _fileNumber = string.Empty;
        protected string _sojName = string.Empty;
        protected UDOHeaderInfo _headerInfo;

        public UDOGetDependentUpdatesRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_contactlogtimer";
            _logSoapField = "udo_contactlogsoap";
            _debugField = "udo_dependent";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_contacttimeout";
            _validEntities = new string[] { "udo_dependant" };
        }

        public override void DoAction()
        {
            _method = "DoAction";
            _dependentId = Parent.Id;

            var targetDependent = GetTargetDependent();

            if (!DidWeNeedData(targetDependent))
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();

            var sourceDependent = GetSourceDependent(targetDependent);

            if (sourceDependent.Attributes.Count == 0)
                return;

            if (targetDependent.Contains("udo_ssn"))
                targetDependent.Attributes.Remove("udo_ssn");

            if (targetDependent.Contains("udo_filenumber"))
                targetDependent.Attributes.Remove("udo_filenumber");

            UpdateTarget(targetDependent, sourceDependent);
        }

        private void UpdateTarget(Entity target, Entity source)
        {
            tracer.Trace("UpdateTarget started");
            Logger.setMethod = "UpdateTarget";

            var updated = false;
            var updatedDependent = new Entity(UDO.Model.udo_dependant.EntityLogicalName);

            foreach (var attribute in source.Attributes)
            {
                var attName = attribute.Key;
                var sourceValue = attribute.Value;

                if (target.Contains(attName))
                {
                    var targetAttValue = target[attName];

                    if (sourceValue != null && sourceValue.GetType().Equals(typeof(DateTime)))
                    {
                        var sourceDate = Convert.ToDateTime(sourceValue);
                        var targetDate = Convert.ToDateTime(targetAttValue);

                        if (sourceDate == null || targetDate == null)
                        {
                            updatedDependent.Attributes.Add(attName, sourceValue);
                            continue;
                        }

                        var result = DateTime.Compare(sourceDate.Date, targetDate.Date);

                        if (result != 0)
                            updatedDependent.Attributes.Add(attName, sourceValue);
                    }
                    else if (!targetAttValue.Equals(sourceValue))
                        updatedDependent.Attributes.Add(attName, sourceValue);
                }
                else if (sourceValue != null)
                    updatedDependent.Attributes.Add(attName, sourceValue);
            }

            if (updatedDependent.Attributes.Count > 0)
            {
                updatedDependent.Id = target.Id;
                ElevatedOrganizationService.Update(updatedDependent);
                updated = true;
            }

            EntityUpdate = updated;
        }

        private Entity GetSourceDependent(Entity targetEntity)
        {
            tracer.Trace("GetSourceDependent started");
            Logger.setMethod = "GetSourceDependent";

            var sourceEntity = new udo_dependant();

            _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            var request = new UDOgetContactRecordsRequest()
            {
                MessageId = PluginExecutionContext.CorrelationId.ToString(),
                fileNumber = _fileNumber,
                ptcpntVetId = _pid,
                ptcpntBeneId = _pid,
                ptpcntRecipId = _pid,
                Debug = _debug,
                RelatedParentEntityName = "contact",
                RelatedParentFieldName = "udo_contactid",
                RelatedParentId = _dependentId,
                LogSoap = _logSoap,
                LogTiming = _logTimer,
                ownerId = _ownerId,
                ownerType = _ownerType,
                UserId = PluginExecutionContext.InitiatingUserId,
                OrganizationName = PluginExecutionContext.OrganizationName,
                VeteranId = PluginExecutionContext.PrimaryEntityId
            };
            request.LegacyServiceHeaderInfo = _headerInfo;

            ////Logger.WriteDebugMessage("UDOgetContactRecordsRequest Request Created");
            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = "Contact.Plugins.PostDependentRetrieveRunner"
            };

            tracer.Trace("calling UDOgetContactRecordsRequest");
            var response = Utility.SendReceive<UDOgetContactRecordsResponse>(_uri, "UDOgetContactRecordsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOgetContactRecordsRequest");

            if (response != null)
            {
                if (response.UDOgetContactRecordsInfo != null)
                {
                    sourceEntity["udo_first"] = response.UDOgetContactRecordsInfo.FirstName;
                    sourceEntity["udo_middle"] = response.UDOgetContactRecordsInfo.MiddleName;
                    sourceEntity["udo_last"] = response.UDOgetContactRecordsInfo.LastName;
                    // sourceEntity["udo_folderlocation"] = response.UDOgetContactRecordsInfo.udo_FolderLocation;
                    //var vetFlashes = "";
                    //if (response.UDOgetContactRecordsInfo.udo_Flashes != null)
                    //{
                    //    if (response.UDOgetContactRecordsInfo.udo_Flashes.Length > 100)
                    //    {
                    //        vetFlashes = "More ... :" + response.UDOgetContactRecordsInfo.udo_Flashes.Substring(0, 90);
                    //    }
                    //    else
                    //    {
                    //        vetFlashes = response.UDOgetContactRecordsInfo.udo_Flashes;
                    //    }

                    //}
                    // sourceEntity["udo_flashes"] = vetFlashes;
                    // sourceEntity["emailaddress1"] = response.UDOgetContactRecordsInfo.EMailAddress1;
                    // sourceEntity["telephone1"] = response.UDOgetContactRecordsInfo.Telephone1;
                    if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_BirthDateString))
                    {
                        // //Logger.WriteDebugMessage("response.UDOgetContactRecordsInfo.udo_BirthDateString:" + response.UDOgetContactRecordsInfo.udo_BirthDateString);
                        DateTime newParsedDate;
                        if (DateTime.TryParse(response.UDOgetContactRecordsInfo.udo_BirthDateString, out newParsedDate))
                        {
                            newParsedDate = newParsedDate.AddHours(12);
                            //     sourceEntity["birthdate"] = newParsedDate;
                        }
                        //    //Logger.WriteDebugMessage("newParsedDate:" + newParsedDate);
                    }
                    //   sourceEntity["udo_dateofdeath"] = response.UDOgetContactRecordsInfo.udo_DateofDeath;
                    //   sourceEntity["udo_branchofservice"] = response.UDOgetContactRecordsInfo.udo_BranchOfService;
                    //    sourceEntity["udo_fiduciaryappointed"] = response.UDOgetContactRecordsInfo.udo_FiduciaryAppointed;
                    //     sourceEntity["udo_poa"] = response.UDOgetContactRecordsInfo.udo_POA;
                    //     sourceEntity["udo_folderlocation"] = response.UDOgetContactRecordsInfo.udo_FolderLocation;
                    //fid data

                    sourceEntity["udo_cfid2personorgname"] = response.UDOgetContactRecordsInfo.udo_cfid2PersonOrgName;
                    if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cfidBeginDate))
                    {
                        sourceEntity["udo_cfidbegindate"] = response.UDOgetContactRecordsInfo.udo_cfidBeginDate.ToDateTime();
                    }
                    if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cfidEndDate))
                    {
                        sourceEntity["udo_cfidenddate"] = response.UDOgetContactRecordsInfo.udo_cfidEndDate.ToDateTime(); 
                    }
                    if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cfidEventDate))
                    {
                        sourceEntity["udo_cfideventdate"] = response.UDOgetContactRecordsInfo.udo_cfidEventDate.ToDateTime();
                    }
                    if (response.UDOgetContactRecordsInfo.udo_cfidHCProviderReleaseSpecified)
                    {
                        if (response.UDOgetContactRecordsInfo.udo_cfidHCProviderRelease)
                        {
                            sourceEntity["udo_cfidhcproviderreleasedrp"] = new OptionSetValue(752280000);
                        }
                        else
                        {
                            sourceEntity["udo_cfidhcproviderreleasedrp"] = new OptionSetValue(752280001);
                        }
                    }
                    if (response.UDOgetContactRecordsInfo.udo_cfidPersonorOrgSpecified)
                    {
                        if (response.UDOgetContactRecordsInfo.udo_cfidPersonorOrg)
                        {
                            sourceEntity["udo_cfidpersonororgdrp"] = new OptionSetValue(752280000);
                        }
                        else
                        {
                            sourceEntity["udo_cfidpersonororgdrp"] = new OptionSetValue(752280001);
                        }
                    }
                    if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cfidJrnDate))
                    {
                        sourceEntity["udo_cfidjrndate"] = response.UDOgetContactRecordsInfo.udo_cfidJrnDate.ToDateTime();
                    }

                    sourceEntity["udo_cfidjrnlocid"] = response.UDOgetContactRecordsInfo.udo_cfidJrnLocID;
                    sourceEntity["udo_cfidjrnobjid"] = response.UDOgetContactRecordsInfo.udo_cfidJrnObjID;
                    sourceEntity["udo_cfidjrnstatustype"] = response.UDOgetContactRecordsInfo.udo_cfidJrnStatusType;
                    sourceEntity["udo_cfidpersonorgattn"] = response.UDOgetContactRecordsInfo.udo_cfidPersonOrgAttn;
                    sourceEntity["udo_cfidpersonorgcode"] = response.UDOgetContactRecordsInfo.udo_cfidPersonOrgCode;
                    sourceEntity["udo_cfidpersonorgname"] = response.UDOgetContactRecordsInfo.udo_cfidPersonOrgName;
                    sourceEntity["udo_cfidpersonororg"] = response.UDOgetContactRecordsInfo.udo_cfidPersonorOrg;
                    sourceEntity["udo_cfidpersonorgparticipantid"] = response.UDOgetContactRecordsInfo.udo_cfidPersonOrgParticipantID;

                    sourceEntity["udo_cfidprepositionalphrase"] = response.UDOgetContactRecordsInfo.udo_cfidPrepositionalPhrase;
                    sourceEntity["udo_cfidratename"] = response.UDOgetContactRecordsInfo.udo_cfidRateName;
                    sourceEntity["udo_cfidrelationship"] = response.UDOgetContactRecordsInfo.udo_cfidRelationship;
                    sourceEntity["udo_cfidstatus"] = response.UDOgetContactRecordsInfo.udo_cfidStatus;
                    sourceEntity["udo_cfidtempcustodian"] = response.UDOgetContactRecordsInfo.udo_cfidTempCustodian;
                    sourceEntity["udo_cfidvetptcpntid"] = response.UDOgetContactRecordsInfo.udo_cfidVetPtcpntID;
                    sourceEntity["udo_cfidhcproviderrelease"] = response.UDOgetContactRecordsInfo.udo_cfidHCProviderRelease;

                    //current POA data
                    if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cpoaBeginDate))
                    {
                        sourceEntity["udo_cpoabegindate"] = response.UDOgetContactRecordsInfo.udo_cpoaBeginDate.ToDateTime();
                    }
                    if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cpoaEndDate))
                    {
                        sourceEntity["udo_cpoaenddate"] = response.UDOgetContactRecordsInfo.udo_cpoaEndDate.ToDateTime();
                    }
                    if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cpoaEventDate))
                    {
                        //  //Logger.WriteDebugMessage("udo_cpoaEventDate:" + response.UDOgetContactRecordsInfo.udo_cpoaEventDate); 
                        sourceEntity["udo_cpoaeventdate"] = response.UDOgetContactRecordsInfo.udo_cpoaEventDate.ToDateTime();
                    }
                    if (response.UDOgetContactRecordsInfo.udo_cpoaHCProviderReleaseSpecified)
                    {
                        if (response.UDOgetContactRecordsInfo.udo_cpoaHCProviderRelease)
                        {
                            sourceEntity["udo_cpoahcproviderreleasedrp"] = new OptionSetValue(752280000);
                        }
                        else
                        {
                            sourceEntity["udo_cpoahcproviderreleasedrp"] = new OptionSetValue(752280001);
                        }
                    }
                    if (!string.IsNullOrEmpty(response.UDOgetContactRecordsInfo.udo_cpoaJrnDate))
                    {
                        sourceEntity["udo_cpoajrndate"] = response.UDOgetContactRecordsInfo.udo_cpoaJrnDate.ToDateTime();
                    }
                    //  //Logger.WriteDebugMessage("udo_cpoaPersonOrOrgSpecified: " + response.UDOgetContactRecordsInfo.udo_cpoaPersonOrOrgSpecified);
                    if (response.UDOgetContactRecordsInfo.udo_cpoaPersonOrOrgSpecified)
                    {

                        ////Logger.WriteDebugMessage("udo_cpoaPersonOrOrg" + response.UDOgetContactRecordsInfo.udo_cpoaPersonOrOrg);
                        if (response.UDOgetContactRecordsInfo.udo_cpoaPersonOrOrg)
                        {
                            sourceEntity["udo_cpoapersonororgdrp"] = new OptionSetValue(752280000);
                        }
                        else
                        {
                            sourceEntity["udo_cpoapersonororgdrp"] = new OptionSetValue(752280001);
                        }
                    }


                    sourceEntity["udo_cpoajrnlocid"] = response.UDOgetContactRecordsInfo.udo_cpoaJrnLocID;
                    sourceEntity["udo_cpoajrnobjid"] = response.UDOgetContactRecordsInfo.udo_cpoaJrnObjID;
                    sourceEntity["udo_cpoajrnstatustype"] = response.UDOgetContactRecordsInfo.udo_cpoaJrnStatusType;
                    sourceEntity["udo_cpoajrnuserid"] = response.UDOgetContactRecordsInfo.udo_cpoaJrnUserID;
                    sourceEntity["udo_cpoaorgpersonname"] = response.UDOgetContactRecordsInfo.udo_cpoaOrgPersonName;
                    sourceEntity["udo_cpoapersonorgattn"] = response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgAttn;
                    sourceEntity["udo_cpoapersonorgcode"] = response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgCode;
                    sourceEntity["udo_cpoapersonorgname"] = response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgName;
                    sourceEntity["udo_cpoapersonorgparticipantid"] = response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgParticipantID;

                    sourceEntity["udo_cpoaprepositionalphrase"] = response.UDOgetContactRecordsInfo.udo_cpoaPrepositionalPhrase;
                    sourceEntity["udo_cpoaratename"] = response.UDOgetContactRecordsInfo.udo_cpoaRateName;
                    sourceEntity["udo_cpoarelationship"] = response.UDOgetContactRecordsInfo.udo_cpoaRelationship;
                    sourceEntity["udo_cpoastatus"] = response.UDOgetContactRecordsInfo.udo_cpoaStatus;
                    sourceEntity["udo_cpoatempcustodian"] = response.UDOgetContactRecordsInfo.udo_cpoaTempCustodian;
                    sourceEntity["udo_cpoavetptcptid"] = response.UDOgetContactRecordsInfo.udo_cpoaVetPtcptID;

                    sourceEntity["udo_phone1type"] = response.UDOgetContactRecordsInfo.udo_Phone1Type;
                    sourceEntity["udo_phonenumber1"] = response.UDOgetContactRecordsInfo.udo_PhoneNumber1;
                    sourceEntity["udo_phone2type"] = response.UDOgetContactRecordsInfo.udo_Phone2Type;
                    sourceEntity["udo_phonenumber2"] = response.UDOgetContactRecordsInfo.udo_PhoneNumber2;
                }
            }


            return sourceEntity;
        }

        private Entity GetTargetDependent()
        {

            tracer.Trace("GetTargetDependent started");
            Logger.setMethod = "GetTargetDependent";
            var columns = new ColumnSet("udo_awardcomplete",
                                        "udo_ptcpntid",
                                        "udo_ssn",
                                        "udo_veteranid",
                                        "ownerid",
                                        "udo_first",
                                        "udo_middle",
                                        "udo_last",
                                        "udo_cfid2personorgname",
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
                                        "udo_cfidpersonororg",
                                        "udo_cfidpersonorgparticipantid",
                                        "udo_cfidprepositionalphrase",
                                        "udo_cfidratename",
                                        "udo_cfidrelationship",
                                        "udo_cfidstatus",
                                        "udo_cfidtempcustodian",
                                        "udo_cfidvetptcpntid",
                                        "udo_cfidhcproviderrelease",
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
                                        "udo_phonenumber2");

            return OrganizationService.Retrieve(UDO.Model.udo_dependant.EntityLogicalName,_dependentId, columns);
        }

        private bool DidWeNeedData(Entity contact)
        {
            tracer.Trace("DidWeNeedData started");
            Logger.setMethod = "DidWeNeedData";

            var currentEntity = contact.ToEntity<UDO.Model.udo_dependant>();

            if (string.IsNullOrEmpty(currentEntity.udo_SSN))
            {
                _responseMessage = "SSN not found. Cannot retrieve Dependent updates.";
                tracer.Trace(_responseMessage);
                return false;
            }
            else
                _fileNumber = currentEntity.udo_SSN;

            if (string.IsNullOrEmpty(currentEntity.udo_PtcpntID))
            {
                _responseMessage = "Participant ID not found. Cannot retrieve Dependent updates.";
                return false;
            }
            else
                _pid = currentEntity.udo_PtcpntID;

            if (currentEntity.udo_VeteranId == null)
            {
                _responseMessage = "Veteran ID not found. Cannot retrieve Dependent updates.";
                return false;
            }
            else
                _veteranId = currentEntity.udo_VeteranId.Id;

            if (currentEntity.Attributes.Contains("ownerid"))
            {
                EntityReference owner = (EntityReference)currentEntity["ownerid"];
                _ownerId = owner.Id;
                _ownerType = owner.LogicalName;
            }

            return true;
        }
    }
}
