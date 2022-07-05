/// <summary>
/// ClaimDocumentsController
/// </summary>
namespace UDO.LOB.ClaimDocuments.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Web.Http;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions.Logging;
    using UDO.LOB.ClaimDocuments.Messages;
    using UDO.LOB.ClaimDocuments.Processors;
    using System.Net;
    using Swashbuckle.Swagger.Annotations;

    public class ClaimDocumentsController : ApiController
    {
        [SwaggerOperation("findUDOClaimDocuments")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("findUDOClaimDocuments")]
        // GET api/ClaimDocuments
        public IMessageBase FindUDOClaimDocuments([FromBody] getUDOClaimDocumentsRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.FindUDOClaimDocuments ");
            try
            {
                GetUDOClaimDocumentsProcessor processor = new GetUDOClaimDocumentsProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                LogHelper.LogError(request.OrganizationName, request.UserId, "FindUDOClaimDocuments", ex.Message);
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.FindUDOClaimDocuments", null, gwatch.ElapsedMilliseconds);
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