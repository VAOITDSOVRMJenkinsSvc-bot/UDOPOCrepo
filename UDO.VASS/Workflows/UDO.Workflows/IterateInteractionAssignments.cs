using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.Workflows
{
    public class IterateInteractionAssignments : CodeActivity
    {
        [RequiredArgument]
        [Input("Workflow")]
        [ReferenceTarget("workflow")]
        public InArgument<EntityReference> Workflow { get; set; }

        [RequiredArgument]
        [Input("SSN")]
        public InArgument<string> SSN { get; set; }

        [Output("CountRecordsFound")]
        public OutArgument<int> CountRecordsOutput { get; set; }

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

                Guid workflowId = Workflow.Get(executionContext).Id;
                string accountId = SSN.Get(executionContext);
                if (workflowId != Guid.Empty && !string.IsNullOrWhiteSpace(accountId))
                {
                    tracingService.Trace("Validated Inputs");
                    tracingService.Trace("WorkflowId: " + workflowId.ToString());

                    tracingService.Trace("Create the query expression");
                    QueryExpression qe = new QueryExpression("udo_mheointeractionassignment");
                    qe.ColumnSet = new ColumnSet("udo_mheointeractionassignmentid");
                    FilterExpression fe = new FilterExpression();
                    fe.AddCondition("udo_ssn", ConditionOperator.Equal, accountId);

                    LinkEntity contactLink = qe.AddLink("contact","udo_contactid", "contactid", JoinOperator.Inner);
                    contactLink.LinkCriteria.Filters.Add(fe);
                    

                    tracingService.Trace("Retrieve matching interaction");
                    EntityCollection vassInteractionAssignments = service.RetrieveMultiple(qe);
                    CountRecordsOutput.Set(executionContext, vassInteractionAssignments.Entities.Count);

                    foreach (var vassInteractionAssignment in vassInteractionAssignments.Entities)
                    {
                        ExecuteWorkflowRequest request = new ExecuteWorkflowRequest() { EntityId = vassInteractionAssignment.Id, WorkflowId = workflowId };
                        try
                        {
                            service.Execute(request);
                        }
                        catch (InvalidPluginExecutionException ex)
                        {
                            Trace.TraceError(string.Format("Message{0} Stack trace: {1}", ex.Message, ex.StackTrace));
                        }
                    }
                }
            }
            catch (InvalidPluginExecutionException ex)
            {
                Trace.TraceError(string.Format("Message{0} Stack trace: {1}", ex.Message, ex.StackTrace));
            }
        }
    }
}
