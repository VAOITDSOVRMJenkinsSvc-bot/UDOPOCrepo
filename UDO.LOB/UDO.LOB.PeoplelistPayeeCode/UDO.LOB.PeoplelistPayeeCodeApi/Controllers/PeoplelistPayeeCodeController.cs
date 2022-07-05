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
using UDO.LOB.PeoplelistPayeeCode.Messages;
using UDO.LOB.PeoplelistPayeeCode.Processors;


namespace UDO.LOB.PeoplelistPayeeCode.Controllers
{
    public class PeoplelistPayeeCodeController : ApiController
    {
        // POST api/PeoplelistPayeeCode
        [SwaggerOperation("createPeoplePayee")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createPeoplePayee")]
        public IMessageBase Post([FromBody]  UDOCreatePeoplePayeeRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createPeoplePayee ");
            try
            {
                UDOCreatePeoplePayeeProcessor processor = new UDOCreatePeoplePayeeProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "createPeoplePayee", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createPeoplePayee ");
            }
        }

        // POST api/PeoplelistPayeeCode
        [SwaggerOperation("findFiduciaryExists")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("findFiduciaryExists")]
        public IMessageBase Post([FromBody] UDOFiduciaryExistsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.findFiduciaryExists ");
            try
            {
                UDOFiduciaryExistsProcessor processor = new UDOFiduciaryExistsProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "findFiduciaryExists", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.findFiduciaryExists ");
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
