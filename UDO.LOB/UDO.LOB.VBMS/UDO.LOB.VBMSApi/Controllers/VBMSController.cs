using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UDO.LOB.Core;
using UDO.LOB.VBMS.Messages;
using UDO.LOB.VBMS.Processors;

namespace UDO.LOB.VBMS.Controllers
{
    public class VBMSController : ApiController
    {
        // POST api/VBMS/UDOVBMSUploadDocument
        [SwaggerOperation("UDOVBMSUploadDocument")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [ActionName("UDOVBMSUploadDocument")]
        [HttpPost]
        public IMessageBase UDOVBMSUploadDocument([FromBody]UDOVBMSUploadDocumentRequest message)
        {
            UDOVBMSUploadDocumentProcessor processor = new UDOVBMSUploadDocumentProcessor();
            return processor.Execute(message);
        }

        // POST api/VBMS/UDOVBMSUploadDocumentAsync
        [SwaggerOperation("UDOVBMSUploadDocumentAsync")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [ActionName("UDOVBMSUploadDocumentAsync")]
        [HttpPost]
        public IMessageBase UDOVBMSUploadDocumentAsync([FromBody]UDOVBMSUploadDocumentAsyncRequest message)
        {
            UDOVBMSUploadDocumentAsyncProcessor processor = new UDOVBMSUploadDocumentAsyncProcessor();
            return processor.Execute(message);
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
