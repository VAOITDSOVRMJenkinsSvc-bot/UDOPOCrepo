using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace VRM.Integration.Servicebus.Bgs.Services
{
    public class AddVvaSecurityHeaderEndpointBehavior : IEndpointBehavior
    {
        private readonly string _UserName;
        private readonly string _Password;

        public AddVvaSecurityHeaderEndpointBehavior(string userName,
            string password)
        {
            _UserName = userName;
            _Password = password;
        }

        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var inspector =
                new AddVvaSecurityHeaderMessageInspector(_UserName,
                    _Password);

            clientRuntime.MessageInspectors.Add(inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint,
            EndpointDispatcher endpointDispatcher)
        { }

        public void Validate(ServiceEndpoint endpoint)
        { }
        #endregion
    }
}