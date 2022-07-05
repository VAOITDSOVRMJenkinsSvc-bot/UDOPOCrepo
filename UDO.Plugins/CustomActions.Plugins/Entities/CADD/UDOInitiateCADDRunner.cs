using System;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using MCSHelperClass;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using UDO.LOB.Core;
using UDO.LOB.CADD.Messages;
//using VRM.Integration.UDO.CADD.Messages;
//using VRM.Integration.UDO.Common.Messages;
using VRMRest;

namespace CustomActions.Plugins.Entities.CADD
{
    public class UDOInitiateCADDRunner : UDOActionRunner
    {

        #region Members
        protected UDOHeaderInfo _headerInfo;
        //protected SecureString _ssid;
        //[Gokul]: Commenting this as '_bypassMvi' is already declared in the UDOActionRunner class
        //protected bool _bypassMvi;
        protected string _payeeCode = string.Empty;
        protected string _awardTypeCode = string.Empty;
        protected string _pid = string.Empty;
        protected string _vet_pid = string.Empty;
        protected string _vet_filenumber = string.Empty;
        protected string _first = string.Empty;
        protected string _last = string.Empty;
        private Entity DWNDEntity = null;

        protected EntityReference _idproof;
        protected EntityReference _snapshot;
        protected EntityReference _vet;
        protected EntityReference _owner;
        #endregion

        private const string FAILURE_MESSAGE = "{0} Unable to initialize a Change of Address. CorrelationId: {1}";

        public UDOInitiateCADDRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_caddlogtimer";
            _logSoapField = "udo_caddlogsoap";
            _debugField = "udo_cadd";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_caddtimeout";
            _validEntities = new string[] { "udo_person" };
        }

        public override void DoAction()
        {
            _method = "DoAction";

            GetSettingValues();
            _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            if (DataIssue = !DidWeFindData()) return;

            #region Build Request
            var request = new UDOInitiateCADDRequest
            {
                MessageId = PluginExecutionContext.CorrelationId.ToString(),
                OrganizationName = PluginExecutionContext.OrganizationName,
                UserId = PluginExecutionContext.InitiatingUserId,
                Debug = _debug,
                LogSoap = _logSoap,
                LogTiming = _logTimer,
                LegacyServiceHeaderInfo = _headerInfo,
                udo_personId = Parent.Id,
                awardtypecode = _awardTypeCode,
                vetfileNumber = _vet_filenumber,
                SSN = DWNDEntity.GetAttributeValue<string>("udo_ssn"),
                PayeeCode = _payeeCode
            };

            //request.appealFirstName = _first;
            //request.appealLastName = _last;
            //bankaccount
            //routingnumber

            if (_idproof != null) request.udo_IDProofId = _idproof.Id;
            if (_snapshot != null) request.udo_snapshotid = _snapshot.Id;
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

            tracer.Trace("calling UDOInitiateCADDRequest");
            var response = Utility.SendReceive<UDOInitiateCADDResponse>(_uri, "UDOInitiateCADDRequest", request, logSettings, _timeOutSetting,_crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOInitiateCADDRequest");

            request.SSN = null;
            //_ssid = null;

            if (response.ExceptionOccurred)
            {
                ExceptionOccurred = true;
                _responseMessage = String.Format(FAILURE_MESSAGE, response.ExceptionMessage, request.MessageId);
                Logger.WriteToFile($"Error message: {_responseMessage}. Inner Exceptions: {GetFormattedInnerException(response)}");
                return;
            }

            PluginExecutionContext.OutputParameters["result"] = new EntityReference("va_bankaccount", response.CADDId);
        }

        private string GetFormattedInnerException(UDOInitiateCADDResponse response)
        {
            string formattedException = $"Inner Exceptions : ";
            if (response.InnerExceptions != null && response.InnerExceptions.Length > 0)
            {
                foreach (var exception in response.InnerExceptions)
                {
                    formattedException+= ($"[ExceptionCategory: {exception.ExceptionCategory}, ExceptionMessage: {exception.ExceptionMessage}]");
                }
            }
            else
            {
                formattedException += "[ NONE ]";
            }
            return formattedException;
        }
        
        private const string FETCH_PERSON =
                    "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' count='1'>" +
                    "<entity name='udo_person'>" +
                    "<attribute name='ownerid'/>" +
                    "<attribute name='udo_payeecode'/>" +
                    "<attribute name='udo_awardtypecode'/>" +
                    "<attribute name='udo_idproofid'/>" +
                    "<attribute name='udo_veteranid'/>" +
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
                    DWNDEntity = entity;

                    _vet = entity.GetAttributeValue<EntityReference>("udo_veteranid");
                    var aliasFilenumber = entity.GetAttributeValue<AliasedValue>(string.Format("{0}.udo_filenumber", aliasGuid));
                    if (aliasFilenumber != null && aliasFilenumber.Value != null)
                    {
                        _vet_filenumber = aliasFilenumber.Value.ToString();
                    }

                    var aliasVetPid =
                        entity.GetAttributeValue<AliasedValue>(string.Format("{0}.udo_participantid", aliasGuid));
                    if (aliasVetPid != null && aliasVetPid.Value != null)
                    {
                        _vet_pid = aliasVetPid.Value.ToString();
                    }

                    _owner = entity.GetAttributeValue<EntityReference>("ownerid");
                    _payeeCode = entity.GetAttributeValue<string>("udo_payeecode");
                    _awardTypeCode = entity.GetAttributeValue<string>("udo_awardtypecode");
                    _idproof = entity.GetAttributeValue<EntityReference>("udo_idproofid");
                    //_ssid = MCSHelper.ConvertToSecureString(entity.GetAttributeValue<string>("udo_ssn"));
                    _pid = entity.GetAttributeValue<string>("udo_ptcpntid");
                    _first = entity.GetAttributeValue<string>("udo_first");
                    _last = entity.GetAttributeValue<string>("udo_last");
                    _snapshot = entity.GetAttributeValue<EntityReference>("udo_veteransnapshotid");

                    if (_vet == null)
                    {
                        _responseMessage = String.Format(FAILURE_MESSAGE, "Veteran ID not found.", PluginExecutionContext.CorrelationId.ToString());
                        return false;
                    }

                    if (_vet_filenumber == null)
                    {
                        _responseMessage = String.Format(FAILURE_MESSAGE, "Veteran File Number not found.", PluginExecutionContext.CorrelationId.ToString());
                        return false;
                    }

                    if (string.IsNullOrEmpty(_pid))
                    {
                        _responseMessage = String.Format(FAILURE_MESSAGE, "Participant ID not found.", PluginExecutionContext.CorrelationId.ToString());
                        return false;
                    }

                    if (!string.IsNullOrEmpty(DWNDEntity.GetAttributeValue<string>("udo_ssn")))
                    {
                        if ((DWNDEntity.GetAttributeValue<string>("udo_ssn")).Length < 9)
                        {
                            _bypassMvi = true;
                            ////Logger.WriteDebugMessage("SSN < 9 - Bypass MVI set to true");
                        }
                    }

                    if (String.IsNullOrEmpty(_vet_pid))
                    {
                        _responseMessage = String.Format(FAILURE_MESSAGE, "Veteran Participant ID not found.", PluginExecutionContext.CorrelationId.ToString());
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
