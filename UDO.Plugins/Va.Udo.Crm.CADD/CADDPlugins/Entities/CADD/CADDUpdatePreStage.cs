using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;


namespace Va.Udo.Crm.CADD.Plugins
{

    public class CADDUpdatePreStage : IPlugin
    {
       
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new CADDUpdatePreStageRunner(serviceProvider);
            runner.Execute(serviceProvider);
        }
    }
}
