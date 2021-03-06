using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Denials
{
    public class UDOGetDenials : IPlugin
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetDenialsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
