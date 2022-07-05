using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Va.Udo.Crm.eBenefits.Plugins
{
    public class eBenefitsCustomAction : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            EBenefitsCustomActionRunner runner = new EBenefitsCustomActionRunner(serviceProvider);
            runner.Execute();
        }
    }
}
