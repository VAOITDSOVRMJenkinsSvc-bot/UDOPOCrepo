using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.Plugins
{
    public class DeactivateVassTagsHistoryOnDisaccociate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new DeactivateVassTagsHistoryOnDisaccociateRunner(serviceProvider);
            runner.Execute();
        }
    }
}
