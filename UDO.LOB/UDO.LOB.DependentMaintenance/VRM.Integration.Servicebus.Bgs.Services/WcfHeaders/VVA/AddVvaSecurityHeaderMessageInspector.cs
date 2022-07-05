using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using VEIS.Core.Wcf;
//using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.Servicebus.Bgs.Services
{
    public class AddVvaSecurityHeaderMessageInspector : IClientMessageInspector, IDispatchMessageInspector
    {
        public AddVvaSecurityHeaderMessageInspector(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (SoapLog.Current.Active)
                SoapLog.Current.LogMessage(LogMessageType.Response,
                null,
                ref reply);
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var securityHeader = new CustomVvaSecurityHeader(Username, Password);

            request.Headers.Add(securityHeader);

            //Forego loggong of VVA requests until we can effectively debug SoapLog
            return null;

            if (SoapLog.Current.Active)
                SoapLog.Current.LogMessage(LogMessageType.Request, 
                channel.Via, 
                ref request);

            return null;
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {}
        
        public string Username { get; set; }
        public string Password { get; set; }
    }
}