using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using VIMT.RatingWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Ratings.Messages;
using VRM.Integration.UDO.Common;

/// <summary>
/// VIMT LOB Component for UDOUDOcreateSMCRatings,UDOcreateSMCRatings method, Processor.
/// Code Generated by IMS on: 6/12/2015 3:18:51 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.Ratings.Processors
{
    public class UDOgetRatingDataProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOgetRatingDataProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOgetRatingDataRequest request)
        {
            //var request = message as UDOcreateSMCRatingsRequest;
            UDOgetRatingDataResponse response = new UDOgetRatingDataResponse();
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
                // prefix = fnrtngdtfindRatingDataRequest();
                var findRatingDataRequest = new VIMTfnrtngdtfindRatingDataRequest();
                findRatingDataRequest.LogTiming = request.LogTiming;
                findRatingDataRequest.LogSoap = request.LogSoap;
                findRatingDataRequest.Debug = request.Debug;
                findRatingDataRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findRatingDataRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findRatingDataRequest.RelatedParentId = request.RelatedParentId;
                findRatingDataRequest.UserId = request.UserId;
                findRatingDataRequest.OrganizationName = request.OrganizationName;
                findRatingDataRequest.mcs_filenumber = request.fileNumber;
                findRatingDataRequest.LegacyServiceHeaderInfo = new VIMT.RatingWebService.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
                progressString = "Request FN# is: " + findRatingDataRequest.mcs_filenumber + ", Request Org is: " + findRatingDataRequest.OrganizationName + ", Request User ID is: " + findRatingDataRequest.UserId;

                var findRatingDataResponse = findRatingDataRequest.SendReceive<VIMTfnrtngdtfindRatingDataResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = findRatingDataResponse.ExceptionMessage;
                response.ExceptionOccured = findRatingDataResponse.ExceptionOccured;

                progressString = "Beginning Creation of Child Records";
                if (findRatingDataResponse != null)
                {
                    response.ExceptionMessage = findRatingDataResponse.ExceptionMessage;
                    response.ExceptionOccured = findRatingDataResponse.ExceptionOccured;

                    if (findRatingDataResponse.VIMTfnrtngdtInfo != null)
                    {
                        //var responseIds = new UDOfindRatingsResponse();
                        //var requestCollection = new OrganizationRequestCollection();
                        if (findRatingDataResponse.VIMTfnrtngdtInfo.VIMTfnrtngdtdisabilityRatingRecordInfo != null)
                        {
                            if (findRatingDataResponse.VIMTfnrtngdtInfo.VIMTfnrtngdtdisabilityRatingRecordInfo.VIMTfnrtngdtratings1Info != null)
                            {
                                var disabilityDetails = findRatingDataResponse.VIMTfnrtngdtInfo.VIMTfnrtngdtdisabilityRatingRecordInfo;
                                if (!String.IsNullOrEmpty(disabilityDetails.mcs_nonServiceConnectedCombinedDegree))
                                {
                                    response.nsccombined = disabilityDetails.mcs_nonServiceConnectedCombinedDegree;
                                }
                                if (!String.IsNullOrEmpty(disabilityDetails.mcs_serviceConnectedCombinedDegree))
                                {
                                    response.sccombined = disabilityDetails.mcs_serviceConnectedCombinedDegree;
                                }
                            }
                        }
                        if (findRatingDataResponse.VIMTfnrtngdtInfo.VIMTfnrtngdtotherRatingRecordInfo.VIMTfnrtngdtratings3Info != null)
                        {
                            var otherRatings = findRatingDataResponse.VIMTfnrtngdtInfo.VIMTfnrtngdtotherRatingRecordInfo.VIMTfnrtngdtratings3Info;

                            foreach (var rating in otherRatings)
                            {
                                if (!String.IsNullOrEmpty(rating.mcs_disabilityTypeName))
                                {
                                    response.disability = rating.mcs_disabilityTypeName;
                                }
                            }
                        }
                    }
                }


                if (request.vetsnapshotId != Guid.Empty)
                {

                    OrganizationServiceProxy OrgServiceProxy;

                    #region connect to CRM
                    try
                    {
                        var CommonFunctions = new CRMConnect();

                        OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

                    }
                    catch (Exception connectException)
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, "UDOgetRatingDataProcessor, Connection Error", connectException.Message);
                        response.ExceptionMessage = "Failed to get CRMConnection";
                        response.ExceptionOccured = true;
                        return response;
                    }
                    #endregion

                    var newSnapShot = new Entity { LogicalName = "udo_veteransnapshot" };
                    newSnapShot.Id = request.vetsnapshotId;

                    newSnapShot["udo_nsccombineddegree"] = response.nsccombined;
                    newSnapShot["udo_sccombinedrating"] = response.sccombined;
                    if (response.disability == "Incompetent")
                    {
                        newSnapShot["udo_cfidstatus"] = "Incompetent";
                    }

                    //RC NEW - this belongs here
                    newSnapShot["udo_ratingscomplete"] = true;

                    OrgServiceProxy.Update(newSnapShot);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Rating Data"; 
                response.ExceptionOccured = true;
                return response;
            }
        }
    }
}
