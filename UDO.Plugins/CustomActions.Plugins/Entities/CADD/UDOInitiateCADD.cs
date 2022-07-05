using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.CADD
{
    public class UDOInitiateCADD : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOInitiateCADDRunner(serviceProvider);
            runner.Execute();
        }
    }
}
