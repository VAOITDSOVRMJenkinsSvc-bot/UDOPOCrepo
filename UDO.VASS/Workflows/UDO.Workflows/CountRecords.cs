using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Workflow.Activities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.Workflows
{
    public class CountRecords : CodeActivity
    {    
       
        [RequiredArgument]
        [Input("FetchXML")]
        public InArgument<string> FetchXML { get; set; }

        [Output("Count")]
        public OutArgument<int> Count { get; set; }
        protected override void Execute(CodeActivityContext executionContext)
        {

            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            tracingService.Trace("Begin request");
            try
            {
                tracingService.Trace("New fetch XML");
                string fetchXml = FetchXML.Get(executionContext);
                tracingService.Trace(fetchXml);
                FetchExpression fetchExpression = new FetchExpression(fetchXml);
                EntityCollection recordsCollection = service.RetrieveMultiple(fetchExpression);
                Count.Set(executionContext, recordsCollection.Entities.Count);               
            }
            catch (Exception ex)
            {
                tracingService.Trace($"Exception Message {ex.Message} {ex.StackTrace}");
                Count.Set(executionContext, 0);
            }

        }
    }
}
