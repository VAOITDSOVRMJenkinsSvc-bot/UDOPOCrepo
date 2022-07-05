using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.ClothingAllowance
{
    public class UDOGetClothingAllowance : IPlugin 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetClothingAllowanceRunner(serviceProvider);
            runner.Execute();
        }
    }
}
