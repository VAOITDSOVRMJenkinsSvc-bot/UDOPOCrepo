using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using VEIS.Core.Wcf;
//using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.Servicebus.Bgs.Services
{
    public class BgsCustomClientChannel<T> : CustomClientChannel<T>
    {
        public BgsCustomClientChannel(string configurationPath) : base(configurationPath)
        {
        }

        public BgsCustomClientChannel(Binding binding, string configurationPath) : base(binding, configurationPath)
        {
        }

        public BgsCustomClientChannel(ServiceEndpoint serviceEndpoint, string configurationPath)
            : base(serviceEndpoint, configurationPath)
        {
        }

        public BgsCustomClientChannel(string endpointConfigurationName, string configurationPath)
            : base(endpointConfigurationName, configurationPath)
        {
        }



        public BgsCustomClientChannel(Binding binding, EndpointAddress endpointAddress, string configurationPath)
            : base(binding, endpointAddress, configurationPath)
        {
        }

        public BgsCustomClientChannel(Binding binding, string remoteAddress, string configurationPath)
            : base(binding, remoteAddress, configurationPath)
        {
        }

        public BgsCustomClientChannel(string endpointConfigurationName, EndpointAddress endpointAddress,
            string configurationPath) : base(endpointConfigurationName, endpointAddress, configurationPath)
        {
        }

        protected override void SetupClientCredentials()
        {
			//if (BgsSecurityConfiguration.Current.RequiresClientCertificate)
			//{
			//    Credentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine,
			//        StoreName.My,
			//        X509FindType.FindBySubjectName,
			//        BgsSecurityConfiguration.Current.ClientCertificateName);
			//}

			//CSDev
			if (BgsSecurityConfiguration.Current.RequiresClientCertificate)
			{
				Credentials.ClientCertificate.SetCertificate(StoreLocation.CurrentUser,
					StoreName.My,
					X509FindType.FindByThumbprint,
					BgsSecurityConfiguration.Current.ClientCertificateName);
			}
		}
    }
}