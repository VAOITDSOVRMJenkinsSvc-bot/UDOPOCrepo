using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Common;
using UDO.LOB.MVI.Messages;
using UDO.LOB.VeteranSnapShot.Messages;
using VEIS.Messages.PersonService;
using VEIS.Messages.VeteranWebService;

namespace UDO.LOB.MVI.Processors
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewe By Brian Greig - 4/30/15
    /// </remarks>
    public class UDOBIRLSandOtherSearchProcessor
    {
        //do not change these exception messages -- these are passed to the Search Page HTML\JS for proper messaging
        private const string VADIRSearchResultsWithMatch = "A search in VADIR found {0} matching record(s).";
        private const string VADIRSearchResultsWithNoMatch = "A search in VADIR did not find any records matching the search criteria.";
        private const string BIRLSSearchResultsWithMatch = "A search in BIRLS found {0} matching record(s).";
        private const string BIRLSSearchResultsWithNoMatch = "A search in BIRLS did not find any records matching the search criteria.";
        private const string CorpDbSearchResultsWithMatch = "A search in CORPDB found {0} matching record(s).";
        private const string CorpDbSearchResultsWithNoMatch = "A search in CORPDB did not find any records matching the search criteria.";
        private const string CorpDbSearchIsDown = "A connection error was encountered during the CORPDB search.";
        private const string CorpDbSearchUnknownError = "An unknown error was encountered during the CORPDB search.";
        private const string BirlsSearchUnknownError = "An unknown error was encountered during the BIRLS search.";
        private const string VADIRSearchUnknownError = "An unknown error was encountered during the VADIR search.";
        private const string MVISearchResultsWithNoMatch = "A search in MVI did not find any records matching the search criteria.";
        private const string CorpDbSearchBackendServicesDown = "Communication to the back end services is currently down. Please try again.";

        private const string method = "UDOBIRLSandOtherSearchProcessor";

        // REM: New variables
        private LogSettings logSettings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IMessageBase Execute(UDOBIRLSandOtherSearchRequest request)
        {
            UDOCombinedPersonSearchResponse response = new UDOCombinedPersonSearchResponse();
			//CSDev REM: Add in new Response 
			VEISfcrpfindCorporateRecordResponse findCorpResponse = new VEISfcrpfindCorporateRecordResponse();
            TraceLogger traceLogger = null;

            try
            {
                Guid interactionId = Guid.Empty;
                Guid.TryParse(request.interactionId, out interactionId);
                if (request.DiagnosticsContext == null && request != null)
                {
                    request.DiagnosticsContext = new DiagnosticsContext()
                    {
                        AgentId = request.UserId,
                        MessageTrigger = method,
                        OrganizationName = request.OrganizationName,
                        StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA",
                        InteractionId = interactionId
                    };
                }
                traceLogger = new TraceLogger(method, request);

                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Top", request.Debug);
                if (request == null)
                {
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method,
                        $"<< Exit from {method}. Recieved a null {request.GetType().Name} message request or request.Person.");
                    response.CORPDbMessage = "Called with no message";
                    response.ExceptionOccurred = true;
                    return response;
                }
                bool foundone = false;
                string foundSSN = "";
                if (request.SSId == null)
                {
                    request.SSId = request.SSIdString.ToSecureString();
                }
                #region look in CORP with partial data
                VEISfcrpfindCorporateRecordRequest findCorpRequest = new VEISfcrpfindCorporateRecordRequest();
                findCorpRequest.MessageId = request.MessageId;
                findCorpRequest.LogTiming = request.LogTiming;
                findCorpRequest.LogSoap = request.LogSoap;
                findCorpRequest.Debug = request.Debug;
                findCorpRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findCorpRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findCorpRequest.RelatedParentId = request.RelatedParentId;
                findCorpRequest.UserId = request.UserId;
                findCorpRequest.OrganizationName = request.OrganizationName;
                findCorpRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };

                // Old_Note: the VEISfcrpReqptcpntSearchPSNInputInfo object is temp remediated with required attribues for name and dob
                // TN: Replaced VEIS reference with latest assembly.
                // TODO: VEIS Dependency missing definitions
                findCorpRequest.VEISfcrpReqptcpntSearchPSNInputInfo = new VEISfcrpReqptcpntSearchPSNInput
                {
                    mcs_firstName = request.FirstName,
                    mcs_lastName = request.FamilyName,
                    mcs_middleName = request.MiddleName,
                    mcs_dateOfBirth = request.BirthDate
                };

				// REM: Invoke VEIS Web Api
				findCorpResponse = WebApiUtility.SendReceive<VEISfcrpfindCorporateRecordResponse>(findCorpRequest, WebApiType.VEIS);
                if (request.LogSoap || findCorpResponse.ExceptionOccurred)
                {
                    if (findCorpResponse.SerializedSOAPRequest != null || findCorpResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findCorpResponse.SerializedSOAPRequest + findCorpResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfcrpfindCorporateRecordRequest Request/Response {requestResponse}", true);
                    }
                }

                if (!findCorpResponse.ExceptionOccurred)
                {
                    // TODO: Note: the VEISfcrpreturnInfo object is temp remediated with required attribues for mcs_numberOfRecords
                    if (Convert.ToInt16(findCorpResponse.VEISfcrpreturnInfo.mcs_numberOfRecords) > 0)
                    {
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Found Corp Data with name,dob", request.Debug);
                        foundone = true;
                        response = MapCORPDbtoResponse(findCorpResponse, request);
                        //foundSSN = findCorpResponse.VIMTfcrpreturnInfo.VIMTfcrppersonsInfo
                    }
                }

                #endregion
                if (!foundone)
                {
                    if (!string.IsNullOrEmpty(request.SSIdString))
                    {
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Searching VADIR", request.Debug);

                        #region look in VADIR with SSN
                        VEISFPSSNfindPersonBySSNRequest VADIRrequest = new VEISFPSSNfindPersonBySSNRequest
                        {
                            MessageId = request.MessageId,
                            mcs_ssn = request.SSId.ToUnsecureString(),
                            OrganizationName = request.OrganizationName,
                            UserId = request.UserId,
                            Debug = request.Debug,
                            LogSoap = request.LogSoap,
                            LogTiming = request.LogTiming,
                            RelatedParentEntityName = request.RelatedParentEntityName,
                            RelatedParentFieldName = request.RelatedParentFieldName,
                            RelatedParentId = request.RelatedParentId,
                            LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                            {
                                ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                            }
                        };

						VEISFPSSNfindPersonBySSNResponse VADIRresponse = WebApiUtility.SendReceive<VEISFPSSNfindPersonBySSNResponse>(VADIRrequest, WebApiType.VEIS);
                        if (request.LogSoap || VADIRresponse.ExceptionOccurred)
                        {
                            if (VADIRresponse.SerializedSOAPRequest != null || VADIRresponse.SerializedSOAPResponse != null)
                            {
                                var requestResponse = VADIRresponse.SerializedSOAPRequest + VADIRresponse.SerializedSOAPResponse;
                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISFPSSNfindPersonBySSNRequest Request/Response {requestResponse}", true);
                            }
                        }

                        if (!VADIRresponse.ExceptionOccurred)
                        {
                            if (VADIRresponse.VEISFPSSNpersonDTOInfo != null)
                            {
                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Found VADIR Data with SSN", request.Debug);

                                foundone = true;
                                response = MapVADIRtoResponse(VADIRresponse, request);
                                //foundSSN = findCorpResponse.VIMTfcrpreturnInfo.VIMTfcrppersonsInfo
                            }
                        }
                        #endregion
                    }

                }
                if (!foundone)
                {
                    #region look in birls
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Going to BIRLS to look", request.Debug);
                    VEISbrlsfindBirlsRecordRequest findBirlsRequest = new VEISbrlsfindBirlsRecordRequest();
                    findBirlsRequest.MessageId = request.MessageId;
                    findBirlsRequest.LogTiming = request.LogTiming;
                    findBirlsRequest.LogSoap = request.LogSoap;
                    findBirlsRequest.Debug = request.Debug;
                    findBirlsRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                    findBirlsRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                    findBirlsRequest.RelatedParentId = request.RelatedParentId;
                    findBirlsRequest.UserId = request.UserId;
                    findBirlsRequest.OrganizationName = request.OrganizationName;
                    findBirlsRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                    VEISbrlsfindBirlsRecordResponse findBirlsResponse = new VEISbrlsfindBirlsRecordResponse();
                    findBirlsRequest.VEISbrlsReqveteranRecordInputInfo = new VEISbrlsReqveteranRecordInput
                    {
                        mcs_firstName = request.FirstName,
                        mcs_lastName = request.FamilyName,
                        mcs_middleName = request.MiddleName,
                        mcs_branchOfService = request.BranchofService,
                        mcs_dateOfBirth = request.BirthDate,
                        mcs_serviceNumber = request.ServiceNumber,
                        mcs_insuranceNumber = request.InsuranceNumber,
                        mcs_dateOfDeath = request.DeceasedDate,
                        mcs_enteredOnDutyDate = request.EnteredonDutyDate,
                        mcs_releasedActiveDutyDate = request.ReleasedActiveDutyDate,
                        mcs_suffix = request.Suffix,
                        mcs_folderLocation = request.FolderLocation,
                        mcs_payeeNumber = request.PayeeNumber
                    };

                    // REM: Already commented in original code
                    if (request.SSId != null && request.SSId.Length > 0)
                    {
                        if (request.SSId.StringLength() == 9)
                        {
                            findBirlsRequest.VEISbrlsReqveteranRecordInputInfo.mcs_ssn = request.SSId.ToUnsecureString();
                        }
                        else
                        {
                            findBirlsRequest.VEISbrlsReqveteranRecordInputInfo.mcs_fileNumber = request.SSId.ToUnsecureString();

                        }
                    }

					findBirlsResponse = WebApiUtility.SendReceive<VEISbrlsfindBirlsRecordResponse>(findBirlsRequest, WebApiType.VEIS);
                    if (request.LogSoap || findBirlsResponse.ExceptionOccurred)
                    {
                        if (findBirlsResponse.SerializedSOAPRequest != null || findBirlsResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = findBirlsResponse.SerializedSOAPRequest + findBirlsResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISbrlsfindBirlsRecordRequest Request/Response {requestResponse}", true);
                        }
                    }

                    #endregion

                    if (!findBirlsResponse.ExceptionOccurred)
                    {
                        if (findBirlsResponse.VEISbrlsreturnInfo.mcs_FIRST_NAME == null)
                        {
                            response.BIRLSMessage = findBirlsResponse.VEISbrlsreturnInfo.mcs_RETURN_MESSAGE;

                            if (findBirlsResponse.VEISbrlsreturnInfo.mcs_NUMBER_OF_RECORDS == null || findBirlsResponse.VEISbrlsreturnInfo.mcs_NUMBER_OF_RECORDS == "0")
                            {
                                response.BIRLSRecordCount = 0;
                                response.CORPDbMessage = BIRLSSearchResultsWithNoMatch;
                                return response;
                            }

                        }
                        else
                        {
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Found BIRLS Data", request.Debug);

                            response = MapBIRLStoResponse(findBirlsResponse, request);
                            foundSSN = findBirlsResponse.VEISbrlsreturnInfo.mcs_CLAIM_NUMBER;
                            foundone = true;
                        }
                    }

                }
                //new logic for 1 person
                if (response.Person != null)
                {

                    if (response.Person.Count() == 1)
                    {
                        //these other searches don't always fill in all the data - so we need to check that and make another call to corp if we need to.
                        if (response.Person[0].BranchOfService == null || response.Person[0].SSIdString == null || response.Person[0].GenderCode == null || response.Person[0].FileNumber == null)
                        {
                            if (response.Person[0].ParticipantId != null)
                            {
                                ///Need to return to Corp here to get Address, Gender, Phone, etc. based on PID
                                findVeteranDetailsByPidRequest(request, response.Person[0]);

                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Going to getIdentifiers", request.Debug);
                                #region - go to CORP and get the current SSN/Filenumber based on PID
                                UDOgetVeteranIdentifiersRequest getVeteranIdentifiers = new UDOgetVeteranIdentifiersRequest();
                                getVeteranIdentifiers.MessageId = request.MessageId;
                                getVeteranIdentifiers.LogTiming = request.LogTiming;
                                getVeteranIdentifiers.LogSoap = request.LogSoap;
                                getVeteranIdentifiers.Debug = request.Debug;
                                getVeteranIdentifiers.RelatedParentEntityName = request.RelatedParentEntityName;
                                getVeteranIdentifiers.RelatedParentFieldName = request.RelatedParentFieldName;
                                getVeteranIdentifiers.RelatedParentId = request.RelatedParentId;
                                getVeteranIdentifiers.UserId = request.UserId;
                                getVeteranIdentifiers.OrganizationName = request.OrganizationName;
                                getVeteranIdentifiers.LegacyServiceHeaderInfo = new UDOHeaderInfo
                                {
                                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                };
                                UDOgetVeteranIdentifiersResponse getVeteranIdentifiersResponse = new UDOgetVeteranIdentifiersResponse();

                                getVeteranIdentifiers.ptcpntId = Convert.ToInt64(response.Person[0].ParticipantId);

								UDOgetVeteranIdentifiersProcessor getveteranIdentifiersLogic = new UDOgetVeteranIdentifiersProcessor();
                                getVeteranIdentifiersResponse = (UDOgetVeteranIdentifiersResponse)getveteranIdentifiersLogic.Execute(getVeteranIdentifiers);



                                if (getVeteranIdentifiersResponse != null)
                                {
                                    //check to see if there is an exception on the getVeteranIdentifier calls. If there is, return and don't do FindInCrm
                                    if (getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo != null && getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_ParticipantID != null)
                                    {

                                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Processing Identifiers", request.Debug);

                                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SensitivityLevel))
                                        {
                                            response.Person[0].VeteranSensitivityLevel = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SensitivityLevel;
                                        }
                                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SSN))
                                        {
                                            response.Person[0].SSIdString = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SSN;
                                        }
                                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SSN))
                                        {
                                            response.Person[0].SSId = SecurityTools.ConvertToSecureString(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SSN);
                                        }
                                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_FileNumber))
                                        {
                                            response.Person[0].FileNumber = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_FileNumber;
                                        }
                                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_Gender))
                                        {
                                            response.Person[0].GenderCode = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_Gender;
                                        }
                                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_PhoneNumber))
                                        {
                                            response.Person[0].PhoneNumber = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_PhoneNumber;
                                        }
                                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_BranchOfService))
                                        {
                                            response.Person[0].BranchOfService = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_BranchOfService;
                                        }
                                        if (response.Person[0].crme_stationofJurisdiction == null || response.Person[0].crme_stationofJurisdiction.Trim() == "")
                                            response.Person[0].crme_stationofJurisdiction = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SOJ;
                                        if (response.Person[0].DeceasedDate == null || response.Person[0].DeceasedDate.Trim() == "")
                                            response.Person[0].DeceasedDate = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_DateofDeath;
                                        if (response.Person[0].crme_CharacterofDishcarge == null || response.Person[0].crme_CharacterofDishcarge.Trim() == "")
                                            response.Person[0].crme_CharacterofDishcarge = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_characterofdischarge;

                                    }
                                }
                                #endregion
                            }
                        }

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

                        if (String.IsNullOrEmpty(response.Person[0].Rank) && !String.IsNullOrEmpty(response.Person[0].EdiPi))
                        {
                            Rank.GetPersonsRank(request.Debug, request.LogSoap, request.LogTiming, request.OrganizationName, request.UserId, response.Person[0]);
                        }

                        if (response.Person[0].SSId == null)
                        {
                            response.Person[0].SSId = response.Person[0].SSIdString.ToSecureString();
                        }

                        #region create ID Proof
                        CRMCommonFunctions CommonFunctions = new CRMCommonFunctions();

                        #region connect to CRM
                        CrmServiceClient OrgServiceProxy = null;

                        try
                        {
                            OrgServiceProxy = ConnectionCache.GetProxy();
                        }
                        catch (Exception connectException)
                        {
                            LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                            response.ExceptionOccurred = true;
                            return response;
                        }
                        #endregion

                        using (OrgServiceProxy)
                        {
                            UDOSelectedPersonRequest selectedPersonRequest = new UDOSelectedPersonRequest();
                            selectedPersonRequest.FileNumber = response.Person[0].FileNumber;
                            selectedPersonRequest.FirstName = response.Person[0].NameList[0].GivenName;
                            selectedPersonRequest.MiddleName = response.Person[0].NameList[0].MiddleName;
                            selectedPersonRequest.FamilyName = response.Person[0].NameList[0].FamilyName;
                            selectedPersonRequest.SSIdString = SecurityTools.ConvertToUnsecureString(response.Person[0].SSId);
                            selectedPersonRequest.participantID = Convert.ToInt64(response.Person[0].ParticipantId);
                            selectedPersonRequest.RecordSource = response.Person[0].RecordSource;
                            selectedPersonRequest.VeteranSensitivityLevel = Convert.ToInt32(response.Person[0].VeteranSensitivityLevel);
                            selectedPersonRequest.SSId = response.Person[0].SSId;
                            selectedPersonRequest.BranchOfService = response.Person[0].BranchOfService;

                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Going to find Person in CRM", request.Debug);
                            Guid? crmPersonId = CommonFunctions.TryGetCrmPerson(OrgServiceProxy, ref selectedPersonRequest, response.Person[0].RecordSource);

                            if (crmPersonId == Guid.Empty)
                            {
                                LogHelper.LogDebug(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, "UDOBIRLSandOtherSearchProcessor, Execute", "Person not found", selectedPersonRequest.Debug);
                                crmPersonId = CommonFunctions.TryCreateNewCrmPerson(OrgServiceProxy, ref selectedPersonRequest, response.Person[0].RecordSource);
                                LogHelper.LogDebug(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, "UDOBIRLSandOtherSearchProcessor, Execute", "Created a new Person", selectedPersonRequest.Debug);
                            }
                            else
                            {
                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Found Person in CRM", request.Debug);
                            }

                            response.contactId = crmPersonId.Value;

                            // NP: REM: Changed to read Crm Base Url from LOB Web Config
                            string crmBaseUrl = ConnectionCache.ConnectManager.BaseUrl;
                            response.URL = $@"{crmBaseUrl}/main.aspx?etn=contact&pagetype=entityrecord&id=%7B{crmPersonId.Value}%7D";

                            #region do IDProof stuff
                            //only create IDProof records if we have an interactionID
                            if (!string.IsNullOrEmpty(request.interactionId))
                            {
                                Entity newIDProof = new Entity();
                                newIDProof["ownerid"] = new EntityReference("team", selectedPersonRequest.OwningTeamId);
                                newIDProof.LogicalName = "udo_idproof";

                                newIDProof["udo_birthdate"] = response.Person[0].BirthDate;
                                newIDProof["udo_phonenumber"] = response.Person[0].PhoneNumber;
                                newIDProof["udo_firstname"] = response.Person[0].NameList[0].GivenName;
                                newIDProof["udo_lastname"] = response.Person[0].NameList[0].FamilyName;
                                string title =  response.Person[0].NameList[0].FamilyName + ", " + response.Person[0].NameList[0].GivenName;

                                if (response.Person[0].SSId != null)
                                {
                                    newIDProof["udo_ssn"] = SecurityTools.ConvertToUnsecureString(response.Person[0].SSId);
                                }
                                else
                                {
                                    newIDProof["udo_ssn"] = response.Person[0].FileNumber;
                                }
                                newIDProof["udo_veteran"] = new EntityReference("contact", response.contactId);
                                newIDProof["udo_interaction"] = new EntityReference("udo_interaction", Guid.Parse(request.interactionId));
                                newIDProof["udo_branchofservice"] = response.Person[0].BranchOfService;
                                newIDProof["udo_rank"] = response.Person[0].Rank;
                                newIDProof["udo_title"] = title;

                                // REM: TODO: Review - Invoked with no CallerId
                                Guid idProofId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(request.MessageId, newIDProof, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));

                                response.idProofId = idProofId;

                                Entity newSnapShot = new Entity("udo_veteransnapshot");
                                newSnapShot["ownerid"] = new EntityReference("team", selectedPersonRequest.OwningTeamId);
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

                                UDOcreateUDOVeteranSnapShotRequest UDOcreateUDOVeteranSnapShotRequest = new UDOcreateUDOVeteranSnapShotRequest
                                {
                                    LegacyServiceHeaderInfo = new UDOHeaderInfo
                                    {
                                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                    },

                                    MessageId = request.MessageId,
                                    Debug = request.Debug,
                                    LogSoap = request.LogSoap,
                                    LogTiming = request.LogTiming,
                                    PID = response.Person[0].ParticipantId,
                                    fileNumber = response.Person[0].FileNumber,
                                    UserId = request.UserId,
                                    OrganizationName = request.OrganizationName,
                                    udo_veteranid = response.contactId,
                                    udo_veteransnapshotid = SnapShotId
                                };

                                // REM: Commented to invoke LOB 
                                WebApiUtility.SendAsync(UDOcreateUDOVeteranSnapShotRequest, WebApiType.LOB);
                            }
                        }
                        #endregion
                    }
                    else if (response.Person.Count() > 1)
                    {
                        foreach (Messages.PatientPerson person in response.Person)
                        {
                            ///Need to go and get additional data from corp based on the PID
                            if (person.ParticipantId != null)
                            {
                                try
                                {
                                    findVeteranDetailsByPidRequest(request, person);
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "#Error in UDOBIRLSandOtherSearchProcessor.findVeteranDetailsByPidRequest", ex);
                                }
                            }
                        }
                    }

                    DateTime myNow = DateTime.Now;
                    string extendedTIWEnd = myNow.Hour.ToString("00") + ":" + myNow.Minute.ToString("00") + ":" + myNow.Second.ToString("00") + ":" + myNow.Millisecond.ToString("000");

                    if (response.idProofId != null && response.idProofId != Guid.Empty)
                    {
                        UDOOpenIDProofAsyncRequest openMe = new UDOOpenIDProofAsyncRequest();
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
                        openMe.MessageId = request.MessageId;
                        openMe.DiagnosticsContext = request.DiagnosticsContext;

                        // REM: Invoke LOB Api
                        WebApiUtility.SendAsync(openMe, WebApiType.LOB);

                        extendedTIWEnd = myNow.Hour.ToString("00") + ":" + myNow.Minute.ToString("00") + ":" + myNow.Second.ToString("00") + ":" + myNow.Millisecond.ToString("000");
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $"Done sending async idproof. Returning to plugin", request.Debug);
                    }
                    #endregion

                    #region do add person stuff

                    UDOAddPersonRequest addPersonRequest = new UDOAddPersonRequest();
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
                    addPersonRequest.MVICheck = request.MVICheck;
                    addPersonRequest.noAddPerson = request.noAddPerson;
                    addPersonRequest.BirthDate = response.Person[0].BirthDate;
                    addPersonRequest.FamilyName = response.Person[0].NameList[0].FamilyName;
                    addPersonRequest.FirstName = response.Person[0].NameList[0].GivenName;
                    //also update ICN and EDIPI (if available) for ContactId
                    addPersonRequest.ContactId = response.contactId;

                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Execute UDOAddPersonRequest to AddPerson", request.Debug);

                    //Remediate to call LOB endpoint instead of the VEIS BASE URL
                    WebApiUtility.SendAsync(addPersonRequest, WebApiType.LOB);

                    #endregion
                }
                if (response.Person != null)
                {
                    if (response.Person.Count() > 0)
                    {
                        if (response.Person[0].SSId != null)
                        {
                            response.Person[0].SSIdString = SecurityTools.ConvertToUnsecureString(response.Person[0].SSId);
                        }
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOBIRLSandOtherSearchProcessor, Execute", "Exception:" + ex.Message);
                if (response == null)
                {
                    response = new UDOCombinedPersonSearchResponse();
                }
                response.MVIMessage = ex.Message;
                response.ExceptionOccurred = true;
                return response;
            }
        }

        private static string dateStringFormat(string date)
        {
            if (date == null) return null;
            if (date.Length == 10) return date;
            if (date.Length < 8) return date;

            date = date.Substring(4, 2) + "/" + date.Substring(6, 2) + "/" + date.Substring(0, 4);
            return date;
        }

        /// <summary>
        /// Formats the data fields in each node.
        /// </summary>
        /// <param name="patientPerson">Array of person obhects.</param>
        private static void FormatData(Messages.PatientPerson[] patientPerson)
        {
            if (patientPerson != null)
            {
                foreach (Messages.PatientPerson person in patientPerson)
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
        private static UDOCombinedPersonSearchResponse MapVADIRtoResponse(VEISFPSSNfindPersonBySSNResponse lobResponse, UDOBIRLSandOtherSearchRequest request)
        {
            if (lobResponse.VEISFPSSNpersonDTOInfo != null)
            {
                UDOCombinedPersonSearchResponse response = new UDOCombinedPersonSearchResponse();
                #region process veteran return
                // GetVeteranInfo returns a record with blank values
                if (string.IsNullOrEmpty(lobResponse.VEISFPSSNpersonDTOInfo.mcs_ssnNbr))
                {
                    response.CORPDbMessage = VADIRSearchResultsWithNoMatch;
                    return response;
                }
                response.Person = new Messages.PatientPerson[1];

                response.Person[0] = new Messages.PatientPerson
                {

                    GenderCode = lobResponse.VEISFPSSNpersonDTOInfo.mcs_genderCd,
                    SSIdString = lobResponse.VEISFPSSNpersonDTOInfo.mcs_ssnNbr,
                    FileNumber = lobResponse.VEISFPSSNpersonDTOInfo.mcs_fileNbr,
                    BirthDate = lobResponse.VEISFPSSNpersonDTOInfo.mcs_brthdyDt,
                    RecordSource = "VADIR",
                    ParticipantId = lobResponse.VEISFPSSNpersonDTOInfo.mcs_ptcpntId.ToString(),
                    NameList = new[]
                    {
                        new Messages.Name
                        {
                            MiddleName = lobResponse.VEISFPSSNpersonDTOInfo.mcs_middleNm,
                            GivenName = lobResponse.VEISFPSSNpersonDTOInfo.mcs_firstNm,
                            FamilyName = lobResponse.VEISFPSSNpersonDTOInfo.mcs_lastNm,
                            NameType = "Legal",
                            NamePrefix = "",
                            NameSuffix = ""
                        }
                    },

                };            

                #endregion


                response.MVIMessage = MVISearchResultsWithNoMatch;

                response.CORPDbMessage = CorpDbSearchResultsWithNoMatch;
                response.CORPDbRecordCount = 0;
                response.VADIRMessage = string.Format(VADIRSearchResultsWithMatch, response.Person.Length); ;
                response.VADIRRecordCount = response.Person.Length; ;

                return response;
            }
            else
            {
                if (lobResponse.ExceptionOccurred)
                {
                    UDOCombinedPersonSearchResponse response = new UDOCombinedPersonSearchResponse();
                    if (lobResponse.ExceptionMessage.Contains("access violation") || lobResponse.ExceptionMessage == "sensitive record check error")
                    {
                        response.ExceptionOccurred = true;
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.VADIRMessage = lobResponse.ExceptionMessage;
                    }
                    else if (lobResponse.ExceptionMessage == "BIRLS communication is down" || lobResponse.ExceptionMessage == "The Tuxedo service is down")
                    {
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOBIRLSandOtherSearchProcessor - exception in Corp", lobResponse.ExceptionMessage);
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.VADIRMessage = lobResponse.ExceptionMessage;
                        response.ExceptionOccurred = true;
                        return response;
                    }
                    else if (lobResponse.ExceptionMessage.Contains("there was no endpoint listening"))
                    {
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOBIRLSandOtherSearchProcessor - exception in Corp", lobResponse.ExceptionMessage);
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.VADIRMessage = CorpDbSearchBackendServicesDown;
                        response.ExceptionOccurred = true;
                    }
                    else
                    {
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOBIRLSandOtherSearchProcessor - exception in Corp", lobResponse.ExceptionMessage);
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.VADIRMessage = CorpDbSearchUnknownError;
                        response.ExceptionOccurred = true;
                    }
                    return response;
                }
                return null;
            }
        }
        private static UDOCombinedPersonSearchResponse MapCORPDbtoResponse(UDOfindVeteranInfoResponse lobResponse, UDOBIRLSandOtherSearchRequest request)
        {
            if (lobResponse.UDOfindVeteranInfoInfo != null)
            {
                UDOCombinedPersonSearchResponse response = new UDOCombinedPersonSearchResponse();
                #region process veteran return
                // GetVeteranInfo returns a record with blank values
                if (string.IsNullOrEmpty(lobResponse.UDOfindVeteranInfoInfo.crme_SSN))
                {
                    response.CORPDbMessage = CorpDbSearchResultsWithNoMatch;
                    return response;
                }
                response.Person = new Messages.PatientPerson[1];

                response.Person[0] = new Messages.PatientPerson
                {

                    GenderCode = lobResponse.UDOfindVeteranInfoInfo.crme_Gender,
                    BranchOfService = lobResponse.UDOfindVeteranInfoInfo.crme_BranchOfService,
                    SSIdString = lobResponse.UDOfindVeteranInfoInfo.crme_SSN,
                    FileNumber = lobResponse.UDOfindVeteranInfoInfo.crme_FileNumber,
                    EdiPi = lobResponse.UDOfindVeteranInfoInfo.crme_EDIPI,
                    BirthDate = lobResponse.UDOfindVeteranInfoInfo.crme_DOB.Date.ToShortDateString(),
                    RecordSource = "CORPDB",
                    ParticipantId = lobResponse.UDOfindVeteranInfoInfo.crme_ParticipantID,
                    VeteranSensitivityLevel = lobResponse.UDOfindVeteranInfoInfo.crme_VeteranSensitivityLevel,
                    NameList = new[]
                    {
                        new Messages.Name
                        {
                            MiddleName = lobResponse.UDOfindVeteranInfoInfo.crme_MiddleName,
                            GivenName = lobResponse.UDOfindVeteranInfoInfo.crme_FirstName,
                            FamilyName = lobResponse.UDOfindVeteranInfoInfo.crme_LastName,
                            NameType = "Legal",
                            NamePrefix = "",
                            NameSuffix = ""
                        }
                    },

                    PhoneNumber = lobResponse.UDOfindVeteranInfoInfo.crme_PrimaryPhone,
                    Address = new Messages.PatientAddress
                    {
                        City = lobResponse.UDOfindVeteranInfoInfo.crme_City,
                        State = lobResponse.UDOfindVeteranInfoInfo.crme_State,
                        PostalCode = lobResponse.UDOfindVeteranInfoInfo.crme_Zip,
                        StreetAddressLine = lobResponse.UDOfindVeteranInfoInfo.crme_Address1
                    },

                };
                Rank.GetPersonsRank(request.Debug, request.LogSoap, request.LogTiming, request.OrganizationName, request.UserId, response.Person[0]);
                #endregion


                response.MVIMessage = MVISearchResultsWithNoMatch;

                response.CORPDbMessage = string.Format(CorpDbSearchResultsWithMatch, response.Person.Length);
                response.CORPDbRecordCount = response.Person.Length;
                return response;
            }
            else
            {
                if (lobResponse.ExceptionOccured)
                {
                    UDOCombinedPersonSearchResponse response = new UDOCombinedPersonSearchResponse();
                    if (lobResponse.ExceptionMessage.Contains("access violation") || lobResponse.ExceptionMessage == "sensitive record check error")
                    {
                        response.ExceptionOccurred = true;
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = lobResponse.ExceptionMessage;
                    }
                    else if (lobResponse.ExceptionMessage == "BIRLS communication is down" || lobResponse.ExceptionMessage == "The Tuxedo service is down")
                    {
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOBIRLSandOtherSearchProcessor - exception in Corp", lobResponse.ExceptionMessage);
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = lobResponse.ExceptionMessage;
                        response.ExceptionOccurred = true;
                        return response;
                    }
                    else if (lobResponse.ExceptionMessage.Contains("there was no endpoint listening"))
                    {
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOBIRLSandOtherSearchProcessor - exception in Corp", lobResponse.ExceptionMessage);
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = CorpDbSearchBackendServicesDown;
                        response.ExceptionOccurred = true;
                    }
                    else
                    {
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOBIRLSandOtherSearchProcessor - exception in Corp", lobResponse.ExceptionMessage);
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = CorpDbSearchUnknownError;
                        response.ExceptionOccurred = true;
                    }
                    return response;
                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, "No record found in CorpDB");
                }
                return null;
            }
        }
        private static UDOCombinedPersonSearchResponse MapCORPDbtoResponse(VEISfcrpfindCorporateRecordResponse lobResponse, UDOBIRLSandOtherSearchRequest request)
        {
            if (lobResponse.VEISfcrpreturnInfo != null)
            {
                UDOCombinedPersonSearchResponse response = new UDOCombinedPersonSearchResponse();
                #region process veteran return
                // GetVeteranInfo returns a record with blank values
                if (lobResponse.VEISfcrpreturnInfo.mcs_numberOfRecords == "0")
                {
                    response.CORPDbMessage = CorpDbSearchResultsWithMatch;
                    return response;
                }
                response.Person = new Messages.PatientPerson[Convert.ToInt32(lobResponse.VEISfcrpreturnInfo.mcs_numberOfRecords)];
                int i = 0;
                foreach (VEISfcrppersonsMultipleResponse personInfo in lobResponse.VEISfcrpreturnInfo.VEISfcrppersonsInfo)
                {

                    response.Person[i] = new Messages.PatientPerson
                    {
                        BranchOfService = personInfo.mcs_branchOfService1,
                        SSIdString = personInfo.mcs_ssn,
                        BirthDate = personInfo.mcs_dateOfBirth,
                        RecordSource = "CORPDB",
                        ParticipantId = personInfo.mcs_ptcpntId,
                        NameList = new[]
                        {
                            new Messages.Name
                            {
                                MiddleName = personInfo.mcs_middleName,
                                GivenName = personInfo.mcs_firstName,
                                FamilyName = personInfo.mcs_lastName,
                                NameType = "Legal",
                                NamePrefix = "",
                                NameSuffix = ""
                            }
                        },

                    };
                    // This only runs if EDIPI is present...
                    i += 1;
                }

                #endregion



                response.CORPDbMessage = string.Format(CorpDbSearchResultsWithMatch, response.Person.Length);
                response.CORPDbRecordCount = response.Person.Length;
                return response;
            }
            else
            {
                if (lobResponse.ExceptionOccurred)
                {
                    UDOCombinedPersonSearchResponse response = new UDOCombinedPersonSearchResponse();
                    if (lobResponse.ExceptionMessage.Contains("access violation") || lobResponse.ExceptionMessage == "sensitive record check error")
                    {
                        response.ExceptionOccurred = true;
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = lobResponse.ExceptionMessage;
                    }
                    else if (lobResponse.ExceptionMessage == "BIRLS communication is down" || lobResponse.ExceptionMessage == "The Tuxedo service is down")
                    {
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, $" Exception in Corp:  {lobResponse.ExceptionMessage}");
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = lobResponse.ExceptionMessage;
                        response.ExceptionOccurred = true;
                    }
                    else if (lobResponse.ExceptionMessage.Contains("there was no endpoint listening"))
                    {
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, $" Exception in Corp:  {lobResponse.ExceptionMessage}");
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = CorpDbSearchBackendServicesDown;
                        response.ExceptionOccurred = true;
                    }
                    else
                    {
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, $" Exception in Corp:  {lobResponse.ExceptionMessage}");
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = CorpDbSearchUnknownError;
                        response.ExceptionOccurred = true;
                    }
                    return response;
                }
                else
                {
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "No record found in CorpDB", request.Debug);
                }
                return null;
            }
        }
        private static UDOCombinedPersonSearchResponse MapBIRLStoResponse(VEISbrlsfindBirlsRecordResponse lobResponse, UDOBIRLSandOtherSearchRequest request)
        {
            if (lobResponse.VEISbrlsreturnInfo != null)
            {
                UDOCombinedPersonSearchResponse response = new UDOCombinedPersonSearchResponse();
                #region process veteran return
                // GetVeteranInfo returns a record with blank values
                if (lobResponse.VEISbrlsreturnInfo.mcs_NUMBER_OF_RECORDS == "0")
                {
                    response.CORPDbMessage = BIRLSSearchResultsWithNoMatch;
                    return response;
                }
                response.Person = new Messages.PatientPerson[Convert.ToInt32(lobResponse.VEISbrlsreturnInfo.mcs_NUMBER_OF_RECORDS)];
                string BOS = "";
                if (lobResponse.VEISbrlsreturnInfo.VEISbrlsSERVICEInfo != null)
                {
                    foreach (VEISbrlsSERVICEMultipleResponse item in lobResponse.VEISbrlsreturnInfo.VEISbrlsSERVICEInfo)
                    {
                        if (!string.IsNullOrEmpty(item.mcs_BRANCH_OF_SERVICE))
                        {
                            if (!BOS.Contains(LongBranchOfService(item.mcs_BRANCH_OF_SERVICE)))
                            {
                                if (!string.IsNullOrEmpty(BOS))
                                {
                                    BOS += ":";
                                }
                                BOS += LongBranchOfService(item.mcs_BRANCH_OF_SERVICE);
                            }
                        }
                    }
                }

                int i = 0;
                response.Person[i] = new Messages.PatientPerson
                {
                    GenderCode = lobResponse.VEISbrlsreturnInfo.mcs_SEX_CODE,
                    //loop for BOS

                    BranchOfService = BOS,

                    SSIdString = lobResponse.VEISbrlsreturnInfo.mcs_SOC_SEC_NUMBER,
                    FileNumber = lobResponse.VEISbrlsreturnInfo.mcs_CLAIM_NUMBER,
                    BirthDate = lobResponse.VEISbrlsreturnInfo.mcs_DATE_OF_BIRTH,
                    RecordSource = "BIRLS",
                    //no SL in BIRLS, have to assume something...
                    VeteranSensitivityLevel = "0",
                    //If it was in corp, we would have found it....
                    //ParticipantId = lobResponse.VIMTbrlsreturnInfo.mcs_ptcpntId,
                    NameList = new[]
                    {
                        new Messages.Name
                        {
                            MiddleName = lobResponse.VEISbrlsreturnInfo.mcs_MIDDLE_NAME,
                            GivenName = lobResponse.VEISbrlsreturnInfo.mcs_FIRST_NAME,
                            FamilyName = lobResponse.VEISbrlsreturnInfo.mcs_LAST_NAME,
                            NameType = "Legal",
                            NamePrefix = "",
                            NameSuffix = ""
                        }
                    },

                };

                //How do I get the rank?

                #endregion

                response.CORPDbMessage = string.Format(BIRLSSearchResultsWithMatch, response.Person.Length);
                response.CORPDbRecordCount = response.Person.Length;
                return response;
            }
            else
            {
                if (lobResponse.ExceptionOccurred)
                {
                    UDOCombinedPersonSearchResponse response = new UDOCombinedPersonSearchResponse();
                    if (lobResponse.ExceptionMessage.Contains("access violation") || lobResponse.ExceptionMessage == "sensitive record check error")
                    {
                        response.ExceptionOccurred = true;
                        response.CORPDbMessage = lobResponse.ExceptionMessage;
                    }
                    else if (lobResponse.ExceptionMessage == "BIRLS communication is down" || lobResponse.ExceptionMessage == "The Tuxedo service is down")
                    {
                        response.CORPDbMessage = lobResponse.ExceptionMessage;
                        response.ExceptionOccurred = true;
                    }
                    else
                    {
                        response.CORPDbMessage = BirlsSearchUnknownError;
                        response.ExceptionOccurred = true;
                    }
                    return response;
                }
                else
                {
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "No record found in BIRLS", request.Debug);
                }
                return null;
            }
        }
        private static string LongBranchOfService(string branchcode)
        {
            //JS switch (branchcode.trim())
            switch (branchcode.Trim())
            {
                case "AF": return "AIR FORCE (AF)";
                case "A": return "ARMY (ARMY)";
                //ARMY AIR CORPS
                case "CG": return "COAST GUARD (CG)";
                case "CA": return "COMMONWEALTH ARMY (CA)";
                case "GCS": return "GUERRILLA AND COMBINATION SVC (GCS)";
                case "M": return "MARINES (M)";
                case "MM": return "MERCHANT MARINES (MM)";
                case "NOAA": return "NATIONAL OCEANIC & ATMOSPHERIC ADMINISTRATION (NOAA)";
                //NAVY (NAVY)
                case "PHS": return "PUBLIC HEALTH SVC (PHS)";
                case "RSS": return "REGULAR PHILIPPINE SCOUT (RSS)";
                //REGULAR PHILIPPINE SCOUT COMBINED WITH SPECIAL
                case "RPS": return "PHILIPPINE SCOUT OR COMMONWEALTH ARMY SVC (RPS)";
                case "SPS": return "SPECIAL PHILIPPINE SCOUTS (SPS)";
                case "WAC": return "WOMEN'S ARMY CORPS (WAC)";
            }
            return branchcode;
        }
        private void findVeteranDetailsByPidRequest(UDOBIRLSandOtherSearchRequest request, Messages.PatientPerson person)
        {
            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Going to findVeteranDetailsByPID", request.Debug);
            #region - go to CORP and get the current SSN/Filenumber based on PID
            UDOfindVeteranInfoByPidRequest findVeteranByPidRequest = new UDOfindVeteranInfoByPidRequest();
            findVeteranByPidRequest.MessageId = request.MessageId;
            findVeteranByPidRequest.LogTiming = request.LogTiming;
            findVeteranByPidRequest.LogSoap = request.LogSoap;
            findVeteranByPidRequest.Debug = request.Debug;
            findVeteranByPidRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findVeteranByPidRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findVeteranByPidRequest.RelatedParentId = request.RelatedParentId;
            findVeteranByPidRequest.UserId = request.UserId;
            findVeteranByPidRequest.OrganizationName = request.OrganizationName;
            findVeteranByPidRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
            {
                ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                LoginName = request.LegacyServiceHeaderInfo.LoginName,
                StationNumber = request.LegacyServiceHeaderInfo.StationNumber
            };
            findVeteranByPidRequest.ParticipantID = person.ParticipantId;

            UDOfindVeteranInfoProcessor findVetarnLogic = new UDOfindVeteranInfoProcessor();

            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $"Executing UDOfindVeteranInfoProcessor for PID: {person.ParticipantId}", request.Debug);
            UDOfindVeteranInfoByPidResponse findVeteranByPidResponse = (UDOfindVeteranInfoByPidResponse)findVetarnLogic.Execute(findVeteranByPidRequest);
            if (findVeteranByPidResponse.UDOfindVeteranInfoInfo != null)
            {
                if (string.IsNullOrEmpty(person.GenderCode))
                    person.GenderCode = findVeteranByPidResponse.UDOfindVeteranInfoInfo.crme_Gender;

                if (string.IsNullOrEmpty(person.BranchOfService))
                {
                    person.BranchOfService = findVeteranByPidResponse.UDOfindVeteranInfoInfo.crme_BranchOfService;
                }

                if (string.IsNullOrEmpty(person.PhoneNumber))
                {
                    person.PhoneNumber = findVeteranByPidResponse.UDOfindVeteranInfoInfo.crme_PrimaryPhone;
                }

                if (string.IsNullOrEmpty(person.VeteranSensitivityLevel))
                {
                    person.VeteranSensitivityLevel = findVeteranByPidResponse.UDOfindVeteranInfoInfo.crme_VeteranSensitivityLevel;
                }


                if (person.Address == null)
                {
                    Messages.PatientAddress address = new Messages.PatientAddress
                    {
                        City = findVeteranByPidResponse.UDOfindVeteranInfoInfo.crme_City,
                        State = findVeteranByPidResponse.UDOfindVeteranInfoInfo.crme_State,
                        PostalCode = findVeteranByPidResponse.UDOfindVeteranInfoInfo.crme_Zip,
                        StreetAddressLine = findVeteranByPidResponse.UDOfindVeteranInfoInfo.crme_Address1
                    };
                    person.Address = address;
                }
            }
            #endregion
        }
    }
}