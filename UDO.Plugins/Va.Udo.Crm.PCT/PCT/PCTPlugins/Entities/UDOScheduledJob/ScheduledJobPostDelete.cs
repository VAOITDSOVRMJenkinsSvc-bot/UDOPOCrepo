using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;


namespace VRM.Integration.ScheduledJob
{

    public class ScheduledJobPostDelete : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrePaymentRetrieve"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new ScheduledJobPostDeleteRunner(serviceProvider);
            runner.Execute();
        }
    }
}
