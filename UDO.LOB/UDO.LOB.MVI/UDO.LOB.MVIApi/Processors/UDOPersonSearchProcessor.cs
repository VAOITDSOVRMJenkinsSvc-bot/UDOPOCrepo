using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Configuration;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Messages;
using UDO.LOB.MVI.Processors;
using VeisMessages = VEIS.Mvi.Messages;

namespace UDO.LOB.PersonSearch.Processors
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewe By Brian Greig - 4/30/15
    /// </remarks>
    public class UDOPersonSearchProcessor
    {
        private const string MVISearchResultsWithMatch = "A search in MVI found {0} matching record(s).";
        private const string MVISearchResultsWithNoMatch = "A search in MVI did not find any records matching the search criteria.";
        private const string CONFIG_VEIS_BASE_URL = "VEIS_BASE_URL";

        private const string method = "UDOPersonSearchProcessor";

        // REM: New variables
        private Uri veisBaseUri;
        private LogSettings logSettings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IMessageBase Execute(UDOPersonSearchRequest request)
        {
            UDOPersonSearchResponse response = null;
            VeisMessages.RetrieveOrSearchPersonResponse personSearchResponse;
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
            try
            {
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOPersonSearchProcessor", $">> Entered {this.GetType().FullName}", request.Debug);
                InitProcessor(request);

                if (request == null)
                {
                    //TODO: Revisit log details; Missing MessageId
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, $"{GetType().FullName}.Execute",
                        $"<< Exit from {GetType().FullName}. Recieved a null {request.GetType().Name} message request or request.Person.");
                }
                else
                {
                    #region - do the real MVI search - MVI handles the whole attended versus unattended thing
                    if (request.AlreadyQueriedMvi == false)
                    {
                        //map from the MVI response to our response, which looks the same.
                        if (request.IsAttended)
                        {
                            VeisMessages.AttendedSearchRequest attendedSearchRequest = new VeisMessages.AttendedSearchRequest(request.FirstName,
                                request.MiddleName, request.FamilyName, request.SSIdString, request.BirthDate,
                                request.PhoneNumber, request.UserId, request.UserFirstName, request.UserLastName,
                                request.OrganizationName, request.MessageId);

							// Remediated VEIS WebApi call.
							//CSDev Updated to New Overloaded Method
							personSearchResponse = WebApiUtility.SendReceive<VeisMessages.RetrieveOrSearchPersonResponse>(attendedSearchRequest, WebApiType.VEIS);
                            if (request.LogSoap || personSearchResponse.ExceptionOccurred)
                            {
                                if (personSearchResponse.SerializedSOAPRequest != null || personSearchResponse.SerializedSOAPResponse != null)
                                {
                                    var requestResponse = personSearchResponse.SerializedSOAPRequest + personSearchResponse.SerializedSOAPResponse;
                                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"UnattendedSearchRequest Request/Response {requestResponse}", true);
                                }
                            }
                        }
						else
                        {
                            VeisMessages.UnattendedSearchRequest unattendedSearchRequest = new VeisMessages.UnattendedSearchRequest(request.Edipi
								, request.OrganizationName, request.UserFirstName, request.UserLastName, request.UserId, request.MessageId);

							//CSDev Updated to New Overloaded Method
							personSearchResponse = WebApiUtility.SendReceive<VeisMessages.RetrieveOrSearchPersonResponse>(unattendedSearchRequest, WebApiType.VEIS);
                            if (request.LogSoap || personSearchResponse.ExceptionOccurred)
                            {
                                if (personSearchResponse.SerializedSOAPRequest != null || personSearchResponse.SerializedSOAPResponse != null)
                                {
                                    var requestResponse = personSearchResponse.SerializedSOAPRequest + personSearchResponse.SerializedSOAPResponse;
                                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"UnattendedSearchRequest Request/Response {requestResponse}", true);
                                }
                            }
                        }
                    }
                    else
                    {
                        personSearchResponse = null;
                    }

                    if (personSearchResponse != null && personSearchResponse.Person != null)
                    {
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOPersonSearchProcessor.Execute", "> Mapping a response from MVI", request.Debug);
                        response = MaptoResponse(personSearchResponse);
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOPersonSearchProcessor.Execute", "< Mapped a response from MVI", request.Debug);
                    }
                    else
                    {
                        //moved this here from below.
                        LogHelper.LogInfo(request.MessageId, request.OrganizationName, request.UserId, "UDOPersonSearchProcessor.Execute", "No response from MVI, going to CORPDb");

                        response = HandleEmptySearchResponse(request, response);
                        response.MVIMessage = MVISearchResultsWithNoMatch;
                    }

                    #endregion
                }

                return response;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "#Error in UDOPersonSearchProcessor.Execute", ex);
                if (response == null)
                {
                    response = new UDOPersonSearchResponse
                    {
                        MVIMessage = ex.Message,
                        ExceptionOccured = true
                    };
                }

                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mviResponse"></param>
        /// <returns></returns>
        private static UDOPersonSearchResponse MaptoResponse(VeisMessages.RetrieveOrSearchPersonResponse mviResponse)
        {
            if (mviResponse == null)
                return null;

            UDOPersonSearchResponse response = new UDOPersonSearchResponse
            {
                ExceptionOccured = mviResponse.ExceptionOccured,
                MVIMessage = mviResponse.Message,
                RawMviExceptionMessage = mviResponse.RawMviExceptionMessage
            };

            if (mviResponse.ExceptionOccured)
                return response;

            #region map mviperson

            List<PatientPerson> people = new List<PatientPerson>();
            if (mviResponse.Person != null)
            {
                foreach (var thisPerson in mviResponse.Person)
                {

                    var newPerson = new PatientPerson();
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
                        foreach (var thisPersonIds in thisPerson.CorrespondingIdList)
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
                    newPerson.SSIdString = thisPerson.SocialSecurityNumber;
                    newPerson.SSId = thisPerson.SocialSecurityNumber.ToSecureString();
                    newPerson.StatusCode = thisPerson.StatusCode;

                    #region namelist
                    System.Collections.Generic.List<Name> names = new System.Collections.Generic.List<Name>();
                    if (thisPerson.NameList != null)
                    {
                        foreach (var thisName in thisPerson.NameList)
                        {
                            var newName = new Name();
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
        private static void FormatData(VeisMessages.PatientPerson[] patientPerson)
        {
            if (patientPerson != null)
            {
                foreach (VeisMessages.PatientPerson person in patientPerson)
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
        private static UDOPersonSearchResponse HandleEmptySearchResponse(UDOPersonSearchRequest request, UDOPersonSearchResponse response)
        {
            var processor = new SecondarySearchProcessor();
            var lobResponse = (UDOPersonSearchResponse)processor.Execute(request) ??
                           new UDOPersonSearchResponse();

            return lobResponse;
        }

        private void InitProcessor(UDOPersonSearchRequest request)
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

