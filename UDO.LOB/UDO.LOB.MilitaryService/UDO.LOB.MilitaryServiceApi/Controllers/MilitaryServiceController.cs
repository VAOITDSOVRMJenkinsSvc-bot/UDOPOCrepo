using Swashbuckle.Swagger.Annotations;
using System;
using System.Diagnostics;
using System.Net;
using System.Web.Http;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MilitaryService.Messages;
using UDO.LOB.MilitaryService.Processors;

namespace UDO.LOB.MilitaryService.Controllers
{
    
    public class MilitaryServiceController : ApiController
    {
        [SwaggerOperation("findMilitaryService")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("findMilitaryService")]
        [HttpPost]
        // POST api/MilitaryService/findMilitaryService
        public IMessageBase FindMilitaryService([FromBody]UDOfindMilitaryServiceRequest message)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.findMilitaryService ");
            try
            {
                var processor = new UDOfindMilitaryServiceProcessor();
                return processor.Execute((UDOfindMilitaryServiceRequest)message);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.findMilitaryService ");
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