using System;
using VEIS.Core.Configuration;
//using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.Servicebus.Bgs.Services
{
    [Serializable]
    public class BgsSecurityConfiguration : ObjectConfigHandler
    {
        public static BgsSecurityConfiguration Current
        {
            get
            {
                var customConfigurationFileReader = new CustomConfigurationFileReader();

                var section = customConfigurationFileReader.GetCustomConfig<BgsSecurityConfiguration>(ConfigurationLocation.ConfigDefiningAssemblyPath,
                    ConfigurationLocation.ConfigFilePath,
                    "BgsSecurityConfiguration");

                if (section == null)
                    throw new Exception("The BGS Security configuration section has not been specified");

                return section;
            }
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string ClientMachine { get; set; }
        public string StnId { get; set; }
        public string ApplicationId { get; set; }
        public bool RequiresClientCertificate { get; set; }
        public string ClientCertificateName { get; set; }
    }


    public class MyServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
