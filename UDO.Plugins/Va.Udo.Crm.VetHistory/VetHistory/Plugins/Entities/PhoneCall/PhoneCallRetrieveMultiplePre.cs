using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;



namespace Va.Udo.Crm.VetHistory.Plugins
{

    public class PhoneCallRetrieveMultiplePre : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrePhoneCallRetrieve"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new PhoneCallRetrieveMultiplePreRunner(serviceProvider);
            runner.Execute();
        }
    }
}
