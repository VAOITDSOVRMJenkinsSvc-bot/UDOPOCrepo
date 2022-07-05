using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.Plugins
{
    public class CallAttemptValidation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new CallAttemptValidationRunner(serviceProvider);
            runner.Execute();
        }
    }
}
