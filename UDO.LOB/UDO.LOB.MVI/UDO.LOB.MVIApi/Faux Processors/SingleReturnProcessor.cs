using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Configuration;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Common;
using UDO.LOB.MVI.Messages;
using UDO.LOB.VeteranSnapShot.Messages;
using MVIMessages = VEIS.Mvi.Messages;

namespace UDO.LOB.MVI.Processors
{
    public  class SingleReturnProcessor
    {
        // REM: New variables
        private const string method = "SingleReturnProcessor";

        private Uri veisBaseUri;
        private LogSettings logSettings { get; set; }

        public UDOCombinedPersonSearchResponse SinglePersonResponseProcessing(UDOCombinedPersonSearchRequest request, UDOCombinedPersonSearchResponse response,  
            CRMCommonFunctions CommonFunctions, IOrganizationService connection, ref StringBuilder _logData, ref StringBuilder _logTimerData)
        {
            Guid owningTeamId = Guid.Empty;
            Stopwatch txnTimer = Stopwatch.StartNew();

            #region setup diagnosticscontext
            Guid interactionId = Guid.Empty;
            Guid.TryParse(request.interactionId, out interactionId);

            DiagnosticsContext dContext = new DiagnosticsContext()
            {
                InteractionId = interactionId,
                AgentId = request.UserId,
                MessageTrigger = "SingleReturnProcessor.SinglePersonResponseProcessing",
                OrganizationName = request.OrganizationName,
                StationNumber = request.LegacyServiceHeaderInfo == null ? request.LegacyServiceHeaderInfo.StationNumber : string.Empty,
                VeteranId = response.contactId
            };
            if (request.DiagnosticsContext == null)
            {
                request.DiagnosticsContext = dContext;
            }
            TraceLogger traceLogger = new TraceLogger("SingleReturnProcessor",request);
            #endregion
            if (response.Person != null)
            {
                if (response.Person.Count() == 1)
                {
                    InitProcessor(request);
                    if (response.Person[0].SSId == null)
                    {
                        response.Person[0].SSId = response.Person[0].SSIdString.ToSecureString();
                    }


                    #region if Source is MVI, first thing we do is go to corp to add in items that aren't in MVI, like BOS and Gender
                    if (response.Person[0].RecordSource == "MVI")
                    {
                        UDOSelectedPersonRequest newRequest = new UDOSelectedPersonRequest();
                       
                        newRequest.MessageId = request.MessageId;
                        newRequest.RecordSource = response.Person[0].RecordSource;
                        GetICN(response.Person[0], newRequest);
                        if (!String.IsNullOrEmpty(response.Person[0].EdiPi))
                            newRequest.Edipi = response.Person[0].EdiPi;
                        if (request.SSId != null && request.SSId.Length > 0)
                            newRequest.SSId = request.SSId;
                        else
                            newRequest.SSId = response.Person[0].SSId;

                        newRequest.CorrespondingIdList = response.Person[0].CorrespondingIdList;
                        newRequest.ByPassMVI = request.BypassMvi;
                        newRequest.UserFirstName = request.UserFirstName;
                        newRequest.UserLastName = request.UserLastName;
                        newRequest.OrganizationName = request.OrganizationName;
                        newRequest.UserId = request.UserId;
                        newRequest.LogSoap = request.LogSoap;
                        newRequest.Debug = request.Debug;
                        newRequest.LogTiming = request.LogTiming;

                        newRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                        newRequest.DiagnosticsContext = request.DiagnosticsContext;

                        var person = response.Person[0];

                        if (person.NameList != null)
                        {
                            var legalName = person.NameList.FirstOrDefault(v => v.NameType.Equals("Legal", StringComparison.OrdinalIgnoreCase));
                            var alias = person.NameList.FirstOrDefault(v => v.NameType.Equals("Alias", StringComparison.OrdinalIgnoreCase));

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
                        newRequest.BranchOfService = response.Person[0].BranchOfService;

                        long PID;
                        Int64.TryParse(person.ParticipantId, out PID);
                        if (PID != null && PID != 0)
                            newRequest.participantID = PID;

                        newRequest.IsAttended = request.IsAttended;

                        UDOSelectedPersonResponse thisNewResponse = null;

                        try
                        {
                            var selectedPersonLogic = new UDOSelectedPersonProcessor();
                            thisNewResponse = (UDOSelectedPersonResponse)selectedPersonLogic.Execute(newRequest);
                            traceLogger.LogEvent("UDOSelectedPersonResponse Complete", "001");
                           
                        }
                        catch (Exception ex)
                        {
                            LogHelper.LogError(request.OrganizationName, request.UserId, request.MessageId, method, ex);
                            traceLogger.LogException(ex, "002");
                        }

                        txnTimer.Stop();
                        _logTimerData.AppendLine("Appending Information to MVI from CORP:" + txnTimer.ElapsedMilliseconds);
                        txnTimer.Restart();
                        if (thisNewResponse == null || thisNewResponse?.ExceptionOccured == true)
                        {
                            //if the orchestration, findVeteran or FindInCrm fail, send the message back, but don't return the person since there will be no ID Proof or contactId
                            response.MVIMessage = "Search failed. " + thisNewResponse.Message;
                            response.RawMviExceptionMessage = thisNewResponse.RawMviExceptionMessage;
                            response.Person = null;
                            response.ExceptionOccurred = true;
                            traceLogger.LogException(new Exception($"{response.MVIMessage} :: {response.RawMviExceptionMessage}"), "003");
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
                                    response.ExceptionOccurred = true;
                                    traceLogger.LogEvent("Security Level Access Violation", "004");
                                    return response;
                                }
                            }
                        }

                        ///Check the duplicate flag and if set then we need to call UDOhandleDuplicateCorpRecordsProcessor
                       
                        if (thisNewResponse.DuplicateCorpPID)
                        {
                            ///Handle Duplicate Corp records identified by MVI Corresponding IDs list

                            if (thisNewResponse.CorrespondingIdsResponse != null)
                            {
                                var handleDuplicatesProcessor = new HandleDupCorpRecordProcessor();
                                var handleDuplicateResponse = (UDOHandleDupCorpRecordResponse)handleDuplicatesProcessor.Execute(thisNewResponse, newRequest, response.Person[0]);

                                response.Person = handleDuplicateResponse.Person;
                                response.UDOMessage = handleDuplicateResponse.UDOMessage;
                                response.CORPDbMessage = handleDuplicateResponse.CORPDbMessage;
                                response.ExceptionOccurred = handleDuplicateResponse.ExceptionOccurred;
                                if (handleDuplicateResponse.ExceptionOccurred)
                                {
                                    traceLogger.LogException(new Exception($"Exception Occured in HandleDupCorpRecordProcessor. UDO Message: {handleDuplicateResponse.UDOMessage} CORPDb Message: {handleDuplicateResponse.CORPDbMessage}"), "005");
                                }
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
                        Rank.GetPersonsRank(request.Debug, request.LogSoap, request.LogTiming, request.OrganizationName, request.UserId, person);
                        
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
                        _logData.AppendLine("person.crme_stationofJurisdiction:" + person.crme_stationofJurisdiction);
                        _logData.AppendLine("person.DeceasedDate:" + person.DeceasedDate);
                        if (person.SSId == null)
                        {
                            if (thisNewResponse.SSId != null)
                                person.SSId = thisNewResponse.SSId;
                        }

                        ////Return the response if we have more than one person record returned due to multiple active CORP PIDs
                        //only create IDProof records if we have an interactionID and 
                         var crmPersonId = thisNewResponse.contactId;
                         owningTeamId = thisNewResponse.OwningTeamId;
                        if (crmPersonId == Guid.Empty)
                        {
                            LogHelper.LogDebug(newRequest.OrganizationName, newRequest.Debug, newRequest.UserId, method, "Person not found from UDOSelectedPersonResponse");
                            crmPersonId = CommonFunctions.TryCreateNewCrmPerson(connection, ref newRequest, "MVI");
                            txnTimer.Stop();
                            _logTimerData.AppendLine("Created New Veteran:" +  txnTimer.ElapsedMilliseconds);
                            txnTimer.Restart();
                            owningTeamId = newRequest.OwningTeamId;
                            LogHelper.LogDebug(newRequest.OrganizationName, newRequest.Debug, newRequest.UserId, method, "Created a new Person as Person not found from UDOSelectedPersonResponse");
                        }
                        
                        response.contactId = crmPersonId;

                        // NP: REM: Changed to read Crm Base Url from LOB Web Config
                        string crmBaseUrl = ConnectionCache.ConnectManager.BaseUrl;
                        response.URL = $@"{crmBaseUrl}/main.aspx?etn=contact&pagetype=entityrecord&id=%7B{crmPersonId}%7D";

                        if (!string.IsNullOrEmpty(request.interactionId))
                        {
                            Entity newIDProof = new Entity();
                            newIDProof.LogicalName = "udo_idproof";

                            newIDProof["udo_birthdate"] = person.BirthDate;
                            newIDProof["udo_phonenumber"] = person.PhoneNumber;
                            newIDProof["udo_firstname"] = newRequest.FirstName;
                            newIDProof["udo_lastname"] = newRequest.FamilyName;
                            newIDProof["udo_ssn"] = SecurityTools.ConvertToUnsecureString(newRequest.SSId);
                            
                            newIDProof["udo_veteran"] = new EntityReference("contact", thisNewResponse.contactId);
                            newIDProof["udo_interaction"] = new EntityReference("udo_interaction", Guid.Parse(request.interactionId));
                            newIDProof["udo_branchofservice"] = person.BranchOfService;
                            newIDProof["udo_rank"] = person.Rank;
                            newIDProof["udo_title"] =  newRequest.FamilyName + ", " + newRequest.FirstName;

                            // REM: Connection is created using App User account.
                            var idProofId = connection.Create(TruncateHelper.TruncateFields(request.MessageId, newIDProof, request.OrganizationName, request.UserId, request.LogTiming, connection));
                            txnTimer.Stop();
                            _logTimerData.AppendLine("Created IDProof:" + txnTimer.ElapsedMilliseconds);
                            txnTimer.Restart();
                            response.idProofId = idProofId;
                        }
                        else
                        {
                            _logData.AppendLine("no interaction id, did not create IDProof");
                        }

                        //The UDOSelectedPersonResponse does a TryGetCrmPerson, so no need to do this again
                    #endregion
                    }
                    else
                    {
                        #region corp results

                        var MVIICN = string.Empty;
                        var MVIEDIPI = string.Empty;

                        if (response.Person[0].VeteranSensitivityLevel != null)
                        {
                            int realSL = 0;
                            if (Int32.TryParse(response.Person[0].VeteranSensitivityLevel, out realSL))
                            {
                                if (request.userSL < realSL)
                                {
                                    response.MVIMessage = string.Format("In UDO your user record is set to a lower Sensitivity Level than CSS. To service this veteran you must have an SL of {0} or higher. Please contact your Administrator to have your level changed.  In the meanwhile, you will have to transfer this call. ", realSL);
                                    response.Person = null;
                                    response.ExceptionOccurred = true;
                                    return response;
                                }
                            }
                        }

                        //This is a configuration setting to check MVI again with the results of the Corp query to see if exists
                        //Even if turned on, it will only be run if the Corp result is different than the initial input
                        #region Get EDIPI and ICN from MVI
                        var mviPersonSearchResponse = new MVIMessages.RetrieveOrSearchPersonResponse();
                        var addPersonToMVI = false;
                        if (request.FirstName != null)
                        {
                            if (request.FirstName != null && response.Person[0].NameList[0].GivenName != null &&
                                request.FamilyName != null && response.Person[0].NameList[0].FamilyName != null &&
                                request.BirthDate != null && response.Person[0].BirthDate != null &&
                                request.FirstName.ToLower().Trim() != response.Person[0].NameList[0].GivenName.ToLower().Trim() ||
                                request.FamilyName.ToLower().Trim() != response.Person[0].NameList[0].FamilyName.ToLower().Trim() ||
                                request.BirthDate != response.Person[0].BirthDate)
                            {
                                if (!request.BypassMvi)
                                {
                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "In MVI grab based on Corp Results", "Going to Call MVI based on Corp Results and BypassMVI equal to false");
                                    ///GO TO MVI AGAIN NOW W/COMPLETE INFO                                   
                                    var findVeteranResponse = response.VEISFindVeteranResponse;

                                    if (findVeteranResponse != null)
                                    {
                                        if (findVeteranResponse.VEISfvetreturnInfo != null)
                                        {
                                            if (findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo != null)
                                            {


                                                var dob = string.Empty;

                                                if (!string.IsNullOrEmpty(findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_dateOfBirth))
                                                {
                                                    var dob1 = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_dateOfBirth;
                                                    if (dob1.Length > 6)
                                                        dob = dob1.Substring(6, 4) + dob1.Substring(0, 2) + dob1.Substring(3, 2);
                                                }
                                                var searchPersonRequest = new MVIMessages.AttendedSearchRequest(findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_firstName,
                                                null, findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_lastName, SecurityTools.ConvertToUnsecureString(response.Person[0].SSId), dob,
                                                null, request.UserId, request.UserFirstName, request.UserLastName,
                                                request.OrganizationName, request.MessageId);

                                                try
                                                {
													mviPersonSearchResponse = WebApiUtility.SendReceive<MVIMessages.RetrieveOrSearchPersonResponse>(searchPersonRequest, WebApiType.VEIS);
                                                    if (request.LogSoap || mviPersonSearchResponse.ExceptionOccurred)
                                                    {
                                                        if (mviPersonSearchResponse.SerializedSOAPRequest != null || mviPersonSearchResponse.SerializedSOAPResponse != null)
                                                        {
                                                            var requestResponse = mviPersonSearchResponse.SerializedSOAPRequest + mviPersonSearchResponse.SerializedSOAPResponse;
                                                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"AttendedSearchRequest Request/Response {requestResponse}", true);
                                                        }
                                                    }

                                                    if (mviPersonSearchResponse.Person == null && mviPersonSearchResponse.ExceptionOccured == false)
                                                    {
                                                        //If Flag to Skip Add Person is set to False then we should add this person to MVI
                                                        if (!request.noAddPerson)
                                                            addPersonToMVI = true;
                                                    }
                                                    else if(mviPersonSearchResponse.Person != null && mviPersonSearchResponse.ExceptionOccured == false)
                                                    {
                                                        //IF we get more than 1 result from MVI for the corp data provided then log and bail out
                                                        if (mviPersonSearchResponse.Person.Count() > 1)
                                                        {
                                                           LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "In MVI grab based on Corp Results", "MVI Returned more than 1 result, exiting");
                                                        }
                                                        else
                                                        {

                                                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "In MVI grab based on Corp Results", "MVI Returned only 1 person, looking to udpate ICN and EDIPI if required");
                                                            Guid? personId = Guid.Empty;

                                                            //Get the ICN from the MVI response if available
                                                            MVIICN = GetICN(mviPersonSearchResponse.Person[0]);

                                                            //Get the EDIP from the MVI response if available. 
                                                            if (!String.IsNullOrEmpty(mviPersonSearchResponse.Person[0].EdiPi))
                                                                MVIEDIPI = mviPersonSearchResponse.Person[0].EdiPi;

                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LogHelper.LogDebug(request.OrganizationName, request.LogTiming, request.UserId, "SinglePersonResponseProcessing, Timing", _logTimerData.ToString());
                                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "SinglePersonResponseProcessing, Debug", _logData.ToString());
                                                    LogHelper.LogError(request.OrganizationName, request.UserId, "SinglePersonResponseProcessing, Execute", "Exception:" + ex.Message);
                                                    LogHelper.LogError(request.OrganizationName, request.UserId, "SinglePersonResponseProcessing, Execute", ex);
                                                    if (response == null)
                                                    {
                                                        response = new UDOCombinedPersonSearchResponse();
                                                    }
                                                    response.MVIMessage = ex.Message;
                                                    response.ExceptionOccurred = true;
                                                    return response;
                                                }
                                            }
                                        }
                                    }

                                }
                                else
                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "In MVI grab based on Corp Results", "Bypass MVI was True");
                            }
                        }
                        #endregion Get EDIPI and ICN from MVI

                        //it came from CORP, we have everything we need
                        UDOSelectedPersonRequest selectedPersonRequest = new UDOSelectedPersonRequest();
                        selectedPersonRequest.FileNumber = response.Person[0].FileNumber;
                        selectedPersonRequest.FirstName = response.Person[0].NameList[0].GivenName;
                        selectedPersonRequest.MiddleName = response.Person[0].NameList[0].MiddleName;
                        selectedPersonRequest.FamilyName = response.Person[0].NameList[0].FamilyName;
                        selectedPersonRequest.SSId = response.Person[0].SSId;
                        selectedPersonRequest.participantID = Convert.ToInt64(response.Person[0].ParticipantId);
                        selectedPersonRequest.RecordSource = "CorpdDB";
                        selectedPersonRequest.VeteranSensitivityLevel = Convert.ToInt32(response.Person[0].VeteranSensitivityLevel);
                        selectedPersonRequest.LogSoap = request.LogSoap;
                        selectedPersonRequest.Debug = request.Debug;
                        selectedPersonRequest.LogTiming = request.LogTiming;
                        selectedPersonRequest.ByPassMVI = request.BypassMvi;
                        selectedPersonRequest.OrganizationName = request.OrganizationName;
                        selectedPersonRequest.BranchOfService = response.Person[0].BranchOfService;
                        selectedPersonRequest.PhoneNumber = response.Person[0].PhoneNumber;

                        if (!String.IsNullOrEmpty(MVIICN))
                            selectedPersonRequest.ICN = MVIICN;

                        if (!String.IsNullOrEmpty(MVIEDIPI))
                            selectedPersonRequest.Edipi = MVIEDIPI;

                        txnTimer.Stop();
                        _logTimerData.AppendLine("Just before Trying to Find Veteran in CRM:" + txnTimer.ElapsedMilliseconds);
                        txnTimer.Restart();

                        var crmPersonId = CommonFunctions.TryGetCrmPerson(connection, ref selectedPersonRequest, "CorpdDB");
                        txnTimer.Stop();
                        _logTimerData.AppendLine("Trying to Find Veteran in CRM:" + txnTimer.ElapsedMilliseconds);
                        txnTimer.Restart();
                        owningTeamId = selectedPersonRequest.OwningTeamId;
                        
                        if (crmPersonId == Guid.Empty)
                        {
                            LogHelper.LogDebug(selectedPersonRequest.OrganizationName, selectedPersonRequest.Debug, selectedPersonRequest.UserId, method, "Person not found");
                            crmPersonId = CommonFunctions.TryCreateNewCrmPerson(connection, ref selectedPersonRequest, "CorpdDB");
                            txnTimer.Stop();
                            _logTimerData.AppendLine("Created New Veteran in CRM:" + txnTimer.ElapsedMilliseconds);
                            txnTimer.Restart();
                            owningTeamId = selectedPersonRequest.OwningTeamId;

                            LogHelper.LogDebug(selectedPersonRequest.OrganizationName, selectedPersonRequest.Debug, selectedPersonRequest.UserId, method, "Created a new Person");
                        }
                        response.contactId = crmPersonId.Value;

                        // NP: REM: Changed to read Crm Base Url from LOB Web Config
                        string crmBaseUrl = ConnectionCache.ConnectManager.BaseUrl;
                        response.URL = $@"{crmBaseUrl}/main.aspx?etn=contact&pagetype=entityrecord&id=%7B{crmPersonId.Value}%7D";

                        if (!String.IsNullOrEmpty(selectedPersonRequest.Edipi))
                        {
                            if (String.IsNullOrEmpty(response.Person[0].EdiPi))
                            {
                                response.Person[0].EdiPi = selectedPersonRequest.Edipi;
                            }
                        }
                        
                        //If we have a Valid EDIPI go get the Rank
                        if (!String.IsNullOrEmpty(response.Person[0].EdiPi) && response.Person[0].EdiPi != "UNK")
                            Rank.GetPersonsRank(request.Debug, request.LogSoap, request.LogTiming, request.OrganizationName, request.UserId, response.Person[0]);

                        #region do IDProof stuff
                        //only create IDProof records if we have an interactionID
                        if (!string.IsNullOrEmpty(request.interactionId))
                        {
                            Entity newIDProof = new Entity();
                            newIDProof.LogicalName = "udo_idproof";
                            newIDProof["ownerid"] = new EntityReference("team", owningTeamId);
                            newIDProof["udo_birthdate"] = response.Person[0].BirthDate;
                            newIDProof["udo_phonenumber"] = response.Person[0].PhoneNumber;
                            newIDProof["udo_firstname"] = response.Person[0].NameList[0].GivenName;
                            newIDProof["udo_lastname"] = response.Person[0].NameList[0].FamilyName;
                            newIDProof["udo_ssn"] = SecurityTools.ConvertToUnsecureString(response.Person[0].SSId);
                            newIDProof["udo_veteran"] = new EntityReference("contact", response.contactId);
                            newIDProof["udo_interaction"] = new EntityReference("udo_interaction", Guid.Parse(request.interactionId));
                            newIDProof["udo_branchofservice"] = response.Person[0].BranchOfService;
                            newIDProof["udo_rank"] = response.Person[0].Rank;
                            newIDProof["udo_title"] =  response.Person[0].NameList[0].FamilyName + ", " + response.Person[0].NameList[0].GivenName;

                            // REM: Connection is created using App User account.
                            var idProofId = connection.Create(TruncateHelper.TruncateFields(request.MessageId, newIDProof, request.OrganizationName, request.UserId, request.LogTiming, connection));
                            txnTimer.Stop();
                            _logTimerData.AppendLine("Created IDProof:" + txnTimer.ElapsedMilliseconds);
                            txnTimer.Restart();

                            response.idProofId = idProofId;
                        }
                        else
                        {
                            _logData.AppendLine("no interaction id, did not create IDProof");
                        }
                        #endregion

                        #region do add person stuff

                        if (addPersonToMVI)
                        {
                            var addPersonRequest = new UDOAddPersonRequest();
                            addPersonRequest.MessageId = request.MessageId;
                            addPersonRequest.ContactId = response.contactId;
                            addPersonRequest.VEISResponse = response.VEISFindVeteranResponse;
                            addPersonRequest.LogTiming = request.LogTiming;
                            addPersonRequest.LogSoap = request.LogSoap;
                            addPersonRequest.Debug = request.Debug;
                            addPersonRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                            addPersonRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                            addPersonRequest.RelatedParentId = request.RelatedParentId;
                            addPersonRequest.UserId = request.UserId;
                            addPersonRequest.OrganizationName = request.OrganizationName;
                            addPersonRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
                            {
                                ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                            };

                            addPersonRequest.SSId = response.Person[0].SSId;
                            addPersonRequest.userfirstname = request.UserFirstName;
                            addPersonRequest.userlastname = request.UserLastName;

                            addPersonRequest.noAddPerson = request.noAddPerson;
                            addPersonRequest.BirthDate = response.Person[0].BirthDate;
                            addPersonRequest.FamilyName = response.Person[0].NameList[0].FamilyName;
                            addPersonRequest.FirstName = response.Person[0].NameList[0].GivenName;
                            addPersonRequest.ContactId = response.contactId;

                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "CombinedSearchProcessor.Execute", "Sending Stuff to AddPerson");

                            // TODO: Make a call to the LOB WebApi; Replace VEISBaseUri with LOB Api Uri
                            WebApiUtility.SendAsync(addPersonRequest, WebApiType.LOB);

                            txnTimer.Stop();
                            _logTimerData.AppendLine("Add Person ASYNC Call:" + txnTimer.ElapsedMilliseconds);
                            txnTimer.Restart();
                        }
                        #endregion
                        #endregion
                    }
                    //only create IDProof records if we have an interactionID
                    if (!string.IsNullOrEmpty(request.interactionId))
                    {
                        var newSnapShot = new Entity { LogicalName = "udo_veteransnapshot" };
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
                        newSnapShot["ownerid"] = new EntityReference("team", owningTeamId);
                        var SnapShotId = connection.Create(TruncateHelper.TruncateFields(request.MessageId, newSnapShot, request.OrganizationName, request.UserId, request.LogTiming, connection));
                        _logData.AppendLine("response.Person[0].crme_stationofJurisdiction:" + response.Person[0].crme_stationofJurisdiction);
                        _logData.AppendLine("response.Person[0].DeceasedDate:" + response.Person[0].DeceasedDate);
                        var UDOcreateUDOVeteranSnapShotRequest = new UDOcreateUDOVeteranSnapShotRequest();

                        UDOHeaderInfo HeaderInfo = new UDOHeaderInfo
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

                        WebApiUtility.SendAsync(UDOcreateUDOVeteranSnapShotRequest, WebApiType.LOB);

                        txnTimer.Stop();
                        _logTimerData.AppendLine("Create SnapShot Async Sent:" + txnTimer.ElapsedMilliseconds);
                        txnTimer.Restart(); 
                        UDOOpenIDProofAsyncRequest openMe = new UDOOpenIDProofAsyncRequest();
                        openMe.MessageId = request.MessageId;
                        openMe.IDProofId = response.idProofId;
                        openMe.OrganizationName = request.OrganizationName;
                        openMe.UserId = request.UserId;
                        openMe.Debug = request.Debug;
                        openMe.LegacyServiceHeaderInfo = new UDOHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                        openMe.DiagnosticsContext = request.DiagnosticsContext;

                        //REM: Invoked the WebApiUtility.SendAsync
                        WebApiUtility.SendAsync(openMe, WebApiType.LOB);

                        txnTimer.Stop();
                        _logTimerData.AppendLine("Open IDProof ASYNC Call:" +  txnTimer.ElapsedMilliseconds);
                        txnTimer.Restart();
                    }
                    else
                    {
                        _logData.AppendLine("no interaction id, did not create snapshot");
                    }
                }
            }
            return response;
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

                    newPerson.RawValueFromMvi = patientNo != null ? patientNo.RawValueFromMvi : string.Empty;
                    newPerson.ICN = patientNo != null ? patientNo.PatientIdentifier : string.Empty;
                }
            }
            catch (Exception)
            {

            }
        }

        private string GetICN(MVIMessages.PatientPerson person)
        {
            var icn = string.Empty;
            try
            {
                if (person.CorrespondingIdList == null || !person.CorrespondingIdList.Any())
                    return icn;
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

                    icn = patientNo != null ? patientNo.PatientIdentifier : string.Empty;
                    return icn;
                }
            }
            catch (InvalidOperationException)
            {
                return string.Empty;
            }
        }

        private void InitProcessor(UDOCombinedPersonSearchRequest request)
        {
            try
            {
                if (logSettings == null)
                {
                    logSettings = new LogSettings
                    {
                        CallingMethod = method,
                        Org = request.OrganizationName,
                        UserId = request.UserId
                    };
                }

                NameValueCollection veisConfigurations = VEISConfiguration.GetConfigurationSettings();
                veisBaseUri = new Uri(veisConfigurations.Get(VEISConfiguration.ECUri));
            }
            catch
            {
                // TODO: Handle any exceptions
            }
        }
    }
}