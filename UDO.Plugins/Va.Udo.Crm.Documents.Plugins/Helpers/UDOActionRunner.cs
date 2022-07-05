using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using System.Diagnostics;
using System.Collections.Generic;
using System.ServiceModel;
// using UDO.LOB.Core;

namespace MCSPlugins
{
    public abstract class UDOActionRunner : PluginRunner
    {
        #region Members
        internal string _logTimerField;
        internal string _logSoapField;
        internal string _debugField;
        internal string _vimtRestEndpointField;
        internal string _vimtTimeoutField;
        internal string _method;
        internal string[] _validEntities;

        //internal string _source = "CRM";
        internal bool _logSoap;
        internal bool _logTimer;
        internal bool _debug;
        internal int _timeOutSetting;
        internal bool _addperson = false;
        internal bool _MVICheck = false;
        internal bool _bypassMvi = false;
        internal ITracingService tracer;
        internal Uri _uri = null;
        internal string _responseMessage = null;
        internal CRMAuthTokenConfiguration _crmAuthTokenConfig;
        #endregion

        public UDOActionRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        public override Entity GetSecondaryEntity()
        {
            throw new NotImplementedException("GetSecondaryEntity is not implemented.");
        }

        public override string McsSettingsDebugField
        {
            get { return _debugField; }
        }
        public EntityReference Parent
        {
            get { return PluginExecutionContext.InputParameters["ParentEntityReference"] as EntityReference; }
        }

        public Entity EntityParameter
        {
            get { return PluginExecutionContext.InputParameters["Entity"] as Entity; }
        }

        public bool PromptForRetry { get; set; }

        public bool Complete { get; set; }

        public bool DataIssue { get; set; }

        public bool ExceptionOccurred { get; set; }

        public bool EntityUpdate { get; set; }

        public abstract void DoAction();

        private string AssemblyMetadata()
        {
            System.Reflection.Assembly currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string name = currentAssembly.FullName;
            string version = ""; // this.GetType().Assembly.GetName().Version.ToString();
            return $" Assembly: {name}, Version: {version}";
        }

        internal void Execute()
        {
            #region start
            _method = "Execute";
            tracer = base.TracingService;
            tracer.Trace($">> {PluginExecutionContext.MessageName} Started. {AssemblyMetadata()}");
            // tracer.Trace(String.Format("{0} started", PluginExecutionContext.MessageName));
            Logger.setMethod = _method;
            Stopwatch txnTimer = Stopwatch.StartNew();
            #endregion

            try
            {
                if (_validEntities.Contains(Parent.LogicalName))
                {
                    DoAction();

                    if (DataIssue)
                    {
                        PluginExecutionContext.OutputParameters["DataIssue"] = true;
                        PluginExecutionContext.OutputParameters["ResponseMessage"] = _responseMessage;
                    }

                    if (ExceptionOccurred)
                    {
                        if (PromptForRetry) _responseMessage = string.Format("Error occurred. Please refresh to try your request again. {0}.", _responseMessage); ;
                        PluginExecutionContext.OutputParameters["Exception"] = true;
                        PluginExecutionContext.OutputParameters["ResponseMessage"] = _responseMessage;
                    }

                    /// EntityUpdate is used to return to calling context that an update was both required and performed. This enables calling context (if on a form) 
                    /// to recognize whether a form refresh should be performed. This is only used in a minority of customaction cases
                    if (EntityUpdate)
                    {
                        PluginExecutionContext.OutputParameters["EntityUpdate"] = true;
                    }

                    _method = PluginExecutionContext.MessageName;
                }
                else
                {
                    _responseMessage = String.Format("Invalid entity supplied to the runner: {0} \n Valid entities: {1}", Parent.LogicalName, string.Join(", ", _validEntities));
                    PluginExecutionContext.OutputParameters["Exception"] = true;
                    PluginExecutionContext.OutputParameters["ResponseMessage"] = _responseMessage;
                }

                txnTimer.Stop();
                Logger.setMethod = _method;
                //Logger.WriteTxnTimingMessage(_method, txnTimer.ElapsedMilliseconds);
            }

            catch (FaultException<OrganizationServiceFault> ex)
            {
                tracer.Trace(string.Format("Error message - {0}", ex.Message));
                Logger.WriteException(ex);
                if (_responseMessage == null || _responseMessage == "") _responseMessage = ex.Message;
                //throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
                PluginExecutionContext.OutputParameters["Exception"] = true;
                PluginExecutionContext.OutputParameters["ResponseMessage"] = _responseMessage;
            }
            catch (VRMRest.VIMTTimeOutExeption ex)
            {
                tracer.Trace(string.Format("Error message - {0}", ex.Message));
                Logger.WriteException(ex);
                if (_responseMessage == null || _responseMessage == "") _responseMessage = ex.Message;
                //throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
                _responseMessage = "Timeout occured retrieving data from source system. Please refresh to try again.";
                PluginExecutionContext.OutputParameters["Timeout"] = true;
                PluginExecutionContext.OutputParameters["ResponseMessage"] = _responseMessage;
            }
            catch (Exception ex)
            {
                tracer.Trace(string.Format("Error message - {0}", ex.Message));
                Logger.WriteException(ex);
                if (_responseMessage == null || _responseMessage == "") _responseMessage = ex.Message;
                //throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
                _responseMessage = string.Format("Error occurred. Please refresh to try your request again. {0}.", _responseMessage);
                PluginExecutionContext.OutputParameters["Exception"] = true;
                PluginExecutionContext.OutputParameters["ResponseMessage"] = _responseMessage;
            }
            finally
            {
                tracer.Trace(String.Format("<< {0} Complete", PluginExecutionContext.MessageName));
                txnTimer.Stop();
            }
        }

        protected void GetSettingValues()
        {
            tracer.Trace("getSettingValues started");

            _logTimer = McsSettings.GetSingleSetting<bool>(_logTimerField);
            _logSoap = McsSettings.GetSingleSetting<bool>(_logSoapField);

            var uri = McsSettings.GetSingleSetting<string>(_vimtRestEndpointField);

            if (string.IsNullOrEmpty(uri)) throw new NullReferenceException("NO URI FOUND, cannot call VIMT");

            _uri = new Uri(uri);
            _debug = McsSettings.GetSingleSetting<bool>(_debugField);
            _timeOutSetting = McsSettings.GetSingleSetting<int>(_vimtTimeoutField);
            _addperson = McsSettings.GetSingleSetting<bool>("udo_addperson");
            _MVICheck = McsSettings.GetSingleSetting<bool>("udo_mvicheck");
            _bypassMvi = McsSettings.GetSingleSetting<bool>("udo_bypassmvi");


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


            #endregion
        }

        internal static T GetAttributeAliasValue<T>(ITracingService tracer, Entity entity, string aliasGuid, string attributeName)
        {
            var aliasValue = entity.GetAttributeValue<AliasedValue>(String.Format("{0}.{1}", aliasGuid, attributeName));
            if (aliasValue != null)
            {
                if (aliasValue.Value != null)
                {
                    return (T)(aliasValue.Value);
                }
                tracer.Trace("{0}.{1} null or empty.", aliasValue.EntityLogicalName, attributeName);
                return default(T);
            }

            tracer.Trace("{1} not found.", aliasGuid, attributeName);
            return default(T);
        }
    }
}