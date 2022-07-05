using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.ClaimEstablishment
{
    public class UDOInsertClaimEstablishment : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOInsertClaimEstablishmentRunner(serviceProvider);
            runner.Execute();
        }
    }
}
