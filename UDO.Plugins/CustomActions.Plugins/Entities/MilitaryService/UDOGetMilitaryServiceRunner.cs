using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using VRMRest;
using System.ServiceModel;
using UDO.Model;
using UDO.LOB.Core;
using UDO.LOB.MilitaryService.Messages;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.MilitaryService.Messages;

namespace CustomActions.Plugins.Entities.MilitaryService
{
    public class UDOGetMilitaryServiceRunner : UDOActionRunner
    {
        #region Members

        protected Guid _ownerId = new Guid();
        protected Guid _militaryServiceId = new Guid();
        protected Guid _veteranId = new Guid();
        protected string _ownerType;
        protected string _pid = "";

        #endregion

        public UDOGetMilitaryServiceRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_vbmsefolderlogtimer";
            _logSoapField = "udo_vbmsefolderlogsoap";
            _debugField = "udo_vbmsefolder";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_vbmsefoldervimttimeout";
            _validEntities = new string[] { "udo_militaryservice" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            tracer = base.TracingService;
            tracer.Trace("UDOGetMilitaryServiceRunner started");

            var txnTimer = Stopwatch.StartNew();
            GetSettingValues();

            _militaryServiceId = Parent.Id;

            if (!DidWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();
           
            var request = new UDOfindMilitaryServiceRequest();
            var veteranReference = new UDOcreateUDOMilitaryServiceRelatedEntitiesMultipleRequest()
                {
                    RelatedEntityFieldName = "udo_veteranid",
                    RelatedEntityId = _veteranId,
                    RelatedEntityName = "contact"
                };
            var udo_militaryserviceReference = new UDOcreateUDOMilitaryServiceRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_militaryserviceid",
                RelatedEntityId = _militaryServiceId,
                RelatedEntityName = "udo_militaryservice"
            };

            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            var references = new[] { veteranReference, udo_militaryserviceReference };
            request.UDOcreateUDOMilitaryServiceRelatedEntitiesInfo = references;

            UDOHeaderInfo headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);
            request.LegacyServiceHeaderInfo = headerInfo;
            request.udo_militaryserviceId = _militaryServiceId;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.ptcpntId = _pid;
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            ////Logger.WriteDebugMessage("Request Created");
            tracer.Trace("Request Created");
            LogSettings logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                //callingMethod = Logger.setModule
            };

            tracer.Trace("calling UDOCreateMilitaryServiceRequest");
            var response = Utility.SendReceive<UDOfindMilitaryServiceResponse>(_uri, "UDOfindMilitaryServiceRequest", request, logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOCreateMilitaryServiceRequest");
            ////Logger.WriteDebugMessage("back from EC");

            if (response.ExceptionOccurred)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Military Service LOB.";

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

                    var getParent = (from mis in xrm.udo_militaryserviceSet
                                     join vet in xrm.ContactSet on mis.udo_VeteranId.Id equals vet.ContactId.Value
                                     where mis.udo_militaryserviceId.Value == _militaryServiceId
                                     select new
                                     {
                                         mis.udo_IdProofId,
                                         mis.udo_MilitaryServiceComplete,
                                         mis.udo_MilitaryServiceMessage,
                                         mis.udo_VeteranId,
                                         vet.udo_ParticipantId,
                                         vet.udo_FileNumber,
                                         vet.OwnerId
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
                            _responseMessage = "Veteran ID not found. Cannot retrieve Military Service.";
                            return false;
                        }

                        if (getParent.udo_ParticipantId != null)
                        {
                            _pid = getParent.udo_ParticipantId;
                        }
                        else
                        {
                            _responseMessage = "No PID found. Cannot get Military Information.";
                            return false;
                        }

                        if (getParent.udo_MilitaryServiceComplete.HasValue)
                        {
                            if (getParent.udo_MilitaryServiceComplete.Value)
                            {
                                Complete = true;
                                return false;
                            }
                        }

                        _ownerId = getParent.OwnerId.Id;
                        _ownerType = getParent.OwnerId.LogicalName;

                    }

                    tracer.Trace("didWeNeedData have been retrieved and set");
                    Logger.setMethod = _method;
                    return gotData;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }      
    }
}
