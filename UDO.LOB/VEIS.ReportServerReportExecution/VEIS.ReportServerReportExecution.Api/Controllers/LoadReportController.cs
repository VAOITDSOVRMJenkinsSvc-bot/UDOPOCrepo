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
    public class VEISLRLoadReportController : ApiController
    {
        // POST api/values
        [SwaggerOperation("Create_LRLoadReport")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public VEISLRLoadReportResponse Post([FromBody]VEISLRLoadReportRequest request)
        {
            EcProcessorBase processor = new VEISLRLoadReportGetDataProcessor();
            VEISLRLoadReportResponse response = (VEISLRLoadReportResponse)processor.Execute<VEISLRLoadReportResponse>(request);
            return response;
        }
    }
}

 