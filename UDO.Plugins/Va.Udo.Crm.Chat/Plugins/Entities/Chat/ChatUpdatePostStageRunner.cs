using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using MCSHelperClass;
using MCSUtilities2011;
using UDO.Model;


namespace Va.Udo.Crm.Chat.Plugins
{


    public class ChatUpdatePostStageRunner : MCSPlugins.PluginRunner
    {
        #region Constructor
        public ChatUpdatePostStageRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
        #endregion

        #region Internal Methods/Properties
        internal void Execute()
        {
            //avoid update looping
            if (PluginExecutionContext.Depth > 1) return;
            try
            {
                TracingService.Trace("at the top");

                Logger.setMethod = "Execute";
                //start the timing for the plugin
                //Logger.WriteTxnTimingMessage(String.Format("Starting : {0}", GetType()));
                //Logger.WriteDebugMessage("Starting");

                //Logger.WriteDebugMessage(String.Format("At the top of: {0}", GetType()));
                TracingService.Trace(String.Format("At the top of: {0}", GetType()));


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

        
        public override string McsSettingsDebugField
        {
            get { return "mcs_patresourcegroupplugin"; }
        }

        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.PostEntityImages["Post"] as Entity;
        }

        public override Entity GetSecondaryEntity()
        {
            return (Entity)PluginExecutionContext.InputParameters["Target"];
        }
        #endregion

    }
}

