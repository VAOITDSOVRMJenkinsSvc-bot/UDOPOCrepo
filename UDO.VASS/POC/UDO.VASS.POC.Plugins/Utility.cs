using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using UDO.VASS.POC.Plugins.Entities;

namespace UDO.VASS.POC.Plugins
{
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

		///// <summary>
		///// Send a Request Message to VIMT and get the response of type T.
		///// The VIMT Message Handler should be a RequestResponseHandler
		///// </summary>
		///// <typeparam name="T">Response Object Type</typeparam>
		///// <param name="baseUri">REST URI to the VIMT Server</param>
		///// <param name="messageId">Request Message</param>
		///// <param name="obj">Request Object</param>
		///// <param name="logSettings">Log Settings</param>
		//public static T SendReceive<T>(Uri baseUri, string messageId, object obj, LogSettings logSettings)
		//{
		//	return SendReceive<T>(baseUri, messageId, obj, logSettings, 0);
		//}
		///// <summary>
		///// Send a Request Message to VIMT and get the response of type T.
		///// The VIMT Message Handler should be a RequestResponseHandler
		///// </summary>
		///// <typeparam name="T">Response Object Type</typeparam>
		///// <param name="baseUri">REST URI to the VIMT Server</param>
		///// <param name="messageId">Request Message</param>
		///// <param name="obj">Request Object</param>
		///// <param name="logSettings">Log Settings</param>
		///// <param name="timeoutSeconds">Timeout in Seconds</param>
		//public static T SendReceive<T>(Uri baseUri, string messageId, object obj, LogSettings logSettings, int timeoutSeconds)
		//{
		//	string message = string.Empty;
		//	//CSDev
		//	//Uri uri = FormatUri(baseUri, SEND_RECEIVE, messageId);
		//	Uri uri = baseUri;

		//	try
		//	{
		//		WebRequest request = WebRequest.Create(uri);
		//		request.Method = WebRequestMethods.Http.Post;

		//		request.Timeout = DEFAULT_TIMEOUT * 1000;
		//		if (timeoutSeconds > 0) request.Timeout = timeoutSeconds * 1000;


		//		Stream requestStream = request.GetRequestStream();

		//		requestStream = ObjectToJSonStream(obj, requestStream);

		//		using (WebResponse response = request.GetResponse())
		//		{
		//			using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
		//			{

		//				string responseValue = responseReader.ReadToEnd();

		//				//Parse message from VIMT, if there are valid tags.
		//				int start = responseValue.IndexOf("<Message>") + 9;
		//				int end = responseValue.IndexOf("</Message>");

		//				if (end != -1)
		//				{

		//					message = responseValue.Substring(start, end - start);
		//				}
		//			}
		//		}
		//	}

		//	catch (Exception ex)
		//	{

		//		if (ex.GetType() == typeof(WebException))
		//		{
		//			if (((WebException)ex).Status == WebExceptionStatus.Timeout)
		//			{
		//				throw new VIMTTimeOutExeption(_vimtExceptionMessage);
		//			}
		//		}
		//		if (logSettings != null)
		//		{
		//			LogError(baseUri, logSettings, "SendReceive", ex);
		//		}
		//		throw;
		//	}
		//	//Debug.WriteLine("Thread ID {0} Ending", Thread.CurrentThread.ManagedThreadId);


		//	T retObj = DeserializeResponse<T>(message);
		//	return retObj;
		//}

		public static T SendReceive<T>(Uri baseUri, string messageId, object obj, int timeoutSeconds, UDOSettings crmAuthTokenConfiguration, ITracingService tracer)
		{
			string message = string.Empty;
			//CSDev
			Uri uri = FormatUri(baseUri, messageId);

			string responseValue;
			try
			{
				tracer.Trace($"Utility.SendReceive: Invoking Uri: {uri.ToString()} \r\n RequestBody: {JsonHelper.Serialize(obj, obj.GetType())}");
				using (HttpClient client = new HttpClient())
				{
					client.AddAuthHeader(crmAuthTokenConfiguration);

					using (MemoryStream memoryStream = ObjectToJSonStream(obj))
					using (StreamContent sc = new StreamContent(memoryStream))
					{
						sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
						client.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT * 1000);
						if (timeoutSeconds > 0)
						{
							client.Timeout = TimeSpan.FromSeconds(timeoutSeconds * 1000);
						}
						tracer.Trace($"Utility.SendReceive: Invoking Uri: {uri.ToString()} ");

						HttpResponseMessage response = client.PostAsync(uri, sc).Result;
						response.EnsureSuccessStatusCode();
						tracer.Trace($"{response.StatusCode}");
						responseValue = response.Content.ReadAsStringAsync().Result;
						tracer.Trace($@"Utility.SendReceive: Invoked Uri: {uri.ToString()} \r\n\Response: {responseValue}");
					}
				}

			}
			catch (Exception ex)
			{
				tracer.Trace("Utility.SendReceive: Outer Exception Fired: " + ex.StackTrace + " | " + ex.Message + " | " + ex.GetType().FullName);
				if (ex.InnerException != null)
				{
					tracer.Trace("Utility.SendReceive: Inner Exception: " + ex.InnerException.GetType().FullName + " | " + ex.InnerException);
				}


				if (ex.GetType() == typeof(WebException))
				{
					if (((WebException)ex).Status == WebExceptionStatus.Timeout)
					{
						throw new Exception(_vimtExceptionMessage);
					}
				}
				//if (logSettings != null)
				//{
				//    LogError(baseUri, logSettings, "SendReceive", ex);
				//}
				throw;
			}

			T retObj = JsonHelper.Deserialize<T>(responseValue);
			tracer.Trace("Utility.SendReceive: DeserializeBase64Response - After - Deserialize SUCCESS  ");

			return retObj;
		}


		/// <summary>
		/// Format the URI with method and Message type.
		/// </summary>
		/// <param name="baseUri">REST URI to the VIMT Server</param>        
		/// <param name="messageId">Request Message</param>
		/// <returns>Destination Uri</returns>
		public static Uri FormatUri(Uri baseUri, string messageId)
		{
			string urlRestPath = "/UDO/NotesSvc/api/notes/createNotes";
			if (urlRestPath == null || urlRestPath == string.Empty)
			{
				throw new Exception("Matching Request API not found in the LOBAPIDictionary for the message: " + messageId);
			}

			Uri retUri = new Uri(baseUri.ToString() + urlRestPath);

			return retUri;
		}

		//public static T DeserializeBase64Response<T>(string message, ITracingService tracer)
		//{
		//	//Logger.WriteDebugMessage("80 DeserializeBase64Response - Top");
		//	T retObj;
		//	tracer.Trace("65 DeserializeBase64Response - Convert.FromBase64String(message);");
		//	//CSDEv CURRENT BROKEN CODE
		//	byte[] b = Convert.FromBase64String(message);
		//	tracer.Trace("651 DeserializeBase64Response - After Convert.FromBase64String(message);");
		//	UTF8Encoding enc = new UTF8Encoding();
		//	string mess = enc.GetString(b);
		//	//Logger.WriteDebugMessage("81 DeserializeBase64Response - After Encoding");
		//	//REplace out the NewtonSoft specific dates with datacontract dates.
		//	string fixedDates = Regex.Replace(mess, @"new Date\(([-+0-9]*)\)", "\"\\/Date($1+0000)\\/\"");
		//	//Logger.WriteDebugMessage("83 DeserializeBase64Response - After Date Fix");
		//	using (MemoryStream ms = new MemoryStream())
		//	{
		//		//Logger.WriteDebugMessage("84 DeserializeBase64Response - Before Write");
		//		ms.Write(enc.GetBytes(fixedDates), 0, enc.GetByteCount(fixedDates));// fixedDates.Length);

		//		ms.Position = 0;
		//		//Logger.WriteDebugMessage("84 DeserializeBase64Response - Before Deserial Contract");
		//		DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
		//		//Logger.WriteDebugMessage("85 DeserializeBase64Response - Before Deserial Read");
		//		retObj = (T)ser.ReadObject(ms);
		//	}
		//	return retObj;
		//}

		///// <summary>
		///// Deserialize Message to type T
		///// </summary>
		///// <typeparam name="T">Response Object Type</typeparam>
		///// <param name="message">String message to deserialize</param>
		///// <returns>Response Object</returns>
		//public static T DeserializeResponse<T>(string message)
		//{
		//	T retObj;
		//	byte[] b = Convert.FromBase64String(message);
		//	UTF8Encoding enc = new UTF8Encoding();
		//	string mess = enc.GetString(b);

		//	//REplace out the NewtonSoft specific dates with datacontract dates.
		//	string fixedDates = Regex.Replace(mess, @"new Date\(([-+0-9]*)\)", "\"\\/Date($1+0000)\\/\"");
		//	//string fixedDates = mess;

		//	using (MemoryStream ms = new MemoryStream())
		//	{
		//		ms.Write(enc.GetBytes(fixedDates), 0, fixedDates.Length);

		//		ms.Position = 0;

		//		DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
		//		retObj = (T)ser.ReadObject(ms);
		//	}
		//	return retObj;
		//}

		private static MemoryStream ObjectToJSonStream(object obj)
		{
			MemoryStream memStream = new MemoryStream();

			DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());

			ser.WriteObject(memStream, obj);

			memStream.Position = 0;
			return memStream;
		}
		///// <summary>
		///// Log Error
		///// </summary>
		///// <param name="baseUri">REST URI to the VIMT Server</param>
		///// <param name="org">CRM Organization</param>
		///// <param name="configFieldName">Not Used: Future On/Off switch</param>
		///// <param name="userId">Calling UserId</param>
		///// <param name="method">Child Calling Method or Sub Procedure</param>
		///// <param name="message">Error Message</param>
		///// <param name="callingMethod">Parent Calling Method</param>
		////public static void LogError(Uri baseUri, string org, string configFieldName, Guid userId, string method, string message, string callingMethod = null)
		////{
		////	string crme_method;
		////	if (!string.IsNullOrEmpty(callingMethod))
		////	{
		////		crme_method = callingMethod + ": " + method;
		////	}
		////	else
		////	{
		////		crme_method = method;
		////	}

		////	try
		////	{
		////		CreateCRMLogEntryRequest logRequestStart = new CreateCRMLogEntryRequest()
		////		{
		////			MessageId = Guid.NewGuid().ToString(),
		////			OrganizationName = org,
		////			UserId = userId,
		////			crme_Name = string.Format("Exception: {0}:{1}", "Error in ", method),
		////			crme_ErrorMessage = message,
		////			crme_Debug = false,
		////			crme_GranularTiming = false,
		////			crme_TransactionTiming = false,
		////			crme_Method = crme_method,
		////			crme_LogLevel = (int)LogLevel.Error,
		////			crme_Sequence = 1,
		////			NameofDebugSettingsField = configFieldName
		////		};

		////		CreateCRMLogEntryResponse logResponse = SendReceive<CreateCRMLogEntryResponse>(baseUri, Utility.CreateCRMLogEntryRequest, logRequestStart, null);
		////	}
		////	catch (Exception) { }

		////}





		///// <summary>
		///// Send a Request Message to VIMT.  
		///// getResposne specifies whether or not a response is expected.  The default is false.
		///// 
		///// If getResponse is true, then a VIMT RequestResponseHandler is expected
		///// If getResponse is false, then a VIMT RequestHandler is expected
		///// </summary>
		///// <param name="baseUri">REST URI to the VIMT Server</param>
		///// <param name="messageId">Request Message</param>
		///// <param name="requestObj">Request Object</param>
		///// <param name="logSettings">Log Settings</param>
		///// <param name="timeoutSeconds">Timeout in Seconds</param>
		///// <param name="getResponse">Get a Response (if true, uses SEND_RECEIVE, if false, uses SEND)</param>
		///// <returns>HttpResponseMessage: null if getResponse is false</returns>
		//public static HttpResponseMessage Send(Uri baseUri, string messageId, object requestObj, int timeoutSeconds, CRMAuthTokenConfiguration crmAuthTokenConfiguration, ITracingService tracer)
		//{
		//	HttpResponseMessage responseMessage = null;
		//	//Uri uri = baseUri;
		//	Uri uri = FormatUri(baseUri, messageId);

		//	try
		//	{
		//		using (HttpClient client = new HttpClient())
		//		using (MemoryStream memStream = ObjectToJSonStream(requestObj))
		//		using (StreamContent sc = new StreamContent(memStream))
		//		{
		//			sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

		//			client.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT * 1000);
		//			if (timeoutSeconds > 0) client.Timeout = TimeSpan.FromSeconds(timeoutSeconds * 1000);

		//			client.AddAuthHeader(crmAuthTokenConfiguration);

		//			try
		//			{
		//				tracer.Trace($" NNN Utility.SendDev :: Invoking Uri: { uri.AbsoluteUri }");

		//				//System.Threading.Tasks.Task<HttpResponseMessage> task = client.PostAsync(uri, sc); //.ConfigureAwait(false);//.Result;
		//				//task.Wait();  //have to change task.start so it runs async?   .ConfigureAwait(false);
		//				//			  //task.Start();
		//				//			  //Or do we delete this? s
		//				//			  //Use the PLT to make the update post asyc and enable then do the configure await >> talk to nithin tmrw
		//				//			  //logger.WriteGranularTimingMessage($" NNN Utility.SendDev :: Invoked Uri | After Wait");

		//				System.Threading.Tasks.Task<HttpResponseMessage> task = client.PostAsync(uri, sc);
		//				task.ConfigureAwait(false);

		//				responseMessage = task.Result;
		//				responseMessage.EnsureSuccessStatusCode();
		//				var message = responseMessage.Content.ReadAsStringAsync().Result;

		//				//logger.WriteGranularTimingMessage($@" NNN Utility.SendDev: Invoked Uri: {uri.ToString()} \r\n\Response: {message}");
		//			}
		//			catch (Exception ex)
		//			{
		//				tracer.Trace($@" NNN Utility.SendDev Inner Exception: {ex.Message}");
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{

		//		if (ex.GetType() == typeof(WebException))
		//		{
		//			if (((WebException)ex).Status == WebExceptionStatus.Timeout)
		//			{
		//				throw new VIMTTimeOutExeption(_vimtExceptionMessage);
		//			}
		//		}

		//		//LogHelper.LogError(messageId, logSettings.Org, logSettings.UserId, logSettings.CallingMethod, ex);
		//		tracer.Trace($@" NNN Utility.SendDev Outer Exception: {ex.Message}");
		//	}

		//	return responseMessage;
		//}
	}
}
