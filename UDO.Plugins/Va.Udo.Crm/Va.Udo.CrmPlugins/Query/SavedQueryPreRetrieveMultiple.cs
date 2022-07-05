using Microsoft.Xrm.Sdk;
using System;

namespace Va.Udo.Crm.Plugins
{
    /// <summary>
    /// Assign records to the default team.
    /// </summary>
    public class SavedQueryPreRetrieveMultiple : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            SavedQueryPreRetrieveMultipleRunner runner = new SavedQueryPreRetrieveMultipleRunner(serviceProvider);
            runner.Execute();
            
        }
    }
}
