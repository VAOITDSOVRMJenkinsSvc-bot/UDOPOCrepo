using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.ITF
{
    public class UDOSubmitITF : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOSubmitITFRunner(serviceProvider);
            runner.Execute();
        }
    }
}
