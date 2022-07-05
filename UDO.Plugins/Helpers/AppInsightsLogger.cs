using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace VA.AppInsights
{
    public class AppInsightsLogger
    {
        public readonly string _instrumentationKey;
        private readonly string _loggingEndpoint;
        private static HttpClient _httpClient;
        private readonly string _operationName;
        private readonly string _userId;
        private readonly string _operationId;
        private readonly string _operationSyntheticSource;

        public AppInsightsLogger(string instrumentationKey, string endPoint, string operationName, string userId, string operationId, string syntheticSource)
        {
            _instrumentationKey = instrumentationKey;
            _loggingEndpoint = endPoint;
            _httpClient = HttpHelper.GetHttpClient();
            _operationName = operationName;
            _operationId = operationId;
            _userId = userId;
            _operationSyntheticSource = syntheticSource;
        }

        ////Data Model for Requests: ID, Duration, Message, Source, Response Code, Success, Custom Properties (Dimensions), Custom Measurements
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="duration"></param>
        ///// <param name="message"></param>
        ///// <param name="source"></param>
        ///// <param name="success"></param>
        ///// <param name="customDimensions"></param>
        ///// <param name="customMeasurements"></param>
        ///// <returns></returns>
        //public List<Error> WriteRequest(string id, double durationSeconds, string message, string source, bool success, 
        //    Dictionary<string, string> customDimensions, Dictionary<string, double> customMeasurements, string responseCode = "")
        //{
        //    var aiTrace = new Request(id, durationSeconds, message, source, success, customDimensions, customMeasurements, responseCode);

        //    string json = GetRequestJsonString(aiTrace);

        //    return SendToAi(json);
        //}

        //public List<Error> WriteTrace(string message, TraceSeverity aiTraceSeverity, Dictionary<string,string> dimensions)
        //{
        //    var aiTrace = new Trace(message, aiTraceSeverity, dimensions);

        //    string json = GetTraceJsonString(aiTrace);

        //    return SendToAi(json);
        //}

        //public List<Error> WriteEvent(string message, Dictionary<string, string> customDimensions, Dictionary<string, double> customMeasures)
        //{
        //    var aiTrace = new Event(message, customDimensions, customMeasures);

        //    string json = GetEventJsonString(aiTrace);

        //    return SendToAi(json);
        //}

        public List<Error> WriteException(AIException exception, Dictionary<string, string> customDimensions, Dictionary<string, double> customMetrics)
        {
            string json = GetExceptionJsonString(exception, customDimensions, customMetrics);

            return SendToAi(json);
        }

        internal List<Error> SendToAi(string json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/x-json-stream");
            var response = _httpClient.PostAsync(_loggingEndpoint, content).ConfigureAwait(false).GetAwaiter().GetResult();
            
            var contentResponse = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = SerializationHelper.Deserialize<Response>(contentResponse);

            if (!response.IsSuccessStatusCode || result.ItemsAccepted == 0 || result.Errors.Any())
            {
                return result.Errors;
            }

            return result.Errors;
        }

        internal string GetRequestJsonString(string id, double durationSeconds, string message, string source, bool success,
            Dictionary<string, string> customDimensions, Dictionary<string, double> customMeasurements, string responseCode = "")
        {
            var aiTrace = new Request(id, durationSeconds, message, source, success, customDimensions, customMeasurements, responseCode);

            return GetRequestJsonString(aiTrace);
        }

        internal string GetRequestJsonString(Request aiTrace)
        {
            var logRequest = new LogRequest
            {
                Name = $"Microsoft.ApplicationInsights.{_instrumentationKey}.Request",
                Time = DateTime.UtcNow.ToString("O"),
                InstrumentationKey = _instrumentationKey,
                Tags = new Tags
                {
                    AuthenticatedUserId = _userId,
                    OperationName = _operationName,
                    RoleInstance = "D365 Plugin",
                    OperationId = _operationId
                },
                Data = new Data
                {
                    BaseType = "RequestData",
                    BaseData = aiTrace
                }
            };

            var json = SerializationHelper.Serialize(logRequest);

            var json2 = SerializationHelper.ManageBaseData(json, aiTrace.CustomDimensions);
            var json3 = SerializationHelper.ManageBaseDataMeasures(json2, aiTrace.CustomMeasurements);

            return json3;
        }

        internal string GetEventJsonString(string message, Dictionary<string, string> customDimensions, Dictionary<string, double> customMeasures)
        {
            var aiTrace = new Event(message, customDimensions, customMeasures);

            return GetEventJsonString(aiTrace);
        }


        internal string GetEventJsonString(Event aiTrace)
        {
            var logRequest = new LogRequest
            {
                Name = $"Microsoft.ApplicationInsights.{_instrumentationKey}.Event",
                Time = DateTime.UtcNow.ToString("O"),
                InstrumentationKey = _instrumentationKey,
                Tags = new Tags
                {
                    AuthenticatedUserId = _userId,
                    OperationName = _operationName,
                    RoleInstance = "D365 Plugin",
                    OperationId = _operationId
                },
                Data = new Data
                {
                    BaseType = "EventData",
                    BaseData = aiTrace
                }
            };
            var json = SerializationHelper.Serialize(logRequest);

            var json2 = SerializationHelper.ManageBaseData(json, aiTrace.CustomDimensions);
            var json3 = SerializationHelper.ManageBaseDataMeasures(json2, aiTrace.CustomMeasurements);

            return json3;
        }

        internal string GetTraceJsonString(string message, TraceSeverity aiTraceSeverity, Dictionary<string, string> dimensions)
        {
            var aiTrace = new Trace(message, aiTraceSeverity, dimensions);

            return GetTraceJsonString(aiTrace);
        }

        internal string GetTraceJsonString(Trace aiTrace)
        {
            var logRequest = new LogRequest
            {
                Name = $"Microsoft.ApplicationInsights.{_instrumentationKey}.Message",
                Time = DateTime.UtcNow.ToString("O"),
                InstrumentationKey = _instrumentationKey,
                Tags = new Tags
                {
                    RoleInstance = "Default",
                    OperationName = _operationName,
                    AuthenticatedUserId = _userId,
                    OperationId = _operationId
                },
                Data = new Data
                {
                    BaseType = "MessageData",
                    BaseData = aiTrace
                }
            };

            var json = SerializationHelper.Serialize(logRequest);
            var json2 = SerializationHelper.ManageBaseData(json, aiTrace.CustomDimensions);

            return json2;
        }

        public string GetExceptionJsonString(AIException aiException, Dictionary<string, string> customDimensions, Dictionary<string, double> customMetrics)
        {
            var logRequest = new LogRequest
            {
                Name = $"Microsoft.ApplicationInsights.{_instrumentationKey}.Exception",
                Time = DateTime.UtcNow.ToString("O"),
                InstrumentationKey = _instrumentationKey,
                Tags = new Tags
                {
                    RoleInstance = "Default",
                    OperationName = _operationName,
                    AuthenticatedUserId = _userId
                },
                Data = new Data
                {
                    BaseType = "ExceptionData",
                    BaseData = new BaseData
                    {
                        Exceptions = new List<AIException> { aiException }
                    }
                }
            };

            var json = SerializationHelper.Serialize(logRequest);
            var json2 = SerializationHelper.ManageBaseData(json, customDimensions);
            var json3 = SerializationHelper.ManageBaseDataMeasures(json2, customMetrics);

            return json3;
        }
    }
}