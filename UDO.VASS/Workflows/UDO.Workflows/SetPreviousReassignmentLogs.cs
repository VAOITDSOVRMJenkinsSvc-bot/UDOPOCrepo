using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.Workflows
{
    public class SetPreviousReassignmentLogs : CodeActivity
    {


        [RequiredArgument]
        [Input("SSN")]
        public InArgument<string> SSN { get; set; }


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
                string SSNfromInputParameter = SSN.Get(executionContext);
                string fetchXmltoGetReassigbmentLogs = $@"<fetch no-lock='true' >
  <entity name='udo_assignmentlog' >
    <attribute name='udo_assignmentlogid' />
    <attribute name='createdon' />
	<attribute name='udo_isprocessed' />
    <order attribute='createdon' descending='true' />
    <filter type='and'>
       <filter type='or'>
        <condition attribute='udo_reassignmentreason' operator='eq' value='752280000' />
        <condition attribute='udo_reassignmentreason' operator='eq' value='752280001' />
       </filter>  
      <filter type='or'>
        <condition attribute='udo_isprocessed' operator='null' />
        <condition attribute='udo_isprocessed' operator='eq' value='0' />
       </filter>   
    </filter>
    <link-entity name='udo_interaction' from='udo_interactionid' to='udo_interaction' link-type='inner' alias='ag'>
        <attribute name='udo_interactionmheointerval'/>
        <filter type='and'>
            <condition attribute='statecode' operator='eq' value='0' />
            <condition attribute='udo_channel' operator='eq' value='752280004' />
            <condition attribute='udo_veteranssn' operator='eq' value='{SSNfromInputParameter}' />
        </filter>
    </link-entity>
  </entity>
</fetch>";


                EntityCollection getReassignmentLogsForThisSSN = service.RetrieveMultiple(new FetchExpression(fetchXmltoGetReassigbmentLogs));
                tracingService.Trace("Reassignmnet Logs for this SSN retrieved");
                if (getReassignmentLogsForThisSSN.Entities.Count > 0)
                {
                    tracingService.Trace($"First record {getReassignmentLogsForThisSSN.Entities[0].GetAttributeValue<DateTime>("createdon")}");
                    tracingService.Trace("Starting from the second record");
                    for (int i = 1; i < getReassignmentLogsForThisSSN.Entities.Count; i++)
                    {
                        if (getReassignmentLogsForThisSSN.Entities[i].Contains("udo_assignmentlogid"))
                        {
                            Entity updateRecord = new Entity(getReassignmentLogsForThisSSN.Entities[i].LogicalName, getReassignmentLogsForThisSSN.Entities[i].GetAttributeValue<Guid>("udo_assignmentlogid"));
                            updateRecord["udo_isprocessed"] = true;
                            service.Update(updateRecord);
                            tracingService.Trace($"Updated record {i} with created date {getReassignmentLogsForThisSSN.Entities[i].GetAttributeValue<DateTime>("createdon")}");
                        }
                        else
                        {
                            tracingService.Trace("Assignment LogId Not Found");
                            return;
                        }

                    }
                }
                else
                {
                    tracingService.Trace("No Reassignment for this SSN");
                    return;
                }

            }
            catch (Exception ex)
            {
                tracingService.Trace($"Exception --- {ex.Message} {ex.StackTrace}");
            }
        }
    }
}
