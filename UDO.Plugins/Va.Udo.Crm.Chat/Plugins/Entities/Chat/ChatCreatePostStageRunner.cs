using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
//using mcsScheduling;
//Replace the namespace 'mcsScheduling' with the namespcae of the xrm helper class you create here. 
//See mcsExtensions as an example. 
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using MCSHelperClass;
using MCSUtilities2011;
using UDO.Model;


namespace Va.Udo.Crm.Chat.Plugins
{
	//Purpose:  The purpose of this plugin is to create the system table entries needed for services to work - this is done on create and may have to be updated on update depending on
	//           The business rule that is decided.

	public class ChatCreatePostStageRunner : MCSPlugins.PluginRunner
	{
		#region Constructor
		public ChatCreatePostStageRunner(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
		}
		#endregion

        #region fields

        private EntityReference CallAgentOwner;

        #endregion fields

		#region Internal Methods/Properties
		internal void Execute()
		{
			try
			{
				TracingService.Trace("at the top");
				Logger.setMethod = "Execute";
                //Logger.WriteTxnTimingMessage(String.Format("Starting : {0}", GetType()));
                //Logger.WriteDebugMessage("Starting");
                //            Logger.WriteDebugMessage(String.Format("At the top of: {0}", GetType()));
                TracingService.Trace(String.Format("Starting : {0}", GetType()));

                CallAgentOwner = null;
				//checks to make sure the registration is correct
				if (!PluginExecutionContext.InputParameters.Contains("Target") ||
					!(PluginExecutionContext.InputParameters["Target"] is Entity))
					return;

				//Logger.WriteGranularTimingMessage("Starting ChatSessionLog processing");
                TracingService.Trace("Starting ChatSessionLog processing");

                var chatSessionLog = GetPrimaryEntity().ToEntity<crme_chatcobrowsesessionlog>();
                if (chatSessionLog != null)
                {
                    if (string.IsNullOrEmpty(chatSessionLog.crme_ChatSessionId))
                    {
                        throw new InvalidPluginExecutionException("The Chat Plugin requires the ChatSessionId to proceed");
                    }
                    var subject = string.Format("Chat Id: {0}", chatSessionLog.crme_ChatSessionId);
                    //Create "Chat" Interaction and link to Chat Session Log
                    var InteractionId = CreateInteraction(OrganizationService, chatSessionLog);

                    //Link InteractionId to new Interaction
                    if (InteractionId == Guid.Empty)
                        throw new InvalidPluginExecutionException("The Chat Plugin failed to create an Interaction");

                    chatSessionLog.udo_InteractionId = new EntityReference()
                    {
                        Name = subject,
                        LogicalName = UDO.Model.udo_interaction.EntityLogicalName,
                        Id = InteractionId
                    };

                    if(CallAgentOwner != null)
                        chatSessionLog.OwnerId = CallAgentOwner;

                    
                    if (chatSessionLog.udo_InteractionId != null && chatSessionLog.udo_InteractionId.Id != Guid.Empty)
                    {
                        //launchUrl is designed for USD Default CTI Listener
                        var url = "http://localhost:5000";
                        chatSessionLog.crme_LaunchUrl = string.Format("{0}/?InteractionId={1}&ChatSessionLogId={2}", url, chatSessionLog.udo_InteractionId.Id.ToString(), chatSessionLog.Id.ToString());
                    }
                   
                    //Plugin+ VIMT executions could result in "EntityState must be set to null, Created or Changed" exception message without setting here
                    chatSessionLog.EntityState = EntityState.Changed;
                    OrganizationService.Update(chatSessionLog);

                    if (chatSessionLog.crme_ChatCompleted == true)
                    {
                        EntityReference moniker = new EntityReference();
                        moniker.LogicalName = "crme_chatcobrowsesessionlog";
                        moniker.Id = chatSessionLog.Id;

                        OrganizationRequest req = new OrganizationRequest()
                        {
                            RequestName = "SetState"
                        };
                        req["State"] = new OptionSetValue(1);
                        req["Status"] = new OptionSetValue(2);
                        req["EntityMoniker"] = moniker;
                        OrganizationService.Execute(req);

                    }
                }
                else
                {
                    throw new InvalidPluginExecutionException("Target entity is null");
                }

				//Logger.WriteGranularTimingMessage("Finish ChatSessionLog processing");
				//Logger.WriteDebugMessage("Ending");
				//Logger.WriteTxnTimingMessage(String.Format("Ending : {0}", GetType()));
                TracingService.Trace(String.Format("Ending : {0}", GetType()));

            }
			catch (FaultException<OrganizationServiceFault> ex)
			{
				Logger.WriteToFile(ex.Message);
				throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
			}
			catch (Exception ex)
			{
				Logger.WriteToFile(ex.Message);
				throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
			}
		}

        private Guid CreateInteraction(IOrganizationService serviceProxy, crme_chatcobrowsesessionlog chatCoBrowse)
        {

            using (var xrm = new UDOContext(OrganizationService))
            {
                //DomainName is AD User name in CRM
                var sysUser = xrm.SystemUserSet.Where(c => c.DomainName == chatCoBrowse.crme_CallAgentId).Select(c => new {Id = c.Id, LogicalName = c.LogicalName, FullName = c.FullName, udo_AccessLevel = c.udo_AccessLevel}).FirstOrDefault();
                if (sysUser == null || sysUser.Id == Guid.Empty)
                    throw new InvalidPluginExecutionException("Could not find user record using Call Agent ID: " + chatCoBrowse.crme_CallAgentId);
                udo_interaction interaction = new udo_interaction()
                {
                    udo_title = sysUser.FullName + " at " + DateTime.Now,
                    udo_PCRSensitivityLevel = sysUser.udo_AccessLevel,
                    udo_Channel = new OptionSetValue(752280001),
                    udo_ChatSessionId = chatCoBrowse.crme_ChatSessionId,
                    OwnerId = new EntityReference
                    {
                        Id = sysUser.Id,
                        LogicalName = sysUser.LogicalName
                    }
                };
                CallAgentOwner = interaction.OwnerId;
                return serviceProxy.Create(interaction);
            }

        }
        
		public override string McsSettingsDebugField
		{
			get { return "mcs_patresourcegroupplugin"; }
		}

		public override Entity GetPrimaryEntity()
		{
			return (Entity)PluginExecutionContext.InputParameters["Target"];
		}

		public override Entity GetSecondaryEntity()
		{
			return (Entity)PluginExecutionContext.InputParameters["Target"];
		}
		#endregion

    }
}

