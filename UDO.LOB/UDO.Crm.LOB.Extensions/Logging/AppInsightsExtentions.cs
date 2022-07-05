using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDO.LOB.Extensions;


namespace UDO.LOB.Extensions.Logging
{
    /// <summary>
    /// Coomon Methods used across UDO to log events, traces and logs
    /// </summary>
    public static class AppInsightsExtentions
    {
        /// <summary>
        /// Use this method to log an AppInsight Event with the web service call duration
        /// </summary>
        /// <param name="name">User friendly string</param>
        /// <param name="traceLogger">Trace Logger</param>
        /// <param name="webApiName">Identify which LOB or service is being called.</param>
        /// <param name="sequence">The sequence in which this occurs/param>
        /// <param name="duration">The we service call duration.</param>
        /// <param name="otherMetrics">Optional parameter. Any other metrics that can be associated with this event which can be used in analytics</param>
        /// <param name="otherDimensions">Optional parameters. Any other properties associated with this call which can be used in analytics</param>
        public static void LogWebResponseDuration(this TraceLogger traceLogger,string name, string webApiName, string sequence ,double duration,IDictionary<string,double> otherMetrics=null, IDictionary<string,string> otherDimensions=null)
        {
            IDictionary<string, double> metrics = new Dictionary<string, double>() { { "WebServiceCallDuration", duration } };
            IDictionary<string, string> properties = new Dictionary<string, string>() { { "WebServiceName", webApiName }, { "OperationType", "WebServiceCall" } };
            if (otherDimensions != null && otherDimensions.Count() > 0)
            {
                foreach (var item in otherDimensions)
                {
                    properties.Add(item.Key, item.Value);
                }
            }
            if (otherDimensions != null && otherDimensions.Count() > 0)
            {
                foreach (var item in otherMetrics)
                {
                    metrics.Add(item.Key, item.Value);
                }
            }
            traceLogger.LogEvent(name, sequence, metrics, properties);               
        }
    }
}
