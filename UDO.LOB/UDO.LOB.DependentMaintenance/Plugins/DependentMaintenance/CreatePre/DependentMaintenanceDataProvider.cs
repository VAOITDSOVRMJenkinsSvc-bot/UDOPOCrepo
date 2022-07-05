using System;
using MCSPlugins;
using System.Text;
using VRMRest;
using MCSUtilities2011;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class DependentMaintenanceDataProvider
	{
        private Messages.GetVeteranInfoMultipleResponse _Veteran;
        private Messages.GetDependentInfoMultipleResponse[] _Dependents;
        private Messages.GetMaritalInfoMultipleResponse[] _MaritalHistory;
        private CrmeSettings _crmeSettings;

		//CSDev extra fields added 
		private const int _searchTimeout = 100;
		internal CRMAuthTokenConfiguration _crmAuthTokenConfig;
		internal string _vimtRestEndpointField = "crme_restendpointforvimt";
		internal Uri _uri = null;


		//CSDev Debug Fields 
		internal string _debugField = "crme_dependentmaintenance";
		internal bool _debug;

		//public DependentMaintenanceDataProvider(string ssn,
		//    string participantId,
		//    PluginRunner pluginRunner)
		//{

		//    Ssn = ssn;
		//    OrganizationName = pluginRunner.PluginExecutionContext.OrganizationName;
		//    UserId = pluginRunner.PluginExecutionContext.InitiatingUserId;
		//    ParticipantId = participantId;
		//    PluginRunner = pluginRunner;

		//}

		public DependentMaintenanceDataProvider(string ssn, string participantId, PluginRunner pluginRunner, CrmeSettings crmeSettings)
        {
            Ssn = ssn;
            OrganizationName = pluginRunner.PluginExecutionContext.OrganizationName;
            UserId = pluginRunner.PluginExecutionContext.InitiatingUserId;
            ParticipantId = participantId;
            PluginRunner = pluginRunner;
            _crmeSettings = crmeSettings;
        }

        public DependentMaintenanceDataProvider(string ssn,
            string participantId,
            string organizationName,
            Guid userId,
            CrmeSettings crmeSettings)
        {
            Ssn = ssn;
            OrganizationName = organizationName;
            UserId = userId;
            ParticipantId = participantId;
            PluginRunner = null;
            _crmeSettings = crmeSettings;
        }

		//Tested and Good 
        public Messages.GetVeteranInfoMultipleResponse GetVeteranInfo()
        {

			if (PluginRunner != null)
				PluginRunner.TracingService.Trace("Starting GetVeteranInfoRequest");

			//CSDev	
			GetSettingValues();

			var request = new Messages.GetVeteranInfoRequest
			{
				crme_SSN = Ssn,
				crme_OrganizationName = OrganizationName,
				crme_UserId = UserId,
				crme_debug = _debug
            };

            if (_crmeSettings == null)
                throw new Exception("CRME Settings cannot be null");
			
			////CSDev	
			//GetSettingValues();

			PluginRunner.TracingService.Trace($"| >> Entered { this.GetType().FullName}.GetVeteranInfo | Before  Utility.SendReceive uri: " + _uri.ToString());
			PluginRunner.TracingService.Trace("SSN: " + request.crme_SSN + "| Org NAme: " + request.crme_OrganizationName 
				+ "| UserID: " + request.crme_UserId + "| Debug: " + request.crme_debug);

			//CSDev We got rid of the logger method
			//var response = VRMRest.Utility.SendReceive<Messages.GetVeteranInfoResponse>(new Uri(_crmeSettings.RestEndPointForVimt), "Bgs#GetVeteranInfoRequest", request, _crmeSettings.LogSettings);
			//var response = Utility.SendReceive<Messages.GetVeteranInfoResponse>(_uri, "Bgs#GetVeteranInfoRequest", request, _crmeSettings.LogSettings, _searchTimeout, _crmAuthTokenConfig, PluginRunner.Logger);
			var response = Utility.SendReceive<Messages.GetVeteranInfoResponse>(_uri, "Bgs#GetVeteranInfoRequest", request, _crmeSettings.LogSettings, _searchTimeout, _crmAuthTokenConfig, PluginRunner.TracingService);

			//CSDev String Builder gives searilization errors in sandboxed plugins
			//StringBuilder soapLogBuilder = new StringBuilder(500000).Append(response.SoapLog);

			if (response.SoapLog == null) PluginRunner.TracingService.Trace($"| >> In { this.GetType().FullName}.GetVeteranInfo | After Utility.SendReceive | Response.SoapLog IS NULL!");

			String soapLogBuilder = response.SoapLog;
            PluginRunner.PluginExecutionContext.SharedVariables.Add("SoapLog", soapLogBuilder);

            if (response.Fault != null) throw new Exception(response.Fault);

			if (response.GetVeteranInfo.Length == 0)
                throw new Exception(string.Format("No VA record found for veteran with SSN of '{0}'", Ssn));

            if (response.GetVeteranInfo.Length > 1)
                throw new Exception(string.Format("Multiple VA records found for veteran with SSN of '{0}'", Ssn));

			//CSDev
			PluginRunner.TracingService.Trace($"| >> Pre End { this.GetType().FullName}.GetSettingValues() | After Get Debug from MCS Settings: " + _debug.ToString());
			PluginRunner.TracingService.Trace($"| >> END { this.GetType().FullName}.GetVeteranInfo()");

			return response.GetVeteranInfo[0];
        }


        private Messages.GetDependentInfoMultipleResponse[] GetDependentInfo()
        {
            var request = new Messages.GetDependentInfoRequest
            {
                crme_SSN = Ssn, 
                crme_ParticipantId = ParticipantId,
                crme_OrganizationName = OrganizationName,
                crme_UserId = UserId
            };

            if (_crmeSettings == null)
                throw new Exception("CRME Settings cannot be null");

            if (PluginRunner != null)
                PluginRunner.TracingService.Trace("Starting GetDependentInfoRequest");

			//CSDev	
			GetSettingValues();
			//PluginRunner.TracingService.Trace($"| >> Entered { this.GetType().FullName}.GetDependentInfo | Before  Utility.SendReceive uri: " + _uri.ToString());

			//CSDev
			//var response = Utility.SendReceive<Messages.GetDependentInfoResponse>(new Uri(_crmeSettings.RestEndPointForVimt), "Bgs#GetDependentInfoRequest", request, _crmeSettings.LogSettings);

			//CSDev we got rid of the logging method
			//var response = Utility.SendReceive<Messages.GetDependentInfoResponse>(_uri, "Bgs#GetDependentInfoRequest", request, _crmeSettings.LogSettings, _searchTimeout, _crmAuthTokenConfig, PluginRunner.Logger);
			var response = Utility.SendReceive<Messages.GetDependentInfoResponse>(_uri, "Bgs#GetDependentInfoRequest", request, _crmeSettings.LogSettings, _searchTimeout, _crmAuthTokenConfig, PluginRunner.TracingService);

			//CSDev 
			if (PluginRunner.PluginExecutionContext.SharedVariables.ContainsKey("SoapLog"))
			{
				//CSDev String Builder gives searilization errors in sandboxed plugins
				//StringBuilder sb = (StringBuilder)PluginRunner.PluginExecutionContext.SharedVariables["SoapLog"];
				//sb.Append(response.SoapLog);
				String sb = (String)PluginRunner.PluginExecutionContext.SharedVariables["SoapLog"];
				sb = String.Concat(sb, response.SoapLog);
			}

			if (response.Fault != null) throw new Exception(response.Fault); //.Message, response.Fault);

			//PluginRunner.TracingService.Trace($"| >> Entered { this.GetType().FullName}.GetDependentInfo | After");

			return response.GetDependentInfo;
        }

        private Messages.GetMaritalInfoMultipleResponse[] GetMaritalInfo()
        {
			PluginRunner.TracingService.Trace($"| >> Entered { this.GetType().FullName}.GetMaritalInfo!");

			var request = new Messages.GetMaritalInfoRequest 
            { crme_SSN = Ssn,
              crme_ParticipantId = ParticipantId,
              crme_OrganizationName = OrganizationName,
              crme_UserId = UserId
            };

            if (_crmeSettings == null)
                throw new Exception("CRME Settings cannot be null");

            if (PluginRunner != null)
                PluginRunner.TracingService.Trace("Starting GetMaritalInfoRequest");

			//CSDev	
			GetSettingValues();
			PluginRunner.TracingService.Trace($"| >> Entered { this.GetType().FullName}.GetMaritalInfo | Before  Utility.SendReceive uri: " + _uri.ToString());

			//CSDev
			//var response = VRMRest.Utility.SendReceive<Messages.GetMaritalInfoResponse>(new Uri(_crmeSettings.RestEndPointForVimt), "Bgs#GetMaritalInfoRequest", request, _crmeSettings.LogSettings);

			//CSDev we got rid of the logging method
			//var response = VRMRest.Utility.SendReceive<Messages.GetMaritalInfoResponse>(_uri, "Bgs#GetMaritalInfoRequest", request, _crmeSettings.LogSettings, _searchTimeout, _crmAuthTokenConfig, PluginRunner.Logger);
			var response = VRMRest.Utility.SendReceive<Messages.GetMaritalInfoResponse>(_uri, "Bgs#GetMaritalInfoRequest", request, _crmeSettings.LogSettings, _searchTimeout, _crmAuthTokenConfig, PluginRunner.TracingService);

			//CSDv
			if (PluginRunner.PluginExecutionContext.SharedVariables.ContainsKey("SoapLog"))
			{
				//CSDev String Builder gives searilization errors in sandboxed plugins
				//StringBuilder sb = (StringBuilder)PluginRunner.PluginExecutionContext.SharedVariables["SoapLog"];
				//sb.Append(response.SoapLog);
				String sb = (String)PluginRunner.PluginExecutionContext.SharedVariables["SoapLog"];
				sb = String.Concat(sb, response.SoapLog);
			}

			if (response.Fault != null) throw new Exception(response.Fault); //.Message, response.Fault);
            

            return response.GetMaritalInfo;
        }

        public string Ssn { get; private set; }

        public string ParticipantId { get; private set; }

        public  string OrganizationName { get; private set; }

        public Guid UserId { get; set; }

        public PluginRunner PluginRunner { get; private set; }

       
        public Messages.GetVeteranInfoMultipleResponse Veteran
        {
            get { return _Veteran ?? (_Veteran = GetVeteranInfo()); }
        }

        public Messages.GetDependentInfoMultipleResponse[] Dependents
        {
            get { return _Dependents ?? (_Dependents = GetDependentInfo()); }
        }

        public Messages.GetMaritalInfoMultipleResponse[] MaritalHistory
        {
            get { return _MaritalHistory ?? (_MaritalHistory = GetMaritalInfo()); }
        }

		//public MessageProcessType FetchMessageProcessType
		//{
		//    get { return PluginRunner != null ? PluginRunner.FetchMessageProcessType : MessageProcessType.Local; }
		//}

		protected void GetSettingValues()
		{
			PluginRunner.TracingService.Trace("getSettingValues started");

			//CSDev
			/*
			//_logTimer = PluginRunner.McsSettings.GetSingleSetting<bool>(_logTimerField);
			//_logSoap = PluginRunner.McsSettings.GetSingleSetting<bool>(_logSoapField);
			//var uri = PluginRunner.McsSettings.GetSingleSetting<string>(_vimtRestEndpointField);

			//if (string.IsNullOrEmpty(uri)) throw new NullReferenceException("NO URI FOUND, cannot call VIMT");
			*/

			_uri = new Uri(PluginRunner.McsSettings.GetSingleSetting<string>(_vimtRestEndpointField));
			_debug = PluginRunner.McsSettings.GetSingleSetting<bool>(_debugField);
			

			//CSdev
			/*
			_uri2 = new Uri(uri);
			_debug = McsSettings.GetSingleSetting<bool>(_debugField);
			_timeOutSetting = McsSettings.GetSingleSetting<int>(_vimtTimeoutField);
			_addperson = McsSettings.GetSingleSetting<bool>("udo_addperson");
			_MVICheck = McsSettings.GetSingleSetting<bool>("udo_mvicheck");
			_bypassMvi = McsSettings.GetSingleSetting<bool>("udo_bypassmvi");
			*/

			#region CRMAuthenticationToken
			//get settings for AuthToken from McsSettings

			//CSDev
			//OAuthResourceid
			//string parentAppId = McsSettings.GetSingleSetting<string>("udo_parentapplicationid");
			string parentAppId = PluginRunner.McsSettings.GetSingleSetting<string>("udo_oauthresourceid");
			//OAuthClientId
			//string clientAppId = McsSettings.GetSingleSetting<string>("udo_clientapplicationid");
			string clientAppId = PluginRunner.McsSettings.GetSingleSetting<string>("udo_oauthclientid");
			string clientSecret = PluginRunner.McsSettings.GetSingleSetting<string>("udo_oauthclientsecret");
			//CSDev
			//string tenentId = McsSettings.GetSingleSetting<string>("udo_tenantId");
			string tenentId = PluginRunner.McsSettings.GetSingleSetting<string>("udo_aadtenent");
			//CSDev
			//string apimsubscriptionkey = McsSettings.GetSingleSetting<string>("udo_apimsubscriptionkey");
			string apimsubscriptionkey = PluginRunner.McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkey");
           
		    string apimsubscriptionkeyS = PluginRunner.McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeysouth");

			string apimsubscriptionkeyE = PluginRunner.McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeyeast");
			//Create the token from settings
			_crmAuthTokenConfig = new CRMAuthTokenConfiguration
			{
				ParentApplicationId = parentAppId,
				ClientApplicationId = clientAppId,
				ClientSecret = clientSecret,
				TenantId = tenentId,
				ApimSubscriptionKey = apimsubscriptionkey,
				ApimSubscriptionKeyS = apimsubscriptionkeyS,
				ApimSubscriptionKeyE = apimsubscriptionkeyE
			};
			#endregion
		}

	}

}
