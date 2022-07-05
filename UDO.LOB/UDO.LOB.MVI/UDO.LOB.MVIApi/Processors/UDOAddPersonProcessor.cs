using System;
using System.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Messages;
using VEIS.Messages.VeteranWebService;
using MVIMessages = VEIS.Mvi.Messages;

namespace UDO.LOB.MVI.Processors
{
    public class UDOAddPersonProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOAddPersonProcessor";

        // Replaced: void execute with IMessageBase to match other processors.
        public void Execute(UDOAddPersonRequest request)
        {
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA",
                };
            }
            TraceLogger logger = new TraceLogger(method, request);

            try
            {
                if (request == null)
                {
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, $"{GetType().FullName}.Execute",
                        $"<< Exit from {GetType().FullName}. Recieved a null {request.GetType().Name} message request or request.Person.");
                }
                else
                {
                    var findVeteranResponse = request.VEISResponse;

                    #region - if there is data, map it
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
                                #region AddPersontoMvi
                                //add code for add person here.
                                MVIMessages.PatientAddress newAddress = new MVIMessages.PatientAddress
                                {
                                    StreetAddressLine = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_addressLine1,
                                    City = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_city,
                                    State = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_state,
                                    Country = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_country,
                                    PostalCode = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_zipCode,
                                    Use = MVIMessages.AddressUse.Home
                                };

                                MVIMessages.AddPatientPerson newPatientPerson = new MVIMessages.AddPatientPerson();
                                var PIDID = new MVIMessages.AsOtherId
                                {
                                    ClassCode = MVIMessages.ClassCode.PATCORP,
                                    Id = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_ptcpntId,
                                    StatusCode = MVIMessages.CorrelationStatusCode.InProgress
                                };
                                var SSNID = new MVIMessages.AsOtherId
                                {
                                    ClassCode = MVIMessages.ClassCode.SSN,
                                    Id = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_ssn,
                                    StatusCode = MVIMessages.CorrelationStatusCode.InProgress
                                };
                                //MARLON UPDATES
                                newPatientPerson.AsOtherIds = new[] { PIDID, SSNID };
                                newPatientPerson.Address = newAddress;

                                newPatientPerson.BirthDate = dob;

                                if (findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo != null)
                                {
                                    newPatientPerson.DeceasedDate = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.mcs_DATE_OF_DEATH;

                                    newPatientPerson.GenderCode = getGenderFromCorpBirls(findVeteranResponse);
                                }
                                #region phone numbers
                                var area1 = "";
                                var area2 = "";
                                var phone1 = "";
                                var phone2 = "";
                                var phoneType1 = "";
                                var phoneType2 = "";
                                if (!string.IsNullOrEmpty(findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_areaNumberOne))
                                {
                                    area1 = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_areaNumberOne;
                                }
                                if (!string.IsNullOrEmpty(findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_areaNumberTwo))
                                {
                                    area2 = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_areaNumberTwo;
                                }
                                if (!string.IsNullOrEmpty(findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_phoneNumberOne))
                                {
                                    phone1 = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_phoneNumberOne;
                                }
                                if (!string.IsNullOrEmpty(findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_phoneNumberTwo))
                                {
                                    phone2 = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_phoneNumberTwo;
                                }
                                if (!string.IsNullOrEmpty(findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_phoneTypeNameOne))
                                {
                                    phoneType1 = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_phoneTypeNameOne;
                                }
                                if (!string.IsNullOrEmpty(findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_phoneTypeNameTwo))
                                {
                                    phoneType2 = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_phoneTypeNameTwo;
                                }

                                if (phoneType1.Equals("Daytime", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    newPatientPerson.HomePhoneNumber = FormatTelephone(area1 + phone1);
                                }
                                if (phoneType2.Equals("Daytime", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    newPatientPerson.HomePhoneNumber = FormatTelephone(area2 + phone2);
                                }
                                #endregion

                                var middleName = string.IsNullOrEmpty(findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_middleName) ? null : findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_middleName;
                                newPatientPerson.NameList = new[] { new MVIMessages.Name { 
                                        GivenName = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_firstName,
                                        MiddleName = middleName,
                                        FamilyName = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo.mcs_lastName,
                                        Use = MVIMessages.NameUse.Legal 
                                    } };

                                var addPersonRequest = new MVIMessages.AddPersonRequest(request.UserId, request.userfirstname, request.userlastname, request.OrganizationName, request.MessageId)
                                {
                                    Subject1 = new MVIMessages.AddSubject1
                                    {
                                        Patient = new MVIMessages.AddPatient
                                        {
                                            PatientPerson = newPatientPerson
                                        }
                                    }
                                    
                                };
                                addPersonRequest.MessageId = request.MessageId;
                                Guid result = Guid.Empty;
                                Guid.TryParse(request.MessageId, out result);
                                addPersonRequest.CorrelationId = result;

                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "About to Add Person", request.Debug);
								var addPersonResponse = WebApiUtility.SendReceive<MVIMessages.AddPersonResponse>(addPersonRequest, WebApiType.VEIS);
                                if (request.LogSoap || addPersonResponse.ExceptionOccurred)
                                {
                                    if (addPersonResponse.SerializedSOAPRequest != null || addPersonResponse.SerializedSOAPResponse != null)
                                    {
                                        var requestResponse = addPersonResponse.SerializedSOAPRequest + addPersonResponse.SerializedSOAPResponse;
                                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"AddPersonRequest Request/Response {requestResponse}", true);
                                    }
                                }

                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Added Person:" + addPersonResponse.Message, request.Debug);
                                #endregion AddPersonToMvi
                            }
                        }
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.MessageId, method, ex);
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

        private MVIMessages.GenderCode getGenderFromCorpBirls(VEISfvetfindVeteranResponse fr)
        {
            if (fr.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.mcs_SEX_CODE != null && fr.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.mcs_SEX_CODE.Trim().ToUpper() == "M")
                return MVIMessages.GenderCode.M;
            else if (fr.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.mcs_SEX_CODE != null && fr.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.mcs_SEX_CODE.Trim().ToUpper() == "F")
                return MVIMessages.GenderCode.F;
            else
                return MVIMessages.GenderCode.NotSpecified;
        }
        private string FormatTelephone(string telephoneNumber)
        {
            var Phone = telephoneNumber;
            var ext = "";
            var result = "";

            if (0 != Phone.IndexOf('+'))
            {
                if (1 < Phone.LastIndexOf('x'))
                {
                    ext = Phone.Substring(Phone.LastIndexOf('x'));
                    Phone = Phone.Substring(0, Phone.LastIndexOf('x'));
                }
            }
            result = Phone;
            if (7 == Phone.Length)
            {
                result = Phone.Substring(0, 3) + "-" + Phone.Substring(3);
            }
            if (10 == Phone.Length)
            {
                result = "(" + Phone.Substring(0, 3) + ") " + Phone.Substring(3, 3) + "-" + Phone.Substring(6);
            }
            if (0 < ext.Length)
            {
                result = result + " " + ext;
            }
            return result;
        }
    }
}

