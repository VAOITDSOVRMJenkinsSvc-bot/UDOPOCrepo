using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Letters
{
    public class UDOInitiateLetter : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOInitiateLetterRunner(serviceProvider);
            runner.Execute();
        }
    }
}
