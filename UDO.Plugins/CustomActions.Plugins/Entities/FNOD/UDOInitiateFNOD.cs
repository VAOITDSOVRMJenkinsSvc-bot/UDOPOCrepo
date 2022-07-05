using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.FNOD
{
    public class UDOInitiateFNOD : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOInitiateFNODRunner(serviceProvider);
            runner.Execute();
        }
    }
}
