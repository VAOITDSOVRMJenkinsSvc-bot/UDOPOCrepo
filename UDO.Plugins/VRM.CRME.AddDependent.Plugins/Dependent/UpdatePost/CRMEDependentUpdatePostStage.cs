using Microsoft.Xrm.Sdk;
using System;

//	set your namespace below
namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class CRMEDependentUpdatePostStage : IPlugin
	{
		#region IPlugin Implementation
		public void Execute(IServiceProvider serviceProvider)
		{
            var runner = new CRMEDependentUpdatePostStageRunner(serviceProvider);

            runner.Execute(serviceProvider);
		}
		#endregion
	}
}
