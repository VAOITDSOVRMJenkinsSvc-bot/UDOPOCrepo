//Ratings
using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Ratings
{
    public class UDOGetRatings : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetRatingsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
