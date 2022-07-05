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
//using VRM.Integration.UDO.Payments.Messages;
using UDO.Model;
using UDO.LOB.Core;
using UDO.LOB.Payments.Messages;

namespace CustomActions.Plugins.Entities.Payments
{
    public class UDOGetPaymentsRunner : UDOActionRunner
    {
        Guid _payeecodeId = new Guid();
        Guid _paymentId = new Guid();
        Guid _veteranId = new Guid();
        Guid _ownerId = new Guid();
        Guid _vetSnapId = Guid.Empty;
        Guid _peopleId = Guid.Empty;
        string _ownerType;
        long _lngPaymentId = new long();
        long _fbtId = new long();
        long _PID = new long();
        string _payeecode = "";
        string _fileNumber = "";

        public UDOGetPaymentsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_paymentlogtimer";
            _logSoapField = "udo_paymentlogsoap";
            _debugField = "udo_payment";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_paymentvimttimeout";
            _validEntities = new string[] { "udo_payeecode", "udo_payment" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            _method = "DoAction";

            if (Parent.LogicalName == udo_payeecode.EntityLogicalName)
            {
                _payeecodeId = Parent.Id;

                if (!didWeNeedPayeeCodeData())
                {
                    DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                    return;
                }

                CreatePayments();
            }


            if (Parent.LogicalName == udo_payment.EntityLogicalName)
            {
                _paymentId = Parent.Id;

                if (!didWeNeedPaymentData())
                {
                    DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                    return;
                }
                CreatePaymentDetails();
            }
        }

        #region Create Payments
        private void CreatePayments()
        {
            GetSettingValues();
            var request = new UDOcreatePaymentsRequest();

            var veteranReference = new UDOcreatePaymentsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_paymentReference = new UDOcreatePaymentsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_payeecodeid",
                RelatedEntityId = _payeecodeId,
                RelatedEntityName = "udo_payeecode"
            };

            var udo_vetSnapReference = new UDOcreatePaymentsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteransnapshotid",
                RelatedEntityId = _vetSnapId,
                RelatedEntityName = "udo_veteransnapshot"
            };

            var peopleReference = new UDOcreatePaymentsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_personid",
                RelatedEntityId = _peopleId,
                RelatedEntityName = "udo_person"
            };

            if (_vetSnapId != Guid.Empty)
            {
                request.UDOcreatePaymentsRelatedEntitiesInfo = new[] { veteranReference, udo_paymentReference, udo_vetSnapReference, peopleReference };
            }
            else
            {
                request.UDOcreatePaymentsRelatedEntitiesInfo = new[] { veteranReference, udo_paymentReference, peopleReference };
            }

            UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.RelatedParentEntityName = "contact";
            request.RelatedParentFieldName = "udo_contactid";
            request.RelatedParentId = _veteranId;
            request.Debug = _debug;
            request.vetsnapshotId = _vetSnapId;
            request.udo_personId = _peopleId;
            request.PaymentId = _lngPaymentId;
            request.FileNumber = _fileNumber;
            request.ParticipantId = _PID;
            request.PayeeCode = _payeecode;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;

            if (PluginExecutionContext.InputParameters.Contains("CreatePaymentRecords"))
            {
                request.CreatePaymentRecords =
                    (bool)PluginExecutionContext.InputParameters["CreatePaymentRecords"];
            }
            else
            {
                request.CreatePaymentRecords = true;
            }

            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            tracer.Trace("calling UDOcreatePaymentsRequest");
            var response = Utility.SendReceive<UDOcreatePaymentsResponse>(_uri, "UDOcreatePaymentsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreatePaymentsRequest");

            if (response.ExceptionOccurred)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Diaries LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
            }
        }

        private bool didWeNeedPayeeCodeData() 
        {
            try
            {
                tracer.Trace("didWeNeedPayeeCodeData started");
                Logger.setMethod = "didWeNeedPayeeCodeData";
                var gotData = false;

                using (var xrm = new UDOContext(OrganizationService))
                {

                    var getParent = from awd in xrm.udo_payeecodeSet
                                    join vetSnap in xrm.udo_veteransnapshotSet on awd.udo_IdProofId.Id equals vetSnap.udo_IDProofID.Id
                                    join people in xrm.udo_personSet on awd.udo_payeecodeId.Value equals people.udo_payeecodeid.Id
                                    join vet in xrm.ContactSet on awd.udo_VeteranId.Id equals vet.ContactId.Value
                                    where awd.udo_payeecodeId.Value == _payeecodeId
                                    select new
                                    {
                                        awd.udo_paymentloadcomplete,
                                        awd.udo_LoadPayment,
                                        awd.udo_VeteranId,
                                        awd.udo_name,
                                        awd.udo_payeecode1,
                                        awd.udo_filenumber,
                                        awd.udo_participantid,
                                        vetSnap.udo_veteransnapshotId,
                                        people.udo_personId,
                                        vet.OwnerId
                                    };

                    foreach (var awd in getParent)
                    {
                        gotData = true;
                        if (awd.udo_veteransnapshotId.HasValue)
                        {
                            _vetSnapId = awd.udo_veteransnapshotId.Value;
                        }

                        if (awd.udo_personId.HasValue)
                        {
                            _peopleId = awd.udo_personId.Value;
                        }

                        if (awd.udo_VeteranId != null)
                        {
                            _veteranId = awd.udo_VeteranId.Id;
                        }
                        if (awd.udo_paymentloadcomplete != null)
                        {
                            if (awd.udo_paymentloadcomplete.Value)
                            {
                                Complete = true;
                                return false;
                            }
                        }
                        if (awd.udo_LoadPayment != null)
                        {
                            if(awd.udo_LoadPayment.Value)
                            {
                                Complete = true;
                                return false;
                            }
                        }
                        if (awd.udo_filenumber != null)
                        {
                            _fileNumber = awd.udo_filenumber;
                        }
                        //else
                        //{
                        //    _responseMessage = "File Number not found. Cannot retrieve payments.";
                        //    return false;
                        //}
                        if (awd.udo_participantid != null)
                        {
                            _PID = Convert.ToInt64(awd.udo_participantid);
                        }
                        //else
                        //{
                        //    _responseMessage = "Participant ID not found. Cannot retrieve payments.";
                        //    return false;
                        //}
                        if (awd.udo_payeecode1 != null)
                        {
                            _payeecode = awd.udo_payeecode1.ToString();
                        }
                        else
                        {
                            _responseMessage = "Payee Code not found. Cannot retrieve payments.";
                            return false;
                        }
                        if (awd.OwnerId != null)
                        {
                            _ownerType = awd.OwnerId.LogicalName;
                            _ownerId = awd.OwnerId.Id;
                        }
                        else
                        {
                            _responseMessage = "Owner ID not found. Cannot retrieve payments.";
                            return false;
                        }

                        if (string.IsNullOrEmpty(_fileNumber) && _PID < 1)
                        {
                            _responseMessage = "Filenumber and PID are blank. Cannot retrieve payments";
                            return false;
                        }


                    }

                    if (!gotData)
                    {
                        ////Logger.WriteDebugMessage("did not get data the first time");

                        var getParent2 = from awd in xrm.udo_payeecodeSet
                                         //join people in xrm.udo_personSet on awd.udo_payeecodeId.Value equals people.udo_payeecodeid.Id
                                         join vet in xrm.ContactSet on awd.udo_VeteranId.Id equals vet.ContactId.Value
                                         where awd.udo_payeecodeId.Value == _payeecodeId
                                         select new
                                         {
                                             awd.udo_paymentloadcomplete,
                                             awd.udo_VeteranId,
                                             awd.udo_name,
                                             awd.udo_payeecode1,
                                             awd.udo_filenumber,
                                             awd.udo_participantid,
                                             //people.udo_personId,
                                             vet.OwnerId
                                         };

                        foreach (var awd in getParent2)
                        {
                            gotData = true;

                            //if (awd.udo_personId.HasValue)
                            //{
                            //    _peopleId = awd.udo_personId.Value;
                            //}
                            ////Logger.WriteDebugMessage("person to pass is:" + _peopleId);
                            if (awd.udo_VeteranId != null)
                            {
                                _veteranId = awd.udo_VeteranId.Id;
                            }
                            if (awd.udo_paymentloadcomplete != null)
                            {
                                if (awd.udo_paymentloadcomplete.Value)
                                {
                                    Complete = true;
                                    return false;
                                }
                            }
                            if (awd.udo_filenumber != null)
                            {
                                _fileNumber = awd.udo_filenumber;
                            }
                            else
                            {
                                _responseMessage = "File Number not found. Cannot retrieve payments.";
                                return false;
                            }
                            if (awd.udo_participantid != null)
                            {
                                _PID = Convert.ToInt64(awd.udo_participantid);
                            }
                            else
                            {
                                _responseMessage = "Participant ID not found. Cannot retrieve payments.";
                                return false;
                            }
                            if (awd.udo_payeecode1 != null)
                            {
                                _payeecode = awd.udo_payeecode1.ToString();
                            }
                            else
                            {
                                _responseMessage = "Payee Code not found. Cannot retrieve payments.";
                                return false;
                            }
                            if (awd.OwnerId != null)
                            {
                                _ownerType = awd.OwnerId.LogicalName;
                                _ownerId = awd.OwnerId.Id;
                            }
                            else
                            {
                                _responseMessage = "Owner ID not found. Cannot retrieve payments.";
                                return false;
                            }
                        }
                    }
                }

                Logger.setMethod = "didWeNeedPayeeCodeData";
                ////Logger.WriteDebugMessage("gotData:" + gotData);
                return gotData;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedPayeeCodeData due to: {0}".Replace("{0}", ex.Message));
            }
        }
        #endregion

        #region Create Payment Details
        private void CreatePaymentDetails()
        {
            var request = new UDOgetPaymentDetailsRequest();

            UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);
            GetSettingValues();

            var veteranReference = new UDOcreateRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_paymentReference = new UDOcreateRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_paymentid",
                RelatedEntityId = _paymentId,
                RelatedEntityName = "udo_payment"
            };
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            var references = new[] { veteranReference, udo_paymentReference };
            request.UDOcreateRelatedEntitiesInfo = references;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.Debug = _debug;
            request.udo_paymentId = _paymentId;
            request.PaymentId = _lngPaymentId;
            request.FbtId = _fbtId;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.udo_personId = _peopleId;
            request.vetsnapshotId = _vetSnapId;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;

            ////Logger.WriteDebugMessage("Request Created");
            LogSettings _logSettings = new LogSettings() { Org = PluginExecutionContext.OrganizationName, ConfigFieldName = "RESTCALL", UserId = PluginExecutionContext.InitiatingUserId };
            tracer.Trace("_fbtId");
            tracer.Trace(_fbtId.ToString());
            tracer.Trace("_lngPaymentId");
            tracer.Trace(_lngPaymentId.ToString());


            tracer.Trace("calling UDOgetPaymentDetailsRequest");
            var response = Utility.SendReceive<UDOgetPaymentDetailsResponse>(_uri, "UDOgetPaymentDetailsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);

            tracer.Trace("Returned from UDOgetPaymentDetailsRequest");

            if (response.ExceptionOccurred)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Diaries LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
            }
        }

        private bool didWeNeedPaymentData()
        {
            try
            {
                tracer.Trace("didWeNeedPaymentData started");
                Logger.setMethod = "didWeNeedPaymentData";
                var gotData = false;

                using (var xrm = new UDOContext(OrganizationService))
                {
                    var getParent = from awd in xrm.udo_paymentSet
                                    where awd.udo_paymentId.Value == _paymentId
                                    select new
                                    {
                                        awd.udo_PaymentDetailsComplete,
                                        awd.udo_PaymentDetailsMessage,
                                        awd.udo_PaymentIdentifier,
                                        awd.udo_TransactionID,
                                        awd.udo_VeteranId,
                                        awd.OwnerId,
                                        awd.udo_VeteranSnapShotId,
                                        awd.udo_PersonId
                                    };
                    foreach (var awd in getParent)
                    {
                        gotData = true;

                        if (awd.udo_VeteranId != null)
                        {
                            _veteranId = awd.udo_VeteranId.Id;
                        }
                        if (awd.udo_PersonId != null)
                        {
                            _peopleId = awd.udo_PersonId.Id;
                        }
                        if (awd.udo_PaymentDetailsComplete != null)
                        {
                            if (awd.udo_PaymentDetailsComplete.Value)
                            {
                                Complete = true;
                                return false;
                            }
                        }
                        if (awd.udo_PaymentIdentifier != null)
                        {
                            _lngPaymentId = Int64.Parse(awd.udo_PaymentIdentifier);
                        }
                        if (awd.udo_TransactionID != null)
                        {
                            _fbtId = Int64.Parse(awd.udo_TransactionID);
                        }
                        if (awd.OwnerId != null)
                        {
                            _ownerType = awd.OwnerId.LogicalName;
                            _ownerId = awd.OwnerId.Id;
                        }
                        else
                        {
                            _responseMessage = "No Owner ID found. Cannot get Payment details.";
                            return false;
                        }
                        if (awd.udo_VeteranSnapShotId != null)
                        {
                            _vetSnapId = awd.udo_VeteranSnapShotId.Id;
                        }

                    }
                }

                Logger.setMethod = "didWeNeedPaymentData";

                return gotData;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedPaymentData due to: {0}".Replace("{0}", ex.Message));
            }
        }
        #endregion

    }
}
