using Microsoft.Xrm.Tooling.Connector;

using System;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.CADDApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LogHelper.LogInfo($" ------------------------------------------------------- Application Start Event ------------------------------------------------------- ");
            LogHelper.LogInfo($" {AssemblyMetadata()}");
            LogHelper.LogInfo($" --------------------------------------------------------------------------------------------------------------------------------------- ");

            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Get CRM Connection
            CrmServiceClient proxy;
            try
            {
                proxy = ConnectionCache.GetProxy();

                string org = proxy.ConnectedOrgFriendlyName;
                string baseURI = proxy.CrmConnectOrgUriActual.AbsoluteUri;
                LogHelper.LogInfo($"Connected to org: {org} base URI: {baseURI}");
            }
            catch (Exception ex)
            {
                LogHelper.LogInfo($"[LOGERROR] ERROR in Application_Start : Unable to connect to CRM :: {WebApiUtility.StackTraceToString(ex)}");
                throw ex;
            }

            try
            {
                var gwatch = Stopwatch.StartNew();
                var watch = Stopwatch.StartNew();
                LogHelper.LogInfo($" >>> Loading EntityCache ...");
                EntityCache.LoadEntityCache();
                LogHelper.LogInfo($" <<< Loaded EntityCache in ({watch.ElapsedMilliseconds}) ms.");
                watch.Stop();
                watch.Start();
                LogHelper.LogInfo($" >>> Loading TruncHelperSettings ...");
                TruncHelperSettings tr = new TruncHelperSettings();
                tr.Load(proxy);
                LogHelper.LogInfo($" <<< Loaded TruncHelperSettings in ({watch.ElapsedMilliseconds}) ms.");
                watch.Stop();
                watch.Start();
                LogHelper.LogInfo($" >>> Loading ExecuteMultipleHelperSettings ...");
                ExecuteMultipleHelperSettings.LoadFromCRM(proxy);
                LogHelper.LogInfo($" <<< Loaded ExecuteMultipleHelperSettings in ({watch.ElapsedMilliseconds}) ms.");
                GetCacheDetails();
                LogHelper.LogInfo($" <<< Initialized the Application in ({gwatch.ElapsedMilliseconds}) ms.");
				
                var disableTelemetry = false;
                if (System.Configuration.ConfigurationManager.AppSettings["DisableTelemetry"] != null)
                {
                    bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["DisableTelemetry"], out disableTelemetry);
                }
                Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.DisableTelemetry = disableTelemetry;
            }
            catch (Exception ex)
            {
                LogHelper.LogInfo($"[LOGERROR] ERROR in Application_Start :: {WebApiUtility.StackTraceToString(ex)}");
            }
            finally
            {
                if (proxy != null)
                {
                    proxy.Dispose();
                }
            }
        }

        private void GetCacheDetails()
        {
            try
            {
                var cacheItems = HttpContext.Current.Cache;

                foreach (var item in cacheItems)
                {
                    string key = ((System.Collections.DictionaryEntry)item).Key.ToString();
                    string value = ((System.Collections.DictionaryEntry)item).Value.ToString();
                    LogHelper.LogInfo($"Key: {key} Value: {value}");
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogInfo($"[LOGERROR] ERROR in Application_Start :: {WebApiUtility.StackTraceToString(ex)}");
            }
        }

        protected void Application_End()
        {
            LogHelper.LogInfo($" ------------------------------------------------------- Application End Event Executed. ------------------------------------------------------- ");
        }

        private string AssemblyMetadata()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string name = currentAssembly.FullName;
            string version = this.GetType().Assembly.GetName().Version.ToString();

            return $" ApiName: {name}, Version: {version}";
        }
    }
}
