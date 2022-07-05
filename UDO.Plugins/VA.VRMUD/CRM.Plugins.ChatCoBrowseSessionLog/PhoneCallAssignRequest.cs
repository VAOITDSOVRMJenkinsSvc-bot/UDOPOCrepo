using System;
using CRMUD;
using System.Linq;
using Microsoft.Xrm.Sdk;
using CRM.Plugins;
using Microsoft.Crm.Sdk.Messages;

namespace CRM.Plugins.ChatCoBrowseSessionLog
{
    public class PhoneCallAssignRequest : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var serviceProxy = serviceFactory.CreateOrganizationService(context.InitiatingUserId);
            
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference &&
                context.InputParameters.Contains("Assignee") && context.InputParameters["Assignee"] is EntityReference
               )
            {
                ServiceContext serviceContext = new ServiceContext(serviceProxy);

                EntityReference e_PhoneCallRecord = (EntityReference)context.InputParameters["Target"];

                EntityReference e_Assignee = (EntityReference)context.InputParameters["Assignee"];

                OptionSetValue chatType = serviceContext.PhoneCallSet.Where(c => c.Id == e_PhoneCallRecord.Id).Select(c => c.va_SessionType).FirstOrDefault();

                if (chatType == null)
                {
                    return;
                }

                if (chatType.Value != 935950001 && chatType.Value != 935950000)
                {
                    return;
                }

                Guid chatCoBrowseID = serviceContext.crme_chatcobrowsesessionlogSet.Where(c => c.crme_PhoneCallId.Id == e_PhoneCallRecord.Id).Select(c => c.Id).FirstOrDefault();
                
                if (chatCoBrowseID == null || chatCoBrowseID == Guid.Empty)
                    throw new InvalidPluginExecutionException("Could not find chat session record using the phone call ID: " + e_PhoneCallRecord.Id.ToString());
                
                // Create the Request Object and Set the Request Object's Properties
                
                AssignRequest assign = new AssignRequest
                {
                    Assignee = new EntityReference(e_Assignee.LogicalName,
                        e_Assignee.Id),
                    Target = new EntityReference(crme_chatcobrowsesessionlog.EntityLogicalName,
                        chatCoBrowseID)
                };

                serviceProxy.Execute(assign);
                
            }
            else
            {
                return;
            }
        }
    }
}
