using System;
using Microsoft.Xrm.Sdk;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class CrmeDependentCreatePostStage : IPlugin
    {
        #region IPlugin Implementation
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new CrmeDependentCreatePostStageRunner(serviceProvider);
            
            runner.Execute(serviceProvider);
        }
        #endregion
    }
}
