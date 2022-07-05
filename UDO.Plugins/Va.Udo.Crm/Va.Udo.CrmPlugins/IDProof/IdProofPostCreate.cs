using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Va.Udo.Crm.Plugins.IDProof
{

    public class IdProofPostCreate : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostContactRetrieve"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new IdProofPostCreateRunner(serviceProvider);
            runner.Execute();
        }
    }
}
