using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.EVRs
{
    public class UDOGetEVR : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetEVRRunner(serviceProvider);
            runner.Execute();
        }
    }
}
