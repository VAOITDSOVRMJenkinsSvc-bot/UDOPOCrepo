using Microsoft.Xrm.Sdk;
using System;

namespace Va.Udo.Crm.Plugins.Veteran
{
    public class VeteranPreUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new VeteranRunner(serviceProvider);
            runner.Execute();
        }
    }
}
