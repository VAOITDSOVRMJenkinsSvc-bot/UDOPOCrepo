using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UDO.LOB.Egain.Messages;
using UDO.LOB.Egain.Processor;
using VRM.Integration.Servicebus.Core;

namespace UDO.LOB.eGainApi.Controllers
{
    public class EgainController : ApiController
    {
        [SwaggerOperation("createAwards")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createAwards")]
        public void HandleAnonChatRequest([FromBody]  AnonChatRequest request)
        {
            var processor = new AnonChatRequestProcessor();
            processor.Execute(request);
        }
    }
}
