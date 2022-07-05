using System;
using System.Linq;
using VRM.Integration.UDO.MVI.Messages;
using IMessageBase = VRM.Integration.Servicebus.Core.IMessageBase;
using Logger = VRM.Integration.Servicebus.Core.Logger;
using VRM.Integration.Servicebus.Core;
using MVIMessages = VRM.Integration.Mvi.PersonSearch.Messages;
// using VRM.Integration.UDO.MVI.Faux_Processors;
// using VRM.Integration.Mvi.PersonSearch;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using UDO.Crm.LOB.Extensions;
using System.Configuration;

namespace VRM.Integration.UDO.MVI.Processors
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
        private readonly string veisBaseUrl;
        private LogSettings logSettings;

        public UDOPersonSearchProcessor()
        {
            //TODO: Read this URL from Settings entity.
            veisBaseUrl = ConfigurationManager.AppSettings[CONFIG_VEIS_BASE_URL].ToString();
            
        
        }
        
        internal void InitLogSettings(UDOPersonSearchRequest request)
        {
            logSettings = new LogSettings()
            {
                callingMethod = "UDOPersonSearchProcessor", Org = request.OrganizationName, UserId = request.UserId, ConfigFieldName = ""
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IMessageBase Execute(UDOPersonSearchRequest request)
        {
            InitLogSettings(request);
            UDOPersonSearchResponse response = null;
            MVIMessages.RetrieveOrSearchPersonResponse personSearhResponse;

            try
            {

                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOPersonSearchProcessor", "Top");

                //var request = message as IMSDEVGetVeteranInfoRequest;
               if (request == null)
                {
                    Logger.Instance.Warn(string.Format("{0} recieved a null message", GetType().FullName));
                }
                else
                {
                    
                    #region - do the real MVI search - MVI handles the whole attended versus unattended thing
                    if (request.AlreadyQueriedMvi != null && request.AlreadyQueriedMvi == false)
                    {
                        Logger.Instance.Info("searching real MVI");
                        //map from the MVI response to our response, which looks the same.
                      

                        if (request.IsAttended)
                        {
                            var searchPersonRequest = new MVIMessages.AttendedSearchRequest(request.FirstName,
                                request.MiddleName, request.FamilyName, request.SSIdString, request.BirthDate,
                                request.PhoneNumber, request.UserId, request.UserFirstName, request.UserLastName,
                                request.OrganizationName, request.MessageId);

                            // Commented old VIMT Call
                            // personSearhResponse = searchPersonRequest.SendReceive<MVIMessages.RetrieveOrSearchPersonResponse>(MessageProcessType.Local);

                            // Remediated VEIS WebApi call.
                            personSearhResponse = WebApiUtility.SendReceive<MVIMessages.RetrieveOrSearchPersonResponse>(new Uri(this.veisBaseUrl),
                                searchPersonRequest.MessageId, searchPersonRequest, this.logSettings);
                        }
                        else
                        {
                            var retrievePersonRequest = new MVIMessages.UnattendedSearchRequest(request.Edipi, request.UserId, request.UserFirstName, request.UserLastName, request.OrganizationName, request.MessageId);

                            personSearhResponse = retrievePersonRequest.SendReceive<MVIMessages.RetrieveOrSearchPersonResponse>(MessageProcessType.Local);
                        }
                    }
                    else
                    {
                        personSearhResponse = null;
                    }

                        if (personSearhResponse != null && personSearhResponse.Person != null)
                        {
                            response = MaptoResponse(personSearhResponse);
                         //   LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOPersonSearchProcessor, Execute", "Mapped a response from MVI");
                            
                        }
                        else
                        {
                            //moved this here from below.
                         //   LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOPersonSearchProcessor, Execute", "No response from MVI, going to CORPDb");

                            response = HandleEmptySearchResponse(request, response);
                            response.MVIMessage = MVISearchResultsWithNoMatch;
                        }

                        #endregion

                      

                }
                return response;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOPersonSearchProcessor, Execute", "Exception:" + ex.Message);
                if (response == null)
                {
                    response = new UDOPersonSearchResponse();
                } 
                response.MVIMessage = ex.Message;
                response.ExceptionOccured = true;
                return response;

            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mviResponse"></param>
        /// <returns></returns>
        private static UDOPersonSearchResponse MaptoResponse(MVIMessages.RetrieveOrSearchPersonResponse mviResponse)
        {
            if (mviResponse == null)
                return null;

            var response = new UDOPersonSearchResponse();

            response.ExceptionOccured = mviResponse.ExceptionOccured;
            response.MVIMessage = mviResponse.Message;
            response.RawMviExceptionMessage = mviResponse.RawMviExceptionMessage;

            if (mviResponse.ExceptionOccured)
                return response;

            #region map mviperson

            System.Collections.Generic.List<PatientPerson> people = new System.Collections.Generic.List<PatientPerson>();
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
                        //newPerson.Address.Use == new PatientAddress.

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
                            var thisId = new CorrespondingIDs();
                            thisId.AssigningAuthority = thisPersonIds.AssigningAuthority;
                            thisId.AssigningFacility = thisPersonIds.AssigningFacility;
                            thisId.FetchMessageProcessType = thisPersonIds.FetchMessageProcessType;
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
        private static void FormatData(PatientPerson[] patientPerson)
        {
            if (patientPerson != null)
            {
                foreach (var person in patientPerson)
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
    
        private static string FormatDate(string dateString, string format="yyyyMMdd")
        {
            DateTime date;
            try
            {
                date=DateTime.ParseExact(dateString, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
                return date.ToString("MM/dd/yyyy");
            }
            catch (FormatException dateFormatException)
            {
                //If date cannot be reformatted return the date present in the system.
                return dateString; 
            }
            catch (ArgumentException dateArgumentException)
            {
                //If date cannot be reformatted return the date present in the system.
                return dateString;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IMSResponse"></param>
        /// <param name="BranchOfService"></param>
        /// <returns></returns>
        //private static UDOPersonSearchResponse MaptoResponse(IMS.IMSDEVMVISearchByFilterResponse IMSResponse, string BranchOfService)
        //{
        //    var finalResponse = new UDOPersonSearchResponse();
        //    System.Collections.Generic.List<PatientPerson> patRecord = new System.Collections.Generic.List<PatientPerson>();

        //    foreach (var item in IMSResponse.IMSDEVMVISearchByFilter)
        //    {
        //        var thisRecord = new PatientPerson();
        //        thisRecord.GenderCode = item.crme_Gender;
        //        thisRecord.PhoneNumber = item.crme_PrimaryPhone;
        //        thisRecord.SSIdString = item.crme_SSN;
        //        thisRecord.BirthDate = item.crme_DOB.ToLongDateString();
        //        thisRecord.RecordSource = "IMS-MVI";
        //        thisRecord.ParticipantId = item.crme_ParticipantID;
        //        thisRecord.BranchOfService = item.crme_BranchOfService;
        //        thisRecord.EdiPi = item.crme_EDIPI;
        //        thisRecord.VeteranSensitivityLevel = item.crme_VeteranSensitivityLevel;
        //        var newName = new Name();
        //        newName.FamilyName = item.crme_LastName;
        //        newName.GivenName = item.crme_FirstName;
        //        newName.NameType = "Legal";
        //        newName.MiddleName = item.crme_MiddleName;
        //        System.Collections.Generic.List<Name> newArr = new System.Collections.Generic.List<Name>();
        //        newArr.Add(newName);
        //        thisRecord.NameList = newArr.ToArray();

        //        var newAddress = new PatientAddress();
        //        newAddress.City = item.crme_City;
        //        if (item.crme_CountryId != null)
        //        {
        //            newAddress.Country = item.crme_CountryId.Name;
        //        }
        //        if (item.crme_ZIPPostalCodeId != null)
        //        {
        //            newAddress.PostalCode = item.crme_ZIPPostalCodeId.Name;
        //        }
        //        if (item.crme_StateProvinceId != null)
        //        {
        //            newAddress.State = item.crme_StateProvinceId.Name;
        //        }
        //        newAddress.StreetAddressLine = item.crme_Address1;

        //        thisRecord.Address = newAddress;

        //        if (!string.IsNullOrEmpty(BranchOfService))
        //        {
        //            if (BranchOfService == item.crme_BranchOfService)
        //            {
        //                patRecord.Add(thisRecord);
        //            }
        //        }
        //        else
        //        {
        //            patRecord.Add(thisRecord);
        //        }
        //    }
        //    finalResponse.Person = patRecord.ToArray(); 
        //    return finalResponse;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IMSResponse"></param>
        /// <param name="BranchOfService"></param>
        /// <returns></returns>
        //private static UDOPersonSearchResponse MaptoResponse(IMS.IMSDEVMVISearchByIdentifierResponse IMSResponse, string BranchOfService)
        //{
        //    var finalResponse = new UDOPersonSearchResponse();
        //    System.Collections.Generic.List<PatientPerson> patRecord = new System.Collections.Generic.List<PatientPerson>();
        //    foreach (var item in IMSResponse.IMSDEVMVISearchByIdentifier)
        //    {
                   
        //               var thisRecord = new PatientPerson();
        //               thisRecord.GenderCode = item.crme_Gender;
        //               thisRecord.PhoneNumber = item.crme_PrimaryPhone;
        //               thisRecord.SSIdString = item.crme_SSN;
        //               thisRecord.BirthDate = item.crme_DOB.ToLongDateString();
        //               thisRecord.RecordSource = "IMS-MVI";
        //               thisRecord.ParticipantId = item.crme_ParticipantID;
        //               thisRecord.BranchOfService = item.crme_BranchOfService;

        //               var newName = new Name();
        //               newName.FamilyName = item.crme_LastName;
        //               newName.GivenName = item.crme_FirstName;
        //               newName.NameType = "Legal";
        //               newName.MiddleName = item.crme_MiddleName;
        //               System.Collections.Generic.List<Name> newArr = new System.Collections.Generic.List<Name>();
        //               newArr.Add(newName);
        //               thisRecord.NameList = newArr.ToArray();

        //               var newAddress = new PatientAddress();
        //               newAddress.City = item.crme_City;
        //               if (item.crme_CountryId != null)
        //               {
        //                   newAddress.Country = item.crme_CountryId.Name;
        //               }
        //               if (item.crme_ZIPPostalCodeId != null)
        //               {
        //                   newAddress.PostalCode = item.crme_ZIPPostalCodeId.Name;
        //               }
        //               if (item.crme_StateProvinceId != null)
        //               {
        //                   newAddress.State = item.crme_StateProvinceId.Name;
        //               }
        //               newAddress.StreetAddressLine = item.crme_Address1;

        //               thisRecord.Address = newAddress;


        //               if (!string.IsNullOrEmpty(BranchOfService))
        //               {
        //                   if (BranchOfService == item.crme_BranchOfService)
        //                   {
        //                       patRecord.Add(thisRecord);
        //                   }
        //               }
        //               else
        //               {
        //                   patRecord.Add(thisRecord);
        //               }

                   
        //    }
        //    finalResponse.Person = patRecord.ToArray();
        //    return finalResponse;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private static UDOPersonSearchResponse HandleEmptySearchResponse(UDOPersonSearchRequest request, UDOPersonSearchResponse response)
        {
            // if an error occured from MVI, return the response. 
            //if (response.ExceptionOccured)
            //{
            //    return response;
            //}

            /* TODO: Implement call to SecondarySearchProcessor
            var processor = new SecondarySearchProcessor();
            var lobResponse = (UDOPersonSearchResponse)processor.Execute(request) ??
                           new UDOPersonSearchResponse();

            return lobResponse;
            */
            return new UDOPersonSearchResponse();
        }
    }
}

