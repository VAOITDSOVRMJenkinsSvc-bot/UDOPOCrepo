using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Fiduciaries
{
    public class GetFiduciaryExists : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostContactRetrieve"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new GetFiduciaryExistsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
