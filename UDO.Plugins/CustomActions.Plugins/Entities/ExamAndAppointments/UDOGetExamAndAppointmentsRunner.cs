using MCSPlugins;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using UDO.LOB.ExamsAppointments.Messages;
using UDO.LOB.Core;
using UDO.Model;
using VRMRest;

namespace CustomActions.Plugins.Entities.ExamAndAppointments
{
    internal class UDOGetExamAndAppointmentsRunner : UDOActionRunner
    {
        Guid _veteranId = new Guid();
        Guid _idProofId = new Guid();
        Guid _ownerId = new Guid();
        Guid _appointmentId = new Guid();
        Guid _examId = new Guid();
        string _ownerType;
        string _nationId = "";
        private UDOHeaderInfo _headerInfo;

        public UDOGetExamAndAppointmentsRunner(IServiceProvider serviceProvider)
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

            if (!didWeNeedExamAndAppointmentData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();
             _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);


            CreateExams();
            CreateExamRequests();

        }

        #region Create Exams And ExamRequests

        private void CreateExams()
        {
            Logger.setMethod = "InvokeExam";

            var examRequest = new UDOcreateUDOExamRequest();
            examRequest.udo_examId = _examId;

            var veteranReference = new UDOcreateUDOExamsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };

            var udo_examapptReference = new UDOcreateUDOExamsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_examandappointmentid",
                RelatedEntityId = _examId,
                RelatedEntityName = "udo_examandappontment"
            };
            var references = new[] { veteranReference, udo_examapptReference };

            examRequest.UDOcreateUDOExamRelatedEntitiesInfo = references;
            examRequest.LegacyServiceHeaderInfo = _headerInfo;
            examRequest.udo_examId = _examId;
            examRequest.transactionId = "";
            examRequest.ICN = _nationId;

            examRequest.RelatedParentEntityName = "udo_examandappontment";
            examRequest.RelatedParentFieldName = "udo_examandappontmentid";
            examRequest.RelatedParentId = _examId;
            examRequest.ownerId = _ownerId;
            examRequest.ownerType = _ownerType;
            examRequest.MessageId = PluginExecutionContext.CorrelationId.ToString();

            examRequest.Debug = _debug;
            examRequest.LogSoap = _logSoap;
            examRequest.LogTiming = _logTimer;
            examRequest.UserId = PluginExecutionContext.InitiatingUserId;
            examRequest.OrganizationName = PluginExecutionContext.OrganizationName;

            LogSettings _logSettings = new LogSettings() { Org = PluginExecutionContext.OrganizationName, ConfigFieldName = "RESTCALL", UserId = PluginExecutionContext.InitiatingUserId };

            tracer.Trace("calling UDOcreateUDOExamRequest");
            var response = Utility.SendReceive<UDOcreateUDOExamRequestResponse>(_uri, "UDOcreateUDOExamRequest", examRequest, _logSettings, _timeOutSetting,_crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateUDOExamRequest");

            if (response.ExceptionOccurred)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Exam LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
            }
        }


        private void CreateExamRequests()
        {
            Logger.setMethod = "InvokeExamRequest";

            var exmrequestRequest = new UDOcreateUDOExamRequestRequest();
            exmrequestRequest.udo_examId = _examId;

            var udo_examapptReference = new UDOcreateUDOExamRequestRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_examandappointmentid",
                RelatedEntityId = _examId,
                RelatedEntityName = "udo_examandappontment"
            };
            var references = new[] { udo_examapptReference };


            exmrequestRequest.UDOcreateUDOExamRelatedEntitiesInfo = references;
            //exmrequestRequest.LegacyServiceHeaderInfo = _headerInfo;
            exmrequestRequest.HeaderInfo = _headerInfo;
            exmrequestRequest.udo_examId = _examId;
            exmrequestRequest.ownerId = _ownerId;
            exmrequestRequest.ownerType = _ownerType;
            exmrequestRequest.transactionId = "";
            exmrequestRequest.ICN = _nationId;

            exmrequestRequest.RelatedParentEntityName = "udo_examandappontment";
            exmrequestRequest.RelatedParentFieldName = "udo_examandappontmentid";
            exmrequestRequest.RelatedParentId = _examId;

            exmrequestRequest.MessageId = PluginExecutionContext.CorrelationId.ToString();

            exmrequestRequest.Debug = _debug;
            exmrequestRequest.LogSoap = _logSoap;
            exmrequestRequest.LogTiming = _logTimer;
            exmrequestRequest.UserId = PluginExecutionContext.InitiatingUserId;
            exmrequestRequest.OrganizationName = PluginExecutionContext.OrganizationName;

            LogSettings _logSettings = new LogSettings() { Org = PluginExecutionContext.OrganizationName, ConfigFieldName = "RESTCALL", UserId = PluginExecutionContext.InitiatingUserId };


            tracer.Trace("calling UDOcreateUDOExamRequestRequest");
            var response = Utility.SendReceive<UDOcreateUDOExamRequestResponse>(_uri, "UDOcreateUDOExamRequestRequest", exmrequestRequest, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateUDOExamRequestRequest");

            if (response.ExceptionOccurred)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Exam Request LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
            }
        }

        internal bool didWeNeedExamAndAppointmentData()
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
                                        exam.udo_ExamCompleted,
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
                            ////Logger.WriteDebugMessage("No Veteran ID or IDProofID");
                            _responseMessage = "No Veteran ID or IDProofID. Cannot retrieve Exams and Exam Requests";
                            return false;
                        }

                        if (getParent.udo_ExamCompleted != null)
                        {
                            if (getParent.udo_ExamCompleted.Value)
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
                            _idProofId = getParent.udo_IdProofId.Id;
                        }

                        if (!string.IsNullOrEmpty(getParent.udo_icn))
                        {
                            _nationId = getParent.udo_icn;
                        }
                        else
                        {
                            ////Logger.WriteDebugMessage("No ICN ID found");
                            _responseMessage = "No ICN ID found. Cannot retrieve Exams and Exam Requests";
                            return false;
                        }
                    }
                }

                Logger.setMethod = "Execute";

                return gotData;
            }
            catch (Exception ex)
            {
                Logger.WriteToFile(ex.Message);
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message) + "STACKTRACEL " + ex.StackTrace);
            }
        }
        #endregion        
    }
}
