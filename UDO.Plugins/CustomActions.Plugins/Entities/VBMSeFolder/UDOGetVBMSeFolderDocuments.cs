using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.VBMSeFolder
{
    public class UDOGetVBMSeFolderDocuments : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetVBMSeFolderDocumentsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
