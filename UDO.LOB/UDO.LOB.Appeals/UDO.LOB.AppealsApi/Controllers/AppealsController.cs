using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Web.Http;
using UDO.LOB.Appeals.Messages;
using UDO.LOB.Appeals.Processors;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.AppealsApi.Controllers
{

    public class AppealsController : ApiController
    {
        [SwaggerOperation("createAppeal")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createAppeal")]
        // POST api/Appeals
        public IMessageBase createAppeal([FromBody] UDOcreateUDOAppealsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.CreateAppeals ");
            try
            {
                var processor = new UDOcreateUDOAppealsProcessor();
                return processor.Execute((UDOcreateUDOAppealsRequest)request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "createAppeal", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.CreateAppeals ");
                // return $"Hello {request}";

            }
        }

        [SwaggerOperation("createAppealDetails")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createAppealDetails")]
        // POST /api/Appeals
        public IMessageBase createAppealDetails([FromBody] UDOcreateUDOAppealDetailsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createAppealDates ");
            var response = new UDOcreateUDOAppealDetailsResponse();
            try
            {
                var processor = new createUDOAppealDetailsProcessor();
                response = processor.Execute((UDOcreateUDOAppealDetailsRequest)request) as UDOcreateUDOAppealDetailsResponse;

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "createAppealDetails", ex.ToString());
                throw ex;
            }

            LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createAppealDates ");
            return response;
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
