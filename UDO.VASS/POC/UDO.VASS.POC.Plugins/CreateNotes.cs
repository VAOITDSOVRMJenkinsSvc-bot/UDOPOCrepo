using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.POC.Plugins
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
