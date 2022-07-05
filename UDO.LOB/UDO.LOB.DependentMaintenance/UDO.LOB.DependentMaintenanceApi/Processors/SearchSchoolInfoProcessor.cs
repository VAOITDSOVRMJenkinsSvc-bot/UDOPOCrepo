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
//using VEIS.Messages.DdeftWebService;
using VEIS.Messages.EBenefitEducationService;
using UDO.LOB.Extensions;

namespace UDO.LOB.DependentMaintenance.Processors
{
    public class SearchSchoolInfoProcessor
    {
        private const string method = "SearchSchoolInfoProcessor";

        public LOB.Core.IMessageBase Execute(SearchSchoolInfoRequest message)
        {
            LogHelper.LogInfo("Calling SearchSchoolInfoProcessor");

            var progressString = $">> Entered {method} ";

            var response = new SearchSchoolInfoResponse();


            try {
                var searchEduInstitutesRequest = new VEISsrcheduinstSearchEduInstitutesRequest
                {
                    MessageId = message.MessageId,
                    OrganizationName = message.OrganizationName,
                    UserId = message.UserId,
                    LogTiming = message.LogTiming,
                    LogSoap = message.LogSoap,
                    Debug = message.Debug,
                    //RelatedParentEntityName = message.RelatedParentEntityName,
                   // RelatedParentFieldName = message.RelatedParentFieldName,
                    //RelatedParentId = message.RelatedParentId,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = message.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = message.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = message.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = message.LegacyServiceHeaderInfo.StationNumber
                    },
                    searcheduinstitutessearchstringInfo = new VEISsrcheduinstsearcheduinstitutessearchstring
                    {
                        mcs_instituteName = message.searcheduinstitutessearchstringInfo.mcs_instituteName,
                        mcs_facilityCode = message.searcheduinstitutessearchstringInfo.mcs_facilityCode
                    },
                    edustateInfo = new VEISsrcheduinstedustate
                    {
                        mcs_stateCodeORForeignCountry = message.edustateInfo.mcs_stateCodeORForeignCountry,
                        mcs_stateNumber = message.edustateInfo.mcs_stateNumber
                    }
                };

                // REM: Invoke VEIS Endpoint
                var searchEduInstitutesResponse = WebApiUtility.SendReceive<UdoEcSearchEduInstitutesResponse>(searchEduInstitutesRequest, WebApiType.VEIS);

                if (searchEduInstitutesRequest.LogSoap || searchEduInstitutesResponse.ExceptionOccurred)
                {
                    if (searchEduInstitutesResponse.SerializedSOAPRequest != null || searchEduInstitutesResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = searchEduInstitutesResponse.SerializedSOAPRequest + searchEduInstitutesResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(searchEduInstitutesRequest.MessageId, searchEduInstitutesRequest.OrganizationName, searchEduInstitutesRequest.UserId, MethodInfo.GetThisMethod().Method, $"VEISsrcheduinstSearchEduInstitutesRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString += " >> After searchEduInstitutesResponse EC Call";

                response.ExceptionOccured = searchEduInstitutesResponse.ExceptionOccured;
                //response.VEISsrcheduinststatusInfo = searchEduInstitutesResponse.VEISsrcheduinststatusInfo;

                // Replaced: VIMTbyRoutingTransitionNumberInfo = VEISbyRoutingTransitionNumberreturnInfo
                if (searchEduInstitutesResponse.VEISsrcheduinststatusInfo != null && searchEduInstitutesResponse.VEISsrcheduinststatusInfo.Length > 0)
                {
                    for (int i = 0; i < searchEduInstitutesResponse.VEISsrcheduinststatusInfo.Length; i++)
                    {
                        response.facilityCode.Add(searchEduInstitutesResponse.VEISsrcheduinststatusInfo[i].mcs_facilityCode);
                        response.participantID.Add(searchEduInstitutesResponse.VEISsrcheduinststatusInfo[i].mcs_participantID);
                        
                    }
                    //Messages.VEISsrcheduinststatusMultipleResponse multipleResponse;
                    //for (int i=0; i<searchEduInstitutesResponse.VEISsrcheduinststatusInfo.Length; i++)
                    //{
                    //    multipleResponse = new Messages.VEISsrcheduinststatusMultipleResponse();
                    //    multipleResponse.ParticipantID = searchEduInstitutesResponse.VEISsrcheduinststatusInfo[i].mcs_participantID;
                    //    multipleResponse.FacilityCode = searchEduInstitutesResponse.VEISsrcheduinststatusInfo[i].mcs_facilityCode;
                    //    multipleResponse.InstituteName = searchEduInstitutesResponse.VEISsrcheduinststatusInfo[i].mcs_instituteName;
                    //    multipleResponse.StatusDate = searchEduInstitutesResponse.VEISsrcheduinststatusInfo[i].mcs_statusDate;

                        //    response.VEISsrcheduinststatusInfo[i] = multipleResponse;   
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