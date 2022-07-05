using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Xrm.Tooling.Connector;

namespace UDO.LOB.ClaimEstablishmentApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LogHelper.LogInfo($" ------------------------------------------------------- Application Start Event ------------------------------------------------------- ");
            LogHelper.LogInfo($" {AssemblyMetadata()}");
            LogHelper.LogInfo($" --------------------------------------------------------------------------------------------------------------------------------------- ");

            var disableTelemetry = false;
            if (System.Configuration.ConfigurationManager.AppSettings["DisableTelemetry"] != null)
            {
                bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["DisableTelemetry"], out disableTelemetry);
            }
            Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.DisableTelemetry = disableTelemetry;

            GlobalConfiguration.Configure(WebApiConfig.Register);
            CrmServiceClient proxy = null;

            try
            {
                var gwatch = Stopwatch.StartNew();
                proxy = ConnectionCache.GetProxy();
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
