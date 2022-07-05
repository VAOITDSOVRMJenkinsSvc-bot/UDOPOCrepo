using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.BIRLS
{

    public class BIRLSCustomAction     : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostContactRetrieve"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new BIRLSCARunner(serviceProvider);
            runner.Execute();
        }
    }
}
