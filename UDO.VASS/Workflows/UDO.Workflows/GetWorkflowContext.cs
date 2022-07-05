using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow.Activities;

namespace UDO.Workflows
{
    public class GetWorkflowContext : CodeActivity
    {

        [Output("Intitiating User")]
        [ReferenceTarget("systemuser")]
        public OutArgument<EntityReference> InitiatingUserOut { get; set; }

        [Output("UserId")]
        [ReferenceTarget("systemuser")]
        public OutArgument<EntityReference> UserIdOut { get; set; }

        [Output("PrimaryEntityId")]       
        public OutArgument<string> PrimaryId { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            ITracingService tracingService = context.GetExtension<ITracingService>();

            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            EntityReference initiatingUserIdRef = new EntityReference("systemuser", workflowContext.InitiatingUserId);

            tracingService.Trace("Initiating User Id - " + workflowContext.InitiatingUserId);

            InitiatingUserOut.Set(context, initiatingUserIdRef);
            EntityReference userIdRef = new EntityReference("systemuser", workflowContext.UserId);

            tracingService.Trace("User Id - " + workflowContext.UserId);

            UserIdOut.Set(context, userIdRef);
            PrimaryId.Set(context, workflowContext.PrimaryEntityId.ToString());
        }
    }
}
