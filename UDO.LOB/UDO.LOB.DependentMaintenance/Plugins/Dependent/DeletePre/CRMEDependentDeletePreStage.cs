using Microsoft.Xrm.Sdk;
using System;

//	set your namespace below
namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class CRMEDependentDeletePreStage : IPlugin
	{
		#region IPlugin Implementation
		public void Execute(IServiceProvider serviceProvider)
		{
            var runner = new CRMEDependentDeletePreStageRunner(serviceProvider);

			runner.Execute();
		}
		#endregion
	}
}
