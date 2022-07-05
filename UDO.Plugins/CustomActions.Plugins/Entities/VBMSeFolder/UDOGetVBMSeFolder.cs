using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.VBMSeFolder
{
    public class UDOGetVBMSeFolder : IPlugin
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetVBMSeFolderRunner(serviceProvider);
            runner.Execute();
        }
    }
}
