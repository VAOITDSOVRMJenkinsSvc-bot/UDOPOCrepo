using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using VRMRest;
using UDO.Model;
using UDO.LOB.Core;
using UDO.LOB.Contact.Messages;

namespace CustomActions.Plugins.Entities.Dependents
{
    internal class UDOGetDependentRunner : UDOActionRunner
    {
        protected Guid _ownerId = new Guid();
        protected string _ownerType;
        protected string _filenumber = "";
        protected string _govId = "";
        protected Guid _veteranId = new Guid();
        protected Guid _dependentId = new Guid();
        protected bool _veteranFocused = false;
        protected const string _veteranSerachField = "udo_veteranid";
        protected const string _dependentSearchField = "udo_relateddependentid";
        
        public UDOGetDependentRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_contactlogtimer";
            _logSoapField = "udo_contactlogsoap";
            _debugField = "udo_dependent";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_contacttimeout";
            _validEntities = new string[] { "udo_dependant", "contact" };
            PromptForRetry = true;
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


            if (!didWeNeedData())
            {
                DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                return;
            }

            GetSettingValues();

            //DeleteDependents();

            UDOHeaderInfo _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            var request = new UDOcreateDependentsRequest();
            request.VeteranId = _veteranId;

            var veteranReference = new UDOcreateDependentsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };
            var references = new[] { veteranReference };
            if (!_veteranFocused)
            {
                var dependentReference = new UDOcreateDependentsRelatedEntitiesMultipleRequest()
                {
                    RelatedEntityFieldName = "udo_relateddependentid",
                    RelatedEntityId = _dependentId,
                    RelatedEntityName = "udo_dependant"
                };
                references = new[] { veteranReference, dependentReference };
                request.DependentId = _dependentId;
            }

            request.UDOcreateDependentsRelatedEntitiesInfo = references;
            request.LegacyServiceHeaderInfo = _headerInfo;
            request.MessageId = PluginExecutionContext.CorrelationId.ToString();
            request.Debug = _debug;
            request.LogSoap = _logSoap;
            request.LogTiming = _logTimer;
            request.RelatedParentEntityName = "contact";
            request.RelatedParentFieldName = "udo_contactid";
            request.RelatedParentId = _veteranId;
            request.fileNumber = _filenumber;
            request.ownerId = _ownerId;
            request.ownerType = _ownerType;
            request.UserId = PluginExecutionContext.InitiatingUserId;
            request.OrganizationName = PluginExecutionContext.OrganizationName;

            if (_veteranFocused)
            {
                request.fileNumber = _filenumber;
            }
            else
            {
                request.fileNumber = _govId;
            }
            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = "Contact.Plugins.DependentsRetrieveMultipleRunner"
            };


            tracer.Trace("calling UDOcreateDependentsRequest");
            var response = Utility.SendReceive<UDOcreateDependentsResponse>(_uri, "UDOcreateDependentsRequest", request, _logSettings,_timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOcreateDependentsRequest");

            if (response.ExceptionOccured)
            {
                ExceptionOccurred = true;
                _responseMessage = "An error occurred while executing the Retrieve Dependents LOB.";

                if (!string.IsNullOrEmpty(response.ExceptionMessage))
                {
                    _responseMessage = response.ExceptionMessage;
                }

                Logger.WriteToFile(string.Format("Error message - {0}", _responseMessage));
                tracer.Trace(string.Format("Error message - {0}", _responseMessage));
            }
        }

        private void DeleteDependents()
        {
            tracer.Trace("DeleteDependants started");
            Logger.setMethod = "DeleteDependants";

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

            var fetchDependants = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                      <entity name='udo_dependant'>
                                                        <attribute name='udo_dependantid' />
                                                        <order attribute='udo_name' descending='false' />
                                                        <filter type='and'>
                                                          <condition attribute='{0}' operator='eq' value='{1}' />
                                                        </filter>
                                                      </entity>
                                                    </fetch>", searchField, searchGuid);

            var response = ElevatedOrganizationService.RetrieveMultiple(new FetchExpression(fetchDependants));

            if (response.Entities.Count > 0)
            {
                tracer.Trace(string.Format("{0} to delete.", response.Entities.Count));

                foreach (var dependent in response.Entities)
                {
                    ElevatedOrganizationService.Delete(udo_dependant.EntityLogicalName, dependent.Id);
                    //OrganizationService.Delete(udo_dependant.EntityLogicalName, dependent.Id);
                }
            }
        }

        private bool didWeNeedData()
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
                            if (awd.udo_FileNumber != null)
                            {
                                _filenumber = awd.udo_FileNumber;
                            }
                            else
                            {
                                _responseMessage = "No FileNumber was found on this Contact; cannot retrieve Dependents.";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        var getDep = from awd in xrm.udo_dependantSet
                                     where awd.Id == _dependentId
                                     select new
                                     {
                                         awd.udo_FileNumber,
                                         awd.udo_VeteranId,
                                         awd.udo_SSN,
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
                            if (awd.udo_FileNumber != null)
                            {
                                _filenumber = awd.udo_FileNumber;
                            }
                            else
                            {
                                if (awd.udo_SSN != null)
                                {
                                    _govId = awd.udo_SSN;
                                }
                                else
                                {
                                    _responseMessage = "No fileNumber or SSN found on this Dependent; cannot retrieve related dependents";
                                    Logger.WriteToFile("no fileNumber or SSN found on this Dependent; cannot retrieve related dependents");
                                    tracer.Trace("no fileNumber or SSN found on this Dependent; cannot retrieve related dependents");
                                    return false;
                                }
                            }
                            if (awd.udo_VeteranId != null)
                            {
                                _veteranId = awd.udo_VeteranId.Id;
                            }
                        }
                    }
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
