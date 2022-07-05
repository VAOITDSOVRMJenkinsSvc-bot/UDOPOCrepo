using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.VirtualVA
{
    public class UDOGetVirtualVA : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetVirtualVARunner(serviceProvider);
            runner.Execute();
        }
    }
}
