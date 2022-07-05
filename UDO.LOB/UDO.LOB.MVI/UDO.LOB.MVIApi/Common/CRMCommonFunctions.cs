using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
//using CRM007.CRM.SDK.Core;
//using CuttingEdge.Conditions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk;
//using VRM.Integration.UDO.MVI.Messages;
using System;
//using System.Security;
using System.Linq;
//using VRM.Integration.Mvi.PersonSearch.Messages;
//using VRM.Integration.Servicebus.Core;
//using Logger = VRM.Integration.Servicebus.Core.Logger;
//using VRM.Integration.Mvi.PersonSearch;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//using VRM.Integration.UDO.Common;
//using VRM.Integration.UDO.eMIS.Messages;

//using UDO.Model;
using UDO.LOB.MVI.Messages;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Extensions;

namespace UDO.LOB.MVI.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    public class CRMCommonFunctions
    {
        private static string UrlFormat;
        private string slCorpDB = string.Empty;
        private SecureString SSId;
        public string getslCorpDB
        {
            get { return slCorpDB; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="organizationName"></param>
        /// <param name="userId"></param>
        /// <param name="ssn"></param>
        /// <param name="edipi"></param>
        /// <param name="participantId"></param>
        /// <param name="recordSource"></param>
        /// <returns></returns>
        public Guid? TryGetCrmPerson(IOrganizationService conn, ref UDOSelectedPersonRequest selectedPersonRequest, string recordSource)
        {
            Guid? personId = Guid.Empty;
            var ownerId = Guid.Empty;
            var ownerName = string.Empty;
            var recSource = string.Empty;
            var firstName = string.Empty;
            var middleName = string.Empty;
            var lastName = string.Empty;
            var fileNumber = string.Empty;
            var phoneNumber = string.Empty;

            //var soc = string.Empty;
            var PID = string.Empty;
            var ICN = string.Empty;
            var Edipi = string.Empty;
            var DOB = string.Empty;
            var gender = string.Empty;
            var BOS = string.Empty;
            var sensitivityLevel = string.Empty;
            var found = false;
            //order of importance is :
            //ICN
            //EDIPI
            //PID
            //Filenumber
            Stopwatch txnTimer = Stopwatch.StartNew();
            Stopwatch EntireTimer = Stopwatch.StartNew();

            var fetchColumns =
                "<attribute name='contactid'/>" +
                "<attribute name='ownerid'/>" +
                "<attribute name='udo_veteransensitivitylevel'/>" +
                "<attribute name='udo_recordsource'/>" +
                "<attribute name='firstname'/>" +
                "<attribute name='middlename'/>" +
                "<attribute name='lastname'/>" +
                "<attribute name='udo_phonenumber1'/>" +
                "<attribute name='udo_filenumber'/>" +
                "<attribute name='udo_ssn'/>" +
                "<attribute name='udo_participantid'/>" +
                "<attribute name='udo_icn'/>" +
                "<attribute name='udo_edipi'/>" +
                "<attribute name='udo_birthdatestring'/>" +
                "<attribute name='udo_gender'/>" +
                "<attribute name='udo_branchofservice'/>";

            //add this check to see if we have a contact id, use it.
            if (selectedPersonRequest.ContactId != Guid.Empty)
            {
                var fetch = "<fetch><entity name='contact'>" + fetchColumns +
                     "<filter>" +
                     "<condition attribute='contactid' operator='eq' value='" +
                     selectedPersonRequest.ContactId + "'/>" +
                     "</filter></entity></fetch>";

                var result = conn.RetrieveMultiple(new FetchExpression(fetch));
                txnTimer.Stop();
                //LogHelper.LogTiming(selectedPersonRequest.OrganizationName, selectedPersonRequest.LogTiming, selectedPersonRequest.UserId, selectedPersonRequest.RelatedParentId, selectedPersonRequest.RelatedParentEntityName, selectedPersonRequest.RelatedParentFieldName, "GetCRMPerson 1", null, txnTimer.ElapsedMilliseconds);
                txnTimer.Restart();
                if (result != null & result.Entities.Count > 0)
                {
                    LogHelper.LogDebug(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, "TryGetCRMPerson, Execute", "Grabbed Contact based on ContactID", selectedPersonRequest.Debug);

                    var crmPerson = result.Entities[0];
                    recSource = crmPerson.GetAttributeValue<string>("udo_recordsource");
                    personId = crmPerson.Id;
                    ownerId = crmPerson.GetAttributeValue<EntityReference>("ownerid").Id;
                    ownerName = crmPerson.GetAttributeValue<EntityReference>("ownerid").Name;
                    firstName = crmPerson.GetAttributeValue<string>("firstname");
                    middleName = crmPerson.GetAttributeValue<string>("middlename");
                    lastName = crmPerson.GetAttributeValue<string>("lastname");
                    fileNumber = crmPerson.GetAttributeValue<string>("udo_filenumber");
                    // SSId = SecurityTools.ConvertToSecureString(crmPerson.GetAttributeValue<string>("udo_ssn"));
                    SSId = crmPerson.GetAttributeValue<string>("udo_ssn").ToSecureString();
                    PID = crmPerson.GetAttributeValue<string>("udo_participantid");
                    ICN = crmPerson.GetAttributeValue<string>("udo_icn");
                    Edipi = crmPerson.GetAttributeValue<string>("udo_edipi");
                    DOB = crmPerson.GetAttributeValue<string>("udo_birthdatestring");
                    gender = crmPerson.GetAttributeValue<string>("udo_gender");
                    BOS = crmPerson.GetAttributeValue<string>("udo_branchofservice");
                    phoneNumber = crmPerson.GetAttributeValue<string>("udo_phonenumber1");
                    found = true;

                    if (crmPerson.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel") != null)
                    {
                        sensitivityLevel = crmPerson.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel").Value.ToString();
                    }
                }
            
            }

            // try to find using ICN from MVI
            if (personId == Guid.Empty && (!string.IsNullOrEmpty(selectedPersonRequest.ICN)) && found == false)
            {
                var fetch = "<fetch><entity name='contact'>" + fetchColumns +
                    "<order attribute='createdon' descending='false' />" +
                    "<filter>" +
                    "<condition attribute='udo_icn' operator='eq' value='" +
                    selectedPersonRequest.ICN + "'/>" +
                    "</filter></entity></fetch>";

                var result = conn.RetrieveMultiple(new FetchExpression(fetch));
                txnTimer.Stop();
                ////LogHelper.LogTiming(selectedPersonRequest.OrganizationName, selectedPersonRequest.LogTiming, selectedPersonRequest.UserId, selectedPersonRequest.RelatedParentId, selectedPersonRequest.RelatedParentEntityName, selectedPersonRequest.RelatedParentFieldName, "GetCRMPerson 1", null, txnTimer.ElapsedMilliseconds);
                txnTimer.Restart();
                if (result != null & result.Entities.Count > 0)
                {
                    var crmPerson = HandleSearchResults(result, selectedPersonRequest);
                    if (crmPerson != null)
                    {
                        recSource = crmPerson.GetAttributeValue<string>("udo_recordsource");
                        personId = crmPerson.Id;
                        ownerId = crmPerson.GetAttributeValue<EntityReference>("ownerid").Id;
                        ownerName = crmPerson.GetAttributeValue<EntityReference>("ownerid").Name;
                        firstName = crmPerson.GetAttributeValue<string>("firstname");
                        middleName = crmPerson.GetAttributeValue<string>("middlename");
                        lastName = crmPerson.GetAttributeValue<string>("lastname");
                        fileNumber = crmPerson.GetAttributeValue<string>("udo_filenumber");
                        // SSId = SecurityTools.ConvertToSecureString(crmPerson.GetAttributeValue<string>("udo_ssn"));
                        SSId = crmPerson.GetAttributeValue<string>("udo_ssn").ToSecureString();
                        PID = crmPerson.GetAttributeValue<string>("udo_participantid");
                        ICN = crmPerson.GetAttributeValue<string>("udo_icn");
                        Edipi = crmPerson.GetAttributeValue<string>("udo_edipi");
                        DOB = crmPerson.GetAttributeValue<string>("udo_birthdatestring");
                        gender = crmPerson.GetAttributeValue<string>("udo_gender");
                        BOS = crmPerson.GetAttributeValue<string>("udo_branchofservice");
                        phoneNumber = crmPerson.GetAttributeValue<string>("udo_phonenumber1");
                        found = true;

                        if (crmPerson.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel") != null)
                        {
                            sensitivityLevel = crmPerson.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel").Value.ToString();
                        }
                    }
                }
            }

            // try to find using edipi from MVI
            if (personId == Guid.Empty && (!string.IsNullOrEmpty(selectedPersonRequest.Edipi)) && found == false)
            {
                var fetch = "<fetch><entity name='contact'>" + fetchColumns +
                    "<order attribute='createdon' descending='false' />" +
                    "<filter>" +
                    "<condition attribute='udo_edipi' operator='eq' value='" +
                    selectedPersonRequest.Edipi + "'/>" +
                    "</filter></entity></fetch>";

                var result = conn.RetrieveMultiple(new FetchExpression(fetch));
                txnTimer.Stop();
                //LogHelper.LogTiming(selectedPersonRequest.OrganizationName, selectedPersonRequest.LogTiming, selectedPersonRequest.UserId, selectedPersonRequest.RelatedParentId, selectedPersonRequest.RelatedParentEntityName, selectedPersonRequest.RelatedParentFieldName, "GetCRMPerson 1", null, txnTimer.ElapsedMilliseconds);
                txnTimer.Restart();
                if (result != null & result.Entities.Count > 0)
                {
                    var crmPerson = HandleSearchResults(result, selectedPersonRequest);
                    if (crmPerson != null)
                    {
                        recSource = crmPerson.GetAttributeValue<string>("udo_recordsource");
                        personId = crmPerson.Id;
                        ownerId = crmPerson.GetAttributeValue<EntityReference>("ownerid").Id;
                        ownerName = crmPerson.GetAttributeValue<EntityReference>("ownerid").Name;
                        firstName = crmPerson.GetAttributeValue<string>("firstname");
                        middleName = crmPerson.GetAttributeValue<string>("middlename");
                        lastName = crmPerson.GetAttributeValue<string>("lastname");
                        fileNumber = crmPerson.GetAttributeValue<string>("udo_filenumber");
                        //SSId = SecurityTools.ConvertToSecureString(crmPerson.GetAttributeValue<string>("udo_ssn"));
                        SSId = crmPerson.GetAttributeValue<string>("udo_ssn").ToSecureString();
                        PID = crmPerson.GetAttributeValue<string>("udo_participantid");
                        ICN = crmPerson.GetAttributeValue<string>("udo_icn");
                        Edipi = crmPerson.GetAttributeValue<string>("udo_edipi");
                        DOB = crmPerson.GetAttributeValue<string>("udo_birthdatestring");
                        gender = crmPerson.GetAttributeValue<string>("udo_gender");
                        BOS = crmPerson.GetAttributeValue<string>("udo_branchofservice");
                        phoneNumber = crmPerson.GetAttributeValue<string>("udo_phonenumber1");
                        found = true;

                        if (crmPerson.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel") != null)
                        {
                            sensitivityLevel = crmPerson.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel").Value.ToString();
                        }
                    }
                }
            }

            // try to find using participant from MVI
            if (personId == Guid.Empty && (selectedPersonRequest.participantID > 0) && found == false)
            {
                var fetch = "<fetch><entity name='contact'>" + fetchColumns +
                             "<order attribute='createdon' descending='false' />" +
                             "<filter type='or'>" +
                             "<condition attribute='udo_participantid' operator='eq' value='" +
                             selectedPersonRequest.participantID.ToString() + "'/>" +
                              "<condition attribute='va_participantid' operator='eq' value='" +
                             selectedPersonRequest.participantID.ToString() + "'/>" +
                             "</filter></entity></fetch>";

                var result = conn.RetrieveMultiple(new FetchExpression(fetch));
                txnTimer.Stop();
                //LogHelper.LogTiming(selectedPersonRequest.OrganizationName, selectedPersonRequest.LogTiming, selectedPersonRequest.UserId, selectedPersonRequest.RelatedParentId, selectedPersonRequest.RelatedParentEntityName, selectedPersonRequest.RelatedParentFieldName, "GetCRMPerson 2", null, txnTimer.ElapsedMilliseconds);
                txnTimer.Restart();

                if (result != null & result.Entities.Count > 0)
                {
                    var crmPerson = result.Entities[0];
                    recSource = crmPerson.GetAttributeValue<string>("udo_recordsource");
                    personId = crmPerson.Id;
                    ownerId = crmPerson.GetAttributeValue<EntityReference>("ownerid").Id;
                    ownerName = crmPerson.GetAttributeValue<EntityReference>("ownerid").Name;
                    firstName = crmPerson.GetAttributeValue<string>("firstname");
                    middleName = crmPerson.GetAttributeValue<string>("middlename");
                    lastName = crmPerson.GetAttributeValue<string>("lastname");
                    fileNumber = crmPerson.GetAttributeValue<string>("udo_filenumber");
                    SSId = SecurityTools.ConvertToSecureString(crmPerson.GetAttributeValue<string>("udo_ssn"));
                    PID = crmPerson.GetAttributeValue<string>("udo_participantid");
                    ICN = crmPerson.GetAttributeValue<string>("udo_icn");
                    Edipi = crmPerson.GetAttributeValue<string>("udo_edipi");
                    DOB = crmPerson.GetAttributeValue<string>("udo_birthdatestring");
                    gender = crmPerson.GetAttributeValue<string>("udo_gender");
                    BOS = crmPerson.GetAttributeValue<string>("udo_branchofservice");
                    phoneNumber = crmPerson.GetAttributeValue<string>("udo_phonenumber1");
                    found = true;

                    if (crmPerson.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel") != null)
                    {
                        sensitivityLevel = crmPerson.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel").Value.ToString();
                    }
                }
            }

            //Changes made for UDO to look into Contact instead of crme_Person
            // try to find using ssn from MVI
            //if (!string.IsNullOrEmpty(selectedPersonRequest.SSIdString) && found == false)
            if (selectedPersonRequest.SSId != null && found == false)
            {
                var fetch = "<fetch><entity name='contact'>" + fetchColumns +
                           "<order attribute='createdon' descending='false' />" +
                           "<filter type='or'>" +
                           "<condition attribute='udo_ssn' operator='eq' value='" +
                           selectedPersonRequest.SSId.ToUnsecureString() + "'/>" +
                           "<condition attribute='va_ssn' operator='eq' value='" +
                           selectedPersonRequest.SSId.ToUnsecureString() + "'/>" +
                           "</filter></entity></fetch>";

                var result = conn.RetrieveMultiple(new FetchExpression(fetch));
                txnTimer.Stop();
                //LogHelper.LogTiming(selectedPersonRequest.OrganizationName, selectedPersonRequest.LogTiming, selectedPersonRequest.UserId, selectedPersonRequest.RelatedParentId, selectedPersonRequest.RelatedParentEntityName, selectedPersonRequest.RelatedParentFieldName, "GetCRMPerson 3", null, txnTimer.ElapsedMilliseconds);
                txnTimer.Restart();

                if (result != null & result.Entities.Count > 0)
                {
                    var crmPerson = HandleSearchResults(result, selectedPersonRequest);
                    if (crmPerson != null)
                    {
                        // var crmPerson = result.Entities[0];
                        recSource = crmPerson.GetAttributeValue<string>("udo_recordsource");
                        personId = crmPerson.Id;
                        ownerId = crmPerson.GetAttributeValue<EntityReference>("ownerid").Id;
                        ownerName = crmPerson.GetAttributeValue<EntityReference>("ownerid").Name;
                        firstName = crmPerson.GetAttributeValue<string>("firstname");
                        middleName = crmPerson.GetAttributeValue<string>("middlename");
                        lastName = crmPerson.GetAttributeValue<string>("lastname");
                        fileNumber = crmPerson.GetAttributeValue<string>("udo_filenumber");
                        //SSId = SecurityTools.ConvertToSecureString(crmPerson.GetAttributeValue<string>("udo_ssn"));
                        SSId = crmPerson.GetAttributeValue<string>("udo_ssn").ToSecureString();
                        PID = crmPerson.GetAttributeValue<string>("udo_participantid");
                        ICN = crmPerson.GetAttributeValue<string>("udo_icn");
                        Edipi = crmPerson.GetAttributeValue<string>("udo_edipi");
                        DOB = crmPerson.GetAttributeValue<string>("udo_birthdatestring");
                        gender = crmPerson.GetAttributeValue<string>("udo_gender");
                        BOS = crmPerson.GetAttributeValue<string>("udo_branchofservice");
                        phoneNumber = crmPerson.GetAttributeValue<string>("udo_phonenumber1");
                        found = true;

                        if (crmPerson.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel") != null)
                        {
                            sensitivityLevel = crmPerson.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel").Value.ToString();
                        }
                    }
                }
            }

            #region Map Updates
            //rc - added check for null person ID
            if (personId != Guid.Empty)
            {
                #region Map Attributes
                var optionSetSLValue = 752280000 + selectedPersonRequest.VeteranSensitivityLevel;

                var updateReason = string.Empty;
                Entity updatedContact = new Entity();
                updatedContact.Id = personId.Value;
                updatedContact.LogicalName = "contact";               
                
                if (firstName != null)
                {
                    if (!firstName.Equals(selectedPersonRequest.FirstName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        updatedContact["firstname"] = selectedPersonRequest.FirstName;
                        updateReason = "fname";
                    }
                }
                else
                {
                    if (selectedPersonRequest.FirstName != null && selectedPersonRequest.FirstName.Trim() != "")
                    {
                        updatedContact["firstname"] = selectedPersonRequest.FirstName;
                        updateReason = "fname";
                    }
                }
                if (middleName != null)
                {
                    if (!middleName.Equals(selectedPersonRequest.MiddleName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        updatedContact["middlename"] = selectedPersonRequest.MiddleName;
                        updateReason += ",mname";
                    }
                }
                else
                {
                    if (selectedPersonRequest.MiddleName != null && selectedPersonRequest.MiddleName.Trim() != "")
                    {
                        updatedContact["middlename"] = selectedPersonRequest.MiddleName;
                        updateReason += ",mname";
                    }
                }

                if (lastName != null)
                {
                    if (!lastName.Equals(selectedPersonRequest.FamilyName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        updatedContact["lastname"] = selectedPersonRequest.FamilyName;
                        updateReason += ",lname";
                    }
                }
                else
                {
                    if (selectedPersonRequest.FamilyName != null && selectedPersonRequest.FirstName.Trim() != "")
                    {
                        updatedContact["lastname"] = selectedPersonRequest.FamilyName;
                        updateReason += ",lname";

                    }
                }

                if (phoneNumber != null)
                {
                    if (!phoneNumber.Equals(selectedPersonRequest.PhoneNumber, StringComparison.InvariantCultureIgnoreCase))
                    {
                        updatedContact["udo_phonenumber1"] = selectedPersonRequest.PhoneNumber;
                        updateReason = ",Phonenumber";
                    }
                }
                else
                {
                    if (selectedPersonRequest.PhoneNumber != null && selectedPersonRequest.PhoneNumber.Trim() != "")
                    {
                        updatedContact["udo_phonenumber1"] = selectedPersonRequest.PhoneNumber;
                        updateReason = ",Phonenumber";
                    }
                }

                if (BOS != null)
                {
                    if (!BOS.Equals(selectedPersonRequest.BranchOfService, StringComparison.InvariantCultureIgnoreCase))
                    {
                        updatedContact["udo_branchofservice"] = selectedPersonRequest.BranchOfService;
                        updateReason += ",BOS";
                    }
                }
                else
                {
                    if (selectedPersonRequest.BranchOfService != null && selectedPersonRequest.BranchOfService.Trim() != "")
                    {
                        updatedContact["udo_branchofservice"] = selectedPersonRequest.BranchOfService;
                        updateReason += ",BOS";
                    }
                }
                if (recSource != null)
                {
                    if (!recSource.Equals(selectedPersonRequest.RecordSource, StringComparison.InvariantCultureIgnoreCase))
                    {
                        updatedContact["udo_recordsource"] = selectedPersonRequest.RecordSource;
                        updateReason += ",RecordSource";
                    }
                }
                if (recSource == null)
                {
                    if (selectedPersonRequest.RecordSource != null && selectedPersonRequest.RecordSource.Trim() != "")
                    {
                        updatedContact["udo_recordsource"] = selectedPersonRequest.RecordSource;
                        updateReason += ",RecordSource";
                    }
                }
                if (!string.IsNullOrEmpty(fileNumber))
                {
                    if (!fileNumber.Equals(selectedPersonRequest.FileNumber, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(selectedPersonRequest.FamilyName))
                        {
                            updatedContact["udo_filenumber"] = selectedPersonRequest.FileNumber;
                            updateReason += ",filenumber";
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(selectedPersonRequest.FileNumber))
                    {
                        updatedContact["udo_filenumber"] = selectedPersonRequest.FileNumber;
                        updateReason += ",filenumber";
                    }
                }

                
                LogHelper.LogDebug(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, "TryGetCRMPerson, Execute", "SSId:" + SecurityTools.ConvertToUnsecureString(SSId), selectedPersonRequest.Debug);
                LogHelper.LogDebug(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, "TryGetCRMPerson, Execute", "selectedPersonRequest.SSId:" + SecurityTools.ConvertToUnsecureString(selectedPersonRequest.SSId), selectedPersonRequest.Debug);
                if (SSId != null && SSId.Length > 0)
                {
                    if (selectedPersonRequest.SSId != null && selectedPersonRequest.SSId.Length > 0)
                    {
                        if (!SecurityTools.ConvertToUnsecureString(SSId).Equals(SecurityTools.ConvertToUnsecureString(selectedPersonRequest.SSId)))
                        {
                            updatedContact["udo_ssn"] = SecurityTools.ConvertToUnsecureString(selectedPersonRequest.SSId);
                            updateReason += ",ssid";
                        }
                    }
                }
                else
                {
                    if (selectedPersonRequest.SSId != null && selectedPersonRequest.SSId.Length > 0)
                    {
                        updatedContact["udo_ssn"] = SecurityTools.ConvertToUnsecureString(selectedPersonRequest.SSId);
                        updateReason += ",ssid";
                    }
                }

                if (!string.IsNullOrEmpty(PID))
                {
                    if (!PID.Equals(selectedPersonRequest.participantID.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        updatedContact["udo_participantid"] = selectedPersonRequest.participantID.ToString();
                        updateReason += ",PID";
                    }
                }
                else
                {
                    if (selectedPersonRequest.participantID != 0)
                    {
                        updateReason += ",PID";
                        updatedContact["udo_participantid"] = selectedPersonRequest.participantID.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(ICN))
                {
                    if (!string.IsNullOrEmpty(selectedPersonRequest.ICN))
                    {
                        if (!ICN.Equals(selectedPersonRequest.ICN, StringComparison.InvariantCultureIgnoreCase))
                        {
                            updatedContact["udo_icn"] = selectedPersonRequest.ICN;
                            updateReason += ",ICN";
                        }
                    }
                    else
                    {
                        selectedPersonRequest.ICN = ICN;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(selectedPersonRequest.ICN))
                    {
                        updatedContact["udo_icn"] = selectedPersonRequest.ICN;
                        updateReason += ",ICN";
                    }
                }
                if (!string.IsNullOrEmpty(Edipi))
                {
                    if (!string.IsNullOrEmpty(selectedPersonRequest.Edipi))
                    {
                        if (!Edipi.Equals(selectedPersonRequest.Edipi, StringComparison.InvariantCultureIgnoreCase))
                        {
                            updatedContact["udo_edipi"] = selectedPersonRequest.Edipi;
                            updateReason += ",EDIPI";
                        }
                    }
                    else
                    {
                        selectedPersonRequest.Edipi = Edipi;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(selectedPersonRequest.Edipi))
                    {
                        updateReason += ",EDIPI";
                        updatedContact["udo_edipi"] = selectedPersonRequest.Edipi;
                    }
                }
                if (!string.IsNullOrEmpty(DOB))
                {
                    if (!string.IsNullOrEmpty(selectedPersonRequest.DateofBirth))
                    {
                        if (!DOB.Equals(FormatDate(selectedPersonRequest.DateofBirth), StringComparison.InvariantCultureIgnoreCase))
                        {
                            updatedContact["udo_birthdatestring"] = FormatDate(selectedPersonRequest.DateofBirth);
                            updateReason += ",DOB";
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(selectedPersonRequest.DateofBirth))
                    {
                        updatedContact["udo_birthdatestring"] = FormatDate(selectedPersonRequest.DateofBirth);
                        updateReason += ",DOB";
                    }
                }
                if (!string.IsNullOrEmpty(gender))
                {
                    if (!string.IsNullOrEmpty(selectedPersonRequest.Gender))
                    {
                        if (!gender.Equals(selectedPersonRequest.Gender, StringComparison.InvariantCultureIgnoreCase))
                        {
                            updatedContact["udo_gender"] = selectedPersonRequest.Gender;
                            updateReason += ",Gender";
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(selectedPersonRequest.Gender))
                    {
                        updatedContact["udo_gender"] = selectedPersonRequest.Gender;
                        updateReason += ",Gender";
                    }
                }


                selectedPersonRequest.OwningTeamId = ownerId;
                //if contact owner is a user (not team) then force update so security update plugin fires
                if (!string.IsNullOrEmpty(ownerName) && ownerName != "PCR")
                {
                    updateReason += ",not owned by team";
                    EntityReference newOwner = GetTeam(conn, "PCR", optionSetSLValue);
                    if (newOwner != null)
                    {
                        selectedPersonRequest.OwningTeamId = newOwner.Id;
                        updatedContact["ownerid"] = newOwner;
                    }
                    LogHelper.LogDebug(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, "TryGetCRMPerson, Execute", "Not owner by team, should update", selectedPersonRequest.Debug);
                }
                //Update Contact (SL and RecordSource) if SL is different in CORPDB
                if (!optionSetSLValue.ToString().Equals(sensitivityLevel))
                {
                    EntityReference newOwner = GetTeam(conn, "PCR", optionSetSLValue);
                    if (newOwner != null)
                    {
                        selectedPersonRequest.OwningTeamId = newOwner.Id;
                        updatedContact["ownerid"] = newOwner;
                    }
                    updateReason += ",SL";
                }
                #endregion Map Attributes


                if (updatedContact.Attributes.Count > 0)
                {
                    LogHelper.LogDebug(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, "TryGetCRMPerson, Execute", "Start of update", selectedPersonRequest.Debug);

                    //TODO: Review** Commented as the Organization Service instance is a delegated user.
                    //conn.CallerId = Guid.Empty;
                    
                    conn.Update(updatedContact);
                    LogHelper.LogDebug(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, "TryGetCRMPerson, Execute", "Should have done update", selectedPersonRequest.Debug);

                    txnTimer.Stop();
                    ////LogHelper.LogTiming(selectedPersonRequest.OrganizationName, selectedPersonRequest.LogTiming, selectedPersonRequest.UserId, selectedPersonRequest.RelatedParentId, selectedPersonRequest.RelatedParentEntityName, selectedPersonRequest.RelatedParentFieldName, "Updated Contact:" + updateReason, null, txnTimer.ElapsedMilliseconds);
                    txnTimer.Restart();

                    if (updatedContact.Attributes.Contains("ownerid"))
                    {
                        AssignRequest assignRequest = new AssignRequest
                        {
                            Assignee = (EntityReference)updatedContact["ownerid"],
                            Target = updatedContact.ToEntityReference()
                        };

                        //TODO: Review** Commented as the Organization Service instance is a delegated user.\
                        // Also replace/combine the Assign call with a Update call.
                        //conn.CallerId = Guid.Empty;
                        conn.Execute(assignRequest);

                        Entity contactOwner = new Entity();
                        contactOwner.LogicalName = "udo_contactownershipchange";
                        contactOwner["udo_name"] = selectedPersonRequest.FirstName + " " + selectedPersonRequest.FamilyName;
                        contactOwner["ownerid"] = updatedContact["ownerid"];
                        contactOwner["udo_veteran"] = new EntityReference("contact", updatedContact.Id);
                        //TODO: Review** Commented as the Organization Service instance is a delegated user.
                        //conn.CallerId = Guid.Empty;
                        conn.Create(contactOwner);
                    }
                }
                else
                {
                    LogHelper.LogDebug(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, "TryGetCRMPerson, Execute", "No Update needed", selectedPersonRequest.Debug);
                }

            }
            #endregion Map Updates
            EntireTimer.Stop();
            ////LogHelper.LogTiming(selectedPersonRequest.OrganizationName, selectedPersonRequest.LogTiming, selectedPersonRequest.UserId, selectedPersonRequest.RelatedParentId, selectedPersonRequest.RelatedParentEntityName, selectedPersonRequest.RelatedParentFieldName, "TryGetCRMPerson Total", null, EntireTimer.ElapsedMilliseconds);


            return personId;
        }

        private Entity HandleSearchResults(EntityCollection resultCollection, UDOSelectedPersonRequest request)
        {
            var selectedPersonParticipantId = request.participantID.ToString();
            selectedPersonParticipantId = selectedPersonParticipantId == "0" ? null : selectedPersonParticipantId;

            if (string.IsNullOrEmpty(selectedPersonParticipantId))
                return resultCollection.Entities.FirstOrDefault();

            //var matchedContact = resultCollection.Entities.Where(c => c.ToEntity<Contact>().udo_ParticipantId == selectedPersonParticipantId || c.ToEntity<Contact>().va_ParticipantID == selectedPersonParticipantId).FirstOrDefault();

            var matchedContact = resultCollection.Entities.Where(c => c.GetAttributeValue<string>("udo_ParticipantId") == selectedPersonParticipantId 
                                                            || c.GetAttributeValue<string>("va_ParticipantID") == selectedPersonParticipantId).FirstOrDefault();


            if (matchedContact != null)
                return matchedContact;

            matchedContact = resultCollection.Entities.Where(c => ((c.GetAttributeValue<string>("va_ParticipantID") == "0" || string.IsNullOrEmpty(c.GetAttributeValue<string>("va_ParticipantID"))) &&
                                                                    (c.GetAttributeValue<string>("udo_ParticipantId") == "0" || string.IsNullOrEmpty(c.GetAttributeValue<string>("udo_ParticipantId"))))).FirstOrDefault();

            if (matchedContact != null)
                return matchedContact;

            ///IF We get this far then we should return null becasuse the returned contact(s) does not match the selected persons based on the particpant ID. 
            return null;
        }
        /// <summary>
        /// Tries to format a given string to mm/dd/yyyy. If the conversion fails, the original string is passed back.
        /// </summary>
        /// <param name="dateString">String to be converted to mm/dd/yyyy</param>
        /// <param name="format">The format in which the date string is passed. The default value is: yyyyMMdd</param>
        /// <returns>Returns a given date string to MM/dd/yyyy format.</returns>

        private static string FormatDate(string dateString, string format = "yyyyMMdd")
        {
            DateTime date;
            try
            {
                date = DateTime.ParseExact(dateString, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
                return date.ToString("MM/dd/yyyy");
            }
            catch (FormatException dateFormatException)
            {
                //If date cannot be reformatted return the date present in the system.
                return dateString;
            }
            catch (ArgumentException dateArgumentException)
            {
                //If date cannot be reformatted return the date present in the system.
                return dateString;
            }
        }

        /// <summary>
        /// Search and Update CRM using personId found with previous CorpDb search and update contact ICN\EDIPI\RecordSource
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="personId"></param>
        /// <param name="recordSource"></param>
        /// <param name="icn"></param>
        /// <param name="edipi"></param>
  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="request"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public Guid TryCreateNewCrmPerson(IOrganizationService connection, ref UDOSelectedPersonRequest request, string recordSource)
        {
            // var newId = new System.Guid();

            if (request.SSId == null)
                request.SSId = String.Empty.ToSecureString(); // SecurityTools.ConvertToSecureString("");
            var newContact = new Entity("contact");
            newContact["udo_ssn"] = request.SSId.Length > 0 ? request.SSId.ToUnsecureString() : string.Empty;
            newContact["udo_participantid"] = request.participantID > 0 ? request.participantID.ToString() : string.Empty;
            newContact["udo_filenumber"] = !string.IsNullOrEmpty(request.FileNumber) ? request.FileNumber : string.Empty;
            newContact["firstname"] = !string.IsNullOrEmpty(request.FirstName) ? request.FirstName : string.Empty;
            newContact["middlename"] = !string.IsNullOrEmpty(request.MiddleName) ? request.MiddleName : string.Empty;
            newContact["lastname"] = !string.IsNullOrEmpty(request.FamilyName) ? request.FamilyName : string.Empty;
            newContact["udo_birthdatestring"] = !string.IsNullOrEmpty(request.DateofBirth) ? FormatDate(request.DateofBirth) : string.Empty;
            newContact["udo_icn"] = !string.IsNullOrEmpty(request.ICN) ? request.ICN : string.Empty;
            newContact["udo_edipi"] = !string.IsNullOrEmpty(request.Edipi) ? request.Edipi : string.Empty;
            newContact["udo_veteransensitivitylevel"] = new OptionSetValue(752280000 + request.VeteranSensitivityLevel);
            newContact["udo_recordsource"] = recordSource;
            newContact["udo_branchofservice"] = !string.IsNullOrEmpty(request.BranchOfService) ? request.BranchOfService : string.Empty;
            newContact["udo_gender"] = !string.IsNullOrEmpty(request.Gender) ? request.Gender : string.Empty;
            EntityReference newOwner = GetTeam(connection, "PCR", request.VeteranSensitivityLevel);
            if (newOwner != null)
            {
                request.OwningTeamId = newOwner.Id;
                newContact["ownerid"] = newOwner;
            }

            return connection.Create(newContact);
        }

        //internal OrganizationServiceProxy ConnectToCrm(string orgName)
        //{
        //    CRMConnect connect = new CRMConnect();
        //    return connect.ConnectToCrm(orgName);
        //}

        /// <summary>
        /// GetTeam: Gets the default team for the specified security level.  If no team is found, it uses
        /// the default team for the user's business unit.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="name"></param>
        /// <param name="securitylevel"></param>
        /// <returns></returns>
        private EntityReference GetTeam(IOrganizationService connection, string name, int securitylevel)
        {
            #region Make sure securitylevel is in range
            const int MinSecurityLevel = 752280000;
            const int MaxSecurityLevel = 752280009;

            // this handles sl levels 0-9
            if (securitylevel >= 0 && securitylevel <= 9) securitylevel += MinSecurityLevel;

            // this handles sl option sets
            if (securitylevel < MinSecurityLevel || securitylevel > MaxSecurityLevel) securitylevel = MinSecurityLevel;
            #endregion

            #region Make sure team is valid
            const string DefaultTeam = "PCR";
            if (String.IsNullOrEmpty(name)) name = DefaultTeam;
            #endregion

            var fetch = @"<fetch count='1'>"
                      + @"<entity name='team'>"
                      + @"<attribute name='teamid' />"
                      + @"<link-entity name='businessunit' from='businessunitid' to='businessunitid'>"
                      + @"<filter>"
                      + @"<condition attribute='udo_veteransensitivitylevel' operator='eq' value='{1}' />"
                      + @"</filter>"
                      + @"</link-entity>"
                      + @"<filter>"
                      + @"<condition attribute='name' operator='eq' value='{0}'/>"
                      + @"</filter>"
                      + @"</entity>"
                      + @"</fetch>";

            var query = String.Format(fetch, name, securitylevel);

            var result = connection.RetrieveMultiple(new FetchExpression(query));

            if (result != null && result.Entities.Count == 1) return result.Entities[0].ToEntityReference();

            #region get default businessunit team
            query  = @"<fetch count='1'><entity name='team'><attribute name='teamid'/><filter>"
                   + @"<condition attribute='businessunitid' operator='eq-businessid'/>"
                   + @"<condition attribute='isdefault' operator='eq' value='1'/>"
                   + @"</filter></entity></fetch>";

            result = connection.RetrieveMultiple(new FetchExpression(query));
            
            // no results... nothing to return
            if (result == null || result.Entities.Count == 0) return null;

            return result.Entities[0].ToEntityReference();
            #endregion
        }

    }
}
