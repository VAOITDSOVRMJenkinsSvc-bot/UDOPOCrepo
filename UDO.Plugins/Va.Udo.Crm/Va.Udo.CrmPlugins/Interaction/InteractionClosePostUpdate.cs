using Microsoft.Xrm.Sdk;
using System;

namespace Va.Udo.Crm.Plugins.Interaction
{
    public class InteractionClosePostUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            InteractionCloseRunner runner = new InteractionCloseRunner(serviceProvider);
            runner.Execute();
        }
    }
}
