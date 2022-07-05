using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.LegacyPayments
{
    public class UDOGetLegacyPayments : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetLegacyPaymentsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
