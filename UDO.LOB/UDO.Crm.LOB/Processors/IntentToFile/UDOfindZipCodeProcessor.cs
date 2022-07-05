using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using VIMT.AddressWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.IntentToFile.Messages;
using Logger = VRM.Integration.Servicebus.Core.Logger;

namespace VRM.Integration.UDO.IntentToFile.Processors
{
    class UDOfindZipCodeProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOfindZipCodeProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOfindZipCodeRequest request)
        {
            //var request = message as createUDOLegacyPaymentDataRequest;
            UDOfindZipCodeResponse response = new UDOfindZipCodeResponse();
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
                // prefix = payHistSSN_findPayHistoryBySSNRequest();
                var findZipCodeByCityStateRequest = new VIMTfpsbcistfindPostalCodeByCityStateCountryRequest
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    LegacyServiceHeaderInfo = new VIMT.AddressWebService.Messages.HeaderInfo
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

                var findZipCodeByCityStateResponse = findZipCodeByCityStateRequest.SendReceive<VIMTfpsbcistfindPostalCodeByCityStateCountryResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = findZipCodeByCityStateResponse.ExceptionMessage;
                response.ExceptionOccured = findZipCodeByCityStateResponse.ExceptionOccured;

                var requestCollection = new OrganizationRequestCollection();

                if (findZipCodeByCityStateResponse != null)
                {
                    if (findZipCodeByCityStateResponse.VIMTfpsbcistreturnInfo != null)
                    {
                        System.Collections.Generic.List<ValidateAddressMultipleResponse> foundPostalCodes = new System.Collections.Generic.List<ValidateAddressMultipleResponse>();
                        foreach (var postalCode in findZipCodeByCityStateResponse.VIMTfpsbcistreturnInfo)
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
                    var parent = new Entity();
                    parent.Id = request.RelatedParentId;
                    parent.LogicalName = "va_intenttofile";
                    parent["va_intenttofilecomplete"] = true;
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOfindZipCode Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to process zip code data";
                response.ExceptionOccured = true;
                return response;
            }
        }
    }
}