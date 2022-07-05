using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using VRMRest;
using UDO.Model;
using System.Diagnostics;
using System.ServiceModel;
using UDO.LOB.Core;
using UDO.LOB.Denials.Messages;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.Denials.Messages;

namespace CustomActions.Plugins.Entities.Denials
{
    public class UDOGetDenialsRunner : UDOActionRunner
    {
        Guid _veteranId = new Guid();
        Guid _IDProofId = new Guid();
        Guid _ownerId = new Guid();
        string _ownerType;
        string _PID = "";

        public UDOGetDenialsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_deniallogtimer";
            _logSoapField = "udo_deniallogsoap";
            _debugField = "udo_denial";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_denialvimttimeout";
            _validEntities = new string[] { "udo_idproof" };
        }

        public override void DoAction()
        {
            _method = "DoAction";
            _IDProofId = Parent.Id;

            if (!didWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();
            UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            var request = new UDOcreateDenialsRequest();

            var idReference = new UDOcreateDenialsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_idproofid",
                RelatedEntityId = _IDProofId,
                RelatedEntityName = "udo_idproof"
            };
            var veteranReference = new UDOcreateDenialsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var references = new[] { veteranReference, idReference };

            request.UDOcreateDenialsRelatedEntitiesInfo = references;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.RelatedParentEntityName = "contact";
            request.RelatedParentFieldName = "udo_contactid";
            request.RelatedParentId = _veteranId;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            request.ptcpntId = _PID;
            request.VeteranId = _veteranId;
            request.udo_idproof = _IDProofId;

            //  //Logger.WriteDebugMessage("Request Created _PID:" + _PID);
            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = "DenialsRetrieveExecute"
            };

            tracer.Trace("calling UDOcreateDenialsRequest");
            var response = Utility.SendReceive<UDOcreateDenialsResponse>(_uri, "UDOcreateDenialsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateDenialsRequest");

            if (response.ExceptionOccurred)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Denials LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
            }

        }

        internal bool didWeNeedData()
        {
            try
            {

                tracer.Trace("DidWeNeedData started");
                Logger.setMethod = "DidWeNeedData";
                var gotData = false;

                using (var xrm = new UDOContext(OrganizationService))
                {
                    var getParent = (from awd in xrm.udo_idproofSet
                                    join vet in xrm.ContactSet on awd.udo_Veteran.Id equals vet.ContactId.Value
                                    where awd.udo_idproofId.Value == _IDProofId
                                    select new
                                    {
                                        vet.udo_ParticipantId,
                                        vet.udo_FileNumber,
                                        vet.ContactId,
                                        awd.udo_denialscomplete,
                                        vet.OwnerId,
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

                        if (getParent.udo_denialscomplete != null)
                        {
                            if (getParent.udo_denialscomplete.Value)
                            {
                                Complete = true;
                                // //Logger.WriteDebugMessage("denial already complete");
                                return false;
                            }
                        }
                        if (getParent.udo_ParticipantId != null)
                        {
                            _PID = getParent.udo_ParticipantId;
                            /// //Logger.WriteDebugMessage("Request Created _PID:" + _PID);

                        }
                        else
                        {
                            return false;
                        }
                        _veteranId = getParent.ContactId.Value;

                        /////Logger.WriteDebugMessage("Request Created _PID:" + _PID);

                    }
                }
                // //Logger.WriteDebugMessage("Ending didWeNeedData Method");
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