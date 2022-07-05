using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.Plugins
{
    public class TeamAssignmentUsingUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new TeamAssignmentUsingUpdateRunner(serviceProvider);
            runner.Execute();
        }
    }
}
