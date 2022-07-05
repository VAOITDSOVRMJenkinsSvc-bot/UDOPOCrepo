using System;
using Microsoft.Xrm.Sdk;

namespace VRM.Integration.UDO.VBMS.Plugins
{

    public class CreateAnnotationPre : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrePaymentRetrieve"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new CreateAnnotationPreRunner(serviceProvider);
            runner.Execute();
        }
    }
}
