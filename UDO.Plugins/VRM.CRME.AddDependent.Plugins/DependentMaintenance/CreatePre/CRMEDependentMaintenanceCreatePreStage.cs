using System;
using Microsoft.Xrm.Sdk;
    
namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class CRMECreatePreDependentMaintenance : IPlugin
    {
        #region IPlugin Implementation
        public void Execute(IServiceProvider serviceProvider)      
        {
			var runner = new CrmeCreatePreDependentMaintenanceRunner(serviceProvider);

            runner.Execute(serviceProvider);
        }
        #endregion
    }
}
