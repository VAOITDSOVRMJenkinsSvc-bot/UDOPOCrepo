using System;
using Microsoft.Xrm.Sdk;


namespace Va.Udo.Crm.SpecialSituation.Plugins
{

    public class InteractionSpecialSituationPostDisassociate : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new InteractionSpecialSituationPostDisassociateRunner(serviceProvider);
            runner.Execute();
        }
    }
}
