using Microsoft.Xrm.Sdk;
using System;

namespace Va.Udo.Crm.Plugins.Person
{
    public class PersonFiduciaryExists : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new PersonFiduciaryExistsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
