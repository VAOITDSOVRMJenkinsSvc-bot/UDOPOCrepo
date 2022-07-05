using System;
using VEIS.Core.Configuration;
//using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.Servicebus.Bgs.Services
{
    [Serializable]
    public class VvaSecurityConfiguration : ObjectConfigHandler
    {
        public static VvaSecurityConfiguration Current
        {
            get
            {
                var customConfigurationFileReader = new CustomConfigurationFileReader();

                var section = customConfigurationFileReader.GetCustomConfig<VvaSecurityConfiguration>(ConfigurationLocation.ConfigDefiningAssemblyPath,
                    ConfigurationLocation.ConfigFilePath,
                    "VvaSecurityConfiguration");

                if (section == null)
                    throw new Exception("The VVA Security configuration section has not been specified");

                return section;
            }
        }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}