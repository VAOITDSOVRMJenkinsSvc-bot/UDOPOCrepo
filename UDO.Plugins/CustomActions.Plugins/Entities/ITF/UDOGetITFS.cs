using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.ITF
{
    public class UDOGetITFS : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UDOGetITFS"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetITFRunner(serviceProvider);
            runner.Execute();
        }
    }
}
