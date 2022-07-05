using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.MilitaryService
{

    public class UDOGetMilitaryService : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetMilitaryServiceRunner(serviceProvider);
            runner.Execute();
        }
    }
}
