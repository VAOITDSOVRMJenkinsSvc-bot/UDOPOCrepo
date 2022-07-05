// using CRM007.CRM.SDK.Core;
using VRM.Integration.UDO.Common;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using System.Linq;
using System;
using System.Collections.Generic;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.eMIS.Messages;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using Logger = VRM.Integration.Servicebus.Core.Logger;
using VIMT.eMISMilitaryInfoService.Messages;
using VRM.Integration.UDO.eMIS.Processor;

namespace VRM.Integration.UDO.eMIS.Processors
{
    class UDOgetMilitaryInformationProcessor
    {
        private bool _debug { get; set; }
        private const string FAILUREMESSAGE = "Failed to get Military Information";
        private const string METHOD = "UDOgetMilitaryInformationProcessor";
        private string LogBuffer { get; set; }

        

        public IMessageBase Execute(UDOgetMilitaryInformationRequest request)
        {
            UDOgetMilitaryInformationResponse response = new UDOgetMilitaryInformationResponse();
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = String.Format("{0}: {1}", FAILUREMESSAGE,
                    "Called with no message");
                response.ExceptionOccured = true;
                return response;
            }

            if (String.IsNullOrEmpty(request.udo_ICN) && (String.IsNullOrEmpty(request.udo_EDIPI) || request.udo_EDIPI.Equals("UNK", StringComparison.InvariantCultureIgnoreCase)))
            {
                response.ExceptionOccured = true;
                response.ExceptionMessage = String.Format("{0}: {1}", FAILUREMESSAGE,
                    "There was no EDIPI or ICN provided to lookup the requested Military Information");
                return response;
            }

            OrganizationServiceProxy OrgServiceProxy;

            #region connect to CRM
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOgetMilitaryInformationProcessor Processor, Connection Error", connectException.Message);
                response.ExceptionMessage = String.Format("{0}: {1}", FAILUREMESSAGE,
                    "Failed to get CRMConnection");
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            try
            {
                var eMISRequest = new VIMTgetMilSEgetMilitaryServiceEpisodesRequest
                {
                    Debug = request.Debug,
                    LogSoap = request.LogSoap,
                    LogTiming = request.LogTiming,
                    MessageId = Guid.NewGuid().ToString(),
                    OrganizationName =request.OrganizationName,
                    UserId = request.UserId
                };

                eMISRequest.mcs_requestInfo = new VIMTgetMilSEmcs_request
                {
                    eMISserviceEpisodeRequestInfo = new VIMTgetMilSEinputEdiPiOrIcn
                    {
                        edipiORicnInfo = new VIMTgetMilSEInputEdipiIcn()
                    }
                };

                var edipioricnInfo = eMISRequest.mcs_requestInfo.eMISserviceEpisodeRequestInfo.edipiORicnInfo;
                edipioricnInfo.mcs_edipiORicnValue = request.udo_EDIPI;
                if (String.IsNullOrEmpty(request.udo_EDIPI) || request.udo_EDIPI.Equals("UNK", StringComparison.InvariantCultureIgnoreCase))
                {
                    edipioricnInfo.mcs_edipiORicnValue = request.udo_ICN;
                }

                var eMISResponse = eMISRequest.SendReceive<VIMTgetMilSEgetMilitaryServiceEpisodesResponse>(MessageProcessType.Local);

                if (eMISResponse != null && !eMISResponse.ExceptionOccured &&
                    eMISResponse.VIMTgetMilSEgetMilitaryServiceEpisodesResponseDataInfo != null &&
                    eMISResponse.VIMTgetMilSEgetMilitaryServiceEpisodesResponseDataInfo.VIMTgetMilSEeMISserviceEpisodeResponseInfo != null)
                {
                    var episodeResponseInfo = eMISResponse.VIMTgetMilSEgetMilitaryServiceEpisodesResponseDataInfo.VIMTgetMilSEeMISserviceEpisodeResponseInfo;
                    VIMTgetMilSEmilitaryServiceEpisodeMultipleResponse mostRecentEpisode = null;
                    var episodes = new List<VIMTgetMilSEmilitaryServiceEpisodeMultipleResponse>();

                    if (episodeResponseInfo.VIMTgetMilSEmilitaryServiceEpisodeInfo != null && episodeResponseInfo.VIMTgetMilSEmilitaryServiceEpisodeInfo.Length > 0)
                    {
                        episodes.AddRange(episodeResponseInfo.VIMTgetMilSEmilitaryServiceEpisodeInfo);
                    }

                    if (request.udo_MostRecentServiceOnly) {
                        if (episodes.Count>1) {
                            mostRecentEpisode = episodes.OrderByDescending((a) =>
                            {
                                var info = a.VIMTgetMilSEmilitaryServiceEpisodeDataInfo;
                                if (!info.mcs_serviceEpisodeEndDateSpecified) return DateTime.Now;
                                return info.mcs_serviceEpisodeEndDate;
                            }).FirstOrDefault();
                        }
                        else if (episodes.Count > 0)
                        {
                            mostRecentEpisode = episodes[0];
                        }
                    } else {
                        var serviceHistory = new List<UDOMilitaryServiceInfo>();
                        foreach(var episode in episodes) {
                            serviceHistory.Add(GetServiceInfo(request.OrganizationName, OrgServiceProxy, episode));
                        }
                    }

                    response.udo_MostRecentService = GetServiceInfo(
                        request.OrganizationName, OrgServiceProxy, mostRecentEpisode);
                  
                }

                // Pass response exception message and data back to the caller.
                if (eMISResponse!=null) {
                    response.ExceptionOccured = eMISResponse.ExceptionOccured;

                    if (eMISResponse.ExceptionOccured)
                    {
                        response.ExceptionMessage = String.Format("{0}: {1}", FAILUREMESSAGE,
                            eMISResponse.ExceptionMessage);
                    }
                    else
                    {
                        response.ExceptionMessage = eMISResponse.ExceptionMessage;
                    }


                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOgetMilitaryInformationProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = FAILUREMESSAGE;
                response.ExceptionOccured = true;
                return response;
            }
        }

        private static UDOMilitaryServiceInfo GetServiceInfo(string orgName, OrganizationServiceProxy orgService, VIMTgetMilSEmilitaryServiceEpisodeMultipleResponse episode)
        {
            if (episode == null) return null;

            UDOMilitaryServiceInfo serviceInfo = new UDOMilitaryServiceInfo();

            var episodeInfo = episode.VIMTgetMilSEmilitaryServiceEpisodeDataInfo;

            if (episodeInfo.mcs_serviceEpisodeStartDateSpecified) {
                serviceInfo.StartDate = episodeInfo.mcs_serviceEpisodeStartDate;
            }
            if (episodeInfo.mcs_serviceEpisodeEndDateSpecified) {
                serviceInfo.EndDate = episodeInfo.mcs_serviceEpisodeEndDate;
            }

            // Bad data with 0 instead of 0 causes problems.  None of the renk codes should have 0's, but they do have O's.
            // Unfortunately sometimes it is an 0 and not a O, so replace it.
            //serviceInfo.RankCode = episodeInfo.mcs_serviceRankNameCode.Replace('0', 'O') ?? string.Empty; 
            
            if (!string.IsNullOrEmpty(episodeInfo.mcs_serviceRankNameCode))
            {
               serviceInfo.RankCode = episodeInfo.mcs_serviceRankNameCode.Replace('0','O'); 
            }
            else
            {
                serviceInfo.RankCode = string.Empty;
            }
            

            if (episodeInfo.mcs_payGradeDateSpecified)
            {
                serviceInfo.PayGradeCode = episodeInfo.mcs_payGradeCode;
            }
            else
            {
                serviceInfo.PayGradeCode = string.Empty;
            }

            serviceInfo.BranchCode = episodeInfo.mcs_branchOfServiceCode ?? string.Empty;

            serviceInfo.PayPlanCode = episodeInfo.mcs_payPlanCode ?? string.Empty;

            serviceInfo.RankName = UDORankHelper.FindRank(orgName, orgService, serviceInfo.BranchCode, serviceInfo.PayGradeCode, serviceInfo.PayPlanCode, serviceInfo.RankCode);

            return serviceInfo;
        }
    }
}
