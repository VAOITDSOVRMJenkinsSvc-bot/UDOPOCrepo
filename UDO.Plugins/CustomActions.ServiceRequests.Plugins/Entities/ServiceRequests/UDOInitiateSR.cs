using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.ServiceRequests.Plugins.Entities.ServiceRequests
{
    public class UDOInitiateSR : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOInitiateSRRunner(serviceProvider);
            runner.Execute();
        }
    }
}
