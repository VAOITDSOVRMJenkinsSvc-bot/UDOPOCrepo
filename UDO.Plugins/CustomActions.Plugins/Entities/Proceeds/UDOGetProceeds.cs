using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Proceeds
{
    public class UDOGetProceeds : IPlugin
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetProceedsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
