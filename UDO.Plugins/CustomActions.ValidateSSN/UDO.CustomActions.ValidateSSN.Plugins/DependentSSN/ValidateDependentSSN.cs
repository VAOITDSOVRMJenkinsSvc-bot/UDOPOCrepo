using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.CustomActions.ValidateSSN.Plugins.DependentSSN
{
    public class ValidateDependentSSN: IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new ValidateDependentSSNRunner(serviceProvider);
            runner.Execute();
        }
    }
}
