using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Denials.Messages;
using VRM.Integration.UDO.Denials.Processors;

namespace UDO.Crm.LOB.Controllers.Denials
{
    public class DenialsController : ApiController
    {
        // GET api/denials
        public IMessageBase Get([FromBody]UDOcreateDenialsRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOcreateDenialsProcessor();
                return processor.Execute((UDOcreateDenialsRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreateDenialsRequest msg = (UDOcreateDenialsRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_createDenials", msg.UserId, "UDOcreateDenialsMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOcreateDenialsMessageHandler Error: {0}", ex.Message), ex);
            }
        }
    }
}