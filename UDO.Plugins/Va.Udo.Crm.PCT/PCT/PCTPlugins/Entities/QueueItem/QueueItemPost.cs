using System;
using Microsoft.Xrm.Sdk;


namespace Va.Udo.Crm.Queue.Plugins
{

    public class QueueItemPost : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new QueueItemPostRunner(serviceProvider);
            runner.Execute();
        }
    }
}
