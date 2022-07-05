using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;


namespace Va.Udo.Crm.eBenefits.Plugins
{

    public class IDProofCreatePostStage : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new IDProofCreatePostStageRunner(serviceProvider);
            runner.Execute();
        }
    }
}
