using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.Workflows
{
    public class GetModelDrivenAppUrl : CodeActivity
    {
        [RequiredArgument]
        [Input("App Domain Url")]
        public InArgument<string> AppDomainUrl { get; set; }

        [RequiredArgument]
        [Input("App Id")]
        public InArgument<string> AppId { get; set; }

        [Output("Model Driven App Url")]
        public OutArgument<string> ModelDrivenAppUrl { get; set; }
        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            try
            {
                tracingService.Trace("Get the input fields.");
                string domainUrl = AppDomainUrl.Get(executionContext);
                string applicationId = AppId.Get(executionContext);

                string modelDrivenUrl = domainUrl + "&appid=" + applicationId;

                tracingService.Trace($"Model Driven App Url is : {modelDrivenUrl}");

                tracingService.Trace("Setting the output variable");
                ModelDrivenAppUrl.Set(executionContext, modelDrivenUrl);
            }
            catch (Exception ex)
            {
                tracingService.Trace("Exception Occurred!");
                tracingService.Trace("Message: " + ex.Message);
                if (ex.InnerException != null)
                {
                    tracingService.Trace("Inner Exception: " + ex.InnerException);
                }
            }

        }
    }
}
