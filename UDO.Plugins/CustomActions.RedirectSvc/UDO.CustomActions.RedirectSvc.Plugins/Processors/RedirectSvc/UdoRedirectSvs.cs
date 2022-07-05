using System;
using Microsoft.Xrm.Sdk;

namespace UDO.CustomActions.RedirectSvc.Plugins.Processors.RedirectSvc
{
    public class UDORedirectSvc : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDORedirectSvcRunner(serviceProvider);
            runner.DoAction();
        }
    }
}
