using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using System.Activities;
using System;
using System.Diagnostics;

namespace UDO.Workflows
{
    public class GetCallAttempt : CodeActivity
    {
        [RequiredArgument]
        [Input("Interaction")]
        [ReferenceTarget("udo_interaction")]
        public InArgument<EntityReference> Interaction { get; set; }

        [Output("Number of Active Call Attempts")]
        public OutArgument<int> ActiveCallAttempts { get; set; }

        [Output("Call Attempt Record")]
        [ReferenceTarget("udo_outboundcallattempt")]
        public OutArgument<EntityReference> CallAttemptOut { get; set; }


        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            tracingService.Trace("Getting the input variables");
            Guid interactionId = Interaction.Get(executionContext).Id;

            tracingService.Trace("Creating query to fetch call attempts");
            QueryExpression qe = new QueryExpression("udo_outboundcallattempt");
            qe.ColumnSet = new ColumnSet("udo_outboundcallattemptid");

            FilterExpression fe = new FilterExpression(LogicalOperator.And);
            fe.AddCondition("udo_interactionid", ConditionOperator.Equal, interactionId);
            fe.AddCondition("statecode", ConditionOperator.Equal, 0);
            qe.AddOrder("createdon", OrderType.Descending);

            qe.Criteria.AddFilter(fe);

            EntityCollection callAttemptColl = service.RetrieveMultiple(qe);
            var activeCount = callAttemptColl.Entities.Count;

            tracingService.Trace($"Number of active call attempts are {activeCount}");

            if (activeCount >= 1)
            {
                ActiveCallAttempts.Set(executionContext, callAttemptColl.Entities.Count);

                EntityReference callAttemptRef = callAttemptColl.Entities[0].ToEntityReference();
                CallAttemptOut.Set(executionContext, callAttemptRef);
                tracingService.Trace("Finished! - (CallAttempts >= 1)");
            }
            else
            {
                ActiveCallAttempts.Set(executionContext, 0);
                CallAttemptOut.Set(executionContext, null);
                tracingService.Trace("Finished! - (CallAttempts = 0)");
            }

        }
    }
}
