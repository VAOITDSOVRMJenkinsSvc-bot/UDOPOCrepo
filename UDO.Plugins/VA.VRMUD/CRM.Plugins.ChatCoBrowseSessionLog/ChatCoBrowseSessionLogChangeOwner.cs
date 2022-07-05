using System;
using CRMUD;
using Microsoft.Xrm.Sdk;
using CRM.Plugins;
using Microsoft.Crm.Sdk.Messages;

namespace CRM.Plugins.ChatCoBrowseSessionLog
{
    public class ChatCoBrowseSessionLogChangeOwner : IPlugin
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
                EntityReference e_ChatSessionLog = (EntityReference)context.InputParameters["Target"];

                EntityReference e_SystemUser = (EntityReference)context.InputParameters["Assignee"];

                //get the chat session
                crme_chatcobrowsesessionlog chatsession = (crme_chatcobrowsesessionlog)serviceProxy.Retrieve
                    (e_ChatSessionLog.LogicalName,
                    e_ChatSessionLog.Id,
                    new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                if (chatsession == null)
                    throw new InvalidPluginExecutionException("Could not find chat session record using: " + e_ChatSessionLog.Id.ToString());

                //  get the phone call record to be updated
                PhoneCall phoneCall = (PhoneCall)serviceProxy.Retrieve("phonecall",
                        chatsession.crme_PhoneCallId.Id,
                        new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                if (phoneCall == null)
                    throw new InvalidPluginExecutionException("Could not find phone call record using: " + chatsession.crme_PhoneCallId.Id.ToString());


                //update Recipient of the Phone Call
                Entity toParty = new Entity("activityparty");

                toParty.Attributes.Add("partyid", new EntityReference("systemuser", e_SystemUser.Id));
                
                //remove the to field to avoid duplicate key issues
                phoneCall.Attributes.Remove("to");

                //now re-add the to field
                phoneCall.Attributes.Add("to", new Entity[] { toParty });
                


                //update the entity
                serviceProxy.Update(phoneCall);

                // Create the Request Object and Set the Request Object's Properties
                /*
                AssignRequest assign = new AssignRequest
                {
                    Assignee = new EntityReference(SystemUser.EntityLogicalName,
                        e_SystemUser.Id),
                    Target = new EntityReference(PhoneCall.EntityLogicalName,
                        phoneCall.Id)
                };

                serviceProxy.Execute(assign);
                */
            }
            else
            {
                return;
            }
        }

    }
}
