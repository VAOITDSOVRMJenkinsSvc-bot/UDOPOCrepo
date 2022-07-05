
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UDO.LOB.ScheduledJob.Controllers
{
    using UDO.LOB.Core;
    using UDO.LOB.Extensions.Logging;
    using UDO.LOB.ScheduledJob.Messages;
    using UDO.LOB.ScheduledJob.Processors;
    using Swashbuckle.Swagger.Annotations;

    
    public class ScheduledJobController : ApiController
    {
        // POST /api/retrieveAwards
        [SwaggerOperation("scheduledJob")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("scheduledJob")]
        public IMessageBase Execute([FromBody]  UDOScheduledJobRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.scheduledJob ");
            try
            { 
            UDOScheduledJobProcessor processor = new UDOScheduledJobProcessor();
            return processor.Execute(request);
            }
            catch (Exception ex)
	        {
	            Trace.TraceError(ex.ToString());
	            throw ex;
	        }
	        finally
	        {
	            LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.scheduledJob ");
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
