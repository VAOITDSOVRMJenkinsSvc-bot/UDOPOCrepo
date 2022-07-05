using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Diagnostics;
using VRM.Integration.UDO.Contact.Messages;
using VRMRest;

namespace Va.Udo.Crm.eBenefits.Plugins
{
    public class EBenefitsCustomActionRunner : PluginRunner
    {
        protected EntityReference _contact;
        protected string _edipi;
        bool _logSoap = false;
        bool _logTimer = false;
        string _uri = "";
        private const int _searchTimeout = 100;
        internal string _logTimerField = "udo_contactlogtimer";
        internal string _logSoapField = "udo_contactlogsoap";
        internal string _vimtRestEndpointField = "crme_restendpointforvimt";
        internal CRMAuthTokenConfiguration _crmAuthTokenConfig;
        internal string _debugField = "udo_contact";
        internal bool _debug = false;
        internal string _responseMessage = "NA";
        Stopwatch stopwatch;

        public EBenefitsCustomActionRunner(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        internal void Execute()
        {
            TracingService.Trace(">> Executing Action: {0}", this.GetType().Name);
            try
            {
                PluginExecutionContext.OutputParameters["ExceptionOccurred"] = false;
                if (PluginExecutionContext.InputParameters.Contains("ParentEntityReference"))
                {
                    _contact = PluginExecutionContext.InputParameters["ParentEntityReference"] as EntityReference;
                }

                if (PluginExecutionContext.InputParameters.Contains("EDIPI"))
                {
                    _edipi = (string) PluginExecutionContext.InputParameters["EDIPI"];
                    if (_edipi == null)
                    {
                        _edipi = "UNK";
                    }
                }

                getSettingValues();
                HeaderInfo headerInfo = GetHeaderInfo();

                _responseMessage = $" >>> Invoking UDOupdateHasBenefitsRequest .. ";
                var request = new UDOupdateHasBenefitsRequest()
                {
                    MessageId = PluginExecutionContext.CorrelationId.ToString(),
                    ContactId = _contact.Id.ToString(),
                    Edipi = _edipi, 
                    Debug = _debug,
                    LogSoap = _logSoap,
                    LogTiming = _logTimer,
                    UserId = PluginExecutionContext.InitiatingUserId,
                    OrganizationName = PluginExecutionContext.OrganizationName,
                    LegacyServiceHeaderInfo = headerInfo
                };

                LogSettings _logSettings = new LogSettings() { Org = PluginExecutionContext.OrganizationName, ConfigFieldName = "RESTCALL", UserId = PluginExecutionContext.InitiatingUserId };
                Uri uri = new Uri(_uri);

                if (string.IsNullOrEmpty(_uri))
                {
                    Logger.WriteToFile("NO URI FOUND, cannot call VIMT");
                    TracingService.Trace("NO URI FOUND, cannot call VIMT");
                    return;
                }
                var response = Utility.SendReceive<UDOupdateHasBenefitsResponse>(uri
                        , "UDOupdateHasBenefitsRequest", request, _logSettings, _searchTimeout, _crmAuthTokenConfig, TracingService);
                _responseMessage = $"{JsonHelper.Serialize<UDOupdateHasBenefitsResponse>(response)}";
            }
            catch (Exception ex)
            {
                _responseMessage+= $"Error in Plugin: {Utility.StackTraceToString(ex)}. Elapsed Time: {stopwatch.ElapsedMilliseconds} ms.";
                Logger.WriteToFile(_responseMessage);
                TracingService.Trace(_responseMessage);
                PluginExecutionContext.OutputParameters["ExceptionOccurred"] = true;
                PluginExecutionContext.OutputParameters["ResponseMessage"] = _responseMessage;
                throw new InvalidPluginExecutionException(_responseMessage, ex);
            }

            _responseMessage = $"<< Exited Action: {this.GetType().Name}. Elapsed Time: {stopwatch.ElapsedMilliseconds} ms."; 
            PluginExecutionContext.OutputParameters["ResponseMessage"] = _responseMessage;
            TracingService.Trace(_responseMessage);
        }

        internal HeaderInfo GetHeaderInfo()
        {
            ColumnSet userCols = new ColumnSet("va_stationnumber", "va_wsloginname", "va_applicationname", "va_ipaddress");
            Entity thisUser = OrganizationService.Retrieve("systemuser", PluginExecutionContext.InitiatingUserId, userCols);

            const string stationNumberIsNotAssignedForCrmUser = "Station Number is not assigned for CRM User.";
            const string vaStationnumber = "va_stationnumber";

            if (!thisUser.Attributes.ContainsKey(vaStationnumber))
                throw new Exception(stationNumberIsNotAssignedForCrmUser);

            const string wsLoginIsNotAssignedForCrmUser = "WS Login is not assigned for CRM User.";
            const string vaWsloginname = "va_wsloginname";

            if (!thisUser.Attributes.ContainsKey(vaWsloginname))
                throw new Exception(wsLoginIsNotAssignedForCrmUser);

            const string applicationNameIsNotAssignedForCrmUser = "Application Name is not assigned for CRM User.";
            const string vaApplicationname = "va_applicationname";

            if (!thisUser.Attributes.ContainsKey(vaApplicationname))
                throw new Exception(applicationNameIsNotAssignedForCrmUser);

            const string clientMachineIsNotAssignedForCrmUser = "Client Machine is not assigned for CRM User.";
            const string vaIpAddress = "va_ipaddress";

            if (!thisUser.Attributes.ContainsKey(vaIpAddress))
                throw new Exception(clientMachineIsNotAssignedForCrmUser);

            var stationNumber = (string)thisUser[vaStationnumber];
            var loginName = (string)thisUser[vaWsloginname];
            var applicationName = (string)thisUser[vaApplicationname];
            var clientMachine = (string)thisUser[vaIpAddress];

            return new HeaderInfo
            {
                StationNumber = stationNumber,
                LoginName = loginName,
                ApplicationName = applicationName,
                ClientMachine = clientMachine,
            };
        }

        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        public override Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        public override string McsSettingsDebugField
        {
            get { return "udo_contact"; }
        }

        protected void getSettingValues()
        {
            _logTimer = McsSettings.GetSingleSetting<bool>(_logTimerField);
            _logSoap = McsSettings.GetSingleSetting<bool>(_logSoapField);
            _debug = McsSettings.GetSingleSetting<bool>(_debugField);
            _uri = McsSettings.GetSingleSetting<string>(_vimtRestEndpointField);
            if (string.IsNullOrEmpty(_uri)) throw new NullReferenceException("NO URI FOUND, cannot call VIMT");
            
            #region CRMAuthenticationToken
            string parentAppId = McsSettings.GetSingleSetting<string>("udo_oauthresourceid");
            string clientAppId = McsSettings.GetSingleSetting<string>("udo_oauthclientid");
            string clientSecret = McsSettings.GetSingleSetting<string>("udo_oauthclientsecret");
            string tenentId = McsSettings.GetSingleSetting<string>("udo_aadtenent");
            string apimsubscriptionkey = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkey");
            string apimsubscriptionkeyS = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeysouth");
            string apimsubscriptionkeyE = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeyeast");

            //Create the token from settings
            _crmAuthTokenConfig = new CRMAuthTokenConfiguration
            {
                ParentApplicationId = parentAppId,
                ClientApplicationId = clientAppId,
                ClientSecret = clientSecret,
                TenantId = tenentId,
                ApimSubscriptionKey = apimsubscriptionkey,
                ApimSubscriptionKeyE = apimsubscriptionkeyE,
                ApimSubscriptionKeyS = apimsubscriptionkeyS
            };
            #endregion
        }
    }
}
