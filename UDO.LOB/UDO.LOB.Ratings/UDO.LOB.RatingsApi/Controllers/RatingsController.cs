
namespace UDO.LOB.Ratings.Controllers
{
    using Swashbuckle.Swagger.Annotations;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Web.Http;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions.Logging;
    using UDO.LOB.Ratings.Messages;
    using UDO.LOB.Ratings.Processors;

    public class RatingsController : ApiController
    {
        // POST api/ratings
        [SwaggerOperation("findRatings")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("findratings")]
        public IMessageBase Post([FromBody]UDOfindRatingsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.findratings ");
            try
            {
                UDOfindRatingsProcessor processor = new UDOfindRatingsProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "findratings", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.findratings ");
            }
        }

        // POST api/ratings
        [SwaggerOperation("getRatings")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("getRatings")]
        public IMessageBase Post([FromBody] UDOgetRatingDataRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.getRatings ");
            try
            {
                UDOgetRatingDataProcessor processor = new UDOgetRatingDataProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "getRatings", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.getRatings ");
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