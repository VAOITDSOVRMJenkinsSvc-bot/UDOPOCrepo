using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;


namespace VRM.Integration.UDO.Notes.Plugins
{

    public class NotesRetrieveMultiplePost : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotesRetrieveMultiplePostRunner"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new NotesRetrieveMultiplePostRunner(serviceProvider);
            runner.Execute();
        }
    }
}
