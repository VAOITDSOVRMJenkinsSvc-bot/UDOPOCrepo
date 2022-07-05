using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using VEIS.Core.Wcf;

namespace VEIS.ReportServerReportExecution.Services
{
    internal class AddWseSecurityHeaderMessageInspector : IClientMessageInspector, IDispatchMessageInspector
    {
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