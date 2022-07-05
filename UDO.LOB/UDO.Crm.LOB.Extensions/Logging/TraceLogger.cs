using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using UDO.LOB.Core;

namespace UDO.LOB.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public class TraceLogger
    {
        private Stopwatch sw;
        private TelemetryClient tClient;
        private string name;

        public TraceLogger(string name, MessageBase message)
        {
            sw = new Stopwatch();
            sw.Start();
            tClient = new TelemetryClient();
            this.name = name;
            LoadCallContext(message);
            LogTrace($"Initialized - {name}", "0");
        }

        /// <summary>
        /// Loads a set of attributes used to search AppInsights for relevant OOB/custom AppInsights Messages
        /// </summary>
        /// <param name="message">Base message</param>
        private void LoadCallContext(MessageBase message)
        {
            if (message != null)
            {

                #region usehttpcontext
                if (message.DiagnosticsContext != null && HttpContext.Current != null && HttpContext.Current.Items != null)
                {
                    if (!HttpContext.Current.Items.Contains("TelemetryName"))
                    {
                        HttpContext.Current.Items.Add("TelemetryName", name);
                    }
                    if (!HttpContext.Current.Items.Contains("InteractionId"))
                    {
                        HttpContext.Current.Items.Add("InteractionId", message.DiagnosticsContext.InteractionId != null ? message.DiagnosticsContext.InteractionId : Guid.Empty);
                    }
                    if (!HttpContext.Current.Items.Contains("VeteranId"))
                    {
                        HttpContext.Current.Items.Add("VeteranId", message.DiagnosticsContext.VeteranId != null ? message.DiagnosticsContext.VeteranId : Guid.Empty);
                    }
                    if (!HttpContext.Current.Items.Contains("RequestId"))
                    {
                        HttpContext.Current.Items.Add("RequestId", message.DiagnosticsContext.RequestId != null ? message.DiagnosticsContext.RequestId : Guid.Empty);
                    }
                    if (!HttpContext.Current.Items.Contains("AgentId"))
                    {
                        HttpContext.Current.Items.Add("AgentId", message.DiagnosticsContext.AgentId != null ? message.DiagnosticsContext.AgentId : Guid.Empty);
                    }
                    if (!HttpContext.Current.Items.Contains("CRMOrganizationName"))
                    {
                        HttpContext.Current.Items.Add("CRMOrganizationName", string.IsNullOrWhiteSpace(message.DiagnosticsContext.OrganizationName) ? "<NA>" : message.DiagnosticsContext.OrganizationName);
                    }
                    if (!HttpContext.Current.Items.Contains("StationNumber"))
                    {
                        HttpContext.Current.Items.Add("StationNumber", string.IsNullOrWhiteSpace(message.DiagnosticsContext.StationNumber) ? "<NA>" : message.DiagnosticsContext.StationNumber);
                    }
                    if (!HttpContext.Current.Items.Contains("MessageId"))
                    {
                        HttpContext.Current.Items.Add("MessageId", message.MessageId);
                    }
                    if (!HttpContext.Current.Items.Contains("IdProofId"))
                    {
                        HttpContext.Current.Items.Add("IdProofId", message.DiagnosticsContext.IdProof);
                    }
                }
                else
                {
                    LogEvent($"No Diagnostic Context present in {name}", "100");
                }

                #endregion
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        private T TelemetryDataFactory<T>() where T : ITelemetry, new()
        {
            T t = new T();
            // Add Metrics
            var supportMetrics = t as ISupportMetrics;
            if (supportMetrics != null)
            {
                supportMetrics.Metrics.Add("ElapsedTime", sw.Elapsed.TotalSeconds);
            }            
            return t;
        }

        private void LoadMetricsDimensions<T>(T telemetry, IDictionary<string, double> eventMetrics, IDictionary<string, string> eventProperties) where T : ITelemetry, new()
        {
            var metricsTelemetry = telemetry as ISupportMetrics;
            if (metricsTelemetry != null && eventMetrics != null && eventMetrics.Count > 0)
            {
                //Add  Metrics here
                foreach (var item in eventMetrics)
                {
                    metricsTelemetry.Metrics.Add(item.Key, item.Value);
                }
            }
            var propertyTelemetry = telemetry as ISupportProperties;
            if (propertyTelemetry != null && eventProperties != null && eventProperties.Count > 0)
            {
                //Add  Properties here here
                foreach (var item in eventProperties)
                {
                    propertyTelemetry.Properties.Add(item.Key, item.Value);
                }
            }
        }

        public void LogEvent(string eventName, string eventSequence, IDictionary<string, double> eventMetrics = null, IDictionary<string, string> eventProperties = null)
        {
            EventTelemetry eventTelemetry = TelemetryDataFactory<EventTelemetry>();
            eventTelemetry.Name = eventName;
            eventTelemetry.Sequence = eventSequence;
            LoadMetricsDimensions(eventTelemetry, eventMetrics, eventProperties);
            tClient.TrackEvent(eventTelemetry);
        }
        public void LogTrace(string message, string traceSequence, SeverityLevel level = SeverityLevel.Information, IDictionary<string, double> eventMetrics = null, IDictionary<string, string> eventProperties = null)
        {
            TraceTelemetry traceTelemetry = TelemetryDataFactory<TraceTelemetry>();
            traceTelemetry.Message = message;
            traceTelemetry.Sequence = traceSequence;
            traceTelemetry.SeverityLevel = level;
            LoadMetricsDimensions(traceTelemetry, eventMetrics, eventProperties);
            tClient.TrackTrace(traceTelemetry);
        }
        public void LogException(Exception ex, string exceptionSequence, IDictionary<string, double> eventMetrics = null, IDictionary<string, string> eventProperties = null)
        {
            ExceptionTelemetry exceptionTelemetry = TelemetryDataFactory<ExceptionTelemetry>();
            exceptionTelemetry.Exception = ex;
            exceptionTelemetry.Sequence = exceptionSequence;
            LoadMetricsDimensions(exceptionTelemetry, eventMetrics, eventProperties);
            tClient.TrackException(exceptionTelemetry);
        }
    }

    /// <summary>
    /// This class loads context information like VeteranId, CRM orgname, InteractionId into all the traces pushed from the system.
    /// </summary>
    public class ContextLoadTelemetryInitializer : ITelemetryInitializer
    {      
        public void Initialize(ITelemetry telemetry)
        {
            //Sometimes traces are sent to App Insights without an actual request. In these scenarios skip the method.
            if (HttpContext.Current == null)
                return;

            #region Loadfromhttpcontext
            //telemetry.Context.Operation.Name= HttpContext.Current.Items["TelemetryName"].ToString();
            //if (Activity.Current != null)
            //{
            //    telemetry.Context.Operation.ParentId = Activity.Current.Id;
            //}

           
            var interactionId = HttpContext.Current.Items.Contains("InteractionId") ? HttpContext.Current.Items["InteractionId"].ToString() : string.Empty;
            var requestId = HttpContext.Current.Items.Contains("RequestId") ? HttpContext.Current.Items["RequestId"].ToString() : string.Empty;
            var agentId = HttpContext.Current.Items.Contains("AgentId") ? HttpContext.Current.Items["AgentId"].ToString() : string.Empty;
            var stationNumber = HttpContext.Current.Items.Contains("StationNumber") ? HttpContext.Current.Items["StationNumber"].ToString() : string.Empty;
            var veteranId = HttpContext.Current.Items.Contains("VeteranId") ? HttpContext.Current.Items["VeteranId"].ToString() : string.Empty;
            var organizationName = HttpContext.Current.Items.Contains("CRMOrganizationName") ? HttpContext.Current.Items["CRMOrganizationName"].ToString() : string.Empty;
            var messageId = HttpContext.Current.Items.Contains("MessageId") && HttpContext.Current.Items["MessageId"] != null ? HttpContext.Current.Items["MessageId"].ToString() : string.Empty;
            var messageTrigger = HttpContext.Current.Items.Contains("MessageTrigger") ? HttpContext.Current.Items["MessageTrrigger"].ToString() : string.Empty;
            var idProofId = HttpContext.Current.Items.Contains("IdProofId") ? HttpContext.Current.Items["IdProofId"].ToString() : string.Empty;
            #endregion

            var propertyTelemetry = telemetry as ISupportProperties;
            if (propertyTelemetry != null)
            {
                if (!string.IsNullOrWhiteSpace(interactionId) && !propertyTelemetry.Properties.Keys.Contains("InteractionId"))
                {
                    propertyTelemetry.Properties.Add("InteractionId", interactionId.ToString());
                }
                if (!string.IsNullOrWhiteSpace(veteranId) && !propertyTelemetry.Properties.Keys.Contains("VeteranId"))
                {
                    propertyTelemetry.Properties.Add("VeteranId", veteranId.ToString());
                }
                if (!string.IsNullOrWhiteSpace(requestId) && !propertyTelemetry.Properties.Keys.Contains("RequestId"))
                {
                    propertyTelemetry.Properties.Add("RequestId", requestId.ToString());
                }
                if (!string.IsNullOrWhiteSpace(agentId) && !propertyTelemetry.Properties.Keys.Contains("AgentId"))
                {
                    propertyTelemetry.Properties.Add("AgentId", agentId.ToString());
                }
                if (!string.IsNullOrWhiteSpace(stationNumber) && !propertyTelemetry.Properties.Keys.Contains("StationNumber"))
                {
                    propertyTelemetry.Properties.Add("StationNumber", stationNumber.ToString());
                }
                if (!string.IsNullOrWhiteSpace(organizationName) && !propertyTelemetry.Properties.Keys.Contains("CRMOrganizationName"))
                {
                    propertyTelemetry.Properties.Add("CRMOrganizationName", organizationName.ToString());
                }
                if (!string.IsNullOrWhiteSpace(messageId) && !propertyTelemetry.Properties.Keys.Contains("MessageId"))
                {
                    propertyTelemetry.Properties.Add("MessageId", messageId.ToString());
                }
                if (!string.IsNullOrWhiteSpace(idProofId) && !propertyTelemetry.Properties.Keys.Contains("IdProofId"))
                {
                    propertyTelemetry.Properties.Add("IdProofId", idProofId.ToString());
                }
            }
        }
    }

    public class TelemetryFilter : ITelemetryProcessor
    {
        public void Process(ITelemetry item)
        {
            
        }
    }
}