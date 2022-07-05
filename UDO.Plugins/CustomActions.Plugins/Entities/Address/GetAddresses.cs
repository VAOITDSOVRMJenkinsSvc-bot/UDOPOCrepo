using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;


namespace CustomActions.Plugins.Entities.Address
{

    public class GetAddresses : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressRetrieveMultiplePreStage"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new GetAddressesRunner(serviceProvider);
            runner.Execute();
        }
    }
}
