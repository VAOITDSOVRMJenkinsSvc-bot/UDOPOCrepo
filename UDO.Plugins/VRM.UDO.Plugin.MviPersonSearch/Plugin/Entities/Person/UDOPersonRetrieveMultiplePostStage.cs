using Microsoft.Xrm.Sdk;
using System;

//	set your namespace below
namespace VRM.UDO.MVI.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
	public class UDOPersonRetrieveMultiplePostStage : IPlugin
	{
		#region IPlugin Implementation
		public void Execute(IServiceProvider serviceProvider)
		{
			var runner = new UDOPersonRetrieveMultiplePostStageRunner(serviceProvider);

			runner.Execute();
		}
		#endregion
	}
}
