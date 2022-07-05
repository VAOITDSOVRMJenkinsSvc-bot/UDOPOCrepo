using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.CustomActions.LogtoAppInsights.Plugins.Plugin
{
    public class LogtoAppInsights : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new LogtoAppInsightsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
