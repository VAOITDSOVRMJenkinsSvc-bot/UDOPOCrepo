using System;
using System.Collections.Generic;
using System.Linq;
using MCSHelperClass;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using VRMRest;
using UDO.LOB.Core;
using UDO.LOB.Notes.Messages;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.Notes.Messages;

namespace CustomActions.Plugins.Entities.Notes
{
    public class UDOInitiateNotesRunner : UDOActionRunner
    {

        #region Members
        protected UDOHeaderInfo _headerInfo;
        protected string _pid = string.Empty;

        //protected EntityReference _idproof;
        protected EntityReference _vet;
        protected EntityReference _owner;
        #endregion

        public UDOInitiateNotesRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_noteslogtimer";
            _logSoapField = "udo_noteslogsoap";
            _debugField = "udo_notes";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_notestimeout";
            _validEntities = new string[] { "udo_person" };

            // you could add support to just get the notes for a claim... but since it sends back all the notes for a person
            // I'm not sure it would be worth it.
        }

        public override void DoAction()
        {
           try
            {
                _method = "DoAction";

                GetSettingValues();
                _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

                if (DataIssue = !DidWeFindData()) return;

                #region Build Request

                var request = new UDORetrieveNotesRequest()
                {
                    MessageId = PluginExecutionContext.CorrelationId.ToString(),
                    OrganizationName = PluginExecutionContext.OrganizationName,
                    UserId = PluginExecutionContext.InitiatingUserId,
                    Debug = _debug,
                    LogSoap = _logSoap,
                    LogTiming = _logTimer,
                    LegacyServiceHeaderInfo = _headerInfo,
                    LoadSize = 100,
                    RelatedParentEntityName = "udo_person",
                    RelatedParentFieldName = "udo_personid",
                    RelatedParentId = Parent.Id,
                    OwnerId = _owner.Id,
                    OwnerType = _owner.LogicalName,
                    //udo_personId = NotesPerson.Value
                };


                tracer.Trace("LegacyServiceHeaderInfo: " + request.LegacyServiceHeaderInfo == null ? "null" : $"{request.LegacyServiceHeaderInfo.StationNumber}");
                Trace("LegacyServiceHeaderInfo: " + request.LegacyServiceHeaderInfo == null ? "null" : $"{request.LegacyServiceHeaderInfo.StationNumber}");

                Int64 pid;
                if (Int64.TryParse(_pid, out pid))
                {
                    request.ptcpntId = pid;
                }

                var veteranReference = new UDORelatedEntity()
                {
                    RelatedEntityFieldName = "udo_veteranid",
                    RelatedEntityId = _vet.Id,
                    RelatedEntityName = "contact"
                };
                var udo_personReference = new UDORelatedEntity()
                {
                    RelatedEntityFieldName = "udo_personid",
                    RelatedEntityId = Parent.Id,
                    RelatedEntityName = "udo_person"
                };
                //var udo_idproofReference = new UDORetrieveNotesRelatedEntitiesMultipleRequest()
                //{
                //    RelatedEntityFieldName = "udo_idproofid",
                //    RelatedEntityId = idproof,
                //    RelatedEntityName = "udo_idproof"
                //};
                var references = new[] { veteranReference, udo_personReference }; //, udo_idproofReference };
                request.RelatedEntities = references;

                #endregion

                #region Setup Log Settings

                LogSettings logSettings = new LogSettings()
                {
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL",
                    UserId = PluginExecutionContext.InitiatingUserId,
                    callingMethod = Logger.setModule
                };
                Logger.setDebug = request.Debug;

                #endregion

                tracer.Trace("calling UDORetrieveNotesRequest");
                Trace("calling UDORetrieveNotesRequest");
                try
                {
                    var response = Utility.SendReceive<UDORetrieveNotesResponse>(_uri, "UDORetrieveNotesRequest", request,
                    logSettings, 100, _crmAuthTokenConfig, tracer);
                }
                catch (TimeoutException ex)
                {
                    PluginError = true;
                    //Ignore TimeoutException
                }
                tracer.Trace("Returned from UDORetrieveNotesRequest");
                Trace("Returned from UDORetrieveNotesRequest");

                //if (response.ExceptionOccurred)
                //{
                //    ExceptionOccurred = true;
                //    _responseMessage = "An error occurred while retrieving Notes from source system.";

                //    if (!string.IsNullOrEmpty(response.ExceptionMessage))
                //    {
                //        _responseMessage = response.ExceptionMessage;
                //    }

                //    Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                //    tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId));
                //}
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

        private const string FETCH_PERSON =
                    "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' count='1'>" +
                    "<entity name='udo_person'>" +
                    "<attribute name='ownerid'/>" +
                    "<attribute name='udo_ptcpntid'/>" +
                    "<attribute name='udo_idproofid'/>" +
                    "<attribute name='udo_veteranid'/>" +
                    "<filter type='and'>" +

                    "<condition attribute='udo_personid' operator='eq' value='{0}' />" +
                    "</filter>" +
                    "</entity></fetch>";

        internal bool DidWeFindData()
        {
            try
            {
                Logger.setMethod = "DidWeFindData";
                ////Logger.WriteDebugMessage("Starting didWeNeedData Method");

                var fetchxml = String.Format(FETCH_PERSON, Parent.Id);

                var response = OrganizationService.RetrieveMultiple(new FetchExpression(fetchxml));

                if (response.Entities.Count > 0)
                {

                    var entity = response.Entities.FirstOrDefault();
                    if (entity == null) return false;

                    _vet = entity.GetAttributeValue<EntityReference>("udo_veteranid");
                    _owner = entity.GetAttributeValue<EntityReference>("ownerid");
                    //_idproof = entity.GetAttributeValue<EntityReference>("udo_idproofid");
                    _pid = entity.GetAttributeValue<string>("udo_ptcpntid");
                }

                tracer.Trace("DidWeFindData: Fields have been retrieved and set.");
                Trace("DidWeFindData: Fields have been retrieved and set.");

                Logger.setMethod = "Execute";

                return true;
            }
            catch (Exception ex)
            {
                PluginError = true;
                throw new InvalidPluginExecutionException("Unable to execute didWeFindData due to: {0}".Replace("{0}", ex.Message));
            }
        }


    }
}
