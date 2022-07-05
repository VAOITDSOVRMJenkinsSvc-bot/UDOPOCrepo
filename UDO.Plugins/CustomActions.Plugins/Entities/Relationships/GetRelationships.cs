using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;


namespace CustomActions.Plugins.Entities.Relationships
{

    public class GetRelationships: IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostContactRetrieve"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new GetRelationshipsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
