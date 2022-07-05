
namespace UDO.Crm.LOB.Controllers.MilitaryService
{
    using Swashbuckle.Swagger.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using VRM.Integration.Servicebus.Core;
    using VRM.Integration.UDO.MilitaryService.Messages;
    using VRM.Integration.UDO.MilitaryService.Processors;

    public class MilitaryServiceController : ApiController
    {
        [SwaggerOperation("findMilitaryService")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        // GET api/MilitaryService
        public IMessageBase Get([FromBody]UDOfindMilitaryServiceRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOfindMilitaryServiceProcessor();
                return processor.Execute((UDOfindMilitaryServiceRequest)message);
            }
            catch (Exception ex)
            {
                UDOfindMilitaryServiceRequest msg = (UDOfindMilitaryServiceRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "UDOfindMilitaryServiceRequest", msg.UserId, "UDOfindMilitaryServiceRequest.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOfindMilitaryServiceRequest Error: {0}", ex.Message), ex);
            }
        }

        
    }
}