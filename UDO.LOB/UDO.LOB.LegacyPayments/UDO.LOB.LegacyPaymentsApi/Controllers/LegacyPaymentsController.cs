using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.LegacyPayments.Messages;
using UDO.LOB.LegacyPayments.Processors;

namespace UDO.LOB.LegacyPayments.Controllers
{
    public class LegacyPaymentsController : ApiController
    {
        // POST api/legacypayments
        [SwaggerOperation("createLegacyPayments")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createLegacyPayments")]
        public IMessageBase Post([FromBody] UDOcreateLegacyPaymentsRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createLegacyPayments ");
            try
            {
                UDOcreateUDOLegacyPaymentsProcessor processor = new UDOcreateUDOLegacyPaymentsProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                LogHelper.LogError(request.OrganizationName, request.UserId, $"#ERROR in {GetType().FullName}.createLegacyPayments", ex);
                throw ex;
            }
            finally
            {
                //LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createLegacyPayments ");
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.createLegacyPayments", null, gwatch.ElapsedMilliseconds);
            }
        }

        // POST api/legacypayments
        [SwaggerOperation("createLegacyPaymentsDetails")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createLegacyPaymentsDetails")]
        public IMessageBase createLegacyPaymentsDetails([FromBody] UDOcreateLegacyPaymentsDetailsRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createLegacyPaymentsDetails ");
            try
            {
                UDOcreateUDOLegacyPaymentsDetailsProcessor processor = new UDOcreateUDOLegacyPaymentsDetailsProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                LogHelper.LogError(request.OrganizationName, request.UserId, $"#ERROR in {GetType().FullName}.createLegacyPaymentsDetails", ex);
                throw ex;
            }
            finally
            {
                //LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createLegacyPaymentsDetails ");
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.createLegacyPaymentsDetails", null, gwatch.ElapsedMilliseconds);
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