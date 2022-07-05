using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using VEIS.Core.Wcf;

namespace VEIS.ReportServerReportExecution.Services
{
    public class ServiceCustomClientChannel<T> : CustomClientChannel<T>
    {
        public ServiceCustomClientChannel(string configurationPath) : base(configurationPath)
        {
        }

        public ServiceCustomClientChannel(Binding binding, string configurationPath) : base(binding, configurationPath)
        {
        }

        public ServiceCustomClientChannel(ServiceEndpoint serviceEndpoint, string configurationPath)
            : base(serviceEndpoint, configurationPath)
        {
        }

        public ServiceCustomClientChannel(string endpointConfigurationName, string configurationPath)
            : base(endpointConfigurationName, configurationPath)
        {
        }



        public ServiceCustomClientChannel(Binding binding, EndpointAddress endpointAddress, string configurationPath)
            : base(binding, endpointAddress, configurationPath)
        {
        }

        public ServiceCustomClientChannel(Binding binding, string remoteAddress, string configurationPath)
            : base(binding, remoteAddress, configurationPath)
        {
        }

        public ServiceCustomClientChannel(string endpointConfigurationName, EndpointAddress endpointAddress,
            string configurationPath) : base(endpointConfigurationName, endpointAddress, configurationPath)
        {
        }

        protected override void SetupClientCredentials()
        {
            if (ReportServerReportExecutionSecurityConfiguration.Current.RequiresClientCertificate)
            {
                Credentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine,
                    StoreName.My,
                    X509FindType.FindBySubjectName,
                    ReportServerReportExecutionSecurityConfiguration.Current.ClientCertificateName);
            }
        }
    }
}