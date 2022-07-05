using MCSPlugins;
using Microsoft.Xrm.Sdk;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Runtime.Serialization;

namespace VRMRest
{

	/// <summary>
	/// 
	/// Instructions on adding to your plugin,
	/// 1) Add a an existing Item to your project, navigate to this cs file, down in the bottom right corner where it says "Add", change to "Add as Link"
	/// 2) Add a Reference to System.Net.Http to the plugin project this is needed by HttpClient
	/// 3) Add a Reference to System.Xml to the plugin project this is needed by the Json Deserializer.
	/// 
	/// </summary>
	public class Utility
	{
		public enum LogLevel { Debug = 935950000, Info = 935950001, Warn = 935950002, Error = 935950003, Fatal = 935950004, Timing = 935950005 };

		public const string OneWayPassTest = "TestMessages#OneWayPassTest";
		public const string TwoWayPassTest = "TestMessages#TwoWayPassTest";
		public const string TwoMinuteTest = "TestMessages#TwoMinuteTest";
		public const string OneWayTimedTest = "TestMessages#OneWayTimedTest";

		public const string CreateCRMLogEntryRequest = "CRMe#CreateCRMLogEntryRequest";
		private const string _urlRestPath = "/Servicebus/Rest/{0}";
		private const string _urlParams = "?messageId={0}&messageType=text%2Fjson&isQueued=false";
		private const string SEND = "Send";
		private const string SEND_RECEIVE = "SendReceive";
		private const string _vimtExceptionMessage = "The Query of the Legacy system timed out, click on refresh to try again";
		private const int DEFAULT_TIMEOUT = 60;

		/// <summary>
		/// Send a Request Message to VIMT.  Response is always null.
		/// VIMT Message Handler should be a RequestHandler.
		/// </summary>
		/// <param name="baseUri">REST URI to the VIMT Server</param>
		/// <param name="messageId">Request Message</param>
		/// <param name="requestObj">Request Object</param>
		/// <param name="logSettings">Log Settings</param>
		/// <returns>HttpResponseMessage: null</returns>
		public static HttpResponseMessage Send(Uri baseUri, string messageId, object requestObj, LogSettings logSettings)
		{
			return Send(baseUri, messageId, requestObj, logSettings, 0);
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

			Uri uri = FormatUri(baseUri, getResponse ? SEND_RECEIVE : SEND, messageId);

			try
			{
				WebRequest request = WebRequest.Create(uri);
				request.Method = WebRequestMethods.Http.Post;

				request.Timeout = DEFAULT_TIMEOUT * 1000;
				if (timeoutSeconds > 0) request.Timeout = timeoutSeconds * 1000;

				Stream requestStream = request.GetRequestStream();

				requestStream = ObjectToJSonStream(requestObj, requestStream);

				if (getResponse)
				{
					using (WebResponse response = request.GetResponse())
					{
						using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
						{
							string text = responseReader.ReadToEnd();
							responseMessage = new HttpResponseMessage();
							responseMessage.Content = new StringContent(text, Encoding.UTF8, "application/json");
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

				if (logSettings != null)
				{
					LogError(baseUri, logSettings, "Send", ex);
				}

				throw;
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
						HttpResponseMessage response = Send(baseUri, messageId, obj, logSettings, timeoutSeconds, true);
						callBack(response);
					}
					else
					{
						Send(baseUri, messageId, obj, logSettings, timeoutSeconds, false);
					}
				}
				catch (Exception ex)
				{
					if (logSettings != null)
					{
						LogError(baseUri, logSettings, "SendAsync", ex);
					}
				}
			}).Start();

		}
		public static void SendAsync(Uri baseUri, string messageId, object obj, LogSettings logSettings, Action<HttpResponseMessage> callBack, int timeoutSeconds, CRMAuthTokenConfiguration crmAuthTokenConfiguration, ITracingService tracer)
		{
			new Thread(() =>
			{
				try
				{
					Thread.CurrentThread.IsBackground = false;
					if (callBack != null)
					{
						HttpResponseMessage response = Send(baseUri, messageId, obj, logSettings, timeoutSeconds, true);
						callBack(response);
					}
					else
					{
						tracer.Trace("Making the call to send the information with 6 params");
						//  Send(baseUri, messageId, obj, logSettings, timeoutSeconds, false);
						Send(baseUri, messageId, obj, 0, crmAuthTokenConfiguration, tracer);
					}
				}
				catch (Exception ex)
				{
					if (logSettings != null)
					{
						LogError(baseUri, logSettings, "SendAsync", ex);
					}
				}
			}).Start();

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
		public static T SendReceive<T>(Uri baseUri, string messageId, object obj, LogSettings logSettings)
		{
			return SendReceive<T>(baseUri, messageId, obj, logSettings, 0);
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
		/// <param name="timeoutSeconds">Timeout in Seconds</param>
		public static T SendReceive<T>(Uri baseUri, string messageId, object obj, LogSettings logSettings, int timeoutSeconds)
		{
			string message = string.Empty;
			//CSDev
			//Uri uri = FormatUri(baseUri, SEND_RECEIVE, messageId);
			Uri uri = baseUri;

			try
			{
				WebRequest request = WebRequest.Create(uri);
				request.Method = WebRequestMethods.Http.Post;

				request.Timeout = DEFAULT_TIMEOUT;
				if (timeoutSeconds > 0) request.Timeout = timeoutSeconds;


				Stream requestStream = request.GetRequestStream();

				requestStream = ObjectToJSonStream(obj, requestStream);

				using (WebResponse response = request.GetResponse())
				{
					using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
					{

						string responseValue = responseReader.ReadToEnd();

						//Parse message from VIMT, if there are valid tags.
						int start = responseValue.IndexOf("<Message>") + 9;
						int end = responseValue.IndexOf("</Message>");

						if (end != -1)
						{

							message = responseValue.Substring(start, end - start);
						}
					}
				}
			}

			catch (Exception ex)
			{
				genericResponse newresponse = new genericResponse();
				if (ex.GetType() == typeof(WebException))
				{
					if (((WebException)ex).Status == WebExceptionStatus.Timeout)
					{
						throw new VIMTTimeOutExeption(_vimtExceptionMessage);
					}
				}
				if (logSettings != null)
				{
					LogError(baseUri, logSettings, "SendReceive", ex);
				}
				throw;
			}
			//Debug.WriteLine("Thread ID {0} Ending", Thread.CurrentThread.ManagedThreadId);


			T retObj = DeserializeResponse<T>(message);
			return retObj;
		}

		//public static T SendReceive<T>(Uri baseUri, string messageId, object obj, LogSettings logSettings, int timeoutSeconds, CRMAuthTokenConfiguration crmAuthTokenConfiguration)
		//{
		//          return SendReceive<T>(baseUri, messageId, obj, logSettings, timeoutSeconds, crmAuthTokenConfiguration, null);
		//}

		//    ///NOTE: THIS SHOULD NOT BE USED----- tracer should be passed, NOT Logger
		///// <summary>
		///// Send a Request Message to VIMT and get the response of type T.
		///// The VIMT Message handler should be a RequestResponseHandler
		///// </summary>
		///// <typeparam name="T">Response Object Type</typeparam>
		///// <param name="baseUri">REST URI to the VIMT Server</param>
		///// <param name="messageId">Request Message</param>
		///// <param name="obj">Request Object</param>
		///// <param name="logSettings">Log Settings</param>
		///// <param name="timeoutSeconds">Timeout in Seconds</param>
		///// <param name="serviceDetails">Web Service Details</param>
		///// <returns></returns>
		//public static T SendReceive<T>(Uri baseUri, string messageId, object obj, LogSettings logSettings, int timeoutSeconds, CRMAuthTokenConfiguration crmAuthTokenConfiguration, MCSUtilities2011.MCSLogger Logger)
		//{
		//    string message = string.Empty;
		//    //CSDev
		//    Uri uri = FormatUri(baseUri, messageId);

		//    string responseValue;
		//    try
		//    {
		//        //Logger?.WriteDebugMessage($"Utility.SendReceive: Invoking Uri: {uri.ToString()} \r\n RequestBody: {JsonHelper.Serialize(obj, obj.GetType())}");
		//        using (HttpClient client = new HttpClient())
		//        {
		//            //Logger.WriteDebugMessage("1 AddAuthHeader - Before");

		//            client.AddAuthHeader(crmAuthTokenConfiguration);

		//            //Logger?.WriteDebugMessage("Utility.SendReceive crmAuthTokenConfiguration.ParentApplicationId: " + crmAuthTokenConfiguration.ParentApplicationId.ToString());
		//            //Logger?.WriteDebugMessage("Utility.SendReceive crmAuthTokenConfiguration.ClientApplicationId: " + crmAuthTokenConfiguration.ClientApplicationId.ToString());
		//            ////Logger?.WriteDebugMessage("Utility.SendReceive crmAuthTokenConfiguration.ClientSecret: " + crmAuthTokenConfiguration.ClientSecret.ToString());
		//            //Logger?.WriteDebugMessage("Utility.SendReceive crmAuthTokenConfiguration.TenantId: " + crmAuthTokenConfiguration.TenantId.ToString());
		//            //Logger?.WriteDebugMessage("Utility.SendReceive crmAuthTokenConfiguration.ApimSubscriptionKey " + crmAuthTokenConfiguration.ApimSubscriptionKey.ToString());

		//            //Logger?.WriteDebugMessage("20 AddAuthHeader - After");
		//            using (MemoryStream memoryStream = ObjectToJSonStream(obj))
		//            using (StreamContent sc = new StreamContent(memoryStream))
		//            {
		//                sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
		//                client.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT * 1000);
		//                if (timeoutSeconds > 0)
		//                {
		//                    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds * 1000);
		//                }
		//                //Logger?.WriteDebugMessage($"Utility.SendReceive: Invoking Uri: {uri.ToString()} ");

		//                HttpResponseMessage response = client.PostAsync(uri, sc).Result;
		//                response.EnsureSuccessStatusCode();
		//                responseValue = response.Content.ReadAsStringAsync().Result;
		//                //Logger?.WriteDebugMessage($@"Utility.SendReceive: Invoked Uri: {uri.ToString()} \r\n\Response: {responseValue}");
		//            }
		//        }

		//    }
		//    catch (Exception ex)
		//    {
		//        //Logger?.WriteDebugMessage("Utility.SendReceive: Outer Exception Fired: " + ex.StackTrace + " | " + ex.Message + " | " + ex.GetType().FullName);
		//        if (ex.InnerException != null)
		//        {
		//            //Logger?.WriteDebugMessage("Utility.SendReceive: Inner Exception: " + ex.InnerException.GetType().FullName + " | " + ex.InnerException);
		//        }


		//        if (ex.GetType() == typeof(WebException))
		//        {
		//            if (((WebException)ex).Status == WebExceptionStatus.Timeout)
		//            {
		//                throw new VIMTTimeOutExeption(_vimtExceptionMessage);
		//            }
		//        }
		//        if (logSettings != null)
		//        {
		//            //LogError(baseUri, logSettings, "SendReceive", ex);
		//        }
		//        throw;
		//    }

		//    T retObj = JsonHelper.Deserialize<T>(responseValue);
		//    //Logger?.WriteDebugMessage("Utility.SendReceive: DeserializeBase64Response - After - Deserialize SUCCESS  ");

		//    return retObj;
		//}

		public static T SendReceive<T>(Uri baseUri, string messageId, object obj, LogSettings logSettings, int timeoutSeconds, CRMAuthTokenConfiguration crmAuthTokenConfiguration, ITracingService tracer)
		{
			string message = string.Empty;
			//CSDev
			Uri uri = FormatUri(baseUri, messageId);

			string responseValue;
			try
			{
				tracer?.Trace($"Utility.SendReceive: Invoking Uri: {uri.ToString()} \r\n RequestBody: {JsonHelper.Serialize(obj, obj.GetType())}");
				using (HttpClient client = new HttpClient())
				{
					client.AddAuthHeader(crmAuthTokenConfiguration);

					using (MemoryStream memoryStream = ObjectToJSonStream(obj))
					using (StreamContent sc = new StreamContent(memoryStream))
					{
						sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
						client.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT);
						if (timeoutSeconds > 0)
						{
							client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
						}
						tracer?.Trace($"timeout set to: {client.Timeout}");
						tracer?.Trace($"Utility.SendReceive: Invoking Uri: {uri.ToString()} ");

						HttpResponseMessage response = client.PostAsync(uri, sc).Result;
						response.EnsureSuccessStatusCode();
						responseValue = response.Content.ReadAsStringAsync().Result;
						tracer?.Trace($@"Utility.SendReceive: Invoked Uri: {uri.ToString()} \r\n\Response: {responseValue}");
					}
				}
				T retObj = JsonHelper.Deserialize<T>(responseValue);
				tracer?.Trace("Utility.SendReceive: DeserializeBase64Response - After - Deserialize SUCCESS  ");

				return retObj;
			}
			catch (Exception ex)
			{
				genericResponse thisRespon = new genericResponse();
				thisRespon.ExceptionOccured = true;


				tracer?.Trace("Utility.SendReceive: Outer Exception Fired: " + ex.StackTrace + " | " + ex.Message + " | " + ex.GetType().FullName);
				if (ex.InnerException != null)
				{
					tracer?.Trace("Utility.SendReceive: Inner Exception: " + ex.InnerException.GetType().FullName + " | " + ex.InnerException);
				}
				if (ex.InnerException.Message == "A task was canceled.")
				{
					thisRespon.ExceptionMessage = "Timed Out";
				}
				else
				{
					thisRespon.ExceptionMessage = ex.InnerException.Message;
				}
				var jsonReturn = JsonHelper.Serialize<genericResponse>(thisRespon);

				T retObj = JsonHelper.Deserialize<T>(jsonReturn);
				tracer?.Trace("Utility.SendReceive: DeserializeBase64Response - After - Deserialize SUCCESS  ");

				return retObj;

			}


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
			Uri retUri = new Uri(baseUri, relativeUri);
			return retUri;
		}

		/// <summary>
		/// Format the URI with method and Message type.
		/// </summary>
		/// <param name="baseUri">REST URI to the VIMT Server</param>        
		/// <param name="messageId">Request Message</param>
		/// <returns>Destination Uri</returns>
		public static Uri FormatUri(Uri baseUri, string messageId)
		{
			string urlRestPath = UDO.LOB.LOBAPIDictionary.LOBAPIDictionary.RequestAPI[messageId];
			if (urlRestPath == null || urlRestPath == string.Empty)
			{
				throw new Exception("Matching Request API not found in the LOBAPIDictionary for the message: " + messageId);
			}

			Uri retUri = new Uri(baseUri.ToString() + urlRestPath);

			return retUri;
		}

		public static T DeserializeBase64Response<T>(string message, ITracingService tracer)
		{
			//Logger.WriteDebugMessage("80 DeserializeBase64Response - Top");
			T retObj;
			tracer.Trace("65 DeserializeBase64Response - Convert.FromBase64String(message);");
			//CSDEv CURRENT BROKEN CODE
			byte[] b = Convert.FromBase64String(message);
			tracer.Trace("651 DeserializeBase64Response - After Convert.FromBase64String(message);");
			UTF8Encoding enc = new UTF8Encoding();
			string mess = enc.GetString(b);
			//Logger.WriteDebugMessage("81 DeserializeBase64Response - After Encoding");
			//REplace out the NewtonSoft specific dates with datacontract dates.
			string fixedDates = Regex.Replace(mess, @"new Date\(([-+0-9]*)\)", "\"\\/Date($1+0000)\\/\"");
			//Logger.WriteDebugMessage("83 DeserializeBase64Response - After Date Fix");
			using (MemoryStream ms = new MemoryStream())
			{
				//Logger.WriteDebugMessage("84 DeserializeBase64Response - Before Write");
				ms.Write(enc.GetBytes(fixedDates), 0, enc.GetByteCount(fixedDates));// fixedDates.Length);

				ms.Position = 0;
				//Logger.WriteDebugMessage("84 DeserializeBase64Response - Before Deserial Contract");
				DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
				//Logger.WriteDebugMessage("85 DeserializeBase64Response - Before Deserial Read");
				retObj = (T)ser.ReadObject(ms);
			}
			return retObj;
		}

		/// <summary>
		/// Deserialize Message to type T
		/// </summary>
		/// <typeparam name="T">Response Object Type</typeparam>
		/// <param name="message">String message to deserialize</param>
		/// <returns>Response Object</returns>
		public static T DeserializeResponse<T>(string message)
		{
			T retObj;
			byte[] b = Convert.FromBase64String(message);
			UTF8Encoding enc = new UTF8Encoding();
			string mess = enc.GetString(b);

			//REplace out the NewtonSoft specific dates with datacontract dates.
			string fixedDates = Regex.Replace(mess, @"new Date\(([-+0-9]*)\)", "\"\\/Date($1+0000)\\/\"");
			//string fixedDates = mess;

			using (MemoryStream ms = new MemoryStream())
			{
				ms.Write(enc.GetBytes(fixedDates), 0, fixedDates.Length);

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

		private static MemoryStream ObjectToJSonStream(object obj)
		{
			MemoryStream memStream = new MemoryStream();

			DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());

			ser.WriteObject(memStream, obj);

			memStream.Position = 0;
			return memStream;
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
					crme_ErrorMessage = Sanitizer(message),
					crme_Debug = false,
					crme_GranularTiming = false,
					crme_TransactionTiming = false,
					crme_Method = crme_method,
					crme_LogLevel = (int)LogLevel.Error,
					crme_Sequence = 1,
					NameofDebugSettingsField = configFieldName
				};

				CreateCRMLogEntryResponse logResponse = SendReceive<CreateCRMLogEntryResponse>(baseUri, Utility.CreateCRMLogEntryRequest, logRequestStart, null);
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
			LogError(baseUri, org, configFieldName, userId, method, ex.Message, callingMethod);
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
			LogError(baseUri, logSettings.Org, logSettings.ConfigFieldName, logSettings.UserId, method, message, logSettings.callingMethod);
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
			LogError(baseUri, logSettings.Org, logSettings.ConfigFieldName, logSettings.UserId, method, ex, logSettings.callingMethod);
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
					crme_ErrorMessage = Sanitizer(message),
					crme_Debug = true,
					crme_GranularTiming = false,
					crme_TransactionTiming = false,
					crme_Method = crme_method,
					crme_LogLevel = (int)LogLevel.Debug,
					crme_Sequence = 1,
					NameofDebugSettingsField = configFieldName
				};

				CreateCRMLogEntryResponse logResponse = SendReceive<CreateCRMLogEntryResponse>(baseUri, Utility.CreateCRMLogEntryRequest, logRequestStart, null);
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
			LogDebug(baseUri, logSettings.Org, logSettings.ConfigFieldName, logSettings.UserId, method, message, logSettings.callingMethod);
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
			sb.AppendLine(ex.StackTrace);

			if (ex.InnerException != null)
			{
				BuildStackTrace(ex.InnerException, sb);
			}
		}
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
		public static HttpResponseMessage Send(Uri baseUri, string messageId, object requestObj, int timeoutSeconds, CRMAuthTokenConfiguration crmAuthTokenConfiguration, ITracingService tracer)
		{
			HttpResponseMessage responseMessage = null;
			//Uri uri = baseUri;
			Uri uri = FormatUri(baseUri, messageId);

			try
			{
				using (HttpClient client = new HttpClient())
				using (MemoryStream memStream = ObjectToJSonStream(requestObj))
				using (StreamContent sc = new StreamContent(memStream))
				{
					sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

					client.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT * 1000);
					if (timeoutSeconds > 0) client.Timeout = TimeSpan.FromSeconds(timeoutSeconds * 1000);

					client.AddAuthHeader(crmAuthTokenConfiguration);

					try
					{
						tracer.Trace($" NNN Utility.SendDev :: Invoking Uri: { uri.AbsoluteUri }");

						//System.Threading.Tasks.Task<HttpResponseMessage> task = client.PostAsync(uri, sc); //.ConfigureAwait(false);//.Result;
						//task.Wait();  //have to change task.start so it runs async?   .ConfigureAwait(false);
						//			  //task.Start();
						//			  //Or do we delete this? s
						//			  //Use the PLT to make the update post asyc and enable then do the configure await >> talk to nithin tmrw
						//			  //logger.WriteGranularTimingMessage($" NNN Utility.SendDev :: Invoked Uri | After Wait");

						System.Threading.Tasks.Task<HttpResponseMessage> task = client.PostAsync(uri, sc);
						task.ConfigureAwait(false);

						responseMessage = task.Result;
						responseMessage.EnsureSuccessStatusCode();
						var message = responseMessage.Content.ReadAsStringAsync().Result;

						//logger.WriteGranularTimingMessage($@" NNN Utility.SendDev: Invoked Uri: {uri.ToString()} \r\n\Response: {message}");
					}
					catch (Exception ex)
					{
						tracer.Trace($@" NNN Utility.SendDev Inner Exception: {ex.Message}");
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

				//LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingMethod, ex);
				tracer.Trace($@" NNN Utility.SendDev Outer Exception: {ex.Message}");
			}

			return responseMessage;
		}

		public static string Sanitizer(string potentiallyDirtyMsg)
		{
			try
			{
				// Fortify required to cleanse messages
				/*Regex r = new Regex(
                      "(?:[^a-zA-Z0-9 ]|(?<=['\"])s)",
                      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                return r.Replace(potentiallyDirtyMsg, String.Empty); */
				return potentiallyDirtyMsg;
			}
			catch (Exception ex)
			{
				return "Error sanitizing message: " + ex.Message;
			}
		}
	}

	/// <summary>
	/// Log Settings: Contains the settings to log the information/errors to
	/// </summary>
	public class LogSettings
	{
		/// <summary>
		/// CRM Organization
		/// </summary>
		public string Org { get; set; }
		/// <summary>
		/// Future Use On/Off Switch
		/// </summary>
		public string ConfigFieldName { get; set; }
		/// <summary>
		/// Calling User
		/// </summary>
		public Guid UserId { get; set; }
		/// <summary>
		/// Calling Method
		/// </summary>
		public string callingMethod { get; set; }
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

	[DataContract]
	public class genericResponse
	{

		[DataMember]
		public bool ExceptionOccured { get; set; }
		[DataMember]
		public string ExceptionMessage { get; set; }

	}
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


}