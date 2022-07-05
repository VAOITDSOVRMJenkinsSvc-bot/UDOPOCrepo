
namespace UDO.LOB.IntentToFile.Processors
{
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections.Specialized;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions;
    using UDO.LOB.Extensions.Configuration;
    using UDO.LOB.Extensions.Logging;
    using UDO.LOB.IntentToFile.Messages;
    using VEIS.Core.Messages;
    using VEIS.Messages.IntentToFileWebService;
    using VEIS.Messages.AddressWebService;
    using Microsoft.Xrm.Sdk.WebServiceClient;
    using Microsoft.Xrm.Tooling.Connector;

    class UDOfindZipCodeProcessor
    {
        private CrmServiceClient OrgServiceProxy;

        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOfindZipCodeProcessor";

        public IMessageBase Execute(UDOfindZipCodeRequest request)
        {
            UDOfindZipCodeResponse response = new UDOfindZipCodeResponse
            {
                MessageId = request.MessageId
            };
        
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
                return response;
            }

            if (request.DiagnosticsContext == null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : string.Empty,
                };
            }
            TraceLogger aiLogger = new TraceLogger(method, request);

            LogHelper.LogInfo(string.Format("Message Id:{0}, Type={2}, Recieved diagnostics message: {1}",
            request.MessageId,
            request.MessageId,
            GetType().FullName));

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

            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
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
                // prefix = payHistSSN_findPayHistoryBySSNRequest();
                var findZipCodeByCityStateRequest = new VEISfpsbcistfindPostalCodeByCityStateCountryRequest
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    mcs_city = request.City,
                    mcs_country = request.Country,
                    mcs_state = request.State
                };

                // Replaced: findZipCodeByCityStateRequest.SendReceive<VIMTfpsbcistfindPostalCodeByCityStateCountryResponse>(MessageProcessType.Local)
                var findZipCodeByCityStateResponse = WebApiUtility.SendReceive<VEISfpsbcistfindPostalCodeByCityStateCountryResponse>(findZipCodeByCityStateRequest, WebApiType.VEIS);
                progressString = "After VIMT EC Call";

                if (request.LogSoap || findZipCodeByCityStateResponse.ExceptionOccurred)
                {
                    if (findZipCodeByCityStateResponse.SerializedSOAPRequest != null || findZipCodeByCityStateResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findZipCodeByCityStateResponse.SerializedSOAPRequest + findZipCodeByCityStateResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfpsbcistfindPostalCodeByCityStateCountryRequest Request/Response {requestResponse}", true);
                    }
                }

                response.ExceptionMessage = findZipCodeByCityStateResponse.ExceptionMessage;
                response.ExceptionOccurred = findZipCodeByCityStateResponse.ExceptionOccurred;

                var requestCollection = new OrganizationRequestCollection();

                if (findZipCodeByCityStateResponse != null)
                {
                    if (findZipCodeByCityStateResponse.VEISfpsbcistreturnInfo != null)
                    {
                        System.Collections.Generic.List<ValidateAddressMultipleResponse> foundPostalCodes = new System.Collections.Generic.List<ValidateAddressMultipleResponse>();
                        foreach (var postalCode in findZipCodeByCityStateResponse.VEISfpsbcistreturnInfo)
                        {
                            var thisPostalCode = new ValidateAddressMultipleResponse();
                            if (!string.IsNullOrEmpty(postalCode.mcs_cityType))
                            {
                                thisPostalCode.cityType = postalCode.mcs_cityType;
                            }
                            if (!string.IsNullOrEmpty(postalCode.mcs_postalCode))
                            {
                                thisPostalCode.postalCode = postalCode.mcs_postalCode;
                            }
                            if (!string.IsNullOrEmpty(postalCode.mcs_processedBy))
                            {
                                thisPostalCode.processedBy = postalCode.mcs_processedBy;
                            }
                            if (!string.IsNullOrEmpty(postalCode.mcs_status))
                            {
                                thisPostalCode.status = postalCode.mcs_status;
                            }
                            if (!string.IsNullOrEmpty(postalCode.mcs_statusCode))
                            {
                                thisPostalCode.statusCode = postalCode.mcs_statusCode;
                            }
                            if (!string.IsNullOrEmpty(postalCode.mcs_statusDescription))
                            {
                                thisPostalCode.statusDescription = postalCode.mcs_statusDescription;
                            }
                            foundPostalCodes.Add(thisPostalCode);
                        }
                        response.ValidatedAddresses = foundPostalCodes.ToArray();
                    }
                }

                //added to generated code
                if (request.RelatedParentId != System.Guid.Empty)
                {
                    var parent = new Entity
                    {
                        Id = request.RelatedParentId,
                        LogicalName = "va_intenttofile"
                    };
                    parent["va_intenttofilecomplete"] = true;
                    //OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, 
                    request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId,
                    "UDOfindZipCode Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to process zip code data";
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
    }
}