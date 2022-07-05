using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;
using System.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Common;
using UDO.LOB.MVI.Messages;
using UDO.LOB.VeteranSnapShot.Messages;

namespace UDO.LOB.MVI.Processors
{
    public class UDOCombinedSelectedPersonProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOCombinedSelectedPersonProcessor";
        private string LogBuffer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IMessageBase Execute(UDOCombinedSelectedPersonRequest req)
        {
            UDOCombinedSelectedPersonResponse response = new UDOCombinedSelectedPersonResponse();
            UDOSelectedPersonRequest request = null;
           
            Guid owningTeamId = Guid.Empty;
            LogHelper.LogDebug(req.MessageId, req.OrganizationName, req.UserId, method, "Top", req.Debug);
            LogBuffer = string.Empty;
            _debug = req.Debug;
            string progressString = method + ".Execute";
            try
            {
                if (req == null)
                {
                    if (request != null)
                    {
                        // REM: Log and set response to 
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, $"{GetType().FullName}.Execute",
                            $"<< Exit from {GetType().FullName}. Recieved a null {request.GetType().Name} message request or request.Person.");
                    }

                    if (response != null)
                    {
                        response.MVIMessage = "Called with no message";
                        response.ExceptionOccured = true;
                    }

                    return response;
                }
                else
                {
                    CRMCommonFunctions CommonFunctions = new CRMCommonFunctions();

                    #region Connect to CRM
                    OrganizationWebProxyClient webProxyClient = ConnectionCache.GetProxy().OrganizationWebProxyClient;
                    if (webProxyClient != null)
                    {
                        webProxyClient.CallerId = req.UserId;
                    }
                    #endregion

                    using (webProxyClient)
                    {
                        IOrganizationService OrgServiceProxy = webProxyClient as IOrganizationService;


                        if (OrgServiceProxy == null)
                            throw new ApplicationException("Cound not connect to CRM");

                        LogHelper.LogDebug(req.MessageId, req.OrganizationName, req.UserId, "UDOCombinedSelectedPersonProcessor", string.Format("Inbound Filenumber {0}", req.FileNumber), req.Debug);

                        request = new UDOSelectedPersonRequest()
                        {
                            AssigningAuthority = string.IsNullOrEmpty(req.AssigningAuthority) ? null : req.AssigningAuthority,
                            AssigningFacility = string.IsNullOrEmpty(req.AssigningFacility) ? null : req.AssigningAuthority,
                            BranchOfService = string.IsNullOrEmpty(req.BranchOfService) ? null : req.BranchOfService,
                            CorrespondingIdList = req.CorrespondingIdList == null ? null : req.CorrespondingIdList,
                            DateofBirth = string.IsNullOrEmpty(req.DateofBirth) ? null : req.DateofBirth,
                            Debug = req.Debug,
                            Edipi = string.IsNullOrEmpty(req.Edipi) ? null : req.Edipi,
                            FamilyName = string.IsNullOrEmpty(req.FamilyName) ? null : req.FamilyName,
                            FileNumber = string.IsNullOrEmpty(req.FileNumber) ? null : req.FileNumber,
                            FirstName = string.IsNullOrEmpty(req.FirstName) ? null : req.FirstName,
                            FullAddress = string.IsNullOrEmpty(req.FullAddress) ? null : req.FullAddress,
                            FullName = string.IsNullOrEmpty(req.FullName) ? null : req.FullName,
                            Gender = string.IsNullOrEmpty(req.Gender) ? null : req.Gender,
                            ICN = string.IsNullOrEmpty(req.ICN) ? null : req.ICN,
                            IdentifierClassCode = string.IsNullOrEmpty(req.IdentifierClassCode) ? null : req.IdentifierClassCode,
                            IdentifierType = string.IsNullOrEmpty(req.IdentifierType) ? null : req.IdentifierType,
                            IsAttended = req.IsAttended,
                            LegacyServiceHeaderInfo = req.LegacyServiceHeaderInfo == null ? null : req.LegacyServiceHeaderInfo,
                            LogSoap = req.LogSoap,
                            LogTiming = req.LogTiming,
                            MessageId = string.IsNullOrEmpty(req.MessageId) ? Guid.NewGuid().ToString() : req.MessageId,
                            MiddleName = string.IsNullOrEmpty(req.MiddleName) ? null : req.MiddleName,
                            MVICheck = req.MVICheck,
                            noAddPerson = req.noAddPerson,
                            OrganizationName = string.IsNullOrEmpty(req.OrganizationName) ? null : req.OrganizationName,
                            participantID = req.participantID == 0 ? 0 : req.participantID,
                            PatientSearchIdentifier = string.IsNullOrEmpty(req.PatientSearchIdentifier) ? null : req.PatientSearchIdentifier,
                            RecordSource = string.IsNullOrEmpty(req.RecordSource) ? null : req.RecordSource,
                            RelatedParentEntityName = string.IsNullOrEmpty(req.RelatedParentEntityName) ? null : req.RelatedParentEntityName,
                            RelatedParentFieldName = string.IsNullOrEmpty(req.RelatedParentFieldName) ? null : req.RelatedParentFieldName,
                            RawValueFromMvi = string.IsNullOrEmpty(req.RawValueFromMvi) ? null : req.RawValueFromMvi,
                            RelatedParentId = req.RelatedParentId,
                            SSId = string.IsNullOrEmpty(req.SSIdString) ? null : req.SSIdString.ToSecureString(),
                            UserFirstName = string.IsNullOrEmpty(req.UserFirstName) ? null : req.UserFirstName,
                            UserId = req.UserId,
                            UserLastName = string.IsNullOrEmpty(req.UserLastName) ? null : req.UserLastName,
                            VeteranSensitivityLevel = req.VeteranSensitivityLevel
                        };

                        if (request.participantID == 0)
                        {
                            long convertedParticipantId;
                            Int64.TryParse(req.ParticipantId, out convertedParticipantId);

                            request.participantID = convertedParticipantId;
                        }
                        if (request.DiagnosticsContext == null && request != null)
                        {
                            request.DiagnosticsContext = new DiagnosticsContext()
                            {
                                AgentId = request.UserId,
                                MessageTrigger = method,
                                OrganizationName = request.OrganizationName,
                                StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                            };
                        }
                        PatientPerson selectedPerson = new PatientPerson()
                        {
                            Address = new PatientAddress()
                            {
                                StreetAddressLine = req.FullAddress
                            },
                            BirthDate = req.DateofBirth,
                            CorrespondingIdList = req.CorrespondingIdList,
                            DeceasedDate = req.DeceasedDate,
                            EdiPi = req.Edipi,
                            GenderCode = req.Gender,
                            Identifier = req.PatientSearchIdentifier,
                            IdentifierType = req.IdentifierType,
                            IdentifyTheft = req.IdentityTheft,
                            ParticipantId = request.participantID.ToString(),
                            PhoneNumber = req.PhoneNumber,
                            RecordSource = req.RecordSource,
                            SSId = req.SSIdString.ToSecureString(),
                            NameList = new Name[2],
                            FileNumber = req.FileNumber,
                            BranchOfService = req.BranchOfService,
                            VeteranSensitivityLevel = req.VeteranSensitivityLevel.ToString()
                        };
                        Name nameList = new Name()
                        {
                            FamilyName = req.FamilyName,
                            GivenName = req.FirstName,
                            MiddleName = req.MiddleName,
                            NamePrefix = req.PrefixName,
                            NameSuffix = req.SuffixName,
                            NameType = "Legal"
                        };

                        selectedPerson.NameList[0] = nameList;
                        if (req.AliasName != null && req.AliasName.Trim() != "")
                        {
                            Name nameListAlias = new Name()
                            {
                                GivenName = req.AliasName,
                                NameType = "Alias"
                            };
                            selectedPerson.NameList[1] = nameListAlias;
                        }
                        response.Person = new PatientPerson[1];
                        response.Person[0] = selectedPerson;


                        #region create ID Proof


                        if (request.RecordSource == "MVI")
                        {
                            #region Handle MVI RecordSource
                            progressString = method + " - request.RecordSource - MVI";
                            PatientPerson person = response.Person[0];

                            UDOSelectedPersonProcessor selectedPersonLogic = new UDOSelectedPersonProcessor();
                            UDOSelectedPersonResponse thisNewResponse = (UDOSelectedPersonResponse)selectedPersonLogic.Execute(request);

                            if (thisNewResponse == null || thisNewResponse.ExceptionOccured == true)
                            {
                                //if the orchestration or FindInCrm fail, send the message back, but don't return the person since there will be no ID Proof or contactId
                                response.MVIMessage = "MVI search failed. " + thisNewResponse.Message;
                                response.RawMviExceptionMessage = thisNewResponse.RawMviExceptionMessage;
                                response.Person = null;
                                return response;
                            }

                            if (string.IsNullOrEmpty(person.VeteranSensitivityLevel) || person.VeteranSensitivityLevel == "0")
                                response.Person[0].VeteranSensitivityLevel = thisNewResponse.VeteranSensitivityLevel;

                            ///TODO: Implement Sensitivity Level Check. Need to add userSL to CombinedSelectedPersonRequest object
                            if (response.Person[0].VeteranSensitivityLevel != null)
                            {
                                int realSL = 0;
                                if (Int32.TryParse(response.Person[0].VeteranSensitivityLevel, out realSL))
                                {
                                    if (req.userSL < realSL)
                                    {
                                        response.MVIMessage = string.Format("In UDO your user record is set to a lower Sensitivity Level than CSS. To service this veteran you must have an SL of {0} or higher. Please contact your Administrator to have your level changed.  In the meanwhile, you will have to transfer this call. ", realSL);
                                        response.Person = null;
                                        response.ExceptionOccured = true;
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
                                    HandleDupCorpRecordProcessor handleDuplicatesProcessor = new HandleDupCorpRecordProcessor();
                                    UDOHandleDupCorpRecordResponse handleDuplicateResponse = (UDOHandleDupCorpRecordResponse)handleDuplicatesProcessor.Execute(thisNewResponse, request, response.Person[0]);

                                    response.Person = handleDuplicateResponse.Person;
                                    response.Message = handleDuplicateResponse.ExceptionOccurred ? handleDuplicateResponse.UDOMessage : handleDuplicateResponse.CORPDbMessage;

                                    response.ExceptionOccured = handleDuplicateResponse.ExceptionOccurred;

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
                            if (String.IsNullOrEmpty(person.ParticipantId) || person.ParticipantId == "0")
                                person.ParticipantId = thisNewResponse.ParticipantId;
                            if (String.IsNullOrEmpty(person.FileNumber))
                                person.FileNumber = thisNewResponse.FileNumber;

                            if (String.IsNullOrEmpty(person.crme_stationofJurisdiction))
                                person.crme_stationofJurisdiction = thisNewResponse.crme_stationofJurisdiction;
                            if (String.IsNullOrEmpty(person.DeceasedDate))
                                person.DeceasedDate = thisNewResponse.crme_DateofDeath;
                            if (String.IsNullOrEmpty(person.crme_CharacterofDishcarge))
                                person.crme_CharacterofDishcarge = thisNewResponse.crme_CharacterofDishcarge;


                            if (person.SSId == null)
                            {
                                if (thisNewResponse.SSId != null)
                                    person.SSId = thisNewResponse.SSId;
                            }

                            Guid? crmPersonId = CommonFunctions.TryGetCrmPerson(OrgServiceProxy, ref request, "MVI");
                            //The UDOSelectedPersonResponse does a TryGetCrmPerson, so no need to do this again
                            //var crmPersonId = thisNewResponse.contactId;
                            owningTeamId = thisNewResponse.OwningTeamId;
                            if (crmPersonId == Guid.Empty)
                            {
                                crmPersonId = CommonFunctions.TryCreateNewCrmPerson(OrgServiceProxy, ref request, "MVI");
                                owningTeamId = request.OwningTeamId;
                            }

                            if (String.IsNullOrEmpty(person.ParticipantId) || person.ParticipantId == "0")
                                person.ParticipantId = request.participantID.ToString();

                            if (!String.IsNullOrEmpty(person.EdiPi))
                            {
                                // This only runs if EDIPI is present...
                                Rank.GetPersonsRank(request.Debug, request.LogSoap, request.LogTiming, request.OrganizationName, request.UserId, person);
                            }

                            response.contactId = crmPersonId.Value;

                            // NP: REM: Changed to read Crm Base Url from LOB Web Config
                            string crmBaseUrl = ConnectionCache.ConnectManager.BaseUrl;
                            response.URL = $@"{crmBaseUrl}/main.aspx?etn=contact&pagetype=entityrecord&id=%7B{crmPersonId.Value}%7D";

                            response.Person[0] = person;

                            //only create IDProof records if we have an interactionID
                            if (!string.IsNullOrEmpty(req.interactionId))
                            {
                                Entity newIDProof = new Entity();
                                newIDProof.LogicalName = "udo_idproof";
                                newIDProof["ownerid"] = new EntityReference("team", owningTeamId);
                                newIDProof["udo_birthdate"] = person.BirthDate;
                                newIDProof["udo_phonenumber"] = person.PhoneNumber;
                                newIDProof["udo_firstname"] = request.FirstName;
                                newIDProof["udo_lastname"] = request.FamilyName;
                                newIDProof["udo_ssn"] = request.SSId.ToUnsecureString();
                                newIDProof["udo_veteran"] = new EntityReference("contact", thisNewResponse.contactId);
                                newIDProof["udo_interaction"] = new EntityReference("udo_interaction", Guid.Parse(req.interactionId));
                                newIDProof["udo_branchofservice"] = person.BranchOfService;
                                newIDProof["udo_rank"] = person.Rank;
                                newIDProof["udo_title"] =  request.FamilyName + ", " + request.FirstName;

                                Guid idProofId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(request.MessageId, newIDProof, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));

                                response.idProofId = idProofId;
                            }
                            #endregion Handle MVI Record Source
                        }
                        else
                        {
                            #region Handle Corp RecordSource
                            progressString = method + " - request.RecordSource - CORP";

                            string MVIICN = string.Empty;
                            string MVIEDIPI = string.Empty;

                            //Go Back to corp to get identifiers as these are not provide in this roundtrip scenario
                            PatientPerson updatedPerson = getVeteranIdentifiers(response.Person[0], request);

                            if (updatedPerson != null)
                            {
                                response.Person = new PatientPerson[1];
                                response.Person[0] = updatedPerson;
                            }

                            ///TODO: Implement Sensitivity Level Check. Need to add userSL to CombinedSelectedPersonRequest object
                            if (response.Person[0].VeteranSensitivityLevel != null)
                            {
                                int realSL = 0;
                                if (Int32.TryParse(response.Person[0].VeteranSensitivityLevel, out realSL))
                                {
                                    if (req.userSL < realSL)
                                    {
                                        response.MVIMessage = string.Format("In UDO your user record is set to a lower Sensitivity Level than CSS. To service this veteran you must have an SL of {0} or higher. Please contact your Administrator to have your level changed.  In the meanwhile, you will have to transfer this call. ", realSL);
                                        response.Person = null;
                                        response.ExceptionOccured = true;
                                        return response;
                                    }
                                }
                            }

                            Guid? crmPersonId = CommonFunctions.TryGetCrmPerson(OrgServiceProxy, ref request, "CorpdDB");
                            owningTeamId = request.OwningTeamId;

                            if (crmPersonId == Guid.Empty)
                            {
                                crmPersonId = CommonFunctions.TryCreateNewCrmPerson(OrgServiceProxy, ref request, "CorpdDB");
                                owningTeamId = request.OwningTeamId;
                            }

                            response.contactId = crmPersonId.Value;

                            // NP: REM: Changed to read Crm Base Url from LOB Web Config
                            string crmBaseUrl = ConnectionCache.ConnectManager.BaseUrl;
                            response.URL = $@"{crmBaseUrl}/main.aspx?etn=contact&pagetype=entityrecord&id=%7B{crmPersonId.Value}%7D";

                            #region do IDProof stuff
                            //only create IDProof records if we have an interactionID
                            if (!string.IsNullOrEmpty(req.interactionId))
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
                                newIDProof["udo_interaction"] = new EntityReference("udo_interaction", Guid.Parse(req.interactionId));
                                newIDProof["udo_branchofservice"] = response.Person[0].BranchOfService;
                                newIDProof["udo_rank"] = response.Person[0].Rank;
                                newIDProof["udo_title"] =  response.Person[0].NameList[0].FamilyName + ", " + response.Person[0].NameList[0].GivenName;

                                Guid idProofId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(req.MessageId, newIDProof, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));

                                response.idProofId = idProofId;
                            }
                            #endregion
                            ///TODO: Need to update this add person to do same as new approach in Selected Person Processor
                            #region do add person stuff
                            if (!request.noAddPerson)
                            {

                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Starting Add Person Logic", request.Debug);

                                //new code - basically if a person is in Corpdb but not MVI, this will do an async add of that person
                                UDOAddPersonRequest addPersonRequest = new UDOAddPersonRequest();
                                addPersonRequest.ContactId = response.contactId;
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
                                addPersonRequest.BirthDate = request.DateofBirth;
                                addPersonRequest.FamilyName = request.FamilyName;
                                addPersonRequest.FirstName = request.FirstName;

                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Sending Stuff to AddPerson", request.Debug);

                                // REM: Invoke LOB Web Api for addPersonRequest
                                WebApiUtility.SendAsync(addPersonRequest, WebApiType.LOB);

                            }
                            else
                            {
                                LogHelper.LogDebug(req.MessageId, req.OrganizationName, req.UserId, method, "Not Adding Person due to Setting", req.Debug);
                            }

                            #endregion

                            #endregion Handle CORP RecordSource
                        }
                        //only create IDProof records if we have an interactionID
                        if (!string.IsNullOrEmpty(req.interactionId))
                        {
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
                            Guid SnapShotId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(req.MessageId, newSnapShot, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));

                            UDOcreateUDOVeteranSnapShotRequest UDOcreateUDOVeteranSnapShotRequest = new UDOcreateUDOVeteranSnapShotRequest
                            {
                                MessageId = req.MessageId,
                                Debug = request.Debug,
                                LogSoap = request.LogSoap,
                                LogTiming = request.LogTiming,
                                PID = response.Person[0].ParticipantId,
                                fileNumber = response.Person[0].FileNumber,
                                UserId = request.UserId,
                                OrganizationName = request.OrganizationName,
                                udo_veteranid = response.contactId,
                                udo_veteransnapshotid = SnapShotId,

                                LegacyServiceHeaderInfo = new UDOHeaderInfo
                                {
                                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                }
                            };

                            //REM: TODO: Invoke LOB Web Api for UDOcreateUDOVeteranSnapShotRequest
                            WebApiUtility.SendAsync(UDOcreateUDOVeteranSnapShotRequest, WebApiType.LOB);

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
                            openMe.MessageId = req.MessageId;
                            openMe.DiagnosticsContext = req.DiagnosticsContext;
                            WebApiUtility.SendAsync(openMe, WebApiType.LOB);
                        }
                        #endregion
                    }
                }
                if (response.Person.Count() > 0)
                {
                    if (response.Person[0].SSId != null)
                    {
                        response.Person[0].SSIdString = SecurityTools.ConvertToUnsecureString(response.Person[0].SSId);

                        //REM: TODO: Need to check this call to getEBenefitsData is required or not
                        // Kirk call here
                        if (req.Edipi != null)
                        {
                            UDOSelectedPersonProcessor selectedPersonLogic = new UDOSelectedPersonProcessor();
                            UDOSelectedPersonResponse thisNewResponse = (UDOSelectedPersonResponse)selectedPersonLogic.Execute(request);
                        }
                    }
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId,
                    method + " \r\nProgess:" + progressString, ExecutionException);
                if (response == null)
                {
                    response = new UDOCombinedSelectedPersonResponse();
                }
                response.ExceptionOccured = true;
                response.Message = ExecutionException.Message;
                return response;
            }
        }

        private PatientPerson getVeteranIdentifiers(PatientPerson person, UDOSelectedPersonRequest selectedPersonRequest)
        {
            string progressString = method + " - Getting Veteran Identifiers";
            try
            {
                LogHelper.LogDebug(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, "getVeteranIdentifiers", string.Format("Getting Veteran Identifiers fro PID {0} after select duplicate call.", person.ParticipantId), selectedPersonRequest.Debug);
                UDOgetVeteranIdentifiersRequest getVeteranIdentifiers = new UDOgetVeteranIdentifiersRequest
                {
                    MessageId = selectedPersonRequest.MessageId,
                    LogTiming = selectedPersonRequest.LogTiming,
                    LogSoap = selectedPersonRequest.LogSoap,
                    Debug = selectedPersonRequest.Debug,
                    RelatedParentEntityName = selectedPersonRequest.RelatedParentEntityName,
                    RelatedParentFieldName = selectedPersonRequest.RelatedParentFieldName,
                    RelatedParentId = selectedPersonRequest.RelatedParentId,
                    UserId = selectedPersonRequest.UserId,
                    OrganizationName = selectedPersonRequest.OrganizationName,
                    LegacyServiceHeaderInfo = new UDOHeaderInfo
                    {
                        ApplicationName = selectedPersonRequest.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = selectedPersonRequest.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = selectedPersonRequest.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = selectedPersonRequest.LegacyServiceHeaderInfo.StationNumber
                    }
                };
                UDOgetVeteranIdentifiersResponse getVeteranIdentifiersResponse = new UDOgetVeteranIdentifiersResponse();

                long convertedPid;
                Int64.TryParse(person.ParticipantId, out convertedPid);

                getVeteranIdentifiers.ptcpntId = convertedPid;

                UDOgetVeteranIdentifiersProcessor udoGetVeteranIdentifiersLogic = new UDOgetVeteranIdentifiersProcessor();
                getVeteranIdentifiersResponse = (UDOgetVeteranIdentifiersResponse)udoGetVeteranIdentifiersLogic.Execute(getVeteranIdentifiers);

                if (getVeteranIdentifiersResponse != null)
                {
                    LogHelper.LogDebug(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, "getVeteranIdentifiers", string.Format("Identifiers found, mapping data.", person.ParticipantId), selectedPersonRequest.Debug);
                    if (getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo != null && getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_ParticipantID != null)
                    {
                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SensitivityLevel))
                        {
                            if (string.IsNullOrEmpty(person.VeteranSensitivityLevel))
                                person.VeteranSensitivityLevel = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SensitivityLevel;

                            selectedPersonRequest.VeteranSensitivityLevel = Convert.ToInt32(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SensitivityLevel);
                        }
                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SSN))
                        {
                            person.SSId = SecurityTools.ConvertToSecureString(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SSN);
                        }
                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_FileNumber))
                        {
                            if (string.IsNullOrEmpty(selectedPersonRequest.FileNumber))
                                selectedPersonRequest.FileNumber = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_FileNumber;

                            person.FileNumber = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_FileNumber;
                        }
                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_Gender))
                        {
                            if (string.IsNullOrEmpty(selectedPersonRequest.Gender))
                                selectedPersonRequest.Gender = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_Gender;

                            person.GenderCode = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_Gender;
                        }
                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_BranchOfService))
                        {
                            if (string.IsNullOrEmpty(selectedPersonRequest.BranchOfService))
                                selectedPersonRequest.BranchOfService = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_BranchOfService;

                            person.BranchOfService = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_BranchOfService;
                        }
                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SOJ))
                        {
                            person.crme_stationofJurisdiction = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SOJ;
                        }
                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_DateofDeath))
                        {
                            person.DeceasedDate = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_DateofDeath;
                        }
                        if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_characterofdischarge))
                        {
                            person.crme_CharacterofDishcarge = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_characterofdischarge;
                        }
                    }
                }
                return person;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, selectedPersonRequest.RelatedParentId, selectedPersonRequest.RelatedParentEntityName, 
                    selectedPersonRequest.RelatedParentFieldName, selectedPersonRequest.MessageId, method + "\r\n Progess:" + progressString, ex);
                LogHelper.LogError(selectedPersonRequest.MessageId, selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, method, ex);
                return null;
            }
        }
    }
}
