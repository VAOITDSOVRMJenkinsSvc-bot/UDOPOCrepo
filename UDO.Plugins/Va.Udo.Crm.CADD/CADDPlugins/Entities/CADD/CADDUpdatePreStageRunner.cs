using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using UDO.Model;
using VRM.Integration.UDO.CADD.Messages;
using VRMRest;
using System.Net.Http;
using Microsoft.Xrm.Sdk.Query;

using System.Diagnostics;
using UDO.LOB.Core;
namespace Va.Udo.Crm.CADD.Plugins
{
    internal class CADDUpdatePreStageRunner : PluginRunner
    {
        bool _logSoap = false;
        bool _logTimer = false;
        string _uri = "";
        internal CRMAuthTokenConfiguration _crmAuthTokenConfig;

        public CADDUpdatePreStageRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

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
            get { return "udo_cadd"; }
        }

        internal void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                //Extract the tracing service for use in debugging sandboxed plug-ins.
                ITracingService tracingService =
                    (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                tracingService.Trace("CADD Plugin started...");
                Stopwatch txnTimer = Stopwatch.StartNew();
                //var Logger = new MCSLogger { setMethod = "Execute" };
               // Logger.WriteDebugMessage("Starting Claims Creation from ID Proof");
                var entity = GetPrimaryEntity();

                if (entity == null)
                {
                    throw new InvalidPluginExecutionException("Target entity is null");
                }

                var cadd = GetPrimaryEntity();
                //Logger.WriteDebugMessage("Target ID Proof set");

                try
                {
                    if (cadd.Attributes.Contains("va_failedidproofing") && cadd.GetAttributeValue<bool>("va_failedidproofing"))
                    {
                        Entity currentUser = OrganizationService.Retrieve("systemuser", PluginExecutionContext.InitiatingUserId, new ColumnSet("va_filenumber", "va_stationnumber", "fullname"));

                        Entity asIs = OrganizationService.Retrieve(cadd.LogicalName, cadd.Id, new ColumnSet("udo_veteranid", "udo_personid"));

                        udo_note newNote = new udo_note()
                        {
                            udo_VeteranId = cadd.Attributes.Contains("udo_veteranid") ? cadd.GetAttributeValue<EntityReference>("udo_veteranid") : asIs.GetAttributeValue<EntityReference>("udo_veteranid"),
                            udo_userid = currentUser.GetAttributeValue<string>("va_filenumber"),
                            udo_User = currentUser.GetAttributeValue<string>("fullname"),
                            udo_Type = "CADD ID Protocol",
                            udo_RO = currentUser.GetAttributeValue<string>("va_stationnumber"),
                            udo_personId = cadd.Attributes.Contains("udo_personid") ? cadd.GetAttributeValue<EntityReference>("udo_personid") : asIs.GetAttributeValue<EntityReference>("udo_personid"),
                            udo_NoteText = "UDO: CADD ID Protocol Failed.",
                            udo_Note = "UDO: CADD ID Protocol Failed.",
                            udo_name = "CADD Detail",
                            udo_idProofId = cadd.Attributes.Contains("udo_idproofid") ? cadd.GetAttributeValue<EntityReference>("udo_idproofid") : asIs.GetAttributeValue<EntityReference>("udo_idproofid"),
                            udo_fromUDO = true,
                            udo_editable = true,
                            udo_dtTime = DateTime.UtcNow,
                            udo_DateTime = DateTime.UtcNow.ToString(),
                            udo_CreateDtString = DateTime.UtcNow.ToString(),
                            udo_createdt = DateTime.UtcNow
                        };

                        OrganizationService.Create(newNote);
                    }
                }
                catch (Exception ex)
                {
                    //Logger.WriteDebugMessage(ex.Message);
                    throw new Exception(ex.StackTrace);
                }

                if (!cadd.Attributes.Contains("udo_caddstatus"))
                {
                    return;
                }
                if (!cadd.Attributes.Contains("va_routingnumber"))
                {
                    return;
                }
                var currentStatus = (OptionSetValue) cadd["udo_caddstatus"];

                if (currentStatus.Value == 752280001)
                {
                    getSettingValues();

                    //var references = new[] { veteranReference };
                    ///Logger.WriteTxnTimingMessage("Starting Claims Creation");
                    HeaderInfo HeaderInfo = GetHeaderInfo();

                    var request = new UDOFindBankRequest()
                    {
                        MessageId = PluginExecutionContext.CorrelationId.ToString(),
                        RoutingNumber = cadd["va_routingnumber"].ToString(),
                        Debug = McsSettings.getDebug,
                        LogSoap = _logSoap,
                        LogTiming = _logTimer,
                        UserId = PluginExecutionContext.InitiatingUserId,
                        OrganizationName = PluginExecutionContext.OrganizationName,
                        LegacyServiceHeaderInfo = HeaderInfo
                    };

                    // Logger.WriteDebugMessage("Request Created");
                    LogSettings _logSettings = new LogSettings() { Org = PluginExecutionContext.OrganizationName, ConfigFieldName = "RESTCALL", UserId = PluginExecutionContext.InitiatingUserId };

                    Uri uri = new Uri(_uri);

                    if (string.IsNullOrEmpty(_uri))
                    {
                        Logger.WriteToFile("NO URI FOUND, cannot call VIMT");
                        tracingService.Trace("NO URI FOUND, cannot call VIMT");
                        return;
                    }
                    var response = Utility.SendReceive<UDOFindBankResponse>(uri, "UDOFindBankRequest", request, _logSettings, 0, _crmAuthTokenConfig, tracingService);

                    cadd["va_bankname"] = response.bankName;
                    cadd["udo_caddstatus"] = new OptionSetValue(752280000);
                    //cadd["udo_crn"] = string.Empty;
                    PluginExecutionContext.InputParameters["Target"] = cadd;
                    txnTimer.Stop();
                    Logger.setMethod = "CADD Update";
                    //Logger.WriteTxnTimingMessage("CADD Update", txnTimer.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                TracingService.Trace("Error in Plugin:" + ex.Message + "STACKTRACE - " + ex.StackTrace);
                Logger.WriteToFile("Error in Plugin:" + ex.Message);
            }
        }
        
        internal void getSettingValues()
        {
            _logTimer = McsSettings.GetSingleSetting<bool>("udo_claimlogtimer");
            _logSoap = McsSettings.GetSingleSetting<bool>("udo_claimlogsoap");
            _uri = McsSettings.getVIMTRESTEndPoint;

            string parentAppId = McsSettings.GetSingleSetting<string>("udo_oauthresourceid");
            string clientAppId = McsSettings.GetSingleSetting<string>("udo_oauthclientid");
            string clientSecret = McsSettings.GetSingleSetting<string>("udo_oauthclientsecret");
            string tenentId = McsSettings.GetSingleSetting<string>("udo_aadtenent");
            string apimsubscriptionkey = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkey");
            string apimsubscriptionkeyE = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeyeast");
            string apimsubscriptionkeyS = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeysouth");

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
        }

        internal HeaderInfo GetHeaderInfo()
        {
            ColumnSet userCols = new ColumnSet("va_stationnumber", "va_wsloginname", "va_applicationname","va_ipaddress");
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


    }
}
