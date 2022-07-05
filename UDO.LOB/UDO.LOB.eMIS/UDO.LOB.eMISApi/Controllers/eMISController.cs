using Swashbuckle.Swagger.Annotations;
using System;
using System.Diagnostics;
using System.Net;
using System.Web.Http;
using UDO.LOB.Core;
using UDO.LOB.eMIS.Messages;
using UDO.LOB.eMIS.Processors;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.eMIS.Controllers
{
    public class eMISController : ApiController
    {
        // POST api/eMIS
        [SwaggerOperation("getMilitaryInformation")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("getMilitaryInformation")]
        public IMessageBase PostMilitaryInformation([FromBody] UDOgetMilitaryInformationRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.getMilitaryInformation ");
            try
            {
                UDOgetMilitaryInformationProcessor processor = new UDOgetMilitaryInformationProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "PostMilitaryInformation", ex.ToString());
                throw ex;
            }
            finally
            {
                //LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.getMilitaryInformation ");
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.getMilitaryInformation", null, gwatch.ElapsedMilliseconds);
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