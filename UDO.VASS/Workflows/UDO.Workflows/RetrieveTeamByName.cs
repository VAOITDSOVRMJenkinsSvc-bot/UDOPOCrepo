using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;

namespace UDO.Workflows
{
    public class RetrieveTeamByName : CodeActivity
    {
        [RequiredArgument]
        [Input("TeamName")]
        public InArgument<string> teamInput { get; set; }

        [Input("BusinessUnitName")]
        public InArgument<string> businessUnitInput { get; set; }

        [Output("Team")]
        [ReferenceTarget("team")]
        public OutArgument<EntityReference> teamReferenceOutput { get; set; }

        [Output("CountRecordsFound")]
        public OutArgument<int> countRecordsOutput { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            tracingService.Trace("Create the team fetch expression");

            string teamName = teamInput.Get(executionContext);
            string buName = businessUnitInput.Get(executionContext);

            string fetchXml = String.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='team'>
                                <attribute name='name' />
                                <attribute name='teamid' />
                                <order attribute='name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='teamtype' operator= 'ne' value='1' />
                                  <condition attribute='name' operator= 'eq' value='{0}' />
                                </filter>", teamName);

            if (!string.IsNullOrEmpty(buName))
            {
                fetchXml += String.Format(@"<link-entity name='businessunit' from='businessunitid' to='businessunitid' link-type='inner' alias='ac'>
                                  <filter type='and'>
                                    <condition attribute='name' operator='eq' value='{0}'/>
                                  </filter>
                                </link-entity>", buName);
            }

            fetchXml += "</entity></fetch>";


            tracingService.Trace("FetchXML:" + fetchXml);

            EntityCollection teamColl = service.RetrieveMultiple(new FetchExpression(fetchXml));

            tracingService.Trace(String.Format("Found {0} matching teams.", teamColl.Entities.Count));

            countRecordsOutput.Set(executionContext, teamColl.Entities.Count);

            if (teamColl.Entities.Count == 1)
            {
                EntityReference teamRef = teamColl.Entities[0].ToEntityReference();
                teamReferenceOutput.Set(executionContext, teamRef);

            }
        }
    }
}
