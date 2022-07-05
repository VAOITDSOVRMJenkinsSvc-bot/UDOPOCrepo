using System;

using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.AwardLines
{
    public class UDOGetAwardLines : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetAwardLinesRunner(serviceProvider);
            runner.Execute();
        }
    }
}
