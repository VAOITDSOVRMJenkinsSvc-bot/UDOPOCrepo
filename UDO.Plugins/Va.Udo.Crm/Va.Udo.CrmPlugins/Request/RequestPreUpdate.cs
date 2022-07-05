using Microsoft.Xrm.Sdk;
using System;
using Va.Udo.Crm.Plugins.Common;

namespace Va.Udo.Crm.Plugins
{
    /// <summary>
    /// Invoked on the update of end time.
    /// </summary>
    public class RequestPreUpdate:IPlugin
    {
        /// <summary>
        /// Invokes the Duration update runner for request
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void Execute(IServiceProvider serviceProvider)
        {
            DurationUpdateRunner runner = new DurationUpdateRunner(serviceProvider);
            runner.Execute();
        }
    }
}
