using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Va.Udo.Crm.Documents.Plugins
{
    public class GenerateDocument : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new GenerateDocumentRunner(serviceProvider);
            runner.Execute();
        }
    }
}
