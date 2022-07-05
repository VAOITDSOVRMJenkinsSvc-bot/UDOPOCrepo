using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using UDO.LOB.ExamsAppointments.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.ExamsAppointments.Processors;

// using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace UDO.LOB.ExamsAppointments.Controllers
{
    public class ExamsAppointmentsController : ApiController
    {
        // POST api/ExamsAppointments
        [SwaggerOperation("createUDOAppointments")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [ActionName("createUDOAppointments")]
        public IMessageBase Post([FromBody] UDOcreateUDOAppointmentsRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createUDOAppointments ");
            try
            {
                UDOcreateUDOAppointmentsProcessor processor = new UDOcreateUDOAppointmentsProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                LogHelper.LogError(request.OrganizationName, request.UserId, "createUDOAppointments", ex.Message);
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.createUDOAppointments", null, gwatch.ElapsedMilliseconds);
            }
        }

        // POST api/ExamsAppointments
        [SwaggerOperation("createUDOExamRequest")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [ActionName("createUDOExamRequest")]
        public IMessageBase Post([FromBody] UDOcreateUDOExamRequestRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createUDOExamRequest ");
            try
            {
                UDOcreateUDOExamRequestProcessor processor = new UDOcreateUDOExamRequestProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "createUDOExamRequest", ex.Message);
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.createUDOExamRequest", null, gwatch.ElapsedMilliseconds);
            }
        }


        // POST api/ExamsAppointments
        [SwaggerOperation("createUDOExam")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [ActionName("createUDOExam")]
        public IMessageBase Post([FromBody] UDOcreateUDOExamRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createUDOExam ");
            try
            {
                UDOcreateUDOExamsProcessor processor = new UDOcreateUDOExamsProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "createUDOExam", ex.Message);
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.createUDOExam", null, gwatch.ElapsedMilliseconds);
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