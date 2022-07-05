using System;
using System.Activities;
using Microsoft.Xrm.Sdk;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class DeleteDependentMaintenance : CodeActivity
    {
        protected override void Execute(CodeActivityContext executionContext)
        {
            try
            {
                // Create the context and tracing service
                var context = executionContext.GetExtension<IExecutionContext>();

                var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();

                var service = serviceFactory.CreateOrganizationService(context.UserId);

                var tracer = executionContext.GetExtension<ITracingService>();

                //  make sure we are only deleting dependent maintenance transactions
                if (context.PrimaryEntityName.ToLower() != "crme_dependentmaintenance")
                {
                    throw new InvalidPluginExecutionException(
                            String.Format("Custom Action: DeleteDependentMaintenance is not valid for entity: {0}.",
                            context.PrimaryEntityName));
                }

                //  perform the delete
                tracer.Trace(String.Format("Deleting record: {0}:{1}", context.PrimaryEntityName, context.PrimaryEntityId));

                service.Delete(context.PrimaryEntityName, context.PrimaryEntityId);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(String.Format("An error occurred in the {0} plug-in.",
                        GetType()),
                      ex);
            }
        }
    }
}
