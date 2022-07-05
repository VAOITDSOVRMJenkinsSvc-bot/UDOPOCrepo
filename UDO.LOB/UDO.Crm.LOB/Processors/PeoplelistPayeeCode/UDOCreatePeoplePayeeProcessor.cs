using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Security;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using VIMT.BenefitClaimService.Messages;
using VIMT.ClaimantWebService.Messages;
using VIMT.VeteranWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Awards.Messages;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.MVI.Messages;
using VRM.Integration.UDO.Payments.Messages;
using VRM.Integration.UDO.PeoplelistPayeeCode.CRM;
using VRM.Integration.UDO.PeoplelistPayeeeCode.Messages;
using HeaderInfo = VIMT.BenefitClaimService.Messages.HeaderInfo;

namespace VRM.Integration.UDO.PeoplelistPayeeeCode.Processors
{
    
    internal class UDOCreatePeoplePayeeProcessor
    {
        public delegate void ProgressSetter(string progress, params object[] args);
        
        public StringBuilder sr_log { get; set; }

        SecureString _ssn = new SecureString();

        public IMessageBase Execute(UDOCreatePeoplePayeeRequest request)
        {
            sr_log = new StringBuilder("UDOCreatePeoplePayeeRequest Log:");
            UpdateProgress("Top of Processor");
            OrganizationServiceProxy OrgServiceProxy;
            var response = new UDOCreatePeoplePayeeResponse();

            #region Check for request

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            #endregion

            #region connect to CRM

            try
            {
                var commonFunctions = new CRMConnect();
                OrgServiceProxy = commonFunctions.ConnectToCrm(request.OrganizationName);
                OrgServiceProxy.CallerId = request.UserId;
            }
            catch (Exception connectException)
            {
                var method = MethodInfo.GetThisMethod().ToString();
                LogHelper.LogError(
                    request.OrganizationName,
                    request.UserId,
                    method + ":CRM Connection Error",
                    connectException.Message);
                response.ExceptionOccured = true;
                response.ExceptionMessage = "Failed to get CRMConnection";

                return response;
            }

            #endregion

            try
            {
                if (request.LogSoap)
                {
                    var method = MethodInfo.GetThisMethod().ToString(false);
                    var requestMessage = "Request: \r\n\r\n" + request.SerializeToString();
                    LogHelper.LogInfo(request.OrganizationName, request.LogSoap, request.UserId, method, requestMessage);
                }

                Entity snapshot = VeteranSnapshot_Load(request, OrgServiceProxy);

                request.ptcpntVetId = GetVeteranPID(UpdateProgress, request);

                _ssn = SecurityTools.ConvertToSecureString(request.udo_ssn);

                // The create00PaymentsRequest is initiated here, then updated in AddVeteran
                // It is then used in the LoadPayments method.
                var create00PaymentsRequest = new UDOcreatePaymentsRequest
                {
                    MessageId = Guid.NewGuid().ToString(),
                    RelatedParentEntityName = "contact",
                    RelatedParentFieldName = "udo_contactid",
                    RelatedParentId = request.udo_contactId,
                    Debug = request.Debug,
                    LogSoap = request.LogSoap,
                    LogTiming = request.LogTiming,
                    ownerId = request.ownerId,
                    ownerType = request.ownerType,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    vetsnapshotId = request.vetsnapshotId
                };

                var people = new PeopleCollection();
                People_AddVeteran(UpdateProgress, request, response, people, snapshot, create00PaymentsRequest);
                People_AddDependents(UpdateProgress, request, response, people, snapshot);

                #region Process Claim Data
                try
                {
                    if (request.findBenefitClaimResponse != null
                        && request.findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo != null
                        && request.findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTparticipantRecordbclmInfo != null)
                    {
                        var participantBenefitClaimRecordInfo =
                            request.findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTparticipantRecordbclmInfo;

                        var claimPeople = new PeopleCollection();

                        #region Single Claim

                        if (participantBenefitClaimRecordInfo.mcs_numberOfRecords == "001")
                        {
                            People_AddFromSingleClaim(UpdateProgress, participantBenefitClaimRecordInfo, claimPeople);
                        }

                        #endregion

                        #region Multiple Claims
                        else if (participantBenefitClaimRecordInfo.VIMTselectionbclmInfo != null)
                        {
                            People_AddFromMultipleClaims(UpdateProgress, participantBenefitClaimRecordInfo, claimPeople);
                        }

                        foreach (var person in claimPeople)
                        {
                            if (person.Contains("rem_benefitclaimid"))
                            {
                                GetParticipantIdFromClaimDetail(UpdateProgress, request, person);
                                person.Attributes.Remove("rem_benefitclaimid");
                            }
                            //UpdateProgress(person.DumpToString("Claim Person"));
                            people.Add(person);
                        }
                        claimPeople = null;

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    var method = String.Format("{0}:Claims", MethodInfo.GetThisMethod().ToString(true));
                    var message = String.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                    if (ex.InnerException != null)
                    {
                        message += String.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                    }
                    message += String.Format("Call Stack:\r\n{0}", ex.StackTrace);
                    sr_log.Insert(0, message);
                    message = sr_log.ToString();
                    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId,
                        request.RelatedParentEntityName, request.RelatedParentFieldName, method, message);
                }
                #endregion

                #region Process Awards Data

                try
                {
                    if (request.findGeneralResponse != null && request.findGeneralResponse.VIMTfgenFNreturnclmsInfo != null)
                    {
                        var generalInfoRecord = request.findGeneralResponse.VIMTfgenFNreturnclmsInfo;

                        if (generalInfoRecord != null)
                        {
                            if (generalInfoRecord.VIMTfgenFNawardBenesclmsInfo != null)
                            {
                                #region Many Awards
                                People_AddFromMultipleAwards(UpdateProgress, request, response, people, generalInfoRecord, snapshot);
                                #endregion
                            }
                            else
                            {
                                #region only 1 award to get people from
                                People_AddFromSingleAward(UpdateProgress, request, response, people, generalInfoRecord);
                                #endregion
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var method = String.Format("{0}:Awards", MethodInfo.GetThisMethod().ToString(true));
                    var message = String.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                    if (ex.InnerException != null)
                    {
                        message += String.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                    }
                    message += String.Format("Call Stack:\r\n{0}", ex.StackTrace);
                    sr_log.Insert(0, message);
                    message = sr_log.ToString();
                    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId,
                        request.RelatedParentEntityName, request.RelatedParentFieldName, method, message);
                }
                #endregion

                var requestCollection = BuildRequestCollection(UpdateProgress, request, OrgServiceProxy, people, snapshot);

                #region add records to CRM

                if (requestCollection.Count > 0)
                {
                   // OrgServiceProxy.CallerId = Guid.Empty;
                    var result = ExecuteMultipleHelper.ExecuteMultipleImpersonate(OrgServiceProxy, requestCollection, request.OrganizationName, request.UserId, request.Debug);


                    if (request.Debug)
                    {
                        this.sr_log.Insert(0, "Debug Log:\r\r\r\n");
                        UpdateProgress("Execute Multiple:\r\n" + result.LogDetail);
                    }

                    if (!String.IsNullOrEmpty(request.ptcpntVetId))
                    {
                        OrgServiceProxy.CallerId = request.UserId;
                        // Do not create payments
                        LoadPayments(UpdateProgress, OrgServiceProxy, request, create00PaymentsRequest);
                    }

                    if (request.Debug)
                    {
                        var method = MethodInfo.GetThisMethod().ToString();
                        var message = sr_log.ToString();
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, message);
                    }

                    if (result.IsFaulted)
                    {
                        var method = MethodInfo.GetThisMethod().ToString();
                        LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                        response.ExceptionMessage = result.FriendlyDetail;
                        response.ExceptionOccured = true;
                        return response;
                    }
                }
                #endregion


                return response;
            }
            catch (Exception ex)
            {
                var method = String.Format("{0}:Claims", MethodInfo.GetThisMethod().ToString(true));
                var message = String.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                if (ex.InnerException != null)
                {
                    message += String.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                }
                message += String.Format("Call Stack:\r\n{0}", ex.StackTrace);
                sr_log.Insert(0, message);
                message = sr_log.ToString();
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId,
                    request.RelatedParentEntityName, request.RelatedParentFieldName, method, message);
                response.ExceptionMessage = "Failed to Process Rating Data";
                response.ExceptionOccured = true;
                return response;
            }
        }

        private bool UpdateFiduciaryExists(ProgressSetter UpdateProgress, UDOCreatePeoplePayeeRequest request, UDOPerson person)
        {
            var fidexist = false;

            UpdateProgress("Getting Current Fiduciary");

            var findFiduciaryRequest = new VIMTfidfindFiduciaryRequest()
            {
                LogTiming = request.LogTiming,
                LogSoap = request.LogSoap,
                Debug = request.Debug,
                RelatedParentEntityName = request.RelatedParentEntityName,
                RelatedParentFieldName = request.RelatedParentFieldName,
                RelatedParentId = request.RelatedParentId,
                UserId = request.UserId,
                OrganizationName = request.OrganizationName
            };

            var searchfor = person.GetAttributeValue<string>("udo_filenumber");
            if (String.IsNullOrEmpty(searchfor)) {
                searchfor = person.GetAttributeValue<string>("udo_ssn");
            }

            if (String.IsNullOrEmpty(searchfor)) return false;

            findFiduciaryRequest.mcs_filenumber = searchfor;

            if (request.LegacyServiceHeaderInfo != null)
            {
                findFiduciaryRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }

            var findFiduciaryResponse = findFiduciaryRequest.SendReceive<VIMTfidfindFiduciaryResponse>(MessageProcessType.Local);
            UpdateProgress("After VIMTfidfindFiduciaryRequest EC Call");

            if (findFiduciaryResponse.VIMTfidreturnclmsInfo != null)
            {

                var fidInfo = findFiduciaryResponse.VIMTfidreturnclmsInfo;

                DateTime fidEndDate = DateTime.Today.AddDays(5);

                if (!String.IsNullOrEmpty(fidInfo.mcs_endDate))
                    fidEndDate = DateTime.ParseExact(fidInfo.mcs_endDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                var currentDate = DateTime.Today;

                if (!String.IsNullOrEmpty(fidInfo.mcs_personOrgName) && fidEndDate > currentDate) fidexist = true;
            }

            person["udo_fidexists"] = fidexist;

            return fidexist;


        }

        /// <summary>
        /// GetVeteranPID: Retrieves the Veteran's Participant ID using the information inside the request.
        /// If the request does not have the PID assigned in request.ptcpntVetId, then it retrieves the
        /// PID from the findGeneralResponse and then from the findBenefitClaimResponse.
        /// 
        /// This is not the only place the pid is pulled from, AddVeteran has a fallback to get the pid by
        /// looking it up in CorpDB using the filenumber.  However, this method is used to get the pid from
        /// the information passed in the LOB without making any external calls.
        /// </summary>
        /// <param name="UpdateProgress">UpdateProgress method to record status updates</param>
        /// <param name="request">LOB Create People Payee Request</param>
        /// <returns>The Veteran's PID</returns>
        private string GetVeteranPID(ProgressSetter UpdateProgress, UDOCreatePeoplePayeeRequest request)
        {
            string pid;
            // If the pid is null or 0, then it is null...
            if (request.ptcpntVetId == null || request.ptcpntVetId.Equals("0")) request.ptcpntVetId = null;
            if (UpdateProgress!=null) UpdateProgress("Getting Veteran PID");

            // Try to get the pid from the request, if it is set, return that.
            if (!String.IsNullOrEmpty(request.ptcpntVetId))
            {
                if (UpdateProgress != null) UpdateProgress("PID Already Set to: {0}", request.ptcpntVetId);
                return request.ptcpntVetId;
            }
        
            // Try to get the PID from the findGeneralResponse
            if (request.findGeneralResponse != null
                && request.findGeneralResponse.VIMTfgenFNreturnclmsInfo != null
                && !String.IsNullOrEmpty(request.findGeneralResponse.VIMTfgenFNreturnclmsInfo.mcs_ptcpntVetID))
            {
                pid =  request.findGeneralResponse.VIMTfgenFNreturnclmsInfo.mcs_ptcpntVetID;
                if (UpdateProgress != null) UpdateProgress("Setting Veteran PID using General Info Record.  Vet PID: {0}", pid);
                return pid;
            }

            // Lastly, try to get the pid from the findBenefitClaimResposne
            if (request.findBenefitClaimResponse != null
                && request.findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo != null)
            {
                var claimInfo = request.findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo;
                if (claimInfo.VIMTparticipantRecordbclmInfo != null)
                {
                    if (!String.IsNullOrEmpty(claimInfo.VIMTparticipantRecordbclmInfo.mcs_participantVetID))
                    {
                        pid = claimInfo.VIMTparticipantRecordbclmInfo.mcs_participantVetID;
                        if (UpdateProgress != null) UpdateProgress("Setting Veteran PID using Claim Info ParticpantId Info.  Vet PID: {0}", pid);
                        return pid;
                    }
                }
                if (claimInfo.VIMTbenefitClaimRecord1bclmInfo != null) {
                    if (!String.IsNullOrEmpty(claimInfo.VIMTbenefitClaimRecord1bclmInfo.mcs_participantVetID)) {
                        pid = claimInfo.VIMTbenefitClaimRecord1bclmInfo.mcs_participantVetID;
                        if (UpdateProgress != null) UpdateProgress("Setting Veteran PID using Claim Info Record.  Vet PID: {0}", pid);
                        return pid;
                    }
                }
            }

            // If no PID is found, return an empty pid string
            if (UpdateProgress != null) UpdateProgress("No Veteran PID");
            return string.Empty;
        }

        /// <summary>
        /// Load the Veteran Snapshot
        /// Retrieve the fields: "udo_lastname", "udo_firstname", "udo_ssn", "udo_participantid", "udo_veteranid"
        /// 
        /// The Veteran Snapshot is used to get basic veteran information, veteranid, pid, ssn, name
        /// </summary>
        /// <param name="request">LOB Create People Payee Request</param>
        /// <param name="OrgServiceProxy">OrgServiceProxy</param>
        /// <returns>Veteran Snapshot Entity</returns>
        private Entity VeteranSnapshot_Load(UDOCreatePeoplePayeeRequest request, OrganizationServiceProxy OrgServiceProxy)
        {
            Entity snapshot = null;
            if (request.vetsnapshotId != Guid.Empty)
            {
                snapshot = OrgServiceProxy.Retrieve("udo_veteransnapshot", request.vetsnapshotId,
                    new ColumnSet("ownerid", "udo_birlsguid", "udo_lastname", "udo_firstname", "udo_ssn", "udo_participantid", "udo_veteranid", "udo_cfidpersonorgname", "udo_address1",
                            "udo_address2", "udo_address3", "udo_mailingcity", "udo_mailingstate", "udo_mailingcountry", "udo_mailingzip",
                            "udo_firstname", "udo_lastname", "udo_eveningphone", "udo_dayphone"));
                
                if (request.udo_contactId == Guid.Empty)
                {
                    var contact = snapshot.GetAttributeValue<EntityReference>("udo_veteranid");
                    if (contact != null) request.udo_contactId = contact.Id;
                }
                if (String.IsNullOrEmpty(request.udo_ssn) && !String.IsNullOrEmpty(snapshot.GetAttributeValue<string>("udo_ssn")))
                {
                    _ssn = SecurityTools.ConvertToSecureString(snapshot["udo_ssn"].ToString());
                }
                if (String.IsNullOrEmpty(request.ptcpntVetId) && !String.IsNullOrEmpty(snapshot.GetAttributeValue<string>("udo_participantid")))
                {
                    request.ptcpntVetId = (string)snapshot["udo_participantid"];
                    request.ptcpntRecipId = request.ptcpntVetId;
                    request.ptcpntBeneId = request.ptcpntVetId;
                }
            }
            return snapshot;
        }


        /// <summary>
        /// BuildRequestCollection :  This method takes the people collection, traverses through it
        /// performing common updates to the people, such as setting veteran information, removes
        /// invalid fields that may have been stored for other reasons/logic and builds a request
        /// collection of people and payeecodes.
        /// 
        /// PayeeCodes that are linked to people are created using a single createRequest.
        /// 
        /// </summary>
        /// <param name="UpdateProgress">UpdateProgress method to record status updates</param>
        /// <param name="request">LOB Create People Payee Request</param>
        /// <param name="people">Collection of UDOPerson</param>
        /// <param name="snapshot">Veteran Snapshot</param>
        /// <returns>The Organization Request Collection to execute using ExecuteMultiple</returns>
        private  OrganizationRequestCollection BuildRequestCollection(ProgressSetter UpdateProgress,
            UDOCreatePeoplePayeeRequest request, OrganizationServiceProxy orgService, PeopleCollection people, Entity snapshot)
        {            
            if (UpdateProgress != null) UpdateProgress("BuildRequestCollection - Starting Post Processing");

            if (UpdateProgress != null) UpdateProgress("Setting Up Request Collection as part of the process.");
            var requestCollection = new OrganizationRequestCollection();

            // Remove existing people.... 
            BuildDeleteRequest(UpdateProgress, request, orgService, requestCollection);

            // remove fields that are flagged rem and are only for processing
            // add required relationships
            // add udo_dob from udo_dobstr
            // build requests to perform actions.
            if (UpdateProgress != null) UpdateProgress("Starting the post processing of the people. {0} people to process.", people.Count());
                        
            foreach(var person in people)
            {
                var missingInfo = !person.Contains("udo_ssn") || String.IsNullOrEmpty(person.GetAttributeValue<string>("udo_ssn")) ||
                                  //!person.Contains("udo_gender") || String.IsNullOrEmpty(person.GetAttributeValue<string>("udo_gender")) ||
                                  !person.Contains("udo_dobstr") || String.IsNullOrEmpty(person.GetAttributeValue<string>("udo_dobstr"));
                if (missingInfo && !String.IsNullOrEmpty(person.ParticipantId))
                {
                    if (UpdateProgress != null) UpdateProgress("Missing information, retrieving from corporate record....");
                    GetParticipantIdGeneralInfo(UpdateProgress, request, person);
                }

                if (person.Contains("udo_ssn") && !String.IsNullOrEmpty(person.GetAttributeValue<string>("udo_ssn")))
                {
                    if (!person.Contains("udo_filenumber") || String.IsNullOrEmpty(person.GetAttributeValue<string>("udo_filenumber")))
                    {
                        person["udo_filenumber"] = person.GetAttributeValue<string>("udo_ssn");
                    }
                }

                // Get the fiduciary information for each person.  fidexists is for each person, not the vet as a whole.
                //if (!person.Contains("udo_fidexists")) UpdateFiduciaryExists(UpdateProgress, request, person);

                if (!person.GetAttributeValue<bool>("udo_fidexists") && request.fidExists)
                {
                    // person does not have fid, veteran does, set fid to true
                    // person["udo_fidexists"] = true;
                }

                #region Update Shared Veteran Values
                if (UpdateProgress != null) UpdateProgress("Updating {0} [{1}] with the shared veteran values.", person.Name, person.ParticipantId);

                #region update dob and dobstr
                if (person.Contains("udo_dobstr") && !String.IsNullOrWhiteSpace(person["udo_dobstr"].ToString()) && !person.Contains("udo_dob"))
                {
                    
                    var dob = person.SetAttributeValue<DateTime?>("udo_dob", person["udo_dobstr"].ToString());
                    var crmdob = dob;

                    if (dob.HasValue) crmdob = dob.Value.ToCRMDateTime();
                    if (!crmdob.Equals(dob)) {
                        person.Attributes.Remove("udo_dob");
                    }
                    else
                    {
                        //if (UpdateProgress != null) UpdateProgress("Set DOB: {0}", dob);
                        if (dob.HasValue) person["udo_dobstr"] = dob.Value.ToString("MM/dd/yyyy");
                    }
                }

                if (person.Contains("udo_dob"))
                {
                    var dob = person.GetAttributeValue<DateTime?>("udo_dob");
                    if (dob.HasValue)
                    {
                        var dobstr = person["udo_dobstr"] = dob.Value.ToString("MM/dd/yyyy");
                        //if (UpdateProgress != null) UpdateProgress("Set DOBStr: {0}", dobstr);
                    }
                }
                #endregion

                // Set shared values
                person.SetAttributeValue<EntityReference>("ownerid", request.ownerType, request.ownerId);
                person["udo_idproofid"] = new EntityReference("udo_idproof", request.idProofId);

                if (request.vetsnapshotId != Guid.Empty)
                {
                    person["udo_vetsnapshotid"] = request.vetsnapshotId.ToString();
                    person["udo_veteransnapshotid"] = new EntityReference(
                        "udo_veteransnapshot",
                        request.vetsnapshotId);
                    if (snapshot != null)
                    {
                        person.SetAttributeValue<string>("udo_vetlastname", snapshot.GetAttributeValue<string>("udo_lastname"));
                        person.SetAttributeValue<string>("udo_vetfirstname", snapshot.GetAttributeValue<string>("udo_firstname"));
                        //request.udo_ssn =  person.SetAttributeValue<string>("udo_vetssn", snapshot.GetAttributeValue<string>("udo_ssn"));
                        person["udo_vetssn"] = snapshot.GetAttributeValue<string>("udo_ssn");
                        _ssn = SecurityTools.ConvertToSecureString(snapshot.GetAttributeValue<string>("udo_ssn"));
                    }
                    else
                    {
                        if (request.findGeneralResponse != null)
                        {
                            var findGeneralResponse = request.findGeneralResponse;

                            var generalInfoRecord = findGeneralResponse.VIMTfgenFNreturnclmsInfo;
                            if (generalInfoRecord != null)
                            {
                                person.SetAttributeValue<string>("udo_vetlastname", generalInfoRecord.mcs_vetLastName);
                                person.SetAttributeValue<string>("udo_vetfirstname", generalInfoRecord.mcs_vetFirstName);
                                //request.udo_ssn = person.SetAttributeValue<string>("udo_vetssn", generalInfoRecord.mcs_vetSSN);
                                person["udo_vetssn"] = generalInfoRecord.mcs_vetSSN;
                                _ssn = SecurityTools.ConvertToSecureString(generalInfoRecord.mcs_vetSSN);
                            }
                        }
                    }
                }

                person.SetAttributeValue<EntityReference>("udo_veteranid", "contact", request.udo_contactId);

                if (request.UDOcreatePeopleRelatedEntitiesInfo != null)
                {
                    foreach (var relatedItem in request.UDOcreatePeopleRelatedEntitiesInfo)
                    {
                        person[relatedItem.RelatedEntityFieldName] = new EntityReference(
                            relatedItem.RelatedEntityName,
                            relatedItem.RelatedEntityId);
                    }
                }
                #endregion

                // remove invalid attributes.
                if (UpdateProgress != null) UpdateProgress("Cleaning Person");
                person.Clean();

                //if (UpdateProgress != null) UpdateProgress("\r\n\r\n{0}\r\n\r\n", person.DumpToString("final person"));

                if (!string.IsNullOrEmpty(person.PayeeCode))
                {
                    // Add a person with a payee code
                    if (UpdateProgress != null) UpdateProgress("Building Create Request for Payee Code with child to create Person");
                    requestCollection.Add(new CreateRequest { Target = PayeeCode_Create(UpdateProgress, request, person.To<Entity>()) });
                }
                else
                {
                    // Add a person without a payee code
                    if (UpdateProgress != null) UpdateProgress("Building Create Request to create the Person");
                    requestCollection.Add(new CreateRequest { Target = person.ToEntity<Entity>() });
                }
            }

            // Now that we have looped through the people, add the Default Payee Codes to the request Collection
            PayeeCodes_AddDefault(UpdateProgress, request, requestCollection, 
                people.HasPayeeCode("00",request.ptcpntVetId, _ssn));

            return requestCollection;
        }

        private void BuildDeleteRequest(ProgressSetter UpdateProgress, UDOCreatePeoplePayeeRequest request, 
            OrganizationServiceProxy orgService, OrganizationRequestCollection requestCollection)
        {
            if (UpdateProgress!=null)
            {
                UpdateProgress("Checking for existing people - this only occurs if the Orchestration process was run multiple times");

                var fetch = "<fetch><entity name='udo_person'><attribute name='udo_personid'/><filter>" +
                            "<condition attribute='udo_idproofid' operator='eq' value='" + request.idProofId.ToString()+"'/>"+
                            "</filter></entity></fetch>";

                var response = orgService.RetrieveMultiple(new FetchExpression(fetch));

                if (response != null && response.Entities.Count > 0)
                {
                    UpdateProgress("People found, Orchestration LOB already run...");
                    // Log Error?

                    // Solve problem by removing records below
                    foreach (var record in response.Entities)
                    {
                        requestCollection.Add(new DeleteRequest() {
                             Target = record.ToEntityReference()
                        });
                    }
                }
                
            }
        }

        /// <summary>
        /// PayeeCodes_AddDefault: Builds a set of create request to add the default payee codes and
        /// adds the create requests to the requestCollection.
        /// </summary>
        /// <param name="UpdateProgress">UpdateProgress method to record status updates</param>
        /// <param name="request">LOB Create People Payee Request</param>
        /// <param name="requestCollection">The OrganizationRequestCollection containing the requests to create records</param>
        /// <param name="hasVetPayeeCode">Set to true if you have already created a payee code for the veteran record.</param>
        private void PayeeCodes_AddDefault(ProgressSetter UpdateProgress, UDOCreatePeoplePayeeRequest request, OrganizationRequestCollection requestCollection, bool hasVetPayeeCode)
        {
            if (UpdateProgress != null) UpdateProgress("Adding Default Payee Codes");

            // This is the list of payee codes to create (excluding the 00 veteran)
            var defaultPayeeCodes = new[] { "41", "42", "43", "44", "45", "50", "60" };
            
            // We don't really use this entity, but because our logic in PayeeCode_Create reads
            // from the person entity passed, we mimic that here by setting the payeecode and pid
            var payee = new Entity("unreal");
            payee["udo_ptcpntid"] = "000000000";
            
            foreach (var payeeCode in defaultPayeeCodes)
            {
                payee["udo_payeecode"] = payeeCode;
                if (UpdateProgress != null) UpdateProgress("Adding default payeecode: {0}", payeeCode);
                requestCollection.Add(new CreateRequest { Target = PayeeCode_Create(UpdateProgress, request, payee, false, false) });
            }
            
            // If the Veteran Payee Code has not been created already, then create the Veteran Payee Code
            if (!hasVetPayeeCode) {
                payee["udo_payeecode"]="00";
                requestCollection.Add(new CreateRequest { Target = PayeeCode_Create(UpdateProgress, request, payee, false, true) });
            }
        }

        /// <summary>
        /// GetParticipantIdGeneralInfo: This gets called when the person is missing infromation, such as the ssn
        /// dob and gender.
        /// 
        /// This method calls the VIMTfgenpidfindGeneralInformationByPtcpntIds to get the information.  Depending
        /// on how a person was created, they could be missing key pieces of information the people record should
        /// have.  This method gets that information from CorpDB.
        /// </summary>
        /// <param name="UpdateProgress">UpdateProgress method to record status updates</param>
        /// <param name="request">LOB Create People Payee Request</param>
        /// <param name="people">Collection of UDOPerson</param>
        private  void GetParticipantIdGeneralInfo(ProgressSetter UpdateProgress, UDOCreatePeoplePayeeRequest request, UDOPerson person)
        {
            if (UpdateProgress != null) UpdateProgress("Getting Participant Id General Info For: {0}", person.ParticipantId);
            var findGeneralInformationByPtcpntIdsRequest = new VIMTfgenpidfindGeneralInformationByPtcpntIdsRequest
            {
                LogTiming = request.LogTiming,
                LogSoap = request.LogSoap,
                Debug = request.Debug,
                OrganizationName = request.OrganizationName,
                UserId = request.UserId,
                RelatedParentEntityName = request.RelatedParentEntityName,
                RelatedParentFieldName = request.RelatedParentFieldName,
                RelatedParentId = request.RelatedParentId,
                mcs_ptcpntvetid = request.ptcpntVetId,
                mcs_ptcpntbeneid = person.ParticipantId,
                mcs_ptpcntrecipid = person.ParticipantId,

                LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                }
            };

            var findGeneralInformationByPtcpntIdsResponse = findGeneralInformationByPtcpntIdsRequest.SendReceive<VIMTfgenpidfindGeneralInformationByPtcpntIdsResponse>(MessageProcessType.Local);
            if (UpdateProgress != null) UpdateProgress("After VIMT EC Call");
            if (findGeneralInformationByPtcpntIdsResponse.VIMTfgenpidreturnclmsInfo != null)
            {
                var pidClaimInfo = findGeneralInformationByPtcpntIdsResponse.VIMTfgenpidreturnclmsInfo;
                //person.SetAttributeValue<string>("udo_ssn", pidClaimInfo.mcs_payeeSSN);
                person["udo_ssn"] = pidClaimInfo.mcs_payeeSSN;
                person.SetAttributeValue<string>("udo_dobstr", pidClaimInfo.mcs_payeeBirthDate);
                person.SetAttributeValue<string>("udo_gender", pidClaimInfo.mcs_payeeSex);
                person.SetAttributeValue<string>("udo_payeetypename", pidClaimInfo.mcs_payeeTypeName);
            }
        }

        /// <summary>
        /// GetParticipantIdFromClaimDetail: This method uses a claim id from the claims that were
        /// found to attempt to get the participant id for the person.  If found, it will also use
        /// the information to properly set the person's name (first, middle, last).
        /// </summary>
        /// <param name="UpdateProgress">UpdateProgress method to record status updates</param>
        /// <param name="request">LOB Create People Payee Request</param>
        /// <param name="person">The UDOPerson for which to retrieve the participant id</param>
        private static void GetParticipantIdFromClaimDetail(ProgressSetter UpdateProgress,
            UDOCreatePeoplePayeeRequest request, UDOPerson person)
        {
            #region findBenefitClaimDetailsbyClaimId
            if (person == null)
            {
                UpdateProgress("Person empty.");
                return;
            }
            if (!person.Contains("rem_benefitclaimid"))
            {
                UpdateProgress("No Benefit Claim Id to lookup");
                return;  // if there is no benefit claim id then it cannot be processed
            }
            
            if (UpdateProgress != null) UpdateProgress("Trying to get Participant ID from claim detail for {0}", person.Name);

            var claimDetailRequest = new VIMTfbendtlfindBenefitClaimDetailRequest
            {
                LogTiming = request.LogTiming,
                LogSoap = request.LogSoap,
                Debug = request.Debug,
                RelatedParentEntityName = request.RelatedParentEntityName,
                RelatedParentFieldName = request.RelatedParentFieldName,
                RelatedParentId = request.RelatedParentId,
                UserId = request.UserId,
                OrganizationName = request.OrganizationName,
                mcs_benefitclaimid = person["rem_benefitclaimid"].ToString(),
                
            };

            if (request.LegacyServiceHeaderInfo!=null)
            {
                claimDetailRequest.LegacyServiceHeaderInfo = new HeaderInfo
                 {
                     ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                     ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                     LoginName = request.LegacyServiceHeaderInfo.LoginName,
                     StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                 };
            }

            var findBenefitClaimDetailsByClaimIdResponse =
                claimDetailRequest.SendReceive<VIMTfbendtlfindBenefitClaimDetailResponse>(
                    MessageProcessType.Local);
            if (UpdateProgress != null) UpdateProgress("After VIMT EC Call for findBenefitClaimDetails...");

            VIMTfbendtlbenefitClaimRecordbclm claimRecord;
            VIMTfbendtlbenefitClaimRecord1bclm claimRecordInfo;

            if ((claimRecord = findBenefitClaimDetailsByClaimIdResponse.VIMTfbendtlbenefitClaimRecordbclmInfo) == null ||
                (claimRecordInfo = claimRecord.VIMTfbendtlbenefitClaimRecord1bclmInfo) == null ||
                string.IsNullOrEmpty(claimRecordInfo.mcs_participantClaimantID))
            {
                var method = MethodInfo.GetThisMethod().ToString();
                LogHelper.LogError(request.OrganizationName, request.UserId, method, "Unable to get Participant Id from Claim");
                return;
            }

            person["udo_ptcpntid"] = claimRecordInfo.mcs_participantClaimantID;
            
            var first = person["udo_first"] = claimRecordInfo.mcs_claimantFirstName;
            var last = person["udo_last"] = claimRecordInfo.mcs_claimantLastName;
            person["udo_middle"] = claimRecordInfo.mcs_claimantMiddleName;
            var orgname = person.SetAttributeValue<string>("rem_organizationname", claimRecordInfo.mcs_organizationName);

            if (!string.IsNullOrEmpty(orgname))
            {
                person["udo_type"] = new OptionSetValue(752280003);
            }
            else
            {
                person["udo_type"] = new OptionSetValue(752280002);
            }

            string name;
            if (!String.IsNullOrEmpty(orgname))
            {
                if (UpdateProgress != null) UpdateProgress("Setting Name to Organization: {0}", orgname);
                person.Name = name = orgname;
                if (person.Contains("udo_first")) person.Attributes.Remove("udo_first");
                if (person.Contains("udo_last")) person.Attributes.Remove("udo_last");
            }
            else
            {
                name = string.Format("{0} {1}", first, last).Trim();
                person.Name = name;
            }
            #endregion;
        }

        /// <summary>
        /// People_AddDependents: Uses VIMTfedpfindDependents to get the veteran's dependents and add them
        /// to the people collection.
        /// </summary>
        /// <param name="UpdateProgress">UpdateProgress method to record status updates</param>
        /// <param name="request">LOB Create People Payee Request</param>
        /// <param name="response">The response to the LOB in case there was a fatal error.</param>
        /// <param name="people">A collection of people to add the dependents to.</param>
        private void People_AddDependents(
            ProgressSetter UpdateProgress, UDOCreatePeoplePayeeRequest request, UDOCreatePeoplePayeeResponse response, IList<UDOPerson> people, Entity snapshot)
        {
            #region createdependents

            if (UpdateProgress != null) UpdateProgress("Adding Dependents to People");
            if (String.IsNullOrEmpty(request.fileNumber))
            {
                UpdateProgress("Aborting Add Dependents - no file number");
                return;
            }

            var findDependentsRequest = new VIMTfedpfindDependentsRequest
            {
                LogTiming = request.LogTiming,
                LogSoap = request.LogSoap,
                Debug = request.Debug,
                RelatedParentEntityName = request.RelatedParentEntityName,
                RelatedParentFieldName = request.RelatedParentFieldName,
                RelatedParentId = request.RelatedParentId,
                UserId = request.UserId,
                OrganizationName = request.OrganizationName,
                mcs_filenumber = request.fileNumber
            };

            if (request.LegacyServiceHeaderInfo != null)
            {
                findDependentsRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }
            if (UpdateProgress != null) UpdateProgress("About to call findDependentsRequest, request.fileNumber: {0}", request.fileNumber);
            var findDependentsResponse = findDependentsRequest.SendReceive<VIMTfedpfindDependentsResponse>(MessageProcessType.Local);
            if (UpdateProgress != null) UpdateProgress("After VIMT EC Call");

            response.ExceptionMessage = findDependentsResponse.ExceptionMessage;
            response.ExceptionOccured = findDependentsResponse.ExceptionOccured;
            if (findDependentsResponse.VIMTfedpreturnclmsInfo == null) return;
            var dependents = findDependentsResponse.VIMTfedpreturnclmsInfo.VIMTfedppersonsclmsInfo;
            if (dependents == null) return;
            foreach (var dependent in dependents)
            {
                if (UpdateProgress != null) UpdateProgress("Adding/Updating Dependent: {0}", dependent.mcs_ptcpntId);

                var person = new UDOPerson();
                person.SetAttributeValue<EntityReference>("ownerid", request.ownerType, request.ownerId);
                if (request.vetsnapshotId != Guid.Empty)
                {
                    person["udo_vetsnapshotid"] = request.vetsnapshotId.ToString();
                    person["udo_veteransnapshotid"] = new EntityReference("udo_veteransnapshot", request.vetsnapshotId);
                }

                if ((dependent.mcs_awardIndicator == "N") || string.IsNullOrEmpty(dependent.mcs_awardIndicator))
                {
                    person["udo_type"] = new OptionSetValue(752280001);
                }
                else
                {
                    person["udo_type"] = new OptionSetValue(752280002);
                }

                person["udo_filenumber"] = dependent.mcs_ssn;
                person["udo_ssn"] = dependent.mcs_ssn;
                person["udo_vetssn"] = SecurityTools.ConvertToUnsecureString(_ssn);
                person["udo_ptcpntid"] = dependent.mcs_ptcpntId;
                person["udo_middle"] = dependent.mcs_middleName;

                var last = person["udo_last"] = dependent.mcs_lastName;

                person["udo_gender"] = dependent.mcs_gender;

                var first = person["udo_first"] = dependent.mcs_firstName;
                person.Name = string.Format("{0} {1}", first, last).Trim();
                person.SetAttributeValue<string>("udo_dobstr", dependent.mcs_dateOfBirth);

                //RC NEW - add address here
                UpdateProgress("Adding address info from snapshot");
                if (snapshot.Contains("udo_address1")) person["udo_address1"] = snapshot["udo_address1"].ToString();
                if (snapshot.Contains("udo_address2")) person["udo_address2"] = snapshot["udo_address2"].ToString();
                if (snapshot.Contains("udo_address3")) person["udo_address3"] = snapshot["udo_address3"].ToString();
                if (snapshot.Contains("udo_mailingcity")) person["udo_city"] = snapshot["udo_mailingcity"].ToString();
                if (snapshot.Contains("udo_mailingstate")) person["udo_state"] = snapshot["udo_mailingstate"].ToString();
                if (snapshot.Contains("udo_mailingcountry")) person["udo_country"] = snapshot["udo_mailingcountry"].ToString();
                if (snapshot.Contains("udo_mailingzip")) person["udo_zip"] = snapshot["udo_mailingzip"].ToString();
                if (snapshot.Contains("udo_firstname")) person["udo_vetfirstname"] = snapshot["udo_firstname"].ToString();
                if (snapshot.Contains("udo_lastname")) person["udo_vetlastname"] = snapshot["udo_lastname"].ToString();
                if (snapshot.Contains("udo_eveningphone")) person["udo_eveningphone"] = snapshot["udo_eveningphone"].ToString();
                if (snapshot.Contains("udo_dayphone")) person["udo_dayphone"] = snapshot["udo_dayphone"].ToString();
                

                //if (UpdateProgress != null) UpdateProgress(person.DumpToString("Dependent"));
                people.Add(person);
            }

            #endregion
        }

        /// <summary>
        /// People_AddVeteran: Create the UDOPerson for the veteran.  This person's information can 
        /// be updated later, but this created the foundation with all of the pertinetn information
        /// for a veteran person record, and then adds it to the people collection.
        /// </summary>
        /// <param name="UpdateProgress">UpdateProgress method to record status updates</param>
        /// <param name="request">LOB Create People Payee Request</param>
        /// <param name="response">The response to the LOB in case there was a fatal error.</param>
        /// <param name="people">A collection of people to add the veteran to.</param>
        /// <param name="snapshot">Veteran Snapshot</param>
        /// <returns>The Veteran UDOPerson/CRM Entity</returns>
        private UDOPerson People_AddVeteran(
            ProgressSetter UpdateProgress,
            UDOCreatePeoplePayeeRequest request,
            UDOCreatePeoplePayeeResponse response,
            IList<UDOPerson> people, Entity snapshot,
            UDOcreatePaymentsRequest payee00Request)
        {
            if (UpdateProgress != null) UpdateProgress("Adding Veteran Record 00");
            
            var payeeCode = "00";
            var person = new UDOPerson();
            person.IsVeteran = true;
            person.GetAttributeValue<string>("blah");
            person.LogicalName = "udo_person";

            bool isNameSet, isSSNSet, isPIDSet;
            isNameSet = isSSNSet = isPIDSet = false;

            if (snapshot != null)
            {
                if (!String.IsNullOrEmpty(snapshot.GetAttributeValue<string>("udo_firstname"))
                    && !String.IsNullOrEmpty(snapshot.GetAttributeValue<string>("udo_lastname")))
                {
                    isNameSet = true;
                    var first = person["udo_first"] = (string)snapshot["udo_firstname"];
                    var last = person["udo_last"] = (string)snapshot["udo_lastname"];
                    person.Name = String.Format("{0} {1}",first,last).Trim();
                }
                if (!String.IsNullOrEmpty(snapshot.GetAttributeValue<string>("udo_ssn")))
                {
                    isSSNSet = true;
                    _ssn = SecurityTools.ConvertToSecureString(snapshot["udo_ssn"].ToString());
                    person["udo_vetssn"] = person["udo_ssn"] = snapshot["udo_ssn"].ToString();
                }
                if (!String.IsNullOrEmpty(snapshot.GetAttributeValue<string>("udo_participantid")) 
                    && !snapshot["udo_participantid"].ToString().Equals("0")) // Birls only gets set to 0 
                {
                    isPIDSet = true;
                    request.ptcpntVetId = person.ParticipantId = (string)snapshot["udo_participantid"];
                }
                //if (!String.IsNullOrEmpty(snapshot.GetAttributeValue<string>("udo_cfidpersonorgname")))
                //{
                //    request.fidExists = true;
                //}
            }

            // true if vetsnapshot has udo_cfidpersonorgname or if request specified true, false by default
            //person["udo_fidexists"] = request.fidExists; 

            // Get the Fid for the veteran here - set vet has fiduciary in the request.
            request.fidExists = UpdateFiduciaryExists(UpdateProgress, request, person);

            person["udo_awardsexist"] = false;
            

            if (request.ptcpntRecipId == "0") request.ptcpntRecipId = null;
            if (request.ptcpntVetId == "0") request.ptcpntVetId = null;
            if (request.ptcpntBeneId == "0") request.ptcpntBeneId = null;
            if (String.IsNullOrEmpty(request.ptcpntRecipId)) request.ptcpntRecipId = request.ptcpntVetId;
            if (String.IsNullOrEmpty(request.ptcpntVetId)) request.ptcpntVetId = request.ptcpntRecipId;

            person["udo_payeetypecode"] = payeeCode;
            person["udo_payeecode"] = payeeCode;

            //person["udo_fidexists"] = request.fidExists;
            //person["udo_filenumber"] = request.fileNumber;
            if (!isPIDSet && !String.IsNullOrEmpty(request.ptcpntVetId))
            {
                person["udo_ptcpntid"] = request.ptcpntVetId;
                isPIDSet = true;
            }
            person["udo_type"] = new OptionSetValue(752280000);

            if (UpdateProgress != null) UpdateProgress("Getting Veteran Information");

            if (!String.IsNullOrEmpty(request.fileNumber))
            {
                person["udo_filenumber"] = request.fileNumber;

                #region Build VIMT Request
                var findCorporateRecordByFileNumberRequest = new VIMTcrpFNfindCorporateRecordByFileNumberRequest
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName
                };

                if (request.LegacyServiceHeaderInfo != null)
                {
                    findCorporateRecordByFileNumberRequest.LegacyServiceHeaderInfo =
                        new VIMT.VeteranWebService.Messages.HeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                }

                // non standard fields
                findCorporateRecordByFileNumberRequest.mcs_filenumber = request.fileNumber;

                #endregion
                var findCorporateRecordByFileNumberResponse =
                   findCorporateRecordByFileNumberRequest.SendReceive<VIMTcrpFNfindCorporateRecordByFileNumberResponse>(
                        MessageProcessType.Local);
                if (UpdateProgress != null) UpdateProgress("After VIMT EC Call");
                response.ExceptionMessage = findCorporateRecordByFileNumberResponse.ExceptionMessage;
                response.ExceptionOccured = findCorporateRecordByFileNumberResponse.ExceptionOccured;

                if (findCorporateRecordByFileNumberResponse == null ||
                    findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo == null
                    || findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_fileNumber == null)
                {
                    // If the veteran was not found in the CorpDB, then we need to retrieve the basic information
                    // from BIRLS.

                    if (UpdateProgress != null) UpdateProgress("Veteran Record not in Corp DB");

                    if (!String.IsNullOrEmpty(request.fileNumber))
                    {
                        var birlsRequest = new VIMTbrlsFNfindBirlsRecordByFileNumberRequest
                        {
                            Debug = request.Debug,
                            LogSoap = request.LogSoap,
                            LogTiming = request.LogTiming,
                            UserId = request.UserId,
                            OrganizationName = request.OrganizationName,
                            RelatedParentEntityName = request.RelatedParentEntityName,
                            RelatedParentFieldName = request.RelatedParentFieldName,
                            RelatedParentId = request.RelatedParentId,
                            LegacyServiceHeaderInfo = new VIMT.VeteranWebService.Messages.HeaderInfo()
                            {
                                ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                            },
                            mcs_filenumber = request.fileNumber,
                            MessageId = Guid.NewGuid().ToString()
                        };

                        var birlsResponse = birlsRequest.SendReceive<VIMTbrlsFNfindBirlsRecordByFileNumberResponse>(MessageProcessType.Local);

                        if (birlsResponse.VIMTbrlsFNreturnInfo != null)
                        {
                            var birlsInfo = birlsResponse.VIMTbrlsFNreturnInfo;
                            //person.SetAttributeValue<string>("udo_ssn", birlsInfo.mcs_SOC_SEC_NUMBER);
                            person["udo_ssn"] = birlsInfo.mcs_SOC_SEC_NUMBER;
                            //request.udo_ssn = person.SetAttributeValue<string>("udo_vetssn", birlsInfo.mcs_SOC_SEC_NUMBER);
                            person["udo_vetssn"] = birlsInfo.mcs_SOC_SEC_NUMBER;
                            _ssn = SecurityTools.ConvertToSecureString(birlsInfo.mcs_SOC_SEC_NUMBER);
                            var first = person.SetAttributeValue<string>("udo_first", birlsInfo.mcs_FIRST_NAME);
                            var last = person.SetAttributeValue<string>("udo_last", birlsInfo.mcs_LAST_NAME);
                            var name = string.Format("{0} {1}", first, last).Trim();
                            person.Name = name;
                            person.SetAttributeValue<string>("udo_middle", birlsInfo.mcs_MIDDLE_NAME);
                            person.SetAttributeValue<string>("udo_dobstr", birlsInfo.mcs_DATE_OF_BIRTH);
                            person.SetAttributeValue<string>("udo_gender", birlsInfo.mcs_SEX_CODE);
                        }
                    }
                }
                else
                {
                    if (!isPIDSet && !String.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_ptcpntId))
                    {
                        request.ptcpntVetId = findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_ptcpntId;
                        person.ParticipantId = request.ptcpntRecipId = request.ptcpntVetId;
                    }
                    if (!isSSNSet)
                    {
                        //person.SetAttributeValue<string>("udo_ssn", findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_ssn);
                        person["udo_ssn"] = findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_ssn;
                        //request.udo_ssn = person.SetAttributeValue<string>("udo_vetssn", findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_ssn);
                        person["udo_vetssn"] = findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_ssn;
                        _ssn = SecurityTools.ConvertToSecureString(findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_ssn);
                    }
                    if (!isNameSet || !String.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_middleName))
                    {
                        var first = person.SetAttributeValue<string>(
                            "udo_first",
                            findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_firstName);
                        var last = person.SetAttributeValue<string>(
                            "udo_last",
                            findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_lastName);

                        person.Name = string.Format("{0} {1}", first, last).Trim();

                        person.SetAttributeValue<string>(
                            "udo_middle",
                            findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_middleName);
                    }
                    person.SetAttributeValue<string>(
                        "udo_dobstr",
                        findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_dateOfBirth);
                    person.SetAttributeValue<string>(
                        "udo_email",
                        findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_emailAddress);

                    var telephone1 = string.Empty;
                    var telephone2 = string.Empty;
                    if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_phoneNumberOne))
                    {
                        telephone1 = "(" + findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_areaNumberOne + ") "
                                     + findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_phoneNumberOne;
                    }

                    if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_phoneNumberTwo))
                    {
                        telephone2 = "(" + findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_areaNumberTwo + ") "
                                     + findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_phoneNumberTwo;
                    }

                    if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_phoneTypeNameOne))
                    {
                        if (findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_phoneTypeNameOne.Equals(
                            "daytime",
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            person["udo_dayphone"] = telephone1;
                            person["udo_eveningphone"] = telephone2;
                        }
                        else
                        {
                            person["udo_dayphone"] = telephone2;
                            person["udo_eveningphone"] = telephone1;
                        }
                    }

                    person.SetAttributeValue<string>(
                        "udo_address1",
                        findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_addressLine1);
                    person.SetAttributeValue<string>(
                        "udo_address2",
                        findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_addressLine2);
                    person.SetAttributeValue<string>(
                        "udo_address3",
                        findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_addressLine3);
                    person.SetAttributeValue<string>(
                        "udo_city",
                        findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_city);
                    person.SetAttributeValue<string>(
                        "udo_country",
                        findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_country);
                    person.SetAttributeValue<string>(
                        "udo_state",
                        findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_state);
                    person.SetAttributeValue<string>(
                        "udo_zip",
                        findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_zipCode);
                }
            }
            else
            {
                //filenumber is null or empty
                var findVeteranRequest = new UDOfindVeteranInfoRequest();
                findVeteranRequest.LogTiming = request.LogTiming;
                findVeteranRequest.LogSoap = request.LogSoap;
                findVeteranRequest.Debug = request.Debug;
                findVeteranRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findVeteranRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findVeteranRequest.RelatedParentId = request.RelatedParentId;
                findVeteranRequest.UserId = request.UserId;
                findVeteranRequest.OrganizationName = request.OrganizationName;
                findVeteranRequest.LegacyServiceHeaderInfo = new VRM.Integration.UDO.MVI.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };

                if (String.IsNullOrEmpty(request.fileNumber))
                {
                    findVeteranRequest.SocialSN = request.udo_ssn;
                }
                else
                {
                    findVeteranRequest.fileNumber = request.fileNumber;
                }

                var findVeteranResponse = findVeteranRequest.SendReceive<UDOfindVeteranInfoResponse>(MessageProcessType.Local);

                if (findVeteranResponse != null && findVeteranResponse.UDOfindVeteranInfoInfo !=null)
                {
                    var vetInfo = findVeteranResponse.UDOfindVeteranInfoInfo;

                    if (!String.IsNullOrEmpty(vetInfo.crme_SSN))
                    {
                        person["udo_vetssn"] = vetInfo.crme_SSN;
                        person["udo_ssn"] = vetInfo.crme_SSN;
                    }

                    person.SetAttributeValue<string>("udo_first", vetInfo.crme_FirstName);
                    person.SetAttributeValue<string>("udo_last", vetInfo.crme_LastName);
                    person.SetAttributeValue<string>("udo_middle", vetInfo.crme_MiddleName);
                    person.SetAttributeValue<string>("udo_dobstr", vetInfo.crme_DOB.ToString(@"MM\/dd\/yyyy"));
                    person.SetAttributeValue<string>("udo_gender", vetInfo.crme_Gender);
                    // Format phone number??
                    person.SetAttributeValue<string>("udo_dayphone", vetInfo.crme_PrimaryPhone);
                    
                    person.SetAttributeValue<string>("udo_address1", vetInfo.crme_Address1);
                    person.SetAttributeValue<string>("udo_address2", vetInfo.crme_Address2);
                    person.SetAttributeValue<string>("udo_address3", vetInfo.crme_Address3);
                    person.SetAttributeValue<string>("udo_city", vetInfo.crme_City);
                    person.SetAttributeValue<string>("udo_country", vetInfo.crme_Country);
                    person.SetAttributeValue<string>("udo_state", vetInfo.crme_State);
                    person.SetAttributeValue<string>("udo_zip", vetInfo.crme_Zip);
                    if (!String.IsNullOrEmpty(vetInfo.crme_FileNumber)) {
                        request.fileNumber = vetInfo.crme_FileNumber;
                        // Should we recall the addvet and return?
                    }
                    
                }
            }
            //if (UpdateProgress != null) UpdateProgress(person.DumpToString("Veteran Person"));
            people.Add(person);

            // Update Payee00Request with snapshotid pid
            payee00Request.PayeeCode = payeeCode;
            if (snapshot != null) payee00Request.vetsnapshotId = snapshot.Id;
            if (isPIDSet)
            {
                long lpid = 0;
                if (long.TryParse(person["udo_ptcpntid"].ToString(), out lpid)) {
                    payee00Request.ParticipantId =lpid;
                }
            }
            if (!String.IsNullOrEmpty(request.fileNumber))
            {
                payee00Request.FileNumber = request.fileNumber;
            }

            UpdateProgress("Veteran PID: {0}", request.ptcpntVetId);

            return person;
        }

        /// <summary>
        /// People_AddFromSingleAward: Add the information from a single award to the person, and then
        /// add that person to the people collection.
        /// </summary>
        /// <param name="UpdateProgress">UpdateProgress method to record status updates</param>
        /// <param name="request">LOB Create People Payee Request</param>
        /// <param name="response">The response to the LOB in case there was a fatal error.</param>
        /// <param name="people">A collection of people to add the award person to.</param>
        /// <param name="generalInfoRecord">The Award Information</param>
        /// <param name="person">The person to update with the information from the award.</param>
        public void People_AddFromSingleAward(ProgressSetter UpdateProgress, UDOCreatePeoplePayeeRequest request,
            UDOCreatePeoplePayeeResponse response, IList<UDOPerson> people, VIMTfgenFNreturnclms generalInfoRecord)
        {
            if (UpdateProgress != null) UpdateProgress("Mapping Singleton Fields");

            var person = new UDOPerson();

            person.SetAttributeValue<EntityReference>("ownerid", request.ownerType, request.ownerId);
            person["udo_idproofid"] = new EntityReference("udo_idproof", request.idProofId);

            if (request.vetsnapshotId != Guid.Empty)
            {
                person["udo_vetsnapshotid"] = request.vetsnapshotId.ToString();
                person["udo_veteransnapshotid"] = new EntityReference(
                    "udo_veteransnapshot",
                    request.vetsnapshotId);
            }

            person["udo_awardsexist"] = true;

            person["udo_filenumber"] = generalInfoRecord.mcs_payeeSSN; // request.fileNumber;
            person["udo_ssn"] = generalInfoRecord.mcs_payeeSSN; // request.udo_ssn;
            person["udo_vetssn"] = SecurityTools.ConvertToUnsecureString(_ssn);
            person["udo_ptcpntid"] = request.ptcpntRecipId;

            person.SetAttributeValue<string>("udo_vetlastname", generalInfoRecord.mcs_vetLastName);
            person.SetAttributeValue<string>("udo_vetfirstname", generalInfoRecord.mcs_vetFirstName);
            //person.SetAttributeValue<string>("udo_vetssn", generalInfoRecord.mcs_vetSSN);
            person["udo_vetssn"] = generalInfoRecord.mcs_vetSSN;

            // Set Date using string:
            person.SetAttributeValue<DateTime?>("udo_statusreasondate", generalInfoRecord.mcs_statusReasonDate);

            person.SetAttributeValue<string>("udo_payeetypename", generalInfoRecord.mcs_payeeTypeName);
            person.Name = generalInfoRecord.mcs_payeeName;

            // Does this need to be here, It gets the data from the selectioninfo before calling this method
            if (!string.IsNullOrEmpty(generalInfoRecord.mcs_ptcpntRecipID))
            {
                person["udo_ptcpntid"] = generalInfoRecord.mcs_ptcpntRecipID;
                request.ptcpntRecipId = generalInfoRecord.mcs_ptcpntRecipID;
            }

            if (!string.IsNullOrEmpty(generalInfoRecord.mcs_ptcpntBeneID))
            {
                request.ptcpntBeneId = generalInfoRecord.mcs_ptcpntBeneID;
            }
            
            //person.SetAttributeValue<string>("udo_ssn", generalInfoRecord.mcs_payeeSSN);
            person["udo_ssn"] = generalInfoRecord.mcs_payeeSSN;
            person.SetAttributeValue<string>("udo_gender", generalInfoRecord.mcs_payeeSex);
            person.SetAttributeValue<string>("udo_dobstr", generalInfoRecord.mcs_payeeBirthDate);

            string payeeCode;

            if (request == null) throw new ArgumentNullException("Request is Null");

            if (!string.IsNullOrEmpty(generalInfoRecord.mcs_awardTypeCode))
            {
                request.awardTypeCd = generalInfoRecord.mcs_awardTypeCode;
                person["udo_awardtypecode"] = generalInfoRecord.mcs_awardTypeCode;
            }

            request.awardTypeCd = generalInfoRecord.mcs_awardTypeCode;
            request.ptcpntBeneId = generalInfoRecord.mcs_ptcpntBeneID;
            request.ptcpntRecipId = generalInfoRecord.mcs_ptcpntRecipID;
            request.ptcpntVetId = generalInfoRecord.mcs_ptcpntVetID;

            if (String.IsNullOrEmpty(request.ptcpntRecipId))
            {
                UpdateProgress("There is not Recipient ID, so there is nothing to add for the award.");
                return;
            }

            var isVeteran = request.ptcpntVetId.Equals(request.ptcpntRecipId, StringComparison.OrdinalIgnoreCase);

            person.SetAttributeValue<string>("udo_benefittypename", generalInfoRecord.mcs_benefitTypeName);
            person.SetAttributeValue<string>("udo_benefittypecode", generalInfoRecord.mcs_benefitTypeCode);

            payeeCode = generalInfoRecord.mcs_payeeTypeCode;

            if (request != null && request.UDOcreateAwardsInfo != null && request.UDOcreateAwardsInfo.Length > 0)
            {
                var awardInfo = request.UDOcreateAwardsInfo.FirstOrDefault(a =>
                    a.mcs_ptcpntRecipId == request.ptcpntRecipId &&
                    a.mcs_ptcpntBeneId == request.ptcpntBeneId &&
                    a.mcs_ptcpntVetId == request.ptcpntVetId &&
                    a.mcs_payeeCd == payeeCode);

                if (awardInfo != null)
                {
                    person["udo_awardid"] = new EntityReference("udo_award", awardInfo.newUDOcreateAwardsId);
                }
            } 
            
            if (!string.IsNullOrEmpty(payeeCode) && !isVeteran) person.Name = generalInfoRecord.mcs_payeeName;

            if (isVeteran)
            {
                payeeCode = "00";
                //AddVeteranInformation(UpdateProgress, request, response, person);
            }

            person["udo_payeecode"] = payeeCode;
            person["udo_payeetypecode"] = payeeCode;
            //if (UpdateProgress != null) UpdateProgress(person.DumpToString("Single Award Person"));
            people.Add(person);
        }

        /// <summary>
        /// People_AddFromMultipleAwards: Loops through the awards in the Award Information (generalInfoRecord)
        /// and then copies the sourcePerson to a new person, updates the new person with information from the
        /// award and then adds them to the people collection.
        /// </summary>
        /// <param name="UpdateProgress">UpdateProgress method to record status updates</param>
        /// <param name="request">LOB Create People Payee Request</param>
        /// <param name="response">The response to the LOB in case there was a fatal error.</param>
        /// <param name="people">A collection of people to add the awards people to.</param>
        /// <param name="generalInfoRecord">The Award Information</param>
        /// <param name="sourcePerson">The person with shared veteran/award information that will be copied</param>
        private void People_AddFromMultipleAwards(ProgressSetter UpdateProgress, UDOCreatePeoplePayeeRequest request,
            UDOCreatePeoplePayeeResponse response, IList<UDOPerson> people, VIMTfgenFNreturnclms generalInfoRecord, Entity snapshot)
        {
            #region Map Multiple
            if (UpdateProgress != null) UpdateProgress("Starting Awards Map Multiple");

            if (request == null) throw new ArgumentNullException("Request is Null");
            bool isVeteran;
            foreach (var generalInfoSelectionItem in generalInfoRecord.VIMTfgenFNawardBenesclmsInfo)
            {
                var person = new UDOPerson();

                person.SetAttributeValue<EntityReference>("ownerid", request.ownerType, request.ownerId);
                person["udo_idproofid"] = new EntityReference("udo_idproof", request.idProofId);

                if (request.vetsnapshotId != Guid.Empty)
                {
                    person["udo_vetsnapshotid"] = request.vetsnapshotId.ToString();
                    person["udo_veteransnapshotid"] = new EntityReference(
                        "udo_veteransnapshot",
                        request.vetsnapshotId);
                }

                request.awardTypeCd = generalInfoSelectionItem.mcs_awardTypeCd;
                request.ptcpntBeneId = generalInfoSelectionItem.mcs_ptcpntBeneId;
                request.ptcpntRecipId = generalInfoSelectionItem.mcs_ptcpntRecipId;
                request.ptcpntVetId = generalInfoSelectionItem.mcs_ptcpntVetId;

                person["udo_awardsexist"] = true;
                //person["udo_filenumber"] = request.fileNumber;
                //person["udo_ssn"] = request.udo_ssn;
                person["udo_vetssn"] = SecurityTools.ConvertToUnsecureString(_ssn);
                person["udo_ptcpntid"] = request.ptcpntRecipId;

                person.SetAttributeValue<string>("udo_vetlastname", snapshot.GetAttributeValue<string>("udo_lastname"));
                person.SetAttributeValue<string>("udo_vetfirstname", snapshot.GetAttributeValue<string>("udo_firstname"));


                isVeteran = request.ptcpntVetId.Equals(request.ptcpntRecipId);

                var payeeCode = generalInfoSelectionItem.mcs_payeeCd;

                if (request != null && request.UDOcreateAwardsInfo != null && request.UDOcreateAwardsInfo.Length > 0)
                {
                    var awardInfo = request.UDOcreateAwardsInfo.FirstOrDefault(a =>
                        a.mcs_ptcpntRecipId == request.ptcpntRecipId &&
                        a.mcs_ptcpntBeneId == request.ptcpntBeneId &&
                        a.mcs_ptcpntVetId == request.ptcpntVetId &&
                        a.mcs_payeeCd == payeeCode &&
                        a.mcs_awardTypeCd == "CPL");

                    if(awardInfo == null)
                        awardInfo = request.UDOcreateAwardsInfo.FirstOrDefault(a =>
                        a.mcs_ptcpntRecipId == request.ptcpntRecipId &&
                        a.mcs_ptcpntBeneId == request.ptcpntBeneId &&
                        a.mcs_ptcpntVetId == request.ptcpntVetId &&
                        a.mcs_payeeCd == payeeCode);


                    if (awardInfo != null)
                    {
                        person["udo_awardid"] = new EntityReference("udo_award", awardInfo.newUDOcreateAwardsId);


                        if (isVeteran)
                        {
                            person.SetAttributeValue<string>("udo_benefittypename", awardInfo.mcs_awardBeneTypeName);
                            person.SetAttributeValue<string>("udo_benefittypecode", awardInfo.mcs_awardBeneTypeCd);
                            person.SetAttributeValue<string>("udo_awardtypecode", awardInfo.mcs_awardTypeCd);
                        }
                    }
                }

                if (!isVeteran)
                {
                    person.SetAttributeValue<string>("udo_benefittypename", generalInfoSelectionItem.mcs_awardBeneTypeName);
                    person.SetAttributeValue<string>("udo_benefittypecode", generalInfoSelectionItem.mcs_awardBeneTypeCd);
                    person.SetAttributeValue<string>("udo_awardtypecode", generalInfoSelectionItem.mcs_awardTypeCd);
                }

                
                if (isVeteran && string.IsNullOrEmpty(payeeCode))
                {
                    payeeCode = "00";
                    if (UpdateProgress != null) UpdateProgress("No Veteran PayeeCode Found, Defaulting to {0} for Veteran Record.", payeeCode);
                }

                person["udo_payeecode"] = payeeCode;
                person["udo_payeetypecode"] = payeeCode;
                if (!string.IsNullOrEmpty(generalInfoSelectionItem.mcs_recipName))
                {
                    person.Name = generalInfoSelectionItem.mcs_recipName;
                }
                //if (UpdateProgress != null) UpdateProgress(person.DumpToString("Multiple Award Person"));
                people.Add(person);
            }

            #endregion
        }

        /// <summary>
        /// People_AddFromMultipleClaims: Traverse through the claims in the participantBenefitClaimRecordInfo
        /// and add the people to the collection, including some temporary information with the claim id for
        /// use later.
        /// </summary>
        /// <param name="UpdateProgress">UpdateProgress method to record status updates</param>
        /// <param name="participantBenefitClaimRecordInfo">The Claims Information</param>
        /// <param name="people">A collection of people to add the claims people to.</param>
        private static void People_AddFromMultipleClaims(ProgressSetter UpdateProgress, VIMTparticipantRecordbclm participantBenefitClaimRecordInfo, IList<UDOPerson> people)
        {
            if (UpdateProgress != null) UpdateProgress("Starting Claims Map Multiple");
            
            foreach (var claimRecordInfo in participantBenefitClaimRecordInfo.VIMTselectionbclmInfo)
            {
                
                var person = new UDOPerson();
                var addPerson = true;

                // e.SetAttributeValue("personOrOrganization",
                // claimRecordInfo.mcs_claimantPersonOrOrganizationIndicator);
                var first = person.SetAttributeValue<string>("udo_first", claimRecordInfo.mcs_claimantFirstName.Trim());
                var last = person.SetAttributeValue<string>("udo_last", claimRecordInfo.mcs_claimantLastName.Trim());
                var middle = person.SetAttributeValue<string>("udo_middle", claimRecordInfo.mcs_claimantMiddleName.Trim());

                // person.SetAttributeValue<string>("rem_suffix", claimRecordInfo.mcs_claimantSuffix);
                var orgname = person.SetAttributeValue<string>("rem_organizationname", claimRecordInfo.mcs_organizationTitleTypeName);
                if (!string.IsNullOrEmpty(orgname))
                {
                    person["udo_type"] = new OptionSetValue(752280003);
                }
                else
                {
                    person["udo_type"] = new OptionSetValue(752280002);
                }
                
                string name;
                if (!String.IsNullOrEmpty(orgname)) {
                    if (UpdateProgress != null) UpdateProgress("Setting Name to Organization: {0}", orgname);
                    person.Name = name = orgname;
                    if (person.Contains("udo_first")) person.Attributes.Remove("udo_first");
                    if (person.Contains("udo_last")) person.Attributes.Remove("udo_last");
                } else {
                    name = string.Format("{0} {1}", first, last).Trim();
                    person.Name = name;
                }
                
                person.SetAttributeValue<string>("udo_payeetypecode", claimRecordInfo.mcs_payeeTypeCode);
                var payeeCode = person.SetAttributeValue<string>("udo_payeecode", claimRecordInfo.mcs_payeeTypeCode);

                person["udo_pendingclaimsexist"] = false;

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(payeeCode))
                {
                    foreach (var claimPerson in people)
                    {
                        if (!String.IsNullOrEmpty(claimPerson.Name)) continue;
                        if (!String.IsNullOrEmpty(claimPerson.PayeeCode)) continue;
                        if (!name.Equals(person.Name, StringComparison.OrdinalIgnoreCase)) continue;
                        if (!payeeCode.Equals(claimPerson["udo_payeecode"].ToString(), StringComparison.OrdinalIgnoreCase)) continue;
                        person = claimPerson;
                        addPerson = false;
                        break;
                    }
                }

                // The claim receive date is used to ensure that we are using the claim id from the most
                // recent claim.
                var oldDateTime = new DateTime(1, 1, 1);
                if (person.Contains("rem_claimreceivedate"))
                {
                    oldDateTime = (DateTime)person["rem_claimreceivedate"];
                }

                var newDateTime = person.SetAttributeValue<DateTime?>(
                    "rem_claimreceivedate",
                    claimRecordInfo.mcs_claimReceiveDate);
                if (oldDateTime < newDateTime)
                {
                    person.SetAttributeValue<string>("rem_benefitclaimid", claimRecordInfo.mcs_benefitClaimID);
                }

                if (claimRecordInfo.mcs_statusTypeCode != string.Empty)
                {
                    switch (claimRecordInfo.mcs_statusTypeCode.ToLower())
                    {
                        case "clr":
                            break;
                        case "clsd":
                            break;
                        case "can":
                            break;
                        default:
                            person["udo_pendingclaimsexist"] = true;
                            person["rem_pendingclaims"] = (person.Contains("rem_pendingclaims")
                                                                     ? (int)person["rem_pendingclaims"]
                                                                     : 0) + 1;
                            break;
                    }
                }

                if (addPerson)
                {
                    //if (UpdateProgress != null) UpdateProgress(person.DumpToString("Multiple Claims Person"));
                    people.Add(person);
                }
            }
        }

        /// <summary>
        /// People_AddFromSingleClaim: Add a person from a single claim to the people collection.
        /// </summary>
        /// <param name="UpdateProgress">UpdateProgress method to record status updates</param>
        /// <param name="participantBenefitClaimRecordInfo">The Claims Information</param>
        /// <param name="people">A collection of people to add the claim person to.</param>
        private static void People_AddFromSingleClaim(ProgressSetter UpdateProgress, VIMTparticipantRecordbclm participantBenefitClaimRecordInfo, IList<UDOPerson> people)
        {
            if (UpdateProgress != null) UpdateProgress("Starting Claims Map Singleton");
            var claimRecordInfo = participantBenefitClaimRecordInfo;
            var person = new UDOPerson();

            // e.SetAttributeValue("personOrOrganization",
            // claimRecordInfo.mcs_claimantPersonOrOrganizationIndicator);
            var first = person.SetAttributeValue<string>("udo_first", claimRecordInfo.mcs_claimantFirstName.Trim());
            var last = person.SetAttributeValue<string>("udo_last", claimRecordInfo.mcs_claimantLastName.Trim());
            var middle = person.SetAttributeValue<string>("udo_middle", claimRecordInfo.mcs_claimantMiddleName.Trim());

            // person.SetAttributeValue<string>("rem_suffix", claimRecordInfo.mcs_claimantSuffix);
            var orgname = person.SetAttributeValue<string>("rem_organizationname", claimRecordInfo.mcs_organizationName);
            if (!string.IsNullOrEmpty(orgname))
            {
                person["udo_type"] = new OptionSetValue(752280003);
            }
            else
            {
                person["udo_type"] = new OptionSetValue(752280002);
            }

            var name = string.IsNullOrEmpty(orgname) ? string.Format("{0} {1}", first, last).Trim() : orgname;

            person.Name = name;

            person.SetAttributeValue<DateTime?>("rem_claimreceivedate", claimRecordInfo.mcs_claimReceiveDate);
            person.SetAttributeValue<string>("rem_benefitclaimid", claimRecordInfo.mcs_benefitClaimID);
            person.SetAttributeValue<string>("udo_payeecode", claimRecordInfo.mcs_payeeTypeCode);
            person.SetAttributeValue<string>("udo_payeetypecode", claimRecordInfo.mcs_payeeTypeCode);
            
            person["udo_pendingclaimsexist"] = false;
            if (claimRecordInfo.mcs_statusTypeCode != string.Empty)
            {
                switch (claimRecordInfo.mcs_statusTypeCode.ToLower())
                {
                    case "clr":
                        break;
                    case "clsd":
                        break;
                    case "can":
                        break;
                    default:
                        person["udo_pendingclaimsexist"] = true;
                        person["rem_pendingclaims"] = (person.Contains("rem_pendingclaims")
                                                                 ? (int)person["rem_pendingclaims"]
                                                                 : 0) + 1;
                        break;
                }
            }
            //if (UpdateProgress != null) UpdateProgress(person.DumpToString("Single Claims Person"));
            people.Add(person);
        }

        /// <summary>
        /// UpdateProgress: This method is simple, it appends the log with information passed to it
        /// </summary>
        /// <param name="progress">A composite format string with a progress update.</param>
        /// <param name="args">The object(s) to format.</param>
        internal void UpdateProgress(string progress, params object[] args)
        {
            try
            {

                var method = MethodInfo.GetCallingMethod(false).ToString(true);
                string progressString = progress;
                if (args.Length > 0) progressString = string.Format(progress, args);
                if (sr_log == null) sr_log = new StringBuilder();
                sr_log.AppendFormat("Progress:[{0}]: {1}\r\n", method, progressString);
            }
            catch (Exception ex)
            {
                // This should not happen - if it does, then the log is not updated.
            }
        }
        

        /// <summary>
        /// PayeeCode_Create: Build the Entity to create as part of the request Collection.  If there is a
        /// person to create, then add it as a related entity so that it is created as part of the create
        /// of the payee code.
        /// </summary>
        /// <param name="UpdateProgress">UpdateProgress method to record status updates</param>
        /// <param name="request">LOB Create People Payee Request</param>
        /// <param name="person">The person to create a payee code for</param>
        /// <param name="createPerson">True to create the person, False to not create the person</param>
        /// <param name="isVeteran">Null if unknown, True if Veteran, False if not Veteran</param>
        /// <returns>The payee code entity to create, with person attached if possible.</returns>
        private static Entity PayeeCode_Create(
            ProgressSetter UpdateProgress,
            UDOCreatePeoplePayeeRequest request,
            Entity person, bool createPerson = true, bool? isVeteran = null)
        {
            if (UpdateProgress != null) UpdateProgress("Creating PayeeCode");
            var payeeCode = person["udo_payeecode"].ToString();
            if (UpdateProgress != null) UpdateProgress("PayeeCode: {0}", payeeCode);

            var participantId = person.GetAttributeValue<string>("udo_ptcpntid");

            // Can't create a payeecode for people with a pid
            #region Determine if the person or payeecode is for a Veteran
            bool blnIsVeteran = false;
            if (isVeteran.HasValue)
            {
                blnIsVeteran = isVeteran.Value;
            } else if (!string.IsNullOrEmpty(participantId) && !string.IsNullOrEmpty(request.ptcpntVetId))
            {
                blnIsVeteran = participantId.Equals(request.ptcpntVetId.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            else if (!string.IsNullOrEmpty(person.GetAttributeValue<string>("udo_ssn"))
              && !string.IsNullOrEmpty(person.GetAttributeValue<string>("udo_vetssn")))
            {
                blnIsVeteran = person["udo_ssn"].ToString().Equals(person["udo_vetssn"].ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (UpdateProgress != null) UpdateProgress("Is Veteran: {0}", blnIsVeteran);
            #endregion

            var recipientName = person.GetAttributeValue<string>("udo_payeename") ?? string.Empty;
            
            var entityPayeeCode = new Entity("udo_payeecode");
            if (request.ownerId != Guid.Empty)
            {
                entityPayeeCode["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
            }

            var payeeCodeName = PayeeCode_GenerateName(payeeCode, recipientName, blnIsVeteran);
            if (UpdateProgress != null) UpdateProgress("Setting PayeeCodeName: {0}", payeeCodeName);
            entityPayeeCode["udo_name"] = payeeCodeName;

            if (request.idProofId != Guid.Empty)
            {
                entityPayeeCode["udo_idproofid"] = new EntityReference("udo_idproof", request.idProofId);
            }

            entityPayeeCode["udo_payeecode"] = payeeCode;
            if (!String.IsNullOrEmpty(request.fileNumber))
            {
                entityPayeeCode["udo_filenumber"] = request.fileNumber;
            }
            entityPayeeCode.SetAttributeValue<string>("udo_participantid", participantId);
            
            if (request.udo_contactId != Guid.Empty)
            {
                entityPayeeCode["udo_veteranid"] = new EntityReference("contact", request.udo_contactId);
            }

            if (request.UDOcreatePayeeRelatedEntitiesInfo != null)
            {
                foreach (var relatedItem in request.UDOcreatePayeeRelatedEntitiesInfo)
                {
                    entityPayeeCode[relatedItem.RelatedEntityFieldName] =
                        new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                }
            }

            // Attach the person to create as part of the payeecode creation
            if (createPerson)
            {
                entityPayeeCode.RelatedEntities.Add(
                    new Relationship("udo_udo_payeecode_udo_person_payeecodeid"),
                    new EntityCollection(new[] { person }));
            }

            return entityPayeeCode;
        }

        public static string PayeeCode_GenerateName(string payeeCode, string recipient, bool isVeteran = false)
        {
            string name;
            if (isVeteran)
            {
                return "Veteran - 00";
            }
            switch (payeeCode)
            {
                case "00": name = "Non-Veteran {0}"; break;
                case "10": name = "Spouse {0}"; break;
                case "11":
                case "12":
                case "13":
                case "14":
                case "15":
                case "16":
                case "17":
                case "18":
                case "19":
                case "20": name = "Child {0}"; break;
                case "41": name = "CH35 First Child"; break;
                case "42": name = "CH35 Second Child"; break;
                case "43": name = "CH35 Third Child"; break;
                case "44": name = "CH35 Fourth Child"; break;
                case "45": name = "CH35 Fifth Child"; break;
                case "50": name = "Dependent Father"; break;
                case "60": name = "Dependent Mother"; break;
                default: name = "Other {0}"; break;
            }

            return string.Format("{0} - {1}", string.Format(name, recipient), payeeCode);
        }

        private static UDOcreatePaymentsRelatedEntitiesMultipleRequest CreatePaymentRelation(string entity, string field, Guid id)
        {
            return new UDOcreatePaymentsRelatedEntitiesMultipleRequest
            {
                RelatedEntityId = id,
                RelatedEntityName = entity,
                RelatedEntityFieldName = field
            };
        }

        private static void LoadPayments(ProgressSetter UpdateProgress, OrganizationServiceProxy OrgServiceProxy, UDOCreatePeoplePayeeRequest request, UDOcreatePaymentsRequest createPaymentsRequest)
        {
            if (UpdateProgress != null) UpdateProgress("Not getting payment in peoplelist anymore");
            return;


            if (UpdateProgress != null) UpdateProgress("Loading Payments");
            var method = MethodInfo.GetThisMethod().ToString();
            var fetch = "<fetch count='1'><entity name='udo_person'>" + 
                "<attribute name='udo_payeecodeid'/>" +
                "<attribute name='udo_personid'/>" +
                "<filter>" +
                "<condition attribute='udo_payeecode' operator='eq' value='00'/>" +
                "<condition attribute='udo_idproofid' operator='eq' value='" + request.idProofId + "'/>" +
                "<condition attribute='udo_ptcpntid' operator='eq' value='" + request.ptcpntVetId + "'/>" +
                "</filter></entity></fetch>";

            if (UpdateProgress != null) UpdateProgress("Executing Fetch: \r\n\r\n{0}\r\n\r\n", fetch);

            OrgServiceProxy.CallerId = request.UserId;
            var people = OrgServiceProxy.RetrieveMultiple(new FetchExpression(fetch));
            if (people == null || people.Entities.Count == 0)
            {
                if (UpdateProgress != null) UpdateProgress("Unable to find Payee Code 00 for veteran to load payments.");
                return;
            }

            var person = people.Entities[0];

            if (!person.Contains("udo_payeecodeid")) {
                if (UpdateProgress != null) UpdateProgress("Unable to find Payee Code 00 for veteran to load payments.");
                return;
            }


            var payeeCodeIdRef = (EntityReference)person["udo_payeecodeid"];
            var payeeCodeId = payeeCodeIdRef.Id;
            if (UpdateProgress != null) UpdateProgress("Veteran PayeeCode ID: {0}", payeeCodeId);

            var personId = person.Id;
            if (UpdateProgress != null) UpdateProgress("Veteran Person ID: {0}", personId);

            #region complete uild of createPaymentsRequest for 00 payeecode

            long pid = 0;
            if (long.TryParse(request.ptcpntVetId, out pid))
            {
                createPaymentsRequest.ParticipantId = pid;
            }
            createPaymentsRequest.udo_personId = personId;
            createPaymentsRequest.udo_payeecodeId = payeeCodeId;
            createPaymentsRequest.PaymentId = new long();

            // Build Relationships
            var udo_veteranReference = CreatePaymentRelation("contact", "udo_veteranid", request.udo_contactId);
            var udo_paymentReference = CreatePaymentRelation("udo_payeecode", "udo_payeecodeid", payeeCodeId);
            var udo_peopleReference = CreatePaymentRelation("udo_person", "udo_personid", personId);

            if (createPaymentsRequest.vetsnapshotId != Guid.Empty)
            {
                var udo_vetsnapshotReference = CreatePaymentRelation("udo_veteransnapshot", "udo_veteransnapshotid", createPaymentsRequest.vetsnapshotId);
                createPaymentsRequest.UDOcreatePaymentsRelatedEntitiesInfo =
                    new[] { udo_veteranReference, udo_paymentReference, udo_vetsnapshotReference, udo_peopleReference };
            }
            else
            {
                createPaymentsRequest.UDOcreatePaymentsRelatedEntitiesInfo =
                    new[] { udo_veteranReference, udo_paymentReference, udo_peopleReference };
            }
            if (request.LegacyServiceHeaderInfo != null)
            {
                createPaymentsRequest.LegacyServiceHeaderInfo = new Payments.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }
            #endregion
            createPaymentsRequest.IDProofOrchestration = true;
           
            //if (request.Debug && UpdateProgress!=null)
            //{
            //    UpdateProgress("Sending Request to Create Payments: \r\n{0}\r\n\r\n", createPaymentsRequest.SerializeToString());
            //}

                  
            //RC NEW - don't need this anymore
            //var response = createPaymentsRequest.SendReceive<UDOcreatePaymentsResponse>(MessageProcessType.Local);
            //createPaymentsRequest.SendAsync(MessageProcessType.Local);
            //if (UpdateProgress != null)
            //{
              //  UpdateProgress("Response: {0}", response.SerializeToString());
            //}
            //if (UpdateProgress != null) UpdateProgress("CreatePaymentsRequest Sent Async");

            //UDORetrievePaymentsRequest paymentRequest = new UDORetrievePaymentsRequest();
            //paymentRequest.payeeCodeId = payeeCodeId;
            //paymentRequest.OrganizationName = request.OrganizationName;
            //paymentRequest.Debug = request.Debug;
            //paymentRequest.UserId = request.UserId;
            //paymentRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            //paymentRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            //paymentRequest.RelatedParentId = request.RelatedParentId;
            //paymentRequest.Debug = request.Debug;


            //paymentRequest.SendAsync(MessageProcessType.Local);
            if (UpdateProgress != null) UpdateProgress("Payment Load Started");
        }
             
    }
}