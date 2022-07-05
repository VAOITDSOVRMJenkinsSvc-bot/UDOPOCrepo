using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Va.Udo.Crm.SSRS.Plugins
{
    public class GenerateDocumentPost : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateDocumentPostRunner"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new GenerateDocumentPostRunner(serviceProvider);
            runner.Execute();
        }
    }
}
