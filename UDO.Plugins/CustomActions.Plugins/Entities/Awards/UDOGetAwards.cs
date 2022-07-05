using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Awards
{
    public class UDOGetAwards : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetAwardsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
