using System;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using VRMRest;
using Microsoft.Xrm.Sdk.Query;
using VRM.Integration.UDO.Contact.Messages;

namespace Va.Udo.Crm.eBenefits.Plugins
{
    internal class IDProofCreatePostStageRunner : PluginRunner
    {
        bool _logSoap = false;
        bool _logTimer = false;
        string _uri = "";
		private const int _searchTimeout = 100;
		internal CRMAuthTokenConfiguration _crmAuthTokenConfig;
		internal string _logTimerField = "udo_contactlogtimer";
		internal string _logSoapField = "udo_contactlogsoap";
		internal string _debugField = "udo_contact";
		internal string _vimtRestEndpointField = "crme_restendpointforvimt";
		//internal string _vimtTimeoutField = "udo_contacttimeout";
		//internal string[] _validEntities = new string[] { "udo_contact" };
		//internal Uri _uri2 = null;
		internal bool _debug;
		//internal int _timeOutSetting;
		public IDProofCreatePostStageRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        internal void Execute()
        {
            try
            {
                var entity = GetPrimaryEntity();

                if (entity == null)
                {
                    PluginError = true;
                    throw new InvalidPluginExecutionException("Target entity is null");
                }

                var image = PluginExecutionContext.PostEntityImages["PostCreateImage"];

                var vetId = image.GetAttributeValue<EntityReference>("udo_veteran");

                var interactionId = image.GetAttributeValue<EntityReference>("udo_interaction");

                if (image != null && interactionId != null && vetId != null)
                {
                    //retrieve createdby from interaction
                    Entity getCreatedBy = OrganizationService.Retrieve(interactionId.LogicalName, interactionId.Id, new ColumnSet("createdby"));

                    var createdbyId = (getCreatedBy.GetAttributeValue<EntityReference>("createdby")).Id;

                    HeaderInfo headerInfo = GetHeaderInfo(createdbyId);
                    //retrieve edipi from contact
                    Entity getEdipi = OrganizationService.Retrieve(vetId.LogicalName, vetId.Id, new ColumnSet("udo_edipi"));

                    var edipi = getEdipi.GetAttributeValue<string>("udo_edipi");
                    if (edipi == null)
                    {
                        edipi = "UNK";
                    }

                    getSettingValues();

                    var request = new UDOupdateHasBenefitsRequest()
                    {
                        MessageId = PluginExecutionContext.CorrelationId.ToString(),
                        ContactId = vetId.Id.ToString(),
                        Edipi = edipi, //getEdipi.GetAttributeValue<string>("udo_edipi"),
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
                        Trace("NO URI FOUND, cannot call VIMT");
                        return;
                    }
                    //CSDev Rem  unify the helpers as links
                    //var response = Utility.SendReceive<UDOupdateHasBenefitsResponse>(uri, "UDOupdateHasBenefitsRequest", request, _logSettings);
                    var response = Utility.SendReceive<UDOupdateHasBenefitsResponse>(uri
                            , "UDOupdateHasBenefitsRequest", request, _logSettings, _searchTimeout, _crmAuthTokenConfig, TracingService);
                }
                else
                {
                    PluginError = true;
                    throw new InvalidPluginExecutionException("Image not provided or does not contain veteranid");
                }


            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.WriteToFile("Error in Plugin:" + Utility.StackTraceToString(ex));
                Trace("Error in Plugin:" + ex.Message);
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

		//internal void getSettingValues()
		//{
		//    _logTimer = McsSettings.GetSingleSetting<bool>("udo_claimlogtimer");
		//    _logSoap = McsSettings.GetSingleSetting<bool>("udo_claimlogsoap");
		//    _uri = McsSettings.getVIMTRESTEndPoint;
		//}

		protected void getSettingValues()
		{
			Trace("getSettingValues started");

			_logTimer = McsSettings.GetSingleSetting<bool>(_logTimerField);
			_logSoap = McsSettings.GetSingleSetting<bool>(_logSoapField);
            _debug = McsSettings.GetSingleSetting<bool>(_debugField);

            //CSDEv Assign to Global
            _uri = McsSettings.GetSingleSetting<string>(_vimtRestEndpointField);
            if (string.IsNullOrEmpty(_uri))
            {
                PluginError = true;
                throw new NullReferenceException("NO URI FOUND, cannot call VIMT");
            }
            #region CRMAuthenticationToken
            //TODO: get settings for AuthToken from McsSettings

            //CSDev
            //OAuthResourceid
            string parentAppId = McsSettings.GetSingleSetting<string>("udo_oauthresourceid");
			//OAuthClientId
			string clientAppId = McsSettings.GetSingleSetting<string>("udo_oauthclientid");
			string clientSecret = McsSettings.GetSingleSetting<string>("udo_oauthclientsecret");
			//CSDev
			string tenentId = McsSettings.GetSingleSetting<string>("udo_aadtenent");
			//CSDev
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

		internal HeaderInfo GetHeaderInfo( Guid createdBy)
        {
            ColumnSet userCols = new ColumnSet("va_stationnumber", "va_wsloginname", "va_applicationname", "va_ipaddress");
            Entity thisUser = OrganizationService.Retrieve("systemuser", createdBy, userCols);

            const string stationNumberIsNotAssignedForCrmUser = "Station Number is not assigned for CRM User.";
            const string vaStationnumber = "va_stationnumber";

            if (!thisUser.Attributes.ContainsKey(vaStationnumber))
            {
                PluginError = true;
                throw new Exception(stationNumberIsNotAssignedForCrmUser);
            }
            const string wsLoginIsNotAssignedForCrmUser = "WS Login is not assigned for CRM User.";
            const string vaWsloginname = "va_wsloginname";

            if (!thisUser.Attributes.ContainsKey(vaWsloginname))
            {
                PluginError = true;
                throw new Exception(wsLoginIsNotAssignedForCrmUser);
            }

            const string applicationNameIsNotAssignedForCrmUser = "Application Name is not assigned for CRM User.";
            const string vaApplicationname = "va_applicationname";

            if (!thisUser.Attributes.ContainsKey(vaApplicationname))
            {
                PluginError = true;
                throw new Exception(applicationNameIsNotAssignedForCrmUser);
            }
            const string clientMachineIsNotAssignedForCrmUser = "Client Machine is not assigned for CRM User.";
            const string vaIpAddress = "va_ipaddress";

            if (!thisUser.Attributes.ContainsKey(vaIpAddress))
            {
                PluginError = true;
                throw new Exception(clientMachineIsNotAssignedForCrmUser);
            }
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
    }
}
