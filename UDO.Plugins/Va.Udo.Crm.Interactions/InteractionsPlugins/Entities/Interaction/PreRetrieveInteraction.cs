namespace Va.Udo.Crm.Interactions.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;

    public class PreRetrieveInteraction : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new PreRetrieveInteractionRunner(serviceProvider);
            runner.Execute();
        }
    }
}
