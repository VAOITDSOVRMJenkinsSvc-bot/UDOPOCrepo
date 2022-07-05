using System;
using System.Collections.Generic;
using CuttingEdge.Conditions;
using UDO.LOB.DependentMaintenance.Messages;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.AddDependent.Util;
using UDO.LOB.Core;
using VEIS.Core.Wcf;
using Microsoft.Xrm.Sdk;
using VEIS.Core.Messages;
using VEIS.Messages.DdeftWebService;
using VEIS.Messages.EBenefitEducationService;
using UDO.LOB.Extensions;

namespace UDO.LOB.DependentMaintenance.Processors
{
    public class GetSchoolInfoProcessor
    {
        private const string method = "GetchoolInfoProcessor";
        public LOB.Core.IMessageBase Execute(GetSchoolInfoRequest message)
        {
            LogHelper.LogInfo("Calling GetSchoolInfoProcessor");

            var progressString = $">> Entered {method} ";

            var response = new GetSchoolInfoResponse();


            try
            {
                var getEduInstitutesRequest = new VEISgteduinstdtlGetEduInstituteDetailRequest
                {
                    LogTiming = message.LogTiming,
                    LogSoap = message.LogSoap,
                    Debug = message.Debug,
                    RelatedParentEntityName = message.RelatedParentEntityName,
                    RelatedParentFieldName = message.RelatedParentFieldName,
                    RelatedParentId = message.RelatedParentId,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = message.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = message.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = message.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = message.LegacyServiceHeaderInfo.StationNumber
                    },
                    mcs_fullFacilityCode = message.mcs_fullFacilityCode
                };

                // REM: Invoke VEIS Endpoint
                var getEduInstitutesResponse = WebApiUtility.SendReceive<UdoEcGetEduInstitueResponse>(getEduInstitutesRequest, WebApiType.VEIS);

                if (getEduInstitutesRequest.LogSoap || getEduInstitutesResponse.ExceptionOccurred)
                {
                    if (getEduInstitutesResponse.SerializedSOAPRequest != null || getEduInstitutesResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = getEduInstitutesResponse.SerializedSOAPRequest + getEduInstitutesResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(getEduInstitutesRequest.MessageId, getEduInstitutesRequest.OrganizationName, getEduInstitutesRequest.UserId, MethodInfo.GetThisMethod().Method, $"VEISgteduinstdtlGetEduInstituteDetailRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString += " >> After getEduInstitutesResponse EC Call";

                response.ExceptionOccured = getEduInstitutesResponse.ExceptionOccured;

                //continue to code here!
                if (getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo != null)
                {
                    response.VEISgteduinstdtleduInstituteInfo.mcs_facilityCode = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_facilityCode;
                    response.VEISgteduinstdtleduInstituteInfo.mcs_participantID = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_participantID;
                    response.VEISgteduinstdtleduInstituteInfo.mcs_instituteName = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_instituteName;
                    //response.VEISgteduinstdtleduInstituteInfo.mcs_status = (Messages.VEISgteduinstdtlStatusTypeCode)getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_status;
                   // response.VEISgteduinstdtleduInstituteInfo.mcs_statusDate = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_statusDate;
                    response.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo = new Messages.VEISgteduinstdtladdress
                    {
                        mcs_addressType = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_addressType,
                        mcs_addressLine1 = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_addressLine1,
                        mcs_addressLine2 = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_addressLine2,
                        mcs_addressLine3 = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_addressLine3,
                        mcs_city = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_city,
                        mcs_county = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_county,
                        mcs_state = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_state,
                        mcs_zipcode = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_zipcode,
                        mcs_zipcodeSuffix = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_zipcodeSuffix,
                        mcs_emailAddress = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_emailAddress,
                        mcs_militaryPostalTypeCodeSpecified = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_militaryPostalTypeCodeSpecified,
                        mcs_militaryPostOfficeTypeCodeSpecified =  getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_militaryPostOfficeTypeCodeSpecified,
                        mcs_foreignPostalCode = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_foreignPostalCode,
                        mcs_province = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_province,
                        mcs_country = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_country,
                        mcs_effectiveDate = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_effectiveDate,
                        mcs_endDate= getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_endDate,
                        mcs_militaryPostOfficeTypeCode = (Messages.VEISgteduinstdtlMilitaryPostOfficeTypeCode)getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_militaryPostOfficeTypeCode,
                        mcs_militaryPostalTypeCode = (Messages.VEISgteduinstdtlMilitaryPostalTypeCode)getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_militaryPostalTypeCode
                    };
                   
                    //switch (getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_status)
                    //{
                    //    case VEIS.Messages.EBenefitEducationService.VEISgteduinstdtlStatusTypeCode.Approved:
                    //        response.VEISgteduinstdtleduInstituteInfo.mcs_status = Messages.VEISgteduinstdtlStatusTypeCode.Approved;
                    //        break;
                    //    case VEIS.Messages.EBenefitEducationService.VEISgteduinstdtlStatusTypeCode.Created:
                    //        response.VEISgteduinstdtleduInstituteInfo.mcs_status = Messages.VEISgteduinstdtlStatusTypeCode.Created;
                    //        break;
                    //    case VEIS.Messages.EBenefitEducationService.VEISgteduinstdtlStatusTypeCode.Suspended:
                    //        response.VEISgteduinstdtleduInstituteInfo.mcs_status = Messages.VEISgteduinstdtlStatusTypeCode.Suspended;
                    //        break;
                    //    case VEIS.Messages.EBenefitEducationService.VEISgteduinstdtlStatusTypeCode.Withdrawn:
                    //        response.VEISgteduinstdtleduInstituteInfo.mcs_status = Messages.VEISgteduinstdtlStatusTypeCode.Withdrawn;
                    //        break;
                    //}
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(message.MessageId, message.OrganizationName, message.UserId, message.RelatedParentId, message.RelatedParentEntityName, message.RelatedParentFieldName,
                    method, ExecutionException);
                response.ExceptionOccured = true;
                return response;
            }
        }
    }
}