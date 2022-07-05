using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.ITF
{
    public class UDOInitiateITF : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOInitiateITFRunner(serviceProvider);
            runner.Execute();
        }
    }
}
