using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.ClaimEstablishment
{
    public class UDOInitiateClaimEstablishment : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOInitiateClaimEstablishmentRunner(serviceProvider);
            runner.Execute();
        }
    }
}
