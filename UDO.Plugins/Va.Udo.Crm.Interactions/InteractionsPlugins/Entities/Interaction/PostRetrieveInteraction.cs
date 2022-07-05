namespace Va.Udo.Crm.Interactions.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;

    public class PostRetrieveInteraction : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new PostRetrieveInteractionRunner(serviceProvider);
            runner.Execute();
        }
    }
}
