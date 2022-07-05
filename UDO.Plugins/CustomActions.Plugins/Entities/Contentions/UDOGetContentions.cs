using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Contentions
{
    public class UDOGetContentions : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetContentionsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
