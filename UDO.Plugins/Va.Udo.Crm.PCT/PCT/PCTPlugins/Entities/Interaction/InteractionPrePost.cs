using System;
using Microsoft.Xrm.Sdk;


namespace Va.Udo.Crm.Queue.Plugins
{

    public class InteractionPrePost : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new InteractionPrePostRunner(serviceProvider);
            runner.Execute();
        }
    }
}
