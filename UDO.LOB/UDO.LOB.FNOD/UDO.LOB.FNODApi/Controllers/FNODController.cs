using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.FNOD.Messages;
using UDO.LOB.FNOD.Processors;

namespace UDO.LOB.FNOD.Controllers
{
    public class FNODController : ApiController
    {
        // POST api/initiateFNOD
        [SwaggerOperation("initiateFNOD")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("initiateFNOD")]
        public IMessageBase Post([FromBody] UDOInitiateFNODRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.initiateFNOD ");
            try
            {
                UDOInitiateFNODProcessor processor = new UDOInitiateFNODProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "initiateFNOD", ex.Message);
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                //LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.initiateFNOD ");
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.initiateFNOD", null, gwatch.ElapsedMilliseconds);

            }
        }

        // POST api/SsaDeathMatchInquiry
        [SwaggerOperation("SsaDeathMatchInquiry")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("SsaDeathMatchInquiry")]
        [HttpPost]
        public IMessageBase SsaDeathMatchInquiry([FromBody] UDOSsaDeathMatchInquiryRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.ssaDeathMatchInquiry ");
            try
            {
                UDOSsaDeathMatchInquiryProcessor processor = new UDOSsaDeathMatchInquiryProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.ssaDeathMatchInquiry ");
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.initiateFNOD", null, gwatch.ElapsedMilliseconds);
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