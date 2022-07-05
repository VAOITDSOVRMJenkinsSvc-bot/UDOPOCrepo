using System;
using System.Linq;
using CRMUD;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace CRM.Plugins.ChatCoBrowseSessionLog
{
    public class ChatCoBrowseSessionLogSetState : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var log = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var pluginContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var serviceProxy = serviceFactory.CreateOrganizationService(pluginContext.InitiatingUserId);

            EntityReference chatCoBrowseRef = pluginContext.InputParameters["EntityMoniker"] as EntityReference;

            if (chatCoBrowseRef != null)
            {
                ServiceContext serviceContext = new ServiceContext(serviceProxy);

                EntityReference phoneCallRef = serviceContext.crme_chatcobrowsesessionlogSet.Where(c => c.Id == chatCoBrowseRef.Id).Select(c => c.RegardingObjectId).FirstOrDefault();

                SetStateRequest setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference
                    {
                        LogicalName = PhoneCall.EntityLogicalName,
                        Id = phoneCallRef.Id
                    },
                    State = new OptionSetValue(1), // Completed
                    Status = new OptionSetValue((int)phonecall_statuscode.Received)
                };

                serviceProxy.Execute(setStateRequest);
            }
        }
    }
}
