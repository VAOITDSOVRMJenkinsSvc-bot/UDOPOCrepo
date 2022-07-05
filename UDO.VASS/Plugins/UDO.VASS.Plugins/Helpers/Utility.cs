using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using UDO.VASS.Plugins.Models;

namespace UDO.VASS.Plugins.Helpers
{
	public class Utility
    {
		private const string _vimtExceptionMessage = "The Query of the Legacy system timed out, click on refresh to try again";
		private const int DEFAULT_TIMEOUT = 60;

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

						HttpResponseMessage response = client.PostAsync(uri, sc).GetAwaiter().GetResult();
						response.EnsureSuccessStatusCode();
						tracer.Trace($"{response.StatusCode}");
						responseValue = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
						tracer.Trace($@"Utility.SendReceive: Invoked Uri: {uri.ToString()} \r\n\Response: {responseValue}");

						var resObject = JsonHelper.Deserialize<T>(responseValue);

						return resObject;
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

				throw;
			}

			T retObj = JsonHelper.Deserialize<T>(responseValue);
			tracer.Trace("Utility.SendReceive: DeserializeBase64Response - After - Deserialize SUCCESS  ");

			return retObj;
		}

		public static Uri FormatUri(Uri baseUri, string messageId)
		{
			string urlRestPath = "/UDO/NotesSvc/api/notes/createNotes";			
			Uri retUri = new Uri(baseUri.ToString() + urlRestPath);

			return retUri;
		}

		private static MemoryStream ObjectToJSonStream(object obj)
		{
			MemoryStream memStream = new MemoryStream();

			DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());

			ser.WriteObject(memStream, obj);

			memStream.Position = 0;
			return memStream;
		}

	}
}
