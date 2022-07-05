using System;
using Microsoft.Xrm.Sdk;


namespace Va.Udo.Crm.Queue.Plugins
{

    public class InteractionPreAssign : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new InteractionPreAssignRunner(serviceProvider);
            runner.Execute();
        }
    }
}
