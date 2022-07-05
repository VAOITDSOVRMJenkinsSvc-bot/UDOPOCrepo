using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
//using VRM.Integration.Servicebus.Core;
using VEIS.Core;
using VEIS.Core.Wcf;

namespace VRM.Integration.Servicebus.Bgs.Services
{
    internal class AddWseSecurityHeaderMessageInspector : IClientMessageInspector, IDispatchMessageInspector
    {
        private readonly string _UserName;
        private readonly string _Password;
        private readonly string _ClientMachine;
        private readonly string _StnId;
        private readonly string _ApplicationId;

        public AddWseSecurityHeaderMessageInspector(string userName, 
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

        #region IClientMessageInspector Members

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (SoapLog.Current.Active)
                SoapLog.Current.LogMessage(LogMessageType.Response,
                    null,
                    ref reply);
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var securityHeader =
                new CustomWseSecurityHeader(_UserName, 
                    _Password, 
                    _ClientMachine, 
                    _StnId, 
                    _ApplicationId);

            request.Headers.Add(securityHeader);

            if (SoapLog.Current.Active)
                SoapLog.Current.LogMessage(LogMessageType.Request, 
                    channel.Via, 
                    ref request);

            return request;
        }

        #endregion

        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref Message request, IClientChannel channel,
            InstanceContext instanceContext)
        {
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        { }

        #endregion
    }
}