using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace Va.Udo.Crm.UserTool.Plugins
{

    public class UDOSecurityPostUpdate : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrePaymentRetrieve"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOSecurityPostUpdateRunner(serviceProvider);
            runner.Execute(serviceProvider);
        }
    }
}
