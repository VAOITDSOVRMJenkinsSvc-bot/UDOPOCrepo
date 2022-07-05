
namespace UDO.LOB.BIRLSApi.Controllers
{
    using Swashbuckle.Swagger.Annotations;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Web.Http;
    using UDO.LOB.BIRLS.Messages;
    using UDO.LOB.BIRLSApi.Processors;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions.Logging;

    public class BIRLSController : ApiController
    {
        [SwaggerOperation("getBIRLS")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("getBIRLS")]
        public IMessageBase RetrieveBIRLS([FromBody] UDOgetBIRLSDataRequest message)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.RetrieveBIRLS ");
            try
            {
                var processor = new UDOgetBIRLSDataProcessor();
                return processor.Execute((UDOgetBIRLSDataRequest)message);
            }
            catch (Exception ex)
            {
                UDOgetBIRLSDataRequest msg = (UDOgetBIRLSDataRequest)message;
                LogHelper.LogError(msg.OrganizationName, msg.UserId, msg.MessageId, $"{this.GetType().FullName}.RetrieveBIRLS", ex);
                throw new Exception(string.Format("UDOgetBIRLSDataMessageHandler Error: {0}", ex.Message), ex);
            }
            finally
            {
                LogHelper.LogTiming(message.MessageId, message.OrganizationName, message.LogTiming, message.UserId, message.RelatedParentId, message.RelatedParentEntityName, message.RelatedParentFieldName
                , $"<< Exited {this.GetType().FullName}.getBIRLS", null, gwatch.ElapsedMilliseconds);
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
            try
            {
                return Ok("pong");
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("ping Error: {0}", ex.Message), ex);
            }
        }
        #endregion
    }
}