using Microsoft.Xrm.Sdk;
using System;

namespace Va.Udo.Crm.Plugins.Security
{
    public class VeteranAgentFilter :IPlugin
    {
         private string _unSecure;
         public VeteranAgentFilter(string unSecure, string Secure)
        {
            _unSecure = unSecure;
        }
        public void Execute(IServiceProvider serviceProvider)
        {
           
            VeteranAgentFilterRunner runner = new VeteranAgentFilterRunner(serviceProvider);
            runner.Execute(_unSecure);
          
        }
    }
}
