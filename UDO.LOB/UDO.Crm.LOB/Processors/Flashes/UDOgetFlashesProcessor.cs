using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIMT.ClaimantWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Flashes.Messages;

namespace VRM.Integration.UDO.Flashes.Processors
{
    class UDOgetFlashesProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOgetFlashesProcessor";
        private string LogBuffer { get; set; }
        private List<UDOFlashItem> flashes = new List<UDOFlashItem>();
        private string combinedFlashes = string.Empty;
        private UDOgetFlashesResponse response = new UDOgetFlashesResponse();

        public IMessageBase Execute(UDOgetFlashesRequest request)
        {


            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }
            
            try
            {
                if (!string.IsNullOrEmpty(request.ptcpntVetId))
                {
                    var findFlashesResponse = FindGeneralInfoByPtcpntIds(request);

                    response.ExceptionMessage = findFlashesResponse.ExceptionMessage;
                    response.ExceptionOccured = findFlashesResponse.ExceptionOccured;

                    if (findFlashesResponse.VIMTfgenpidreturnclmsInfo != null)
                    {
                        if (findFlashesResponse.VIMTfgenpidreturnclmsInfo.VIMTfgenpidflashesclmsInfo != null)
                        {
                            var flash = findFlashesResponse.VIMTfgenpidreturnclmsInfo.VIMTfgenpidflashesclmsInfo;

                            ///Map Flashes
                            MapFlashesFromPidResponse(flash, request);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(request.fileNumber))
                {
                    var findFlashesResponse = FindGeneralInfoByFileNumber(request);

                    response.ExceptionMessage = findFlashesResponse.ExceptionMessage;
                    response.ExceptionOccured = findFlashesResponse.ExceptionOccured;

                    if (findFlashesResponse.VIMTfgenFNreturnclmsInfo != null)
                    {
                        if (findFlashesResponse.VIMTfgenFNreturnclmsInfo.VIMTfgenFNflashesclmsInfo != null)
                        {
                            var flash = findFlashesResponse.VIMTfgenFNreturnclmsInfo.VIMTfgenFNflashesclmsInfo;

                            ///Map Flashes
                            MapFlashesFromFNResponse(flash, request);
                        }
                    }
                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateFlashesProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Map EC data to LOB";
                response.ExceptionOccured = true;
                return response;
            }
        }

        private void MapFlashesFromPidResponse(VIMTfgenpidflashesclmsMultipleResponse[] flash, UDOgetFlashesRequest request)
        {
            var numOfFlashes = 0;

            foreach (var flashItem in flash)
            {
                if (numOfFlashes > 0)
                {
                    combinedFlashes += " : " + flashItem.mcs_flashName.Trim();
                }
                else
                {
                    combinedFlashes = flashItem.mcs_flashName.Trim();
                }
                numOfFlashes += 1;

                flashes.Add(new UDOFlashItem() { FlashText = flashItem.mcs_flashName.Trim(), FlashType = flashItem.mcs_flashType.Trim() });
            }

            response.combinedFlashes = combinedFlashes;
            response.flashes = flashes.ToArray();

            #region Log Results
            string logInfo = string.Format("Flash Records Retrieved: {0}", numOfFlashes);
            LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Flash Records Retrieved", logInfo);
            #endregion

        }

        private VIMTfgenpidfindGeneralInformationByPtcpntIdsResponse FindGeneralInfoByPtcpntIds(UDOgetFlashesRequest request)
        {
            // prefix = fflfindFlashesRequest();
            var findFlashesRequest = new VIMTfgenpidfindGeneralInformationByPtcpntIdsRequest();
            findFlashesRequest.LogTiming = request.LogTiming;
            findFlashesRequest.LogSoap = request.LogSoap;
            findFlashesRequest.Debug = request.Debug;
            findFlashesRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findFlashesRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findFlashesRequest.RelatedParentId = request.RelatedParentId;
            findFlashesRequest.UserId = request.UserId;
            findFlashesRequest.OrganizationName = request.OrganizationName;
            if (request.LegacyServiceHeaderInfo != null)
            {
                findFlashesRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }
            //findFlashesRequest.mcs_filenumber = request.fileNumber;
            findFlashesRequest.mcs_ptcpntvetid = request.ptcpntVetId;
            findFlashesRequest.mcs_ptcpntbeneid = request.ptcpntBeneId;
            findFlashesRequest.mcs_ptpcntrecipid = request.ptpcntRecipId;
            //findFlashesRequest.mcs_awardtypecd = request.awardTypeCd;
            var findFlashesResponse = new VIMTfgenpidfindGeneralInformationByPtcpntIdsResponse();

            return findFlashesRequest.SendReceive<VIMTfgenpidfindGeneralInformationByPtcpntIdsResponse>(MessageProcessType.Local);
        }

        private void MapFlashesFromFNResponse(VIMTfgenFNflashesclmsMultipleResponse[] flash, UDOgetFlashesRequest request)
        {
            var numOfFlashes = 0;

            foreach (var flashItem in flash)
            {
                if (numOfFlashes > 0)
                {
                    combinedFlashes += " : " + flashItem.mcs_flashName.Trim();
                }
                else
                {
                    combinedFlashes = flashItem.mcs_flashName.Trim();
                }
                numOfFlashes += 1;

                flashes.Add(new UDOFlashItem() { FlashText = flashItem.mcs_flashName.Trim(), FlashType = flashItem.mcs_flashType.Trim() });
            }

            response.combinedFlashes = combinedFlashes;
            response.flashes = flashes.ToArray();

            #region Log Results
            string logInfo = string.Format("Flash Records Retrieved: {0}", numOfFlashes);
            LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Flash Records Retrieved", logInfo);
            #endregion
        }

        private VIMTfgenFNfindGeneralInformationByFileNumberResponse FindGeneralInfoByFileNumber(UDOgetFlashesRequest request)
        {
            // prefix = fflfindFlashesRequest();
            var findFlashesRequest = new VIMTfgenFNfindGeneralInformationByFileNumberRequest();
            findFlashesRequest.LogTiming = request.LogTiming;
            findFlashesRequest.LogSoap = request.LogSoap;
            findFlashesRequest.Debug = request.Debug;
            findFlashesRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findFlashesRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findFlashesRequest.RelatedParentId = request.RelatedParentId;
            findFlashesRequest.UserId = request.UserId;
            findFlashesRequest.OrganizationName = request.OrganizationName;
            if (request.LegacyServiceHeaderInfo != null)
            {
                findFlashesRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }
            findFlashesRequest.mcs_filenumber = request.fileNumber;

            //findFlashesRequest.mcs_awardtypecd = request.awardTypeCd;
            var findFlashesResponse = new VIMTfgenFNfindGeneralInformationByFileNumberResponse();

            // LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateFlashesProcessor ","Didn't get response passed in");
            return findFlashesRequest.SendReceive<VIMTfgenFNfindGeneralInformationByFileNumberResponse>(MessageProcessType.Local);
        }
    }
}
