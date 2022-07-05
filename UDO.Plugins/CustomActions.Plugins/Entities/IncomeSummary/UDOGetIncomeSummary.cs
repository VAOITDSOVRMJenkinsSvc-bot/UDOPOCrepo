using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.IncomSummary
{
    public class UDOGetIncomeSummary : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetIncomeSummaryRunner(serviceProvider);
            runner.Execute();
        }
    }
}
