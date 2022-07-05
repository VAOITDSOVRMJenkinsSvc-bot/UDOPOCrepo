using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.PersonSearch
{
    public class UDOPersonSearch : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOPersonSearchRunner(serviceProvider);
            runner.Execute();
        }
    }
}
