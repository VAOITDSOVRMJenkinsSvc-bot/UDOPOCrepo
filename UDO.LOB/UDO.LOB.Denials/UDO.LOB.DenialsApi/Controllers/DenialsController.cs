using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Denials.Messages;
using UDO.LOB.Denials.Processors;

//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Denials.Messages;
//using VRM.Integration.UDO.Denials.Processors;

namespace UDO.LOB.Denials.Controllers
{
    public class DenialsController : ApiController
    {
        // POST api/denials
        [SwaggerOperation("createDenials")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createDenials")]
        public IMessageBase createDenials([FromBody] UDOcreateDenialsRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createDenials ");
            try
            {
                UDOcreateDenialsProcessor processor = new UDOcreateDenialsProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                LogHelper.LogError(request.OrganizationName, request.UserId, "CreateDenials", ex.Message);
                throw ex;
            }
            finally
            {
                //LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createDenials ");
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.createDenials", null, gwatch.ElapsedMilliseconds);
            }
        }

        [HttpGet]
        [HttpPost]
        [Route("api/ping")]
        [SwaggerOperation("ping")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult ping()
        {
            return Ok("pong");
        }

    }
}