using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

namespace UDO.Workflows
{
    public class GetInteractionBySSN : CodeActivity
    {
        [RequiredArgument]
        [Input("SSN")]
        [AttributeTarget("udo_vadircontactstaging", "udo_ssn")]
        public InArgument<string> SSN { get; set; }

        [Output("HasActiveInteractions")]
        public OutArgument<bool> hasActiveInteractions { get; set; }


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

                string ssn = SSN.Get(executionContext);

                tracingService.Trace("Create the query expression");

                QueryExpression qe = new QueryExpression("udo_interaction");
                qe.ColumnSet = new ColumnSet("udo_interactionid");


                FilterExpression fe = new FilterExpression();
                fe.AddCondition("udo_veteranssn", ConditionOperator.Equal, ssn);
                fe.AddCondition("statecode", ConditionOperator.Equal, 0);
                fe.AddCondition("udo_channel", ConditionOperator.Equal, 752280004);

                qe.Criteria.AddFilter(fe);

                tracingService.Trace("Retrieve matching interaction");
                EntityCollection interactionColl = service.RetrieveMultiple(qe);

                var noOfActiveInteractions = interactionColl.Entities.Count;

                tracingService.Trace(String.Format("Found {0} matching interactions.", noOfActiveInteractions));
                if(noOfActiveInteractions >= 1)
                {
                    hasActiveInteractions.Set(executionContext, true);
                }
                else
                {
                    hasActiveInteractions.Set(executionContext, false);
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
