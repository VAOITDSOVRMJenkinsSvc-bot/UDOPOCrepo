using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using UDO.LOB.Core;
using UDO.LOB.Letters.Messages;
//using VRM.Integration.UDO.Common.Messages;
using VRMRest;

namespace CustomActions.Plugins.Entities.Letters
{
    public class UDOInitiateLetterRunner : UDOActionRunner
    {

        #region Members
        protected UDOHeaderInfo _headerInfo;
        //protected SecureString _ssid;
        protected string _payeeCode = string.Empty;
        protected string _awardTypeCode = string.Empty;
        protected string _pid = string.Empty;
        protected string _first = string.Empty;
        protected string _last = string.Empty;

        protected string _vet_first = string.Empty;
        protected string _vet_middle = string.Empty;
        protected string _vet_last = string.Empty;
        protected string _vet_dobstr = string.Empty;
        protected string _vet_gender = string.Empty;
        protected string _vet_pid = string.Empty;
        protected string _vet_filenumber = string.Empty;
        //protected SecureString _vet_ssid;

        private string _aliasGuid = null;
        private Entity DWNDEntity = null;
        private Entity VetEntity = null;
        protected EntityReference _idproof;
        protected EntityReference _snapshot;
        protected EntityReference _vet;
        protected EntityReference _owner;
        #endregion

        private const string FAILURE_MESSAGE = "{0} Unable to initialize a letter. CorrelationId: {1}";

        public UDOInitiateLetterRunner(IServiceProvider serviceProvider)
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
           try
            {
                _method = "DoAction";

                GetSettingValues();
                _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

                if (DataIssue = !DidWeFindData()) return;

                #region Build Request
                var request = new UDOInitiateLettersRequest
                {
                    MessageId = PluginExecutionContext.CorrelationId.ToString(),
                    OrganizationName = PluginExecutionContext.OrganizationName,
                    UserId = PluginExecutionContext.InitiatingUserId,
                    Debug = _debug,
                    LogSoap = _logSoap,
                    LogTiming = _logTimer,
                    LegacyServiceHeaderInfo = _headerInfo,
                    udo_personId = Parent.Id,
                    vetfileNumber = _vet_filenumber,
                    SSN = DWNDEntity.GetAttributeValue<string>("udo_ssn"),
                    vetSSN = GetAttributeAliasValue<string>(tracer, DWNDEntity, _aliasGuid, "udo_ssn"),
                    vetFirstName = _vet_first,
                    vetLastName = _vet_last,
                    vetMiddleInitial = _vet_middle,
                    vetDOB = _vet_dobstr,
                    vetGender = _vet_gender,
                    OwnerId = _owner.Id,
                    OwnerType = _owner.LogicalName
                };

                if (_snapshot != null) request.udo_vetsnapshotId = _snapshot.Id;
                if (_vet != null) request.udo_veteranId = _vet.Id;

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

                tracer.Trace("calling UDOInitiateLettersRequest");
                Trace("calling UDOInitiateLettersRequest");
                var response = Utility.SendReceive<UDOInitiateLettersResponse>(_uri, "UDOInitiateLettersRequest", request, logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
                tracer.Trace("Returned from UDOInitiateLettersRequest");
                Trace("Returned from UDOInitiateLettersRequest");

                #region clear PII markers
                request.SSN = null;
                request.vetSSN = null;
                //_ssid = null;
                //_vet_ssid = null;
                #endregion

                if (response.ExceptionOccurred)
                {
                    _responseMessage = string.Format(FAILURE_MESSAGE, response.ExceptionMessage, PluginExecutionContext.CorrelationId.ToString());
                    ExceptionOccurred = true;
                    return;
                }
                //if (response.UDOInitiateLettersExceptions != null && response.UDOInitiateLettersExceptions.VeteranExceptionOccured)
                //{
                //    _responseMessage = String.Format(FAILURE_MESSAGE, response.UDOInitiateLettersExceptions.VeteranExceptionMessage);
                //    ExceptionOccurred = true;
                //    return; // don't set result if exception occurred
                //}

                PluginExecutionContext.OutputParameters["result"] = new EntityReference("udo_lettergeneration",
                    response.newUDOInitiateLetterId);
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
                    "<attribute name='udo_payeecode'/>" +
                    "<attribute name='udo_awardtypecode'/>" +
                    "<attribute name='udo_idproofid'/>" +
                    "<attribute name='udo_veteranid'/>" +
                    "<attribute name='udo_filenumber'/>" +
                    "<attribute name='udo_ssn'/>" +
                    "<attribute name='udo_first'/>" +
                    "<attribute name='udo_last'/>" +
                    "<attribute name='udo_ptcpntid'/>" +
                    "<attribute name='udo_veteransnapshotid'/>" +
                    "<filter type='and'>" +
                    "<condition attribute='udo_personid' operator='eq' value='{0}' />" +
                    "</filter>" +
                    "<link-entity name='contact' from='contactid' to='udo_veteranid' visible='false' link-type='outer' alias='{1}'>" +
                    "<attribute name='udo_filenumber' />" +
                    "<attribute name='udo_participantid' />" +
                    "<attribute name='udo_ssn' />" +
                    "<attribute name='firstname' />" +
                    "<attribute name='lastname' />" +
                    "<attribute name='middlename' />" +
                    "<attribute name='udo_birthdatestring' />" +
                    "<attribute name='udo_gender' />" +
                    "</link-entity>" +
                    "</entity></fetch>";

        internal bool DidWeFindData()
        {
            try
            {
                Logger.setMethod = "DidWeFindData";
                ////Logger.WriteDebugMessage("Starting didWeNeedData Method");

                _aliasGuid = String.Format("a_{0:N}", Guid.NewGuid());
                var fetchxml = String.Format(FETCH_PERSON, Parent.Id, _aliasGuid);

                var response = OrganizationService.RetrieveMultiple(new FetchExpression(fetchxml));

                if (response.Entities.Count > 0)
                {

                    var entity = response.Entities.FirstOrDefault();
                    if (entity == null) return false;
                    DWNDEntity = entity;

                    _vet = entity.GetAttributeValue<EntityReference>("udo_veteranid");
                    _vet_filenumber = GetAttributeAliasValue<string>(tracer, entity, _aliasGuid, "udo_filenumber");
                    _vet_pid = GetAttributeAliasValue<string>(tracer, entity, _aliasGuid, "udo_participantid");
                    //_vet_ssid = MCSHelper.ConvertToSecureString(GetAttributeAliasValue<string>(tracer, entity, aliasGuid, "udo_ssn"));
                    _vet_first = GetAttributeAliasValue<string>(tracer, entity, _aliasGuid, "firstname");
                    _vet_last = GetAttributeAliasValue<string>(tracer, entity, _aliasGuid, "lastname");
                    _vet_middle = GetAttributeAliasValue<string>(tracer, entity, _aliasGuid, "middlename");
                    _vet_dobstr = GetAttributeAliasValue<string>(tracer, entity, _aliasGuid, "udo_birthdatestring");
                    _vet_gender = GetAttributeAliasValue<string>(tracer, entity, _aliasGuid, "udo_gender");


                    _owner = entity.GetAttributeValue<EntityReference>("ownerid");

                    _idproof = entity.GetAttributeValue<EntityReference>("udo_idproofid");
                    _snapshot = entity.GetAttributeValue<EntityReference>("udo_veteransnapshotid");
                    //_ssid = MCSHelper.ConvertToSecureString(entity.GetAttributeValue<string>("udo_ssn"));
                    _pid = entity.GetAttributeValue<string>("udo_ptcpntid");
                    _first = entity.GetAttributeValue<string>("udo_first");
                    _last = entity.GetAttributeValue<string>("udo_last");
                    _awardTypeCode = entity.GetAttributeValue<string>("udo_awardtypecode");
                    _payeeCode = entity.GetAttributeValue<string>("udo_payeecode");

                    if (_vet == null)
                    {
                        _responseMessage = String.Format(FAILURE_MESSAGE, "Veteran ID not found.", PluginExecutionContext.CorrelationId.ToString());
                        return false;
                    }

                    if (GetAttributeAliasValue<string>(tracer, DWNDEntity, _aliasGuid, "udo_ssn") == null)
                    {
                        _responseMessage = String.Format(FAILURE_MESSAGE, "Veteran SSN not found.", PluginExecutionContext.CorrelationId.ToString());
                        return false;
                    }

                    if (string.IsNullOrEmpty(_pid))
                    {
                        _responseMessage = String.Format(FAILURE_MESSAGE, "Participant ID not found.", PluginExecutionContext.CorrelationId.ToString());
                        return false;
                    }

                    if (string.IsNullOrEmpty(_vet_filenumber))
                    {
                        _responseMessage = String.Format(FAILURE_MESSAGE, "Veteran File Number not found.", PluginExecutionContext.CorrelationId.ToString());
                        return false;
                    }

                    if (String.IsNullOrEmpty(_vet_pid))
                    {
                        _responseMessage = String.Format(FAILURE_MESSAGE, "Veteran Participant ID not found.", PluginExecutionContext.CorrelationId.ToString());
                        return false;
                    }

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
