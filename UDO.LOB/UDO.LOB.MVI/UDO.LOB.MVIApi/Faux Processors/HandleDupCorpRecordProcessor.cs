using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Common;
using UDO.LOB.MVI.Messages;

namespace UDO.LOB.MVI.Processors
{
    public class HandleDupCorpRecordProcessor
    {
        public IMessageBase Execute(UDOSelectedPersonResponse selectedPersonResponse, UDOSelectedPersonRequest selectedPersonRequest, PatientPerson originalPerson)
        {
            LogHelper.LogDebug(selectedPersonRequest.OrganizationName, selectedPersonRequest.Debug, selectedPersonRequest.UserId, "HandleDupCorpRecordProcessor, Execute", "Entering exeucte for Handling Duplicate Corp records identified by MVI.");
            var handleDupResponse = new UDOHandleDupCorpRecordResponse();

            try
            {
                var corpPidList = selectedPersonResponse.CorrespondingIdsResponse.CorrespondingIdList.Where(
                            (v => v.AssigningAuthority.Equals("USVBA", StringComparison.InvariantCultureIgnoreCase) &&
                                  v.AssigningFacility.Equals("200CORP", StringComparison.InvariantCultureIgnoreCase) &&
                                  v.IdentifierType.Equals("PI", StringComparison.InvariantCultureIgnoreCase)));

                LogHelper.LogDebug(selectedPersonRequest.OrganizationName, selectedPersonRequest.Debug, selectedPersonRequest.UserId, "HandleDupCorpRecordProcessor, Execute", string.Format("Number of PIDs returned from MVI: {0}", corpPidList.Count()));

                var newPersonList = new List<PatientPerson>();

                foreach (var pid in corpPidList)
                {
                    Stopwatch txnTimer = Stopwatch.StartNew();
                    Stopwatch EntireTimer = Stopwatch.StartNew();

                    var findVeteranByPidRequest = new UDOfindVeteranInfoByPidRequest();
                    findVeteranByPidRequest.LogTiming = selectedPersonRequest.LogTiming;
                    findVeteranByPidRequest.LogSoap = selectedPersonRequest.LogSoap;
                    findVeteranByPidRequest.Debug = selectedPersonRequest.Debug;
                    findVeteranByPidRequest.RelatedParentEntityName = selectedPersonRequest.RelatedParentEntityName;
                    findVeteranByPidRequest.RelatedParentFieldName = selectedPersonRequest.RelatedParentFieldName;
                    findVeteranByPidRequest.RelatedParentId = selectedPersonRequest.RelatedParentId;
                    findVeteranByPidRequest.UserId = selectedPersonRequest.UserId;
                    findVeteranByPidRequest.OrganizationName = selectedPersonRequest.OrganizationName;
                    findVeteranByPidRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
                    {
                        ApplicationName = selectedPersonRequest.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = selectedPersonRequest.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = selectedPersonRequest.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = selectedPersonRequest.LegacyServiceHeaderInfo.StationNumber
                    };
                    findVeteranByPidRequest.ParticipantID = pid.PatientIdentifier;

                    var findVetarnLogic = new UDOfindVeteranInfoProcessor();

                    LogHelper.LogDebug(selectedPersonRequest.OrganizationName, selectedPersonRequest.Debug, selectedPersonRequest.UserId, "HandleDupCorpRecordProcessor.Execute", 
                        string.Format("Executing UDOfindVeteranInfoProcessor for PID: {0}", pid.PatientIdentifier));
                    var findVeteranByPidResponse = (UDOfindVeteranInfoByPidResponse)findVetarnLogic.Execute(findVeteranByPidRequest);

                    var newPerson = MapCORPDBtoResponse(findVeteranByPidResponse, selectedPersonRequest, originalPerson);
                    EntireTimer.Stop();
                    LogHelper.LogDebug(selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, selectedPersonRequest.Debug, selectedPersonRequest.RelatedParentId, selectedPersonRequest.RelatedParentEntityName, 
                        selectedPersonRequest.RelatedParentFieldName, "HandleDupCorpRecordProcessor.Execute", $"Find Veteran by PTCPT ID Time: {EntireTimer.ElapsedMilliseconds} ms.", false);

                    var getVeteranIdentifiers = new UDOgetVeteranIdentifiersRequest();
                    getVeteranIdentifiers.MessageId = Guid.NewGuid().ToString();
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
                    var getVeteranIdentifiersResponse = new UDOgetVeteranIdentifiersResponse();

                    getVeteranIdentifiers.ptcpntId = Convert.ToInt64(pid.PatientIdentifier);

                    selectedPersonRequest.participantID = getVeteranIdentifiers.ptcpntId;

                    var udoGetVeteranIdentifiersLogic = new UDOgetVeteranIdentifiersProcessor();
                    getVeteranIdentifiersResponse = (UDOgetVeteranIdentifiersResponse)udoGetVeteranIdentifiersLogic.Execute(getVeteranIdentifiers);

                    txnTimer.Stop();
                    txnTimer.Restart();

                    var newPersonCorrespondingIds = new List<CorrespondingIDs>();
                    if (selectedPersonResponse.CorrespondingIdsResponse.CorrespondingIdList != null)
                    {
                        foreach (var corId in selectedPersonResponse.CorrespondingIdsResponse.CorrespondingIdList)
                        {
                            newPersonCorrespondingIds.Add(new CorrespondingIDs{

                                AssigningAuthority = corId.AssigningAuthority,
                                AssigningFacility = corId.AssigningFacility,
                                AuthorityOid = corId.AuthorityOid,
                                IdentifierType = corId.IdentifierType,
                                OrganizationName = corId.OrganizationName,
                                PatientIdentifier = corId.PatientIdentifier,
                                RawValueFromMvi = corId.RawValueFromMvi,
                                UserFirstName = corId.UserFirstName,
                                UserLastName = corId.UserLastName,
                                UserId = corId.UserId
                            });
                        }
                    }

                    if (newPerson != null)
                    {
                        newPerson.CorrespondingIdList = newPersonCorrespondingIds.ToArray();
                        if (getVeteranIdentifiersResponse != null)
                            newPerson = MapCORPDBVeteranIdentifiersToResponse(getVeteranIdentifiersResponse, newPerson);

                        LogHelper.LogDebug(selectedPersonRequest.OrganizationName, selectedPersonRequest.Debug, selectedPersonRequest.UserId, "HandleDupCorpRecordProcessor, Execute", "Adding to new person to list");
                        newPersonList.Add(newPerson);
                    }
                }

                handleDupResponse.Person = newPersonList.ToArray();
                handleDupResponse.CORPDbMessage = "Multiple matches found for veteran in CorpDB based on MVI data";
                return handleDupResponse;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, "HandleDupCorpRecordProcessor.Execute with pipeline", "Exception:" + ex.Message);
                handleDupResponse.UDOMessage = ex.Message;
                handleDupResponse.ExceptionOccurred = true;
                return handleDupResponse;
            }
        }

        private static PatientPerson MapCORPDBVeteranIdentifiersToResponse(UDOgetVeteranIdentifiersResponse getVeteranIdentifiersResponse, PatientPerson person )
        {
            if (getVeteranIdentifiersResponse != null)
            {
                if (getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo != null && getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_ParticipantID != null)
                {
                    if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SSN))
                    {
                        person.SSId = SecurityTools.ConvertToSecureString(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_SSN);
                    }
                    if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_FileNumber))
                    {
                        person.FileNumber = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_FileNumber;
                    }                   
                    if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_Gender))
                    {
                        person.GenderCode = getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_Gender;
                    }
                    if (!string.IsNullOrEmpty(getVeteranIdentifiersResponse.UDOgetVeteranIdentifiersInfo.crme_BranchOfService))
                    {
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

        private static PatientPerson MapCORPDBtoResponse(UDOfindVeteranInfoByPidResponse response, UDOSelectedPersonRequest request, PatientPerson originalPerson)
        {
            if (response.UDOfindVeteranInfoInfo != null)
            {
                // GetVeteranInfo returns a record with blank values
                if (string.IsNullOrEmpty(response.UDOfindVeteranInfoInfo.crme_SSN))
                {
                    return null;
                }
                
                var person = new PatientPerson
                {
                    GenderCode = response.UDOfindVeteranInfoInfo.crme_Gender,
                    Identifier = originalPerson.Identifier,
                    IdentifierType = originalPerson.IdentifierType,
                    IdentifyTheft = originalPerson.IdentifyTheft,
                    IsDeceased = originalPerson.IsDeceased,
                    BranchOfService = response.UDOfindVeteranInfoInfo.crme_BranchOfService,
                    SSIdString = response.UDOfindVeteranInfoInfo.crme_SSN,
                    FileNumber = response.UDOfindVeteranInfoInfo.crme_FileNumber,
                    EdiPi = response.UDOfindVeteranInfoInfo.crme_EDIPI,
                    BirthDate = response.UDOfindVeteranInfoInfo.crme_DOB.Date.ToShortDateString(),
                    RecordSource = "CORPDB",
                    ParticipantId = response.UDOfindVeteranInfoInfo.crme_ParticipantID,
                    VeteranSensitivityLevel = response.UDOfindVeteranInfoInfo.crme_VeteranSensitivityLevel,
                    StatusCode = originalPerson.StatusCode,
                    NameList = new[]
                    {
                        new Name
                        {
                            MiddleName = response.UDOfindVeteranInfoInfo.crme_MiddleName,
                            GivenName = response.UDOfindVeteranInfoInfo.crme_FirstName,
                            FamilyName = response.UDOfindVeteranInfoInfo.crme_LastName,
                            NameType = "Legal",
                            NamePrefix = "",
                            NameSuffix = ""
                        }
                    },

                    PhoneNumber = response.UDOfindVeteranInfoInfo.crme_PrimaryPhone,
                    Address = new PatientAddress
                    {
                        City = response.UDOfindVeteranInfoInfo.crme_City,
                        State = response.UDOfindVeteranInfoInfo.crme_State,
                        PostalCode = response.UDOfindVeteranInfoInfo.crme_Zip,
                        StreetAddressLine = response.UDOfindVeteranInfoInfo.crme_Address1
                    },
                    crme_CauseOfDeath = response.UDOfindVeteranInfoInfo.crme_CauseOfDeath,
                    crme_stationofJurisdiction = response.UDOfindVeteranInfoInfo.crme_stationofJurisdiction,
                    crme_CharacterofDishcarge = response.UDOfindVeteranInfoInfo.crme_CharacterofDishcarge,
                    DeceasedDate = response.UDOfindVeteranInfoInfo.crme_DeceasedDate,
                };

                if (!String.IsNullOrEmpty(request.Edipi) && request.Edipi != "0")
                {
                    person.EdiPi = request.Edipi;
                    Rank.GetPersonsRank(request.Debug, request.LogSoap, request.LogTiming, request.OrganizationName, request.UserId, person);
                }

                if (person.SSId == null)
                {
                    person.SSId = person.SSIdString.ToSecureString();
                }

                return person;
            }
            else
                return null;
        }
    }
}
