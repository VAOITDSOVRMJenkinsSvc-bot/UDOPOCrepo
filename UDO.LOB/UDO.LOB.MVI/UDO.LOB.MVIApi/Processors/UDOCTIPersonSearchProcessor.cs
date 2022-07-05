using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Common;
using UDO.LOB.MVI.Messages;
using MVIMessages = VEIS.Mvi.Messages;

namespace UDO.LOB.MVI.Processors
{
    /// <summary>
    /// UDOCTIPersonSearchProcessor
    /// </summary>
    /// <remarks>
    /// Performs actions for 
    /// </remarks>
    public class UDOCTIPersonSearchProcessor
    {
        private const string MVISearchResultsWithMatch = "A search in MVI found {0} matching record(s). ";
        private const string MVISearchResultsWithNoMatch = "A search in MVI did not find any records matching the search criteria. ";
        private const string MVISearchResultsWithTooManyMatches = "A search in MVI returned more than the allowable number of matches. ";
        private const string MVISearchUnknownError = "An unknown error was returned from MVI. ";
        private const string MVISearchConnectionError = "A connection error was encountered performing the MVI search. ";
        private StringBuilder _logData;
        private StringBuilder _logTimerData;

        // REM: New variables
        private const string method = "SingleReturnProcessor";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IMessageBase Execute(UDOCTIPersonSearchRequest CTIrequest)
        {
            UDOCombinedPersonSearchResponse response = null;
            _logData = new StringBuilder();
            _logTimerData = new StringBuilder();
            if (CTIrequest != null && CTIrequest.DiagnosticsContext == null)
            {
                CTIrequest.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = CTIrequest.UserId,
                    MessageTrigger = method,
                    OrganizationName = CTIrequest.OrganizationName,
                    StationNumber = CTIrequest.LegacyServiceHeaderInfo != null ? CTIrequest.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            try
            {
                _logData.AppendLine("UDOCTIPersonSearchProcessor - Top");
                Stopwatch txnTimer = Stopwatch.StartNew();
                Stopwatch EntireTimer = Stopwatch.StartNew();
                if (CTIrequest == null)
                {
                    LogHelper.LogError(CTIrequest.MessageId, CTIrequest.OrganizationName, CTIrequest.UserId, $"{GetType().FullName}.Execute",
                        $"<< Exit from {GetType().FullName}. Recieved a null {CTIrequest.GetType().Name} message request or request.Person.");
                }
                else
                {
                    UDOCombinedPersonSearchRequest request = new UDOCombinedPersonSearchRequest
                    {
                        BirthDate = CTIrequest.dob,
                        BypassMvi = CTIrequest.BypassMvi,
                        Debug = CTIrequest.Debug,
                        Edipi = CTIrequest.Edipi,
                        IsAttended = CTIrequest.IsAttended,
                        LegacyServiceHeaderInfo = new UDOHeaderInfo
                        {
                            ApplicationName = CTIrequest.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = CTIrequest.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = CTIrequest.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = CTIrequest.LegacyServiceHeaderInfo.StationNumber
                        },
                        LogSoap = CTIrequest.LogSoap,
                        LogTiming = CTIrequest.LogTiming,
                        MessageId = CTIrequest.MessageId,
                        MVICheck = CTIrequest.MVICheck,
                        noAddPerson = CTIrequest.noAddPerson,
                        OrganizationName = CTIrequest.OrganizationName,
                        SSId = CTIrequest.SSId,
                        SSIdString = CTIrequest.SSIdString,
                        UserLastName = CTIrequest.UserLastName,
                        UserFirstName = CTIrequest.UserFirstName,
                        UserId = CTIrequest.UserId,
                        userSL = CTIrequest.userSL,
                        interactionId = CTIrequest.interactionId
                    };

                    var CommonFunctions = new CRMCommonFunctions();

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

                    if (OrgServiceProxy == null)
                        throw new ApplicationException("Could not connect to CRM");

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
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOCTIPersonSearchProcessor", "About to do deterministic Search", request.Debug);
                            MVIMessages.RetrieveOrSearchPersonResponse personSearhResponse;
                            if (request.IsAttended)
                            {
                                #region deterministic
                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOCTIPersonSearchProcessor", "is Attended = true", request.Debug);

                                MVIMessages.DeterministicSearchRequest deterministicSearchRequest = new MVIMessages.DeterministicSearchRequest();
                                deterministicSearchRequest.MessageId = request.MessageId;
                                deterministicSearchRequest.OrganizationName = request.OrganizationName;
                                deterministicSearchRequest.UserFirstName = request.UserFirstName;
                                deterministicSearchRequest.UserLastName = request.UserLastName;
                                deterministicSearchRequest.UserId = request.UserId;

                                deterministicSearchRequest.BirthDate = request.BirthDate;
                                deterministicSearchRequest.MessageId = request.MessageId;
                                if (string.IsNullOrEmpty(request.Edipi))
                                {
                                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOCTIPersonSearchProcessor", "Go do SOC deterministic Search", request.Debug);
                                    deterministicSearchRequest.SearchType = MVIMessages.DeterministicSearchType.SocialSecurity;
                                    deterministicSearchRequest.SocialSecurityNumber = request.SSId.ToUnsecureString();
                                }
                                else
                                {
                                    if (request.Edipi.Length > 5)
                                    {
                                        deterministicSearchRequest.SearchType = MVIMessages.DeterministicSearchType.Edipi;
                                        deterministicSearchRequest.EdiPi = request.Edipi;
                                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOCTIPersonSearchProcessor", "Go do EDIPI deterministic Search", request.Debug);
                                    }
                                    else
                                    {
                                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOCTIPersonSearchProcessor", "Go do SOC deterministic Search", request.Debug);
                                        deterministicSearchRequest.SearchType = MVIMessages.DeterministicSearchType.SocialSecurity;
                                        deterministicSearchRequest.SocialSecurityNumber = request.SSId.ToUnsecureString();
                                    }
                                }
                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOCTIPersonSearchProcessor", "Go do deterministic Search", request.Debug);

                                personSearhResponse = WebApiUtility.SendReceive<MVIMessages.RetrieveOrSearchPersonResponse>(deterministicSearchRequest, WebApiType.VEIS);
                                if (request.LogSoap || personSearhResponse.ExceptionOccurred)
                                {
                                    if (personSearhResponse.SerializedSOAPRequest != null || personSearhResponse.SerializedSOAPResponse != null)
                                    {
                                        var requestResponse = personSearhResponse.SerializedSOAPRequest + personSearhResponse.SerializedSOAPResponse;
                                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"UnattendedSearchRequest Request/Response {requestResponse}", true);
                                    }
                                }

                                txnTimer.Stop();
                                _logTimerData.AppendLine("Deterministic Search Time:" + txnTimer.ElapsedMilliseconds);
                                txnTimer.Restart();

                                _logData.AppendLine("back from deterministic Search");
                                #endregion
                            }
                            else
                            {
                                #region unattended
                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOCTIPersonSearchProcessor", "is Attended = false", request.Debug);

                                MVIMessages.UnattendedSearchRequest retrievePersonRequest = new MVIMessages.UnattendedSearchRequest(request.Edipi
                                    , request.OrganizationName, request.UserFirstName, request.UserLastName, request.UserId, request.MessageId);

                                //REM: Invoke VEIS WebApi
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
                                _logTimerData.AppendLine("MVI Unattended Search with CTI:" + txnTimer.ElapsedMilliseconds);
                                txnTimer.Restart();
                                #endregion
                            }
                            response = EvaluateMviResponse(personSearhResponse);
                            personSearhResponse.ExceptionOccured = response.ExceptionOccurred;
                            personSearhResponse.Message = response.MVIMessage;
                            personSearhResponse.RawMviExceptionMessage = response.RawMviExceptionMessage;

                            //person result from MVI
                            if (personSearhResponse.Person != null)
                            {
                                response = MaptoResponse(personSearhResponse);
                                //for EDIPI searches, SSN input field is blank. If Traits search, use request SSN for output
                                if (request.SSId != null && request.SSId.Length > 0)
                                {
                                    foreach (PatientPerson item in response.Person)
                                    {
                                        if (item.SSId != null && item.SSId.Length == 0)
                                        {
                                            item.SSId = request.SSId;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //no person result from MVI
                                _logData.AppendLine("Not found, going to handle");
                                response = HandleEmptySearchResponse(request, response);
                                txnTimer.Stop();
                                _logTimerData.AppendLine("No MVI, did CORP Search:" + txnTimer.ElapsedMilliseconds);
                                txnTimer.Restart();
                            }
                        }
                        else
                        {
                            //Bypass MVI for debug purposes
                            _logData.AppendLine("by pass, going to handle");
                            response = new UDOCombinedPersonSearchResponse();
                            response = HandleEmptySearchResponse(request, response);
                            response.MVIMessage = MVISearchResultsWithNoMatch;
                            txnTimer.Stop();
                            txnTimer.Restart();
                        }

                        #endregion

                        #region 1 person logic
                        //new logic for 1 person
                        var srresponse = new SingleReturnProcessor();
                        _logData.AppendLine("Single Person Response");

                        response = srresponse.SinglePersonResponseProcessing(request, response, CommonFunctions, OrgServiceProxy,
                            ref _logData, ref _logTimerData);

                        txnTimer.Stop();
                        _logTimerData.AppendLine("1 person Time:" + txnTimer.ElapsedMilliseconds);
                        txnTimer.Restart();
                        #endregion


                        EntireTimer.Stop();
                        _logTimerData.AppendLine("Entire Search:" + EntireTimer.ElapsedMilliseconds);
                        if (response != null && response.VEISFindVeteranResponse != null) response.VEISFindVeteranResponse = null;
                        if (response != null)
                        {
                            if (response.Person != null)
                            {
                                if (response.Person.Count() > 0)
                                {
                                    if (response.Person[0].SSId != null)
                                    {
                                        response.Person[0].SSIdString = response.Person[0].SSId.ToUnsecureString();
                                    }
                                }
                            }
                        }

                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOCTIPersonSearchProcessor, Debug", _logData.ToString(), request.Debug);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                LogHelper.LogDebug(CTIrequest.MessageId, CTIrequest.OrganizationName, CTIrequest.UserId, "UDOCTIPersonSearchProcessor, Timing", _logTimerData.ToString(), CTIrequest.LogTiming);
                LogHelper.LogDebug(CTIrequest.MessageId, CTIrequest.OrganizationName, CTIrequest.UserId, "UDOCTIPersonSearchProcessor, Debug", _logData.ToString(), CTIrequest.Debug);
                LogHelper.LogError(CTIrequest.MessageId, CTIrequest.OrganizationName, CTIrequest.UserId, "UDOCTIPersonSearchProcessor, Execute", "Exception:" + ex.Message);
                LogHelper.LogError(CTIrequest.MessageId, CTIrequest.OrganizationName, CTIrequest.UserId, "UDOCTIPersonSearchProcessor, Execute", ex);
                if (response == null)
                {
                    response = new UDOCombinedPersonSearchResponse();
                }
                response.MVIMessage = ex.Message;
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
                                combinedResponse.ExceptionOccurred = true;
                                combinedResponse.MVIMessage = MVISearchUnknownError;
                                combinedResponse.RawMviExceptionMessage = response.Acknowledgement.AcknowledgementDetails[0].Text;
                            }
                        }
                        else
                        {
                            combinedResponse.ExceptionOccurred = true;
                            combinedResponse.MVIMessage = MVISearchUnknownError;
                            combinedResponse.RawMviExceptionMessage = response.RawMviExceptionMessage;
                        }
                        break;
                    default:  //bad ack code
                        combinedResponse.ExceptionOccurred = true;
                        combinedResponse.MVIMessage = MVISearchUnknownError;
                        combinedResponse.RawMviExceptionMessage = response.RawMviExceptionMessage;
                        break;
                }
            }
            else
            {
                //if response.Acknowledgement is null, this is likely a connection or internal error
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mviResponse"></param>
        /// <returns></returns>
        private static UDOCombinedPersonSearchResponse MaptoResponse(MVIMessages.RetrieveOrSearchPersonResponse mviResponse)
        {
            if (mviResponse == null)
                return null;

            UDOCombinedPersonSearchResponse response = new UDOCombinedPersonSearchResponse();

            response.ExceptionOccurred = mviResponse.ExceptionOccured;
            response.MVIMessage = mviResponse.Message;
            response.RawMviExceptionMessage = mviResponse.RawMviExceptionMessage;

            if (mviResponse.ExceptionOccured)
            {
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
                    newPerson.SSId = thisPerson.SocialSecurityNumber.ToSecureString();
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
        private static UDOCombinedPersonSearchResponse HandleEmptySearchResponse(UDOCombinedPersonSearchRequest request, UDOCombinedPersonSearchResponse response)
        {
            var processor = new CombinedSecondarySearchProcessor();
            var lobResponse = (UDOCombinedPersonSearchResponse)processor.Execute(request) ??
                           new UDOCombinedPersonSearchResponse();


            lobResponse.ExceptionOccurred = response.ExceptionOccurred;
            lobResponse.MVIMessage = response.MVIMessage;
            lobResponse.RawMviExceptionMessage = response.RawMviExceptionMessage;
            return lobResponse;
        }
    }
}