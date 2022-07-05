/// <summary>
/// VirtualVA Api Controller
/// </summary>
namespace UDO.LOB.VirtualVA.Controllers
{
    using Swashbuckle.Swagger.Annotations;
    using System.Diagnostics;
    using System;
    using System.Net;
    using System.Web.Http;
    using UDO.LOB.Core;
    using UDO.LOB.VirtualVA.Messages;
    using UDO.LOB.VirtualVA.Processors;
    using UDO.LOB.Extensions.Logging;

    public class VirtualVAController : ApiController
    {
        /// <summary>
        /// Processor call for UDOcreateUDOVirtualVARequest
        /// </summary>
        /// <param name="request">UDOcreateUDOVirtualVARequest</param>
        /// <returns>UDOcreateUDOVirtualVAResponse</returns>
        // POST /api/virtualVA/createUDOVirtualVA
        [SwaggerOperation("createUDOVirtualVA")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createUDOVirtualVA")]
        public IMessageBase CreateVirtualVA([FromBody] UDOcreateUDOVirtualVARequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createUDOVirtualVA ");
            try
            {
                UDOcreateUDOVirtualVAProcessor processor = new UDOcreateUDOVirtualVAProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "CreateUDOVirtualVA", ex.Message);
                throw ex;
            }
            finally
            {
                //LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createAwards ");
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.createUDOVirtualVA", null, gwatch.ElapsedMilliseconds);
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