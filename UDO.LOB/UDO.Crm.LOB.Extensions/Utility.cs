using System;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Configuration;
using UDO.LOB.Extensions.Logging;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace UDO.LOB.Core
{
    public enum WebApiType
    {
        LOB = 1, VEIS = 2
    }

    /// <summary>
    /// 
    /// Instructions on adding to your plugin,
    /// 1) Add a an existing Item to your project, navigate to this cs file, down in the bottom right corner where it says "Add", change to "Add as Link"
    /// 2) Add a Reference to System.Net.Http to the plugin project this is needed by HttpClient
    /// 3) Add a Reference to System.Xml to the plugin project this is needed by the Json Deserializer.
    /// 
    /// </summary>
    public class WebApiUtility
    {
        public enum LogLevel { Debug = 935950000, Info = 935950001, Warn = 935950002, Error = 935950003, Fatal = 935950004, Timing = 935950005 };

        public const string OneWayPassTest = "TestMessages#OneWayPassTest";
        public const string TwoWayPassTest = "TestMessages#TwoWayPassTest";
        public const string TwoMinuteTest = "TestMessages#TwoMinuteTest";
        public const string OneWayTimedTest = "TestMessages#OneWayTimedTest";

        public const string CreateCRMLogEntryRequest = "CRMe#CreateCRMLogEntryRequest";
        private const string _urlRestPath = "/api/vimt/{0}";
        private const string _urlParams = "?messageId={0}&messageType=text%2Fjson&isQueued=false";
        private const string SEND = "Send";
        private const string SEND_RECEIVE = "SendReceive";
        private const string _vimtExceptionMessage = "The Query of the Legacy system timed out, click on refresh to try again";
        private const int DEFAULT_TIMEOUT = 20;

        private const string ParentApplicationIdKey = "ParentApplicationId";
        private const string ClientApplicationIdKey = "ClientApplicationId";
        private const string ClientSecretKey = "ClientSecret";
        private const string TenantIdKey = "TenantId";

        private const string method = "WebApiUtility";

        /// <summary>
        /// Send a Request Message to VIMT.  Response is always null.
        /// VIMT Message Handler should be a RequestHandler.
        /// </summary>
        /// <param name="baseUri">REST URI to the VIMT Server</param>
        /// <param name="messageId">Request Message</param>
        /// <param name="requestObj">Request Object</param>
        /// <param name="logSettings">Log Settings</param>
        /// <returns>HttpResponseMessage: null</returns>
        public static void Send(Uri baseUri, string messageId, object requestObj, LogSettings logSettings)
        {
            HttpResponseMessage response = null;
            try
            {
                response = Send(baseUri, messageId, requestObj, logSettings, 0);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingMethod, ex);
                throw ex;
            }
            finally
            {
                if (response != null)
                {
                    response.Dispose();
                }
            }
        }

        /// <summary>
        /// Send a Request Message to VIMT.  
        /// getResposne specifies whether or not a response is expected.  The default is false.
        /// 
        /// If getResponse is true, then a VIMT RequestResponseHandler is expected
        /// If getResponse is false, then a VIMT RequestHandler is expected
        /// </summary>
        /// <param name="baseUri">REST URI to the VIMT Server</param>
        /// <param name="messageId">Request Message</param>
        /// <param name="requestObj">Request Object</param>
        /// <param name="logSettings">Log Settings</param>
        /// <param name="timeoutSeconds">Timeout in Seconds</param>
        /// <param name="getResponse">Get a Response (if true, uses SEND_RECEIVE, if false, uses SEND)</param>
        /// <returns>HttpResponseMessage: null if getResponse is false</returns>
        public static HttpResponseMessage Send(Uri baseUri, string messageId, object requestObj, LogSettings logSettings, int timeoutSeconds, bool getResponse = false)
        {
            HttpResponseMessage responseMessage = null;
            Uri uri = baseUri;

            try
            {
                Type requestType = requestObj.GetType();

                if ((System.Net.ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls12) == 0)
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                }

                using (HttpClient client = new HttpClient())
                using (MemoryStream memStream = ObjectToJSonStream(requestObj))
                using (StreamContent sc = new StreamContent(memStream))
                {
                    sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    //FOR USE ONLY WHEN CAPTURING JSON OBJECT FOR DOCUMENTATION
                    //var thingy = sc.ReadAsStringAsync();

                    client.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT * 1000);
                    if (timeoutSeconds > 0) client.Timeout = TimeSpan.FromSeconds(timeoutSeconds * 1000);

                    BypassCertificateError();

                    CRMAuthTokenConfiguration authTokenConfig = GenerateAuthHeader();

                    client.AddAuthHeader(authTokenConfig);

                    try
                    {
                        uri = new Uri(!uri.ToString().Contains("?") ? $"{uri.ToString()}?MessageId={ messageId }" : $"{uri.ToString()}&MessageId={ messageId }");
                        LogHelper.LogDebug(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingMethod, $"Utility.Send :: Invoking Uri: { uri.AbsoluteUri }", logSettings.Debug);

                        Task<HttpResponseMessage> task = client.PostAsync(uri, sc);
                        task.Wait();
                        responseMessage = task.Result;
                        responseMessage.EnsureSuccessStatusCode();
                    }
                    catch (Exception ex)
                    {
                        if (logSettings != null)
                        {

                            var errmsg = "";
                            if (!string.IsNullOrEmpty(uri.AbsoluteUri))
                            {
                                errmsg = uri.AbsoluteUri.ToString();
                            }
                            else
                            {
                                errmsg = messageId;
                            }

                            LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingMethod,
                                $"Error Invoking {uri.AbsoluteUri} \r\n  Exception: {WebApiUtility.StackTraceToString(ex)}");

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(WebException))
                {
                    if (((WebException)ex).Status == WebExceptionStatus.Timeout)
                    {
                        throw new VIMTTimeOutExeption(_vimtExceptionMessage);
                    }
                }

                LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingMethod, ex);
            }

            return responseMessage;
        }

        /// <summary>
        /// Send an Async Request Message to VIMT.  
        /// If there is a callback, the VIMT message handler should be a RequestResponseHandler.
        /// If there is no callback, the VIMT message handler should be RequestHandler. 
        /// </summary>
        /// <param name="baseUri">REST URI to the VIMT Server</param>
        /// <param name="messageId">Request Message</param>
        /// <param name="obj">Request Object</param>
        /// <param name="logSettings">Log Settings</param>
        /// <param name="callBack">Callback Method (if null, a SEND is used, if not null, a SEND_RECEIVE is used)</param>
        public static void SendAsync(Uri baseUri, string messageId, object obj, LogSettings logSettings, Action<HttpResponseMessage> callBack)
        {
            SendAsync(baseUri, messageId, obj, logSettings, callBack, 0);
        }

        /// <summary>
        /// Send an Async Request Message to VIMT.  
        /// If there is a callback, the VIMT message handler should be a RequestResponseHandler.
        /// If there is no callback, the VIMT message handler should be RequestHandler. 
        /// </summary>
        /// <param name="baseUri">REST URI to the VIMT Server</param>
        /// <param name="messageId">Request Message</param>
        /// <param name="obj">Request Object</param>
        /// <param name="logSettings">Log Settings</param>
        /// <param name="callBack">Callback Method (if null, a SEND is used, if not null, a SEND_RECEIVE is used)</param>
        /// <param name="timeoutSeconds">Timeout in Seconds</param>
        public static void SendAsync(Uri baseUri, string messageId, object obj, LogSettings logSettings, Action<HttpResponseMessage> callBack, int timeoutSeconds)
        {

            new Thread(() =>
            {
                try
                {
                    Thread.CurrentThread.IsBackground = false;
                    if (callBack != null)
                    {
                        LogHelper.LogDebug(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingMethod, $">>> Start Send: {baseUri}", logSettings.Debug);
                        using (HttpResponseMessage response = Send(baseUri, messageId, obj, logSettings, timeoutSeconds, true))
                        {
                            LogHelper.LogDebug(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingMethod, $"<<< End Send: {baseUri}", logSettings.Debug);
                            callBack(response);
                        }
                    }
                    else
                    {
                        HttpResponseMessage response = null;
                        try
                        {
                            LogHelper.LogDebug(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingMethod, $">>> Start SendAsync: {baseUri}", logSettings.Debug);
                            response = Send(baseUri, messageId, obj, logSettings, 0, false);
                            LogHelper.LogDebug(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingMethod, $"<<< End SendAsync: {baseUri}", logSettings.Debug);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingMethod, ex);
                            throw ex;
                        }
                        finally
                        {
                            if (response != null)
                            {
                                response.Dispose();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingMethod, ex);
                }
            }).Start();
        }


        public static void SendAsync(dynamic request, WebApiType webApiType)
        {
            Type requestType = request.GetType();   // Get type of request

            Uri webApiAbsUri = null;
            Uri webApiBaseUri = null;

            if (webApiType == WebApiType.LOB)
            {
                string apiRoute = LOBAPIDictionary.LOBAPIDictionary.RequestAPI[requestType.Name];
                webApiBaseUri = new Uri(LOBConfiguration.GetConfigurationSettings()[LOBConfiguration.LobApimUri]);
                webApiAbsUri = new Uri(webApiBaseUri.AbsoluteUri + apiRoute);
                LogHelper.LogInfo($"DEPENDENCY URI: {webApiAbsUri.AbsoluteUri}");
            }
            else if (webApiType == WebApiType.VEIS)
            {
                ApiCatalog apiCatalog = ApiCatalogManager.LoadApiSettings();

                if (apiCatalog != null && apiCatalog.ApiCollection != null)
                {
                    string apiRoute = apiCatalog[requestType.Name];
                    webApiBaseUri = new Uri(VEISConfiguration.GetConfigurationSettings()[VEISConfiguration.ECUri]);
                    webApiAbsUri = new Uri(webApiBaseUri.AbsoluteUri + apiRoute);
                    LogHelper.LogInfo($"DEPENDENCY URI: {webApiAbsUri.AbsoluteUri}");
                }
            }

            List<PropertyInfo> list = new List<PropertyInfo>(requestType.GetProperties());

            LogSettings logSettings = new LogSettings
            {
                Org = list.Contains(requestType.GetProperty("OrganizationName")) == true ? (string)requestType.GetProperty("OrganizationName").GetValue(request) : String.Empty,
                UserId = list.Contains(requestType.GetProperty("UserId")) == true ? (Guid)requestType.GetProperty("UserId").GetValue(request) : Guid.Empty,
                CallingMethod = list.Contains(requestType.GetProperty("CallingMethod")) == true ? (string)requestType.GetProperty("CallingMethod").GetValue(request) : method,
                ConfigFieldName = list.Contains(requestType.GetProperty("ConfigFieldName")) == true ? (string)requestType.GetProperty("ConfigFieldName").GetValue(request) : String.Empty,
                MessageId = list.Contains(requestType.GetProperty("MessageId")) == true ? (string)requestType.GetProperty("MessageId").GetValue(request) : String.Empty
            };

            SendAsync(webApiAbsUri, request.MessageId, request, logSettings, null, 0);

        }

        public static void Send(MessageBase request, WebApiType webApiType)
        {
            Uri webApiBaseUri = null;
            Type requestType = request.GetType();   // Get type of request
            if (webApiType == WebApiType.LOB)
            {
                webApiBaseUri = new Uri(LOBConfiguration.GetConfigurationSettings()[LOBConfiguration.LobApimUri]);
            }
            else if (webApiType == WebApiType.VEIS)
            {
                webApiBaseUri = new Uri(VEISConfiguration.GetConfigurationSettings()[VEISConfiguration.ECUri]);
            }

            var attr = request.GetType().Attributes;
            List<PropertyInfo> list = new List<PropertyInfo>(requestType.GetProperties());
            LogSettings logSettings = new LogSettings
            {
                Org = list.Contains(requestType.GetProperty("OrganizationName")) == true ? (string)requestType.GetProperty("OrganizationName").GetValue(request) : String.Empty,
                UserId = list.Contains(requestType.GetProperty("UserId")) == true ? (Guid)requestType.GetProperty("UserId").GetValue(request) : Guid.Empty,
                CallingMethod = list.Contains(requestType.GetProperty("CallingMethod")) == true ? (string)requestType.GetProperty("CallingMethod").GetValue(request) : method,
                ConfigFieldName = list.Contains(requestType.GetProperty("ConfigFieldName")) == true ? (string)requestType.GetProperty("ConfigFieldName").GetValue(request) : String.Empty,
                MessageId = list.Contains(requestType.GetProperty("MessageId")) == true ? (string)requestType.GetProperty("MessageId").GetValue(request) : String.Empty
            };

            HttpResponseMessage response = null;
            try
            {
                response = Send(webApiBaseUri, request.MessageId, request, logSettings, 0, false);
            }
            catch (Exception ex)
            {
                LogHelper.LogError((string)requestType.GetProperty("MessageId").GetValue(request), logSettings.Org, logSettings.UserId, logSettings.CallingMethod, ex);
                throw ex;
            }
            finally
            {
                if (response != null)
                {
                    response.Dispose();
                }
            }
        }

        public static T SendReceive<T>(dynamic request, WebApiType webApiType) where T : new()
        {
            Type requestType = request.GetType();   // Get type of request
            Type responseType = typeof(T);  // Get type of response

            Uri webApiAbsUri = null;
            Uri webApiBaseUri = null;

            List<PropertyInfo> list = new List<PropertyInfo>(requestType.GetProperties());

            LogSettings logSettings = new LogSettings
            {
                CallingAssembly = System.Reflection.Assembly.GetCallingAssembly().FullName.Split(',')[0],
                Org = list.Contains(requestType.GetProperty("OrganizationName")) == true ? (string)requestType.GetProperty("OrganizationName").GetValue(request) : String.Empty,
                UserId = list.Contains(requestType.GetProperty("UserId")) == true ? (Guid)requestType.GetProperty("UserId").GetValue(request) : Guid.Empty,
                CallingMethod = list.Contains(requestType.GetProperty("CallingMethod")) == true ? (string)requestType.GetProperty("CallingMethod").GetValue(request) : Extensions.MethodInfo.GetCallingMethod().Class,
                ConfigFieldName = list.Contains(requestType.GetProperty("ConfigFieldName")) == true ? (string)requestType.GetProperty("ConfigFieldName").GetValue(request) : String.Empty,
                MessageId = list.Contains(requestType.GetProperty("MessageId")) == true ? (string)requestType.GetProperty("MessageId").GetValue(request) : String.Empty,
                Debug = list.Contains(requestType.GetProperty("Debug")) == true ? (bool)requestType.GetProperty("Debug").GetValue(request) : false,
                logSoap = list.Contains(requestType.GetProperty("LogSoap")) == true ? (bool)requestType.GetProperty("LogSoap").GetValue(request) : false,
            };


            if (webApiType == WebApiType.LOB)
            {
                try
                {
                    string apiRoute = LOBAPIDictionary.LOBAPIDictionary.RequestAPI[requestType.Name];
                    webApiBaseUri = new Uri(LOBConfiguration.GetConfigurationSettings()[LOBConfiguration.LobApimUri]);
                    LogHelper.LogInfo($"LOGINFO: [{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff")}] DEPENDENCY URI: {apiRoute}");
                    // webApiAbsUri = new Uri(webApiBaseUri, apiRoute);
                    webApiAbsUri = new Uri(webApiBaseUri.AbsoluteUri + apiRoute);
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(request.MessageId, logSettings.Org, logSettings.UserId, $"ERROR :: Utility.Execute : Duplicate LOBDictionary Entry: {requestType.Name}: ", ex);
                    throw ex;
                }

            }
            else if (webApiType == WebApiType.VEIS)
            {
                ApiCatalog apiCatalog = ApiCatalogManager.LoadApiSettings();

                if (apiCatalog != null && apiCatalog.ApiCollection != null)
                {
                    string apiRoute = apiCatalog[requestType.Name];
                    webApiBaseUri = new Uri(VEISConfiguration.GetConfigurationSettings()[VEISConfiguration.ECUri]);
                    webApiAbsUri = new Uri(webApiBaseUri.AbsoluteUri + apiRoute);
                    LogHelper.LogInfo($"DEPENDENCY URI: {webApiAbsUri.AbsoluteUri}");
                }
            }

            return SendReceive<T>(webApiAbsUri, request.MessageId, request, logSettings, 0, null);

        }

        public static T SendReceiveNotes<T>(dynamic request, WebApiType webApiType)
        {
            Type requestType = request.GetType();   // Get type of request
            Type responseType = typeof(T);  // Get type of response

            Uri webApiAbsUri = null;
            Uri webApiBaseUri = null;

            List<PropertyInfo> list = new List<PropertyInfo>(requestType.GetProperties());

            LogSettings logSettings = new LogSettings
            {
                CallingAssembly = System.Reflection.Assembly.GetCallingAssembly().FullName.Split(',')[0],
                Org = list.Contains(requestType.GetProperty("OrganizationName")) == true ? (string)requestType.GetProperty("OrganizationName").GetValue(request) : String.Empty,
                UserId = list.Contains(requestType.GetProperty("UserId")) == true ? (Guid)requestType.GetProperty("UserId").GetValue(request) : Guid.Empty,
                CallingMethod = list.Contains(requestType.GetProperty("CallingMethod")) == true ? (string)requestType.GetProperty("CallingMethod").GetValue(request) : Extensions.MethodInfo.GetCallingMethod().Class,
                ConfigFieldName = list.Contains(requestType.GetProperty("ConfigFieldName")) == true ? (string)requestType.GetProperty("ConfigFieldName").GetValue(request) : String.Empty,
                MessageId = list.Contains(requestType.GetProperty("MessageId")) == true ? (string)requestType.GetProperty("MessageId").GetValue(request) : String.Empty,
                Debug = list.Contains(requestType.GetProperty("Debug")) == true ? (bool)requestType.GetProperty("Debug").GetValue(request) : false,
                logSoap = list.Contains(requestType.GetProperty("LogSoap")) == true ? (bool)requestType.GetProperty("LogSoap").GetValue(request) : false,
            };


            if (webApiType == WebApiType.LOB)
            {
                try
                {
                    string apiRoute = LOBAPIDictionary.LOBAPIDictionary.RequestAPI[requestType.Name];
                    webApiBaseUri = new Uri(LOBConfiguration.GetConfigurationSettings()[LOBConfiguration.LobApimUri]);
                    LogHelper.LogInfo($"API Route: {apiRoute}");
                    webApiAbsUri = new Uri(webApiBaseUri.AbsoluteUri + apiRoute);
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(request.MessageId, logSettings.Org, logSettings.UserId, $"ERROR :: Utility.Execute : Duplicate LOBDictionary Entry: {requestType.Name}: ", ex);
                    throw ex;
                }

            }
            else if (webApiType == WebApiType.VEIS)
            {
                ApiCatalog apiCatalog = ApiCatalogManager.LoadApiSettings();

                if (apiCatalog != null && apiCatalog.ApiCollection != null)
                {
                    string apiRoute = apiCatalog[requestType.Name];
                    webApiBaseUri = new Uri(VEISConfiguration.GetConfigurationSettings()[VEISConfiguration.ECUri]);
                    webApiAbsUri = new Uri(webApiBaseUri.AbsoluteUri + apiRoute);
                    LogHelper.LogInfo($"DEPENDENCY URI: {webApiAbsUri.AbsoluteUri}");
                }
            }

            return SendReceiveNotes<T>(webApiAbsUri, request.MessageId, request, logSettings, 0, null);

        }

        /// <summary>
        /// Send a Request Message to VIMT and get the response of type T.
        /// The VIMT Message Handler should be a RequestResponseHandler
        /// </summary>
        /// <typeparam name="T">Response Object Type</typeparam>
        /// <param name="baseUri">REST URI to the VIMT Server</param>
        /// <param name="messageId">Request Message</param>
        /// <param name="obj">Request Object</param>
        /// <param name="logSettings">Log Settings</param>
        public static T SendReceive<T>(Uri baseUri, string messageId, object obj, LogSettings logSettings) where T : new()
        {
            return SendReceive<T>(baseUri, messageId, obj, logSettings, 0, null);
        }

        public static T SendReceive<T>(Uri baseUri, string messageId, object obj, LogSettings logSettings, WebServiceDetails serviceDetails) where T : new()
        {
            return SendReceive<T>(baseUri, messageId, obj, logSettings, 0, serviceDetails);
        }

        public static T SendReceive<T>(Uri absUri, string messageId, object obj, LogSettings logSettings, int timeoutSeconds, WebServiceDetails serviceDetails = null) where T : new()
        {
            string message = string.Empty;
            string requestBody = string.Empty;
            T retObj;

            try
            {
                Uri uri = absUri;

                Type requestType = typeof(T);
                LogHelper.LogInfo($"Utility.SendReceive :: Request Type: {requestType}");

                if ((System.Net.ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls12) == 0)
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                }

                using (HttpClient client = new HttpClient())
                using (MemoryStream memStream = ObjectToJSonStream(obj))
                using (StreamContent sc = new StreamContent(memStream))
                {
                    sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    requestBody = sc.ReadAsStringAsync().Result;

                    client.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT * 5000);
                    if (timeoutSeconds > 0) client.Timeout = TimeSpan.FromSeconds(timeoutSeconds * 5000);

                    if (serviceDetails != null)
                    {
                        byte[] byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", serviceDetails.WSUserName, serviceDetails.Password));
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    BypassCertificateError();

                    CRMAuthTokenConfiguration authTokenConfig = GenerateAuthHeader();

                    client.AddAuthHeader(authTokenConfig);

                    try
                    {
                        var url = !uri.ToString().Contains("?") ? $"{uri.ToString()}?MessageId={ messageId }" : $"{uri.ToString()}&MessageId={ messageId }";
                        uri = new Uri(url);

                        LogHelper.LogSoap(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingAssembly, logSettings.CallingMethod, $"Utility.SendReceive :: Invoking Uri: { uri.AbsoluteUri } \r\n  RequestBody: {requestBody}", logSettings.logSoap);

                        Task<HttpResponseMessage> task = client.PostAsync(uri, sc);
                        task.Wait();

                        using (HttpResponseMessage response = task.Result)
                        {
                            if (response != null && response.Content != null)
                            {
                                message = response.Content.ReadAsStringAsync().Result;
                            }
                            response.EnsureSuccessStatusCode();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogInfo($"Client Response error: {ex.Message}");
                        if (logSettings != null)
                        {
                            var errorMessage = $"Error invoking Uri: {uri.AbsoluteUri} \r\n Request Body: {requestBody} \r\n Response Body: {message}";
                            LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, method, errorMessage);
                            LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, method, ex);
                        }
                    }

                    LogHelper.LogSoap(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingAssembly, logSettings.CallingMethod, $"Utility.SendReceive :: Invoked Uri: {uri.ToString()} ResponseBody: {message}", logSettings.logSoap);
                    retObj = DeserializeResponse<T>(message);
                    LogHelper.LogSoap(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingAssembly, logSettings.CallingMethod, $"Utility.SendReceive :: Invoked and Deserialized Uri: {uri.ToString()} {Environment.NewLine}ResponseType: {retObj.GetType().Name} {Environment.NewLine}ResponseBody: {message}", logSettings.logSoap);
                }
            }
            catch (Exception ex)
            {
                retObj = new T();
                retObj.GetType().GetProperty("ExceptionOccurred").SetValue(retObj, true);
                var errorMessage = $"Error invoking Uri: {absUri.AbsoluteUri} \r\n Request Body: {requestBody} \r\n Response: {message}";
                if (ex.GetType() == typeof(WebException))
                {
                    if (((WebException)ex).Status == WebExceptionStatus.Timeout)
                    {
                        LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, Extensions.MethodInfo.GetCallingMethod(true).Method,
                        $"ERROR :: Utility.SendReceive : MessageId: {messageId} VIMTTimeOutExeption:: {WebApiUtility.StackTraceToString(ex)} \r\n {errorMessage}");
                    }
                }
                else if (ex.GetType() == typeof(SerializationException))
                {

                    LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, Extensions.MethodInfo.GetCallingMethod(true).Method,
                        $"ERROR :: Utility.SendReceive : MessageId: {messageId} SerializationException:: {WebApiUtility.StackTraceToString(ex)} \r\n {errorMessage}");
                }
                else
                {
                    if (logSettings != null)
                    {
                        LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, method, ex);
                    }
                }

                retObj.GetType().GetProperty("ExceptionMessage").SetValue(retObj, $"ERROR :: Utility.SendReceive : MessageId: {messageId} ExceptionStack:: {WebApiUtility.StackTraceToString(ex)} \r\n {errorMessage}");
            }

            return retObj;
        }

        public static T SendReceiveNotes<T>(Uri absUri, string messageId, object obj, LogSettings logSettings, int timeoutSeconds, WebServiceDetails serviceDetails = null)
        {
            string message = string.Empty;

            T retObj;

            try
            {
                Uri uri = absUri;

                Type requestType = typeof(T);
                LogHelper.LogInfo($"Utility.SendReceive :: Request Type: {requestType}");

                if ((System.Net.ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls12) == 0)
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                }

                string requestBody = string.Empty;

                using (HttpClient client = new HttpClient())
                using (MemoryStream memStream = ObjectToJSonStream(obj))
                using (StreamContent sc = new StreamContent(memStream))
                {
                    sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    requestBody = sc.ReadAsStringAsync().Result;

                    client.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT * 5000);
                    if (timeoutSeconds > 0) client.Timeout = TimeSpan.FromSeconds(timeoutSeconds * 5000);

                    if (serviceDetails != null)
                    {
                        byte[] byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", serviceDetails.WSUserName, serviceDetails.Password));
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    BypassCertificateError();

                    CRMAuthTokenConfiguration authTokenConfig = GenerateAuthHeader();

                    LogHelper.LogInfo($"Utility.SendReceive :: CRM Token Config[ Client Application Id: {authTokenConfig.ClientApplicationId}, Client Secret: {authTokenConfig.ClientSecret}, Tenant Id: {authTokenConfig.TenantId}, Parent Application Id: {authTokenConfig.ParentApplicationId}, APIM Subscription Key: {authTokenConfig.ApimSubscriptionKey}]");

                    client.AddAuthHeader(authTokenConfig);

                    try
                    {
                        var url = !uri.ToString().Contains("?") ? $"{uri.ToString()}?MessageId={ messageId }" : $"{uri.ToString()}&MessageId={ messageId }";
                        uri = new Uri(url);

                        Task<HttpResponseMessage> task = client.PostAsync(uri, sc);
                        task.Wait();
                        using (HttpResponseMessage response = task.Result)
                        {
                            if (response != null && response.Content != null)
                            {
                                message = response.Content.ReadAsStringAsync().Result;
                            }
                            response.EnsureSuccessStatusCode();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogInfo($"Client Response error: {ex.Message}");
                        if (logSettings != null)
                        {
                            var errorMessage = $"Error invoking Uri: {uri.AbsoluteUri} \r\n Request Body: {requestBody}";
                            LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, method, errorMessage);
                            LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, method, ex);
                        }
                    }

                    retObj = DeserializeNotesResponse<T>(message);
                }
            }
            catch (Exception ex)
            {

                if (ex.GetType() == typeof(WebException))
                {
                    if (((WebException)ex).Status == WebExceptionStatus.Timeout)
                    {
                        throw new VIMTTimeOutExeption(_vimtExceptionMessage);
                    }
                }
                else if (ex.GetType() == typeof(SerializationException))
                {
                    LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, $"ERROR :: Utility.SendReceive : MessageId: {messageId} SerializationException:: ", ex);
                }
                else
                {
                    if (logSettings != null)
                    {
                        LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, method, ex);
                    }
                }

                retObj = default(T);
            }

            return retObj;
        }

        private static MemoryStream ObjectToJSonStream(object obj)
        {
            MemoryStream memStream = new MemoryStream();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());

            ser.WriteObject(memStream, obj);

            memStream.Position = 0;
            return memStream;
        }

        /// <summary>
        /// Format the URI with method and Message type.
        /// </summary>
        /// <param name="baseUri">REST URI to the VIMT Server</param>
        /// <param name="method">SEND or SEND_RECEIVE</param>
        /// <param name="messageId">Request Message</param>
        /// <returns>Destination Uri</returns>
        public static Uri FormatUri(Uri baseUri, string method, string messageId)
        {
            string urlRestPath = string.Format(_urlRestPath, method);
            string urlParams = string.Format(_urlParams, Uri.EscapeDataString(messageId));
            Uri relativeUri = new Uri(urlRestPath + urlParams, UriKind.Relative);

            Uri retUri = new Uri(baseUri.ToString() + relativeUri.ToString());
            return retUri;
        }


        /// <summary>
        /// Deserialize  Message to type T
        /// </summary>
        /// <typeparam name="T">Response Object Type</typeparam>
        /// <param name="message">String message to deserialize</param>
        /// <returns>Response Object</returns>
        public static T DeserializeResponse<T>(string message)
        {
            T retObj;
            UTF8Encoding enc = new UTF8Encoding();

            //REplace out the NewtonSoft specific dates with datacontract dates.

            string fixedDates = Regex.Replace(message, @"new Date\(([-+0-9]*)\)", "\"\\/Date($1+0000)\\/\"");

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(enc.GetBytes(fixedDates), 0, enc.GetByteCount(fixedDates));// fixedDates.Length);

                ms.Position = 0;

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                retObj = (T)ser.ReadObject(ms);
            }
            return retObj;
        }

        public static T DeserializeNotesResponse<T>(string message)
        {
            LogHelper.LogInfo(">> Entering the DeserializeNotesResponse.");
            T retObj;

            UTF8Encoding enc = new UTF8Encoding();

            DataContractJsonSerializerSettings microsoftDateFormatSettings = new DataContractJsonSerializerSettings();
            microsoftDateFormatSettings.DateTimeFormat = new DateTimeFormat("yyyy-MM-dd'T'HH:mm:ss+00:00");

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(enc.GetBytes(message), 0, enc.GetByteCount(message));

                ms.Position = 0;

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T), microsoftDateFormatSettings);
                retObj = (T)ser.ReadObject(ms);
            }

            LogHelper.LogInfo($"<< Exiting the DeserializeNotesResponse. With the following Object: {retObj}");
            return retObj;
        }

        /// <summary>
        /// Deserialize Base64 Message to type T
        /// </summary>
        /// <typeparam name="T">Response Object Type</typeparam>
        /// <param name="message">String message to deserialize</param>
        /// <returns>Response Object</returns>
        public static T DeserializeBase64Response<T>(string message)
        {
            T retObj;
            byte[] b = Convert.FromBase64String(message);
            UTF8Encoding enc = new UTF8Encoding();
            string mess = enc.GetString(b);

            //REplace out the NewtonSoft specific dates with datacontract dates.
            string fixedDates = Regex.Replace(mess, @"new Date\(([-+0-9]*)\)", "\"\\/Date($1+0000)\\/\"");

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(enc.GetBytes(fixedDates), 0, enc.GetByteCount(fixedDates));// fixedDates.Length);

                ms.Position = 0;

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                retObj = (T)ser.ReadObject(ms);
            }
            return retObj;
        }

        /// <summary>
        /// Convert object to MemoryStream in Json.
        /// </summary>
        /// <param name="obj">Object to convert to a json stream</param>
        /// <returns>MemoryStream</returns>
        private static Stream ObjectToJSonStream(object obj, Stream stream)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
            ser.WriteObject(stream, obj);
            return stream;
        }

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="baseUri">REST URI to the VIMT Server</param>
        /// <param name="org">CRM Organization</param>
        /// <param name="configFieldName">Not Used: Future On/Off switch</param>
        /// <param name="userId">Calling UserId</param>
        /// <param name="method">Child Calling Method or Sub Procedure</param>
        /// <param name="message">Error Message</param>
        /// <param name="callingMethod">Parent Calling Method</param>
        public static void LogError(Uri baseUri, string org, string configFieldName, Guid userId, string method, string message, string callingMethod = null)
        {
            string crme_method;
            if (!string.IsNullOrEmpty(callingMethod))
            {
                crme_method = callingMethod + ": " + method;
            }
            else
            {
                crme_method = method;
            }

            try
            {
                CreateCRMLogEntryRequest logRequestStart = new CreateCRMLogEntryRequest()
                {
                    MessageId = Guid.NewGuid().ToString(),
                    OrganizationName = org,
                    UserId = userId,
                    crme_Name = string.Format("Exception: {0}:{1}", "Error in ", method),
                    crme_ErrorMessage = message,
                    crme_Debug = false,
                    crme_GranularTiming = false,
                    crme_TransactionTiming = false,
                    crme_Method = crme_method,
                    crme_LogLevel = (int)LogLevel.Error,
                    crme_Sequence = 1,
                    NameofDebugSettingsField = configFieldName
                };

                CreateCRMLogEntryResponse logResponse = SendReceive<CreateCRMLogEntryResponse>(baseUri, WebApiUtility.CreateCRMLogEntryRequest, logRequestStart, null);
            }
            catch (Exception) { }

        }

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="baseUri">REST URI to the VIMT Server</param>
        /// <param name="org">CRM Organization</param>
        /// <param name="configFieldName">Not Used: Future On/Off switch</param>
        /// <param name="userId">Calling UserId</param>
        /// <param name="method">Child Calling Method or Sub Procedure</param>
        /// <param name="ex">Exception to log</param>
        /// <param name="callingMethod">Parent Calling Method</param>
        public static void LogError(Uri baseUri, string org, string configFieldName, Guid userId, string method, Exception ex, string callingMethod = null)
        {
            string stackTrace = StackTraceToString(ex);
            LogError(baseUri, org, configFieldName, userId, method, stackTrace, callingMethod);
        }

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="baseUri">REST URI to the VIMT Server</param>
        /// <param name="logSettings">LogSettings Object</param>
        /// <param name="method">Calling Method</param>
        /// <param name="message">Error Message</param>
        public static void LogError(Uri baseUri, LogSettings logSettings, string method, string message)
        {
            LogError(baseUri, logSettings.Org, logSettings.ConfigFieldName, logSettings.UserId, method, message, logSettings.CallingMethod);
        }

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="baseUri">REST URI to the VIMT Server</param>
        /// <param name="logSettings">LogSettings Object</param>
        /// <param name="method">Calling Method</param>
        /// <param name="ex">Exception to log</param>
        public static void LogError(Uri baseUri, LogSettings logSettings, string method, Exception ex)
        {
            LogError(baseUri, logSettings.Org, logSettings.ConfigFieldName, logSettings.UserId, method, ex, logSettings.CallingMethod);
        }

        /// <summary>
        /// Log Debug
        /// </summary>
        /// <param name="baseUri">REST URI to the VIMT Server</param>
        /// <param name="org">CRM Organization</param>
        /// <param name="configFieldName">Not Used: Future On/Off switch</param>
        /// <param name="userId">Calling UserId</param>
        /// <param name="method">Calling Method</param>
        /// <param name="message">Debug Message</param>
        public static void LogDebug(Uri baseUri, string org, string configFieldName, Guid userId, string method, string message, string callingMethod = null)
        {
            string crme_method;
            if (!string.IsNullOrEmpty(callingMethod))
            {
                crme_method = callingMethod + ": " + method;
            }
            else
            {
                crme_method = method;
            }

            try
            {
                CreateCRMLogEntryRequest logRequestStart = new CreateCRMLogEntryRequest()
                {
                    MessageId = Guid.NewGuid().ToString(),
                    OrganizationName = org,
                    UserId = userId,
                    crme_Name = string.Format("Debug: {0}", method),
                    crme_ErrorMessage = message,
                    crme_Debug = true,
                    crme_GranularTiming = false,
                    crme_TransactionTiming = false,
                    crme_Method = crme_method,
                    crme_LogLevel = (int)LogLevel.Debug,
                    crme_Sequence = 1,
                    NameofDebugSettingsField = configFieldName
                };

                CreateCRMLogEntryResponse logResponse = SendReceive<CreateCRMLogEntryResponse>(baseUri, WebApiUtility.CreateCRMLogEntryRequest, logRequestStart, null);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Log Debug
        /// </summary>
        /// <param name="baseUri">REST URI to the VIMT Server</param>
        /// <param name="logSettings">LogSettings Object</param>
        /// <param name="method">Calling Method</param>
        /// <param name="message">Message</param>
        public static void LogDebug(Uri baseUri, LogSettings logSettings, string method, string message)
        {
            LogDebug(baseUri, logSettings.Org, logSettings.ConfigFieldName, logSettings.UserId, method, message, logSettings.CallingMethod);
        }

        /// <summary>
        /// concatentate message and stack traces for exceptions and subsequent innerexceptions.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string StackTraceToString(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            BuildStackTrace(ex, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Recursive call to concatentate message and stack traces for exceptions and subsequent innerexceptions.
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="sb">StringBuilder to append to.</param>
        private static void BuildStackTrace(Exception ex, StringBuilder sb)
        {
            sb.AppendLine("***************************");
            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.GetType().FullName);
            sb.AppendLine(ex.StackTrace);

            if (ex.InnerException != null)
            {
                BuildStackTrace(ex.InnerException, sb);
            }
        }
        /// <summary>
        /// solution for exception
        /// System.Net.WebException: 
        /// The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel. ---> System.Security.Authentication.AuthenticationException: The remote certificate is invalid according to the validation procedure.
        /// </summary>
        public static void BypassCertificateError()
        {
            ServicePointManager.ServerCertificateValidationCallback +=

                delegate (
                    Object sender1,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
        }

        public static string ConvertByteStringToDocBody(string rawDocBody)
        {
            byte[] compressedDoc = Convert.FromBase64String(rawDocBody);
            byte[] docText = null;

            using (GZipStream stream = new GZipStream(new MemoryStream(compressedDoc), CompressionMode.Decompress))
            {
                int size = compressedDoc.Length;
                byte[] buffer = new byte[size];
                if (buffer != null)
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        docText = memory.ToArray();
                    }
                }

            }
            return Convert.ToBase64String(docText);
        }

        private static CRMAuthTokenConfiguration GenerateAuthHeader()
        {
            NameValueCollection configVals = VEISConfiguration.GetConfigurationSettings();

            string clientId = configVals[VEISConfiguration.OAuthClientId];
            string clientSecret = configVals[VEISConfiguration.OAuthClientSecret];

            string apiResourceID = configVals[VEISConfiguration.OAuthResourceId];
            string tenet = configVals[VEISConfiguration.AADTenent];
            string aadInstance = configVals[VEISConfiguration.AADInstance];
            string apimSubscriptionKey = configVals[VEISConfiguration.OcpApimSubscriptionKey];
            string apimSubscriptionKeyE = configVals[VEISConfiguration.OcpApimSubscriptionKeyE];
            string apimSubscriptionKeyS = configVals[VEISConfiguration.OcpApimSubscriptionKeyS];

            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException("ClientID", "#ERROR: Unable to retrieve Client Id from the VEIS Configuration OAuthClientId.");

            if (string.IsNullOrEmpty(clientSecret))
                throw new ArgumentNullException("clientSecret", "#ERROR: Unable to retrieve Client Secret from the Azure KeyVault.");

            if (string.IsNullOrEmpty(apiResourceID))
                throw new ArgumentNullException("APIResourceID", "#ERROR: Unable to retrieve Resource Id from the VEIS Configuration OAuthResourceId.");

            if (string.IsNullOrEmpty(tenet))
                throw new ArgumentNullException("Tenet", "#ERROR: Unable to retrieve Tenet from the VEIS Configuration AADTenent.");

            if (string.IsNullOrEmpty(aadInstance))
                throw new ArgumentNullException("AADInstance", "#ERROR: Unable to retrieve AAD Instance from the VEIS Configuration AADInstance.");

            if (string.IsNullOrEmpty(apimSubscriptionKey))
                throw new ArgumentNullException("OcpApimSubscriptionKey", "#ERROR: Unable to retrieve Ocp Apim Subscription Key from the VEIS Configuration OcpApimSubscriptionKey.");

            CRMAuthTokenConfiguration authTokenConfig = new CRMAuthTokenConfiguration
            {
                ClientApplicationId = clientId,
                ClientSecret = clientSecret,
                TenantId = tenet,
                ParentApplicationId = apiResourceID,
                ApimSubscriptionKey = apimSubscriptionKey
            };
            if (!string.IsNullOrEmpty(apimSubscriptionKeyE))
                authTokenConfig.ApimSubscriptionKeyE = apimSubscriptionKeyE;
            if (!string.IsNullOrEmpty(apimSubscriptionKeyS))
                authTokenConfig.ApimSubscriptionKeyS = apimSubscriptionKeyS;

            return authTokenConfig;
        }
    }

    #region Log Messages

    public class CreateCRMLogEntryRequest
    {
        public string MessageId { get; set; }

        public string OrganizationName { get; set; }

        public Guid UserId { get; set; }

        public int crme_Sequence { get; set; }

        public string crme_Name { get; set; }

        public string NameofDebugSettingsField { get; set; }

        public string crme_ErrorMessage { get; set; }

        public string crme_Method { get; set; }

        public bool crme_GranularTiming { get; set; }

        public bool crme_TransactionTiming { get; set; }

        public bool crme_Debug { get; set; }

        public int crme_LogLevel { get; set; }

        public Guid crme_RelatedParentId { get; set; }

        public string crme_RelatedParentEntityName { get; set; }

        public string crme_RelatedParentFieldName { get; set; }

        public string crme_RelatedWebMethodName { get; set; }

        public string crme_TimeStart { get; set; }

        public string crme_TimeEnd { get; set; }

        public Decimal crme_Duration { get; set; }
    }

    public class CreateCRMLogEntryResponse
    {
        public string MessageId { get; set; }
        public Guid crme_loggingId { get; set; }
    }



    #endregion

    #region VRMRest Excptions
    public class VIMTTimeOutExeption : System.Exception
    {
        public VIMTTimeOutExeption()
            : base()
        {
        }

        public VIMTTimeOutExeption(string message)
            : base(message)
        {
        }

        public VIMTTimeOutExeption(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    #endregion


    public class MessagePayload
    {
        public string MessageId { get; set; }
        public string MessageType { get; set; }
        public bool IsQueued { get; set; }
        public string Message { get; set; }
    }

    public class WebServiceDetails
    {
        public string WSUserName { get; set; }
        public string Password { get; set; }
        public string TargetURL { get; set; }
    }

}

