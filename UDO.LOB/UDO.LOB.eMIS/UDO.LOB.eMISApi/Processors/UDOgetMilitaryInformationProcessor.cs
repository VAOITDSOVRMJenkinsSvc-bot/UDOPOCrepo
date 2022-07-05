using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using UDO.LOB.Core;
using UDO.LOB.eMIS.Messages;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Messages.MilitaryInfoService;

namespace UDO.LOB.eMIS.Processors
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
                response.ExceptionOccurred = true;
                return response;
            }

            if (String.IsNullOrEmpty(request.udo_ICN) && (String.IsNullOrEmpty(request.udo_EDIPI) || request.udo_EDIPI.Equals("UNK", StringComparison.InvariantCultureIgnoreCase)))
            {
                response.ExceptionOccurred = true;
                response.ExceptionMessage = String.Format("{0}: {1}", FAILUREMESSAGE,
                    "There was no EDIPI or ICN provided to lookup the requested Military Information");
                return response;
            }

            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = "UDOgetMilitaryInformationProcessor",
                    OrganizationName = request.OrganizationName
                };
            }
            TraceLogger tLogger = new TraceLogger(METHOD, request);

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOgetMilitaryInformationRequest>(request)}");

            #region connect to CRM
            CrmServiceClient OrgServiceProxy = null;

            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOgetMilitaryInformationProcessor", connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                var eMISRequest = new VEISgetMilSEgetMilitaryServiceEpisodesRequest
                {
                    Debug = request.Debug,
                    LogSoap = request.LogSoap,
                    LogTiming = request.LogTiming,
                    MessageId = Guid.NewGuid().ToString(),
                    OrganizationName = request.OrganizationName,
                    UserId = request.UserId
                };

                eMISRequest.VEISgetMilSEReqeMISserviceEpisodeRequestInfo = new VEISgetMilSEReqeMISserviceEpisodeRequest()
                {
                    VIMTgetMilSEReqedipiORicnInfo = new VIMTgetMilSEReqedipiORicn()
                };

                var edipioricnInfo = eMISRequest.VEISgetMilSEReqeMISserviceEpisodeRequestInfo.VIMTgetMilSEReqedipiORicnInfo;


                if (String.IsNullOrEmpty(request.udo_EDIPI) || request.udo_EDIPI.Equals("UNK", StringComparison.InvariantCultureIgnoreCase))
                {
                    edipioricnInfo.mcs_edipiORicnValue = request.udo_ICN;
                    edipioricnInfo.VIMTgetMilSEReqinputTypeInfo = VIMTgetMilSEReqinputType.ICN;
                }
                else
                {
                    edipioricnInfo.mcs_edipiORicnValue = request.udo_EDIPI;
                    edipioricnInfo.VIMTgetMilSEReqinputTypeInfo = VIMTgetMilSEReqinputType.EDIPI;
                }

                // REM: Invoke VEIS WebApi
                var eMISResponse = WebApiUtility.SendReceive<VEISgetMilSEgetMilitaryServiceEpisodesResponse>(eMISRequest, WebApiType.VEIS);
                if (request.LogSoap || eMISResponse.ExceptionOccurred)
                {
                    if (eMISResponse.SerializedSOAPRequest != null || eMISResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = eMISResponse.SerializedSOAPRequest + eMISResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISgetMilSEReqeMISserviceEpisodeRequest Request/Response {requestResponse}", true);
                    }
                }

                if (eMISResponse != null && !eMISResponse.ExceptionOccurred &&
                    eMISResponse.VEISgetMilSEeMISserviceEpisodeResponseTypeInfo != null &&
                    eMISResponse.VEISgetMilSEeMISserviceEpisodeResponseTypeInfo.VEISgetMilSEmilitaryServiceEpisodeInfo != null)
                {
                    var episodeResponseInfo = eMISResponse.VEISgetMilSEeMISserviceEpisodeResponseTypeInfo;
                    VEISgetMilSEmilitaryServiceEpisodeMultipleResponse mostRecentEpisode = null;
                    var episodes = new List<VEISgetMilSEmilitaryServiceEpisodeMultipleResponse>();

                    if (episodeResponseInfo.VEISgetMilSEmilitaryServiceEpisodeInfo != null && episodeResponseInfo.VEISgetMilSEmilitaryServiceEpisodeInfo.Length > 0)
                    {
                        episodes.AddRange(episodeResponseInfo.VEISgetMilSEmilitaryServiceEpisodeInfo);
                    }

                    if (request.udo_MostRecentServiceOnly)
                    {
                        if (episodes.Count > 1)
                        {
                            //TODO: fix orderby
                            mostRecentEpisode = episodes.OrderByDescending((a) =>
                            {
                                var info = a.VEISgetMilSEmilitaryServiceEpisodeDataInfo;
                                if (!info.mcs_serviceEpisodeEndDateSpecified) return DateTime.Now;
                                return Convert.ToDateTime(info.mcs_serviceEpisodeEndDate);
                            }).FirstOrDefault();
                        }
                        else if (episodes.Count > 0)
                        {
                            mostRecentEpisode = episodes[0];
                        }
                    }
                    else
                    {
                        var serviceHistory = new List<UDOMilitaryServiceInfo>();
                        foreach (var episode in episodes)
                        {
                            serviceHistory.Add(GetServiceInfo(request.OrganizationName, OrgServiceProxy, episode));
                        }
                    }

                    response.udo_MostRecentService = GetServiceInfo(
                        request.OrganizationName, OrgServiceProxy, mostRecentEpisode);

                }

                // Pass response exception message and data back to the caller.
                if (eMISResponse != null)
                {
                    response.ExceptionOccurred = eMISResponse.ExceptionOccurred;

                    if (eMISResponse.ExceptionOccurred)
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
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOgetMilitaryInformationProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = FAILUREMESSAGE;
                response.ExceptionOccurred = true;
                return response;
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }
        }

        private static UDOMilitaryServiceInfo GetServiceInfo(string orgName, IOrganizationService orgService, VEISgetMilSEmilitaryServiceEpisodeMultipleResponse episode)
        {
            if (episode == null) return null;

            UDOMilitaryServiceInfo serviceInfo = new UDOMilitaryServiceInfo();

            var episodeInfo = episode.VEISgetMilSEmilitaryServiceEpisodeDataInfo;

            if (episodeInfo.mcs_serviceEpisodeStartDateSpecified)
            {
                serviceInfo.StartDate = episodeInfo.mcs_serviceEpisodeStartDate;
            }
            if (episodeInfo.mcs_serviceEpisodeEndDateSpecified)
            {
                serviceInfo.EndDate = episodeInfo.mcs_serviceEpisodeEndDate;
            }

            // Bad data with 0 instead of 0 causes problems.  None of the renk codes should have 0's, but they do have O's.
            // Unfortunately sometimes it is an 0 and not a O, so replace it.
            if (!string.IsNullOrEmpty(episodeInfo.mcs_serviceRankNameCode))
            {
                serviceInfo.RankCode = episodeInfo.mcs_serviceRankNameCode.Replace('0', 'O');
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
            serviceInfo.RankName = UDORankHelper.FindRank(orgName, serviceInfo.BranchCode, serviceInfo.PayGradeCode, serviceInfo.PayPlanCode, serviceInfo.RankCode);

            return serviceInfo;
        }
    }
}