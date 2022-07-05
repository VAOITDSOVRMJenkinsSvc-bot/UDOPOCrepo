using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using UDO.LOB.IDProofOrchestration.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.IDProofOrchestration.Processors;

namespace UDO.LOB.IDProofOrchestration.Controllers
{
    public class IDProofOrchestrationController : ApiController
    {
        // POST api/IDProofOrchestration/getUDOIDProofOrchestration
        [SwaggerOperation("getUDOIDProofOrchestration")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("getUDOIDProofOrchestration")]
        public IMessageBase POST([FromBody] UDOIDProofOrchestrationRequest request)
        {
            UDOIDProofOrchestrationProcessor processor = new UDOIDProofOrchestrationProcessor();
            return processor.Execute(request);
        }
        #region Ping Controller
        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/ping")]
        [SwaggerOperation("ping")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult ping()
        {
            return Ok("pong");
        }

        #endregion
    }
}
