using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Payments
{
    public class UDOGetPayments : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetPaymentsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
