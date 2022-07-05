using Microsoft.Xrm.Sdk;
using System;

namespace Va.Udo.Crm.Plugins.ITF
{
    public class ITFPostCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new ITFPostCreateRunner(serviceProvider);
            runner.Execute();
        }
    }
}
