using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Hosting;
using UDO.LOB.Awards.Messages;
using UDO.LOB.Awards.Processors;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.AwardsApi.Controllers
{
    public class AwardsController : ApiController
    {
        [ActionName("whoami")]
        public object WhoAmI()
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.WhoAmI ");

            UDOcreateAwardsProcessor processor = new UDOcreateAwardsProcessor();
            var response = processor.WhoAmI();
            LogHelper.LogInfo($" > {this.GetType().FullName}.WhoAmI :: processor.WhoAmI returned : {response.UserId.ToString()} ");
            LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.WhoAmI ");
            return response;
        }

        [ActionName("apicatalog")]
        public ApiCatalog ApiCatalog()
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.ApiCatalog ");
            ApiCatalog catalog = new ApiCatalog();
            try
            {
                catalog = ApiCatalogManager.LoadApiSettings();
                foreach (var api in catalog.ApiCollection)
                {
                    LogHelper.LogInfo($@"Api - Request: {api.RequestName} Response: {api.ResponseName} ApiName: {api.ApiRoute}");
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("", Guid.Empty, "#Error in ApiCatalog.", ex);
            }

            LogHelper.LogInfo($" > {this.GetType().FullName}.ApiCatalog :: processor.ApiCatalog returned : {catalog.ToString()} ");
            LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.ApiCatalog ");

            return catalog;
        }

        [SwaggerOperation("createAwards")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createAwards")]
        public IMessageBase CreateAwards([FromBody] UDOcreateAwardsRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createAwards ");
            try
            {
                UDOcreateAwardsProcessor processor = new UDOcreateAwardsProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "CreateAwards", ex.Message);
                throw ex;
            }
            finally
            {
                //LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createAwards ");
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.createAwards", null, gwatch.ElapsedMilliseconds);
            }
        }

        [SwaggerOperation("createAwardLines")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createAwardLines")]
        public IMessageBase CreateAwardLines([FromBody] UDOcreateAwardLinesRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createAwardLines ");
            try
            {
                var processor = new UDOcreateAwardLinesProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, $"#ERROR in {GetType().FullName}.CreateAwardLines", ex);
                throw ex;
            }
            finally
            {
                //LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createAwardLines ");
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.createAwardLines", null, gwatch.ElapsedMilliseconds);
            }
        }

        [SwaggerOperation("createAwardsSyncOrch")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createAwardsSyncOrch")]
        public IMessageBase CreateAwardsSyncOrch([FromBody] UDOcreateAwardsSyncOrchRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createAwardsSyncOrch ");
            try
            {
                var processor = new UDOcreateAwardsOrchProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, $"#ERROR in {GetType().FullName}.createAwardsSyncOrch", ex);
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createAwardsSyncOrch ");
            }
        }

        [SwaggerOperation("createClothingAllowance")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createClothingAllowance")]
        public IMessageBase CreateClothingAllowance([FromBody] UDOcreateClothingAllowanceRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createClothingAllowance ");
            try
            {
                var processor = new UDOcreateClothingAllowanceProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, $"#ERROR in {GetType().FullName}.createClothingAllowance", ex);
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createClothingAllowance ");
            }
        }

        /// createEVR
        // POST /api/createEVR
        [SwaggerOperation("createEVR")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpPost]
        public IMessageBase CreateEVR([FromBody] UDOcreateEVRsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createEVR ");
            try
            {
                UDOcreateEVRsProcessor processor = new UDOcreateEVRsProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "CreateEVR", ex.Message);
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createEVR ");
            }
        }


        // POST /api/CreateIncomeSummary
        [SwaggerOperation("createIncomeSummary")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createIncomeSummary")]
        public IMessageBase CreateIncomeSummary([FromBody] UDOcreateIncomeSummaryRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createIncomeSummary ");
            try
            {
                UDOcreateIncomeSummaryProcessor processor = new UDOcreateIncomeSummaryProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "CreateIncomeSummary", ex.Message);
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createIncomeSummary ");
            }
        }

        [SwaggerOperation("createDeductions")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createDeductions")]
        public IMessageBase CreateDeductions([FromBody] UDOcreateDeductionsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createDeductions ");
            try
            {
                UDOcreateDeductionsProcessor processor = new UDOcreateDeductionsProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, $"Error in {this.GetType().FullName}.CreateDeductions", ex);
                return new UDOcreateDeductionsResponse()
                {
                    ExceptionOccured = true,
                    ExceptionMessage = ex.Message,
                    MessageId = request.MessageId
                };

            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createDeductions ");
            }
        }

        [SwaggerOperation("createDiaries")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createDiaries")]
        public IMessageBase CreateDiaries([FromBody] UDOcreateDiariesRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createDiaries ");
            IMessageBase response = new UDOcreateDiariesResponse();
            UDOcreateDiariesProcessor processor = new UDOcreateDiariesProcessor();
            response = processor.Execute(request);
            LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createDiaries ");
            return response;
        }

        [SwaggerOperation("createProceeds")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createProceeds")]
        public IMessageBase CreateProceeds([FromBody] UDOcreateProceedsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createProceeds. Message Id: {request.MessageId}");
            IMessageBase response = new UDOcreateProceedsResponse();
            UDOcreateProceedsProcessor processor = new UDOcreateProceedsProcessor();
            response = processor.Execute(request);
            LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createProceeds. Message Id: {request.MessageId}");
            return response;
        }

        [SwaggerOperation("createReceivables")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createReceivables")]
        public IMessageBase CreateReceivables([FromBody] UDOcreateReceivablesRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.CreateReceivables. Message Id: {request.MessageId}");
            IMessageBase response = new UDOcreateProceedsResponse();
            UDOcreateReceivablesProcessor processor = new UDOcreateReceivablesProcessor();
            response = processor.Execute(request);
            LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.CreateReceivables. Message Id: {request.MessageId}");
            return response;
        }

        // POST /api/retrieveAwards
        [SwaggerOperation("retrieveAwards")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IMessageBase RetrieveAwards([FromBody] UDOretrieveAwardRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.retrieveAwards ");
            try
            {
                UDOretrieveAwardProcessor processor = new UDOretrieveAwardProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "RetrieveAwards", ex.Message);
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.retrieveAwards ");
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