using System;
using Microsoft.Xrm.Sdk;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class CrmeDependentMaintenanceCreatePostStage : IPlugin
    {
        #region IPlugin Implementation
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new CrmeDependentMaintenanceCreatePostStageRunner(serviceProvider);

            runner.Execute(serviceProvider);
        }
        #endregion
    }
}
