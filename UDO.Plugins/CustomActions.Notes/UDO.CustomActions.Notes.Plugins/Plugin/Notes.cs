using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.CustomActions.Notes.Plugins
{
    public class Notes : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new NotesRunner(serviceProvider);
            runner.Execute();
        }
    }
}
