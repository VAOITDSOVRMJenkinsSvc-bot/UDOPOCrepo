using System;
//using System.ServiceModel;
using Microsoft.Xrm.Sdk;


namespace VRM.Integration.UDO.Notes.Plugins
{

    public class CreateNotesPre : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrePaymentRetrieve"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new CreateNotesPreRunner(serviceProvider);
            runner.Execute(serviceProvider);
        }
    }
}
