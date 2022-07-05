using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using VRMRest;
using System.ServiceModel;
using System.Diagnostics;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.LegacyPayments.Messages;
using UDO.Model;
using UDO.LOB.Core;
using UDO.LOB.LegacyPayments.Messages;

namespace CustomActions.Plugins.Entities.LegacyPayments
{
    public class UDOGetLegacyPaymentsRunner : UDOActionRunner
    {
        protected string _fileNumber = "";
        protected string _PID = "";
        protected string _ssn = "";
        protected Guid _ownerId = new Guid();
        protected string _ownerType;
        protected Guid _veteranId = new Guid();
        protected Guid _idproofId = new Guid();
        protected Guid _legacyPaymentHistoryId = new Guid();

        public UDOGetLegacyPaymentsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_legacypaymentslogtimer";
            _logSoapField = "udo_legacypaymentslogsoap";
            _debugField = "udo_legacypayments";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_legacypaymentsvimttimeout";
            _validEntities = new string[] { "udo_idproof","udo_legacypaymenthistory" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            _method = "DoAction";

            GetSettingValues();


            if (Parent.LogicalName == udo_idproof.EntityLogicalName)
            {
                _idproofId = Parent.Id;

                if (!didWeNeedIdProofData())
                {
                    DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                    return;
                }

                CreateLegacyPayments();
            }
            if (Parent.LogicalName == udo_legacypaymenthistory.EntityLogicalName)
            {
                _legacyPaymentHistoryId = Parent.Id;

                if (!DidWeNeedLegacyPaymentHistoryData())
                {
                    DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                    return;
                }

                CreateLegacyPaymentDetails();
            }

        }

        #region Create Legacy Payments

        private void CreateLegacyPayments()
        {
            var request = new UDOcreateLegacyPaymentsRequest();
            ////Logger.WriteDebugMessage("Need to get data");
            tracer.Trace("Need to get data");

            UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            var idReference = new UDOcreateUDOLegacyPaymentsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_idproofid",
                RelatedEntityId = _idproofId,
                RelatedEntityName = "udo_idproof"
            };
            var veteranReference = new UDOcreateUDOLegacyPaymentsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var references = new[] { veteranReference, idReference };

            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.ssn = _ssn;
            request.filenumber = _fileNumber;
            request.RelatedParentEntityName = "udo_idproof";
            request.RelatedParentFieldName = "udo_idproofid";
            request.RelatedParentId = _idproofId;
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            request.UDOcreateUDOLegacyPaymentsRelatedEntitiesInfo = references;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            request.idProofId = _idproofId;

            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            tracer.Trace("calling UDOcreateLegacyPaymentsRequest");
            var response = Utility.SendReceive<UDOcreateLegacyPaymentsResponse>(_uri, "UDOcreateLegacyPaymentsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateLegacyPaymentsRequest");

            if (response.ExceptionOccurred)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Legacy Payments LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
            }
        }
        private bool didWeNeedIdProofData()
        {
            try
            {
                tracer.Trace("didWeNeedIdProofData started");
                Logger.setMethod = "didWeNeedIdProofData";
                var gotData = false;

                using (var xrm = new UDOContext(OrganizationService))
                {
                    var getParent = (from awd in xrm.udo_idproofSet
                                    join vet in xrm.ContactSet on awd.udo_Veteran.Id equals vet.ContactId.Value
                                    where awd.udo_idproofId.Value == _idproofId

                                    select new
                                    {
                                        vet.ContactId,
                                        vet.udo_ParticipantId,
                                        vet.udo_FileNumber,
                                        vet.OwnerId,
                                        vet.udo_SSN,
                                        awd.udo_legacyPaymentIntegration
                                    }).FirstOrDefault();
                    if (getParent != null)
                    {
                        gotData = true;
                        if (getParent.udo_SSN != null)
                        {
                            _ssn = getParent.udo_SSN;
                        }
                        else
                        {
                            _responseMessage = "No SSN found. Cannot get Payments.";
                            return false;
                        }
                        if (getParent.udo_legacyPaymentIntegration != null)
                        {
                            var legacyPaymentInt = getParent.udo_legacyPaymentIntegration.Value;
                            ////Logger.WriteDebugMessage("claimInt==" + legacyPaymentInt);
                            tracer.Trace("claimInt==" + legacyPaymentInt);

                            if (legacyPaymentInt != 752280000)
                            {
                                //anything but not started means we don't do anything
                                Complete = true;
                                return false;
                            }
                        }
                        if (getParent.ContactId.HasValue)
                        {
                            _veteranId = getParent.ContactId.Value;
                        }
                        if (getParent.udo_ParticipantId != null)
                        {
                            _PID = getParent.udo_ParticipantId;
                        }
                        else
                        {
                            _responseMessage = "No PID found. Cannot get Payments.";
                            return false;
                        }
                        if (getParent.udo_FileNumber != null)
                        {
                            _fileNumber = getParent.udo_FileNumber;
                        }
                        else
                        {
                            _responseMessage = "No File Number found. Cannot get Payments.";
                            return false;
                        }
                        if (getParent.OwnerId != null)
                        {
                            _ownerType = getParent.OwnerId.LogicalName;
                            _ownerId = getParent.OwnerId.Id;
                        }
                        else
                        {
                            return false;
                        }

                    }
                }
                tracer.Trace("didWeNeedData have been retrieved and set");
                Logger.setMethod = "Execute";

                return gotData;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }

        #endregion

        #region Create Legacy Payment Details

        private void CreateLegacyPaymentDetails()
        {

            var request = new UDOcreateLegacyPaymentsDetailsRequest();
            request.udo_legacypaymenthistoryId = _legacyPaymentHistoryId;

            var veteranReference = new UDOcreateUDOLegacyPaymentsDetailsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_legacyPaymentHistoryReference = new UDOcreateUDOLegacyPaymentsDetailsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_legacypaymentid",
                RelatedEntityId = _legacyPaymentHistoryId,
                RelatedEntityName = "udo_legacypaymenthistory"
            };
            var references = new[] { veteranReference, udo_legacyPaymentHistoryReference };

            UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            request.filenumber = _fileNumber;
            request.udo_legacypaymenthistoryId = _legacyPaymentHistoryId;
            request.UDOcreateUDOLegacyPaymentsDetailsRelatedEntitiesInfo = references;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.ssn = _ssn;
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;

            LogSettings _logSettings = new LogSettings() { Org = PluginExecutionContext.OrganizationName, ConfigFieldName = "RESTCALL", UserId = PluginExecutionContext.InitiatingUserId };

            tracer.Trace("calling UDOcreateLegacyPaymentsDetailsRequest");
            var response = Utility.SendReceive<UDOcreateLegacyPaymentsResponse>(_uri, "UDOcreateLegacyPaymentsDetailsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateLegacyPaymentsDetailsRequest");
            if (response.ExceptionOccurred)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Legacy Payment Details LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage += response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
            }
        }

        private bool DidWeNeedLegacyPaymentHistoryData()
        {
            try
            {
                tracer.Trace("DidWeNeedLegacyPaymentHistoryData started");
                Logger.setMethod = "DidWeNeedLegacyPaymentHistoryData";
                var gotData = false;

                using (var xrm = new UDOContext(OrganizationService))
                {
                    var getParent = (from vet in xrm.ContactSet
                                     join leg in xrm.udo_legacypaymenthistorySet on vet.ContactId equals leg.udo_VeteranId.Id
                                     where leg.udo_legacypaymenthistoryId == _legacyPaymentHistoryId
                                    select new
                                    {
                                        vet.udo_ParticipantId,
                                        vet.udo_FileNumber,
                                        vet.udo_SSN,
                                        vet.OwnerId,
                                        leg.udo_VeteranId,
                                        leg.udo_legacypaymentdatacomplete,
                                        leg.udo_CallComplete
                                    }).FirstOrDefault();
                    if (getParent != null)
                    {
                        gotData = true;
                        if (getParent.udo_VeteranId != null)
                        {
                            _veteranId = getParent.udo_VeteranId.Id;
                        }
                        else
                        {
                            _responseMessage = "Veteran ID is null! Cannot get Legacy Payment details.";
                            return false;
                        }

                        if (getParent.udo_legacypaymentdatacomplete.HasValue)
                        {
                            if (getParent.udo_legacypaymentdatacomplete.Value)
                            {
                                Complete = true;
                                return false;
                            }
                        }
                        else
                        {
                            _responseMessage = "Legacy Payment Data Complete is null! Cannot get Legacy Payment details.";
                            return false;
                        }

                        bool? callFlag = getParent.udo_CallComplete;
                        if (getParent.udo_CallComplete.HasValue)
                        {
                            if (getParent.udo_CallComplete.Value)
                            {
                                _responseMessage = "Call is already complete. Cannot get Legacy Payment details.";
                                return false;
                            }
                        }
                        else
                        {
                            _responseMessage = "Cannot deterimine status of call. Cannot get Legacy Payment details";
                            return false;
                        }
                        if (getParent.udo_SSN != null)
                        {
                            _ssn = getParent.udo_SSN;
                        }
                        else
                        {
                            _responseMessage = "SSN not found. Cannot get Legacy Payment details.";
                            return false;
                        }
                        if (getParent.udo_ParticipantId != null)
                        {
                            _PID = getParent.udo_ParticipantId;
                        }
                        else
                        {
                            _responseMessage = "No PID found. Cannot get Legacy Payment details.";
                            return false;
                        }
                        if (getParent.udo_FileNumber != null)
                        {
                            _fileNumber = getParent.udo_FileNumber;
                        }
                        else
                        {
                            _responseMessage = "File Number not found. Cannot get Legacy Payment details.";
                            return false;
                        }
                        if (getParent.OwnerId != null)
                        {
                            _ownerType = getParent.OwnerId.LogicalName;
                            _ownerId = getParent.OwnerId.Id;
                        }
                        else
                        {
                            _responseMessage = "Owner ID not found. Cannot get Legacy Payment details.";
                            return false;
                        }
                    }
                }

                Logger.setMethod = "Execute";
                return gotData;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }

        #endregion
    }
}
