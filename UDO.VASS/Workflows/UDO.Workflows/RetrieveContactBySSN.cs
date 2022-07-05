using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;

namespace UDO.Workflows
{
    public class RetrieveContactBySSN : CodeActivity
    {
        [RequiredArgument]
        [Input("SSN")]
        public InArgument<string> ssnInput { get; set; }

        [RequiredArgument]
        [Input("Return First Record if Duplicates")]
        public InArgument<bool> returnFirstRecordIfDupesInput { get; set; }

        [Output("Contact")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> contactReferenceOutput { get; set; }

        [Output("CountRecordsFound")]
        public OutArgument<int> countRecordsOutput { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            tracingService.Trace("Create the SSN query expression");

            string ssn = ssnInput.Get(executionContext);

            QueryExpression qe = new QueryExpression("contact");
            qe.ColumnSet = new ColumnSet("contactid");
            FilterExpression fe = new FilterExpression(LogicalOperator.And);
            fe.AddCondition("udo_ssn", ConditionOperator.Equal, ssn);
            qe.AddOrder("createdon", OrderType.Ascending);
            qe.Criteria.AddFilter(fe);

            tracingService.Trace("Retrieve matching contact records by SSN: " + ssn);
            EntityCollection contactColl = service.RetrieveMultiple(qe);

            tracingService.Trace(String.Format("Found {0} matching contacts.", contactColl.Entities.Count));

            countRecordsOutput.Set(executionContext, contactColl.Entities.Count);

            bool returnFirstRecordIfDupes = returnFirstRecordIfDupesInput.Get(executionContext);

            if (contactColl.Entities.Count == 1 || (contactColl.Entities.Count > 1 && returnFirstRecordIfDupes))
            {
                EntityReference contRef = contactColl.Entities[0].ToEntityReference();
                contactReferenceOutput.Set(executionContext, contRef);
            }


        }
    }
}
