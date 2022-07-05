using System;
using System.Linq;
using MCSHelperClass;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using System.Security;
using System.Runtime.InteropServices;
using System.Text;
using VRMRest;
using VRM.Integration.UDO.ServiceRequest.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Va.Udo.Crm.ServiceRequests.Plugins
{
    internal class PostUpdateServiceRequestRunner : PluginRunner
    {
        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity"s attributes before the core platform operation executes.
        /// The image contains the following attributes:
        /// All Attributes
        /// </summary>
        private readonly string preImageAlias = "PreImage";

        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity"s attributes after the core platform operation executes.
        /// The image contains the following attributes:
        /// All Attributes
        /// 
        /// Note: Only synchronous post-event and asynchronous registered plug-ins 
        /// have PostEntityImages populated.
        /// </summary>
        private readonly string postImageAlias = "PostImage";
        private const string _vimtRestEndpointField = "crme_restendpointforvimt";
        internal bool _logSoap;
        internal bool _logTimer;
        internal bool _debug;
        internal string _uri;
        private CRMAuthTokenConfiguration _crmAuthTokenConfig;
        public PostUpdateServiceRequestRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        public Entity GetPrimaryEntityPostImage()
        {
            return (PluginExecutionContext.PostEntityImages != null && PluginExecutionContext.PostEntityImages.Contains(this.postImageAlias)) ? PluginExecutionContext.PostEntityImages[this.postImageAlias] as Entity : null;
        }

        public Entity GetPrimaryEntityPreImage()
        {
            return (PluginExecutionContext.PreEntityImages != null && PluginExecutionContext.PreEntityImages.Contains(this.preImageAlias)) ? PluginExecutionContext.PreEntityImages[this.preImageAlias] as Entity : null;
        }

        public override Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        public T GetAttributeValue<T>(string field)
        {
            var primary = GetPrimaryEntity();
            if (primary != null && primary.Contains(field)) return primary.GetAttributeValue<T>(field);
            var pre = GetPrimaryEntityPreImage();
            if (pre != null && pre.Contains(field)) return pre.GetAttributeValue<T>(field);
            return default(T);
        }

        public override string McsSettingsDebugField
        {
            get { return "udo_servicerequest"; }
        }

        /// <summary>
        /// The primary execute method initiated from the plugin and the entry function to the runner class
        /// </summary>
        internal void Execute()
        {
            

            var target = PluginExecutionContext.InputParameters["Target"] as Entity;
            if (target == null || !target.LogicalName.Equals("udo_servicerequest", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("No Target udo_servicerequest found.");
            }
            //Logger.WriteDebugMessage("{0}: Starting Service Request Post Update Service Request Runner", PluginExecutionContext.MessageName);

            if (!GetAttributeValue<Boolean>("udo_sendnotestomapd"))
            {
                //Logger.WriteDebugMessage("udo_sendnotestomapd: false");
                return;
            }
            if (GetAttributeValue<Boolean>("udo_notecreated"))
            {
                //Logger.WriteDebugMessage("udo_notecreated: true");
                return;
            }

            var sr = new Entity(target.LogicalName);
            sr.Id = target.Id;
            sr["udo_notecreated"] = true;
            
            try
            {
                OrganizationService.Update(sr);

                TracingService.Trace("\r\nGetting Settings\r\n");
                getSettingValues();
                TracingService.Trace("Keys Passed: " + String.Join(",", PluginExecutionContext.InputParameters.Keys) + "\r\n");

                #region Get VIMT URI

                var uri = new Uri(_uri);

                if (string.IsNullOrEmpty(_uri))
                {
                    //Logger.WriteToFile("NO URI FOUND, cannot call VIMT");
                    TracingService.Trace("NO URI FOUND, cannot call VIMT");
                    return;
                }

                #endregion
                UDOUpdateSRRequest request = new UDOUpdateSRRequest()
                {
                    LogSoap = _logSoap,
                    Debug = _debug,
                    LogTiming = _logTimer,
                    OrganizationName = PluginExecutionContext.OrganizationName,
                    UserId = PluginExecutionContext.InitiatingUserId,
                    MessageId = Guid.NewGuid().ToString(),
                    udo_ServiceRequestId = target.Id
                };
                
                LogSettings _logSettings = new LogSettings()
                {
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL",
                    UserId = PluginExecutionContext.InitiatingUserId,
                    callingMethod = "PostUpdateServiceRequest"
                };
                
                var response = Utility.SendReceive<UDOUpdateSRResponse>(uri, "UDOUpdateSRRequest", request, _logSettings, 0, _crmAuthTokenConfig, TracingService);
                if (response.ExceptionOccured)
                {
                    throw new Exception(response.ExceptionMessage);
                }
            }
            catch (InvalidPluginExecutionException ex)
            {
                // Pass all invalidpluginexceptions
                sr["udo_notecreated"] = false;
                OrganizationService.Update(sr);
                throw ex;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                //Logger.WriteToFile(ex.Message);
                TracingService.Trace(ex.Message + ex.StackTrace);
                sr["udo_notecreated"] = false;
                OrganizationService.Update(sr);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
            catch (Exception ex)
            {
                //Logger.WriteToFile(ex.Message);
                TracingService.Trace(ex.Message + ex.StackTrace);
                sr["udo_notecreated"] = false;
                OrganizationService.Update(sr);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
            //Logger.WriteDebugMessage("Ending Service Request Post Update Request Runner");
        }
        internal void getSettingValues()
        {
            _logTimer = McsSettings.GetSingleSetting<bool>("udo_appeallogtimer");
            _logSoap = McsSettings.GetSingleSetting<bool>("udo_appeallogsoap");
            _debug = McsSettings.GetSingleSetting<bool>("udo_ssrsdebug");

            _uri = McsSettings.GetSingleSetting<string>(_vimtRestEndpointField); ;

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

            try
            {

                if (_debug)
                {

                    TracingService.Trace("CRMAuthTokenConfiguration : " + JsonHelper.Serialize<CRMAuthTokenConfiguration>(_crmAuthTokenConfig));

                }
            }
            catch (Exception e)
            {
                TracingService.Trace(e.Message);
            }

        }
    }
}
