
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
    using VRM.Integration.UDO.Awards.Messages;
    using VRM.Integration.UDO.Awards.Processors;

    public class AwardsController : ApiController
    {
        [SwaggerOperation("createAwards")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createAwards")]
        public IMessageBase createAwards([FromBody]  UDOcreateAwardsRequest request)
        {
            Trace.WriteLine($">> Entered {this.GetType().FullName}.createAwards ");
            try
            {
                UDOcreateAwardsProcessor processor = new UDOcreateAwardsProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                Trace.WriteLine($"<< Exited{this.GetType().FullName}.createAwards ");
            }
        }

        [SwaggerOperation("retrieveAwards")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpGet]
        public IMessageBase retrieveAwards([FromBody]  UDOretrieveAwardRequest request)
        {
            Trace.WriteLine($">> Entered {this.GetType().FullName}.retrieveAwards ");
            try
            {
                UDOretrieveAwardProcessor processor = new UDOretrieveAwardProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                Trace.WriteLine($"<< Exited{this.GetType().FullName}.retrieveAwards ");
            }
        }

    }
}
