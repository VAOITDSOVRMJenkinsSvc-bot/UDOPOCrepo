using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Contact
{
    public class UDOGetContactUpdates : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetContactUpdatesRunner(serviceProvider);
            runner.Execute();
        }
    }
}
