using Swashbuckle.Swagger.Annotations;
using System;
using System.Diagnostics;
using System.Net;
using System.Web.Http;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.VeteranSnapShot.Messages;
using UDO.LOB.VeteranSnapShot.Processors;

namespace UDO.LOB.VeteranSnapShot.Controllers
{
    public class VeteranSnapShotController : ApiController
    {
        // POST api/veteransnapshot
        [SwaggerOperation("createUDOVeteranSnapShot")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createUDOVeteranSnapShot")]
        public IMessageBase createUDOVeteranSnapShot([FromBody] UDOcreateUDOVeteranSnapShotRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createUDOVeteranSnapShot ");
            try
            {
                UDOcreateUDOVeteranSnapShotProcessor processor = new UDOcreateUDOVeteranSnapShotProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "createUDOVeteranSnapShot", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.createUDOVeteranSnapShot", null, gwatch.ElapsedMilliseconds);

            }
        }

        // POST api/veteransnapshot
        [SwaggerOperation("createVeteranSnapShot")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createVeteranSnapShot")]
        public IMessageBase createVeteranSnapShot([FromBody] UDOCreateVeteranSnapshotRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createVeteranSnapShot ");
            try
            {
                UDOCreateVeteranSnapshotProcessor processor = new UDOCreateVeteranSnapshotProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "createVeteranSnapShot", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                                    , $"<< Exited {this.GetType().FullName}.createVeteranSnapShot", null, gwatch.ElapsedMilliseconds);
            }
        }

        // POST api/veteransnapshot
        [SwaggerOperation("loadVeteranSnapshotAsync")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("loadVeteranSnapshotAsync")]
        // TODO: REM: Use of UDOException here on the controller. The Processor class using UDOException
        //       TN changed IMessageBase to UDO.LOB.Core.UDOException
        public UDOException loadVeteranSnapshotAsync([FromBody] UDOLoadVeteranSnapshotAsyncRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.loadVeteranSnapshotAsync ");
            try
            {
                UDOLoadVeteranSnapshotAsyncProcessor processor = new UDOLoadVeteranSnapshotAsyncProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "loadVeteranSnapshotAsync", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.loadVeteranSnapshotAsync", null, gwatch.ElapsedMilliseconds);
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