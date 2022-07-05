using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using System.Diagnostics;
using Microsoft.Xrm.Sdk.Messages;
using MCSHelperClass;
using Microsoft.Crm.Sdk.Messages;
using VRM.Integration.UDO.UserTool.Messages;
using VRMRest;

namespace Va.Udo.Crm.UserTool.Plugins
{
    internal class SecurityGroupPostAssociateRunner : PluginRunner
    {
        private string _uri = string.Empty;
        private bool _debug { get; set; }

        private CRMAuthTokenConfiguration _crmAuthTokenConfig;

        public SecurityGroupPostAssociateRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }
        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.PreEntityImages["pre"] as Entity;
        }
        public override Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }
        public override string McsSettingsDebugField
        {
            get { return "udo_usertool"; }
        }
        internal void Execute()
        {
            try
            {
                // Only updates 1 deep or through the UI trigger this plugin.
                //Logger.WriteDebugMessage("initiating validation.");
                TracingService.Trace("initiating validation.");
                if (PluginExecutionContext.Depth > 2) return;

                if (!PluginExecutionContext.InputParameters.Contains("Relationship") ||
                    !PluginExecutionContext.InputParameters.Contains("Target") ||
                    !PluginExecutionContext.InputParameters.Contains("RelatedEntities"))
                {
                    return;
                }

                var relationship = (Relationship)PluginExecutionContext.InputParameters["Relationship"];

                if (relationship.SchemaName != "udo_udo_securityrole_systemuser" &&
                    relationship.SchemaName != "udo_udo_securitygroup_systemuser" && 
                    relationship.SchemaName != "udo_udo_securitygroup_udo_securityrole" && 
                    relationship.SchemaName != "udo_udo_securitygroup_queue")
                {
                    return; // not the type of relationship for this plugin..
                }

                //Logger.WriteDebugMessage("Relationship: {0}:", relationship.SchemaName);
                #region Debug
                _debug = McsSettings.getDebug;
                #endregion
                
                Stopwatch txnTimer = Stopwatch.StartNew();

                var requests = new OrganizationRequestCollection();

                bool associate = (PluginExecutionContext.MessageName.ToUpperInvariant() == "ASSOCIATE");

                //Logger.WriteDebugMessage("Associate?: {0}:", associate);

                var target = (EntityReference)PluginExecutionContext.InputParameters["Target"];
                var related = (EntityReferenceCollection)PluginExecutionContext.InputParameters["RelatedEntities"];

                getSettingValues();

                if (associate)
                {
                    // Build LOB Request
                    var request = new UDOSecurityAssocRequest
                    {
                        MessageId = Guid.NewGuid().ToString(),
                        Debug = _debug,
                        LogSoap = false,
                        LogTiming = false,
                        UserId = PluginExecutionContext.InitiatingUserId,
                        OrganizationName = PluginExecutionContext.OrganizationName,
                        One = target,
                        Many = related.ToArray(),
                        Relationship = relationship.SchemaName
                    };
                    
                    //Logger.WriteDebugMessage("Request: {0}:", request);

                    LogSettings _logSettings = new LogSettings()
                    {
                        Org = PluginExecutionContext.OrganizationName,
                        ConfigFieldName = "RESTCALL",
                        UserId = PluginExecutionContext.InitiatingUserId,
                        callingMethod = "SecurityGroupPostAssociate"
                    };

                    Uri uri = new Uri(_uri);

                    if (string.IsNullOrEmpty(_uri))
                    {
                        //Logger.WriteToFile("NO URI FOUND, cannot call VIMT");
                        TracingService.Trace("NO URI FOUND, cannot call VIMT");
                        return;
                    }

                    

                    //Utility.Send(uri, "UDOSecurityAssocRequest", request, _logSettings);
                    var response = Utility.SendReceive<UDOSecurityAssocResponse>(uri, "UDOSecurityAssocRequest", request, _logSettings, 0, _crmAuthTokenConfig, TracingService);
                }
                else
                {
                    // Build LOB Request
                    var request = new UDOSecurityDisassocRequest
                    {
                        MessageId = Guid.NewGuid().ToString(),
                        Debug = _debug,
                        LogSoap = false,
                        LogTiming = false,
                        UserId = PluginExecutionContext.InitiatingUserId,
                        OrganizationName = PluginExecutionContext.OrganizationName,
                        One = target,
                        Many = related.ToArray(),
                        Relationship = relationship.SchemaName
                    };

                    //Logger.WriteDebugMessage("Request: {0}:", request);

                    LogSettings _logSettings = new LogSettings()
                    {
                        Org = PluginExecutionContext.OrganizationName,
                        ConfigFieldName = "RESTCALL",
                        UserId = PluginExecutionContext.InitiatingUserId,
                        callingMethod = "SecurityGroupPostDisassociate"
                    };

                    Uri uri = new Uri(_uri);

                    if (string.IsNullOrEmpty(_uri))
                    {
                        //Logger.WriteToFile("NO URI FOUND, cannot call VIMT");
                        TracingService.Trace("NO URI FOUND, cannot call VIMT");
                        return;
                    }

                    //Utility.Send(uri, "UDOSecurityDisassocRequest", request, _logSettings);
                    var response = Utility.SendReceive<UDOSecurityDisassocResponse>(uri, "UDOSecurityDisassocRequest", request, _logSettings, 0, _crmAuthTokenConfig, TracingService);
                }

                return;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                //Logger.WriteException(ex);
                TracingService.Trace(ex.Message);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
            catch (Exception ex)
            {
                //Logger.WriteException(ex);
                TracingService.Trace(ex.Message);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
        }

        internal void getSettingValues()
        {
            _uri = McsSettings.getVIMTRESTEndPoint;

            #region CRMAuthenticationToken
            //TODO: get settings for AuthToken from McsSettings
            
            //CSDev
            //OAuthResourceid
            //string parentAppId = McsSettings.GetSingleSetting<string>("udo_parentapplicationid");
            string parentAppId = McsSettings.GetSingleSetting<string>("udo_oauthresourceid");
            //OAuthClientId
            //string clientAppId = McsSettings.GetSingleSetting<string>("udo_clientapplicationid");
            string clientAppId = McsSettings.GetSingleSetting<string>("udo_oauthclientid");
            string clientSecret = McsSettings.GetSingleSetting<string>("udo_oauthclientsecret");
            //CSDev
            //string tenentId = McsSettings.GetSingleSetting<string>("udo_tenantId");
            string tenentId = McsSettings.GetSingleSetting<string>("udo_aadtenent");
            //CSDev
            //string apimsubscriptionkey = McsSettings.GetSingleSetting<string>("udo_apimsubscriptionkey");
            string apimsubscriptionkey = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkey");
            
            string apimsubscriptionkeyS = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeysouth");

            string apimsubscriptionkeyE = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeyeast");
            //TODO: Remove following values and uncomment above once settings McsSettings has been confirmed
            //string parentAppId = "309c45c5-4af4-4bc1-8f66-38ff1bf1f2dc";
            //string clientAppId = "1bfe8a8d-ba9e-459a-99e3-6e4057179f51";
            //string clientSecret = "SPogjAbtlBI7oJm9CN9Pu5iOfQoj4Yytmwm0AvNKLmg=";
            //string tenentId = "edeazclabs.va.gov";

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

            #endregion
        }

    }

}
