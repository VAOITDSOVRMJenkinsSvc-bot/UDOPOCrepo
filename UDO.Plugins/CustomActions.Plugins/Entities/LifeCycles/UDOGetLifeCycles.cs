using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.LifeCycles
{
    public class UDOGetLifeCycles : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetLifeCyclesRunner(serviceProvider);
            runner.Execute();
        }
    }
}
