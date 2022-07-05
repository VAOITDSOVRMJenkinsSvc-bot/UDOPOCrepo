using System.Diagnostics;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Common;
using UDO.LOB.MVI.Messages;

namespace UDO.LOB.MVI.Processors
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    public class SecondarySearchProcessor
    {
        private const string CorpDbSearchResultsWithMatch = "A search in CORPDB found {0} matching record(s).";
        private const string CorpDbSearchResultsWithNoMatch = "A search in CORPDB did not find any records matching the search criteria.";

        private const string MVISearchResultsWithNoMatch = "A search in MVI did not find any records matching the search criteria.";

        public IMessageBase Execute(UDOPersonSearchRequest request)
        {
            UDOPersonSearchResponse response = new UDOPersonSearchResponse();
            Stopwatch txnTimer = Stopwatch.StartNew();
            Stopwatch EntireTimer = Stopwatch.StartNew();
            if (request == null)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, $"{GetType().FullName}.Execute",
                        $"<< Exit from {GetType().FullName}. Recieved a null {request.GetType().Name} message request or request.Person.");
            }

            //use the EC to find the veteran by filenumber if
            UDOfindVeteranInfoRequest findVeteranRequest = new UDOfindVeteranInfoRequest
            {
                MessageId = request.MessageId,
                LogTiming = request.LogTiming,
                LogSoap = request.LogSoap,
                Debug = request.Debug,
                RelatedParentEntityName = request.RelatedParentEntityName,
                RelatedParentFieldName = request.RelatedParentFieldName,
                RelatedParentId = request.RelatedParentId,
                UserId = request.UserId,
                OrganizationName = request.OrganizationName,
                LegacyServiceHeaderInfo = new UDOHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                }
            };
            UDOfindVeteranInfoResponse findVeteranResponse = new UDOfindVeteranInfoResponse();

            if (request.SSIdString != null && request.SSIdString.Length == 9)
            {
                UDOfindVeteranInfoProcessor findVeteranLogic = new UDOfindVeteranInfoProcessor();

                findVeteranRequest.SocialSN = request.SSIdString;

                findVeteranResponse = (UDOfindVeteranInfoResponse)findVeteranLogic.Execute(findVeteranRequest);
                txnTimer.Stop();
                txnTimer.Restart();
                //if we can't find it as a soc, maybe it is a file number
                if (findVeteranResponse.UDOfindVeteranInfoInfo == null)
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
                UDOfindVeteranInfoProcessor findVeteranLogic = new UDOfindVeteranInfoProcessor();
                findVeteranResponse = (UDOfindVeteranInfoResponse)findVeteranLogic.Execute(findVeteranRequest);
                txnTimer.Stop();
                txnTimer.Restart();
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
        private static UDOPersonSearchResponse MapCORPDbtoResponse(UDOfindVeteranInfoResponse lobResponse, UDOPersonSearchRequest request)
        {
            if (lobResponse.UDOfindVeteranInfoInfo != null)
            {
                UDOPersonSearchResponse response = new UDOPersonSearchResponse();
                #region process veteran retgurn
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
                    UDOPersonSearchResponse response = new UDOPersonSearchResponse();
                    if (lobResponse.ExceptionMessage.Contains("access violation"))
                    {
                        response.ExceptionOccured = true;
                        response.CORPDbMessage = lobResponse.ExceptionMessage;
                        LogHelper.LogError(request.OrganizationName, request.UserId, "SecondarySearchProcessor - exception in findVeteran :: access violation", lobResponse.ExceptionMessage);
                    }
                    else
                    {

                        LogHelper.LogError(request.OrganizationName, request.UserId, "SecondarySearchProcessor - exception in findVeteran", lobResponse.ExceptionMessage);
                    }
                    return response;
                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "SecondarySearchProcessor", "No record found in CorpDB");
                }
                return null;
            }
        }
    }
}