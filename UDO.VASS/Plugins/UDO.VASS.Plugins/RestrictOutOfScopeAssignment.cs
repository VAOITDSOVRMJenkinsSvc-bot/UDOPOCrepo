using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.Plugins
{
    public class RestrictOutOfScopeAssignment : IPlugin
    {
        private readonly string teamName;
        public RestrictOutOfScopeAssignment(string unsecureString)
        {
            teamName = unsecureString;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new RestrictOutOfScopeAssignmentRunner(serviceProvider);

            runner.Execute(teamName);
        }

        
    }
}
