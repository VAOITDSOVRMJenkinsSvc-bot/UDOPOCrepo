using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.ClaimDocs
{
    public class UDOGetClaimDocuments : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetClaimDocumentsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
