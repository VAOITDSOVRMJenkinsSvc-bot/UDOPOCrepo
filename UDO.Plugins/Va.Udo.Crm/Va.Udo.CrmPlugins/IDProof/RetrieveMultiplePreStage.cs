using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;


namespace Va.Udo.Crm.Plugins.RetrieveMultiple
{

    public class RetrieveMultiplePreStage : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        
        private string _unSecure;
        public RetrieveMultiplePreStage(string unSecure, string Secure)
        {
            _unSecure = unSecure;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new RetrieveMultiplePreStageRunner(serviceProvider);
            runner.Execute(_unSecure);
        }
    }
}
