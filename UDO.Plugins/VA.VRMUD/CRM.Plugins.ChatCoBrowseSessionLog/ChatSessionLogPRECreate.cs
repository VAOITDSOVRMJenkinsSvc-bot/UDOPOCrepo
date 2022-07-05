using System;
using CRMUD;
using Microsoft.Xrm.Sdk;
using CRM.Plugins;
using System.Linq;

namespace CRM.Plugins.ChatCoBrowseSessionLog
{
    public class ChatSessionLogPRECreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var log = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var pluginContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var serviceProxy = serviceFactory.CreateOrganizationService(pluginContext.InitiatingUserId);

            crme_chatcobrowsesessionlog chatCoBrowse = ((Entity)pluginContext.InputParameters["Target"]).ToEntity<crme_chatcobrowsesessionlog>();

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

                var phoneCallId = CreatePhoneCall(serviceProxy, chatCoBrowse, subject);

                chatCoBrowse.RegardingObjectId = new EntityReference()
                {
                    Name = subject,
                    LogicalName = PhoneCall.EntityLogicalName,
                    Id = phoneCallId
                };

                chatCoBrowse.crme_PhoneCallId = new EntityReference()
                {
                    Name = subject,
                    LogicalName = PhoneCall.EntityLogicalName,
                    Id = phoneCallId
                };


                string path = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                path = path.Substring(0, path.IndexOf(".gov/") + 5);
                chatCoBrowse.crme_LaunchUrl = string.Format("{0}main.aspx?etn=phonecall&id={1}&pagetype=entityrecord", path, chatCoBrowse.RegardingObjectId.Id.ToString());
           
            }
        }
        private Guid CreatePhoneCall(IOrganizationService serviceProxy, crme_chatcobrowsesessionlog chatCoBrowse, string subject)
        {
            PhoneCall phoneCall = new PhoneCall()
            {
                Subject = subject,
                va_SessionType = chatCoBrowse.crme_SessionType,
                va_ChatSessionId = chatCoBrowse.crme_ChatSessionId,
                va_CoBrowseSessionId = chatCoBrowse.crme_CoBrowseSessionId,
                va_CallAgentId = chatCoBrowse.crme_CallAgentId,
                va_VSOId = chatCoBrowse.crme_VSOId,
                va_VSOAgentId = chatCoBrowse.crme_VSOAgentId,
                va_EDIPI = chatCoBrowse.crme_EDIPI,
                va_CoBrowseSessionIndicator = !string.IsNullOrEmpty(chatCoBrowse.crme_CoBrowseSessionId),
                va_ChatCompleted = chatCoBrowse.crme_ChatCompleted
            };




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

            //map chat session log sensitivity level to phone call Veteran sensitivity level
            ChatUtility.getSensitivityLevel(phoneCall, chatCoBrowse.crme_VeteranSensitivityLevel);

            return serviceProxy.Create(phoneCall);
        }


    }
}
