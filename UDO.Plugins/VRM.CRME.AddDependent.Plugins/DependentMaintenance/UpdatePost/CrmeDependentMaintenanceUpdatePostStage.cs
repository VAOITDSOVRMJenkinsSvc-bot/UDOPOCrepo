using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
    
namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class CrmeDependentMaintenanceUpdatePostStage : IPlugin
    {
        #region IPlugin Implementation
        public void Execute(IServiceProvider serviceProvider)
        {
            CrmeDependentMaintenanceUpdatePostStageRunner runner = null;
            try
            {
                runner = new CrmeDependentMaintenanceUpdatePostStageRunner(serviceProvider);

                runner.Execute(serviceProvider);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                if (runner == null) 
                    throw;

                runner.Logger.WriteToFile(ex.Message);

                throw new InvalidPluginExecutionException(runner.McsSettings.getUnexpectedErrorMessage);
            }
            catch (Exception ex)
            {
                if (runner == null)
                    throw;

                if (ex.Message.StartsWith("custom"))
                {
                    runner.Logger.WriteDebugMessage(ex.Message.Substring(6));

                    throw new InvalidPluginExecutionException(ex.Message.Substring(6));
                }

                runner.Logger.WriteToFile(ex.Message);

                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
        #endregion
    }
}

