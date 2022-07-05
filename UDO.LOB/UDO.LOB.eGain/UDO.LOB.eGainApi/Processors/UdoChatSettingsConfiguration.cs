using System;
using System.Runtime.CompilerServices;
using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.Servicebus.Egain
{
	public class UdoChatSettingsConfiguration : ObjectConfigHandler
	{
		private readonly string _orgName;

		public UdoChatSettingsConfiguration Current
		{
			get
			{
				CustomConfigurationFileReader customConfigurationFileReader = new CustomConfigurationFileReader();
				UdoChatSettingsConfiguration section = customConfigurationFileReader.GetCustomConfig<UdoChatSettingsConfiguration>(ConfigurationLocation.get_ConfigDefiningAssemblyPath(), ConfigurationLocation.GetConfigFilePath(this._orgName), "UdoChatSettingsConfiguration");
				if (section == null)
				{
					throw new ApplicationException("The UdoChatSettingsConfiguration section could not be found");
				}
				return section;
			}
		}

		public string UdoChatOrg
		{
			get;
			set;
		}

		public UdoChatSettingsConfiguration(string orgName)
		{
			this._orgName = orgName;
		}

		public UdoChatSettingsConfiguration()
		{
		}
	}
}