using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Diagnostics;
using System.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Common;
using UDO.LOB.MVI.Messages;
using UDO.LOB.MVI.Processors;
using UDO.LOB.VeteranSnapShot.Messages;
using MVIMessages = VEIS.Mvi.Messages;

namespace VRM.Integration.UDO.MVI.Processors
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewe By Brian Greig - 4/30/15
    /// </remarks>
    public class UDOCHATPersonSearchProcessor
    {
        private const string MVISearchResultsWithMatch = "A search in MVI found {0} matching record(s). ";
        private const string MVISearchResultsWithNoMatch = "A search in MVI did not find any records matching the search criteria. ";
        private const string MVISearchResultsWithTooManyMatches = "A search in MVI returned more than the allowable number of matches. ";
        private const string MVISearchUnknownError = "An unknown error was returned from MVI. ";
        private const string MVISearchConnectionError = "A connection error was encountered performing the MVI search. ";

        // CSDev REM: New variables
        private const string method = "UDOCHATPersonSearchProcessor";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IMessageBase Execute(UDOCHATPersonSearchRequest request)
        {
            UDOCombinedPersonSearchResponse response = null;
            Guid owningTeamId = Guid.Empty;
            try
            {
                //CSDev REM
                LogHelper.LogDebug(request.MessageId,  request.OrganizationName, request.UserId, $"{GetType().FullName}.Execute", "Top", request.Debug);

                Stopwatch txnTimer = Stopwatch.StartNew();
                Stopwatch EntireTimer = Stopwatch.StartNew();

                if (request == null)
                {
                    //CSDev REM
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, $"{GetType().FullName}.Execute", string.Format("{0} recieved a null message", GetType().FullName));
                }
                else
                {
                    CRMCommonFunctions CommonFunctions = new CRMCommonFunctions();

                    //CSDev REM

                    #region connect to CRM
                    CrmServiceClient OrgServiceProxy = null;

                    try
                    {
                        OrgServiceProxy = ConnectionCache.GetProxy();
                    }
                    catch (Exception connectException)
                    {
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);

                        if (response != null)
                        {
                            response.ExceptionOccurred = true;
                        }

                        return response;
                    }
                    #endregion

                    using (OrgServiceProxy)
                    {
                        if (request.SSId == null)
                        {
                            request.SSId = request.SSIdString.ToSecureString();
                        }
                        #region - do the real MVI search - MVI handles the whole attended versus unattended thing
                        if (request.BypassMvi == false)
                        {
                            //map from the MVI response to our response, which looks the same.
                            MVIMessages.RetrieveOrSearchPersonResponse personSearhResponse;
                            if (request.IsAttended)
                            {
                                //can't do attended 
                                personSearhResponse = new MVIMessages.RetrieveOrSearchPersonResponse();
                            }
                            else
                            {
                                MVIMessages.UnattendedSearchRequest retrievePersonRequest = new MVIMessages.UnattendedSearchRequest(request.Edipi
                                    , request.OrganizationName, request.UserFirstName, request.UserLastName, request.UserId, request.MessageId);

                                //CSDev Rem
                                personSearhResponse = WebApiUtility.SendReceive<MVIMessages.RetrieveOrSearchPersonResponse>(retrievePersonRequest, WebApiType.VEIS);
                                if (request.LogSoap || personSearhResponse.ExceptionOccurred)
                                {
                                    if (personSearhResponse.SerializedSOAPRequest != null || personSearhResponse.SerializedSOAPResponse != null)
                                    {
                                        var requestResponse = personSearhResponse.SerializedSOAPRequest + personSearhResponse.SerializedSOAPResponse;
                                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"UnattendedSearchRequest Request/Response {requestResponse}", true);
                                    }
                                }

                                txnTimer.Stop();

                                txnTimer.Restart();
                                response = EvaluateMviResponse(personSearhResponse);
                                //CSDev Misspelled 
                                personSearhResponse.ExceptionOccured = response.ExceptionOccurred;
                                personSearhResponse.Message = response.MVIMessage;
                                personSearhResponse.RawMviExceptionMessage = response.RawMviExceptionMessage;

                            }
                            //person result from MVI
                            if (personSearhResponse.Person != null)
                            {

                                response = MaptoResponse(personSearhResponse, request);
                                //for EDIPI searches, SSN input field is blank. If Traits search, use request SSN for output
                                foreach (PatientPerson item in response.Person)
                                {
                                    if (item.SSId != null && item.SSId.Length == 0)
                                    {
                                        item.SSId = request.SSId;
                                    }
                                }
                            }
                            else if (request.IsAttended == false)
                            {
                                //if EDIPI search comes up blank, just return response...no need to go back to Corp since no SSN
                                if (request.SSId == null)
                                {
                                    if (string.IsNullOrEmpty(request.ParticipantId))
                                    {
                                        return response;
                                    }
                                }
                            }
                            else
                            {
                                //no person result from MVI
                                if (request.ParticipantId != null)
                                {
                                    UDOgetVeteranIdentifiersRequest getVeteranIdentifiers = new UDOgetVeteranIdentifiersRequest();
                                    getVeteranIdentifiers.MessageId = request.MessageId;
                                    getVeteranIdentifiers.LogTiming = request.LogTiming;
                                    getVeteranIdentifiers.LogSoap = request.LogSoap;
                                    getVeteranIdentifiers.Debug = request.Debug;
                                    getVeteranIdentifiers.RelatedParentEntityName = request.RelatedParentEntityName;
                                    getVeteranIdentifiers.RelatedParentFieldName = request.RelatedParentFieldName;
                                    getVeteranIdentifiers.RelatedParentId = request.RelatedParentId;
                                    getVeteranIdentifiers.UserId = request.UserId;
                                    getVeteranIdentifiers.ptcpntId = Convert.ToInt64(request.ParticipantId);
                                    getVeteranIdentifiers.OrganizationName = request.OrganizationName;
                                    //CSDev REM 
                                    getVeteranIdentifiers.LegacyServiceHeaderInfo = new UDOHeaderInfo
                                    {
                                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                    };
                                    UDOgetVeteranIdentifiersResponse getVeteranIdentifiersResponse = new UDOgetVeteranIdentifiersResponse();

                                    UDOgetVeteranIdentifiersProcessor getveteranIdentifiersLogic = new UDOgetVeteranIdentifiersProcessor();
                                    getVeteranIdentifiersResponse = (UDOgetVeteranIdentifiersResponse)getveteranIdentifiersLogic.Execute(getVeteranIdentifiers);

                                    if (getVeteranIdentifiersResponse != null)
                                    {
                                        if (getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo != null)
                                        {
                                            request.SSId = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SSN.ToSecureString();
                                        }
                                    }
                                }

                                response = HandleEmptySearchResponse(request, response);
                                txnTimer.Stop();
                                txnTimer.Restart();
                            }
                        }
                        else
                        {
                            //Bypass MVI for debug purposes
                            response = new UDOCombinedPersonSearchResponse();
                            response = HandleEmptySearchResponse(request, response);
                            response.MVIMessage = MVISearchResultsWithNoMatch;
                            txnTimer.Stop();
                            txnTimer.Restart();
                        }

                        #endregion

                        //new logic for 1 person
                        if (response != null && response.Person != null)
                        {
                            if (response.Person.Count() == 1)
                            {
                                if (response.Person[0].SSId == null)
                                {
                                    response.Person[0].SSId = response.Person[0].SSIdString.ToSecureString();
                                }
                                #region create ID Proof

                                #region if Source is MVI, first thing we do is go to corp to add in items that aren't in MVI, like BOS and Gender
                                if (response.Person[0].RecordSource == "MVI")
                                {
                                    UDOSelectedPersonRequest newRequest = new UDOSelectedPersonRequest();
                                    newRequest.RecordSource = response.Person[0].RecordSource;
                                    GetICN(response.Person[0], newRequest);
                                    if (!String.IsNullOrEmpty(response.Person[0].EdiPi))
                                        newRequest.Edipi = response.Person[0].EdiPi;
                                    if (request.SSId != null && request.SSId.Length > 0)
                                        newRequest.SSId = request.SSId;
                                    else
                                        newRequest.SSId = response.Person[0].SSId;

                                    newRequest.CorrespondingIdList = response.Person[0].CorrespondingIdList;

                                    newRequest.UserFirstName = request.UserFirstName;
                                    newRequest.UserLastName = request.UserLastName;
                                    newRequest.OrganizationName = request.OrganizationName;
                                    newRequest.UserId = request.UserId;
                                    newRequest.LogSoap = request.LogSoap;
                                    newRequest.Debug = request.Debug;
                                    newRequest.LogTiming = request.LogTiming;
                                    //CSDev REM
                                    newRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
                                    {
                                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                    };

                                    PatientPerson person = response.Person[0];

                                    if (person.NameList != null)
                                    {
                                        Name legalName = person.NameList.FirstOrDefault(v => v.NameType.Equals("Legal", StringComparison.OrdinalIgnoreCase));
                                        Name alias = person.NameList.FirstOrDefault(v => v.NameType.Equals("Alias", StringComparison.OrdinalIgnoreCase));

                                        if (legalName != null)
                                        {
                                            newRequest.FirstName = legalName.GivenName;
                                            newRequest.FamilyName = legalName.FamilyName;
                                            newRequest.MiddleName = legalName.MiddleName;
                                        }
                                        else
                                        {
                                            legalName = person.NameList.FirstOrDefault();

                                            if (legalName != null)
                                            {
                                                newRequest.FirstName = legalName.GivenName;
                                                newRequest.FamilyName = legalName.FamilyName;
                                                newRequest.MiddleName = legalName.MiddleName;
                                            }
                                        }
                                    }

                                    newRequest.DateofBirth = person.BirthDate;
                                    newRequest.Gender = person.GenderCode;
                                    newRequest.BranchOfService = person.BranchOfService;

                                    long PID;
                                    Int64.TryParse(person.ParticipantId, out PID);
                                    if (PID != null && PID != 0)
                                        newRequest.participantID = PID;

                                    newRequest.IsAttended = request.IsAttended;

                                    UDOSelectedPersonProcessor selectedPersonLogic = new UDOSelectedPersonProcessor();
                                    UDOSelectedPersonResponse thisNewResponse = (UDOSelectedPersonResponse)selectedPersonLogic.Execute(newRequest);

                                    txnTimer.Stop();
                                    txnTimer.Restart();
                                    if (thisNewResponse == null || thisNewResponse.ExceptionOccured == true)
                                    {
                                        //if the orchestration, findVeteran or FindInCrm fail, send the message back, but don't return the person since there will be no ID Proof or contactId
                                        response.MVIMessage = "Search failed. " + thisNewResponse.Message;
                                        response.RawMviExceptionMessage = thisNewResponse.RawMviExceptionMessage;
                                        response.Person = null;
                                        //CSDev Rem Misspelled
                                        response.ExceptionOccurred = true;
                                        return response;
                                    }

                                    if (person.VeteranSensitivityLevel == null || person.VeteranSensitivityLevel.Trim() == "")
                                        person.VeteranSensitivityLevel = thisNewResponse.VeteranSensitivityLevel;

                                    if (response.Person[0].VeteranSensitivityLevel != null)
                                    {
                                        int realSL = 0;
                                        if (Int32.TryParse(response.Person[0].VeteranSensitivityLevel, out realSL))
                                        {
                                            if (request.userSL < realSL)
                                            {
                                                response.MVIMessage = string.Format("In UDO your user record is set to a lower Sensitivity Level than CSS. To service this veteran you must have an SL of {0} or higher. Please contact your Administrator to have your level changed.  In the meanwhile, you will have to transfer this call. ", realSL);
                                                response.Person = null;
                                                //CSDev Misspelled
                                                response.ExceptionOccurred = true;
                                                return response;
                                            }
                                        }
                                    }

                                    ///TODO: Check the duplicate flag and if set then we need to call UDOhandleDuplicateCorpRecordsProcessor

                                    if (thisNewResponse.DuplicateCorpPID)
                                    {
                                        ///Handle Duplicate Corp records identified by MVI Corresponding IDs list

                                        if (thisNewResponse.CorrespondingIdsResponse != null)
                                        {
                                            HandleDupCorpRecordProcessor handleDuplicatesProcessor = new HandleDupCorpRecordProcessor();
                                            UDOHandleDupCorpRecordResponse handleDuplicateResponse = (UDOHandleDupCorpRecordResponse)handleDuplicatesProcessor.Execute(thisNewResponse, newRequest, response.Person[0]);

                                            response.Person = handleDuplicateResponse.Person;
                                            response.CORPDbMessage = handleDuplicateResponse.CORPDbMessage;
                                            response.UDOMessage = handleDuplicateResponse.UDOMessage;
                                            //CSDev Misspelled 
                                            response.ExceptionOccurred = handleDuplicateResponse.ExceptionOccurred;

                                            return response;
                                        }
                                    }

                                    if (person.BranchOfService == null || person.BranchOfService.Trim() == "")
                                        person.BranchOfService = thisNewResponse.BranchOfService;
                                    if (person.GenderCode == null || person.GenderCode.Trim() == "")
                                        person.GenderCode = thisNewResponse.GenderCode;
                                    if (person.PhoneNumber == null || person.PhoneNumber.Trim() == "")
                                        person.PhoneNumber = thisNewResponse.phonenumber;
                                    if (person.EdiPi == null || person.EdiPi.Trim() == "")
                                        person.EdiPi = thisNewResponse.Edipi;
                                    if (!String.IsNullOrEmpty(person.EdiPi))
                                    {
                                        // This only runs if EDIPI is present...
                                        Rank.GetPersonsRank(request.Debug, request.LogSoap, request.LogTiming, request.OrganizationName, request.UserId, person);
                                    }
                                    if (person.FileNumber == null || person.FileNumber.Trim() == "")
                                        person.FileNumber = thisNewResponse.FileNumber;
                                    if (person.ParticipantId == null || person.ParticipantId.Trim() == "")
                                        person.ParticipantId = thisNewResponse.ParticipantId;
                                    if (person.crme_stationofJurisdiction == null || person.crme_stationofJurisdiction.Trim() == "")
                                        person.crme_stationofJurisdiction = thisNewResponse.crme_stationofJurisdiction;
                                    if (person.DeceasedDate == null || person.DeceasedDate.Trim() == "")
                                        person.DeceasedDate = thisNewResponse.crme_DateofDeath;
                                    if (person.crme_CharacterofDishcarge == null || person.crme_CharacterofDishcarge.Trim() == "")
                                        person.crme_CharacterofDishcarge = thisNewResponse.crme_CharacterofDishcarge;

                                    if (person.SSId == null)
                                    {
                                        if (thisNewResponse.SSId != null)
                                            person.SSId = thisNewResponse.SSId;
                                    }
                                    Guid? crmPersonId = CommonFunctions.TryGetCrmPerson(OrgServiceProxy, ref newRequest, "MVI");
                                    //The UDOSelectedPersonResponse does a TryGetCrmPerson, so no need to do this again
                                    if (!crmPersonId.HasValue || (crmPersonId.HasValue && crmPersonId.Value == Guid.Empty))
                                    {
                                        crmPersonId = CommonFunctions.TryCreateNewCrmPerson(OrgServiceProxy, ref newRequest, "MVI");
                                        txnTimer.Stop();
                                        txnTimer.Restart();
                                        owningTeamId = newRequest.OwningTeamId;
                                    }
                                    response.contactId = crmPersonId.Value;
                                    owningTeamId = newRequest.OwningTeamId;

                                    //CSDev Rem
                                    //CSDev Add Property from the CRMConfiguration.cs 
                                    // NP: REM: Changed to read Crm Base Url from LOB Web Config
                                    string crmBaseUrl = ConnectionCache.ConnectManager.BaseUrl;
                                    response.URL = $@"{crmBaseUrl}/main.aspx?etn=contact&pagetype=entityrecord&id=%7B{crmPersonId.Value}%7D";

                                    Entity newIDProof = new Entity();
                                    newIDProof.LogicalName = "udo_idproof";
                                    newIDProof["ownerid"] = new EntityReference("team", owningTeamId);
                                    newIDProof["udo_birthdate"] = person.BirthDate;
                                    newIDProof["udo_phonenumber"] = person.PhoneNumber;
                                    newIDProof["udo_firstname"] = newRequest.FirstName;
                                    newIDProof["udo_lastname"] = newRequest.FamilyName;
                                    newIDProof["udo_ssn"] = newRequest.SSId.ToUnsecureString();
                                    newIDProof["udo_veteran"] = new EntityReference("contact", thisNewResponse.contactId);
                                    newIDProof["udo_interaction"] = new EntityReference("udo_interaction", Guid.Parse(request.interactionId));
                                    newIDProof["udo_branchofservice"] = person.BranchOfService;
                                    newIDProof["udo_rank"] = person.Rank;
                                    newIDProof["udo_title"] =  newRequest.FamilyName + ", " + newRequest.FirstName;

                                    //CSDev TODO Ask Matt/Nithin after Connection Code remediation
                                    Guid idProofId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(request.MessageId, newIDProof, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                                    txnTimer.Stop();
                                    txnTimer.Restart();
                                    response.idProofId = idProofId;
                                    #endregion
                                }
                                else
                                {
                                    #region corp results

                                    if (response.Person[0].VeteranSensitivityLevel != null)
                                    {
                                        int realSL = 0;
                                        if (Int32.TryParse(response.Person[0].VeteranSensitivityLevel, out realSL))
                                        {
                                            if (request.userSL < realSL)
                                            {
                                                response.MVIMessage = string.Format("In UDO your user record is set to a lower Sensitivity Level than CSS. To service this veteran you must have an SL of {0} or higher. Please contact your Administrator to have your level changed.  In the meanwhile, you will have to transfer this call. ", realSL);
                                                response.Person = null;
                                                //CSDev REM Misspelled 
                                                response.ExceptionOccurred = true;
                                                return response;
                                            }
                                        }
                                    }

                                    //it came from CORP, we have everything we need
                                    UDOSelectedPersonRequest selectedPersonRequest = new UDOSelectedPersonRequest();
                                    selectedPersonRequest.FileNumber = response.Person[0].FileNumber;
                                    selectedPersonRequest.FirstName = response.Person[0].NameList[0].GivenName;
                                    selectedPersonRequest.MiddleName = response.Person[0].NameList[0].MiddleName;
                                    selectedPersonRequest.FamilyName = response.Person[0].NameList[0].FamilyName;
                                    selectedPersonRequest.SSId = response.Person[0].SSId;
                                    selectedPersonRequest.BranchOfService = response.Person[0].BranchOfService;

                                    selectedPersonRequest.participantID = Convert.ToInt64(response.Person[0].ParticipantId);
                                    selectedPersonRequest.RecordSource = "CorpdDB";
                                    selectedPersonRequest.VeteranSensitivityLevel = Convert.ToInt32(response.Person[0].VeteranSensitivityLevel);
                                    selectedPersonRequest.LogSoap = request.LogSoap;
                                    selectedPersonRequest.Debug = request.Debug;
                                    selectedPersonRequest.LogTiming = request.LogTiming;


                                    Guid? crmPersonId = CommonFunctions.TryGetCrmPerson(OrgServiceProxy, ref selectedPersonRequest, "CorpdDB");
                                    txnTimer.Stop();
                                    txnTimer.Restart();

                                    if (!crmPersonId.HasValue || (crmPersonId.HasValue && crmPersonId.Value == Guid.Empty))
                                    {
                                        crmPersonId = CommonFunctions.TryCreateNewCrmPerson(OrgServiceProxy, ref selectedPersonRequest, "CorpdDB");
                                        txnTimer.Stop();
                                        txnTimer.Restart();
                                    }
                                    response.contactId = crmPersonId.Value;
                                    owningTeamId = selectedPersonRequest.OwningTeamId;

                                    // NP: REM: Changed to read Crm Base Url from LOB Web Config
                                    string crmBaseUrl = ConnectionCache.ConnectManager.BaseUrl;
                                    response.URL = $@"{crmBaseUrl}/main.aspx?etn=contact&pagetype=entityrecord&id=%7B{crmPersonId.Value}%7D";

                                    #region do IDProof stuff
                                    Entity newIDProof = new Entity();
                                    newIDProof.LogicalName = "udo_idproof";
                                    newIDProof["ownerid"] = new EntityReference("team", owningTeamId);
                                    newIDProof["udo_birthdate"] = response.Person[0].BirthDate;
                                    newIDProof["udo_phonenumber"] = response.Person[0].PhoneNumber;
                                    newIDProof["udo_firstname"] = response.Person[0].NameList[0].GivenName;
                                    newIDProof["udo_lastname"] = response.Person[0].NameList[0].FamilyName;
                                    newIDProof["udo_ssn"] = response.Person[0].SSId.ToUnsecureString();
                                    newIDProof["udo_veteran"] = new EntityReference("contact", response.contactId);
                                    newIDProof["udo_interaction"] = new EntityReference("udo_interaction", Guid.Parse(request.interactionId));
                                    newIDProof["udo_branchofservice"] = response.Person[0].BranchOfService;
                                    newIDProof["udo_rank"] = response.Person[0].Rank;
                                    newIDProof["udo_title"] =  response.Person[0].NameList[0].FamilyName + ", " + response.Person[0].NameList[0].GivenName;

                                    Guid idProofId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(request.MessageId, newIDProof, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                                    txnTimer.Stop();
                                    LogHelper.LogTiming(request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "Created IDProof", null, txnTimer.ElapsedMilliseconds);
                                    txnTimer.Restart();

                                    response.idProofId = idProofId;
                                    #endregion
                                    #endregion
                                }
                                Entity newSnapShot = new Entity { LogicalName = "udo_veteransnapshot" };
                                newSnapShot["ownerid"] = new EntityReference("team", owningTeamId);
                                newSnapShot["udo_veteranid"] = new EntityReference("contact", response.contactId); ;
                                newSnapShot["udo_name"] = "Veteran Summary";
                                newSnapShot["udo_idproofid"] = new EntityReference("udo_idproof", response.idProofId);
                                newSnapShot["udo_phonenumber"] = response.Person[0].PhoneNumber;
                                newSnapShot["udo_firstname"] = response.Person[0].NameList[0].GivenName;
                                newSnapShot["udo_lastname"] = response.Person[0].NameList[0].FamilyName;
                                newSnapShot["udo_ssn"] = SecurityTools.ConvertToUnsecureString(response.Person[0].SSId);
                                newSnapShot["udo_characterofdischarge"] = response.Person[0].crme_CharacterofDishcarge;
                                newSnapShot["udo_dateofdeath"] = response.Person[0].DeceasedDate;
                                newSnapShot["udo_soj"] = response.Person[0].crme_stationofJurisdiction;
                                newSnapShot["udo_branchofservice"] = response.Person[0].BranchOfService;
                                newSnapShot["udo_rank"] = response.Person[0].Rank;
                                newSnapShot["udo_filenumber"] = response.Person[0].FileNumber;
                                newSnapShot["udo_gender"] = response.Person[0].GenderCode;
                                newSnapShot["udo_birthdatestring"] = response.Person[0].BirthDate;
                                newSnapShot["udo_participantid"] = response.Person[0].ParticipantId;
                                Guid SnapShotId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(request.MessageId, newSnapShot, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));

                                //CSDev Rem
                                UDOcreateUDOVeteranSnapShotRequest UDOcreateUDOVeteranSnapShotRequest = new UDOcreateUDOVeteranSnapShotRequest();
                                UDOHeaderInfo HeaderInfo = new UDOHeaderInfo()
                                {
                                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                };

                                UDOcreateUDOVeteranSnapShotRequest.MessageId = request.MessageId;
                                UDOcreateUDOVeteranSnapShotRequest.Debug = request.Debug;
                                UDOcreateUDOVeteranSnapShotRequest.LegacyServiceHeaderInfo = HeaderInfo;
                                UDOcreateUDOVeteranSnapShotRequest.LogSoap = request.LogSoap;
                                UDOcreateUDOVeteranSnapShotRequest.LogTiming = request.LogTiming;
                                UDOcreateUDOVeteranSnapShotRequest.PID = response.Person[0].ParticipantId;
                                UDOcreateUDOVeteranSnapShotRequest.fileNumber = response.Person[0].FileNumber;
                                UDOcreateUDOVeteranSnapShotRequest.UserId = request.UserId;
                                UDOcreateUDOVeteranSnapShotRequest.OrganizationName = request.OrganizationName;
                                UDOcreateUDOVeteranSnapShotRequest.udo_veteranid = response.contactId;
                                UDOcreateUDOVeteranSnapShotRequest.udo_veteransnapshotid = SnapShotId;
                                //CSDev REM
                                WebApiUtility.SendAsync(UDOcreateUDOVeteranSnapShotRequest, WebApiType.LOB);

                                UDOOpenIDProofAsyncRequest openMe = new UDOOpenIDProofAsyncRequest();
                                openMe.IDProofId = response.idProofId;
                                openMe.OrganizationName = request.OrganizationName;
                                openMe.UserId = request.UserId;
                                openMe.Debug = request.Debug;

                                openMe.LegacyServiceHeaderInfo = new UDOHeaderInfo()
                                {
                                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                };

                                openMe.MessageId = request.MessageId;
                                openMe.DiagnosticsContext = request.DiagnosticsContext;
                                WebApiUtility.SendAsync(openMe, WebApiType.LOB);

                                txnTimer.Stop();

                                LogHelper.LogTiming(request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "Open IDProof ASYNC Call", null, txnTimer.ElapsedMilliseconds);
                                txnTimer.Restart();
                                #endregion
                            }
                        }
                    }
                }
                EntireTimer.Stop();
                //CSDev REM 
                if (response != null && response.VEISFindVeteranResponse != null) response.VEISFindVeteranResponse = null;
                return response;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOCHATPersonSearchProcessor, Execute", "Exception:" + ex.Message);
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOCHATPersonSearchProcessor, Execute", ex);
                if (response == null)
                {
                    response = new UDOCombinedPersonSearchResponse();
                }
                response.MVIMessage = ex.Message;
                //CSDev REM Misspelling
                response.ExceptionOccurred = true;
                return response;
            }
        }

        private static UDOCombinedPersonSearchResponse EvaluateMviResponse(MVIMessages.RetrieveOrSearchPersonResponse response)
        {
            UDOCombinedPersonSearchResponse combinedResponse = new UDOCombinedPersonSearchResponse();

            if (response != null && response.Acknowledgement != null && response.Acknowledgement.TypeCode != null)
            {
                switch (response.Acknowledgement.TypeCode)
                {
                    case "AA":  //message processed OK by MVI
                        if (response.QueryAcknowledgement.QueryResponseCode == "NF")   //query did not return any results
                        {
                            combinedResponse.MVIMessage = MVISearchResultsWithNoMatch;
                        }
                        else if (response.QueryAcknowledgement.QueryResponseCode == "QE")     //query returned too many results
                        {
                            combinedResponse.MVIMessage = MVISearchResultsWithTooManyMatches;
                        }
                        else
                        {
                            combinedResponse.MVIMessage = response.Message;
                        }
                        break;
                    case "AE":  //error in processing the query
                    case "AR":  //error processing the message
                        if (response.Acknowledgement.AcknowledgementDetails != null && response.Acknowledgement.AcknowledgementDetails.Length > 0)
                        {
                            //the two messages below are specific to EDIPI (unattended) searches and should be treated as a "not found" error.
                            if (response.Acknowledgement.AcknowledgementDetails.Any(a => a.Text.Contains("Correlation Does Not Exist") ||
                                                                                         a.Text.Contains("No ACTIVE Correlation found")))
                            {
                                combinedResponse.MVIMessage = MVISearchResultsWithNoMatch;
                            }
                            else
                            {
                                //CSDev REM Misspelling
                                combinedResponse.ExceptionOccurred = true;
                                combinedResponse.MVIMessage = MVISearchUnknownError;
                                combinedResponse.RawMviExceptionMessage = response.Acknowledgement.AcknowledgementDetails[0].Text;
                            }
                        }
                        else
                        {
                            //CSDev REM Misspelling
                            combinedResponse.ExceptionOccurred = true;
                            combinedResponse.MVIMessage = MVISearchUnknownError;
                            combinedResponse.RawMviExceptionMessage = response.RawMviExceptionMessage;
                        }
                        break;
                    default:  //bad ack code
                              //CSDev REM Misspelling
                        combinedResponse.ExceptionOccurred = true;
                        combinedResponse.MVIMessage = MVISearchUnknownError;
                        combinedResponse.RawMviExceptionMessage = response.RawMviExceptionMessage;
                        break;
                }
            }
            else
            {
                //if response.Acknowledgement is null, this is likely a connection or internal error
                //CSDev REM Misspelling
                combinedResponse.ExceptionOccurred = true;
                combinedResponse.MVIMessage = MVISearchConnectionError;
                combinedResponse.RawMviExceptionMessage = response.RawMviExceptionMessage;
            }

            return combinedResponse;
        }

        private static string dateStringFormat(string date)
        {
            if (date == null) return null;
            if (date.Length == 10) return date;
            if (date.Length < 8) return date;

            date = date.Substring(4, 2) + "/" + date.Substring(6, 2) + "/" + date.Substring(0, 4);
            return date;
        }
        private static void GetICN(PatientPerson person, UDOSelectedPersonRequest newPerson)
        {
            try
            {
                if (person.CorrespondingIdList == null || !person.CorrespondingIdList.Any())
                {
                    newPerson.RawValueFromMvi = string.Empty;
                    newPerson.ICN = string.Empty;
                }
                else
                {
                    CorrespondingIDs patientNo = person.CorrespondingIdList.FirstOrDefault((v =>
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

                    newPerson.RawValueFromMvi = patientNo != null ? patientNo.RawValueFromMvi : string.Empty;
                    newPerson.ICN = patientNo != null ? patientNo.PatientIdentifier : string.Empty;
                }
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mviResponse"></param>
        /// <returns></returns>
        private static UDOCombinedPersonSearchResponse MaptoResponse(MVIMessages.RetrieveOrSearchPersonResponse mviResponse, UDOCHATPersonSearchRequest request)
        {
            if (mviResponse == null)
                return null;

            UDOCombinedPersonSearchResponse response = new UDOCombinedPersonSearchResponse();
            //CSDev REM Misspelling
            response.ExceptionOccurred = mviResponse.ExceptionOccured;
            response.MVIMessage = mviResponse.Message;
            response.RawMviExceptionMessage = mviResponse.RawMviExceptionMessage;

            if (mviResponse.ExceptionOccured)
            {
                //CSDev Rem Pull in UserId from Request 
                LogHelper.LogError(mviResponse.OrganizationName, request.UserId, mviResponse.RawMviExceptionMessage, new Exception());
                return response;
            }

            #region map mviperson

            System.Collections.Generic.List<PatientPerson> people = new System.Collections.Generic.List<PatientPerson>();
            if (mviResponse.Person != null)
            {
                foreach (MVIMessages.PatientPerson thisPerson in mviResponse.Person)
                {

                    PatientPerson newPerson = new PatientPerson();
                    #region address
                    newPerson.Address = new PatientAddress();
                    if (thisPerson.Address != null)
                    {
                        newPerson.Address.City = thisPerson.Address.City;
                        newPerson.Address.Country = thisPerson.Address.Country;
                        newPerson.Address.PostalCode = thisPerson.Address.PostalCode;
                        newPerson.Address.State = thisPerson.Address.State;
                        newPerson.Address.StreetAddressLine = thisPerson.Address.StreetAddressLine;
                    }
                    #endregion

                    if (!string.IsNullOrEmpty(thisPerson.BirthDate))
                    {
                        newPerson.BirthDate = FormatDate(thisPerson.BirthDate);
                    }
                    if (!string.IsNullOrEmpty(thisPerson.DeceasedDate))
                    {
                        newPerson.DeceasedDate = FormatDate(thisPerson.DeceasedDate);
                    }
                    #region corresponding IDs
                    System.Collections.Generic.List<CorrespondingIDs> ids = new System.Collections.Generic.List<CorrespondingIDs>();
                    if (thisPerson.CorrespondingIdList != null)
                    {
                        foreach (MVIMessages.UnattendedSearchRequest thisPersonIds in thisPerson.CorrespondingIdList)
                        {
                            CorrespondingIDs thisId = new CorrespondingIDs();
                            thisId.AssigningAuthority = thisPersonIds.AssigningAuthority;
                            thisId.AssigningFacility = thisPersonIds.AssigningFacility;
                            //CSDev Rem: Removed in CorrespondingIDs Class to Build
                            thisId.IdentifierType = thisPersonIds.IdentifierType;
                            thisId.OrganizationName = thisPersonIds.OrganizationName;
                            thisId.PatientIdentifier = thisPersonIds.PatientIdentifier;
                            thisId.RawValueFromMvi = thisPersonIds.RawValueFromMvi;
                            thisId.UserFirstName = thisPersonIds.UserFirstName;
                            thisId.UserId = thisPersonIds.UserId;
                            thisId.UserLastName = thisPersonIds.UserLastName;
                            ids.Add(thisId);
                        }
                        newPerson.CorrespondingIdList = ids.ToArray();
                    }
                    #endregion

                    newPerson.EdiPi = thisPerson.EdiPi;
                    newPerson.GenderCode = thisPerson.GenderCode;
                    newPerson.Identifier = thisPerson.Identifier;
                    newPerson.IdentifierType = thisPerson.IdentifierType;
                    newPerson.IdentifyTheft = thisPerson.IdentifyTheft;
                    newPerson.IsDeceased = thisPerson.IsDeceased;
                    newPerson.ParticipantId = thisPerson.ParticipantId;
                    newPerson.PhoneNumber = thisPerson.PhoneNumber;
                    newPerson.RecordSource = thisPerson.RecordSource;
                    newPerson.SSId = SecurityTools.ConvertToSecureString(thisPerson.SocialSecurityNumber);
                    newPerson.SSIdString = newPerson.SSId.ToUnsecureString();
                    newPerson.StatusCode = thisPerson.StatusCode;

                    #region namelist
                    System.Collections.Generic.List<Name> names = new System.Collections.Generic.List<Name>();
                    if (thisPerson.NameList != null)
                    {
                        foreach (MVIMessages.Name thisName in thisPerson.NameList)
                        {
                            Name newName = new Name();
                            newName.FamilyName = thisName.FamilyName;
                            newName.GivenName = thisName.GivenName;
                            newName.MiddleName = thisName.MiddleName;
                            newName.NamePrefix = thisName.NamePrefix;
                            newName.NameSuffix = thisName.NameSuffix;
                            newName.NameType = thisName.NameType;

                            names.Add(newName);
                        }
                        newPerson.NameList = names.ToArray();
                    }
                    #endregion
                    people.Add(newPerson);
                }
                response.Person = people.ToArray();
            }
            #endregion
            return response;
        }

        /// <summary>
        /// Formats the data fields in each node.
        /// </summary>
        /// <param name="patientPerson">Array of person obhects.</param>
        private static void FormatData(PatientPerson[] patientPerson)
        {
            if (patientPerson != null)
            {
                foreach (PatientPerson person in patientPerson)
                {
                    if (!string.IsNullOrEmpty(person.BirthDate))
                    {
                        person.BirthDate = FormatDate(person.BirthDate);
                    }
                    if (!string.IsNullOrEmpty(person.DeceasedDate))
                    {
                        person.DeceasedDate = FormatDate(person.DeceasedDate);
                    }
                }
            }
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
            catch (FormatException)
            {
                //If date cannot be reformatted return the date present in the system.
                return dateString;
            }
            catch (ArgumentException)
            {
                //If date cannot be reformatted return the date present in the system.
                return dateString;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private static UDOCombinedPersonSearchResponse HandleEmptySearchResponse(UDOCHATPersonSearchRequest request, UDOCombinedPersonSearchResponse response)
        {
            CombinedSecondarySearchProcessor processor = new CombinedSecondarySearchProcessor();
            UDOCombinedPersonSearchResponse lobResponse = (UDOCombinedPersonSearchResponse)processor.Execute(request) ??
                           new UDOCombinedPersonSearchResponse();

            return lobResponse;
        }
    }
}

