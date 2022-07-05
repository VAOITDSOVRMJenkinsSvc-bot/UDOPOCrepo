using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.MVI.Messages;
using VRM.Integration.UDO.MVI.Processors;

namespace UDO.Crm.LOB.MVIApi.Controllers
{
    public class PersonSearchController : ApiController
    {

        // POST api/PersonSearch
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IMessageBase Get([FromBody]UDOPersonSearchRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOPersonSearchProcessor();
                return processor.Execute((UDOPersonSearchRequest)message);
            }
            catch (Exception ex)
            {
                UDOPersonSearchRequest msg = (UDOPersonSearchRequest)message;
                // LogHelper.LogError(msg.OrganizationName, msg.UserId, "UDOPersonSearchMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOPersonSearchMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        
    }
}
