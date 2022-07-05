using MCSHelperClass;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
//additions
using VA.AppInsights;
using System.Diagnostics;
using System.Text;
using VA.UDO.Plugins.Helpers;
//end additions

namespace MCSPlugins
{
    public abstract class PluginRunner
    {
        #region Private Fields
        private readonly IServiceProvider _ServiceProvider;
        private IPluginExecutionContext _PluginExecutionContext;
        private IOrganizationServiceFactory _OrganizationServiceFactory;
        private IOrganizationService _OrganizationService;
        private IOrganizationService _ElevatedOrganizationService;
        private ITracingService _TracingService;
        private MCSLogger _Logger;
        private UtilityFunctions _UtilityFunctions;
        private MCSSettings _McsSettings;
        private MCSHelper _McsHelper;
        #endregion
        //addition
        public IServiceEndpointNotificationService NotificationService;
        public Stopwatch Timer;
        public Entity PrimaryEntity;
        public string Secure { get; set; }
        public string UnSecure { get; set; }

        public Guid PluginExecutionInstanceId { get; set; }

        public AppInsightsLogData LogData { get; set; }

        public StringBuilder TraceLog { get; set; }

        public bool PluginError { get; set; }

        public Entity SettingsRecord { get; set; }

        internal bool VerboseTracing { get; set; }
        //end addition
        #region Constructor
        protected PluginRunner(IServiceProvider serviceProvider)
        {
            //addition
            Timer = Stopwatch.StartNew();
            //end addition
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            _ServiceProvider = serviceProvider;

            Logger.setDebug = McsSettings.getDebug;
            Logger.setTxnTiming = McsSettings.getTxnTiming;
            Logger.setGranularTiming = McsSettings.getGranular;

            McsSettings.setLogger = Logger;
            //addition
            NotificationService = (IServiceEndpointNotificationService)serviceProvider.GetService(typeof(IServiceEndpointNotificationService));
            PluginExecutionInstanceId = Guid.NewGuid();
            LogData = new AppInsightsLogData();
            TraceLog = new StringBuilder();
            PluginError = false;
            Trace($"{PluginExecutionInstanceId}: Plugin Execution Instance, Base Constructed, Correlation Id: {PluginExecutionContext.CorrelationId}, Initiating User: {PluginExecutionContext.InitiatingUserId}");
            //end addition
        }
        #endregion

        #region Internal Methods/Properties
        internal IServiceProvider ServiceProvider
        {
            get
            {
                return _ServiceProvider;
            }
        }

        internal IPluginExecutionContext PluginExecutionContext
        {
            get
            {
                return _PluginExecutionContext ??
                       (_PluginExecutionContext =
                        (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext)));
            }
        }

        internal IOrganizationServiceFactory OrganizationServiceFactory
        {
            get
            {
                return _OrganizationServiceFactory ??
                       (_OrganizationServiceFactory =
                        (IOrganizationServiceFactory)ServiceProvider.GetService(typeof(IOrganizationServiceFactory)));
            }
        }

        internal IOrganizationService OrganizationService
        {
            get
            {
                return _OrganizationService ??
                       (_OrganizationService =
                        OrganizationServiceFactory.CreateOrganizationService(PluginExecutionContext.UserId));
            }
        }

        internal IOrganizationService ElevatedOrganizationService
        {
            get
            {
                return _ElevatedOrganizationService ??
                       (_ElevatedOrganizationService =
                        OrganizationServiceFactory.CreateOrganizationService(null));
            }
        }

        internal OrganizationServiceContext OrganizationServiceContext
        {

            get
            {
                var OrgService = _OrganizationService ??
                       (_OrganizationService =
                        OrganizationServiceFactory.CreateOrganizationService(PluginExecutionContext.UserId));
                return new OrganizationServiceContext(OrgService);

            }
        }

        internal ITracingService TracingService
        {
            get
            {
                return _TracingService ??
                       (_TracingService =
                        (ITracingService)ServiceProvider.GetService(typeof(ITracingService)));
            }
        }

        internal MCSLogger Logger
        {
            get
            {
                return _Logger ??
                       (_Logger = new MCSLogger
                       {
                           setService = OrganizationService,
                           setTracingService = TracingService,
                           setModule = String.Format("{0}:", GetType()),
                           CallingUserId = PluginExecutionContext.InitiatingUserId,
                           OrganizationName = PluginExecutionContext.OrganizationName,
                           CorrelationId = PluginExecutionContext.CorrelationId
                       });
            }
        }

        internal UtilityFunctions Utilities
        {
            get
            {
                return _UtilityFunctions ??
                    (_UtilityFunctions = new UtilityFunctions
                    {
                        setService = OrganizationService,
                        setLogger = Logger
                    });
            }
        }

        internal MCSSettings McsSettings
        {
            get
            {
                if (_McsSettings == null)
                {
                    _McsSettings = new MCSSettings
                    {
                        setService = ElevatedOrganizationService,
                        setDebugField = McsSettingsDebugField,
                        systemSetting = "Active Settings"
                    };

                    _McsSettings.GetStartupSettings(PluginExecutionContext);
                }

                return _McsSettings;
            }
        }

        internal MCSHelper McsHelper
        {
            get
            {
                return _McsHelper ?? (_McsHelper = new MCSHelper(GetPrimaryEntity(), GetSecondaryEntity()));
            }
        }

        public abstract Entity GetPrimaryEntity();

        public abstract Entity GetSecondaryEntity();

        public abstract string McsSettingsDebugField { get; }
        #endregion
        public void Trace(string message, int level = 4, bool writeTraceToAI = true)
        {
            if (level <= 4 && level >= 1)
                Trace(message, (LogLevel)level, writeTraceToAI);
            else
                Trace(message, LogLevel.Debug, writeTraceToAI);
        }
        internal void Trace(string message, LogLevel level, bool writeTraceToAI = true)
        {
            if (string.IsNullOrWhiteSpace(message) || TracingService == null)
            {
                return;
            }
            if (Timer != null)
                message = $"{Timer.ElapsedMilliseconds}ms: {message}";
            else
                message = $"{message}; Warning: No Timer Found";
            message = $"{level.ToString()}: {message}";

            TracingService.Trace(message);
            if (writeTraceToAI)
                TraceLog.AppendLine(message);
        }
        public virtual bool LogToAppInsights()
        {
           // return McsSettings
           return ((bool?)_McsSettings[LogToAppInsightsField] ?? false) ;

           // return true;
            
            //return SettingsRecord.GetAttributeValue<bool?>(LogToAppInsightsField.ToLower()) ?? false;
        }

        public virtual bool LogToAppInsightsCheck()
        {
            return ((bool?)_McsSettings.getLogtoAppInsights ?? false);
        }

        public virtual bool CheckPluginSwitch(string pluginName)
        {
            switch (pluginName)
            {
                case "IDProofCreatePostStageRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.geteBenefitsLogToAI);
                    return ((bool?)_McsSettings.geteBenefitsLogToAI ?? false);
                case "UDOGetVeteranSnapshotRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getVeteranSnapshotLogToAI);
                    return ((bool?)_McsSettings.getVeteranSnapshotLogToAI ?? false);
                case "UDOGetAwardsRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getAwardsLogToAI);
                    return ((bool?)_McsSettings.getAwardsLogToAI ?? false);
                case "UDOGetClaimsRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getClaimsLogToAI);
                    return ((bool?)_McsSettings.getClaimsLogToAI ?? false);
                case "UDOGetContactUpdatesRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getContactsLogToAI);
                    return ((bool?)_McsSettings.getContactsLogToAI ?? false);
                case "GetFiduciaryExistsRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getFiduciaryLogToAI);
                    return ((bool?)_McsSettings.getFiduciaryLogToAI ?? false);
                case "GetFlashesRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getFlashLogToAI);
                    return ((bool?)_McsSettings.getFlashLogToAI ?? false);
                case "UDOInitiateLetterRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getLetterLogToAI);
                    return ((bool?)_McsSettings.getLetterLogToAI ?? false);
                case "UDOInitiateNotesRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getNotesLogToAI);
                    return ((bool?)_McsSettings.getNotesLogToAI ?? false);
                case "UDOGetRatingsRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getRatingsLogToAI);
                    return ((bool?)_McsSettings.getRatingsLogToAI ?? false);
                case "UDOGetVBMSeFolderRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getVBMSEFolderLogToAI);
                    return ((bool?)_McsSettings.getVBMSEFolderLogToAI ?? false);
                case "UDOGetVBMSeFolderDocumentsRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getVBMSEFolderLogToAI);
                    return ((bool?)_McsSettings.getVBMSEFolderLogToAI ?? false);
                case "IdProofPostCreateRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getIDProofLogToAI);
                    return ((bool?)_McsSettings.getIDProofLogToAI ?? false);
                case "InteractionCloseRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getInteractionLogToAI);
                    return ((bool?)_McsSettings.getInteractionLogToAI ?? false);
                case "DurationUpdateRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getInteractionLogToAI);
                    return ((bool?)_McsSettings.getInteractionLogToAI ?? false);
                case "RequestPostCreateRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getRequestLogToAI);
                    return ((bool?)_McsSettings.getRequestLogToAI ?? false);
                case "RequestPreCreateRuner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getRequestLogToAI);
                    return ((bool?)_McsSettings.getRequestLogToAI ?? false);
                case "RequestCreatePostRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getRequestLogToAI);
                    return ((bool?)_McsSettings.getRequestLogToAI ?? false);
                case "GenerateDocumentPostRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getSSRSLogToAI);
                    return ((bool?)_McsSettings.getSSRSLogToAI ?? false);
                case "CreateAnnotationPreRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getVBMSLogToAI);
                    return ((bool?)_McsSettings.getVBMSLogToAI ?? false);
                case "UDOPersonRetrieveMultiplePostStageRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getMVISearchLogToAI);
                    return ((bool?)_McsSettings.getMVISearchLogToAI ?? false);
                case "GenerateDocumentRunner":
                    TracingService.Trace("Value of PluginName is: " + pluginName);
                    TracingService.Trace("Value of the Setting is: " + _McsSettings.getGenerateDocumentLogToAI);
                    return ((bool?)_McsSettings.getGenerateDocumentLogToAI ?? false);
                case "UDOInitiateSRRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getServiceRequestLogToAI);
                    return ((bool?)_McsSettings.getServiceRequestLogToAI ?? false);
                case "PostLetterGenerationCreateUpdateRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getLetterLogToAI);
                    return ((bool?)_McsSettings.getLetterLogToAI ?? false);
                case "CreateNotesPreRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getNotesLogToAI);
                    return ((bool?)_McsSettings.getNotesLogToAI ?? false);
                case "DeleteNotesPreRunner":
                    Trace("Value of PluginName is: " + pluginName);
                    Trace("Value of the Setting is: " + _McsSettings.getNotesLogToAI);
                    return ((bool?)_McsSettings.getNotesLogToAI ?? false);
                default:
                    return false;
            }
        }

        public virtual string LogToAppInsightsField
        {
            get
            {
                return "udo_" + this.GetType().Name.ToLower();
            }
        }

        public void AddCustomDimension(string name, string value, int count = 1)
        {
            if (LogData?.CustomDimensions == null)
                SetupLogger();
            var newName = name + (count++).ToString();
            if (LogData.CustomDimensions.ContainsKey(name))
            {
                if (LogData.CustomDimensions.ContainsKey(newName))
                    AddCustomDimension(name, value, count);
                else
                    LogData.CustomDimensions.Add(newName, value);
            }
            else
                LogData.CustomDimensions.Add(name, value);
        }
        internal void SetupLogger()
        {
            //addition
            // VerboseTracing = McsSettings
            VerboseTracing = McsSettings.getVerbose;
            var aiKey = McsSettings.getAppInsightsKey;
            var aiUrl = McsSettings.getAppInsightsURL;

            //VerboseTracing = SettingsRecord.GetAttributeValue<bool>("udo_verbosetracing");
            //var aiKey = SettingsRecord.GetAttributeValue<string>("udo_appinsightskey");
           // var aiUrl = SettingsRecord.GetAttributeValue<string>("udo_appinsightsurl") ?? "https://dc.services.visualstudio.com/v2/track";
            //end addition
            this.LogData = new AppInsightsLogData
            {
                InstrumentationKey = aiKey,
                Url = aiUrl,
                OperationName = "udo-" + this.GetType().Name,
                OperationSyntheticSource = "UDO Plugins",
                CustomDimensions = new System.Collections.Generic.Dictionary<string, string>(),
                CustomMetrics = new System.Collections.Generic.Dictionary<string, double>(),
                CustomLogType = AiLogType.Request
            };
            LogData.CorrelationId = PluginExecutionContext.CorrelationId.ToString();
            LogData.UserId = PluginExecutionContext.UserId.ToString();
            LogData.Message = $"{LogData.OperationName} Plugin";
        }
        public void AddCustomMetric(string metricName, double metricValue, int count = 1)
        {
            if (LogData?.CustomMetrics == null)
                SetupLogger();
            var newName = metricName + (count++).ToString();
            if (LogData.CustomMetrics.ContainsKey(metricName))
            {
                if (LogData.CustomMetrics.ContainsKey(newName))
                    AddCustomMetric(metricName, metricValue, count);
                else
                    LogData.CustomMetrics.Add(newName, metricValue);
            }
            else
                LogData.CustomMetrics.Add(metricName, metricValue);
        }
        public void LogToAi()
        {
            var req = new OrganizationRequest("udo_ApplicationInsightsLogger");
            var json = SerializationHelper.Serialize(LogData);
            req.Parameters["SerializedLogData"] = json;
            OrganizationService.Execute(req);
            Trace("Wrote to App Insights: " + json);
        }

        public void LogException()
        {
            LogData.CustomLogType = AiLogType.Exception;
            var logger = new AppInsightsLogger(LogData.InstrumentationKey, LogData.Url, LogData.OperationName, LogData.UserId, LogData.CorrelationId, LogData.OperationSyntheticSource);
            var json = logger.GetExceptionJsonString(LogData.Exception, LogData.CustomDimensions, LogData.CustomMetrics);
            var errors = logger.SendToAi(json);
            foreach (var e in errors)
                Trace("Failures writing to AI: " + e.Message);
            if (errors != null && errors.Count > 0)
                Trace($"Failed to write to app insights following string: {json}");
            Trace(logger.GetExceptionJsonString(LogData.Exception, LogData.CustomDimensions, LogData.CustomMetrics));
            Trace("Write Exception to AI directly (via REST) due to OrganizationService failure blocking cascading action");
        }

        public void ExecuteFinally()
        {
            Trace($"Completed executing Plugin Logic", LogLevel.Information);
            try
            {
                // if (GetType().Name != "LogToAppInsightsLogic" && LogToAppInsights())
                // PluginExecutionContext.MessageName != 
                Trace("GetTypeName is: " + GetType().Name);
                bool pluginSwitchValue = CheckPluginSwitch(GetType().Name);
                Trace("Switch is: " + pluginSwitchValue);
                
                if (GetType().Name != "LogtoAppInsightsRunner" && LogToAppInsightsCheck())
                {
                    if (pluginSwitchValue == true)
                    {
                        Trace("Logging");
                        double convertToSeconds = 1000;
                        var pluginTime = ((double)Timer.ElapsedMilliseconds) / convertToSeconds;
                        AddCustomDimension("OrganizationName", PluginExecutionContext.OrganizationName);
                        AddCustomDimension("PluginError", PluginError.ToString());
                        AddCustomDimension("TraceLog", TraceLog.ToString().Trim());
                        AddCustomDimension("CorrelationId", PluginExecutionContext.CorrelationId.ToString());
                        AddCustomDimension("PluginInstance", PluginExecutionInstanceId.ToString());
                        AddCustomDimension("MessageName", PluginExecutionContext.MessageName);
                        AddCustomMetric("PluginTime", pluginTime);
                        AddCustomMetric("PluginDepth", PluginExecutionContext.Depth);
                        //add org name here
                        if (PluginExecutionContext.Mode == 1 || LogData.Exception == null) //Async or Sync with No exception to block action call
                            LogToAi();
                        else
                            LogException();
                    }
                    else
                    {
                        Trace("Not Logging to App Insights as the switch for this plugin is set to No.");
                    }                    
                }
                else
                    Trace("Not Logging to App Insights");
                   // Trace($"Not Logging to App Insights. Type: {GetType().Name}|LogToAI: {LogToAppInsights()}. Field: {LogToAppInsightsField}");
            }
            catch (Exception ex)
            {
                Trace($"Failed to log plugin data to app insights: {ex.ToString()}");
            }
            Timer.Stop();
        }
        public void ExecuteFinally(string searchType)
        {
            Trace($"Completed executing Plugin Logic", LogLevel.Information);
            try
            {
                // if (GetType().Name != "LogToAppInsightsLogic" && LogToAppInsights())
                // PluginExecutionContext.MessageName != 
                Trace("GetTypeName is: " + GetType().Name);
                string getTypeName = GetType().Name;
                bool pluginSwitchValue = CheckPluginSwitch(GetType().Name);
                Trace("Switch is: " + pluginSwitchValue);

                if (GetType().Name != "LogtoAppInsightsRunner" && LogToAppInsightsCheck())
                {
                    if ((pluginSwitchValue == true) && (getTypeName == "UDOPersonRetrieveMultiplePostStageRunner" ))
                    {
                        Trace("Logging");
                        double convertToSeconds = 1000;
                        var pluginTime = ((double)Timer.ElapsedMilliseconds) / convertToSeconds;
                        AddCustomDimension("OrganizationName", PluginExecutionContext.OrganizationName);
                        AddCustomDimension("PluginError", PluginError.ToString());
                        AddCustomDimension("TraceLog", TraceLog.ToString().Trim());
                        AddCustomDimension("CorrelationId", PluginExecutionContext.CorrelationId.ToString());
                        AddCustomDimension("PluginInstance", PluginExecutionInstanceId.ToString());
                        AddCustomDimension("MessageName", PluginExecutionContext.MessageName);
                        AddCustomDimension("MVISearchType", searchType);
                        AddCustomMetric("PluginTime", pluginTime);
                        AddCustomMetric("PluginDepth", PluginExecutionContext.Depth);
                        //add org name here
                        if (PluginExecutionContext.Mode == 1 || LogData.Exception == null) //Async or Sync with No exception to block action call
                            LogToAi();
                        else
                            LogException();
                    }
                    else
                    {
                        Trace("Not Logging to App Insights as the switch for this plugin is set to No.");
                    }
                }
                else
                    Trace("Not Logging to App Insights");
                // Trace($"Not Logging to App Insights. Type: {GetType().Name}|LogToAI: {LogToAppInsights()}. Field: {LogToAppInsightsField}");
            }
            catch (Exception ex)
            {
                Trace($"Failed to log plugin data to app insights: {ex.ToString()}");
            }
            Timer.Stop();
        }
    }
}
