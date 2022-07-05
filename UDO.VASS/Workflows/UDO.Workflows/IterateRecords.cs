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
    public class IterateRecords : CodeActivity
    {
        [RequiredArgument]
        [Input("Workflow")]
        [ReferenceTarget("workflow")]
        public InArgument<EntityReference> Workflow { get; set; }

        [RequiredArgument]
        [Input("NumberOfRecordsToExclude")]
        public InArgument<int> NumberOfRecordsToExclude { get; set; }

        [RequiredArgument]
        [Input("FetchXML")]
        public InArgument<string> FetchXML { get; set; }

        [Output("CountRecordsFound")]
        public OutArgument<int> CountRecordsOutput { get; set; }
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
                Guid workflowId = Workflow.Get(executionContext).Id;
                int numberOfRecordsToExlcude = NumberOfRecordsToExclude.Get(executionContext);

                if (!(numberOfRecordsToExlcude>0))
                {
                    tracingService.Trace("Please pass a valid number to exclude the record");
                    return;
                }
                FetchExpression fetchExpression = new FetchExpression(fetchXml);
                EntityCollection recordsCollection = service.RetrieveMultiple(fetchExpression);
                CountRecordsOutput.Set(executionContext, recordsCollection.Entities.Count);
                if (recordsCollection.Entities.Count > 0)
                {
                    tracingService.Trace($"first Record --{recordsCollection.Entities[0].GetAttributeValue<DateTime>("createdon")}");
                    tracingService.Trace($"Count of records {recordsCollection.Entities.Count}");
                    tracingService.Trace($"Fetch XML {fetchXml}");
                    for (int i = numberOfRecordsToExlcude; i < recordsCollection.Entities.Count; i++)
                    {
                        tracingService.Trace($"All records --{recordsCollection.Entities[i].GetAttributeValue<DateTime>("createdon")}");


                        ExecuteWorkflowRequest request = new ExecuteWorkflowRequest() { EntityId = recordsCollection.Entities[i].Id, WorkflowId = workflowId };
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
                else
                {
                    tracingService.Trace("No Records Exist");
                    return;
                }

            }
            catch (Exception ex)
            {

                tracingService.Trace($"Exception Message {ex.Message} {ex.StackTrace}");
                throw;
            }

        }
    }
}
