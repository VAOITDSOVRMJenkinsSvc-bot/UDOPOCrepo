using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace VRM.CRME.Plugin.DependentMaintenance.McsSettings
{
    public class McsSettingsUpdatePostPlugin : IPlugin
    {
        #region IPlugin Implementation
        public void Execute(IServiceProvider serviceProvider)
        {
            McsSettingsUpdatePostPluginRunner runner = null;
            try
            {
                runner = new McsSettingsUpdatePostPluginRunner(serviceProvider);

                runner.Execute(serviceProvider);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                if (runner == null)
                    throw;

                runner.TracingService.Trace(ex.Message);

                throw new InvalidPluginExecutionException(runner.McsSettings.getUnexpectedErrorMessage);
            }
            catch (Exception ex)
            {
                if (runner == null)
                    throw;

                if (ex.Message.StartsWith("custom"))
                {
                    runner.TracingService.Trace(ex.Message.Substring(6));

                    throw new InvalidPluginExecutionException(ex.Message.Substring(6));
                }

                runner.TracingService.Trace(ex.Message);

                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
        #endregion
    }
}
