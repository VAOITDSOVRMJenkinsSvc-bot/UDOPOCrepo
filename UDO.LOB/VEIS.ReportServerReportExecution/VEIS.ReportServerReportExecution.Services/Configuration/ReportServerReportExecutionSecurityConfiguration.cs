using System;
using VEIS.Core.Configuration;

namespace VEIS.ReportServerReportExecution.Services
{
    [Serializable]
    public class ReportServerReportExecutionSecurityConfiguration : ObjectConfigHandler
    {
        public static ReportServerReportExecutionSecurityConfiguration Current
        {
            get
            {
                var customConfigurationFileReader = new CustomConfigurationFileReader();

                var section = customConfigurationFileReader.GetCustomConfig<ReportServerReportExecutionSecurityConfiguration>(ConfigurationLocation.ConfigDefiningAssemblyPath,
                     ConfigurationLocation.GetConfigFilePath("EC"),
                    "ReportServerReportExecutionSecurityConfiguration");

                if (section == null)
                    throw new Exception("The Report Server Web Service Security configuration section has not been specified");

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
        public bool EnableLogging { get; set; }
    }


    public class MyServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
