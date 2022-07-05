using System;
using Microsoft.Xrm.Sdk;


namespace Va.Udo.Crm.SpecialSituation.Plugins
{

    public class InteractionSpecialSituationPreCreateDelete : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new InteractionSpecialSituationPreCreateDeleteRunner(serviceProvider);
            runner.Execute();
        }
    }
}
