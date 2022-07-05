using System;
using Microsoft.Xrm.Sdk;


namespace Va.Udo.Crm.Request.Plugins
{

    public class RequestCreatePost : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new RequestCreatePostRunner(serviceProvider);
            runner.Execute();
        }
    }
}
