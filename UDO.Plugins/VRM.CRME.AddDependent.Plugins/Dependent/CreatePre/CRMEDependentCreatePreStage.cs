using System;
using Microsoft.Xrm.Sdk;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class CrmeDependentCreatePreStage : IPlugin
    {
        #region IPlugin Implementation
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new CrmeDependentCreatePreStageRunner(serviceProvider);

            runner.Execute(serviceProvider);
        }
        #endregion
    }
}
