using MCSPlugins;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using UDO.LOB.ExamsAppointments.Messages;
using UDO.Model;
using UDO.LOB.Core;
using VRMRest;

namespace CustomActions.Plugins.Entities.ExamAndAppointments
{
    class UDOGetAppointmentsRunner : UDOActionRunner
    {
        protected Guid _veteranId = new Guid();
        protected Guid _idproofId = new Guid();
        protected Guid _examId = new Guid();
        protected Guid _ownerId = new Guid();
        protected string _ownerType;
        protected string _nationId = string.Empty;

        public UDOGetAppointmentsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_examlogtimer";
            _logSoapField = "udo_examlogsoap";
            _debugField = "udo_exam";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_examvimttimeout";
            _validEntities = new string[] { "udo_examandappontment" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            _method = "DoAction";
            _examId = Parent.Id;

            if (!DidWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();
            UDOHeaderInfo _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            var request = new UDOcreateUDOAppointmentsRequest();
            var veteranReference = new UDOcreateUDOAppointmentsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };

            var udo_examapptReference = new UDOcreateUDOAppointmentsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_examandappointmentid",
                RelatedEntityId = _examId,
                RelatedEntityName = "udo_examandappontment"
            };
            var references = new[] { veteranReference, udo_examapptReference };
            
            request.UDOcreateUDOAppointmentRelatedEntitiesInfo = references;
            request.udo_appointmentId = _examId;
            request.LegacyServiceHeaderInfo = _headerInfo;

            request.transactionId = "";
            request.ICN = _nationId;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;

            request.RelatedParentEntityName = "udo_examandappontment";
            request.RelatedParentFieldName = "udo_examandappontmentid";
            request.RelatedParentId = _examId;

            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;

            ////Logger.WriteDebugMessage("Request Created");

            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            tracer.Trace("calling UDOcreateUDOAppointmentsRequest");
            var response = Utility.SendReceive<UDOcreateUDOAppointmentsResponse>(_uri, "UDOcreateUDOAppointmentsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateUDOAppointmentsRequest");

            if (response.ExceptionOccurred)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Appointments LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
            }
        }

        private bool DidWeNeedData()
        {
            try
            {
                tracer.Trace("DidWeNeedData started");
                Logger.setMethod = "DidWeNeedData";
                var gotData = false;

                using (var xrm = new UDOContext(OrganizationService))
                {
                    var getParent = (from exam in xrm.udo_examandappontmentSet
                                    join vet in xrm.ContactSet on exam.udo_veteranid.Id equals vet.ContactId.Value
                                    where exam.udo_examandappontmentId == _examId
                                    select new
                                    {
                                        exam.udo_AppointmentCompleted,
                                        exam.udo_veteranid,
                                        exam.udo_IdProofId,
                                        vet.udo_icn,
                                        vet.OwnerId

                                    }).FirstOrDefault();
                    if (getParent != null)
                    {
                        gotData = true;
                        if (getParent.OwnerId != null)
                        {
                            _ownerType = getParent.OwnerId.LogicalName;
                            _ownerId = getParent.OwnerId.Id;
                        }
                        else
                        {
                            return false;
                        }
                        if (getParent.udo_veteranid == null || getParent.udo_IdProofId == null)
                        {
                            _responseMessage = "No Veteran ID or IDProofID. Cannot retrieve Exams and Exam Requests";
                            return false;
                        }

                        if (getParent.udo_AppointmentCompleted != null)
                        {
                            if (getParent.udo_AppointmentCompleted.Value)
                            {
                                Complete = true;
                                return false;
                            }
                        }

                        if (getParent.udo_veteranid != null)
                        {
                            _veteranId = getParent.udo_veteranid.Id;
                        }

                        if (getParent.udo_IdProofId != null)
                        {
                            _idproofId = getParent.udo_IdProofId.Id;
                        }

                        if (!string.IsNullOrEmpty(getParent.udo_icn))
                        {
                            _nationId = getParent.udo_icn;
                        }
                        else
                        {
                            _responseMessage = "No ICN ID found. Cannot retrieve Appointments";
                            return false;
                        }
                    }
                }
                ////Logger.WriteDebugMessage("Ending didWeNeedData Method");
                Logger.setMethod = "Execute";

                return gotData;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
    }
}
