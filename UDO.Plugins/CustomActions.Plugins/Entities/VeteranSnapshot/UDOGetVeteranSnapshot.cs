using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.VeteranSnapshot
{
    public class UDOGetVeteranSnapshot : IPlugin
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetVeteranSnapshotRunner(serviceProvider);
            runner.Execute();
        }
    }
}
