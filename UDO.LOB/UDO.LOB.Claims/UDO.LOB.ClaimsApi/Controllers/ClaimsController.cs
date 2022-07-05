using Swashbuckle.Swagger.Annotations;
using System;
using System.Diagnostics;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using UDO.LOB.Core;
using UDO.LOB.Claims.Messages;
using UDO.LOB.Claims.Processors;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.Claims.Controllers
{
    
    public class ClaimsController : ApiController
    {
        [SwaggerOperation("createClaims")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [System.Web.Http.ActionName("createClaims")]
        // POST api/claims
        public IMessageBase CreateClaims([FromBody]UDOcreateUDOClaimsRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            //LogHelper.LogInfo($" <<< Initialized the Controller in ({gwatch.ElapsedMilliseconds}) ms.");
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createClaims ");
            try
            {
                createUDOClaimsProcessor processor = new createUDOClaimsProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "CreateClaims", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.createClaims", null, gwatch.ElapsedMilliseconds);
            }

        }

        [SwaggerOperation("updateUDOClaims")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [System.Web.Http.ActionName("updateUDOClaims")]
        // PUT api/claims
        public IMessageBase updateUDOClaims([FromBody]UDOUpdateUDOClaimsRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.updateUDOClaims ");
            try
            {
                UpdateUDOClaimsProcessor processor = new UpdateUDOClaimsProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "updateUDOClaims", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.updateUDOClaims", null, gwatch.ElapsedMilliseconds);
            }
        }
        //veis api has http instead of https
        [SwaggerOperation("createUDOStatus")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [System.Web.Http.ActionName("createUDOStatus")]
        public IMessageBase CreateUDOStatus([FromBody]UDOcreateUDOStatusRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createUDOStatus ");
            try
            {
                UDOcreateUDOStatusProcessor processor = new UDOcreateUDOStatusProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "CreateUDOStatus", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.CreateUDOStatus", null, gwatch.ElapsedMilliseconds);
            }
        }
        //404 from http cert
        [SwaggerOperation("createTrackedItems")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [System.Web.Http.ActionName("createTrackedItems")]
        public IMessageBase CreateTrackedItems([FromBody]UDOcreateUDOTrackedItemsRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createTrackedItems ");
            try
            {
                UDOcreateUDOTrackedItemsProcessor processor = new UDOcreateUDOTrackedItemsProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "CreateTrackedItems", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.CreateTrackedItems", null, gwatch.ElapsedMilliseconds);
            }
        }
        //veis has http cert
        [SwaggerOperation("createSuspense")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [System.Web.Http.ActionName("createSuspense")]
        public IMessageBase CreateSuspense([FromBody]UDOcreateUDOSuspenseRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createSuspense ");
            try
            {
                UDOcreateUDOSuspenseProcessor processor = new UDOcreateUDOSuspenseProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "CreateSuspense", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.createSuspense", null, gwatch.ElapsedMilliseconds);
            }
        }

        //
        [SwaggerOperation("createLifecycles")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [System.Web.Http.ActionName("createLifecycles")]
        public IMessageBase CreateLifecycles([Bind(Include = "OrganizationName,UserId,RelatedParentId,RelatedParentEntityName,RelatedParentFieldName,LogTiming,LogSoap,Debug,ownerId,ownerType,LegacyServiceHeaderInfo,udo_claimId,claimId,UDOcreateUDOLifecyclesRelatedEntitiesInfo")]UDOcreateUDOLifecyclesRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createLifecycles ");
            try
            {
                UDOcreateUDOLifecyclesProcessor processor = new UDOcreateUDOLifecyclesProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "CreateLifecycles", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.createLifecycles", null, gwatch.ElapsedMilliseconds);
            }
        }
        //http cert
        [SwaggerOperation("createEvidence")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [System.Web.Http.ActionName("createEvidence")]
        public IMessageBase CreateEvidence([FromBody]UDOcreateUDOEvidenceRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createEvidence ");
            try
            {
                UDOcreateUDOEvidenceProcessor processor = new UDOcreateUDOEvidenceProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "CreateEvidence", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.createEvidence", null, gwatch.ElapsedMilliseconds);
            }
        }

        [SwaggerOperation("createContentions")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [System.Web.Http.ActionName("createContentions")]
        public IMessageBase CreateContentions([FromBody]UDOcreateUdoContentionsRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createContentions ");
            try
            {
                UDOcreateUdoContentionsProcessor processor = new UDOcreateUdoContentionsProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "CreateContentions", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.CreateContentions", null, gwatch.ElapsedMilliseconds);
            }
        }

        [SwaggerOperation("createUDOClaimsOrch")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [System.Web.Http.ActionName("createUDOClaimsOrch")]
        public IMessageBase CreateUDOClaimsOrch([FromBody]UDOcreateUDOClaimsRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createUDOClaimsOrch ");
            try
            {
                createUDOClaimsOrchProcessor processor = new createUDOClaimsOrchProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "CreateUDOClaimsOrch", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.CreateUDOClaimsOrch", null, gwatch.ElapsedMilliseconds);
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/ping")]
        [SwaggerOperation("ping")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult ping()
        {
            return Ok("pong");
        }
    }
}