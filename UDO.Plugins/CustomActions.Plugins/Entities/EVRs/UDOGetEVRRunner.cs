using System;
using System.Linq;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using UDO.LOB.Core;
using UDO.LOB.Awards.Messages;
using MCSUtilities2011;
using VRMRest;
using UDO.Model;

namespace CustomActions.Plugins.Entities.EVRs
{
    public class UDOGetEVRRunner : UDOActionRunner
    {
        protected Guid _awardId = new Guid();
        protected Guid _ownerId = new Guid();
        protected Guid _veteranId = new Guid();
        protected Guid _dependentId = new Guid();
        protected string _ownerType = string.Empty;
        protected string _awardTypeCode = string.Empty;
        protected string _beneId = string.Empty;
        protected string _recipId = string.Empty;
        protected string _PID = string.Empty;
        protected string _fileNumber = string.Empty;
        protected bool _veteranFocused;


        public UDOGetEVRRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_awardlogtimer";
            _logSoapField = "udo_awardlogsoap";
            _debugField = "udo_award";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_evrsvimttimeout";
            _validEntities = new string[] { "udo_award" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            _method = "DoAction";
            _awardId = Parent.Id;

            if (!DidWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();
            UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            var request = new UDOcreateEVRsRequest();
            request.AwardId = _awardId;

            var veteranReference = new UDOcreateEVRsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var udo_awardReference = new UDOcreateEVRsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_awardid",
                RelatedEntityId = _awardId,
                RelatedEntityName = "udo_award"
            };
            var references = new[] { veteranReference, udo_awardReference };
            request.UDOcreateEVRsRelatedEntitiesInfo = references;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.RelatedParentEntityName = "udo_award";
            request.RelatedParentFieldName = "udo_awardid";
            request.RelatedParentId = _awardId;
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.fileNumber = _fileNumber;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;

            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            tracer.Trace("calling UDOcreateEVRsRequest");
            var response = Utility.SendReceive<UDOcreateEVRsResponse>(_uri, "UDOcreateEVRsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateEVRsRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create EVRs LOB.";

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
                    var getParent = from awd in xrm.udo_awardSet
                                    join vet in xrm.ContactSet on awd.udo_VeteranId.Id equals vet.ContactId.Value
                                    where awd.udo_awardId.Value == _awardId
                                    select new
                                    {
                                        awd.udo_evrcomplete,
                                        awd.udo_CallComplete,
                                        awd.udo_VeteranId,
                                        vet.udo_FileNumber,
                                        vet.OwnerId
                                    };
                    foreach (var awd in getParent)
                    {
                        gotData = true;

                        if (awd.OwnerId != null)
                        {
                            _ownerType = awd.OwnerId.LogicalName;
                            _ownerId = awd.OwnerId.Id;
                        }
                        else
                        {

                            return false;
                        }
                        if (awd.udo_CallComplete != null)
                        {
                            if (awd.udo_CallComplete.Value)
                            {
                                _responseMessage = "Call complete. Cannot retrieve EVRs.";
                                return false;
                            }
                        }
                        if (awd.udo_evrcomplete != null)
                        {
                            if (awd.udo_evrcomplete.Value)
                            {
                                Complete = true;
                                return false;
                            }
                        }
                        if (awd.udo_VeteranId != null)
                        {
                            _veteranId = awd.udo_VeteranId.Id;
                        }
                        if (awd.udo_FileNumber != null)
                        {
                            _fileNumber = awd.udo_FileNumber;
                        }
                        else
                        {
                            _responseMessage = "File Number not found. Cannot retrieve EVRs.";
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
    }
}

