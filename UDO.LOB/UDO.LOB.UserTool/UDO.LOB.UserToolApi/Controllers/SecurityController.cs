
namespace UDO.LOB.UserTool.Controllers
{
    using Swashbuckle.Swagger.Annotations;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Web.Http;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions.Logging;
    using UDO.LOB.UserTool.Messages;
    using UDO.LOB.UserTool.Processors;

    public class SecurityController : ApiController
    {
        // GET api/security/Associate
        [SwaggerOperation("Associate")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("Associate")]
        public IMessageBase Associate([FromBody] UDOSecurityAssocRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "SecurityController", $">> Entered {this.GetType().FullName}.Associate ", request.Debug);
            try
            {
                UDOSecurityAssocProcessor processor = new UDOSecurityAssocProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, $"#ERROR in {GetType().FullName}.SecurityController", ex);
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.SecurityController", null, gwatch.ElapsedMilliseconds);
            }
        }

        // GET api/security/Disassociate
        [SwaggerOperation("Disassociate")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("Disassociate")]
        public IMessageBase Disassociate([FromBody] UDOSecurityDisassocRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "SecurityController", $">> Entered {this.GetType().FullName}.Disassociate ", request.Debug);
            try
            {
                UDOSecurityDisassocProcessor processor = new UDOSecurityDisassocProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, $"#ERROR in {GetType().FullName}.Disassociate", ex);
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.Disassociate", null, gwatch.ElapsedMilliseconds);

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