
namespace UDO.Crm.LOB.Controllers
{
    using Swashbuckle.Swagger.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using VRM.Integration.Servicebus.Core;
    using VRM.Integration.UDO.Messages;
    using VRM.Integration.UDO.Payments.Messages;
    using VRM.Integration.UDO.Payments.Processors;

    public class PaymentsController : ApiController
    {
        [SwaggerOperation("createPayments")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createPayments")]
        public IMessageBase createPayments([FromBody]  UDOcreatePaymentsRequest request)
        {
            Trace.WriteLine($">> Entered {this.GetType().FullName}.createPayments ");
            try
            {
                var processor = new UDOcreatePaymentsProcessor();
                return processor.Execute((UDOcreatePaymentsRequest)request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                Trace.WriteLine($"<< Exited{this.GetType().FullName}.createPayments ");
            }
        }

        [SwaggerOperation("retrievePayments")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpGet]
        public IMessageBase Get([FromBody]  UDOgetPaymentDetailsRequest request)
        {
            Trace.WriteLine($">> Entered {this.GetType().FullName}.retrievePayments ");
            try
            {
                var processor = new UDOgetPaymentDetailsProcessor();
                return processor.Execute((UDOgetPaymentDetailsRequest)request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                Trace.WriteLine($"<< Exited{this.GetType().FullName}.retrievePayments ");
            }
        }

    }
}
