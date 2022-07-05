using System;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using MCSUtilities2011;
//using VRM.Integration.UDO.ClaimEstablishment.Messages;
//using VRM.Integration.UDO.Common;
//using VRM.Integration.UDO.Common.Messages;
using UDO.LOB.Core;
using UDO.LOB.ClaimEstablishment.Messages;
using VRMRest;
namespace CustomActions.Plugins.Entities.ClaimEstablishment
{
    public class UDOInsertClaimEstablishmentRunner : UDOActionRunner
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

        private const string FAILURE_MESSAGE = "{0} Unable to insert a Claim Establishment. CorrelationId: {1}";

        public UDOInsertClaimEstablishmentRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_claimestablishmentlogtimer";
            _logSoapField = "udo_claimestablishmentlogsoap";
            _debugField = "udo_claimestablishment";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_claimestablishmentvimttimeout";
            _validEntities = new string[] { "udo_claimestablishment" };
        }


        public override void DoAction()
        {
            _method = "DoAction";

            GetSettingValues();

            #region Build Request

            //_headerInfo = HeaderInfo.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

            var request = new UDOInsertClaimEstablishmentRequest
            {
                MessageId = PluginExecutionContext.CorrelationId.ToString(),
                OrganizationName = PluginExecutionContext.OrganizationName,
                UserId = PluginExecutionContext.InitiatingUserId,
                Debug = _debug,
                LogSoap = _logSoap,
                LogTiming = _logTimer,
                LegacyServiceHeaderInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId),
                ClaimEstablishmentId = Parent.Id
            };

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

            tracer.Trace("calling UDOInsertClaimEstablishmentRequest");
            ////Logger.WriteDebugMessage("");
            var response = Utility.SendReceive<UDOInsertClaimEstablishmentResponse>(_uri, "UDOInsertClaimEstablishmentRequest", request, logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
            tracer.Trace("Returned from UDOInsertClaimEstablishmentRequest");

            ExceptionOccurred = response.ExceptionOccurred;
            PluginExecutionContext.OutputParameters["result"] = new EntityReference("udo_claimestablishment", request.ClaimEstablishmentId);

            if (response.ExceptionOccurred)
            {
                _responseMessage = String.Format(FAILURE_MESSAGE, response.ExceptionMessage, PluginExecutionContext.CorrelationId.ToString());
                ExceptionOccurred = true;

                if (!string.IsNullOrEmpty(response.StackTrace))
                    PluginExecutionContext.OutputParameters["StackTrace"] = response.StackTrace;

                return;
            }
        }
    }
}
