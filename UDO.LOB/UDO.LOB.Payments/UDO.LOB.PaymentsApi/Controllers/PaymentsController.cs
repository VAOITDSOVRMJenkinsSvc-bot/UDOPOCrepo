using System.Diagnostics;
using System.Reflection;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Payments.Messages;
using UDO.LOB.Payments.Processors;
using VEIS.Core.Messages;

//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;
//using VRM.Integration.UDO.Payments.Messages;
//using VRM.Integration.UDO.Payments.Processors;

namespace UDO.LOB.Payments.Controllers
{
    
    public class PaymentsController : ApiController
    {
        [SwaggerOperation("createPayments")]
        [SwaggerResponse(HttpStatusCode.OK)]
        //[SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createPayments")]
        [HttpPost]
        public IMessageBase Post([FromBody]  UDOcreatePaymentsRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($" <<< Initialized the Controller in ({gwatch.ElapsedMilliseconds}) ms.");
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createPayments ");
            try
            {
                var processor = new UDOcreatePaymentsProcessor();
                return processor.Execute((UDOcreatePaymentsRequest)request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "createPayments", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($" <<< Exited the Controller in ({gwatch.ElapsedMilliseconds}) ms.");
                LogHelper.LogInfo($"<< Exited{this.GetType().FullName}.createPayments ");
            }
        }

        [SwaggerOperation("getPaymentDetails")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("getPaymentDetails")]
        [HttpPost]
        public IMessageBase getPaymentDetails([FromBody]  UDOgetPaymentDetailsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.getPaymentDetails ");
            try
            {
                var processor = new UDOgetPaymentDetailsProcessor();
                return processor.Execute((UDOgetPaymentDetailsRequest)request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "getPaymentDetails", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited{this.GetType().FullName}.getPaymentDetails ");
            }
        }

        // POST api/payments
        [SwaggerOperation("createPaymentAdjustment")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createPaymentAdjustment")]
        [HttpPost]
        public IMessageBase Post([FromBody] UDOcreatePaymentAdjustmentsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createPaymentAdjustment ");
            try
            {
                UDOcreatePaymentAdjustmentsProcessor processor = new UDOcreatePaymentAdjustmentsProcessor();
                return processor.Execute((UDOcreatePaymentAdjustmentsRequest)request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "createPaymentAdjustment", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited{this.GetType().FullName}.createPaymentAdjustment ");
            }
        }

        [SwaggerOperation("createAwardAdjustment")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [ActionName("createAwardAdjustment")]
        [HttpPost]
        public IMessageBase Post([FromBody] UDOcreateAwardAdjustmentRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createAwardAdjustment ");
            try
            {
                UDOcreateAwardAdjustmentProcessor processor = new UDOcreateAwardAdjustmentProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "createAwardAdjustment", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited{this.GetType().FullName}.createAwardAdjustment ");

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
