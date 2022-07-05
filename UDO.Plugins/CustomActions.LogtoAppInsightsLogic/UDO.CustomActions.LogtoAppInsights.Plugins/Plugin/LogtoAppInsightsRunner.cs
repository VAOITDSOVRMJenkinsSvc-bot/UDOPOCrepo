using MCSPlugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VA.AppInsights;
using VA.UDO.Plugins.Helpers;

namespace UDO.CustomActions.LogtoAppInsights.Plugins.Plugin
{
    public class LogtoAppInsightsRunner : UDOActionRunner
    {
        public LogtoAppInsightsRunner(IServiceProvider serviceProvider)
           : base(serviceProvider)
        {
            TracingService.Trace("Base");

            _logTimerField = "udo_contactlogtimer";
            _logSoapField = "udo_contactlogsoap";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_contacttimeout";
            _debugField = "none";
            _validEntities = new string[] { "crme_dependentmaintenance" };
        }

        public override void DoAction()
        {
            TracingService.Trace("Entered Do Action");
            _method = "DoAction";
            GetSettingValues();
            if (!PluginExecutionContext.InputParameters.ContainsKey("SerializedLogData"))
                throw new ArgumentNullException("SerializedLogData");

            var json = PluginExecutionContext.InputParameters["SerializedLogData"].ToString();

            Trace($"Writing to App Insights: {json}");
            TracingService.Trace("Writing to App Insights");
            var logData = SerializationHelper.DeserializeJson<AppInsightsLogData>(json);
            if (logData == null)
                throw new ArgumentNullException("Log Data - Deserialized");

            if (string.IsNullOrEmpty(logData.OperationName))
                throw new ArgumentNullException("OperationName");

            if (string.IsNullOrEmpty(logData.CorrelationId))
                throw new ArgumentNullException("CorrelationId");
            if (logData.SeverityLevel < 0 || logData.SeverityLevel > 4)
                logData.SeverityLevel = 0;

            if (logData.Message == null)
                logData.Message = logData.OperationName;

            if (logData.UserId != null)
            {
                var user = OrganizationService.Retrieve("systemuser", new Guid(logData.UserId), new Microsoft.Xrm.Sdk.Query.ColumnSet("fullname"));
                var name = user.GetAttributeValue<string>("fullname");
                if (!logData.CustomDimensions.ContainsKey("UserName"))
                    logData.CustomDimensions.Add("UserName", name);
                else if (!logData.CustomDimensions.ContainsKey("AgentName"))
                    logData.CustomDimensions.Add("AgentName", name);
            }

            Trace("Passed Input Null Checks");
            TracingService.Trace("Passed Input Null Checks !!");

            var logger = new AppInsightsLogger(logData.InstrumentationKey, logData.Url, logData.OperationName, logData.UserId, logData.CorrelationId, logData.OperationSyntheticSource);

            Trace("Initialized AI Logger");

            if (logData.Exception != null)
                WriteToAi(logData, logger, AiLogType.Exception);

            TracingService.Trace("Calling Write to AI.");
            WriteToAi(logData, logger, logData.CustomLogType);

        }
        public override bool LogToAppInsights()
        {
            return false;
        }
        private List<Error> WriteToAi(AppInsightsLogData logData, AppInsightsLogger logger, AiLogType type)
        {
            var errors = new List<Error>();
            var json = string.Empty;
            switch (type)
            {
                case AiLogType.Exception:
                    Trace("Writing Exception to App insights");
                    json = logger.GetExceptionJsonString(logData.Exception, logData.CustomDimensions, logData.CustomMetrics);
                    break;
                case AiLogType.Request:
                    Trace("Writing Request to App insights");
                    var hasDuration = logData.CustomMetrics.TryGetValue("PluginTime", out double duration);
                    json = logger.GetRequestJsonString(logData.CorrelationId, duration, logData.Message, PluginExecutionContext.OrganizationName,
                        logData.Exception == null, logData.CustomDimensions, logData.CustomMetrics, logData.HttpStatusCode);
                    break;
                case AiLogType.Trace:
                    Trace("Writing Trace to App insights");
                    json = logger.GetTraceJsonString(logData.Message, TraceSeverity.Verbose, logData.CustomDimensions);
                    break;
                case AiLogType.Event:
                default:
                    Trace("Writing Event to App insights");
                    json = logger.GetEventJsonString(logData.Message, logData.CustomDimensions, logData.CustomMetrics);
                    break;

            }
            Trace("JSON for AI: " + json);
            errors = logger.SendToAi(json);
            if (errors == null || errors.Count == 0)
                Trace($"Successful write of {type.ToString()} to App Insights");
            else
            {
                var errorString = string.Empty;
                errors.ForEach(e => errorString += $"|{e.Message}");
                Trace($"Failed {errors.Count} times. {errorString}");

                if (logData.CustomDimensions.ContainsKey("TraceLog"))
                {
                    logData.CustomDimensions.Remove("TraceLog");
                    WriteToAi(logData, logger, logData.CustomLogType);
                }
                else
                {
                    var dims = new Dictionary<string, string>();
                    dims.Add("FailureString", errorString);
                    var metrics = new Dictionary<string, double>();
                    metrics.Add("AiFailures", errors.Count);
                    var ex = new AIException(new Exception("Failed to write to App Insights: " + errorString), ExceptionSeverity.Warning);
                    var backupErrors = logger.WriteException(ex, dims, metrics);
                    if (backupErrors == null || backupErrors.Count == 0)
                        Trace($"Successful write of {type.ToString()} to App Insights");
                    else
                        Trace($"Failed to write backupException to AppInsights {backupErrors.FirstOrDefault()?.Message}");
                }
            }
            return errors;
        }
    }
}
