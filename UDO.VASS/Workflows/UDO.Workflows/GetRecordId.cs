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
    public class GetRecordId : CodeActivity
    {
        [RequiredArgument]
        [Input("Identifier Attribute")]
        public InArgument<string> IdAttributeName { get; set; }
        [RequiredArgument]
        [Input("Lookup Attribute Name")]
        public InArgument<string> AttributeName { get; set; }
        [RequiredArgument]
        [Output("Lookup Logical Name")]
        public OutArgument<string> LogicalName { get; set; }
        [RequiredArgument]
        [Output("RecordId")]
        public OutArgument<string> RecordId { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            var attributeName = AttributeName.Get(context);
            var primaryIdAttributeName = IdAttributeName.Get(context);
            if (string.IsNullOrEmpty(attributeName))
                throw new InvalidPluginExecutionException("Invalid input: Lookup Attribute Name");
            if (string.IsNullOrEmpty(primaryIdAttributeName))
                throw new InvalidPluginExecutionException("Invalid input: Identifier Attribute");

            IWorkflowContext wContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService svc = serviceFactory.CreateOrganizationService(wContext.UserId);
            ITracingService trace = context.GetExtension<ITracingService>();
            trace.Trace($"All inputs present: Attribute Name{attributeName} /  IdAttributeName{primaryIdAttributeName}. Starting Code Activity.");
            var primaryEntityId = wContext.PrimaryEntityId;
            var entityLogicalName = wContext.PrimaryEntityName;
            trace.Trace($"Entity Logical Name {entityLogicalName}. Primary Entity Id : {primaryEntityId}");
            QueryExpression qe = new QueryExpression(entityLogicalName);
            qe.ColumnSet = new ColumnSet(attributeName);
            FilterExpression fe = new FilterExpression(LogicalOperator.And);
            fe.Conditions.Add(new ConditionExpression(primaryIdAttributeName, ConditionOperator.Equal, primaryEntityId));
            qe.Criteria = fe;
            trace.Trace("Created Query Expression");
            try
            {
                var resultSet = svc.RetrieveMultiple(qe);
                trace.Trace($"Completed Retrieve {resultSet.Entities.Count}");
                if (resultSet.Entities.Count > 0)
                {
                    if (resultSet.Entities[0].Attributes.ContainsKey(attributeName))
                    {
                        var attributeValue = resultSet.Entities[0].Attributes[attributeName];
                        if (attributeValue is EntityReference)
                        {
                            trace.Trace($"Found Attribute");
                            var returnValue = attributeValue as EntityReference;
                            RecordId.Set(context, returnValue.Id.ToString());
                            LogicalName.Set(context, returnValue.LogicalName);
                            trace.Trace($"Logical Name : {returnValue.LogicalName} ");
                            trace.Trace($"Id {returnValue.Id.ToString()}");
                            return;
                        }
                    }
                }
                trace.Trace("Did not find attribute");
                RecordId.Set(context, "NA");
                LogicalName.Set(context, "NA");
            }
            catch (Exception ex)
            {
                trace.Trace($"Exception Occured: {ex.Message}. Stack Trace: {ex.StackTrace}");
                RecordId.Set(context, "NA");
                LogicalName.Set(context, "NA");
            }
        }
    }
}
