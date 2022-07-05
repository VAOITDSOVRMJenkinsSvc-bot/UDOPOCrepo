using Microsoft.Xrm.Sdk;
using System;

namespace Va.Udo.Crm.Plugins.Request
{
    /// <summary>
    /// Executes the RequestPreCreate runner.
    /// </summary>
    public class RequestPreCreate:IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            RequestPreCreateRuner runner = new RequestPreCreateRuner(serviceProvider);
            runner.TracingService.Trace("Execute");
            runner.Execute();
        }
    }
}
