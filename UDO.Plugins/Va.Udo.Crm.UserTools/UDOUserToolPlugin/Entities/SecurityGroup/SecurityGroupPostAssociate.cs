using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;



namespace Va.Udo.Crm.UserTool.Plugins
{

    public class SecurityGroupPostAssociate : IPlugin
    {
                /// <summary>
        /// Initializes a new instance of the <see cref="PrePaymentRetrieve"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new SecurityGroupPostAssociateRunner(serviceProvider);

            runner.Execute();
        }
    }
}
