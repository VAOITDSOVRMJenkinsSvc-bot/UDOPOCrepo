using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Receivables
{
    public class UDOGetReceivables : IPlugin
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetReceivablesRunner(serviceProvider);
            runner.Execute();
        }
    }
}
