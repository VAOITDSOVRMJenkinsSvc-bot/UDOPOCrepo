using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

namespace UDO.Workflows
{
    public class GetInteraction : CodeActivity
    {
        [RequiredArgument]
        [Input("Interaction Assignment Record")]
        [ReferenceTarget("udo_mheointeractionassignment")]
        public InArgument<EntityReference> InteractionAssignRecord { get; set; }

        [RequiredArgument]
        [Input("Interval")]
        [AttributeTarget("udo_interaction", "udo_interactionmheointerval")]
        public InArgument<OptionSetValue> VassInterval { get; set; }

        [Output("VASS Interaction")]
        [ReferenceTarget("udo_interaction")]
        public OutArgument<EntityReference> VassInteractionOut { get; set; }

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

                Guid recordId = InteractionAssignRecord.Get(executionContext).Id;
                int interval = VassInterval.Get(executionContext).Value;

                tracingService.Trace("Record Id - " + recordId + " Interval - " + interval);

                tracingService.Trace("Create the query expression");

                QueryExpression qe = new QueryExpression("udo_interaction");
                qe.ColumnSet = new ColumnSet("udo_interactionid");


                FilterExpression fe = new FilterExpression();
                fe.AddCondition("udo_mheointeractionassignmentid", ConditionOperator.Equal, recordId);
                fe.AddCondition("udo_interactionmheointerval", ConditionOperator.Equal, interval);
                //qe.AddOrder("createdon", OrderType.Descending);

                qe.Criteria.AddFilter(fe);

                tracingService.Trace("Retrieve matching interaction");
                EntityCollection interactionColl = service.RetrieveMultiple(qe);



                tracingService.Trace(String.Format("Found {0} matching interactions.", interactionColl.Entities.Count));


                countRecordsOutput.Set(executionContext, interactionColl.Entities.Count);

                if (interactionColl.Entities.Count == 1)
                {

                    tracingService.Trace("Found one interaction");

                    tracingService.Trace("Found Interaction details - " + interactionColl.Entities[0].ToEntityReference().Name + "  " + interactionColl.Entities[0].ToEntityReference().LogicalName);

                    EntityReference interactionRef = interactionColl.Entities[0].ToEntityReference();
                    VassInteractionOut.Set(executionContext, interactionRef);
                }
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
