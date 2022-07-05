using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Appeals
{
    public class UDOGetAppeals : IPlugin
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetAppealsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
