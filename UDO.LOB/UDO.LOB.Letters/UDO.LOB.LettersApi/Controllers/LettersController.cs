/// <summary>
/// LettersController
/// </summary>
namespace UDO.LOB.Letters.Controllers
{
    using Swashbuckle.Swagger.Annotations;
    using System.Net;
    using System.Web.Http;
    using UDO.LOB.Core;
    using UDO.LOB.Letters.Messages;
    using UDO.LOB.Letters.Processors;

    public class LettersController : ApiController
    {
        // POST api/Letters/initiateLetters
        [SwaggerOperation("initiateLetters")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("initiateLetters")]
        public IMessageBase Post([FromBody]  UDOInitiateLettersRequest request)
        {
            UDOInitiateLettersProcessor processor = new UDOInitiateLettersProcessor();
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
