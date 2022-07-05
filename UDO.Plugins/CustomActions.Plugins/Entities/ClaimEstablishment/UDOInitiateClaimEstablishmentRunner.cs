using System;
using System.Linq;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MCSUtilities2011;
using UDO.LOB.Core;
//using VRM.Integration.UDO.ClaimEstablishment.Messages;
//using VRM.Integration.UDO.Common.Messages;
using VRMRest;
using UDO.LOB.ClaimEstablishment.Messages;

namespace CustomActions.Plugins.Entities.ClaimEstablishment
{
    public class UDOInitiateClaimEstablishmentRunner : UDOActionRunner
    {

        #region Members

        protected UDOHeaderInfo _headerInfo;
        //protected bool _bypassMvi;
        protected string _payeeCode = string.Empty;
        protected string _awardTypeCode = string.Empty;
        protected string _pid = string.Empty;
        protected string _vet_pid = string.Empty;
        protected string _vet_filenumber = string.Empty;
        protected string _first = string.Empty;
        protected string _last = string.Empty;
        protected string _addressline1 = string.Empty;
        protected string _addressline2 = string.Empty;
        protected string _addressline3 = string.Empty;
        protected string _city = string.Empty;
        protected string _state = string.Empty;
        protected string _filenumber = string.Empty;
        protected string _postalcode = string.Empty;
        private Entity DWNDEntity = null;

        protected string _aliasGuid;
        protected string _fetchxml;

        protected EntityReference _idproof;
        protected EntityReference _snapshot;
        protected EntityReference _vet;
        protected EntityReference _owner;
        protected EntityReference _payeecodeid;
        protected EntityReference _interaction;
        #endregion

        private const string FAILURE_MESSAGE = "{0} Unable to initialize a Claim Establishment. CorrelationId: {1}";

        public UDOInitiateClaimEstablishmentRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_claimestablishmentlogtimer";
            _logSoapField = "udo_claimestablishmentlogsoap";
            _debugField = "udo_claimestablishment";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_claimestablishmentvimttimeout";
            _validEntities = new string[] { "udo_person" };
        }


        public override void DoAction()
        {
            _method = "DoAction";

            GetSettingValues();

            // Retrieve all the data we need
            if (DataIssue = !DidWeFindData()) return;

            _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            _vet = DWNDEntity.GetAttributeValue<EntityReference>("udo_veteranid");
            var aliasFilenumber = DWNDEntity.GetAttributeValue<AliasedValue>("contact1.udo_filenumber");
            if (aliasFilenumber != null && aliasFilenumber.Value != null)
            {
                _vet_filenumber = aliasFilenumber.Value.ToString();
            }

            var aliasVetPid = DWNDEntity.GetAttributeValue<AliasedValue>("contact1.udo_participantid");
            if (aliasVetPid != null && aliasVetPid.Value != null)
            {
                _vet_pid = aliasVetPid.Value.ToString();
            }
            _filenumber = DWNDEntity.GetAttributeValue<string>("udo_filenumber");
            _owner = DWNDEntity.GetAttributeValue<EntityReference>("ownerid");
            _payeeCode = DWNDEntity.GetAttributeValue<string>("udo_payeecode");
            _payeecodeid = DWNDEntity.GetAttributeValue<EntityReference>("udo_payeecodeid");
            _awardTypeCode = DWNDEntity.GetAttributeValue<string>("udo_awardtypecode");
            _idproof = DWNDEntity.GetAttributeValue<EntityReference>("udo_idproofid");
            _pid = DWNDEntity.GetAttributeValue<string>("udo_ptcpntid");
            _first = DWNDEntity.GetAttributeValue<string>("udo_first");
            _last = DWNDEntity.GetAttributeValue<string>("udo_last");
            _snapshot = DWNDEntity.GetAttributeValue<EntityReference>("udo_veteransnapshotid");
            _addressline1 = DWNDEntity.GetAttributeValue<string>("udo_address1");
            _addressline2 = DWNDEntity.GetAttributeValue<string>("udo_address2");
            _addressline3 = DWNDEntity.GetAttributeValue<string>("udo_address3");
            _city = DWNDEntity.GetAttributeValue<string>("udo_city");
            _state = DWNDEntity.GetAttributeValue<string>("udo_state");
            _postalcode = DWNDEntity.GetAttributeValue<string>("udo_zip");


            var aliasInteraction = DWNDEntity.GetAttributeValue<AliasedValue>("udo_idproof2.udo_interaction");
            if (aliasInteraction != null && aliasInteraction.Value != null)
            {
                _interaction = (EntityReference)aliasInteraction.Value;
            }

            #region Build Request

            var request = new UDOInitiateClaimEstablishmentRequest
            {
                MessageId = PluginExecutionContext.CorrelationId.ToString(),
                OrganizationName = PluginExecutionContext.OrganizationName,
                UserId = PluginExecutionContext.InitiatingUserId,
                Debug = _debug,
                LogSoap = _logSoap,
                LogTiming = _logTimer,
                LegacyServiceHeaderInfo = _headerInfo,
                awardtypecode = _awardTypeCode,
                vetfileNumber = _vet_filenumber,
                fileNumber = _filenumber,
                SSN = DWNDEntity.GetAttributeValue<string>("udo_ssn"),
                PayeeCode = _payeeCode,
                FirstName = _first,
                LastName = _last,
                udo_payeecodeid = _payeecodeid != null ? _payeecodeid.Id : Guid.Empty,
                udo_personid = Parent.Id,
                udo_idproofid = _idproof.Id,
                udo_veteranid = _vet.Id,
                udo_veteransnapshotid = _snapshot.Id,
                udo_interaction = _interaction.Id,
                address1 = _addressline1,
                address2 = _addressline2,
                address3 = _addressline3,
                city = _city,
                state = _state,
                postalcode = _postalcode
            };

            Int64 pid;
            if (Int64.TryParse(_pid, out pid))
            {
                request.ptcpntId = pid;
            }

            if (Int64.TryParse(_vet_pid, out pid))
            {
                request.vetptcpntId = pid;
            }
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

            tracer.Trace("calling UDOInitiateClaimEstablishmentRequest");
            var response = Utility.SendReceive<UDOInitiateClaimEstablishmentResponse>(_uri, "UDOInitiateClaimEstablishmentRequest", request, logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOInitiateClaimEstablishmentRequest");
            
            if (response.ExceptionOccurred)
            {
                _responseMessage = String.Format(FAILURE_MESSAGE, response.ExceptionMessage, PluginExecutionContext.CorrelationId.ToString());
                ExceptionOccurred = true;

                if (!string.IsNullOrEmpty(response.StackTrace))
                    PluginExecutionContext.OutputParameters["StackTrace"] = response.StackTrace;

                return;
            }

            PluginExecutionContext.OutputParameters["result"] = new EntityReference("udo_claimestablishment", response.ClaimEstablishmentId);
        }

        private const string FETCH_PERSON =
            "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' count='1' >" +
            "<entity name='udo_person' >" +
            "<attribute name='udo_first' />" +
            "<attribute name='udo_idproofid' />" +
            "<attribute name='udo_filenumber' />" +
            "<attribute name='udo_awardtypecode' />" +
            "<attribute name='udo_payeecode' />" +
            "<attribute name='udo_ssn' />" +
            "<attribute name='udo_state' />" +
            "<attribute name='udo_address1' />" +
            "<attribute name='udo_veteransnapshotid' />" +
            "<attribute name='udo_address3' />" +
            "<attribute name='ownerid' />" +
            "<attribute name='udo_veteranid' />" +
            "<attribute name='udo_ptcpntid' />" +
            "<attribute name='udo_last' />" +
            "<attribute name='udo_zip' />" +
            "<attribute name='udo_address2' />" +
            "<attribute name='udo_payeecodeid' />" +
            "<attribute name='udo_city' />" +
            "<filter type='and' >" +
            "<condition attribute='udo_personid' operator='eq' value='{0}' />" +
            "</filter>" +
            "<link-entity name='contact' from='contactid' to='udo_veteranid' link-type='outer' >" +
            "<attribute name='udo_filenumber' />" +
            "<attribute name='udo_participantid' />" +
            "</link-entity>" +
            "<link-entity name='udo_idproof' from='udo_idproofid' to='udo_idproofid' link-type='outer' >" +
            "<attribute name='udo_interaction' />" +
            "</link-entity>" +
            "</entity>" +
            "</fetch>";

        internal bool DidWeFindData()
        {
            try
            {
                Logger.setMethod = "DidWeFindData";

                _aliasGuid = String.Format("a_{0:N}", Guid.NewGuid());
                _fetchxml = String.Format(FETCH_PERSON, Parent.Id, _aliasGuid);

                var response = OrganizationService.RetrieveMultiple(new FetchExpression(_fetchxml));

                if (response.Entities.Count > 0)
                {
                    var entity = response.Entities.FirstOrDefault();
                    if (entity == null) return false;

                    #region Key Field Verification Check

                    if (entity.GetAttributeValue<EntityReference>("udo_veteranid") == null)
                    {
                        _responseMessage = String.Format(FAILURE_MESSAGE, "Veteran ID not found.", PluginExecutionContext.CorrelationId.ToString());
                        return false;
                    }

                    if (entity.GetAttributeValue<AliasedValue>("contact1.udo_filenumber") == null)
                    {
                        _responseMessage = String.Format(FAILURE_MESSAGE, "Veteran File Number not found.", PluginExecutionContext.CorrelationId.ToString());
                        return false;
                    }

                    if (string.IsNullOrEmpty(entity.GetAttributeValue<string>("udo_ptcpntid")))
                    {
                        _responseMessage = String.Format(FAILURE_MESSAGE, "Participant ID not found.", PluginExecutionContext.CorrelationId.ToString());
                        return false;
                    }

                    //if (string.IsNullOrEmpty(entity.GetAttributeValue<string>("udo_ssn")))
                    //{
                    //    if ((DWNDEntity.GetAttributeValue<string>("udo_ssn")).Length < 9)
                    //    {
                    //        _bypassMvi = true;
                    //    }
                    //}

                    //if (entity.GetAttributeValue<string>("contact1.udo_participantid") == null)
                    //{
                    //    _responseMessage = String.Format(FAILURE_MESSAGE, "Veteran Participant ID not found.");
                    //    return false;
                    //}

                    #endregion

                    DWNDEntity = entity;
                }
                else
                {
                    _responseMessage = String.Format(FAILURE_MESSAGE, "Person record not found.", PluginExecutionContext.CorrelationId.ToString());
                    return false;
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
