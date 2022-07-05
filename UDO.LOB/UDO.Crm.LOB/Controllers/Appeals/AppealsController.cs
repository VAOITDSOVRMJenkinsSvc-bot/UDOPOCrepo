
namespace UDO.Crm.LOB.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Diagnostics;
    using VRM.Integration.UDO.Appeals.Messages;
    using VRM.Integration.UDO.Appeals.Processors;
    using VRM.Integration.Servicebus.Core;
    using Swashbuckle.Swagger.Annotations;
    public class AppealsController : ApiController
    {
        [SwaggerOperation("createAppeal")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createAppeal")]
        public IMessageBase createAppeal([FromBody] UDOcreateUDOAppealsRequest request)
        {
            Trace.WriteLine($">> Entered {this.GetType().FullName}.CreateAppeals ");
            try
            {
                var processor = new UDOcreateUDOAppealsProcessor();
                return processor.Execute((UDOcreateUDOAppealsRequest)request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                Trace.WriteLine($"<< Exited {this.GetType().FullName}.CreateAppeals ");
                // return $"Hello {request}";

            }
        }

        [SwaggerOperation("createAppealDetails")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createAppealDetails")]
        public IMessageBase createAppealDetails([FromBody] UDOcreateUDOAppealDetailsRequest request)
        {
            Trace.WriteLine($">> Entered {this.GetType().FullName}.createAppealDates ");
            var response = new UDOcreateUDOAppealDetailsResponse();
            try
            {
                var processor = new createUDOAppealDetailsProcessor();
                response = processor.Execute((UDOcreateUDOAppealDetailsRequest)request) as UDOcreateUDOAppealDetailsResponse;

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }

            Trace.WriteLine($"<< Exited {this.GetType().FullName}.createAppealDates ");
            return response;
        }

        [SwaggerOperation("createAppealDates")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createAppealDates")]
        public IMessageBase createAppealDates([FromBody] UDOcreateUDOAppealDatesRequest request)
        {
            Trace.WriteLine($">> Entered {this.GetType().FullName}.createAppealDates ");
            var response = new UDOcreateUDOAppealDatesResponse();
            try
            {
                var processor = new createUDOAppealDatesProcessor();
                response = processor.Execute((UDOcreateUDOAppealDatesRequest)request) as UDOcreateUDOAppealDatesResponse;

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }

            Trace.WriteLine($"<< Exited {this.GetType().FullName}.createAppealDates ");
            return response;
        }

        [SwaggerOperation("createUDOAppealDiaries")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createUDOAppealDiaries")]
        public IMessageBase createUDOAppealDiaries([FromBody] UDOcreateUDOAppealDiariesRequest request)
        {
            Trace.WriteLine($">> Entered {this.GetType().FullName}.createUDOAppealDiaries ");
            var response = new UDOcreateUDOAppealDiariesResponse();
            try
            {
                var processor = new UDOcreateUDOAppealDiariesProcessor();
                response = processor.Execute((UDOcreateUDOAppealDiariesRequest)request) as UDOcreateUDOAppealDiariesResponse;

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }

            Trace.WriteLine($"<< Exited {this.GetType().FullName}.createUDOAppealDiaries ");
            return response;
        }

    }
}
