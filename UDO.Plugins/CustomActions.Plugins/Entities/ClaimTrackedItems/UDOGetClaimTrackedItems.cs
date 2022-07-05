using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.ClaimsTrackedItems
{
    public class UDOGetClaimTrackedItems : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetClaimTrackedItemsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
