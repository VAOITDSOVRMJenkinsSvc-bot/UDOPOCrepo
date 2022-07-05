using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Dependents
{
    public class UDOGetDependentUpdates : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetDependentUpdatesRunner(serviceProvider);
            runner.Execute();
        }
    }
}
