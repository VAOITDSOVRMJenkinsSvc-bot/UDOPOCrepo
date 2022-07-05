using Microsoft.Xrm.Sdk;
using System;


namespace Va.Udo.Crm.Plugins.Request
{
    public class RequestPostCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            RequestPostCreateRunner runner = new RequestPostCreateRunner(serviceProvider);
            runner.TracingService.Trace("Execute");
            runner.Execute();
        }
    }
}
