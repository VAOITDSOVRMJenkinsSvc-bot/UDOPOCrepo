using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using UDO.LOB.Contact.Messages;
using UDO.Model;
using UDO.LOB.Core;
using VRMRest;

namespace CustomActions.Plugins.Entities.Address
{
    internal class GetAddressesRunner : UDOActionRunner
    {
        protected Guid _ownerId = new Guid();
        protected string _ownerType;
        protected long _ptcpntId;
        protected string _fileNumber;
        protected Guid _veteranId = new Guid();
        protected Guid _dependentId = new Guid();
        protected bool _veteranFocused = false;
        protected Guid _veteranSnapShotId = new Guid();
        protected const string _veteranSerachField = "udo_veteranid";
        protected const string _dependentSearchField = "udo_dependentid";

        public GetAddressesRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_contactlogtimer";
            _logSoapField = "udo_contactlogsoap";
            _debugField = "udo_dependent";
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

           var veteranSnapshotRef =  PluginExecutionContext.InputParameters["VeteranSnapshotReference"] as EntityReference;

           if (veteranSnapshotRef != null)
               _veteranSnapShotId = veteranSnapshotRef.Id;

            if (!DidWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();

            DeleteAddresses();
            Logger.setMethod = "Execute";
            var request = new UDOcreateAddressRecordsRequest();
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.VeteranId = _veteranId;

            var veteranReference = new UDOcreateAddressRecordsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };

            var references = new[] { veteranReference };

            if (!_veteranFocused)
            {
                var dependentReference = new UDOcreateAddressRecordsRelatedEntitiesMultipleRequest()
                {
                    RelatedEntityFieldName = "udo_dependentid",
                    RelatedEntityId = _dependentId,
                    RelatedEntityName = "udo_dependant"
                };
                references = new[] { veteranReference, dependentReference };
                request.DependentId = _dependentId;
            }

            
            UDOHeaderInfo _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            request.LegacyServiceHeaderInfo = _headerInfo;

            request.ptcpntId = _ptcpntId;
            request.UDOcreateAddressRecordsRelatedEntitiesInfo = references;
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
            request.fileNumber = _fileNumber;

            if (_veteranSnapShotId != Guid.Empty)
                request.vetsnapshotId = _veteranSnapShotId;

            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            tracer.Trace("Invoking UDOcreateAddressRecordsRequest");
            Logger.setDebug = request.Debug;
            var response = Utility.SendReceive<UDOcreateAddressRecordsResponse>(_uri, "UDOcreateAddressRecordsRequest", request, _logSettings, 
                _timeOutSetting, _crmAuthTokenConfig, tracer);            
            tracer.Trace($"Executed from UDOcreateAddressRecordsRequest. Response Details :: ExceptionOccured? : {response?.ExceptionOccured} ");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Create Address LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
            }
        }

        private void DeleteAddresses()
        {
            tracer.Trace("DeleteAddresses started");
            Logger.setMethod = "DeleteAddresses";

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

            var fetchAddresses = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                      <entity name='udo_address'>
                                                        <attribute name='udo_addressid' />
                                                        <order attribute='udo_name' descending='false' />
                                                        <filter type='and'>
                                                          <condition attribute='{0}' operator='eq' value='{1}' />
                                                        </filter>
                                                      </entity>
                                                    </fetch>", searchField, searchGuid);

            var response = ElevatedOrganizationService.RetrieveMultiple(new FetchExpression(fetchAddresses));

            if (response.Entities.Count > 0)
            {
                tracer.Trace(string.Format("{0} to delete.", response.Entities.Count));

                foreach (var address in response.Entities)
                {
                    ElevatedOrganizationService.Delete(udo_address.EntityLogicalName, address.Id);
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

                using (var xrm = new UDOContext(OrganizationService))
                {
                    if (_veteranFocused)
                    {
                        var getParent = from awd in xrm.ContactSet
                                        where awd.Id == _veteranId
                                        select new
                                        {
                                            awd.udo_FileNumber,
                                            awd.udo_ParticipantId,
                                            awd.OwnerId
                                        };
                        foreach (var awd in getParent)
                        {
                            gotData = true;
                            _veteranFocused = true;
                            if (awd.OwnerId != null)
                            {
                                _ownerType = awd.OwnerId.LogicalName;
                                _ownerId = awd.OwnerId.Id;
                            }
                            else
                            {
                                _responseMessage = "Invalid ownership of Contact record. Please contact your application support team with this message.";
                                return false;
                            }
                            if (awd.udo_ParticipantId != null)
                            {
                                _ptcpntId = Int64.Parse(awd.udo_ParticipantId);
                            }

                            if (awd.udo_FileNumber != null)
                            {
                                _fileNumber = awd.udo_FileNumber;
                            }
                            //else
                            //{
                            //    _responseMessage = "No PID was found on this Contact; cannot retrieve addresses.";
                            //    return false;
                            //}

                        }
                    }
                    else
                    {
                        var getDep = from awd in xrm.udo_dependantSet
                                     where awd.Id == _dependentId
                                     select new
                                     {
                                         awd.udo_FileNumber,
                                         awd.udo_PtcpntID,
                                         awd.udo_VeteranId,
                                         awd.OwnerId
                                     };
                        foreach (var awd in getDep)
                        {
                            gotData = true;
                            if (awd.OwnerId != null)
                            {
                                _ownerType = awd.OwnerId.LogicalName;
                                _ownerId = awd.OwnerId.Id;
                            }
                            else
                            {
                                _responseMessage = "Invalid ownership of Dependent record. Please contact your application support team with this message.";
                                return false;
                            }
                            if (awd.udo_PtcpntID != null)
                            {
                                _ptcpntId = Int64.Parse(awd.udo_PtcpntID);
                            }

                            if (awd.udo_FileNumber != null)
                            {
                                _fileNumber = awd.udo_FileNumber;
                            }

                            if (awd.udo_VeteranId != null)
                            {
                                _veteranId = awd.udo_VeteranId.Id;
                            }
                            else
                            {
                                _responseMessage = "No Veteran ID found. Cannot get Address data.";
                                return false;
                            }
                        }
                    }
                }
                Logger.setMethod = "Execute";

                if (String.IsNullOrEmpty(_fileNumber) && _ptcpntId == null)
                {
                    _responseMessage = "No FileNumber or PID found. Cannot get Address data.";
                    return false;
                }

                return gotData;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
    }
}
