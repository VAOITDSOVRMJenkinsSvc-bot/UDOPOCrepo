using System;

using Microsoft.Xrm.Sdk;


namespace Va.Udo.Crm.VetHistory.Plugins
{

    public class RequestRetrieveMultiplePre : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrePhoneCallRetrieve"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new RequestRetrieveMultiplePreRunner(serviceProvider);
            runner.Execute();
        }
    }
}
