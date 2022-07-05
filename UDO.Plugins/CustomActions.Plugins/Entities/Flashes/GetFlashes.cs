using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;


namespace CustomActions.Plugins.Entities.Flashes
{

    public class GetFlashes : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostContactRetrieve"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new GetFlashesRunner(serviceProvider);
            runner.Execute();
            //throw new FaultException<OrganizationServiceFault>(null, "failure");
        }
    }
}
