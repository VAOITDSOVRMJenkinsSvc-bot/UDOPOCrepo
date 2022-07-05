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

namespace Va.Udo.Crm.ServiceRequests.Plugins
{
    internal class PostCloneServiceRequestRunner : PluginRunner
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

        public PostCloneServiceRequestRunner(IServiceProvider serviceProvider)
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

        public override string McsSettingsDebugField
        {
            get { return "udo_servicerequest"; }
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
        }

        /// <summary>
        /// The primary execute method initiated from the plugin and the entry function to the runner class
        /// </summary>
        internal void Execute()
        {
            try
            {
                TracingService.Trace("{0}: Starting Service Request Post Clone Service Request Runner", PluginExecutionContext.MessageName);
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
                if (!PluginExecutionContext.InputParameters.ContainsKey("Target"))
                {
                    TracingService.Trace("Target not found");

                    throw new Exception("No Target udo_servicerequest found");
                }
                var target = PluginExecutionContext.InputParameters["Target"] as EntityReference;

                if (target == null || !target.LogicalName.Equals("udo_servicerequest", StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                UDOCloneSRRequest request = new UDOCloneSRRequest()
                {
                    udo_ServiceRequestId = target.Id,
                    Debug = _debug,
                    OrganizationName = PluginExecutionContext.OrganizationName,
                    UserId = PluginExecutionContext.InitiatingUserId,
                    MessageId = Guid.NewGuid().ToString(),
                };

                TracingService.Trace("Target: {0} {1}", target.LogicalName, target.Id);

                LogSettings _logSettings = new LogSettings()
                {
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL",
                    UserId = PluginExecutionContext.InitiatingUserId,
                    callingMethod = "PostCloneServiceRequest"
                };
                
                var response = Utility.SendReceive<UDOCloneSRRequest>(uri, "UDOCloneSRRequest", request, _logSettings, 0, _crmAuthTokenConfig, TracingService);

                //Logger.WriteDebugMessage("Service Request {0} was cloned to: {1}", target.Id, response.udo_ServiceRequestId);

                PluginExecutionContext.OutputParameters["CloneReference"] = new EntityReference("udo_servicerequest", response.udo_ServiceRequestId);

                //Logger.WriteDebugMessage("Ending Service Request Post Clone Request Runner");
                TracingService.Trace("Ending Service Request Post Clone Request Runner");
            }
            catch(Exception ex)
            {
                //Logger.WriteToFile(ex.Message);
                TracingService.Trace(ex.Message + ex.StackTrace);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
        }
    }
}
