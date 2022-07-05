using Microsoft.Xrm.Sdk.Client;
using System;
using VIMT.IntentToFileWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.IntentToFile.Messages;
using Logger = VRM.Integration.Servicebus.Core.Logger;

namespace VRM.Integration.UDO.IntentToFile.Processors
{
    public class UDOSubmitITFProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOSubmitITFProcessor";
        private string LogBuffer { get; set; }
        public IMessageBase Execute(UDOSubmitITFRequest request)
        {
            //var request = message as createUDOLegacyPaymentDataRequest;
            var response = new UDOSubmitITFResponse();
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            Logger.Instance.Info(string.Format("Message Id:{0}, Type={2}, Recieved diagnostics message: {1}",
            request.MessageId,
            request.MessageId,
            GetType().FullName));

            OrganizationServiceProxy OrgServiceProxy;

            #region connect to CRM
            try
            {
                var CommonFunctions = new CRMConnect();
                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOfindZipCodeProcessor Processor, Connection Error", connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {

                #region - Insert intent to file

                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Execute", "Request - " + request.SerializeToString());

                var fileIntentRequest = new VIMTinsertInt2FileinsertIntentToFileRequest
                {
                    Debug = request.Debug,
                    LogSoap = request.LogSoap,
                    LogTiming = request.LogTiming,
                    OrganizationName = request.OrganizationName,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    LegacyServiceHeaderInfo = new VIMT.IntentToFileWebService.Messages.HeaderInfo()
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    intenttofiledtoInfo = new VIMTinsertInt2Fileintenttofiledto()
                    {
                        mcs_itfTypeCd = request.Claimant.CompensationType,
                        mcs_ptcpntClmantIdSpecified = true,
                        mcs_ptcpntClmantId = Convert.ToInt64(request.Claimant.ClaimantParticipantId),
                        mcs_ptcpntVetIdSpecified = true,
                        mcs_ptcpntVetId = Convert.ToInt64(request.Claimant.VeteranParticipantId),
                        mcs_clmantFirstNm = request.Claimant.ClaimantFirstName,
                        mcs_clmantLastNm = request.Claimant.ClaimantLastName,
                        mcs_clmantMiddleNm = request.Claimant.ClaimantMiddleInitial,
                        mcs_clmantSsn = request.Claimant.ClaimantSsn,
                        mcs_vetFirstNm = request.Claimant.VeteranFirstName,
                        mcs_vetLastNm = request.Claimant.VeteranLastName,
                        mcs_vetMiddleNm = request.Claimant.VeteranMiddleInitial,
                        mcs_vetSsnNbr = request.Claimant.VeteranSsn,
                        mcs_vetFileNbr = request.Claimant.VeteranFileNumber,
                        mcs_genderCd = request.Claimant.VeteranGender,
                        mcs_vetBrthdyDt = ConvertToDateTime(request.Claimant.VeteranBirthDate),
                        mcs_clmantPhoneAreaNbr = request.Claimant.PhoneAreaCode,
                        mcs_clmantPhoneNbr = request.Claimant.Phone,
                        mcs_clmantEmailAddrsTxt = request.Claimant.Email,
                        mcs_clmantAddrsOneTxt = request.Claimant.AddressLine1,
                        mcs_clmantAddrsTwoTxt = request.Claimant.AddressLine2,
                        mcs_clmantAddrsUnitNbr = request.Claimant.AddressLine3,
                        mcs_clmantCityNm = request.Claimant.City,
                        mcs_clmantStateCd = request.Claimant.State,
                        mcs_clmantZipCd = request.Claimant.Zip,
                        mcs_clmantCntryNm = request.Claimant.Country,
                        mcs_jrnLctnId = request.Claimant.StationLocation,
                        mcs_createDt = DateTime.UtcNow,
                        mcs_rcvdDtSpecified = true,
                        mcs_rcvdDt = DateTime.UtcNow,
                        mcs_signtrInd = "Y",
                        mcs_submtrApplcnTypeCd = "CRM"
                    }
                };

                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Execute", "fileIntentRequest - " + fileIntentRequest.SerializeToString());

                var fileIntentResponse = fileIntentRequest.SendReceive<VIMTinsertInt2FileinsertIntentToFileResponse>(MessageProcessType.Local);
                progressString = "After VIMTinsertInt2FileinsertIntentToFileRequest EC Call";

                #endregion

                response.ExceptionOccured = fileIntentResponse.ExceptionOccured;
                response.ExceptionMessage = fileIntentResponse.ExceptionMessage;
                response.MessageId = fileIntentResponse.MessageId;
                response.request = fileIntentRequest.SerializeToString();
                response.response = fileIntentResponse.SerializeToString();

                if (!string.IsNullOrEmpty(fileIntentResponse.VIMTinsertInt2FileintentToFileDTOInfo.mcs_jrnUserId))
                    response.jrnUserId = fileIntentResponse.VIMTinsertInt2FileintentToFileDTOInfo.mcs_jrnUserId;

                if (!string.IsNullOrEmpty(fileIntentResponse.VIMTinsertInt2FileintentToFileDTOInfo.mcs_jrnLctnId))
                    response.jrnLctnId = fileIntentResponse.VIMTinsertInt2FileintentToFileDTOInfo.mcs_jrnLctnId;

                if (fileIntentResponse.VIMTinsertInt2FileintentToFileDTOInfo.mcs_createDtSpecified)
                    response.createDt = fileIntentResponse.VIMTinsertInt2FileintentToFileDTOInfo.mcs_createDt.ToShortDateString();

                if (!string.IsNullOrEmpty(fileIntentResponse.VIMTinsertInt2FileintentToFileDTOInfo.mcs_submtrApplcnTypeCd))
                    response.submtrApplcnTypeCd = fileIntentResponse.VIMTinsertInt2FileintentToFileDTOInfo.mcs_submtrApplcnTypeCd;

                if (fileIntentResponse.VIMTinsertInt2FileintentToFileDTOInfo.mcs_intentToFileIdSpecified)
                    response.intentToFileId = fileIntentResponse.VIMTinsertInt2FileintentToFileDTOInfo.mcs_intentToFileId.ToString();

                response.IntentToFileDto = fileIntentResponse.VIMTinsertInt2FileintentToFileDTOInfo;

            }
            catch (Exception executionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOSubmitITFProcessor Processor, Progess:" + progressString, executionException);
                response.ExceptionMessage = "Failed to process submit ITF";
                response.ExceptionOccured = true;
            }

            return response;
        }

        private long ConvertToLong(string input)
        {
            long rtnInput = 0;

            if (long.TryParse(input, out rtnInput))
            {
                return rtnInput;
            }

            return 0;
        }

        private DateTime ConvertToDateTime(string inputDate)
        {
            DateTime chkDateTime = DateTime.MinValue;

            if (DateTime.TryParse(inputDate, out chkDateTime))
            {
                return chkDateTime;
            }

            return DateTime.MinValue;
        }

    }
}