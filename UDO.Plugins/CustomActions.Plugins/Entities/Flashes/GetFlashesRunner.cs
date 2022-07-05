using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using UDO.LOB.Core;
using UDO.LOB.Flashes.Messages;
using UDO.Model;
using VRMRest;

namespace CustomActions.Plugins.Entities.Flashes
{
    public class GetFlashesRunner : UDOActionRunner
    {
        #region members
        protected Guid? _dependantId;
        protected Guid? _veteranId;
        protected UDOHeaderInfo _headerInfo;
        protected Guid _ownerId = new Guid();
        protected string _ownerType = string.Empty;
        protected string _vetPID = string.Empty;
        protected string _depPID = string.Empty;
        protected string _fileNumber = string.Empty;
        protected string _depFileNumber = string.Empty;
        protected LogSettings _logSettings;
        #endregion

        public GetFlashesRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_flashlogtimer";
            _logSoapField = "udo_flashlogsoap";
            _debugField = "udo_flash";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_flashvimttimeout";
            _validEntities = new string[] { "udo_veteransnapshot", "contact", "udo_dependant" };
            PromptForRetry = true;
        }

        public override void DoAction()
        {
            try
            {
                _method = "ACtion Lookup";
                Logger.setMethod = _method;
                _method = "DoAction";

                if (!DidWeNeedData())
                {
                    DataIssue = (!String.IsNullOrEmpty(_responseMessage) && !Complete);
                    return;
                }

                Logger.setMethod = "GetSettingValues";
                GetSettingValues();

                Logger.setMethod = "DoAction";
                _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

                _logSettings = new LogSettings()
                {
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL",
                    UserId = PluginExecutionContext.InitiatingUserId,
                    callingMethod = Logger.setModule
                };

                var response = new UDOgetFlashesResponse();

                SendRequest(BuildCreateFlashRequest());

                if (response.ExceptionOccurred)
                {
                    ExceptionOccurred = true;
                    string recordType = string.Empty;
                    Guid recordId = Guid.Empty;
                    if (_dependantId.HasValue)
                    {
                        recordId = _dependantId.Value;
                        recordType = "dependent";
                    }
                    else if (_veteranId.HasValue)
                    {
                        recordId = _veteranId.Value;
                        recordType = "veteran (contact)";
                    }

                    _responseMessage = String.Format("An error occurred while retrieving the flashes for {0} ({1}).", recordType, recordId);

                    if (!string.IsNullOrEmpty(response.ExceptionMessage))
                    {
                        _responseMessage = response.ExceptionMessage;
                    }

                    Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                    tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                    Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                }
            }
            finally
            {
                Trace("Entered Finally");
                SetupLogger();
                Trace("Set up logger done.");
                ExecuteFinally();
                Trace("Exit Finally");
            }
        }

        private UDOgetFlashesResponse SendRequest(UDOgetFlashesRequest request)
        {
            _method = "SendRequest";
            Logger.setMethod = "SendRequest";
            //request.UDOcreateFlashesRelatedEntitiesInfo = references;
            tracer.Trace("calling UDOgetFlashesRequest");
            Trace("calling UDOgetFlashesRequest");
            var response = Utility.SendReceive<UDOgetFlashesResponse>(_uri, "UDOgetFlashesRequest", request, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOgetFlashesRequest");
            Trace("Returned from UDOgetFlashesRequest");

            return ProcessResponse(response);
        }

        private UDOgetFlashesResponse ProcessResponse(UDOgetFlashesResponse response)
        {
            Logger.setMethod = "ProcessResponse";
            tracer.Trace("ProcessResponse");
            Trace("ProcessResponse");
            // No response or empty...
            if (response == null
                || response.ExceptionOccurred
                || response.flashes == null
                || response.flashes.Length == 0) return response;

            // Post Process Results..?  Maybe eventually not create records, but instead just pass them back.
            EntityCollection ec = new EntityCollection(response.flashes.Select((flash) => ConvertFlashItemToEntity(flash)).ToList());

            if (PluginExecutionContext.InputParameters.ContainsKey("Save"))
            {
                if ((bool)PluginExecutionContext.InputParameters["Save"])
                {
                    ClearAndSaveFlashes(ec);
                }
            }

            PluginExecutionContext.OutputParameters["Flashes"] = ec;

            return response;
        }

        private void ClearAndSaveFlashes(EntityCollection ec)
        {
            tracer.Trace("ClearAndSaveFlashes");
            Trace("ClearAndSaveFlashes");
            var requests = new OrganizationRequestCollection();

            if (Parent.LogicalName.Equals("udo_veteransnapshot", StringComparison.InvariantCultureIgnoreCase))
            {
                var SnapShot = new Entity(Parent.LogicalName);
                SnapShot.Id = Parent.Id;
                SnapShot["udo_flashescomplete"] = true;
                requests.Add(new UpdateRequest() { Target = SnapShot });
            }

            DeleteExistingFlashes();
            CreateFlashesInCrm(ec.Entities.ToList());
        }

        private void CreateFlashesInCrm(List<Entity> list)
        {
            tracer.Trace("BuildCreateRequests");
            Trace("BuildCreateRequests");
            if (list == null || list.Count == 0) return;

            foreach (var e in list)
            {
                e.Id = OrganizationService.Create(e);
            }
            //return list.Select((e) => new CreateRequest { Target = e });
        }

        private void DeleteExistingFlashes()
        {
            tracer.Trace("BuildDeleteRequests");
            Trace("BuildDeleteRequests");
            var query = new QueryByAttribute("udo_flash");
            query.ColumnSet = new ColumnSet("udo_flashid");

            if (_dependantId.HasValue)
            {
                query.AddAttributeValue("udo_dependentid", _dependantId.Value);
            }
            else if (_veteranId.HasValue)
            {
                query.AddAttributeValue("udo_veteranid", _veteranId.Value);
            }
            else
            {
                // Not enough information to delete anything
                return;
            }

            EntityCollection retrieved = OrganizationService.RetrieveMultiple(query);

            if (retrieved == null || retrieved.Entities == null || retrieved.Entities.Count == 0)
            {
                return;
            }

            foreach (var e in retrieved.Entities)
            {
                ElevatedOrganizationService.Delete(e.LogicalName, e.Id);
            }
        }

        private Entity ConvertFlashItemToEntity(UDOFlashItem flash)
        {
            tracer.Trace("ConvertFlashItemToEntity");
            Trace("ConvertFlashItemToEntity");
            var entity = new Entity("udo_flash");
            var id = Guid.NewGuid();
            entity["udo_flashid"] = id;
            entity.Id = id;
            entity["ownerid"] = new EntityReference(_ownerType, _ownerId);
            entity["udo_name"] = flash.FlashText.Trim();
            entity["udo_veteranid"] = new EntityReference("contact", _veteranId.Value);
            if (_dependantId.HasValue)
            {
                entity["udo_dependentid"] = new EntityReference("udo_dependant", _dependantId.Value);
            }

            return entity;
        }

        private UDOgetFlashesRequest BuildCreateFlashRequest()
        {
            tracer.Trace("BuildCreateFlashRequest");
            Trace("BuildCreateFlashRequest");
            var UDOcreateFlashesRequest = new UDOgetFlashesRequest()
            {
                LegacyServiceHeaderInfo = _headerInfo,
                MessageId = PluginExecutionContext.CorrelationId.ToString(),
                Debug = _debug,
                LogSoap = _logSoap,
                LogTiming = _logTimer,
                UserId = PluginExecutionContext.InitiatingUserId,
                OrganizationName = PluginExecutionContext.OrganizationName,
                ptcpntBeneId = _depPID,
                ptpcntRecipId = _depPID,
                ptcpntVetId = _vetPID,
                fileNumber = _fileNumber,
                depFileNumber = _depFileNumber,
            };
            return UDOcreateFlashesRequest;
        }

        private bool DidWeNeedData()
        {
            tracer.Trace("DidWeNeedData");
            Trace("DidWeNeedData");
            var fetch = string.Empty;
            try
            {
                tracer.Trace("DidWeNeedData started");
                Trace("DidWeNeedData started");
                Logger.setMethod = "DidWeNeedData";

                fetch = "<fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='true' count='1'><entity name='contact'>" +
                            "<attribute name='contactid'/>" +
                            "<attribute name='udo_participantid'/>" +
                            "<attribute name='udo_filenumber'/>" +
                            "<attribute name='ownerid'/>";
                switch (Parent.LogicalName)
                {

                    case "udo_veteransnapshot":

                        //Get Contact ID from the snapshot
                        var ssResults = OrganizationService.Retrieve(udo_veteransnapshot.EntityLogicalName, Parent.Id, new ColumnSet("udo_veteranid"));

                        if (ssResults == null)
                            return false;

                        var snapShot = ssResults.ToEntity<udo_veteransnapshot>();
                        //Set Filter to user contactid retrieved
                        fetch += String.Format("<filter><condition attribute='contactid' operator='eq' value='{0}'/></filter>", snapShot.udo_VeteranID.Id);
                        //fetch += "<link-entity name='udo_veteransnapshot' from='udo_veteranid' to='contactid' alias='snapshot'>"
                        //+ "<filter type='and'><condition attribute='udo_veteransnapshotid' operator='eq' value='{0}'/>"
                        //+ "</filter></link-entity>";
                        break;
                    case "contact":
                        fetch += String.Format("<filter><condition attribute='contactid' operator='eq' value='{0}'/></filter>", Parent.Id.ToString());
                        break;
                    case "udo_dependant":
                        fetch += String.Format("<link-entity name='udo_dependant' from='udo_veteranid' to='contactid' alias='dep'>"
                            + "<attribute name='udo_ptcpntid'/>"
                            + "<attribute name='udo_filenumber'/>"
                            + "<filter type='and'><condition attribute='udo_dependantid' operator='eq' value='{0}'/>"
                            + "</filter></link-entity>", Parent.Id.ToString());
                        _dependantId = Parent.Id;
                        break;
                }

                fetch += "</entity></fetch>";

                if (Parent.LogicalName != udo_veteransnapshot.EntityLogicalName)
                    fetch = String.Format(fetch, Parent.Id);

                var result = OrganizationService.RetrieveMultiple(new FetchExpression(fetch));

                if (result != null && result.Entities != null && result.Entities.Count > 0)
                {
                    var vet = result.Entities[0];
                    ////Logger.WriteDebugMessage(vet, Parent.LogicalName);

                    _veteranId = vet.GetAttributeValue<Guid>("contactid");

                    _vetPID = vet.GetAttributeValue<string>("udo_participantid");

                    _fileNumber = vet.GetAttributeValue<string>("udo_filenumber");

                    if (Parent.LogicalName == "udo_dependant")
                    {
                        var aliased_depPID = vet.GetAttributeValue<AliasedValue>("dep.udo_ptcpntid");
                        if (aliased_depPID != null)
                        {
                            _depPID = aliased_depPID.Value.ToString();
                        }
                    }

                    var owner = vet.GetAttributeValue<EntityReference>("ownerid");
                    _ownerId = owner.Id;
                    _ownerType = owner.LogicalName;

                    tracer.Trace("didWeNeedData have been retrieved and set");
                    Trace("didWeNeedData have been retrieved and set");
                    Logger.setMethod = _method;
                    return true;
                }

                _responseMessage = "No results returned in Fetch";
                return false;
            }
            catch (Exception ex)
            {
                PluginError = true;
                var err = ex.Message;

                try
                {
                    if (string.IsNullOrEmpty(ex.StackTrace))
                    {
                        err += Environment.NewLine + "Stack Trace " + ex.StackTrace;
                    }
                }
                catch (Exception e)
                {
                    PluginError = true;
                }

                //Logger.WriteException(ex);
                tracer.Trace(ex.ToString() + "STACKTRACE: " + ex.StackTrace);
                Trace(ex.ToString() + "STACKTRACE: " + ex.StackTrace);
                ////Logger.WriteDebugMessage("Unable to didWeNeedData due to: {0}\r\n\r\nFetchXml: \r\n{1}", err, fetch);

                _responseMessage = ex.Message;
                return false;
                //throw new InvalidPluginExecutionException(String.Format("Unable to didWeNeedData due to: {0}\r\n\r\nFetchXml: \r\n{1}", err, fetch));
            }
        }
    }
}
