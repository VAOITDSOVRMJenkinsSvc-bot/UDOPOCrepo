using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Flashes.Messages;
using UDO.LOB.Flashes.Processors;

namespace UDO.LOB.Flashes.Controllers
{
    public class FlashesController : ApiController
    {
        // POST api/Flashes
        [SwaggerOperation("getFlashes")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("getFlashes")]
        public IMessageBase Post([FromBody] UDOgetFlashesRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.getFlashes ");
            try
            {
                UDOgetFlashesProcessor processor = new Processors.UDOgetFlashesProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "getFlashes", ex.Message);
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                //LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createAwards ");
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.getFlashes", null, gwatch.ElapsedMilliseconds);
            }
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