using System;
using CRMUD;
using Microsoft.Xrm.Sdk;
using CRM.Plugins;
using System.Linq;

namespace CRM.Plugins.ChatCoBrowseSessionLog
{
    public class ChatSessionLogPOSTUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var log = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var pluginContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var serviceProxy = serviceFactory.CreateOrganizationService(pluginContext.InitiatingUserId);

            //crme_chatcobrowsesessionlog chatCoBrowse = (crme_chatcobrowsesessionlog)pluginContext.PostEntityImages["crme_chatcobrowsesessionlog"];
            crme_chatcobrowsesessionlog chatCoBrowse = ((Entity)pluginContext.PostEntityImages["crme_chatcobrowsesessionlog"]).ToEntity<crme_chatcobrowsesessionlog>();

            if (chatCoBrowse != null)
            {
                string subject = null;

                if (!string.IsNullOrEmpty(chatCoBrowse.crme_ChatSessionId))
                {
                    subject = string.Format("Chat Id: {0}", chatCoBrowse.crme_ChatSessionId);
                }
                else if (!string.IsNullOrEmpty(chatCoBrowse.crme_CoBrowseSessionId))
                {
                    subject = string.Format("CoBrowse Id: {0}", chatCoBrowse.crme_CoBrowseSessionId);
                }

                //if (ChatUtility.ValidateChatSessionRecord(chatCoBrowse, pluginContext) == false)
                //{
                //    return;
                //}

                var phoneCallId = UpdatePhoneCall(serviceProxy, chatCoBrowse, subject);

            }
        }
        private Guid UpdatePhoneCall(IOrganizationService serviceProxy, crme_chatcobrowsesessionlog chatCoBrowse, string subject)
        {

            if (chatCoBrowse == null || chatCoBrowse.crme_PhoneCallId == null || chatCoBrowse.crme_PhoneCallId.Id == null)
            {
                return Guid.Empty;
            }



            PhoneCall phoneCall = (PhoneCall)serviceProxy.Retrieve(PhoneCall.EntityLogicalName,
                chatCoBrowse.crme_PhoneCallId.Id,
                new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

            if (phoneCall == null)
                throw new InvalidPluginExecutionException("Could not find phone call record using: " + chatCoBrowse.crme_PhoneCallId.Id.ToString());



            //map chat session log sensitivity level to phone call Veteran sensitivity level
            //phoneCall.va_SensitivityLevelValue = getSensitivityLevel(chatCoBrowse.crme_VeteranSensitivityLevel);

            //find the system user record so we can update the Recipient field
            ServiceContext serviceContext = new ServiceContext(serviceProxy);

            Guid sysUser = serviceContext.SystemUserSet.Where(c => c.DomainName == chatCoBrowse.crme_CallAgentId).Select(c => c.Id).FirstOrDefault();

            if (sysUser == null || sysUser == Guid.Empty)
                throw new InvalidPluginExecutionException("Could not find user record using Call Agent ID: " + chatCoBrowse.crme_CallAgentId);

            //update Recipient of the Phone Call
            Entity toParty = new Entity("activityparty");

            toParty.Attributes.Add("partyid", new EntityReference("systemuser", sysUser));

            //remove the to field to avoid duplicate key issues
            phoneCall.Attributes.Remove("to");

            //now re-add the to field
            phoneCall.Attributes.Add("to", new Entity[] { toParty });

            //If SEP, set Participant ID, else set SSN.  If we don't have either, then leave them blank
            if (chatCoBrowse.crme_VSOOrgId != null)
            {
                if (chatCoBrowse.crme_ParticipantId != null)
                {
                    phoneCall.va_ParticipantID = chatCoBrowse.crme_ParticipantId;
                }
                else
                {
                    phoneCall.va_ParticipantID = "";
                }
            }
            else
            {
                if (chatCoBrowse.crme_SSN != null)
                {
                    phoneCall.va_SSN = chatCoBrowse.crme_SSN;
                }
                else
                {
                    phoneCall.va_SSN = "";
                }
            }

            ChatUtility.parseCallTypes(phoneCall, chatCoBrowse);

            phoneCall.va_ChatCompleted = chatCoBrowse.crme_ChatCompleted;

            //map chat session log sensitivity level to phone call Veteran sensitivity level
            ChatUtility.getSensitivityLevel(phoneCall, chatCoBrowse.crme_VeteranSensitivityLevel);
            
            serviceProxy.Update(phoneCall);

            return phoneCall.Id;
        }

    }
}
