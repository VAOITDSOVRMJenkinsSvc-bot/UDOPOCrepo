using Microsoft.Xrm.Sdk;
using System;

namespace UDO.VASS.Plugins
{
    public class CreateNotes : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new CreateNotesRunner(serviceProvider);
            runner.Execute();
        }
    }
}
