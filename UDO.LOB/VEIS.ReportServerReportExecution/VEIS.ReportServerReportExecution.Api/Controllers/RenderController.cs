using System;
using System.Diagnostics;
using System.Collections.Generic;
using VEIS.Messages.ReportServerReportExecution;
using Swashbuckle.Swagger.Annotations;
using System.Net;
using System.Web.Http;
using VEIS.Core.Processor;
using VEIS.ReportServerReportExecution.Api.Processors;

namespace VEIS.ReportServerReportExecution.Api.Controllers
{
    public class VEISrdrRenderController : ApiController
    {
        // POST api/values
        [SwaggerOperation("Create_Render")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public VEISrdrRenderResponse Post([FromBody]VEISrdrRenderRequest request)
        {
            EcProcessorBase processor = new VEISrdrRenderGetDataProcessor();
            VEISrdrRenderResponse response = (VEISrdrRenderResponse)processor.Execute<VEISrdrRenderResponse>(request);
            return response;
        }
    }
}