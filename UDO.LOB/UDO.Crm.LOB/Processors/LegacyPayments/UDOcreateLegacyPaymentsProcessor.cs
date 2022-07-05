using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using VIMT.PaymentHistoryService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.LegacyPayments.Messages;

namespace VRM.Integration.UDO.LegacyPayments.Processors
{
    class UDOcreateUDOLegacyPaymentsProcessor
    {
        private const string LOG_CONFIG_FIELD = "mcs_createUDOLegacyPaymentData";
        private VRM.Integration.UDO.Common.TimeTracker timer { get; set; }
	    private bool _debug { get; set; }
        private const string method = "UDOcreateUDOLegacyPaymentsProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateLegacyPaymentsRequest request)
        {
            #region Start Timer

            if (request.LogTiming)
            {
                timer = new VRM.Integration.UDO.Common.TimeTracker();
                timer.Restart();
            }

            #endregion
            
            UDOcreateLegacyPaymentsResponse response = new UDOcreateLegacyPaymentsResponse();
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                timer.Stop();
                timer = null;
                return response;
            }
            
            #region connect to CRM
            OrganizationServiceProxy OrgServiceProxy;
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateUDOLegacyPaymentsProcessor Processor, Connection Error", connectException.Message);                
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                timer.Stop();
                timer = null;
                return response;
            }
            progressString = "After Connection";
            #endregion
            
            try
            {
                // prefix = payHistSSN_findPayHistoryBySSNRequest();
                if (request.LogTiming) timer.MarkStart("findPayHistoryBySSNResponse");
                var findPayHistoryBySSNRequest = new VIMTpayHistSSN_findPayHistoryBySSNRequest
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    LegacyServiceHeaderInfo = new VIMT.PaymentHistoryService.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    mcs_ssn = String.IsNullOrEmpty(request.filenumber) ? request.ssn : request.filenumber
                };

                var findPayHistoryBySSNResponse = findPayHistoryBySSNRequest.SendReceive<VIMTpayHistSSN_findPayHistoryBySSNResponse>(MessageProcessType.Local);
                if (request.LogTiming) timer.MarkStop("findPayHistoryBySSNResponse");
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = findPayHistoryBySSNResponse.ExceptionMessage;
                response.ExceptionOccured = findPayHistoryBySSNResponse.ExceptionOccured;

                var requestCollection = new OrganizationRequestCollection();
                #region process response
                if (findPayHistoryBySSNResponse != null)
                {
                    if (findPayHistoryBySSNResponse.VIMTpayHistSSN_Info != null)
                    {
                        if (findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo != null)
                        {
                            var legacyPayments = findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo;

                            //instantiate the new Entity
                            Entity thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_legacypaymenthistory";
                            thisNewEntity["udo_name"] = "Legacy Payment Summary";
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            if (!string.IsNullOrEmpty(findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.mcs_fileNumber))
                            {
                                thisNewEntity["udo_filenumber"] = findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.mcs_fileNumber;
                            }
                            if (!string.IsNullOrEmpty(findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.mcs_fullName))
                            {
                                thisNewEntity["udo_fullname"] = findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.mcs_fullName;
                            }
                            if (!string.IsNullOrEmpty(findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.mcs_lastActivityDt))
                            {
                                thisNewEntity["udo_lastactivitydate"] = findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.mcs_lastActivityDt;
                            }
                            if (!string.IsNullOrEmpty(findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.mcs_lastFicheDt))
                            {
                                thisNewEntity["udo_lastfichedate"] = findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.mcs_lastFicheDt;
                            }
                            if (!string.IsNullOrEmpty(findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.mcs_payeeCode))
                            {
                                thisNewEntity["udo_payee"] = findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.mcs_payeeCode;
                            }
                            if (!string.IsNullOrEmpty(findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.mcs_priorFicheDt))
                            {
                                thisNewEntity["udo_priorfichedate"] = findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.mcs_priorFicheDt;
                            }

                            if (request.UDOcreateUDOLegacyPaymentsRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedItem in request.UDOcreateUDOLegacyPaymentsRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }
                            CreateRequest createPayments = new CreateRequest
                            {
                                Target = thisNewEntity
                            };
                            requestCollection.Add(createPayments);
                        }
                    }
                }
                if (request.LogTiming) timer.MarkStop("Processing Data");
                #endregion
            
                if (requestCollection.Count > 0)
                {
                    #region Execute Multiple

                    var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, request.UserId, request.Debug);

                    if (_debug)
                    {
                        LogBuffer += result.LogDetail;
                        LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                    }

                    if (result.IsFaulted)
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                        response.ExceptionMessage = result.FriendlyDetail;
                        response.ExceptionOccured = true;
                        return response;
                    }

                    string logInfo = string.Format("Number of Legacy Payment Records Created: {0}", requestCollection.Count);
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, method, logInfo);
                    #endregion
                }
              
                if (request.idProofId != Guid.Empty)
                {
                    var idProof = new Entity();
                    idProof.Id = request.idProofId;
                    idProof.LogicalName = "udo_idproof";
                    idProof["udo_legacypaymentintegration"] = new OptionSetValue(752280002);
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(idProof, request.OrganizationName, request.UserId, request.LogTiming));
                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, "no idProofId found");
                }

                #region Stop Timer

                if (request.LogTiming)
                {
                    var elapsedMilliseconds = timer.LogDurations(request.OrganizationName, request.Debug, request.UserId,
                        string.Format("{0}, Progress: {1}", method, progressString), true);
                    LogHelper.LogTiming(request.OrganizationName, request.Debug, request.UserId,
                        request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method,
                        method,
                        Convert.ToDecimal(elapsedMilliseconds));
                }
                #endregion
                timer = null;
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateUDOLegacyPaymentsProcessor Processor, Progess:" + progressString, ExecutionException);
    
                if (request.LogTiming)
                {
                    timer.LogDurations(request.OrganizationName, request.Debug, request.UserId, method, true);
                    timer = null;
                }
                response.ExceptionMessage = "Failed to process Legacy payment Data";
                response.ExceptionOccured = true;
                if (request.idProofId != Guid.Empty)
                {
                    var idProof = new Entity();
                    idProof.Id = request.idProofId;
                    idProof.LogicalName = "udo_idproof";
                    idProof["udo_legacypaymentintegration"] = new OptionSetValue(752280003);
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(idProof, request.OrganizationName, request.UserId, request.LogTiming));
                } 
                return response;
            }
        }
    }
}