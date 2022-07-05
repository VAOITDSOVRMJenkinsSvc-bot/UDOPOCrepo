using System;

using Microsoft.Xrm.Sdk;


namespace Va.Udo.Crm.VetHistory.Plugins
{

    public class ADRetrieveMultiplePre : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrePhoneCallRetrieve"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new ADRetrieveMultiplePreRunner(serviceProvider);
            runner.Execute();
        }
    }
}
