using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//using VRM.Integration.UDO.ClaimEstablishment.Messages;
//using VRM.Integration.UDO.Common;
//using VRM.Integration.UDO.Common.Messages;
//using VIMT.AddressWebService.Messages;
//using VIMT.BenefitClaimServiceV2.Messages;
//using VIMT.VeteranWebService.Messages;
//using VRM.Integration.UDO.ClaimEstablishment.Helper;
using UDO.LOB.Core;
using UDO.LOB.ClaimEstablishment.Messages;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Extensions;
using VEIS.Messages.BenefitClaimServiceV2;
using System.Collections.Specialized;
using UDO.LOB.Extensions.Configuration;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;

namespace UDO.LOB.ClaimEstablishment.Processors
{
    public class UDOInitiateClaimEstablishmentProcessor
    {
        private bool _debug { get; set; }
        public string MachineName { get; set; }
        private CrmServiceClient OrgServiceProxy;
        private string method = "UDOInitiateClaimEstablishmentProcessor";
        private string LogBuffer { get; set; }

        public UDOInitiateClaimEstablishmentProcessor()
        {
            MachineName = System.Environment.MachineName;
        }

        public IMessageBase Execute(UDOInitiateClaimEstablishmentRequest request)
        {
            var common = new UDOClaimEstablishmentCommon();
            var response = new UDOInitiateClaimEstablishmentResponse();
            var claimEstablishmentExceptions = new List<UDOException>();

            var progressString = "Top of Processor";

            #region Check for no Request Message

            if (request == null)
            {
                claimEstablishmentExceptions.Add(new UDOException()
                {
                    ExceptionMessage = "Called with no message",
                    ExceptionOccurred = true,
                    ExceptionCategory = "Person"
                });

                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
                response.InnerExceptions = claimEstablishmentExceptions.ToArray();
                return response;
            }

            #endregion

            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            TraceLogger tLogger = new TraceLogger(method, request);

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOInitiateClaimEstablishmentRequest>(request)}");

            _debug = request.Debug;
            LogBuffer = string.Empty;

            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, "About to create Claim Establishment");
                response.ClaimEstablishmentId = common.CreateClaimEstablishment(request, OrgServiceProxy);
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, $"Claim Establishment Created - Id: {response.ClaimEstablishmentId}");

                //if (!response.ExceptionOccured && claimEstablishmentExceptions.Count > 0)
                //{
                //    response.ExceptionOccured = true;
                //    response.ExceptionMessage = "Internal Exceptions Occurred";
                //    response.InnerExceptions = claimEstablishmentExceptions.ToArray();
                //}

                return response;
            }
            catch (Exception executionException)
            {
                var stInfo = string.Empty;

                var st = new StackTrace(executionException, true);
                for (var i = 0; i < st.FrameCount; i++)
                {
                    var sf = st.GetFrame(i);
                    stInfo = stInfo +
                             string.Format("LOB Machine Name: {0}, Method: {1}, File: {2}, Line Number: {3}", MachineName, sf.GetMethod(), sf.GetFileName(),
                                 sf.GetFileLineNumber());
                    stInfo = stInfo + System.Environment.NewLine;
                }

                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId,
                    method, executionException);

                response.ExceptionMessage = $"{method}:  Failed to Map EC data to LOB. Execution Progress: {progressString}";
                response.ExceptionOccurred = true;
                response.StackTrace = stInfo;

                if (claimEstablishmentExceptions.Count > 0) response.InnerExceptions = claimEstablishmentExceptions.ToArray();
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

        public VEISfindBenefitClaimResponse FindClaimEstablishment(UDOInitiateClaimEstablishmentRequest request, Entity thisNewEntity)
        {

            var findBenefitClaimRequest = new VEISfindBenefitClaimRequest()
            {
                Debug = request.Debug,
                LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo()
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                },
                LogSoap = request.LogSoap,
                LogTiming = request.LogTiming,
                OrganizationName = request.OrganizationName,
                RelatedParentEntityName = request.RelatedParentEntityName,
                RelatedParentFieldName = request.RelatedParentFieldName,
                RelatedParentId = request.RelatedParentId,
                UserId = request.UserId,
                MessageId = request.MessageId,
                mcs_filenumber = request.fileNumber
            };

            var findBenefitClaimResponse = WebApiUtility.SendReceive<VEISfindBenefitClaimResponse>(findBenefitClaimRequest, WebApiType.VEIS);

            if (request.LogSoap || findBenefitClaimResponse.ExceptionOccurred)
            {
                if (findBenefitClaimResponse.SerializedSOAPRequest != null || findBenefitClaimResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findBenefitClaimResponse.SerializedSOAPRequest + findBenefitClaimResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfindBenefitClaimRequest Request/Response {requestResponse}", true);
                }
            }

            if (findBenefitClaimResponse.ExceptionOccured == false)
            {
                if (findBenefitClaimResponse.VEISbenefitClaimRecordBCS2Info != null)
                {

                    thisNewEntity["udo_homelessindicator"] = findBenefitClaimResponse.VEISbenefitClaimRecordBCS2Info.mcs_homelessIndicator;
                    thisNewEntity["udo_persiangulfwarserviceindicator"] = findBenefitClaimResponse.VEISbenefitClaimRecordBCS2Info.mcs_gulfWarRegistryPermit;
                    thisNewEntity["udo_powdays"] = findBenefitClaimResponse.VEISbenefitClaimRecordBCS2Info.mcs_powNumberOfDays;
                }
            }

            return findBenefitClaimResponse;
        }

    }
}