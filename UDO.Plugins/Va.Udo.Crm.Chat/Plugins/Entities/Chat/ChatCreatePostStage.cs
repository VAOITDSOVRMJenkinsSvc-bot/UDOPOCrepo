using Microsoft.Xrm.Sdk;
using System;

//	set your namespace below
namespace Va.Udo.Crm.Chat.Plugins
{
	public class ChatCreatePostStage : IPlugin
	{
		#region IPlugin Implementation
		public void Execute(IServiceProvider serviceProvider)
		{
			var runner = new Plugins.ChatCreatePostStageRunner(serviceProvider);

			runner.Execute();
		}
		#endregion
	}
}
