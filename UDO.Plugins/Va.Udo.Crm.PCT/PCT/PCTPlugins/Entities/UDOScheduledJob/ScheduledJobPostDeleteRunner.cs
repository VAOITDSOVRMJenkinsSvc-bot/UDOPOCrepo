using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using VRMRest;
using System.ServiceModel;
using VRM.Integration.UDO.ScheduledJob.Messages;

namespace VRM.Integration.ScheduledJob
{
    internal class ScheduledJobPostDeleteRunner : PluginRunner
    {
        private Uri _uri = null;
   
        public ScheduledJobPostDeleteRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }
        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.PreEntityImages["Pre"] as Entity;
        }
        public override Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.PreEntityImages["Pre"] as Entity;
        }
        public override string McsSettingsDebugField
        {
            get { return "udo_scheduledjob"; }
        }
        internal void Execute()
        {
            try
            {
                var uri = McsSettings.GetSingleSetting<string>("crme_restendpointforvimt");
                _uri = new Uri(uri);

                var request = new UDOScheduledJobRequest()
                {
                    MessageId = Guid.NewGuid().ToString(),
                    UserId = PluginExecutionContext.InitiatingUserId,
                    OrganizationName = PluginExecutionContext.OrganizationName,
                    JobName = "QueueCleanup",
                    Debug = true
                };
                LogSettings logSettings = new LogSettings()
                {
                    callingMethod = "ScheduledJobPostDeleteRunner",
                    UserId = PluginExecutionContext.UserId,
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL"
                };
                var response = Utility.SendReceive<UDOScheduledJobResponse>(_uri, "UDOScheduledJobRequest", request, logSettings);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Logger.WriteToFile(ex.Message);
                Logger.WriteToFile(ex.StackTrace);
                TracingService.Trace(ex.Message);
                TracingService.Trace(ex.StackTrace);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
            catch (Exception ex)
            {
                Logger.WriteToFile(ex.Message);
                Logger.WriteToFile(ex.StackTrace);
                TracingService.Trace(ex.Message);
                TracingService.Trace(ex.StackTrace);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
        }
    }
}