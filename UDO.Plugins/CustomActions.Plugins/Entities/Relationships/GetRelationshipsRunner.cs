using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
//using VRM.Integration.UDO.Contact.Messages;
using VRMRest;
using System.ServiceModel;
using System.Diagnostics;
using UDO.Model;
//using VRM.Integration.UDO.Common.Messages;
using UDO.LOB.Core;
using UDO.LOB.Contact.Messages;

namespace CustomActions.Plugins.Entities.Relationships
{
    internal class GetRelationshipsRunner : UDOActionRunner
    {

        protected string _ptcpntId = "";
        protected Guid _veteranId = new Guid();
        protected Guid _ownerId = new Guid();
        protected string _ownerType;
        protected Guid _dependentId = new Guid();
        protected bool _veteranFocused = false;
        protected const string _veteranSerachField = "udo_veteranid";
        protected const string _dependentSearchField = "udo_dependentid";

        public GetRelationshipsRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_contactlogtimer";
            _logSoapField = "udo_contactlogsoap";
            _debugField = "udo_contact";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_contacttimeout";
            _validEntities = new string[] { "udo_dependant", "contact" };
        }

        public override void DoAction()
        {

            if (Parent.LogicalName == UDO.Model.Contact.EntityLogicalName)
            {
                _veteranId = Parent.Id;
                _veteranFocused = true;
            }

            if (Parent.LogicalName == udo_dependant.EntityLogicalName)
            {
                _dependentId = Parent.Id;
                _veteranFocused = false;
            }

            if (!DidWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();

            DeleteRelationships();

            var request = new UDOcreateRelationshipsRequest();
            request.VeteranId = _veteranId;

            GetSettingValues();

            var veteranReference = new UDOcreateRelationshipsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var references = new[] { veteranReference };
            if (!_veteranFocused)
            {
                var dependentReference = new UDOcreateRelationshipsRelatedEntitiesMultipleRequest()
                {
                    RelatedEntityFieldName = "udo_dependentid",
                    RelatedEntityId = _dependentId,
                    RelatedEntityName = "udo_dependant"
                };
                references = new[] { veteranReference, dependentReference };
                request.DependentId = _dependentId;
            }
            UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            request.UDOcreateRelationshipsRelatedEntitiesInfo = references;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.RelatedParentEntityName = "contact";
            request.RelatedParentFieldName = "udo_contactid";
            request.RelatedParentId = _veteranId;
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.ptcpntId = _ptcpntId;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = "CustomActions.Plugins.Entities.Relationships"
            };

            var response = Utility.SendReceive<UDOcreateRelationshipsResponse>(_uri, "UDOcreateRelationshipsRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);

        }

        private void DeleteRelationships()
        {
            tracer.Trace("DeleteRelationships started");
            Logger.setMethod = "DeleteRelationships";

            string searchField = string.Empty;
            Guid searchGuid = new Guid();

            if (_veteranFocused)
            {
                searchField = _veteranSerachField;
                searchGuid = _veteranId;
            }
            else
            {
                searchField = _dependentSearchField;
                searchGuid = _dependentId;
            }

            var fetchRelationships = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                      <entity name='udo_relationships'>
                                                        <attribute name='udo_relationshipsid' />
                                                        <order attribute='udo_name' descending='false' />
                                                        <filter type='and'>
                                                          <condition attribute='{0}' operator='eq' value='{1}' />
                                                        </filter>
                                                      </entity>
                                                    </fetch>", searchField, searchGuid);

            var response = ElevatedOrganizationService.RetrieveMultiple(new FetchExpression(fetchRelationships));

            if (response.Entities.Count > 0)
            {
                tracer.Trace(string.Format("{0} to delete.", response.Entities.Count));

                foreach (var relationship in response.Entities)
                {
                    ElevatedOrganizationService.Delete(udo_relationships.EntityLogicalName, relationship.Id);
                }
            }
        }

        private bool DidWeNeedData()
        {
            try
            {
                tracer.Trace("DidWeNeedData started");
                Logger.setMethod = "DidWeNeedData";
                var gotData = false;

                if (_veteranFocused)
                {
                    var parentFetch = String.Format("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' >" +
                                                      "<entity name='contact' >" +
                                                        "<attribute name='udo_participantid' />" +
                                                        "<attribute name='ownerid' />" +
                                                        "<filter type='and' >" +
                                                          "<condition attribute='contactid' operator='eq' value='{0}' />" +
                                                        "</filter>" +
                                                      "</entity>" +
                                                    "</fetch>", _veteranId);

                    var response = OrganizationService.RetrieveMultiple(new FetchExpression(parentFetch));

                    if (response.Entities.Count > 0)
                    {
                        gotData = true;
                        var parent = response.Entities.FirstOrDefault().ToEntity<UDO.Model.Contact>();
                        if (parent.OwnerId != null)
                        {
                            _ownerType = parent.OwnerId.LogicalName;
                            _ownerId = parent.OwnerId.Id;
                        }
                        else
                        {
                            return false;
                        }
                        if (parent.udo_ParticipantId != null)
                        {
                            _ptcpntId = parent.udo_ParticipantId;
                        }
                        else
                        {
                            _responseMessage = "No PID found on this record; cannot retrieve related relationships";
                            return false;
                        }
                    }
                }
                else
                {
                    var parentFetch = String.Format("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' >" +
                                                      "<entity name='udo_dependant' >" +
                                                        "<attribute name='udo_ptcpntid' />" +
                                                        "<attribute name='ownerid' />" +
                                                        "<attribute name='udo_veteranid' />" +
                                                        "<filter type='and' >" +
                                                          "<condition attribute='udo_dependantid' operator='eq' value='{0}' />" +
                                                        "</filter>" +
                                                      "</entity>" +
                                                    "</fetch>", _dependentId);

                    var response = OrganizationService.RetrieveMultiple(new FetchExpression(parentFetch));

                    if (response.Entities.Count > 0)
                    {
                        gotData = true;
                        var parent = response.Entities.FirstOrDefault().ToEntity<udo_dependant>();

                        if (parent.OwnerId != null)
                        {
                            _ownerType = parent.OwnerId.LogicalName;
                            _ownerId = parent.OwnerId.Id;
                        }
                        else
                        {
                            return false;
                        }
                        if (parent.udo_PtcpntID != null)
                        {
                            _ptcpntId = parent.udo_PtcpntID;
                        }
                        else
                        {
                            _responseMessage = "No PID found on this record. Cannot retrieve related relationships";
                            return false;
                        }
                        if (parent.udo_VeteranId != null)
                        {
                            _veteranId = parent.udo_VeteranId.Id;
                        }
                        else
                        {
                            _responseMessage = "No Veteran ID found on this record. Cannot retrieve related relationships";
                            return false;
                        }
                    }
                }

                Logger.setMethod = "DoAction";

                return gotData;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
    }
}
