using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.UserTool.Messages;
using VRM.Integration.UDO.UserTool.Processors;

namespace UDO.Crm.LOB.Controllers.UserTool
{
    public class SecurityController : ApiController
    {
        // GET api/security
        [SwaggerOperation("Associate")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("Associate")]
        public IMessageBase Associate([FromBody]UDOSecurityAssocRequest message)
        {
            try
            {
                // TODO: LogMessageReceipt(message);
                var processor = new UDOSecurityAssocProcessor();
                return processor.Execute((UDOSecurityAssocRequest)message);
            }
            catch (Exception ex)
            {
                var msg = (UDOSecurityAssocRequest)message;
                // LogHelper.LogError(msg.OrganizationName, msg.UserId, "UDOSecurityAssocMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOSecurityAssocMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        // GET api/security
        [SwaggerOperation("Disassociate")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("Disassociate")]
        public IMessageBase Disassociate([FromBody]UDOSecurityDisassocRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOSecurityDisassocProcessor();
                return processor.Execute(message);
            }
            catch (Exception ex)
            {
                var msg = (UDOSecurityDisassocRequest)message;
                // LogHelper.LogError(msg.OrganizationName, msg.UserId, "UDOSecurityDisassocMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOSecurityDisassocMessageHandler Error: {0}", ex.Message), ex);
            }
        }
    }
}