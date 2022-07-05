using System;
using MCSPlugins;
using System.Text;
using Microsoft.Xrm.Sdk.Client;
using UDO.LOB.DependentMaintenance.Messages;
//using VRMRest;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class DependentMaintenanceDataProvider
    {
        private GetVeteranInfoMultipleResponse _Veteran;
        private GetDependentInfoMultipleResponse[] _Dependents;
        private GetMaritalInfoMultipleResponse[] _MaritalHistory;
        private CrmeSettings _crmeSettings;
        bool _logSoap = false;
        bool _logTimer = false;
        string _pcrspid = "";
        string _uri = "";
        private const string _vimtRestEndpointField = "crme_restendpointforvimt";
        private CRMAuthTokenConfiguration _crmAuthTokenConfig;


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

        public DependentMaintenanceDataProvider(string ssn,
    string participantId,
    PluginRunner pluginRunner,
    CrmeSettings crmeSettings)
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


        public GetVeteranInfoMultipleResponse GetVeteranInfo()
        {
            var request = new GetVeteranInfoRequest
            {
                crme_SSN = Ssn, 
                crme_OrganizationName = OrganizationName, 
                crme_UserId = UserId
            };

            getSettingValues();

            if (PluginRunner != null)
                PluginRunner.Logger.WriteGranularTimingMessage("Starting GetDependentInfoRequest");

            if (_crmeSettings == null)
                throw new Exception("CRME Settings cannot be null");

            //var response = VRMRest.Utility.SendReceive<GetVeteranInfoResponse>(new Uri(_crmeSettings.RestEndPointForVimt), "Bgs#GetVeteranInfoRequest", request, _crmeSettings.LogSettings);
            PluginRunner.TracingService.Trace("before response");
            var response = VRMRest.Utility.SendReceive<GetVeteranInfoResponse>(new Uri(_crmeSettings.RestEndPointForVimt), "Bgs#GetVeteranInfoRequest", request, _crmeSettings.LogSettings, 0, _crmAuthTokenConfig, PluginRunner.TracingService);

            PluginRunner.TracingService.Trace("Before String builder");
           // string responseSoap = response.SoapLog;
            PluginRunner.TracingService.Trace("1");
            StringBuilder soapLogBuilder = new StringBuilder(500000).Append(response.SoapLog);
            PluginRunner.TracingService.Trace("2");
            PluginRunner.PluginExecutionContext.SharedVariables.Add("SoapLog", soapLogBuilder.ToString());

            if (response.Fault != null) throw new Exception(response.Fault);

            if (response.GetVeteranInfo.Length == 0)
                throw new Exception(string.Format("No VA record found for veteran with SSN of '{0}'", Ssn));

            if (response.GetVeteranInfo.Length > 1)
                throw new Exception(string.Format("Multiple VA records found for veteran with SSN of '{0}'", Ssn));
            PluginRunner.TracingService.Trace("got veteran info");
            return response.GetVeteranInfo[0];
        }

        internal void getSettingValues()
        {
            _logTimer = PluginRunner.McsSettings.GetSingleSetting<bool>("udo_noteslogtimer");
            _logSoap = PluginRunner.McsSettings.GetSingleSetting<bool>("udo_noteslogsoap");


            _uri = PluginRunner.McsSettings.GetSingleSetting<string>(_vimtRestEndpointField);

            string parentAppId = PluginRunner.McsSettings.GetSingleSetting<string>("udo_oauthresourceid");
            string clientAppId = PluginRunner.McsSettings.GetSingleSetting<string>("udo_oauthclientid");
            string clientSecret = PluginRunner.McsSettings.GetSingleSetting<string>("udo_oauthclientsecret");
            string tenentId = PluginRunner.McsSettings.GetSingleSetting<string>("udo_aadtenent");
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
                ApimSubscriptionKeyE = apimsubscriptionkeyE,
                ApimSubscriptionKeyS = apimsubscriptionkeyS
            };
        }


        private GetDependentInfoMultipleResponse[] GetDependentInfo()
        {
            var request = new GetDependentInfoRequest
            {
                crme_SSN = Ssn, 
                crme_ParticipantId = ParticipantId,
                crme_OrganizationName = OrganizationName,
                crme_UserId = UserId
            };

            if (_crmeSettings == null)
                throw new Exception("CRME Settings cannot be null");
            getSettingValues();

            if (PluginRunner != null)
                PluginRunner.Logger.WriteGranularTimingMessage("Starting GetDependentInfoRequest");

            //var response = VRMRest.Utility.SendReceive<GetDependentInfoResponse>(new Uri(_crmeSettings.RestEndPointForVimt),
            //     "Bgs#GetDependentInfoRequest",
            //     request,
            //     _crmeSettings.LogSettings);
            PluginRunner.TracingService.Trace("sending Dependent Request");
            var response = VRMRest.Utility.SendReceive<GetDependentInfoResponse>(new Uri(_crmeSettings.RestEndPointForVimt),"Bgs#GetDependentInfoRequest", request, _crmeSettings.LogSettings, 0, _crmAuthTokenConfig, PluginRunner.TracingService);
            PluginRunner.TracingService.Trace("got Dependent Response");
           // var test = response.GetDependentInfo[0].crme_DOB;
           // PluginRunner.TracingService.Trace("Sample DOB in string is: " + response.GetDependentInfo[0].crme_DOB);
           // var month = test.Substring(0, 2);
           // var day = test.Substring(2, 2);
           // var year = test.Substring(4, 4);

           // DateTime testDate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));
           //// DateTime parsedDate = Convert.ToDateTime(test);
           ////  DateTime parsedDate = DateTime.Parse(test);
           //PluginRunner.TracingService.Trace("Sample DOB in string is: " + testDate);
            

            if (PluginRunner.PluginExecutionContext.SharedVariables.ContainsKey("SoapLog")) {
                string soaplog = (string)(PluginRunner.PluginExecutionContext.SharedVariables["SoapLog"]);
                StringBuilder sb = new StringBuilder(soaplog);
                sb.Append(response.SoapLog);
                PluginRunner.PluginExecutionContext.SharedVariables["SoapLog"] = sb.ToString();
            }

            if (response.Fault != null) throw new Exception(response.Fault); //.Message, response.Fault);
           
            return response.GetDependentInfo;
        }

        private GetMaritalInfoMultipleResponse[] GetMaritalInfo()
        {
            var request = new GetMaritalInfoRequest 
            { crme_SSN = Ssn,
              crme_ParticipantId = ParticipantId,
              crme_OrganizationName = OrganizationName,
              crme_UserId = UserId
            };

            if (_crmeSettings == null)
                throw new Exception("CRME Settings cannot be null");
            getSettingValues();

            if (PluginRunner != null)
                PluginRunner.Logger.WriteGranularTimingMessage("Starting GetMaritalInfoRequest");

            //var response = VRMRest.Utility.SendReceive<GetMaritalInfoResponse>(new Uri(_crmeSettings.RestEndPointForVimt),
            //    "Bgs#GetMaritalInfoRequest",
            //    request,
            //    _crmeSettings.LogSettings);

            var response = VRMRest.Utility.SendReceive<GetMaritalInfoResponse>(new Uri(_crmeSettings.RestEndPointForVimt),
                "Bgs#GetMaritalInfoRequest",
                request,
                _crmeSettings.LogSettings, 0, _crmAuthTokenConfig, PluginRunner.TracingService);

            if (PluginRunner.PluginExecutionContext.SharedVariables.ContainsKey("SoapLog"))
            {

                string soaplog = (string)(PluginRunner.PluginExecutionContext.SharedVariables["SoapLog"]);
                StringBuilder sb = new StringBuilder(soaplog);
                sb.Append(response.SoapLog);
                PluginRunner.PluginExecutionContext.SharedVariables["SoapLog"] = sb.ToString();
                //    StringBuilder sb = (StringBuilder)PluginRunner.PluginExecutionContext.SharedVariables["SoapLog"];
                //    sb.Append(response.SoapLog);
            }

            if (response.Fault != null) throw new Exception(response.Fault); //.Message, response.Fault);
            

            return response.GetMaritalInfo;
        }

        public string Ssn { get; private set; }

        public string ParticipantId { get; private set; }

        public  string OrganizationName { get; private set; }

        public Guid UserId { get; set; }

        public PluginRunner PluginRunner { get; private set; }

       
        public GetVeteranInfoMultipleResponse Veteran
        {
            get { return _Veteran ?? (_Veteran = GetVeteranInfo()); }
        }

        public GetDependentInfoMultipleResponse[] Dependents
        {
            get { return _Dependents ?? (_Dependents = GetDependentInfo()); }
        }

        public GetMaritalInfoMultipleResponse[] MaritalHistory
        {
            get { return _MaritalHistory ?? (_MaritalHistory = GetMaritalInfo()); }
        }

        //public MessageProcessType FetchMessageProcessType
        //{
        //    get { return PluginRunner != null ? PluginRunner.FetchMessageProcessType : MessageProcessType.Local; }
        //}
    }
}
