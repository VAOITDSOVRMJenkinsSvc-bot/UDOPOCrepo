/// <summary>
/// Created By: Tom Northrup
/// Description: New controller for calling Claim Establishment Processors
/// </summary>

namespace UDO.LOB.ClaimEstablishment.Controllers
{
    using Swashbuckle.Swagger.Annotations;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Web.Http;
    using UDO.LOB.ClaimEstablishment.Messages;
    using UDO.LOB.ClaimEstablishment.Processors;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions.Logging;

    public class ClaimEstablishmentController : ApiController
    {
        [SwaggerOperation("findClaimEstablishment")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("findClaimEstablishment")]
        public IMessageBase FindClaimEstablishment([FromBody] UDOFindClaimEstablishmentRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.findClaimEstablishment ");
            try
            {
                UDOFindClaimEstablishmentProcessor processor = new UDOFindClaimEstablishmentProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                LogHelper.LogError(request.OrganizationName, request.UserId, "findClaimEstablishment", ex.Message);
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.findClaimEstablishment", null, gwatch.ElapsedMilliseconds);
            }
        }

        [SwaggerOperation("initiateClaimEstablishment")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("initiateClaimEstablishment")]
        public IMessageBase InitiateClaimEstablishment([FromBody] UDOInitiateClaimEstablishmentRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.initiateClaimEstablishment ");
            try
            {
                UDOInitiateClaimEstablishmentProcessor processor = new UDOInitiateClaimEstablishmentProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                LogHelper.LogError(request.OrganizationName, request.UserId, "initiateClaimEstablishment", ex.Message);
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                , $"<< Exited {this.GetType().FullName}.initiateClaimEstablishment", null, gwatch.ElapsedMilliseconds);
            }
        }

        [SwaggerOperation("insertClaimEstablishment")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("insertClaimEstablishment")]
        public IMessageBase InsertClaimEstablishment([FromBody] UDOInsertClaimEstablishmentRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.insertClaimEstablishment ");
            try
            {
                UDOInsertClaimEstablishmentProcessor processor = new UDOInsertClaimEstablishmentProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                LogHelper.LogError(request.OrganizationName, request.UserId, "insertClaimEstablishment", ex.Message);
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                , $"<< Exited {this.GetType().FullName}.initiateClaimEstablishment", null, gwatch.ElapsedMilliseconds);
            }
        }

        [SwaggerOperation("clearClaimEstablishment")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("clearClaimEstablishment")]
        public IMessageBase ClearClaimEstablishment([FromBody] UDOClearClaimEstablishmentRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.clearClaimEstablishment ");
            try
            {
                UDOClearClaimEstablishmentProcessor processor = new UDOClearClaimEstablishmentProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                LogHelper.LogError(request.OrganizationName, request.UserId, "clearClaimEstablishment", ex.Message);
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                , $"<< Exited {this.GetType().FullName}.clearClaimEstablishment", null, gwatch.ElapsedMilliseconds);
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