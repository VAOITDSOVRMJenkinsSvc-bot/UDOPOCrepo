using System;
using System.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Messages;
using VEIS.Messages.VeteranWebService;

/// <summary>
/// LOB Component for UDOfindVeteranInfo,findVeteranInfo method, Processor.
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace UDO.LOB.MVI.Processors
{
    public class UDOfindVeteranInfoProcessor 
	{
        public IMessageBase Execute(UDOfindVeteranInfoRequest request)
		{
			//var request = message as findVeteranInfoRequest;
			UDOfindVeteranInfoResponse response = new UDOfindVeteranInfoResponse();
			var progressString = "Top of Processor";
            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOfindVeteranInfoProcessor", "Top", request.Debug);
			if (request == null)
			{
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, $"{GetType().FullName}.Execute",
                        $"<< Exit from {GetType().FullName}. Recieved a null {request.GetType().Name} message request or request.Person.");

				response.ExceptionMessage = "Called with no message";
				response.ExceptionOccured = true;
				return response;
			}
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = "UDOfindVeteranInfoProcessor",
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            try
            {
                var findVeteranRequest = new VEISfvetfindVeteranRequest();
                findVeteranRequest.MessageId = request.MessageId;
                findVeteranRequest.LogTiming = request.LogTiming;
                findVeteranRequest.LogSoap = request.LogSoap;
                findVeteranRequest.Debug = request.Debug;
                findVeteranRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findVeteranRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findVeteranRequest.RelatedParentId = request.RelatedParentId;
                findVeteranRequest.UserId = request.UserId;
                findVeteranRequest.OrganizationName = request.OrganizationName;
                findVeteranRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
                if (!string.IsNullOrEmpty(request.SocialSN))
                {
                    findVeteranRequest.VEISfvetReqveteranRecordInputInfo = new VEISfvetReqveteranRecordInput
                    {
                        mcs_ssn = request.SocialSN
                        
                    };
                }
                else
                {
                    findVeteranRequest.VEISfvetReqveteranRecordInputInfo = new VEISfvetReqveteranRecordInput
                    {
                        mcs_fileNumber = request.fileNumber
                    };
                }

                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "FindinCRMProcessor, Execute", "Trying to call VIMTfvetfindVeteranResponse", request.Debug);
				var findVeteranResponse = WebApiUtility.SendReceive<VEISfvetfindVeteranResponse>(findVeteranRequest, WebApiType.VEIS);
                if (request.LogSoap || findVeteranResponse.ExceptionOccurred)
                {
                    if (findVeteranResponse.SerializedSOAPRequest != null || findVeteranResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findVeteranResponse.SerializedSOAPRequest + findVeteranResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfvetfindVeteranRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VIMT EC Call";
                if (findVeteranResponse != null && findVeteranResponse.VEISfvetreturnInfo != null && findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo != null &&
                    findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.mcs_RETURN_MESSAGE != null &&
                    findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.mcs_RETURN_MESSAGE.Contains("BIRLS communication is down"))
                {
                    response.ExceptionMessage = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.mcs_RETURN_MESSAGE;
                    response.ExceptionOccured = true;
                    return response;
                }
                response.ExceptionMessage = findVeteranResponse.ExceptionMessage;
                response.ExceptionOccured = findVeteranResponse.ExceptionOccurred;
               
                #region - if there is data, map it
                if (findVeteranResponse.VEISfvetreturnInfo != null)
                {
                    response.UDOfindVeteranInfoInfo = new UDOfindVeteranInfo();
                    response.UDOfindVeteranInfoInfo.VEISResponse = findVeteranResponse;
                    #region vetInfo
                    if (findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo != null)
                    {
                        var vetCorpRecord = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetCorpRecordInfo;
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_sensitiveLevelOfRecord))
                        {
                            response.UDOfindVeteranInfoInfo.crme_VeteranSensitivityLevel = vetCorpRecord.mcs_sensitiveLevelOfRecord;
                        }
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_ptcpntId))
                        {
                            response.UDOfindVeteranInfoInfo.crme_ParticipantID = vetCorpRecord.mcs_ptcpntId;
                        }
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_middleName))
                        {
                            response.UDOfindVeteranInfoInfo.crme_MiddleName = vetCorpRecord.mcs_middleName;
                        }
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_lastName))
                        {
                            response.UDOfindVeteranInfoInfo.crme_LastName = vetCorpRecord.mcs_lastName;
                        }
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_firstName))
                        {
                            response.UDOfindVeteranInfoInfo.crme_FirstName = vetCorpRecord.mcs_firstName;
                        }
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_fileNumber))
                        {
                            response.UDOfindVeteranInfoInfo.crme_FileNumber = vetCorpRecord.mcs_fileNumber;
                        }
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_dateOfBirth))
                        {
                            DateTime newDateTime;
                            if (DateTime.TryParse(vetCorpRecord.mcs_dateOfBirth, out newDateTime))
                            {
                                response.UDOfindVeteranInfoInfo.crme_DOB = newDateTime;
                            }
                        }
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_city))
                        {
                            response.UDOfindVeteranInfoInfo.crme_City = vetCorpRecord.mcs_city;
                        }                        
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_phoneNumberOne))
                        {
                            response.UDOfindVeteranInfoInfo.crme_PrimaryPhone = "(" + vetCorpRecord.mcs_areaNumberOne + ") " + vetCorpRecord.mcs_phoneNumberOne.Substring(0,3) + "-" + vetCorpRecord.mcs_phoneNumberOne.Substring(3);
                        }                        
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_addressLine3))
                        {       
                            response.UDOfindVeteranInfoInfo.crme_Address3 = vetCorpRecord.mcs_addressLine3;
                        }
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_addressLine2))
                        {
                            response.UDOfindVeteranInfoInfo.crme_Address2 = vetCorpRecord.mcs_addressLine2;
                        }
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_addressLine1))
                        {
                            response.UDOfindVeteranInfoInfo.crme_Address1 = vetCorpRecord.mcs_addressLine1;
                        }
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_ssn))
                        {
                            response.UDOfindVeteranInfoInfo.crme_SSN = vetCorpRecord.mcs_ssn;
                        }
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_zipCode))
                        {
                            response.UDOfindVeteranInfoInfo.crme_Zip = vetCorpRecord.mcs_zipCode;
                        }
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_country))
                        {
                            response.UDOfindVeteranInfoInfo.crme_Country = vetCorpRecord.mcs_country;
                        }
                        if (!string.IsNullOrEmpty(vetCorpRecord.mcs_state))
                        {
                            response.UDOfindVeteranInfoInfo.crme_State = vetCorpRecord.mcs_state;
                        }
                    }
                    #endregion

                    #region birlsInfo
                    if (findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo != null)
                    {
                        var vetvetBirlsRecord = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo;
                        if (!string.IsNullOrEmpty(vetvetBirlsRecord.mcs_SEX_CODE))
                        {
                            response.UDOfindVeteranInfoInfo.crme_Gender = vetvetBirlsRecord.mcs_SEX_CODE;
                        }
                        if (!string.IsNullOrEmpty(vetvetBirlsRecord.mcs_CAUSE_OF_DEATH))
                        {
                            response.UDOfindVeteranInfoInfo.crme_CauseOfDeath = vetvetBirlsRecord.mcs_CAUSE_OF_DEATH;
                        }
                        if (!string.IsNullOrEmpty(vetvetBirlsRecord.mcs_CLAIM_FOLDER_LOCATION))
                        {
                            response.UDOfindVeteranInfoInfo.crme_stationofJurisdiction = vetvetBirlsRecord.mcs_CLAIM_FOLDER_LOCATION;
                        }
                        if (!string.IsNullOrEmpty(vetvetBirlsRecord.mcs_DATE_OF_DEATH))
                        {
                            response.UDOfindVeteranInfoInfo.crme_DeceasedDate = vetvetBirlsRecord.mcs_DATE_OF_DEATH;
                        }
                        if (vetvetBirlsRecord.VEISfvetSERVICEInfo != null)
                        {
                            var serviceDTO = vetvetBirlsRecord.VEISfvetSERVICEInfo;
                            var BOS = "";
                            foreach (var item in serviceDTO)
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
                            response.UDOfindVeteranInfoInfo.crme_BranchOfService = BOS;

                            var charOfSvcCode = serviceDTO.OrderByDescending(h => DateTime.TryParse(h.mcs_ENTERED_ON_DUTY_DATE, out DateTime newDateTime)).FirstOrDefault().mcs_CHAR_OF_SVC_CODE;
                            if (!string.IsNullOrEmpty(charOfSvcCode)) response.UDOfindVeteranInfoInfo.crme_CharacterofDishcarge = charOfSvcCode;
                        }
                    }
                    #endregion
                }
                #endregion
                return response;
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOfindVeteranInfoProcessor Processor, Progess:" + progressString, connectException.Message);
                
                response.ExceptionMessage = "Failed to get Map EC to LOB";
                response.ExceptionOccured = true;
                return response;
            }
		}

        public IMessageBase Execute(UDOfindVeteranInfoByPidRequest request)
        {
            UDOfindVeteranInfoByPidResponse response = new UDOfindVeteranInfoByPidResponse();
            var progressString = "Top of Processor";
            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOfindVeteranInfoProcessor", "Top", request.Debug);
            if (request == null)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, $"{GetType().FullName}.Execute",
                        $"<< Exit from {GetType().FullName}. Recieved a null {request.GetType().Name} message request or request.Person.");

                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = "UDOfindVeteranInfoProcessor",
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }

            try
            {
                var findVeteranByPidRequest = new VEISvetPctfindVeteranByPtcpntIdRequest();
                findVeteranByPidRequest.MessageId = request.MessageId;
                findVeteranByPidRequest.LogTiming = request.LogTiming;
                findVeteranByPidRequest.LogSoap = request.LogSoap;
                findVeteranByPidRequest.Debug = request.Debug;
                findVeteranByPidRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findVeteranByPidRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findVeteranByPidRequest.RelatedParentId = request.RelatedParentId;
                findVeteranByPidRequest.UserId = request.UserId;
                findVeteranByPidRequest.OrganizationName = request.OrganizationName;
                findVeteranByPidRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };

                Int64.TryParse(request.ParticipantID, out long convertedPid);
                findVeteranByPidRequest.mcs_ptcpntid = convertedPid;

                var findVetByPidResponse = WebApiUtility.SendReceive<VEISvetPctfindVeteranByPtcpntIdResponse>(findVeteranByPidRequest, WebApiType.VEIS);
                if (request.LogSoap || findVetByPidResponse.ExceptionOccurred)
                {
                    if (findVetByPidResponse.SerializedSOAPRequest != null || findVetByPidResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findVetByPidResponse.SerializedSOAPRequest + findVetByPidResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISvetPctfindVeteranByPtcpntIdRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VIMT EC Call";
                if (findVetByPidResponse != null && findVetByPidResponse.VEISvetPctreturnInfo != null && findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo != null &&
                    findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo.mcs_RETURN_MESSAGE != null &&
                    findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo.mcs_RETURN_MESSAGE.Contains("BIRLS communication is down"))
                {
                    response.ExceptionMessage = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo.mcs_RETURN_MESSAGE;
                    response.ExceptionOccured = true;
                    return response;
                }
                response.ExceptionMessage = findVetByPidResponse.ExceptionMessage;
                response.ExceptionOccured = findVetByPidResponse.ExceptionOccurred;

                if (findVetByPidResponse.VEISvetPctreturnInfo != null)
                {
                    response.UDOfindVeteranInfoInfo = new UDOfindVeteranInfo();
                    response.UDOfindVeteranInfoInfo.VEISByPtcpntIdResponse = findVetByPidResponse;
                    #region vetInfo
                    if (findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo != null)
                    {
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_sensitiveLevelOfRecord))
                        {
                            response.UDOfindVeteranInfoInfo.crme_VeteranSensitivityLevel = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_sensitiveLevelOfRecord;
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_ptcpntId))
                        {
                            response.UDOfindVeteranInfoInfo.crme_ParticipantID = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_ptcpntId;
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_middleName))
                        {
                            response.UDOfindVeteranInfoInfo.crme_MiddleName = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_middleName;
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_lastName))
                        {
                            response.UDOfindVeteranInfoInfo.crme_LastName = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_lastName;
                        }

                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_firstName))
                        {
                            response.UDOfindVeteranInfoInfo.crme_FirstName = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_firstName;
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_fileNumber))
                        {
                            response.UDOfindVeteranInfoInfo.crme_FileNumber = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_fileNumber;
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_dateOfBirth))
                        {
                            DateTime newDateTime;
                            if (DateTime.TryParse(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_dateOfBirth, out newDateTime))
                            {
                                response.UDOfindVeteranInfoInfo.crme_DOB = newDateTime;
                            }
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_city))
                        {
                            response.UDOfindVeteranInfoInfo.crme_City = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_city;
                        }

                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_phoneNumberOne))
                        {
                            response.UDOfindVeteranInfoInfo.crme_PrimaryPhone = "(" + findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_areaNumberOne + ") " + findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_phoneNumberOne.Substring(0, 3) + "-" + findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_phoneNumberOne.Substring(3);
                        }

                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_addressLine3))
                        {
                            response.UDOfindVeteranInfoInfo.crme_Address3 = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_addressLine3;
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_addressLine2))
                        {
                            response.UDOfindVeteranInfoInfo.crme_Address2 = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_addressLine2;
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_addressLine1))
                        {
                            response.UDOfindVeteranInfoInfo.crme_Address1 = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_addressLine1;
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_ssn))
                        {
                            response.UDOfindVeteranInfoInfo.crme_SSN = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_ssn;
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_zipCode))
                        {
                            response.UDOfindVeteranInfoInfo.crme_Zip = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_zipCode;
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_country))
                        {
                            response.UDOfindVeteranInfoInfo.crme_Country = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_country;
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_state))
                        {
                            response.UDOfindVeteranInfoInfo.crme_State = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo.mcs_state;
                        }
                    }
                    #endregion

                    #region birlsInfo
                    if (findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo != null)
                    {
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo.mcs_SEX_CODE))
                        {
                            response.UDOfindVeteranInfoInfo.crme_Gender = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo.mcs_SEX_CODE;
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo.mcs_CAUSE_OF_DEATH))
                        {
                            response.UDOfindVeteranInfoInfo.crme_CauseOfDeath = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo.mcs_CAUSE_OF_DEATH;
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo.mcs_CLAIM_FOLDER_LOCATION))
                        {
                            response.UDOfindVeteranInfoInfo.crme_stationofJurisdiction = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo.mcs_CLAIM_FOLDER_LOCATION;
                        }
                        if (!string.IsNullOrEmpty(findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo.mcs_DATE_OF_DEATH))
                        {
                            response.UDOfindVeteranInfoInfo.crme_DeceasedDate = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo.mcs_DATE_OF_DEATH;
                        }
                        if (findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo.VEISvetPctSERVICEInfo != null)
                        {
                            var serviceDTO = findVetByPidResponse.VEISvetPctreturnInfo.VEISvetPctvetBirlsRecordInfo.VEISvetPctSERVICEInfo;
                            var BOS = "";
                            foreach (var item in serviceDTO)
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
                            response.UDOfindVeteranInfoInfo.crme_BranchOfService = BOS;

                            var charOfSvcCode = serviceDTO.OrderByDescending(h => DateTime.TryParse(h.mcs_ENTERED_ON_DUTY_DATE, out DateTime newDateTime)).FirstOrDefault().mcs_CHAR_OF_SVC_CODE;
                            if (!string.IsNullOrEmpty(charOfSvcCode)) response.UDOfindVeteranInfoInfo.crme_CharacterofDishcarge = charOfSvcCode;

                        }
                    }
                    #endregion
                }

                return response;
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOfindVeteranInfoProcessor UDOfindVeteranInfoByPid, Progess:" + progressString, connectException.Message);

                response.ExceptionMessage = "Failed to get Veteran by Particpant ID";
                response.ExceptionOccured = true;
                return response;
            }
        }

        private string LongBranchOfService(string branchcode)
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
	}
}
