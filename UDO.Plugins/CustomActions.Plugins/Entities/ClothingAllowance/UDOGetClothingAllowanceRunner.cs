using System;
using System.Linq;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using VRMRest;
using UDO.Model;

using UDO.LOB.Core;
using UDO.LOB.Awards.Messages;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.Awards.Messages;

namespace CustomActions.Plugins.Entities.ClothingAllowance
{
    public class UDOGetClothingAllowanceRunner : UDOActionRunner
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
        protected bool _veteranFocused = false;

        public UDOGetClothingAllowanceRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_awardlogtimer";
            _logSoapField = "udo_awardlogsoap";
            _debugField = "udo_award";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_clothingallowancetimeout";
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
            UDOHeaderInfo _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            var request = new UDOcreateClothingAllowanceRequest();
            request.AwardId = _awardId;

            var udo_awardReference = new UDOcreateClothingAllowanceRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_awardid",
                RelatedEntityId = _awardId,
                RelatedEntityName = "udo_award"
            };
            var veteranReference = new UDOcreateClothingAllowanceRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var references = new[] { veteranReference, udo_awardReference };

            /*
            // TODO: Check for dependent on clothing
            if (!_veteranFocused)
            {
                var dependentReference = new UDOcreateClothingAllowanceRelatedEntitiesMultipleRequest()
                {
                    RelatedEntityFieldName = "udo_dependentid",
                    RelatedEntityId = _dependentId,
                    RelatedEntityName = "udo_dependent"
                };
                references = new[] { dependentReference, udo_awardReference, veteranReference };
            }
            */

            request.UDOcreateClothingAllowanceRelatedEntitiesInfo = references;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.Debug = _debug;
            request.RelatedParentEntityName = "udo_award";
            request.RelatedParentFieldName = "udo_awardid";
            request.RelatedParentId = _awardId;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            request.awardTypeCd = _awardTypeCode;
            request.ptcpntBeneId = String.IsNullOrEmpty(_beneId) ? _PID : _beneId;
            request.ptcpntRecipId = String.IsNullOrEmpty(_recipId) ? _PID : _recipId;
            request.ptcpntVetId = _PID;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;

            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            tracer.Trace("calling UDOcreateClothingAllowanceRequest");
            var response = Utility.SendReceive<UDOcreateClothingAllowanceResponse>(_uri, "UDOcreateClothingAllowanceRequest", request, _logSettings, _timeOutSetting,_crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateClothingAllowanceRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Clothing Allowance LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
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
                                        awd.udo_clothingallowanceComplete,
                                        awd.udo_CallComplete,
                                        awd.udo_VeteranId,
                                        awd.udo_AwardTypeCode,
                                        awd.udo_PtcpntBeneID,
                                        awd.udo_PtcpntRecipID,
                                        vet.udo_ParticipantId,
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
                                _responseMessage = "Call complete. Cannot retrieve Award Lines.";
                                return false;
                            }
                        }
                        if (awd.udo_clothingallowanceComplete != null)
                        {
                            if (awd.udo_clothingallowanceComplete.Value)
                            {
                                Complete = true;
                                return false;
                            }
                        }
                        if (awd.udo_VeteranId != null)
                        {
                            _veteranId = awd.udo_VeteranId.Id;
                        }
                        if (awd.udo_AwardTypeCode != null)
                        {
                            _awardTypeCode = awd.udo_AwardTypeCode;
                        }
                        else
                        {
                            _responseMessage = "Award Type Code not found. Cannot retrieve Award Lines.";
                            return false;
                        }
                        if (awd.udo_PtcpntBeneID != null)
                        {
                            _beneId = awd.udo_PtcpntBeneID;
                        }

                        if (awd.udo_PtcpntRecipID != null)
                        {
                            _recipId = awd.udo_PtcpntRecipID;
                        }

                        if (awd.udo_ParticipantId != null)
                        {
                            _PID = awd.udo_ParticipantId;
                        }
                        else
                        {
                            _responseMessage = "Participant Id not found. Cannot retrieve Award Lines.";
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
