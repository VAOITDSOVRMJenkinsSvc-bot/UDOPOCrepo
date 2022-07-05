using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.ClaimEstablishment
{
    public class UDOClearClaimEstablishment : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOClearClaimEstablishmentRunner(serviceProvider);
            runner.Execute();
        }
    }
}
