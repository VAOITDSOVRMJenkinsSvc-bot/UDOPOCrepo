using Swashbuckle.Swagger.Annotations;
using System;
using System.Diagnostics;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using UDO.LOB.Core;
using UDO.LOB.ServiceRequest.Messages;
using UDO.LOB.ServiceRequest.Processors;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.ServiceRequest.Controllers
{
    public class ServiceRequestController : ApiController
    {
        [SwaggerOperation("initiateSR")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [System.Web.Http.ActionName("initiateSR")]
        public IMessageBase InitiateSR([FromBody] UDOInitiateSRRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.initiateSR ");
            try
            {
                UDOInitiateSRProcessor processor = new UDOInitiateSRProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "initiateSR", ex.Message);
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.initiateSR", null, gwatch.ElapsedMilliseconds);
            }
        }

        [SwaggerOperation("updateSR")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [System.Web.Http.ActionName("updateSR")]
        public IMessageBase UpdateSR([Bind(Include = "udo_ServiceRequestId")] UDOUpdateSRRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.updateSR ");
            try
            {
                UDOUpdateSRProcessor processor = new UDOUpdateSRProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "updateSR", ex.Message);
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                                    , $"<< Exited {this.GetType().FullName}.updateSR", null, gwatch.ElapsedMilliseconds);
            }
        }

        [SwaggerOperation("cloneSR")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [System.Web.Http.ActionName("cloneSR")]
        public IMessageBase CloneSR([Bind(Include = "udo_ServiceRequestId")] UDOCloneSRRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.cloneSR ");
            try
            {
                UDOCloneSRProcessor processor = new UDOCloneSRProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "cloneSR", ex.Message);
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                                    , $"<< Exited {this.GetType().FullName}.cloneSR", null, gwatch.ElapsedMilliseconds);
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