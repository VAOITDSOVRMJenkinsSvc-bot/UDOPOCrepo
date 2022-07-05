using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace VRM.Integration.Servicebus.Bgs.Services
{
    public class AddWseSecurityHeaderEndpointBehavior : IEndpointBehavior
    {
        private readonly string _UserName;
        private readonly string _Password;
        private readonly string _ClientMachine;
        private readonly string _StnId;
        private readonly string _ApplicationId;

        public AddWseSecurityHeaderEndpointBehavior(string userName, 
            string password,
            string clientMachine,
            string stnId,
            string applicationId)
        {
            _UserName = userName;
            _Password = password;
            _ClientMachine = clientMachine;
            _StnId = stnId;
            _ApplicationId = applicationId;
        }

        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var inspector =
                new AddWseSecurityHeaderMessageInspector(_UserName, 
                    _Password, 
                    _ClientMachine, 
                    _StnId, 
                    _ApplicationId);

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