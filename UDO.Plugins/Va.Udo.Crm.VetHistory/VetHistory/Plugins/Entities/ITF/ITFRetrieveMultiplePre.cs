using System;

using Microsoft.Xrm.Sdk;


namespace Va.Udo.Crm.VetHistory.Plugins
{

    public class ITFRetrieveMultiplePre: IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrePhoneCallRetrieve"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new ITFRetrieveMultiplePreRunner(serviceProvider);
            runner.Execute();
        }
    }
}
