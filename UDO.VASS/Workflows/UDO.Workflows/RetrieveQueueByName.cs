using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;

namespace UDO.Workflows
{
    public class RetrieveQueueByName : CodeActivity
    {
        [RequiredArgument]
        [Input("QueueName")]
        public InArgument<string> queueInput { get; set; }       

        [Output("Queue")]
        [ReferenceTarget("queue")]
        public OutArgument<EntityReference> queueReferenceOutput { get; set; }

        [Output("CountRecordsFound")]
        public OutArgument<int> countRecordsOutput { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            tracingService.Trace("Create the queue fetch expression");

            string queueName = queueInput.Get(executionContext);

            QueryExpression qe = new QueryExpression("queue");
            qe.ColumnSet = new ColumnSet("queueid");
            FilterExpression fe = new FilterExpression(LogicalOperator.And);
            fe.AddCondition("name", ConditionOperator.Equal, queueName);
            qe.Criteria.AddFilter(fe);

            EntityCollection queueColl = service.RetrieveMultiple(qe);

            tracingService.Trace(String.Format("Found {0} matching queues.", queueColl.Entities.Count));

            countRecordsOutput.Set(executionContext, queueColl.Entities.Count);

            if (queueColl.Entities.Count == 1)
            {
                EntityReference queueRef = queueColl.Entities[0].ToEntityReference();
                queueReferenceOutput.Set(executionContext, queueRef);
            }
        }
    }
}
