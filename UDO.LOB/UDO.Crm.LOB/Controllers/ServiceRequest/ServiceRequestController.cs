using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.ServiceRequest.Messages;
using VRM.Integration.UDO.ServiceRequest.Processors;

namespace UDO.Crm.LOB.Controllers
{
    public class ServiceRequestController : ApiController
    {
        [SwaggerOperation("initiateSR")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("initiateSR")]
        public IMessageBase InitiateSR([FromBody] UDOInitiateSRRequest request)
        {
            Trace.WriteLine($">> Entered {this.GetType().FullName}.initiateSR ");
            try
            {
                var processor = new UDOInitiateSRProcessor();
                return processor.Execute((UDOInitiateSRRequest)request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                Trace.WriteLine($"<< Exited {this.GetType().FullName}.initiateSR ");
            }
        }

        [SwaggerOperation("updateSR")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("updateSR")]
        public IMessageBase UpdateSR([FromBody] UDOUpdateSRRequest request)
        {
            Trace.WriteLine($">> Entered {this.GetType().FullName}.updateSR ");
            try
            {
                var processor = new UDOUpdateSRProcessor();
                return processor.Execute((UDOUpdateSRRequest)request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                Trace.WriteLine($"<< Exited {this.GetType().FullName}.updateSR ");
            }
        }

        [SwaggerOperation("cloneSR")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("cloneSR")]
        public IMessageBase CloneSR([FromBody] UDOCloneSRRequest request)
        {
            Trace.WriteLine($">> Entered {this.GetType().FullName}.cloneSR ");
            try
            {
                var processor = new UDOCloneSRProcessor();
                return processor.Execute((UDOCloneSRRequest)request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                Trace.WriteLine($"<< Exited {this.GetType().FullName}.cloneSR ");
            }
        }
    }
}
