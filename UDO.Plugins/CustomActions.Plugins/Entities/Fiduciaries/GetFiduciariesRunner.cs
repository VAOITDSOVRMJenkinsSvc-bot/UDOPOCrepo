using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
//using Microsoft.Xrm.Sdk.Query;
//using VRM.Integration.UDO.Contact.Messages;
//using VRM.Integration.UDO.Common.Messages;
using UDO.LOB.Core;
using UDO.LOB.Contact.Messages;
using VRMRest;
using System.ServiceModel;
using System.Diagnostics;
using UDO.Model;
using Microsoft.Xrm.Sdk.Query;

namespace CustomActions.Plugins.Entities.Fiduciaries
{
    internal class GetFiduciariesRunner : UDOActionRunner
    {

        protected string _filenumber = "";
        protected Guid _veteranId = new Guid();
        protected Guid _dependentId = new Guid();
        protected bool _veteranFocused = false;
        protected Guid _ownerId = new Guid();
        protected string _ownerType;
        protected const string _veteranSerachField = "udo_veteranid";
        protected const string _dependentSearchField = "udo_dependentid";

        public GetFiduciariesRunner(IServiceProvider serviceProvider)
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

            DeleteRecords("udo_pastpoa");
            DeleteRecords("udo_pastfiduciary");

            var request = new UDOcreatePastFiduciariesRequest();
            request.VeteranId = _veteranId;

            var veteranReference = new UDOcreatePastFiduciariesRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var references = new[] { veteranReference };
            if (!_veteranFocused)
            {
                var dependentReference = new UDOcreatePastFiduciariesRelatedEntitiesMultipleRequest()
                {
                    RelatedEntityFieldName = "udo_dependentid",
                    RelatedEntityId = _dependentId,
                    RelatedEntityName = "udo_dependant"
                };
                references = new[] { veteranReference, dependentReference };
                request.DependentId = _dependentId;
            }

            UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            request.UDOcreatePastFiduciariesRelatedEntitiesInfo = references;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.RelatedParentEntityName = "contact";
            request.RelatedParentFieldName = "udo_contactid";
            request.RelatedParentId = _veteranId;
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.fileNumber = _filenumber;
            request.OrganizationName = PluginExecutionContext.OrganizationName;
            //      //Logger.WriteDebugMessage("Request Created");
            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = "Contact.Plugins.FidsRetrieveMultipleRunner"
            };

            var response = Utility.SendReceive<UDOcreatePastFiduciariesResponse>(_uri, "UDOcreatePastFiduciariesRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
        }

        private void DeleteRecords(string logicalName)
        {
            tracer.Trace("DeleteRecords started");
            Logger.setMethod = "DeleteRecords";

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

            var pastPoaCondition = (logicalName == "udo_pastpoa") ? "<condition attribute='udo_currentpoasrecord' operator='ne' value='true' />" : string.Empty;

            var fetchAddresses = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                      <entity name='{0}'>
                                                        <attribute name='{0}id' />
                                                        <order attribute='udo_name' descending='false' />
                                                        <filter type='and'>
                                                          <condition attribute='{1}' operator='eq' value='{2}' />
                                                          {3}
                                                        </filter>
                                                      </entity>
                                                    </fetch>", logicalName, searchField, searchGuid, pastPoaCondition);

            var response = ElevatedOrganizationService.RetrieveMultiple(new FetchExpression(fetchAddresses));

            if (response.Entities.Count > 0)
            {
                tracer.Trace(string.Format("{0} to delete.", response.Entities.Count));

                foreach (var entity in response.Entities)
                {
                    ElevatedOrganizationService.Delete(logicalName, entity.Id);
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
                                            awd.OwnerId

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
                                _responseMessage = "Invalid ownership of Contact record. Please contact your application support team with this message.";
                                return false;
                            }
                            if (awd.udo_FileNumber != null)
                            {
                                _filenumber = awd.udo_FileNumber;
                            }
                            else
                            {
                                _responseMessage = "No FileNumber was found on this Contact; cannot retrieve Fiduciaries.";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        ////Logger.WriteDebugMessage("getting depend Method");
                        var getDep = from awd in xrm.udo_dependantSet
                                     where awd.Id == _dependentId
                                     select new
                                     {
                                         awd.udo_SSN,
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
                            if (awd.udo_SSN != null)
                            {
                                _filenumber = awd.udo_SSN;
                            }
                            else
                            {
                                _responseMessage = "No SSN found on this Dependent; cannot retrieve related fiduciaries";

                                Logger.WriteToFile("No SSN found on this Dependent; cannot retrieve related fiduciaries");
                                tracer.Trace("No SSN found on this Dependent; cannot retrieve related fiduciaries");
                                return false;
                            }
                            if (awd.udo_VeteranId != null)
                            {
                                _veteranId = awd.udo_VeteranId.Id;
                            }
                            else
                            {
                                _responseMessage = "No Veteran found. Cannot get Fiduciaries.";
                                return false;
                            }

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