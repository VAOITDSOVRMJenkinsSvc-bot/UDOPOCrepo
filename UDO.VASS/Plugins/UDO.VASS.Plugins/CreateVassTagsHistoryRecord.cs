using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.Plugins
{
    public class CreateVassTagsHistoryRecord : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new CreateVassTagsHistoryRecordRunner(serviceProvider);
            runner.Execute();
        }
    }
}
