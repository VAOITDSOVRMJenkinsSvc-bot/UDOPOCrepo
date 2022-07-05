using Microsoft.Pfe.Xrm;
using System.Diagnostics;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Common;
using UDO.LOB.MVI.Messages;

#region Legacy Usings
//using IMessageBase = VRM.Integration.Servicebus.Core.IMessageBase;
//using Logger = VRM.Integration.Servicebus.Core.Logger;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//using VRM.Integration.UDO.MVI.Messages;
//using VRM.Integration.UDO.MVI.Processors;
//using MVIMessages = VRM.Integration.Mvi.PersonSearch.Messages;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common;
//using VRM.Integration.UDO.eMIS.Messages;
//using VRM.Integration.UDO.MVI.Common;
#endregion


namespace UDO.LOB.MVI.Processors
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    public class CombinedSecondarySearchProcessor
    {
        //do not change these messages -- these are passed to the Search Page HTML\JS for proper messaging
        private const string CorpDbSearchResultsWithMatch = "A search in CORPDB found {0} matching record(s).";
        private const string CorpDbSearchResultsWithNoMatch = "A search in CORPDB did not find any records matching the search criteria.";
        private const string CorpDbSearchIsDown = "A connection error was encountered during the CORPDB search.";
        private const string CorpDbSearchUnknownError = "An unknown error was encountered during the CORPDB search.";
        private const string CorpDbSearchBackendServicesDown = "Communication to the back end services is currently down. Please try again.";
        private const string MVISearchResultsWithNoMatch = "A search in MVI did not find any records matching the search criteria.";

        public IMessageBase Execute(UDOCHATPersonSearchRequest request)
        {
            var response = new UDOCombinedPersonSearchResponse();
            Stopwatch txnTimer = Stopwatch.StartNew();
            Stopwatch EntireTimer = Stopwatch.StartNew();
            if (request == null)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, $"{GetType().FullName}.Execute", 
                    $"<< Exit from {GetType().FullName}. Recieved a null {request.GetType().Name} message request or request.Person.");
                response.RawMviExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
                return response;
            }

            var findVeteranRequest = new UDOfindVeteranInfoRequest();
            findVeteranRequest.MessageId = request.MessageId;
            findVeteranRequest.LogTiming = request.LogTiming;
            findVeteranRequest.LogSoap = request.LogSoap;
            findVeteranRequest.Debug = request.Debug;
            findVeteranRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findVeteranRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findVeteranRequest.RelatedParentId = request.RelatedParentId;
            findVeteranRequest.UserId = request.UserId;
            findVeteranRequest.OrganizationName = request.OrganizationName;
            findVeteranRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
            {
                ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                LoginName = request.LegacyServiceHeaderInfo.LoginName,
                StationNumber = request.LegacyServiceHeaderInfo.StationNumber
            };
            var findVeteranResponse = new UDOfindVeteranInfoResponse();


            if (request.SSIdString != null)
            {
                var findVeteranLogic = new UDOfindVeteranInfoProcessor();

                if (request.SSIdString.Length == 9)
                {

                    findVeteranRequest.SocialSN = request.SSIdString;

                    findVeteranResponse = (UDOfindVeteranInfoResponse)findVeteranLogic.Execute(findVeteranRequest);

                    if (findVeteranResponse != null)
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "CombinedSecondarySearchProcessor, Find Veteran By Socaial: ", string.Format("Exception Occurred: {0}", findVeteranResponse.ExceptionOccured));

                    txnTimer.Stop();
                    txnTimer.Restart();
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "CombinedSecondarySearchProcessor", "First Search");
                    //if we can't find it as a soc, maybe it is a file number
                    if (findVeteranResponse != null && findVeteranResponse.ExceptionOccured == false && findVeteranResponse.UDOfindVeteranInfoInfo == null)
                    {
                        findVeteranRequest.SocialSN = null;
                        findVeteranRequest.fileNumber = request.SSIdString;

                        findVeteranResponse = (UDOfindVeteranInfoResponse)findVeteranLogic.Execute(findVeteranRequest);
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "CombinedSecondarySearchProcessor", "2nd Search");
                        txnTimer.Stop();
                        txnTimer.Restart();
                    }
                }
                else
                {
                    //if it isn't long enough, find by filenumber only
                    findVeteranRequest.fileNumber = request.SSIdString;

                    findVeteranResponse = (UDOfindVeteranInfoResponse)findVeteranLogic.Execute(findVeteranRequest);

                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "CombinedSecondarySearchProcessor", "3rd Search");

                    if (findVeteranResponse != null)
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "CombinedSecondarySearchProcessor, Find Veteran By Socaial: ", string.Format("Exception Occurred: {0}", findVeteranResponse.ExceptionOccured));

                    txnTimer.Stop();
                    txnTimer.Restart();
                }
            }
            else
            {
                if (request.SSId !=null && request.SSId.Length > 0)
                {
                    var findVeteranLogic = new UDOfindVeteranInfoProcessor();

                    if (request.SSId.Length == 9)
                    {

                        findVeteranRequest.SocialSN = request.SSId.ToUnsecureString();

                           findVeteranResponse = (UDOfindVeteranInfoResponse)findVeteranLogic.Execute(findVeteranRequest);
                           LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "CombinedSecondarySearchProcessor", "First Search");
                           txnTimer.Stop();
                           txnTimer.Restart();

                        if(findVeteranResponse != null)
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "CombinedSecondarySearchProcessor, Find Veteran By Socaial: ", string.Format("Exception Occurred: {0}", findVeteranResponse.ExceptionOccured));

                        //if we can't find it as a soc, maybe it is a file number
                        if (findVeteranResponse != null && findVeteranResponse.ExceptionOccured == false && findVeteranResponse.UDOfindVeteranInfoInfo == null)
                        {
                            findVeteranRequest.SocialSN = null;
                            findVeteranRequest.fileNumber = request.SSId.ToUnsecureString();

                            findVeteranResponse = (UDOfindVeteranInfoResponse)findVeteranLogic.Execute(findVeteranRequest);
                            txnTimer.Stop();
                            txnTimer.Restart();
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "CombinedSecondarySearchProcessor", "2nd Search");
                        }
                    }
                    else
                    {
                        //if it isn't long enough, find by filenumber only
                        findVeteranRequest.fileNumber = request.SSId.ToUnsecureString();

                        findVeteranResponse = (UDOfindVeteranInfoResponse)findVeteranLogic.Execute(findVeteranRequest);

                        if (findVeteranResponse != null)
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "CombinedSecondarySearchProcessor, Find Veteran By FileNumber: ", string.Format("Exception Occurred: {0}", findVeteranResponse.ExceptionOccured));

                        txnTimer.Stop();
                        txnTimer.Restart();
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "CombinedSecondarySearchProcessor", "3rd Search");
                    }
                }
            }

            response = MapCORPDbtoResponse(findVeteranResponse, request);
            EntireTimer.Stop();

            return response;
        }
        public IMessageBase Execute(UDOCombinedPersonSearchRequest request)
        {
            LogHelper.LogInfo("In CombinedSecondarySearchProcessor");
            var response = new UDOCombinedPersonSearchResponse();
            Stopwatch txnTimer = Stopwatch.StartNew();
            Stopwatch EntireTimer = Stopwatch.StartNew();
            if (request == null)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, $"{GetType().FullName}.Execute",
                    $"<< Exit from {GetType().FullName}. Recieved a null {request.GetType().Name} message request or request.Person.");

            }

            //use the EC to find the veteran by filenumber if
            var findVeteranRequest = new UDOfindVeteranInfoRequest();
            findVeteranRequest.MessageId = request.MessageId;
            findVeteranRequest.LogTiming = request.LogTiming;
            findVeteranRequest.LogSoap = request.LogSoap;
            findVeteranRequest.Debug = request.Debug;
            findVeteranRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findVeteranRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findVeteranRequest.RelatedParentId = request.RelatedParentId;
            findVeteranRequest.UserId = request.UserId;
            findVeteranRequest.OrganizationName = request.OrganizationName;
            findVeteranRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
            {
                ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                LoginName = request.LegacyServiceHeaderInfo.LoginName,
                StationNumber = request.LegacyServiceHeaderInfo.StationNumber
            };
            var findVeteranResponse  = new UDOfindVeteranInfoResponse();


            if (request.SSIdString != null)
            {
                var findVeteranLogic = new UDOfindVeteranInfoProcessor();
                if (request.SSIdString.Length == 9)
                {

                    findVeteranRequest.SocialSN = request.SSIdString;

                    findVeteranResponse = (UDOfindVeteranInfoResponse)findVeteranLogic.Execute(findVeteranRequest);
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "CombinedSecondarySearchProcessor", "First Search");
                    //if we can't find it as a soc, maybe it is a file number
                    if (findVeteranResponse != null && findVeteranResponse.ExceptionOccured == false && findVeteranResponse.UDOfindVeteranInfoInfo == null)
                    {
                        findVeteranRequest.SocialSN = null;
                        findVeteranRequest.fileNumber = request.SSIdString;

                        findVeteranResponse = (UDOfindVeteranInfoResponse)findVeteranLogic.Execute(findVeteranRequest);
                        txnTimer.Stop();
                        txnTimer.Restart();
                    }
                }
                else
                {
                    //if it isn't long enough, find by filenumber only
                    findVeteranRequest.fileNumber = request.SSIdString;

                    findVeteranResponse = (UDOfindVeteranInfoResponse)findVeteranLogic.Execute(findVeteranRequest);
                    txnTimer.Stop();
                    txnTimer.Restart();
                }
            }

            response = MapCORPDbtoResponse(findVeteranResponse, request);
            EntireTimer.Stop();

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lobResponse"></param>
        /// <param name="BranchOfService"></param>
        /// <returns></returns>
        private static UDOCombinedPersonSearchResponse MapCORPDbtoResponse(UDOfindVeteranInfoResponse lobResponse, UDOCombinedPersonSearchRequest request)
        {
            if (lobResponse.UDOfindVeteranInfoInfo != null)
            {
                var response = new UDOCombinedPersonSearchResponse();
                response.VEISFindVeteranResponse = lobResponse.UDOfindVeteranInfoInfo.VEISResponse;
                #region process veteran return
                // GetVeteranInfo returns a record with blank values
                if (string.IsNullOrEmpty(lobResponse.UDOfindVeteranInfoInfo.crme_SSN))
                {
                    response.CORPDbMessage = CorpDbSearchResultsWithNoMatch;
                    return response;
                }
                response.Person = new PatientPerson[1];

                response.Person[0] = new PatientPerson
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
                        new Name
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
                    Address = new PatientAddress
                    {
                        City = lobResponse.UDOfindVeteranInfoInfo.crme_City,
                        State = lobResponse.UDOfindVeteranInfoInfo.crme_State,
                        PostalCode = lobResponse.UDOfindVeteranInfoInfo.crme_Zip,
                        StreetAddressLine = lobResponse.UDOfindVeteranInfoInfo.crme_Address1
                    },
                    crme_CauseOfDeath = lobResponse.UDOfindVeteranInfoInfo.crme_CauseOfDeath,
                    crme_stationofJurisdiction = lobResponse.UDOfindVeteranInfoInfo.crme_stationofJurisdiction,
                    crme_CharacterofDishcarge = lobResponse.UDOfindVeteranInfoInfo.crme_CharacterofDishcarge,
                    DeceasedDate = lobResponse.UDOfindVeteranInfoInfo.crme_DeceasedDate,
                };

                Rank.GetPersonsRank(request.Debug, request.LogSoap, request.LogTiming, request.OrganizationName, request.UserId, response.Person[0]);

                #endregion
                if (response.Person[0].SSId == null)
                {
                    response.Person[0].SSId = response.Person[0].SSIdString.ToSecureString();
                }

                response.MVIMessage = MVISearchResultsWithNoMatch;

                response.CORPDbMessage = string.Format(CorpDbSearchResultsWithMatch, response.Person.Length);
                response.CORPDbRecordCount = response.Person.Length;
                return response;
            }
            else
            {
                if (lobResponse.ExceptionOccured)
                {
                    var response = new UDOCombinedPersonSearchResponse();
                    if (lobResponse.ExceptionMessage.Contains("access violation") || lobResponse.ExceptionMessage == "sensitive record check error")
                    {
                        response.ExceptionOccurred = true;
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = lobResponse.ExceptionMessage;
                    }
                    else if (lobResponse.ExceptionMessage == "BIRLS communication is down" || lobResponse.ExceptionMessage == "The Tuxedo service is down")
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, "CombinedSecondarySearchProcessor - exception in findVeteran", lobResponse.ExceptionMessage);
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = lobResponse.ExceptionMessage;
                        response.ExceptionOccurred = true;
                    }
                    else if (lobResponse.ExceptionMessage.Contains("there was no endpoint listening"))
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, "CombinedSecondarySearchProcessor - exception in findVeteran", lobResponse.ExceptionMessage);
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = CorpDbSearchBackendServicesDown;
                        response.ExceptionOccurred = true;
                    }
                    else
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, "CombinedSecondarySearchProcessor - exception in findVeteran", lobResponse.ExceptionMessage);
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = CorpDbSearchUnknownError;
                        response.ExceptionOccurred = true;
                    }
                    return response;
                }
                return null;
            }
        }

        
        private static UDOCombinedPersonSearchResponse MapCORPDbtoResponse(UDOfindVeteranInfoResponse lobResponse, UDOCHATPersonSearchRequest request)
        {
            if (lobResponse.UDOfindVeteranInfoInfo != null)
            {
                var response = new UDOCombinedPersonSearchResponse();
                response.VEISFindVeteranResponse = lobResponse.UDOfindVeteranInfoInfo.VEISResponse;
                #region process veteran return
                // GetVeteranInfo returns a record with blank values
                if (string.IsNullOrEmpty(lobResponse.UDOfindVeteranInfoInfo.crme_SSN))
                {
                    response.CORPDbMessage = CorpDbSearchResultsWithNoMatch;
                    return response;
                }
                response.Person = new PatientPerson[1];

                response.Person[0] = new PatientPerson
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
                        new Name
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
                    Address = new PatientAddress
                    {
                        City = lobResponse.UDOfindVeteranInfoInfo.crme_City,
                        State = lobResponse.UDOfindVeteranInfoInfo.crme_State,
                        PostalCode = lobResponse.UDOfindVeteranInfoInfo.crme_Zip,
                        StreetAddressLine = lobResponse.UDOfindVeteranInfoInfo.crme_Address1
                    },
                    crme_CauseOfDeath = lobResponse.UDOfindVeteranInfoInfo.crme_CauseOfDeath,
                    crme_stationofJurisdiction = lobResponse.UDOfindVeteranInfoInfo.crme_stationofJurisdiction,
                    crme_CharacterofDishcarge = lobResponse.UDOfindVeteranInfoInfo.crme_CharacterofDishcarge,
                    DeceasedDate = lobResponse.UDOfindVeteranInfoInfo.crme_DeceasedDate,
                };

                Rank.GetPersonsRank(request.Debug, request.LogSoap, request.LogTiming, request.OrganizationName, request.UserId, response.Person[0]);
                #endregion

                if (response.Person[0].SSId == null)
                {
                    response.Person[0].SSId = response.Person[0].SSIdString.ToSecureString();
                }
                response.MVIMessage = MVISearchResultsWithNoMatch;

                response.CORPDbMessage = string.Format(CorpDbSearchResultsWithMatch, response.Person.Length);
                response.CORPDbRecordCount = response.Person.Length;
                return response;
            }
            else
            {
                if (lobResponse.ExceptionOccured)
                {
                    var response = new UDOCombinedPersonSearchResponse();
                    if (lobResponse.ExceptionMessage.Contains("access violation") || lobResponse.ExceptionMessage == "sensitive record check error")
                    {
                        response.ExceptionOccurred = true;
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = lobResponse.ExceptionMessage;
                    }
                    else if (lobResponse.ExceptionMessage == "BIRLS communication is down" || lobResponse.ExceptionMessage == "The Tuxedo service is down")
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, "CombinedSecondarySearchProcessor - exception in findVeteran", lobResponse.ExceptionMessage);
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = lobResponse.ExceptionMessage;
                        response.ExceptionOccurred = true;
                    }
                    else if (lobResponse.ExceptionMessage.Contains("there was no endpoint listening"))
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, "CombinedSecondarySearchProcessor - exception in findVeteran", lobResponse.ExceptionMessage);
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = CorpDbSearchBackendServicesDown;
                        response.ExceptionOccurred = true;
                    }
                    else
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, "CombinedSecondarySearchProcessor - exception in findVeteran", lobResponse.ExceptionMessage);
                        //do not change the exception message -- these are passed to the Search Page HTML\JS for proper messaging
                        response.CORPDbMessage = CorpDbSearchUnknownError;
                        response.ExceptionOccurred = true;
                    }
                    return response;
                }

                return null;
            }
        }
    }
}