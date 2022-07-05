using Microsoft.Xrm.Sdk;
using System;
using Va.Udo.Crm.Plugins.Common;

namespace Va.Udo.Crm.Plugins.Interaction
{
    /// <summary>
    /// Pre update plugin to update the duration of the interaction.
    /// </summary>
    public class InteractionDurationPreUpdate:IPlugin
    {
        /// <summary>
        /// Invokes the DurationUpdateRunner
        /// </summary>
        /// <param name="serviceProvider">Service provider passed by plugin execution engine</param>
        public void Execute(IServiceProvider serviceProvider)
        {
            DurationUpdateRunner runner = new DurationUpdateRunner(serviceProvider);
            runner.Execute();
        }
    }
}
