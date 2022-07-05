using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Address
{
    public class ValidateAddress : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new ValidateAddressRunner(serviceProvider);
            runner.Execute();
        }
    }
}
