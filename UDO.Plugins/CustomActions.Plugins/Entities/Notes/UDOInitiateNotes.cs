using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Notes
{
    public class UDOInitiateNotes : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOInitiateNotesRunner(serviceProvider);
            runner.Execute();
        }
    }
}
