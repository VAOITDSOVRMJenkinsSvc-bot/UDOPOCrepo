/// <summary>
/// Controller for VBMS eFolder Processors
/// </summary>
namespace UDO.LOB.VBMSeFolder.Controllers
{
    using Swashbuckle.Swagger.Annotations;
    using System.Net;
    using System.Web.Http;
    using UDO.LOB.Core;
    using UDO.LOB.VBMSeFolder.Messages;
    using UDO.LOB.VBMSeFolder.Processors;

    public class VBMSeFolderController : ApiController
    {
        // POST /api/vbmsefolder/getVBMSDocumentContent
        [SwaggerOperation("getVBMSDocumentContent")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("getVBMSDocumentContent")]
        public IMessageBase Post([FromBody]  UDOgetVBMSDocumentContentRequest request)
        {
            getVBMSDocumentContentProcessor processor = new getVBMSDocumentContentProcessor();
            return processor.Execute(request);
        }

        // POST /api/vbmsefolder/createVBMSeFolder
        [SwaggerOperation("createVBMSeFolder")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createVBMSeFolder")]
        public IMessageBase Post([FromBody]  UDOCreateVBMSeFolderRequest request)
        {
            UDOCreateVBMSeFolderProcessor processor = new UDOCreateVBMSeFolderProcessor();
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
