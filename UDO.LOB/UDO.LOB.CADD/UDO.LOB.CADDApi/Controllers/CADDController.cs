/// <summary>
/// UDO.LOB.CADD.Controllers
/// </summary>
namespace UDO.LOB.CADD.Controllers
{
    using System.Net;
    using System.Web.Http;
    using Swashbuckle.Swagger.Annotations;
    using UDO.LOB.CADD.Messages;
    using UDO.LOB.CADD.Processors;
    using UDO.LOB.Core;

    public class CADDController : ApiController
    {
        [SwaggerOperation("UDOFindBank")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("UDOFindBank")]
        // POST api/CADD
        public IMessageBase UDOFindBank([FromBody]UDOFindBankRequest request)
        {
            UDOFindBankProcessor processor = new UDOFindBankProcessor();
            return processor.Execute(request);
            
        }

        [SwaggerOperation("UDOInitiateCADD")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("UDOInitiateCADD")]
        // POST api/CADD
        public IMessageBase UDOInitiateCADD([FromBody]UDOInitiateCADDRequest request)
        {
            UDOInitiateCADDProcessor processor = new UDOInitiateCADDProcessor();
            return processor.Execute(request);
        }

        [SwaggerOperation("UDOupdateCADDAddress")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("UDOupdateCADDAddress")]
        // POST api/CADD/UDOupdateCADDAddress
        public IMessageBase UDOupdateCADDAddress([FromBody]UDOupdateCADDAddressRequest request)
        {
            UDOupdateCADDAddressProcessor processor = new UDOupdateCADDAddressProcessor();
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