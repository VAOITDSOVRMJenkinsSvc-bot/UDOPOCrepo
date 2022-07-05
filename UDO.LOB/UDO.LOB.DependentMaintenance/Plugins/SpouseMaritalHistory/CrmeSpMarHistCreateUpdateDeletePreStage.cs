using System;
using Microsoft.Xrm.Sdk;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class CrmeSpMarHistCRUDPreStage : IPlugin
    {
        #region IPlugin Implementation
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new CrmeSpMarHistCreateUpdateDeletePreStageRunner(serviceProvider);

            runner.Execute(serviceProvider);
        }
        #endregion
    }
}
