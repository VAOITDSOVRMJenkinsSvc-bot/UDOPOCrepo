using System;
using Microsoft.Xrm.Sdk;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class CrmeMaritalHistoryCRUDPreStage : IPlugin
    {
        #region IPlugin Implementation
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new CrmeMaritalHistoryCreateUpdateDeletePreStageRunner(serviceProvider);

            runner.Execute(serviceProvider);
        }
        #endregion
    }
}
