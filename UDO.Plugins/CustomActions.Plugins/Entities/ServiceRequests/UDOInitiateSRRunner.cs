using System;
using System.Collections.Generic;
using System.Linq;
using MCSHelperClass;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.ServiceRequests.Messages;
using VRMRest;
using UDO.Model;
using UDO.LOB.Core;
using UDO.LOB.ServiceRequest.Messages;

namespace CustomActions.Plugins.Entities.ServiceRequests
{
    public class UDOInitiateSRRunner : UDOActionRunner
    {

        #region Members
        protected UDOHeaderInfo _headerInfo;
        protected EntityReference _idproof;
        protected EntityReference _interaction;
        protected EntityReference _vet;
        protected EntityReference _owner;
        protected bool _noPayeeDetails;
        protected string _requesttype;
        protected string _requestsubtype;
        #endregion

        private const string FAILURE_MESSAGE = "{0} Unable to initiate a service request. CorrelationId: {1}";

        public UDOInitiateSRRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_letterslogtimer";
            _logSoapField = "udo_letterslogsoap";
            _debugField = "udo_letters";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_leterstimeout";
            _validEntities = new string[] { "udo_person" };
        }

        public override void DoAction()
        {
            _method = "DoAction";

            GetSettingValues();
            _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            if (DataIssue = !DidWeFindData()) return;

            //if (PluginExecutionContext.InputParameters.Contains("Interaction"))
            //{
            //    _interaction = PluginExecutionContext.InputParameters["Interaction"] as EntityReference;
            //}

            //if (PluginExecutionContext.InputParameters.Contains("NoPayeeDetails"))
            //{
            //    _noPayeeDetails = (bool)PluginExecutionContext.InputParameters["NoPayeeDetails"];
            //}

            if (PluginExecutionContext.InputParameters.Contains("RequestType"))
            {
                _requesttype = (string)PluginExecutionContext.InputParameters["RequestType"];
            }

            if (PluginExecutionContext.InputParameters.Contains("RequestSubType"))
            {
                _requestsubtype = (string)PluginExecutionContext.InputParameters["RequestSubType"];
            }

            //if ((_owner == null || _owner.Id == Guid.Empty) && _vet != null)
            //{
            //    var vet = OrganizationService.Retrieve("contact", _vet.Id, new ColumnSet("ownerid"));
            //    _owner = vet.GetAttributeValue<EntityReference>("ownerid");
            //}

            #region Build Request

            var payeeId = Parent.Id.ToString();
            if (_noPayeeDetails)
            {
                payeeId = string.Empty;
            }

            var request = new UDOInitiateSRRequest
            {
                MessageId = PluginExecutionContext.CorrelationId.ToString(),
                OrganizationName = PluginExecutionContext.OrganizationName,
                UserId = PluginExecutionContext.InitiatingUserId,
                Debug = _debug,
                LogSoap = _logSoap,
                LogTiming = _logTimer,
                LegacyServiceHeaderInfo = _headerInfo,
                udo_PersonId = payeeId,
                OwnerId = _owner.Id,
                OwnerType = _owner.LogicalName,
                udo_RequestType = _requesttype,
                udo_RequestSubType = _requestsubtype
            };

            if (_vet != null) request.udo_VeteranId = _vet.Id.ToString();
            if (_interaction != null) request.udo_InteractionId = _interaction.Id.ToString();
            if (_idproof != null) request.udo_IDProofId = _idproof.Id.ToString();
            #endregion

            #region Setup Log Settings
            LogSettings logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };
            #endregion

            tracer.Trace("calling UDOInitiateSRRequest");
            tracer.Trace("Request Type: " + _requesttype + " Request Subtype: " + _requestsubtype);
            var response = Utility.SendReceive<UDOInitiateSRResponse>(_uri, "UDOInitiateSRRequest", request, logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOInitiateSRRequest");

            if (response.ExceptionOccurred)
            {
                _responseMessage = String.Format(FAILURE_MESSAGE, response.ExceptionMessage, PluginExecutionContext.CorrelationId.ToString());
                ExceptionOccurred = true;
                return;
            }

            if (!String.IsNullOrEmpty(response.UDOServiceRequestId))
            {
                PluginExecutionContext.OutputParameters["result"] = new EntityReference("udo_servicerequest",
                    Guid.Parse(response.UDOServiceRequestId));
            }
        }

        private const string FETCH_PERSON =
                    "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' count='1'>" +
                    "<entity name='udo_person'>" +
                    "<attribute name='ownerid'/>" +
                    "<attribute name='udo_idproofid'/>" +
                    "<attribute name='udo_veteranid'/>" +
                    "<filter type='and'>" +
                    "<condition attribute='udo_personid' operator='eq' value='{0}' />" +
                    "</filter>" +
                    "<link-entity name='contact' from='contactid' to='udo_veteranid' visible='false' link-type='outer' alias='{1}'>" +
                    "<attribute name='udo_gender' />" +
                    "</link-entity>" +
                    "</entity></fetch>";

        internal bool DidWeFindData()
        {
            try
            {
                Logger.setMethod = "DidWeFindData";
                ////Logger.WriteDebugMessage("Starting didWeNeedData Method");

                var aliasGuid = String.Format("a_{0:N}", Guid.NewGuid());
                var fetchxml = String.Format(FETCH_PERSON, Parent.Id, aliasGuid);

                var response = OrganizationService.RetrieveMultiple(new FetchExpression(fetchxml));

                if (response.Entities.Count > 0)
                {

                    var entity = response.Entities.FirstOrDefault();
                    if (entity == null) return false;

                    _vet = entity.GetAttributeValue<EntityReference>("udo_veteranid");
                    _owner = entity.GetAttributeValue<EntityReference>("ownerid");
                    _idproof = entity.GetAttributeValue<EntityReference>("udo_idproofid");

                    if (_vet == null)
                    {
                        _responseMessage = String.Format(FAILURE_MESSAGE, "Veteran ID not found.", PluginExecutionContext.CorrelationId.ToString());
                        return false;
                    }
                }
                tracer.Trace("DidWeFindData: Fields have been retrieved and set.");
                Logger.setMethod = "Execute";

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to execute didWeFindData due to: {0}".Replace("{0}", ex.Message));
            }
        }

    }
}
