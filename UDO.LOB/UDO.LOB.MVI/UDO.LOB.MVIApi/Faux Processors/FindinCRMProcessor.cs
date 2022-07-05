using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Common;
using UDO.LOB.MVI.Messages;
using UDO.LOB.MVI.Processors;
using UDO.LOB.PersonSearch.Processors;
using VEIS.Messages.VeteranWebService;
using MVIMessages = VEIS.Mvi.Messages;

namespace VRM.Integration.UDO.MVI.Faux_Processors
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    internal class FindinCRMProcessor
    {
        private static string UrlFormat;
        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
        private const string CorpDbSearchUnknownError = "An unknown error was encountered during the CORPDB search.";
        private const string CorpDbSearchBackendServicesDown = "Communication to the back end services is currently down. Please try again.";

        // REM: New variables
        private const string method = "FindinCRMProcessor";

        /// <summary>
        /// Selected Person Search Handler.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IMessageBase Execute(MVIMessages.CorrespondingIdsResponse request, UDOSelectedPersonRequest selectedPersonRequest)
        {
            UDOSelectedPersonResponse response = new UDOSelectedPersonResponse();
            UrlFormat = "https://{0}/main.aspx?etn=contact&pagetype=entityrecord&id=%7B{1}%7D";
            Stopwatch txnTimer = Stopwatch.StartNew();
            Stopwatch EntireTimer = Stopwatch.StartNew();
            try
            {
                if (request == null)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, $"{GetType().FullName}.Execute",
                        $"<< Exit from {GetType().FullName}. Recieved a null {request.GetType().Name} message request or request.Person.");
                }
                else
                {
                    if (selectedPersonRequest.SSId == null)
                    {
                        selectedPersonRequest.SSId = selectedPersonRequest.SSIdString.ToSecureString();
                    }
                    SecureString CorrIDSSId = request.SocialSecurityNumber.ToSecureString();

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
                        response.ExceptionOccured = true;
                        return response;
                    }
                    #endregion

                    if (OrgServiceProxy == null)
                    {
                        response.ExceptionOccured = true;
                        response.Message = "Cound not connect to CRM";
                        return response;
                    }

                    using (OrgServiceProxy)
                    {
                        if (request.CorrespondingIdList == null)
                        {
                            response.Message = "No Corresponding List was obtained from MVI, unable to create or find person";
                            return response;
                        }

                        // REM: See changes below
                        MVIMessages.UnattendedSearchRequest ssn = new MVIMessages.UnattendedSearchRequest();
                        MVIMessages.UnattendedSearchRequest edipi = new MVIMessages.UnattendedSearchRequest();
                        MVIMessages.UnattendedSearchRequest corpDb = new MVIMessages.UnattendedSearchRequest();

                        if (CorrIDSSId != null && CorrIDSSId.Length > 0)
                        {
                            ssn.PatientIdentifier = CorrIDSSId.ToUnsecureString();
                        }
                        else
                        {
                            ssn = request.CorrespondingIdList.FirstOrDefault(
                                (v => v.IdentifierType.Equals("SSN", StringComparison.InvariantCultureIgnoreCase) ||
                                      v.IdentifierType.Equals("SS", StringComparison.InvariantCultureIgnoreCase) ||
                                      v.IdentifierType.Equals("PI", StringComparison.InvariantCultureIgnoreCase)));
                            if (ssn == null)
                            {
                                ssn = request.CorrespondingIdList.FirstOrDefault(
                             (v => v.AssigningAuthority.Equals("USVBA", StringComparison.InvariantCultureIgnoreCase) &&
                                   v.AssigningFacility.Equals("200BRLS", StringComparison.InvariantCultureIgnoreCase) &&
                                   v.IdentifierType.Equals("PI", StringComparison.InvariantCultureIgnoreCase)));
                            }
                        }

                        MVIMessages.UnattendedSearchRequest icn = request.CorrespondingIdList.FirstOrDefault(
                             (v => v.AssigningAuthority.Equals("USVHA", StringComparison.InvariantCultureIgnoreCase) &&
                                   v.IdentifierType.Equals("NI", StringComparison.InvariantCultureIgnoreCase)));
                        if (!string.IsNullOrEmpty(request.Edipi))
                        {
                            edipi.PatientIdentifier = request.Edipi;
                        }
                        else
                        {
                            edipi = request.CorrespondingIdList.FirstOrDefault(
                               (v => v.AssigningAuthority.Equals("USDOD", StringComparison.InvariantCultureIgnoreCase) &&
                                   v.IdentifierType.Equals("NI", StringComparison.InvariantCultureIgnoreCase)));
                        }

                        int pidCount = request.CorrespondingIdList.Count((x => x.AssigningAuthority.Equals("USVBA", StringComparison.InvariantCultureIgnoreCase) &&
                                                        x.AssigningFacility.Equals("200CORP", StringComparison.InvariantCultureIgnoreCase) &&
                                                        x.IdentifierType.Equals("PI", StringComparison.InvariantCultureIgnoreCase)));

                        LogHelper.LogDebug(selectedPersonRequest.OrganizationName, selectedPersonRequest.Debug, selectedPersonRequest.UserId, "FindinCRMProcessor", string.Format("Count of CORP PIDs from MVI: {0}", pidCount));


                        if (!selectedPersonRequest.DuplicateCorpPID && pidCount > 1)
                        {
                            ///We have multiple active PIDs corralated. Need to set flag DuplicateCorpPID = true and return reponse
                            response.DuplicateCorpPID = true;
                            selectedPersonRequest.DuplicateCorpPID = true;
                            //Need to provide the original Corresponding IDs Response from MVI so as that we won't need to call it again.
                            response.CorrespondingIdsResponse = request;
                            return response;
                        }
                        else if (selectedPersonRequest.DuplicateCorpPID && selectedPersonRequest.participantID != 0)
                        {
                            corpDb.PatientIdentifier = selectedPersonRequest.participantID.ToString();
                        }
                        else
                        {
                            corpDb = request.CorrespondingIdList.FirstOrDefault(
                                (v => v.AssigningAuthority.Equals("USVBA", StringComparison.InvariantCultureIgnoreCase) &&
                                      v.AssigningFacility.Equals("200CORP", StringComparison.InvariantCultureIgnoreCase) &&
                                      v.IdentifierType.Equals("PI", StringComparison.InvariantCultureIgnoreCase)));
                        }


                        //if MVI doesn't return a PID, then we have an issue, or at least could have!
                        string corpDbString = (corpDb != null) ? corpDb.PatientIdentifier : null;
                        SecureString SSIdString = (ssn != null) ? ssn.PatientIdentifier.ToSecureString() : selectedPersonRequest.SSId;
                        string edipiString = (edipi != null) ? edipi.PatientIdentifier : null;
                        string icnString = (icn != null) ? icn.PatientIdentifier : null;

                        selectedPersonRequest.SSIdString = SSIdString.ToUnsecureString();
                        edipiString = (edipiString == "UNK") ? null : edipiString;
                        selectedPersonRequest.Edipi = edipiString;
                        selectedPersonRequest.ICN = icnString;

                        corpDbString = (corpDbString == "UNK") ? null : corpDbString;

                        txnTimer.Stop();

                        txnTimer.Restart();
                        if (corpDbString == null)
                        {
                            #region - exception processing for no PID in MVI - still work to do here

                            //eventually call MVI to do the unattendedsearch with orchestration
                            //let's call the same search we would have if we hadn't found the record in MVI - this handles filenumber and SSN searches
                            UDOPersonSearchRequest findVeteranRequest = new UDOPersonSearchRequest();
                            findVeteranRequest.MessageId = request.MessageId;
                            findVeteranRequest.LogTiming = selectedPersonRequest.LogTiming;
                            findVeteranRequest.LogSoap = selectedPersonRequest.LogSoap;
                            findVeteranRequest.Debug = selectedPersonRequest.Debug;
                            findVeteranRequest.RelatedParentEntityName = selectedPersonRequest.RelatedParentEntityName;
                            findVeteranRequest.RelatedParentFieldName = selectedPersonRequest.RelatedParentFieldName;
                            findVeteranRequest.RelatedParentId = selectedPersonRequest.RelatedParentId;
                            findVeteranRequest.UserId = selectedPersonRequest.UserId;
                            findVeteranRequest.OrganizationName = selectedPersonRequest.OrganizationName;
                            findVeteranRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
                            {
                                ApplicationName = selectedPersonRequest.LegacyServiceHeaderInfo.ApplicationName,
                                ClientMachine = selectedPersonRequest.LegacyServiceHeaderInfo.ClientMachine,
                                LoginName = selectedPersonRequest.LegacyServiceHeaderInfo.LoginName,
                                StationNumber = selectedPersonRequest.LegacyServiceHeaderInfo.StationNumber
                            };

                            if (selectedPersonRequest.SSId == null || selectedPersonRequest.SSId.Length == 0)
                                findVeteranRequest.SSIdString = SSIdString.ToUnsecureString();
                            else
                                findVeteranRequest.SSIdString = selectedPersonRequest.SSId.ToUnsecureString();

                            findVeteranRequest.IsAttended = selectedPersonRequest.IsAttended;
                            findVeteranRequest.Edipi = edipiString;
                            findVeteranRequest.AlreadyQueriedMvi = true;

                            UDOPersonSearchProcessor udoPersonSearchlogic = new UDOPersonSearchProcessor();
                            UDOPersonSearchResponse findVeteranResponse = (UDOPersonSearchResponse)udoPersonSearchlogic.Execute(findVeteranRequest);

                            txnTimer.Stop();

                            txnTimer.Restart();

                            if (findVeteranResponse != null)
                            {
                                if (findVeteranResponse.Person != null)
                                {
                                    corpDbString = findVeteranResponse.Person[0].ParticipantId;
                                }
                            }
                            //possibly return from here - will figuyre it out
                            #endregion
                        }

                        #region - go to CORP and get the current SSN/Filenumber based on PID
                        UDOgetVeteranIdentifiersRequest getVeteranIdentifiers = new UDOgetVeteranIdentifiersRequest();
                        getVeteranIdentifiers.MessageId = request.MessageId;
                        getVeteranIdentifiers.LogTiming = selectedPersonRequest.LogTiming;
                        getVeteranIdentifiers.LogSoap = selectedPersonRequest.LogSoap;
                        getVeteranIdentifiers.Debug = selectedPersonRequest.Debug;
                        getVeteranIdentifiers.RelatedParentEntityName = selectedPersonRequest.RelatedParentEntityName;
                        getVeteranIdentifiers.RelatedParentFieldName = selectedPersonRequest.RelatedParentFieldName;
                        getVeteranIdentifiers.RelatedParentId = selectedPersonRequest.RelatedParentId;
                        getVeteranIdentifiers.UserId = selectedPersonRequest.UserId;
                        getVeteranIdentifiers.OrganizationName = selectedPersonRequest.OrganizationName;
                        getVeteranIdentifiers.LegacyServiceHeaderInfo = new UDOHeaderInfo
                        {
                            ApplicationName = selectedPersonRequest.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = selectedPersonRequest.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = selectedPersonRequest.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = selectedPersonRequest.LegacyServiceHeaderInfo.StationNumber
                        };
                        UDOgetVeteranIdentifiersResponse getVeteranIdentifiersResponse = new UDOgetVeteranIdentifiersResponse();
                        if (corpDbString != null && corpDbString.Trim() != "")
                        {
                            getVeteranIdentifiers.ptcpntId = Convert.ToInt64(corpDbString);

                            selectedPersonRequest.participantID = getVeteranIdentifiers.ptcpntId;

                            UDOgetVeteranIdentifiersProcessor udoGetVeteranIdentifiersLogic = new UDOgetVeteranIdentifiersProcessor();
                            getVeteranIdentifiersResponse = (UDOgetVeteranIdentifiersResponse)udoGetVeteranIdentifiersLogic.Execute(getVeteranIdentifiers);

                            txnTimer.Stop();
                            txnTimer.Restart();
                        }

                        if (getVeteranIdentifiersResponse != null)
                        {
                            //check to see if there is an exception on the getVeteranIdentifier calls. If there is, return and don't do FindInCrm
                            response = CorpResponseValidation(getVeteranIdentifiersResponse.ExceptionOccured, getVeteranIdentifiersResponse.ExceptionMessage, request.OrganizationName, request.UserId, response);
                            LogHelper.LogDebug(selectedPersonRequest.OrganizationName, selectedPersonRequest.Debug, selectedPersonRequest.UserId, "FindinCRMProcessor, getVeteranIdentifiers: ", string.Format("Exception Occurred: {0}", response.ExceptionOccured));
                            if (response.ExceptionOccured == true)
                                return response;
                            if (getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo != null && getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_ParticipantID != null)
                            {
                                if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SensitivityLevel))
                                {
                                    if (string.IsNullOrEmpty(response.VeteranSensitivityLevel))
                                        response.VeteranSensitivityLevel = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SensitivityLevel;

                                    selectedPersonRequest.VeteranSensitivityLevel = Convert.ToInt32(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SensitivityLevel);
                                }
                                if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SSN))
                                {
                                    selectedPersonRequest.SSId = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SSN.ToSecureString();
                                }
                                if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_FileNumber))
                                {
                                    selectedPersonRequest.FileNumber = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_FileNumber;
                                }
                                if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_FirstName))
                                {
                                    selectedPersonRequest.FirstName = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_FirstName;
                                }
                                if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_LastName))
                                {
                                    selectedPersonRequest.FamilyName = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_LastName;
                                }
                                if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_MiddleName))
                                {
                                    selectedPersonRequest.MiddleName = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_MiddleName;
                                }
                                if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_Gender))
                                {
                                    response.GenderCode = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_Gender;
                                }
                                if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_PhoneNumber))
                                {
                                    response.phonenumber = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_PhoneNumber;
                                    selectedPersonRequest.PhoneNumber = response.phonenumber;
                                }
                                if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_BranchOfService))
                                {
                                    response.BranchOfService = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_BranchOfService;
                                    selectedPersonRequest.BranchOfService = response.BranchOfService;
                                }
                                if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SOJ))
                                {
                                    response.crme_stationofJurisdiction = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SOJ;
                                }
                                if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_DateofDeath))
                                {
                                    response.crme_DateofDeath = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_DateofDeath;
                                }
                                if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_characterofdischarge))
                                {
                                    response.crme_CharacterofDishcarge = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_characterofdischarge;
                                }
                            }
                            else
                            {
                                LogHelper.LogDebug(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, method, "CORPDB did not have the PID:" + selectedPersonRequest.participantID, selectedPersonRequest.Debug);

                                //Just for now , I HOPE, look up the data in CORP by FN
                                var r = selectedPersonRequest;
                                LogHelper.LogDebug(r.MessageId, r.OrganizationName, r.UserId, method, $"ALTERNATE SEARCH - VEISfvetfindVeteranRequest", r.Debug);
                                VEISfvetfindVeteranRequest findVeteranRequest = new VEISfvetfindVeteranRequest();
                                findVeteranRequest.MessageId = selectedPersonRequest.MessageId;
                                findVeteranRequest.LogTiming = selectedPersonRequest.LogTiming;
                                findVeteranRequest.LogSoap = selectedPersonRequest.LogSoap;
                                findVeteranRequest.Debug = selectedPersonRequest.Debug;
                                findVeteranRequest.RelatedParentEntityName = selectedPersonRequest.RelatedParentEntityName;
                                findVeteranRequest.RelatedParentFieldName = selectedPersonRequest.RelatedParentFieldName;
                                findVeteranRequest.RelatedParentId = selectedPersonRequest.RelatedParentId;
                                findVeteranRequest.UserId = selectedPersonRequest.UserId;
                                findVeteranRequest.OrganizationName = selectedPersonRequest.OrganizationName;
                                findVeteranRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                                {
                                    ApplicationName = selectedPersonRequest.LegacyServiceHeaderInfo.ApplicationName,
                                    ClientMachine = selectedPersonRequest.LegacyServiceHeaderInfo.ClientMachine,
                                    LoginName = selectedPersonRequest.LegacyServiceHeaderInfo.LoginName,
                                    StationNumber = selectedPersonRequest.LegacyServiceHeaderInfo.StationNumber
                                };
                                bool blnCanSearch = false;
                                //if (!string.IsNullOrEmpty(selectedPersonRequest.SSIdString))
                                if (selectedPersonRequest.SSId != null)
                                {
                                    blnCanSearch = true;
                                    findVeteranRequest.VEISfvetReqveteranRecordInputInfo = new VEISfvetReqveteranRecordInput()
                                    {
                                        mcs_ssn = selectedPersonRequest.SSId.ToUnsecureString()
                                    };
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(selectedPersonRequest.FileNumber))
                                    {
                                        blnCanSearch = true;
                                        findVeteranRequest.VEISfvetReqveteranRecordInputInfo = new VEISfvetReqveteranRecordInput()
                                        {
                                            mcs_fileNumber = selectedPersonRequest.FileNumber
                                        };
                                    }
                                    else
                                    {
                                        if (SSIdString.StringLength() > 0)
                                        {
                                            blnCanSearch = true;
                                            selectedPersonRequest.SSIdString = SSIdString.ToUnsecureString();
                                            findVeteranRequest.VEISfvetReqveteranRecordInputInfo = new VEISfvetReqveteranRecordInput()
                                            {
                                                mcs_ssn = selectedPersonRequest.SSIdString
                                            };
                                        }
                                    }
                                }
                                if (blnCanSearch)
                                {
                                    VEISfvetfindVeteranResponse findVeteranResponse = WebApiUtility.SendReceive<VEISfvetfindVeteranResponse>(findVeteranRequest, WebApiType.VEIS);

                                    txnTimer.Stop();
                                    if (findVeteranRequest.LogSoap || findVeteranResponse.ExceptionOccurred)
                                    {
                                        if (findVeteranResponse.SerializedSOAPRequest != null || findVeteranResponse.SerializedSOAPResponse != null)
                                        {
                                            var requestResponse = findVeteranResponse.SerializedSOAPRequest + findVeteranResponse.SerializedSOAPResponse;
                                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfvetfindVeteranRequest Request/Response {requestResponse}", true);
                                        }
                                    }

                                    txnTimer.Restart();

                                    //check to see if there is an exception on the FindVeteran calls. If there is, return and don't do FindInCrm
                                    response = CorpResponseValidation(findVeteranResponse.ExceptionOccurred, findVeteranResponse.ExceptionMessage, request.OrganizationName, request.UserId, response);
                                    if (response.ExceptionOccured == true)
                                        return response;
                                    #region - if there is data, map it
                                    if (findVeteranResponse.VEISfvetreturnInfo != null)
                                    {
                                        if (findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo != null)
                                        {
                                            VEISfvetvetCorpRecord vetCorpRecordInfo = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo;
                                            if (!string.IsNullOrEmpty(vetCorpRecordInfo.mcs_fileNumber))
                                            {
                                                if (!string.IsNullOrEmpty(vetCorpRecordInfo.mcs_sensitiveLevelOfRecord))
                                                {
                                                    int sLevel = 0;
                                                    Int32.TryParse(vetCorpRecordInfo.mcs_sensitiveLevelOfRecord, out sLevel);
                                                    selectedPersonRequest.VeteranSensitivityLevel = sLevel;
                                                }
                                                if (!string.IsNullOrEmpty(vetCorpRecordInfo.mcs_ptcpntId))
                                                {
                                                    selectedPersonRequest.participantID = Convert.ToInt64(vetCorpRecordInfo.mcs_ptcpntId);
                                                }

                                                if (vetCorpRecordInfo.mcs_lastName != null)
                                                {
                                                    selectedPersonRequest.FamilyName = vetCorpRecordInfo.mcs_lastName;
                                                }

                                                if (vetCorpRecordInfo.mcs_firstName != null)
                                                {
                                                    selectedPersonRequest.FirstName = vetCorpRecordInfo.mcs_firstName;
                                                }
                                                if (vetCorpRecordInfo.mcs_middleName != null)
                                                {
                                                    selectedPersonRequest.MiddleName = vetCorpRecordInfo.mcs_middleName;
                                                }
                                                if (vetCorpRecordInfo.mcs_fileNumber != null)
                                                {
                                                    selectedPersonRequest.FileNumber = vetCorpRecordInfo.mcs_fileNumber;
                                                }
                                                if (!string.IsNullOrEmpty(findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.mcs_SEX_CODE))
                                                {
                                                    response.GenderCode = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.mcs_SEX_CODE;
                                                }
                                                if (!string.IsNullOrEmpty(vetCorpRecordInfo.mcs_phoneNumberOne))
                                                {
                                                    response.phonenumber = "(" + vetCorpRecordInfo.mcs_areaNumberOne + ") " + vetCorpRecordInfo.mcs_phoneNumberOne.Substring(0, 3) + "-" + vetCorpRecordInfo.mcs_phoneNumberOne.Substring(3);
                                                    selectedPersonRequest.PhoneNumber = response.phonenumber;
                                                }
                                                if (findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.mcs_CLAIM_FOLDER_LOCATION != null)
                                                {
                                                    response.crme_stationofJurisdiction = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.mcs_CLAIM_FOLDER_LOCATION;
                                                }
                                                if (findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.mcs_DATE_OF_DEATH != null)
                                                {
                                                    response.crme_DateofDeath = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.mcs_DATE_OF_DEATH;
                                                }

                                                if (findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.VEISfvetSERVICEInfo != null)
                                                {
                                                    VEISfvetSERVICEMultipleResponse[] serviceDTO = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.VEISfvetSERVICEInfo;
                                                    foreach (VEISfvetSERVICEMultipleResponse item in serviceDTO)
                                                    {
                                                        if (!string.IsNullOrEmpty(item.mcs_BRANCH_OF_SERVICE))
                                                        {
                                                            response.BranchOfService = item.mcs_BRANCH_OF_SERVICE;
                                                            selectedPersonRequest.BranchOfService = item.mcs_BRANCH_OF_SERVICE;
                                                        }
                                                    }

                                                    var charOfSvcCode = serviceDTO.OrderByDescending(h => DateTime.TryParse(h.mcs_ENTERED_ON_DUTY_DATE, out DateTime newDateTime)).FirstOrDefault().mcs_CHAR_OF_SVC_CODE;
                                                    if (!string.IsNullOrEmpty(charOfSvcCode)) response.crme_CharacterofDishcarge = charOfSvcCode;
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        #endregion

                        if (response.Edipi == null && (edipiString != null && edipiString.Trim() != ""))
                            response.Edipi = edipiString;
                        if (response.FileNumber == null && (selectedPersonRequest.FileNumber != null && selectedPersonRequest.FileNumber.Trim() != ""))
                            response.FileNumber = selectedPersonRequest.FileNumber;
                        if (response.ParticipantId == null && selectedPersonRequest.participantID != null)
                            response.ParticipantId = selectedPersonRequest.participantID.ToString();

                        if (response.SSIdString == null && (SSIdString != null && SSIdString.ToUnsecureString().Trim() != ""))
                            response.SSIdString = SSIdString.ToUnsecureString();

                        //If we made it this fare with DuplicateCorpPID = true then we need to return out with all corp data returned
                        //for specified duplicate record searched in corp
                        if (selectedPersonRequest.DuplicateCorpPID)
                            return response;

                        Guid? crmPersonId2 = CommonFunctions.TryGetCrmPerson(OrgServiceProxy, ref selectedPersonRequest, "MVI");

                        txnTimer.Stop();
                        txnTimer.Restart();
                        if (crmPersonId2 == Guid.Empty)
                        {
                            crmPersonId2 = CommonFunctions.TryCreateNewCrmPerson(OrgServiceProxy, ref selectedPersonRequest, "MVI");
                            txnTimer.Stop();
                            txnTimer.Restart();
                        }

                        //TODO: Commented the URL property 
                        //CrmConfiguration.GetConfigurationSettings()[CrmConfiguration.BaseUrl];
                        string crmBaseUrl = ConnectionCache.ConnectManager.BaseUrl;
                        response.URL = $@"{crmBaseUrl}/main.aspx?etn=contact&pagetype=entityrecord&id=%7B{crmPersonId2.Value}%7D";
                        response.contactId = crmPersonId2.Value;
                        response.VeteranSensitivityLevel = selectedPersonRequest.VeteranSensitivityLevel.ToString();
                        response.OwningTeamId = selectedPersonRequest.OwningTeamId;
                    }
                }
                EntireTimer.Stop();

                return response;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "#Error in FindinCRMProcessor.Execute ", ex);
                response.Message = ex.Message;
                response.ExceptionOccured = true;
                return response;
            }
        }

        public UDOSelectedPersonResponse CorpResponseValidation(bool exceptionOccured, string exceptionMessage, string orgName, Guid userId, UDOSelectedPersonResponse response)
        {
            if (exceptionOccured == true)
            {
                if (exceptionMessage.Contains("access violation") || exceptionMessage == "sensitive record check error")
                {
                    //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                    response.ExceptionOccured = true;
                    response.Message = exceptionMessage;
                }
                else if (exceptionMessage == "BIRLS communication is down" || exceptionMessage == "The Tuxedo service is down")
                {
                    //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                    LogHelper.LogError(orgName, userId, "FindInCrmProcessor - exception in findVeteran", exceptionMessage);
                    response.Message = exceptionMessage;
                    response.ExceptionOccured = true;
                }
                else if (exceptionMessage.Contains("there was no endpoint listening"))
                {
                    //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                    LogHelper.LogError(orgName, userId, "CombinedSecondarySearchProcessor - exception in findVeteran", exceptionMessage);
                    response.Message = CorpDbSearchBackendServicesDown;
                    response.ExceptionOccured = true;
                }
                else
                {
                    //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                    LogHelper.LogError(orgName, userId, "CombinedSecondarySearchProcessor - exception in findVeteran", exceptionMessage);
                    response.Message = CorpDbSearchUnknownError;
                    response.ExceptionOccured = true;
                }
            }
            return response;
        }
        /// <summary>
        /// Selected Person Search Processor for all records where record source is not MVI
        /// </summary>
        /// <param name="request"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public IMessageBase Execute(UDOSelectedPersonRequest selectedPersonRequest)
        {

            UDOSelectedPersonResponse response = new UDOSelectedPersonResponse();
            Stopwatch txnTimer = Stopwatch.StartNew();
            Stopwatch EntireTimer = Stopwatch.StartNew();
            try
            {
                UrlFormat = "https://{0}/main.aspx?etn=contact&pagetype=entityrecord&id=%7B{1}%7D";
                //get the connection parms for this org

                if (selectedPersonRequest == null)
                {
                    LogHelper.LogError(selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, $"#Error in {GetType().FullName} ", "Recieved a null UDOSelectedPersonRequest");
                }
                else
                {
                    if (selectedPersonRequest.SSId == null)
                    {
                        selectedPersonRequest.SSId = SecurityTools.ConvertToSecureString(selectedPersonRequest.SSIdString);
                    }
                    CRMCommonFunctions CommonFunctions = new CRMCommonFunctions();

                    #region Connect to CRM
                    OrganizationWebProxyClient webProxyClient = ConnectionCache.GetProxy().OrganizationWebProxyClient;
                    if (webProxyClient != null)
                    {
                        webProxyClient.CallerId = selectedPersonRequest.UserId;
                    }
                    #endregion

                    using (webProxyClient)
                    {
                        IOrganizationService connection = webProxyClient as IOrganizationService;

                        //let's call the same search we would have if we hadn't found the record in MVI - this handles filenumber and SSN searches
                        UDOfindVeteranInfoRequest findVeteranRequest = new UDOfindVeteranInfoRequest();
                        findVeteranRequest.MessageId = selectedPersonRequest.MessageId;
                        findVeteranRequest.LogTiming = selectedPersonRequest.LogTiming;
                        findVeteranRequest.LogSoap = selectedPersonRequest.LogSoap;
                        findVeteranRequest.Debug = selectedPersonRequest.Debug;
                        findVeteranRequest.RelatedParentEntityName = selectedPersonRequest.RelatedParentEntityName;
                        findVeteranRequest.RelatedParentFieldName = selectedPersonRequest.RelatedParentFieldName;
                        findVeteranRequest.RelatedParentId = selectedPersonRequest.RelatedParentId;
                        findVeteranRequest.UserId = selectedPersonRequest.UserId;
                        findVeteranRequest.OrganizationName = selectedPersonRequest.OrganizationName;
                        findVeteranRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
                        {
                            ApplicationName = selectedPersonRequest.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = selectedPersonRequest.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = selectedPersonRequest.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = selectedPersonRequest.LegacyServiceHeaderInfo.StationNumber
                        };

                        findVeteranRequest.SocialSN = selectedPersonRequest.SSIdString;
                        findVeteranRequest.fileNumber = selectedPersonRequest.FileNumber;
                        UDOfindVeteranInfoProcessor findVeteranLogic = new UDOfindVeteranInfoProcessor();
                        UDOfindVeteranInfoResponse findVeteranResponse = (UDOfindVeteranInfoResponse)findVeteranLogic.Execute(findVeteranRequest);

                        txnTimer.Stop();
                        txnTimer.Restart();
                        if (findVeteranResponse.UDOfindVeteranInfoInfo != null)
                        {
                            Int64 pid = 0;
                            Int64.TryParse(findVeteranResponse.UDOfindVeteranInfoInfo.crme_ParticipantID, out pid);
                            selectedPersonRequest.participantID = pid;
                            selectedPersonRequest.FirstName = findVeteranResponse.UDOfindVeteranInfoInfo.crme_FirstName;
                            selectedPersonRequest.FamilyName = findVeteranResponse.UDOfindVeteranInfoInfo.crme_LastName;
                            selectedPersonRequest.FileNumber = findVeteranResponse.UDOfindVeteranInfoInfo.crme_FileNumber;
                            selectedPersonRequest.PhoneNumber = findVeteranResponse.UDOfindVeteranInfoInfo.crme_PrimaryPhone;
                            selectedPersonRequest.VeteranSensitivityLevel = (findVeteranResponse.UDOfindVeteranInfoInfo.crme_VeteranSensitivityLevel != null) ? Int32.Parse(findVeteranResponse.UDOfindVeteranInfoInfo.crme_VeteranSensitivityLevel) : 0;

                            response.BirthDate = findVeteranResponse.UDOfindVeteranInfoInfo.crme_DOB.ToString("MM/dd/yyyy");
                            response.BranchOfService = findVeteranResponse.UDOfindVeteranInfoInfo.crme_BranchOfService;
                            response.FileNumber = findVeteranResponse.UDOfindVeteranInfoInfo.crme_FileNumber;
                            response.ParticipantId = findVeteranResponse.UDOfindVeteranInfoInfo.crme_ParticipantID;
                            response.SSIdString = selectedPersonRequest.SSIdString;
                            response.VeteranSensitivityLevel = selectedPersonRequest.VeteranSensitivityLevel.ToString();
                            response.FirstName = findVeteranResponse.UDOfindVeteranInfoInfo.crme_FirstName;
                            response.FamilyName = findVeteranResponse.UDOfindVeteranInfoInfo.crme_LastName;
                            response.GenderCode = findVeteranResponse.UDOfindVeteranInfoInfo.crme_Gender;
                            response.phonenumber = findVeteranResponse.UDOfindVeteranInfoInfo.crme_PrimaryPhone;
                        }

                        Guid? crmPersonId = CommonFunctions.TryGetCrmPerson(connection, ref selectedPersonRequest, "CorpdDB");
                        txnTimer.Stop();
                        txnTimer.Restart();

                        if (crmPersonId == Guid.Empty)
                        {
                            crmPersonId = CommonFunctions.TryCreateNewCrmPerson(connection, ref selectedPersonRequest, "CorpdDB");
                            txnTimer.Stop();
                            txnTimer.Restart();
                        }

                        response.OwningTeamId = selectedPersonRequest.OwningTeamId;
                        response.contactId = crmPersonId.Value;
                        response.VeteranSensitivityLevel = findVeteranResponse.UDOfindVeteranInfoInfo.crme_VeteranSensitivityLevel;

                        //TODO: Commented the use of connParms; Review this.
                        if (!selectedPersonRequest.noAddPerson)
                        {
                            //new code - basically if a person is in Corpdb but not MVI, this will do an async add of that person
                            UDOAddPersonRequest addPersonRequest = new UDOAddPersonRequest();
                            addPersonRequest.MessageId = selectedPersonRequest.MessageId;
                            addPersonRequest.ContactId = response.contactId;
                            addPersonRequest.LogTiming = selectedPersonRequest.LogTiming;
                            addPersonRequest.LogSoap = selectedPersonRequest.LogSoap;
                            addPersonRequest.Debug = selectedPersonRequest.Debug;
                            addPersonRequest.RelatedParentEntityName = selectedPersonRequest.RelatedParentEntityName;
                            addPersonRequest.RelatedParentFieldName = selectedPersonRequest.RelatedParentFieldName;
                            addPersonRequest.RelatedParentId = selectedPersonRequest.RelatedParentId;
                            addPersonRequest.UserId = selectedPersonRequest.UserId;
                            addPersonRequest.OrganizationName = selectedPersonRequest.OrganizationName;
                            addPersonRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
                            {
                                ApplicationName = selectedPersonRequest.LegacyServiceHeaderInfo.ApplicationName,
                                ClientMachine = selectedPersonRequest.LegacyServiceHeaderInfo.ClientMachine,
                                LoginName = selectedPersonRequest.LegacyServiceHeaderInfo.LoginName,
                                StationNumber = selectedPersonRequest.LegacyServiceHeaderInfo.StationNumber
                            };

                            addPersonRequest.SSIdString = selectedPersonRequest.SSIdString;
                            addPersonRequest.userfirstname = selectedPersonRequest.UserFirstName;
                            addPersonRequest.userlastname = selectedPersonRequest.UserLastName;
                            addPersonRequest.MVICheck = selectedPersonRequest.MVICheck;
                            LogHelper.LogDebug(selectedPersonRequest.OrganizationName, selectedPersonRequest.Debug, selectedPersonRequest.UserId, "FindinCRMProcessor, Execute", "Sending Stuff to AddPerson");

                            WebApiUtility.SendAsync(addPersonRequest, WebApiType.LOB);

                            txnTimer.Stop();
                            LogHelper.LogTiming(selectedPersonRequest.OrganizationName, selectedPersonRequest.LogTiming, selectedPersonRequest.UserId, selectedPersonRequest.RelatedParentId, selectedPersonRequest.RelatedParentEntityName, selectedPersonRequest.RelatedParentFieldName, "addPersonRequest - Corpdb", null, txnTimer.ElapsedMilliseconds);
                            txnTimer.Restart();
                        }
                        else
                        {
                            LogHelper.LogDebug(selectedPersonRequest.OrganizationName, selectedPersonRequest.Debug, selectedPersonRequest.UserId, "FindinCRMProcessor, Execute", "Not Adding Person due to Setting");
                        }
                    }
                }
                EntireTimer.Stop();
                LogHelper.LogTiming(selectedPersonRequest.OrganizationName, selectedPersonRequest.LogTiming, selectedPersonRequest.UserId, selectedPersonRequest.RelatedParentId, selectedPersonRequest.RelatedParentEntityName, selectedPersonRequest.RelatedParentFieldName, "EntireTimer", null, txnTimer.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, "FindInCrmProcessor, Execute", "Exception:" + ex.Message);
                response.ExceptionOccured = true;
                response.Message = ex.Message;
                return response;
            }
        }
        /// <summary>
        /// Combined Selected Person Search Processor for all records where record source is not MVI
        /// </summary>
        /// <param name="request"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public IMessageBase Execute(UDOCombinedSelectedPersonRequest selectedPersonRequest)
        {

            UDOCombinedSelectedPersonResponse response = new UDOCombinedSelectedPersonResponse();
            Stopwatch txnTimer = Stopwatch.StartNew();
            Stopwatch EntireTimer = Stopwatch.StartNew();
            try
            {
                UrlFormat = "https://{0}/main.aspx?etn=contact&pagetype=entityrecord&id=%7B{1}%7D";

                if (selectedPersonRequest == null)
                {
                    LogHelper.LogError(selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, $"{GetType().FullName}.Execute", $"<< Exit from {GetType().FullName}. Recieved a null UDOCombinedSelectedPersonRequest request.");

                }
                else
                {
                    CRMCommonFunctions CommonFunctions = new CRMCommonFunctions();

                    #region Connect to CRM
                    // Removed CRM connection as it wasn't being used
                    #endregion

                    //let's call the same search we would have if we hadn't found the record in MVI - this handles filenumber and SSN searches
                    UDOfindVeteranInfoRequest findVeteranRequest = new UDOfindVeteranInfoRequest();
                    findVeteranRequest.MessageId = selectedPersonRequest.MessageId;
                    findVeteranRequest.LogTiming = selectedPersonRequest.LogTiming;
                    findVeteranRequest.LogSoap = selectedPersonRequest.LogSoap;
                    findVeteranRequest.Debug = selectedPersonRequest.Debug;
                    findVeteranRequest.RelatedParentEntityName = selectedPersonRequest.RelatedParentEntityName;
                    findVeteranRequest.RelatedParentFieldName = selectedPersonRequest.RelatedParentFieldName;
                    findVeteranRequest.RelatedParentId = selectedPersonRequest.RelatedParentId;
                    findVeteranRequest.UserId = selectedPersonRequest.UserId;
                    findVeteranRequest.OrganizationName = selectedPersonRequest.OrganizationName;
                    findVeteranRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
                    {
                        ApplicationName = selectedPersonRequest.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = selectedPersonRequest.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = selectedPersonRequest.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = selectedPersonRequest.LegacyServiceHeaderInfo.StationNumber
                    };

                    findVeteranRequest.SocialSN = selectedPersonRequest.SSIdString;
                    findVeteranRequest.fileNumber = selectedPersonRequest.FileNumber;

                    LogHelper.LogDebug(selectedPersonRequest.OrganizationName, selectedPersonRequest.Debug, selectedPersonRequest.UserId, "FindinCRMProcessor, Execute", "Trying to call UDOPersonSearchRequest, non MVI Call");

                    UDOfindVeteranInfoProcessor findVeteranInfoProcessor = new UDOfindVeteranInfoProcessor();
                    UDOfindVeteranInfoResponse findVeteranResponse = (UDOfindVeteranInfoResponse)findVeteranInfoProcessor.Execute(findVeteranRequest);

                    txnTimer.Stop();
                    txnTimer.Restart();
                    if (findVeteranResponse.UDOfindVeteranInfoInfo != null)
                    {
                        Int64 pid = 0;
                        Int64.TryParse(findVeteranResponse.UDOfindVeteranInfoInfo.crme_ParticipantID, out pid);
                        selectedPersonRequest.participantID = pid;
                        selectedPersonRequest.FirstName = findVeteranResponse.UDOfindVeteranInfoInfo.crme_FirstName;
                        selectedPersonRequest.FamilyName = findVeteranResponse.UDOfindVeteranInfoInfo.crme_LastName;
                        selectedPersonRequest.FileNumber = findVeteranResponse.UDOfindVeteranInfoInfo.crme_FileNumber;
                        selectedPersonRequest.PhoneNumber = findVeteranResponse.UDOfindVeteranInfoInfo.crme_PrimaryPhone;
                        selectedPersonRequest.VeteranSensitivityLevel = (findVeteranResponse.UDOfindVeteranInfoInfo.crme_VeteranSensitivityLevel != null) ? Int32.Parse(findVeteranResponse.UDOfindVeteranInfoInfo.crme_VeteranSensitivityLevel) : 0;

                        response.BirthDate = findVeteranResponse.UDOfindVeteranInfoInfo.crme_DOB.ToString("MM/dd/yyyy");
                        response.BranchOfService = findVeteranResponse.UDOfindVeteranInfoInfo.crme_BranchOfService;
                        response.FileNumber = findVeteranResponse.UDOfindVeteranInfoInfo.crme_FileNumber;
                        response.ParticipantId = findVeteranResponse.UDOfindVeteranInfoInfo.crme_ParticipantID;
                        response.SSIdString = selectedPersonRequest.SSIdString;
                        response.VeteranSensitivityLevel = selectedPersonRequest.VeteranSensitivityLevel.ToString();
                        response.FirstName = findVeteranResponse.UDOfindVeteranInfoInfo.crme_FirstName;
                        response.FamilyName = findVeteranResponse.UDOfindVeteranInfoInfo.crme_LastName;
                        response.GenderCode = findVeteranResponse.UDOfindVeteranInfoInfo.crme_Gender;
                        response.phonenumber = findVeteranResponse.UDOfindVeteranInfoInfo.crme_PrimaryPhone;
                    }

                    response.VeteranSensitivityLevel = findVeteranResponse.UDOfindVeteranInfoInfo.crme_VeteranSensitivityLevel;

                    if (!selectedPersonRequest.noAddPerson)
                    {
                        //new code - basically if a person is in Corpdb but not MVI, this will do an async add of that person
                        UDOAddPersonRequest addPersonRequest = new UDOAddPersonRequest();
                        addPersonRequest.MessageId = selectedPersonRequest.MessageId;
                        addPersonRequest.ContactId = response.contactId;
                        addPersonRequest.LogTiming = selectedPersonRequest.LogTiming;
                        addPersonRequest.LogSoap = selectedPersonRequest.LogSoap;
                        addPersonRequest.Debug = selectedPersonRequest.Debug;
                        addPersonRequest.RelatedParentEntityName = selectedPersonRequest.RelatedParentEntityName;
                        addPersonRequest.RelatedParentFieldName = selectedPersonRequest.RelatedParentFieldName;
                        addPersonRequest.RelatedParentId = selectedPersonRequest.RelatedParentId;
                        addPersonRequest.UserId = selectedPersonRequest.UserId;
                        addPersonRequest.OrganizationName = selectedPersonRequest.OrganizationName;
                        addPersonRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
                        {
                            ApplicationName = selectedPersonRequest.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = selectedPersonRequest.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = selectedPersonRequest.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = selectedPersonRequest.LegacyServiceHeaderInfo.StationNumber
                        };

                        addPersonRequest.SSIdString = selectedPersonRequest.SSIdString;
                        addPersonRequest.userfirstname = selectedPersonRequest.UserFirstName;
                        addPersonRequest.userlastname = selectedPersonRequest.UserLastName;
                        addPersonRequest.MVICheck = selectedPersonRequest.MVICheck;
                        LogHelper.LogDebug(selectedPersonRequest.OrganizationName, selectedPersonRequest.Debug, selectedPersonRequest.UserId, method, "Sending Stuff to AddPerson");

                        //REM: Invoked the WebApiUtility.SendAsync
                        WebApiUtility.SendAsync(addPersonRequest, WebApiType.LOB);
                        txnTimer.Stop();
                        LogHelper.LogTiming(selectedPersonRequest.OrganizationName, selectedPersonRequest.LogTiming, selectedPersonRequest.UserId, selectedPersonRequest.RelatedParentId, selectedPersonRequest.RelatedParentEntityName, selectedPersonRequest.RelatedParentFieldName, "Addperson ASYNC in FindinCRM", null, txnTimer.ElapsedMilliseconds);
                        txnTimer.Restart();
                    }
                    else
                    {
                        LogHelper.LogDebug(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, method, "Not Adding Person due to Setting", selectedPersonRequest.Debug);
                    }
                }
                EntireTimer.Stop();
                LogHelper.LogTiming(selectedPersonRequest.OrganizationName, selectedPersonRequest.LogTiming, selectedPersonRequest.UserId, selectedPersonRequest.RelatedParentId, selectedPersonRequest.RelatedParentEntityName, selectedPersonRequest.RelatedParentFieldName, "FindinCRM", null, EntireTimer.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, method, ex);
                response.ExceptionOccured = true;
                response.Message = ex.Message;
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="retrieveContact"></param>
        /// <returns></returns>
        private static UDOPersonSearchResponse MaptoResponseCRM(Entity retrieveContact)
        {
            UDOPersonSearchResponse finalResponse = new UDOPersonSearchResponse();

            System.Collections.Generic.List<PatientPerson> patRecord = new System.Collections.Generic.List<PatientPerson>();
            PatientPerson thisRecord = new PatientPerson();
            thisRecord.GenderCode = retrieveContact.GetAttributeValue<string>("udo_gender");
            thisRecord.PhoneNumber = retrieveContact.GetAttributeValue<string>("telephone1");
            thisRecord.SSIdString = retrieveContact.GetAttributeValue<string>("udo_ssn");
            thisRecord.BirthDate = retrieveContact.GetAttributeValue<string>("udo_birthdatestring");
            thisRecord.RecordSource = "UDO";
            thisRecord.ParticipantId = retrieveContact.GetAttributeValue<string>("udo_participantid");
            thisRecord.BranchOfService = retrieveContact.GetAttributeValue<string>("udo_branchofservice");
            thisRecord.EdiPi = retrieveContact.GetAttributeValue<string>("udo_edipi");

            Name newName = new Name();
            newName.FamilyName = retrieveContact.GetAttributeValue<string>("lastname");
            newName.GivenName = retrieveContact.GetAttributeValue<string>("firstname");
            newName.NameType = "Legal";
            newName.MiddleName = retrieveContact.GetAttributeValue<string>("middlename");
            System.Collections.Generic.List<Name> newArr = new System.Collections.Generic.List<Name>();
            newArr.Add(newName);
            thisRecord.NameList = newArr.ToArray();

            PatientAddress newAddress = new PatientAddress();
            newAddress.City = retrieveContact.GetAttributeValue<string>("address1_city");
            newAddress.StreetAddressLine = retrieveContact.GetAttributeValue<string>("address1_line1");

            thisRecord.Address = newAddress;
            
            patRecord.Add(thisRecord);
            
            finalResponse.Person = patRecord.ToArray();
            
            return finalResponse;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="retrieveContact"></param>
        /// <returns></returns>
        private static UDOSelectedPersonResponse MaptoSelectedPersonResponseCRM(Entity retrieveContact)
        {

            UDOSelectedPersonResponse selectedPerson = new UDOSelectedPersonResponse();
            //selectedPerson.contactId = (Guid)retrieveContact.ContactId;
            if (retrieveContact.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel") != null)
            {
                selectedPerson.VeteranSensitivityLevel = retrieveContact.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel").Value.ToString();
            }

            return selectedPerson;
        }
    }
}
