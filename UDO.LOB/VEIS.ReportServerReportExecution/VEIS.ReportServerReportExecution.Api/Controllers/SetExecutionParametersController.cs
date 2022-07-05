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
    public class SEPSetExecutionParametersController : ApiController
    {
        // POST api/values
        [SwaggerOperation("Create_SetExecutionParameters")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public VEISSEPSetExecutionParametersResponse Post([FromBody]VEISSEPSetExecutionParametersRequest request)
        {
            EcProcessorBase processor = new VEISSEPSetExecutionParametersGetDataProcessor();
            VEISSEPSetExecutionParametersResponse response = (VEISSEPSetExecutionParametersResponse)processor.Execute<VEISSEPSetExecutionParametersResponse>(request);
            return response;
        }
    }
}

 