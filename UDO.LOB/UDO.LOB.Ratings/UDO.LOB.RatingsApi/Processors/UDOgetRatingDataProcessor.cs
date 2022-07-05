using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Ratings.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.RatingService;

/// <summary>
/// LOB Component for UDOUDOcreateSMCRatings,UDOcreateSMCRatings method, Processor.
/// </summary>
namespace UDO.LOB.Ratings.Processors
{
    public class UDOgetRatingDataProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOgetRatingDataProcessor";
        private string LogBuffer { get; set; }
        public IMessageBase Execute(UDOgetRatingDataRequest request)
        {
            UDOgetRatingDataResponse response = new UDOgetRatingDataResponse();
            response.MessageId = request.MessageId;
            if (request.DiagnosticsContext == null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    OrganizationName = request.OrganizationName,
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA",
                };
            }
            TraceLogger tLogger = new TraceLogger(method, request);

            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
                return response;
            }

            try
            {
                var findRatingDataRequest = new VEISfnrtngdtfindRatingDataRequest();
                findRatingDataRequest.MessageId = request.MessageId;
                findRatingDataRequest.LogTiming = request.LogTiming;
                findRatingDataRequest.LogSoap = request.LogSoap;
                findRatingDataRequest.Debug = request.Debug;
                findRatingDataRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findRatingDataRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findRatingDataRequest.RelatedParentId = request.RelatedParentId;
                findRatingDataRequest.UserId = request.UserId;
                findRatingDataRequest.OrganizationName = request.OrganizationName;
                findRatingDataRequest.mcs_filenumber = request.fileNumber;
                findRatingDataRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
                progressString = "Request FN# is: " + findRatingDataRequest.mcs_filenumber + ", Request Org is: " + findRatingDataRequest.OrganizationName + ", Request User ID is: " + findRatingDataRequest.UserId;

                var findRatingDataResponse = WebApiUtility.SendReceive<VEISfnrtngdtfindRatingDataResponse>(findRatingDataRequest, WebApiType.VEIS);
                if (request.LogSoap || findRatingDataResponse.ExceptionOccurred)
                {
                    if (findRatingDataResponse.SerializedSOAPRequest != null || findRatingDataResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findRatingDataResponse.SerializedSOAPRequest + findRatingDataResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfnrtngdtfindRatingDataRequest Request/Response {requestResponse}", true);
                    }
                }
                progressString = "After VEIS EC Call";
                tLogger.LogEvent("Web Service Call Complete VEISfnrtngdtfindRatingDataResponse", "001");

                response.ExceptionMessage = findRatingDataResponse.ExceptionMessage;
                response.ExceptionOccurred = findRatingDataResponse.ExceptionOccurred;
                response.MessageId = request.MessageId;

                progressString = "Beginning Creation of Child Records";
                if (findRatingDataResponse != null)
                {
                    response.ExceptionMessage = findRatingDataResponse.ExceptionMessage;
                    response.ExceptionOccurred = findRatingDataResponse.ExceptionOccurred;

                    if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo != null)
                    {
                        if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo != null)
                        {
                            if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo.VEISVIMTfnrtngdtratings1Info != null)
                            {
                                var disabilityDetails = findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo;
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

                        if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtotherRatingRecordInfo != null)
                        {
                            if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtotherRatingRecordInfo.VEISVIMTfnrtngdtratings3Info != null)
                            {
                                var otherRatings = findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtotherRatingRecordInfo.VEISVIMTfnrtngdtratings3Info;

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
                }


                if (request.vetsnapshotId != Guid.Empty)
                {

                    #region connect to CRM
                    CrmServiceClient OrgServiceProxy = null;

                    try
                    {
                        OrgServiceProxy = ConnectionCache.GetProxy();
                    }
                    catch (Exception connectException)
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, "UDOgetRatingDataProcessor, Connection Error", connectException.Message);
                        response.ExceptionMessage = "Failed to get CRMConnection";
                        response.ExceptionOccurred = true;
                        return response;
                    }
                    #endregion
                    tLogger.LogEvent("Connect to CRM", "002");
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

                    using (OrgServiceProxy)
                    {
                        OrgServiceProxy.Update(newSnapShot);
                    }

                    tLogger.LogEvent("CRM UPDATE: Ratings Updated on udo_veteransnapshot", "003");
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, 
                     method + " Processor, Progress:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Rating Data"; 
                response.ExceptionOccurred = true;
                tLogger.LogException(ExecutionException, "004");
                return response;
            }
        }
    }
}
