using System;
using System.ComponentModel.Composition;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Claims.Messages;
using VRM.Integration.UDO.Claims.Processors;

namespace VRM.Integration.UDO.Claims.MessageHandler
{
    [Export(typeof(IMessageHandler))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOMAPDLettersRequest)]
    public class UDOcreateUDOMAPDLettersMessageHandler : RequestResponseHandler
    {
        public override IMessageBase HandleRequestResponse(object message)
        {
            try
            {
                LogMessageReceipt(message);
                var processor = new createUDOMAPDLettersProcessor();
                return processor.Execute((UDOcreateUDOMAPDLettersRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreateUDOMAPDLettersRequest msg = (UDOcreateUDOMAPDLettersRequest)message;
                LogHelper.LogError(msg.OrganizationName, "mcs_createUDOMAPDLetters", msg.UserId, "UDOcreateUDOMAPDLettersMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOcreateUDOMAPDLettersMessageHandler Error: {0}", ex.Message), ex);
            }
        }
    }
}