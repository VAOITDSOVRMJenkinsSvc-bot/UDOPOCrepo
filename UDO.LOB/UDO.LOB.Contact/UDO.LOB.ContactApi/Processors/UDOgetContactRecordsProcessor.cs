using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UDO.LOB.Contact.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Core.Messages;
using VEIS.Messages.ClaimantService;
using VEIS.Messages.EBenefitsAccountActivity;
using VEIS.Messages.VeteranWebService;

/// <summary>
/// UDO LOB Component for UDOgetContactRecords,getContactRecords method, Processor.
/// </summary>
namespace UDO.LOB.Contact.Processors
{
    class UDOgetContactRecordsProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOgetContactRecordsProcessor";

        private static TraceLogger tLogger;

        public IMessageBase Execute(UDOgetContactRecordsRequest request)
        {
            Stopwatch txnTimer = Stopwatch.StartNew();
            UDOgetContactRecordsResponse response = new UDOgetContactRecordsResponse { MessageId = request?.MessageId };

            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };

                response.DiagnosticsContext = request.DiagnosticsContext;
            }

            tLogger = new TraceLogger(method, request);

            txnTimer.Stop();

            LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "Contact Connect", null, txnTimer.ElapsedMilliseconds);
            progressString = "After Connection";

            try
            {
                txnTimer.Start();

                response.UDOgetContactRecordsInfo = new UDOgetContactRecords();

                if (request.performBIRLSCall)
                {
                    Task[] taskArray = {
                                        Task.Factory.StartNew(() =>  progressString = getCorpData(request, response, progressString)),
                                        Task.Factory.StartNew(() =>  progressString = getBirlsData(request, response, ref progressString)),
                                   };

                    Task.WaitAll(taskArray);
                }
                else
                {
                    progressString = getCorpData(request, response, progressString);

                }

                txnTimer.Stop();

                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, Guid.Empty, null, null, "Contact LOB", null, txnTimer.ElapsedMilliseconds);

                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, $"Response: {Environment.NewLine}{JsonHelper.Serialize<UDOgetContactRecordsResponse>(response)}{Environment.NewLine}");
                return response;
            }
            catch (Exception ExecutionException)
            {
                var eventMetrics = new System.Collections.Generic.Dictionary<string, double>
                {
                    { "ElapsedTime", double.Parse(txnTimer.ElapsedMilliseconds.ToString()) }
                };
                tLogger.LogException(ExecutionException, "999", eventMetrics);
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method, ExecutionException);
                response.ExceptionMessage = $"Failed to process contact related records. MessageId: {request.MessageId}";
                response.ExceptionOccured = true;
                return response;
            }
        }
        private static string getFlashes(UDOgetContactRecordsRequest request, UDOgetContactRecordsResponse response, string progressString)
        {
            try
            {

                DateTime myNow = DateTime.Now;
                var extendedTIWEnd = myNow.Hour.ToString("00") + ":" + myNow.Minute.ToString("00") + ":" + myNow.Second.ToString("00") + ":" + myNow.Millisecond.ToString("000");

                Stopwatch txnTimerconn = Stopwatch.StartNew();

                // Replaced: VIMTfgenpidfindGeneralInformationByPtcpntIdsRequest = VEISfgenpidfindGeneralInformationByPtcpntIdRequest
                var findFlashesRequest = new VEISfgenpidfindGeneralInformationByPtcpntIdsRequest();
                findFlashesRequest.MessageId = request.MessageId;
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
                    findFlashesRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                // Replace: VEIS Dependency missing
                findFlashesRequest.mcs_ptcpntvetid = request.ptcpntVetId;
                findFlashesRequest.mcs_ptcpntbeneid = request.ptcpntBeneId;
                findFlashesRequest.mcs_ptpcntrecipid = request.ptpcntRecipId;

                return progressString;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "getFlashes Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to getFlashes";
                response.ExceptionOccured = true;
                return progressString;
            }
        }

        // Removed 'static' form this method
        private string getPOAFIDData(UDOgetContactRecordsRequest request, UDOgetContactRecordsResponse response, string progressString)
        {
            try
            {

                DateTime myNow = DateTime.Now;
                var extendedTIWEnd = myNow.Hour.ToString("00") + ":" + myNow.Minute.ToString("00") + ":" + myNow.Second.ToString("00") + ":" + myNow.Millisecond.ToString("000");

                Stopwatch txnTimerconn = Stopwatch.StartNew();

                //if this doesn't contain anything, don't go asking for it!
                if (!string.IsNullOrEmpty(request.fileNumber))
                {
                    var findAllFiduciaryPoaRequest = new VEISafidpoafindAllFiduciaryPoaRequest();
                    findAllFiduciaryPoaRequest.MessageId = request.MessageId;
                    findAllFiduciaryPoaRequest.LogTiming = request.LogTiming;
                    findAllFiduciaryPoaRequest.LogSoap = request.LogSoap;
                    findAllFiduciaryPoaRequest.Debug = request.Debug;
                    findAllFiduciaryPoaRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                    findAllFiduciaryPoaRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                    findAllFiduciaryPoaRequest.RelatedParentId = request.RelatedParentId;
                    findAllFiduciaryPoaRequest.UserId = request.UserId;
                    findAllFiduciaryPoaRequest.OrganizationName = request.OrganizationName;

                    //non standard fields
                    findAllFiduciaryPoaRequest.mcs_filenumber = request.fileNumber;
                    if (request.LegacyServiceHeaderInfo != null)
                    {
                        findAllFiduciaryPoaRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                    }

                    var findAllFiduciaryPoaResponse = WebApiUtility.SendReceive<VEISafidpoafindAllFiduciaryPoaResponse>(findAllFiduciaryPoaRequest, WebApiType.VEIS);
                    if (request.LogSoap || findAllFiduciaryPoaResponse.ExceptionOccurred)
                    {
                        if (findAllFiduciaryPoaResponse.SerializedSOAPRequest != null || findAllFiduciaryPoaResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = findAllFiduciaryPoaResponse.SerializedSOAPRequest + findAllFiduciaryPoaResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISafidpoafindAllFiduciaryPoaRequest Request/Response {requestResponse}", true);
                        }
                    }

                    progressString = "After VEIS EC Call";
                    LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, Guid.Empty, null, null, "Contact:Back from findfidpoa", null, txnTimerconn.ElapsedMilliseconds);

                    response.ExceptionMessage+= $"{Environment.NewLine}POA/FID ERROR: {findAllFiduciaryPoaResponse.ExceptionMessage}";
                    response.ExceptionOccured = findAllFiduciaryPoaResponse.ExceptionOccurred | response.ExceptionOccured;
                    // Replaced: VIMTafidpoareturnclmsInfo = VEISafidpoareturnInfo
                    if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo != null)
                    {
                        var currentFiduciary = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentFiduciaryInfo;
                        if (currentFiduciary != null)
                        {
                            #region map current FID data

                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_personOrgName))
                            {
                                response.UDOgetContactRecordsInfo.udo_cfid2PersonOrgName =
                                response.UDOgetContactRecordsInfo.udo_FiduciaryAppointed =
                                currentFiduciary.mcs_personOrgName.TrimWhiteSpace();
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_prepositionalPhraseName))
                            {
                                response.UDOgetContactRecordsInfo.udo_cfidPrepositionalPhrase = currentFiduciary.mcs_prepositionalPhraseName;
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_personOrganizationName))
                            {
                                response.UDOgetContactRecordsInfo.udo_personOrganizationName = currentFiduciary.mcs_personOrganizationName.TrimWhiteSpace();
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_beginDate))
                            {
                                DateTime newDateTime;
                                if (DateTime.TryParse(currentFiduciary.mcs_beginDate, out newDateTime))
                                {
                                    response.UDOgetContactRecordsInfo.udo_cfidBeginDate = currentFiduciary.mcs_beginDate;
                                }
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_endDate))
                            {
                                DateTime newDateTime;
                                if (DateTime.TryParse(currentFiduciary.mcs_endDate, out newDateTime))
                                {
                                    response.UDOgetContactRecordsInfo.udo_cfidEndDate = currentFiduciary.mcs_endDate;
                                }
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_eventDate))
                            {
                                DateTime newDateTime;
                                if (DateTime.TryParse(currentFiduciary.mcs_eventDate, out newDateTime))
                                {
                                    response.UDOgetContactRecordsInfo.udo_cfidEventDate = currentFiduciary.mcs_eventDate;
                                }
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_healthcareProviderReleaseIndicator))
                            {
                                //Valide N is correct for this IND field
                                var thisValue = currentFiduciary.mcs_healthcareProviderReleaseIndicator;

                                if (thisValue == "N")
                                {
                                    response.UDOgetContactRecordsInfo.udo_cfidHCProviderRelease = false;
                                }
                                else
                                {
                                    response.UDOgetContactRecordsInfo.udo_cfidHCProviderRelease = true;
                                }
                                response.UDOgetContactRecordsInfo.udo_cfidHCProviderReleaseSpecified = true;

                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_journalDate))
                            {
                                DateTime newDateTime;
                                if (DateTime.TryParse(currentFiduciary.mcs_journalDate, out newDateTime))
                                {
                                    response.UDOgetContactRecordsInfo.udo_cfidJrnDate = currentFiduciary.mcs_journalDate;
                                }
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_journalLocationID))
                            {
                                response.UDOgetContactRecordsInfo.udo_cfidJrnLocID = currentFiduciary.mcs_journalLocationID;
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_journalObjectID))
                            {
                                response.UDOgetContactRecordsInfo.udo_cfidJrnObjID = currentFiduciary.mcs_journalObjectID;
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_journalStatusTypeCode))
                            {
                                response.UDOgetContactRecordsInfo.udo_cfidJrnStatusType = currentFiduciary.mcs_journalStatusTypeCode;
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_personOrgAttentionText))
                            {
                                response.UDOgetContactRecordsInfo.udo_cfidPersonOrgAttn = currentFiduciary.mcs_personOrgAttentionText;
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_personOrganizationCode))
                            {
                                response.UDOgetContactRecordsInfo.udo_cfidPersonOrgCode = currentFiduciary.mcs_personOrganizationCode;
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_personOrgName))
                            {
                                response.UDOgetContactRecordsInfo.udo_cfidPersonOrgName = currentFiduciary.mcs_personOrgName.TrimWhiteSpace();
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_personOrgPtcpntID))
                            {
                                response.UDOgetContactRecordsInfo.udo_cfidPersonOrgParticipantID = currentFiduciary.mcs_personOrgPtcpntID;
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_personOrOrganizationIndicator))
                            {
                                //Valide N is correct for this IND field
                                var thisValue = currentFiduciary.mcs_personOrOrganizationIndicator;

                                if (thisValue == "O")
                                {
                                    response.UDOgetContactRecordsInfo.udo_cfidPersonorOrg = false;
                                }
                                else
                                {
                                    response.UDOgetContactRecordsInfo.udo_cfidPersonorOrg = true;
                                }
                                response.UDOgetContactRecordsInfo.udo_cfidPersonorOrgSpecified = true;
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_rateName))
                            {
                                response.UDOgetContactRecordsInfo.udo_cfidRateName = currentFiduciary.mcs_rateName;
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_relationshipName))
                            {
                                response.UDOgetContactRecordsInfo.udo_cfidRelationship = currentFiduciary.mcs_relationshipName;
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_statusCode))
                            {
                                response.UDOgetContactRecordsInfo.udo_cfidStatus = currentFiduciary.mcs_statusCode;
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_temporaryCustodianIndicator))
                            {
                                response.UDOgetContactRecordsInfo.udo_cfidTempCustodian = currentFiduciary.mcs_temporaryCustodianIndicator;
                            }
                            if (!string.IsNullOrEmpty(currentFiduciary.mcs_veteranPtcpntID))
                            {
                                response.UDOgetContactRecordsInfo.udo_cfidVetPtcpntID = currentFiduciary.mcs_veteranPtcpntID;
                            }
                            #endregion
                        }

                        LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, Guid.Empty, null, null, "Contact:Done with FID", null, txnTimerconn.ElapsedMilliseconds);

                        if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo != null)
                        {
                            #region map current POA data

                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_beginDate))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaBeginDate = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_beginDate;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_endDate))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaEndDate = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_endDate;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_eventDate))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaEventDate = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_eventDate;

                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_healthcareProviderReleaseIndicator))
                            {
                                //Valide N is correct for this IND field
                                var thisValue = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_healthcareProviderReleaseIndicator;

                                if (thisValue == "N")
                                {
                                    response.UDOgetContactRecordsInfo.udo_cpoaHCProviderRelease = false;
                                }
                                else
                                {
                                    response.UDOgetContactRecordsInfo.udo_cpoaHCProviderRelease = true;
                                }
                                response.UDOgetContactRecordsInfo.udo_cpoaHCProviderReleaseSpecified = true;
                            }

                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_journalDate))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaJrnDate = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_journalDate;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_journalLocationID))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaJrnLocID = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_journalLocationID;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_journalObjectID))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaJrnObjID = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_journalObjectID;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_journalStatusTypeCode))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaJrnStatusType = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_journalStatusTypeCode;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_journalUserID))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaJrnUserID = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_journalUserID;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrgName))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaOrgPersonName = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrgName;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrgAttentionText))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgAttn = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrgAttentionText;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrganizationCode))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgCode = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrganizationCode;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrganizationName))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgName = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrganizationName;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrgPtcpntID))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgParticipantID = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrgPtcpntID;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrOrganizationIndicator))
                            {
                                //Valide N is correct for this IND field
                                var thisValue = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrOrganizationIndicator;

                                if (thisValue == "O")
                                {
                                    response.UDOgetContactRecordsInfo.udo_cpoaPersonOrOrg = false;
                                }
                                else
                                {
                                    response.UDOgetContactRecordsInfo.udo_cpoaPersonOrOrg = true;
                                }
                                response.UDOgetContactRecordsInfo.udo_cpoaPersonOrOrgSpecified = true;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_prepositionalPhraseName))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaPrepositionalPhrase = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_prepositionalPhraseName;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_rateName))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaRateName = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_rateName;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_relationshipName))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaRelationship = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_relationshipName;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_statusCode))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaStatus = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_statusCode;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_temporaryCustodianIndicator))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaTempCustodian = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_temporaryCustodianIndicator;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_veteranPtcpntID))
                            {
                                response.UDOgetContactRecordsInfo.udo_cpoaVetPtcptID = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_veteranPtcpntID;
                            }
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrgName))
                            {
                                response.UDOgetContactRecordsInfo.udo_POA = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrgName;
                            }
                            #endregion
                        }

                        LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, Guid.Empty, null, null, "Contact:Done with POA", null, txnTimerconn.ElapsedMilliseconds);
                    }

                    txnTimerconn.Stop();

                    LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, Guid.Empty, null, null, "Contact:fidData", null, txnTimerconn.ElapsedMilliseconds);
                }
                return progressString;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, "get POAFIDData Processor, Progess:" + progressString);
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, ExecutionException);
                response.ExceptionMessage = $"Failed to get POAFIDDATA. MessageId: {request.MessageId}";
                response.ExceptionOccured = true;
                return progressString;
            }
        }

        private string getBirlsData(UDOgetContactRecordsRequest request, UDOgetContactRecordsResponse response, ref string progressString)
        {
            try
            {


                DateTime myNow = DateTime.Now;
                var extendedTIWEnd = myNow.Hour.ToString("00") + ":" + myNow.Minute.ToString("00") + ":" + myNow.Second.ToString("00") + ":" + myNow.Millisecond.ToString("000");

                Stopwatch txnTimerconn = Stopwatch.StartNew();
                var findBirlsRecordByFileNumberRequest = new VEISbrlsFNfindBirlsRecordByFileNumberRequest();
                findBirlsRecordByFileNumberRequest.MessageId = request.MessageId;
                findBirlsRecordByFileNumberRequest.LogTiming = request.LogTiming;
                findBirlsRecordByFileNumberRequest.LogSoap = request.LogSoap;
                findBirlsRecordByFileNumberRequest.Debug = request.Debug;
                findBirlsRecordByFileNumberRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findBirlsRecordByFileNumberRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findBirlsRecordByFileNumberRequest.RelatedParentId = request.RelatedParentId;
                findBirlsRecordByFileNumberRequest.UserId = request.UserId;
                findBirlsRecordByFileNumberRequest.OrganizationName = request.OrganizationName;

                //non standard fields
                findBirlsRecordByFileNumberRequest.mcs_filenumber = request.fileNumber;
                if (request.LegacyServiceHeaderInfo != null)
                {
                    findBirlsRecordByFileNumberRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                var findBirlsRecordByFileNumberResponse = WebApiUtility.SendReceive<VEISbrlsFNfindBirlsRecordByFileNumberResponse>(findBirlsRecordByFileNumberRequest, WebApiType.VEIS);
                if (request.LogSoap || findBirlsRecordByFileNumberResponse.ExceptionOccurred)
                {
                    if (findBirlsRecordByFileNumberResponse.SerializedSOAPRequest != null || findBirlsRecordByFileNumberResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findBirlsRecordByFileNumberResponse.SerializedSOAPRequest + findBirlsRecordByFileNumberResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISbrlsFNfindBirlsRecordByFileNumberRequest Request/Response {requestResponse}", true);
                    }
                }

                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, Guid.Empty, null, null, "Contact:Done with findbirls", null, txnTimerconn.ElapsedMilliseconds);
                progressString = "After VEIS EC Call";

                response.ExceptionMessage += $"{Environment.NewLine}BIRLS ERROR (VEISbrlsFNfindBirlsRecordByFileNumberResponse): {findBirlsRecordByFileNumberResponse.ExceptionMessage}";
                response.ExceptionOccured = findBirlsRecordByFileNumberResponse.ExceptionOccurred | response.ExceptionOccured;

                if (findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo != null)
                {
                    LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, Guid.Empty, null, null, "Parsing BIRLS Data", null, txnTimerconn.ElapsedMilliseconds);
                    var birlsRecord = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo;
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CAUSE_OF_DEATH))
                    {
                        response.UDOgetContactRecordsInfo.udo_CauseofDeath = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CAUSE_OF_DEATH;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_SEX_CODE))
                    {
                        response.UDOgetContactRecordsInfo.udo_gender = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_SEX_CODE;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_DEATH))
                    {
                        response.UDOgetContactRecordsInfo.udo_DateofDeath = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_DEATH;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CLAIM_FOLDER_LOCATION))
                    {
                        response.UDOgetContactRecordsInfo.udo_FolderLocation = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CLAIM_FOLDER_LOCATION;
                    }
                    if (findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNSERVICEInfo != null)
                    {
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, "Parsing VEISbrlsFNSERVICEInfo ...");

                        var serviceDTO = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNSERVICEInfo;
                        var latestServiceItem = serviceDTO.OrderByDescending(h => h.mcs_ENTERED_ON_DUTY_DATE).FirstOrDefault();

                        if (!string.IsNullOrEmpty(latestServiceItem.mcs_BRANCH_OF_SERVICE) && !string.IsNullOrWhiteSpace(latestServiceItem.mcs_BRANCH_OF_SERVICE))
                            response.UDOgetContactRecordsInfo.udo_BranchOfService = LongBranchOfService(latestServiceItem.mcs_BRANCH_OF_SERVICE);

                        if (!string.IsNullOrEmpty(latestServiceItem.mcs_CHAR_OF_SVC_CODE) && !string.IsNullOrWhiteSpace(latestServiceItem.mcs_CHAR_OF_SVC_CODE))
                            response.UDOgetContactRecordsInfo.udo_charofdisccode = latestServiceItem.mcs_CHAR_OF_SVC_CODE;

                        if (!string.IsNullOrEmpty(latestServiceItem.mcs_RELEASED_ACTIVE_DUTY_DATE) && !string.IsNullOrWhiteSpace(latestServiceItem.mcs_RELEASED_ACTIVE_DUTY_DATE))
                            if (DateTime.TryParse(dateStringFormat(latestServiceItem.mcs_RELEASED_ACTIVE_DUTY_DATE), out DateTime newDateTime))
                                response.UDOgetContactRecordsInfo.udo_activeReleaseDate = dateStringFormat(latestServiceItem.mcs_RELEASED_ACTIVE_DUTY_DATE);
                    }
                    else
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
                        findVeteranRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                        if (!string.IsNullOrEmpty(request.vetsoc))
                        {
                            // Replaced: 'veteranrecordinputInfo = VIMTfvetveteranrecordinput'
                            //          = 'VEISfvetReqveteranRecordInputInfo = VEISfvetReqveteranRecordInput'
                            findVeteranRequest.VEISfvetReqveteranRecordInputInfo = new VEISfvetReqveteranRecordInput
                            {
                                mcs_ssn = request.vetsoc
                            };
                        }
                        else
                        {
                            findVeteranRequest.VEISfvetReqveteranRecordInputInfo = new VEISfvetReqveteranRecordInput
                            {
                                mcs_fileNumber = request.fileNumber
                            };
                        }

                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, "Trying to call VEISfvetfindVeteranRequest");

                        var findVeteranResponse = WebApiUtility.SendReceive<VEISfvetfindVeteranResponse>(findVeteranRequest, WebApiType.VEIS);
                        if (request.LogSoap || findVeteranResponse.ExceptionOccurred)
                        {
                            if (findVeteranResponse.SerializedSOAPRequest != null || findVeteranResponse.SerializedSOAPResponse != null)
                            {
                                var requestResponse = findVeteranResponse.SerializedSOAPRequest + findVeteranResponse.SerializedSOAPResponse;
                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfvetfindVeteranRequest Request/Response {requestResponse}", true);
                            }
                        }

                        response.ExceptionMessage+= $"{Environment.NewLine}BIRLS ERROR (VEISfvetfindVeteranResponse):  {findVeteranResponse.ExceptionMessage}";
                        response.ExceptionOccured = findVeteranResponse.ExceptionOccurred | response.ExceptionOccured;

                        if (findVeteranResponse.VEISfvetreturnInfo != null)
                        {
                            if (findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo != null)
                            {
                                if (findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.VEISfvetSERVICEInfo != null)
                                {

                                    var serviceDTO = findVeteranResponse.VEISfvetreturnInfo.VEISfvetvetBirlsRecordInfo.VEISfvetSERVICEInfo;
                                    foreach (var item in serviceDTO)
                                    {
                                        if (!string.IsNullOrEmpty(item.mcs_BRANCH_OF_SERVICE))
                                        {
                                            response.UDOgetContactRecordsInfo.udo_BranchOfService = LongBranchOfService(item.mcs_BRANCH_OF_SERVICE);
                                        }

                                        if (!string.IsNullOrEmpty(item.mcs_CHAR_OF_SVC_CODE))
                                        {
                                            response.UDOgetContactRecordsInfo.udo_charofdisccode = item.mcs_CHAR_OF_SVC_CODE;
                                        }
                                        if (!string.IsNullOrEmpty(item.mcs_RELEASED_ACTIVE_DUTY_DATE))
                                        {
                                            DateTime newDateTime;
                                            if (DateTime.TryParse(dateStringFormat(item.mcs_RELEASED_ACTIVE_DUTY_DATE), out newDateTime))
                                            {
                                                response.UDOgetContactRecordsInfo.udo_activeReleaseDate = dateStringFormat(item.mcs_RELEASED_ACTIVE_DUTY_DATE);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                txnTimerconn.Stop();

                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, Guid.Empty, null, null, "Contact:Birls", null, txnTimerconn.ElapsedMilliseconds);

                return progressString;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, ExecutionException);

                response.ExceptionMessage = $"Failed to get BirlsData. MessageId {request.MessageId}";
                response.ExceptionOccured = true;
                return progressString;
            }
        }

        private static string LongBranchOfService(string branchcode)
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

        // Removed: static from method
        private string getCorpData(UDOgetContactRecordsRequest request, UDOgetContactRecordsResponse response, string progressString)
        {
            try
            {
                DateTime myNow = DateTime.Now;
                var extendedTIWEnd = myNow.Hour.ToString("00") + ":" + myNow.Minute.ToString("00") + ":" + myNow.Second.ToString("00") + ":" + myNow.Millisecond.ToString("000");


                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "getCorpData ", extendedTIWEnd + "starting");
                Stopwatch txnTimerconn = Stopwatch.StartNew();
                var findCorporateRecordByFileNumberRequest = new VEIScrpFNfindCorporateRecordByFileNumberRequest();
                findCorporateRecordByFileNumberRequest.MessageId = request.MessageId;
                findCorporateRecordByFileNumberRequest.LogTiming = request.LogTiming;
                findCorporateRecordByFileNumberRequest.LogSoap = request.LogSoap;
                findCorporateRecordByFileNumberRequest.Debug = request.Debug;
                findCorporateRecordByFileNumberRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findCorporateRecordByFileNumberRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findCorporateRecordByFileNumberRequest.RelatedParentId = request.RelatedParentId;
                findCorporateRecordByFileNumberRequest.UserId = request.UserId;
                findCorporateRecordByFileNumberRequest.OrganizationName = request.OrganizationName;

                if (request.LegacyServiceHeaderInfo != null)
                {
                    findCorporateRecordByFileNumberRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }
                //non standard fields
                findCorporateRecordByFileNumberRequest.mcs_filenumber = request.fileNumber;

                var findCorporateRecordByFileNumberResponse = WebApiUtility.SendReceive<VEIScrpFNfindCorporateRecordByFileNumberResponse>(findCorporateRecordByFileNumberRequest, WebApiType.VEIS);
                if (request.LogSoap || findCorporateRecordByFileNumberResponse.ExceptionOccurred)
                {
                    if (findCorporateRecordByFileNumberResponse.SerializedSOAPRequest != null || findCorporateRecordByFileNumberResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findCorporateRecordByFileNumberResponse.SerializedSOAPRequest + findCorporateRecordByFileNumberResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEIScrpFNfindCorporateRecordByFileNumberRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VEIS EC Call";
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "Contact:Done with findCorp", null, txnTimerconn.ElapsedMilliseconds);

                response.ExceptionMessage += $"{Environment.NewLine}CORPDB ERROR: {findCorporateRecordByFileNumberResponse.ExceptionMessage}";
                response.ExceptionOccured = findCorporateRecordByFileNumberResponse.ExceptionOccurred | response.ExceptionOccured;

                if (findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo != null)
                {
                    var shrinq2Person = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo;
                    if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_firstName))
                    {
                        response.UDOgetContactRecordsInfo.FirstName = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_firstName;

                    }
                    if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_lastName))
                    {
                        response.UDOgetContactRecordsInfo.LastName = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_lastName;
                    }
                    if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_middleName))
                    {
                        response.UDOgetContactRecordsInfo.MiddleName = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_middleName;
                    }


                    if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_dateOfBirth))
                    {
                        DateTime newDateTime;
                        if (DateTime.TryParse(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_dateOfBirth, out newDateTime))
                        {
                            response.UDOgetContactRecordsInfo.udo_DateofBirth = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_dateOfBirth;
                        }
                    }
                    if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_emailAddress))
                    {
                        response.UDOgetContactRecordsInfo.EMailAddress1 = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_emailAddress;
                    }

                    if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneTypeNameOne))
                    {
                        response.UDOgetContactRecordsInfo.udo_Phone1Type = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneTypeNameOne;
                    }
                    if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneNumberOne))
                    {
                        response.UDOgetContactRecordsInfo.Telephone1 = phoneNumberFormat(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_areaNumberOne + findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneNumberOne);
                        response.UDOgetContactRecordsInfo.udo_PhoneNumber1 = phoneNumberFormat(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_areaNumberOne + findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneNumberOne);
                    }
                    if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_dateOfBirth))
                    {
                        response.UDOgetContactRecordsInfo.udo_BirthDateString = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_dateOfBirth;
                    }

                    if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneTypeNameTwo))
                    {
                        response.UDOgetContactRecordsInfo.udo_Phone2Type = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneTypeNameTwo;
                    }
                    var telephone2 = "";
                    if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneNumberTwo))
                    {
                        telephone2 = "(" + findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_areaNumberTwo + ") " + findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneNumberTwo;
                        response.UDOgetContactRecordsInfo.udo_PhoneNumber2 = phoneNumberFormat(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_areaNumberTwo + findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_phoneNumberTwo);
                    }
                    if (!string.IsNullOrEmpty(findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_fiduciaryFolderLocation))
                    {
                        response.UDOgetContactRecordsInfo.udo_fidfolderloc = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo.mcs_fiduciaryFolderLocation;
                    }
                }
                txnTimerconn.Stop();

                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, Guid.Empty, null, null, "Contact:Corpdb", null, txnTimerconn.ElapsedMilliseconds);

                return progressString;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "get CorpDb Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = $"Failed to get CorpDb. MessageId: {request.MessageId}";
                response.ExceptionOccured = true;
                return progressString;
            }
        }
        // Rmeoved: static from method
        private string getEBenefitsData(UDOgetContactRecordsRequest request, UDOgetContactRecordsResponse response, string progressString)
        {
            Stopwatch txnTimerconn = Stopwatch.StartNew();
            try
            {
                DateTime myNow = DateTime.Now;
                var extendedTIWEnd = myNow.Hour.ToString("00") + ":" + myNow.Minute.ToString("00") + ":" + myNow.Second.ToString("00") + ":" + myNow.Millisecond.ToString("000");

                if (!string.IsNullOrEmpty(request.edipi) && request.edipi != "UNK")
                {
                    var getEbenefitsStatus = new VEISebenAccgetRegistrationStatusRequest();
                    getEbenefitsStatus.MessageId = request.MessageId;
                    getEbenefitsStatus.LogTiming = request.LogTiming;
                    getEbenefitsStatus.LogSoap = request.LogSoap;
                    getEbenefitsStatus.Debug = request.Debug;
                    getEbenefitsStatus.RelatedParentEntityName = request.RelatedParentEntityName;
                    getEbenefitsStatus.RelatedParentFieldName = request.RelatedParentFieldName;
                    getEbenefitsStatus.RelatedParentId = request.RelatedParentId;
                    getEbenefitsStatus.UserId = request.UserId;
                    getEbenefitsStatus.OrganizationName = request.OrganizationName;

                    if (request.LegacyServiceHeaderInfo != null)
                    {
                        getEbenefitsStatus.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                    }
                    //non standard fields
                    getEbenefitsStatus.mcs_edipi = request.edipi;

                    var benefitsresponse = WebApiUtility.SendReceive<VEISebenAccgetRegistrationStatusResponse>(getEbenefitsStatus, WebApiType.VEIS);
                    if (request.LogSoap || benefitsresponse.ExceptionOccurred)
                    {
                        if (benefitsresponse.SerializedSOAPRequest != null || benefitsresponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = benefitsresponse.SerializedSOAPRequest + benefitsresponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISebenAccgetRegistrationStatusRequest Request/Response {requestResponse}", true);
                        }
                    }

                    progressString = "After VEIS EC Call";
                    LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, Guid.Empty, null, null, "Contact:Done with getEBenefitsData", null, txnTimerconn.ElapsedMilliseconds);

                    response.ExceptionMessage+= $"{Environment.NewLine}EBenefits ERROR: {benefitsresponse.ExceptionMessage}";
                    response.ExceptionOccured = benefitsresponse.ExceptionOccured | response.ExceptionOccured;
                    if (benefitsresponse.VEISebenAccgetRegistrationStatusResponseDataInfo != null)
                    {
                        if (benefitsresponse.VEISebenAccgetRegistrationStatusResponseDataInfo != null)
                        {
                            var credLevel = benefitsresponse.VEISebenAccgetRegistrationStatusResponseDataInfo.mcs_credLevelAtLastLogin;
                            var credLevelString = "Basic";
                            switch (credLevel)
                            {
                                case 0:
                                    credLevelString = "None";
                                    break;
                                case 1:
                                    credLevelString = "Basic";
                                    break;
                                default:
                                    credLevelString = "Premium";
                                    break;
                            }

                            response.UDOgetContactRecordsInfo.udo_ebenefitsStatus = "Registered: " + benefitsresponse.VEISebenAccgetRegistrationStatusResponseDataInfo.mcs_isRegistered + "; Credlevel:" + credLevelString + "; Status:" + benefitsresponse.VEISebenAccgetRegistrationStatusResponseDataInfo.mcs_status;
                            if (benefitsresponse.VEISebenAccgetRegistrationStatusResponseDataInfo.mcs_isRegistered)
                            {
                                response.UDOgetContactRecordsInfo.udo_hasebenefits = 752280001;
                            }
                            else
                            {
                                response.UDOgetContactRecordsInfo.udo_hasebenefits = 752280000;
                            }
                        }
                    }
                }
                else
                {
                    //BUG FIX 1489: Set the default value if undefined EDIPI
                    response.UDOgetContactRecordsInfo.udo_ebenefitsStatus = "Not Applicable";
                    response.UDOgetContactRecordsInfo.udo_hasebenefits = 752280000;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "getEBenefitsData ", "No EDIPI, skipping ebenefits", request.Debug);
                }
                txnTimerconn.Stop();

                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, Guid.Empty, null, null, "Contact:eBenefits", null, txnTimerconn.ElapsedMilliseconds);

                return progressString;
            }
            catch (Exception ExecutionException)
            {
                var elapsedTime = double.Parse(txnTimerconn.ElapsedMilliseconds.ToString());
                var eventMetrics = new System.Collections.Generic.Dictionary<string, double>();
                eventMetrics.Add("ElapsedTime", elapsedTime);
                tLogger.LogException(ExecutionException, "998", eventMetrics);

                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "get EBenefits DataProcessor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = $"Failed to get EBenefits. MessageId: {request.MessageId}";
                response.ExceptionOccured = true;
                return progressString;
            }
        }

        private static string dateStringFormat(string date)
        {
            if (date == null) return null;
            if (date.Length == 10) return date;
            if (date.Length < 8) return date;

            date = date.Insert(2, "/");
            date = date.Insert(5, "/");

            return date;
        }

        private static string phoneNumberFormat(string phonenumber)
        {
            var returnNumber = "";

            if (phonenumber == null) return null;
            if (phonenumber.Length == 7)
            {
                returnNumber = phonenumber.Substring(0, 3) + "-" + phonenumber.Substring(4);
            }

            if (phonenumber.Length == 10)
            {
                returnNumber = "(" + phonenumber.Substring(0, 3) + ") " + phonenumber.Substring(3, 3) + "-" + phonenumber.Substring(6);
            }

            if (phonenumber.Length > 10)
            {
                returnNumber = "(" + phonenumber.Substring(0, 3) + ") " + phonenumber.Substring(3, 3) + "-" + phonenumber.Substring(6, 4) + " " + phonenumber.Substring(9);
            }
            return returnNumber;
        }
    }
}