using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;

namespace UDO.Workflows
{
    public class GetTimeLogRecord : CodeActivity
    {
        [RequiredArgument]
        [Input("Interaction")]
        [ReferenceTarget("udo_interaction")]
        public InArgument<EntityReference> Interaction { get; set; }


        [Output("Time Log Record")]
        [ReferenceTarget("udo_interactiontimelog")]
        public OutArgument<EntityReference> TimeLogOut { get; set; }

        [Output("CountRecordsFound")]
        public OutArgument<int> countRecordsOutput { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                tracingService.Trace("Get the input fields.");

                Guid interactionId = Interaction.Get(executionContext).Id;


                tracingService.Trace("Create the query expression");

                QueryExpression qe = new QueryExpression("udo_interactiontimelog");
                qe.ColumnSet = new ColumnSet("udo_interactiontimelogid");

                FilterExpression fe = new FilterExpression(LogicalOperator.And);
                fe.AddCondition("udo_interactionid", ConditionOperator.Equal, interactionId);
                fe.AddCondition("createdby", ConditionOperator.Equal, context.InitiatingUserId);
                qe.AddOrder("createdon", OrderType.Descending);

                qe.Criteria.AddFilter(fe);

                tracingService.Trace("Retrieve matching interaction");
                EntityCollection timeLogColl = service.RetrieveMultiple(qe);


                tracingService.Trace(String.Format("Found {0} matching interactions.", timeLogColl.Entities.Count));


                countRecordsOutput.Set(executionContext, timeLogColl.Entities.Count);

                tracingService.Trace("Fetching latest matching entry");

                EntityReference timelogRef = timeLogColl.Entities[0].ToEntityReference();
                TimeLogOut.Set(executionContext, timelogRef);

                tracingService.Trace("Finished!");
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
