using System;
using System.Linq;
using MCSHelperClass;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using System.Security;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xrm.Sdk.Query;

namespace Va.Udo.Crm.Interactions.Plugins
{
    internal class PostRetrieveInteractionRunner : PluginRunner
    {
     
        internal bool _logSoap;
        internal bool _logTimer;
        internal bool _debug;
        internal string _uri;
        public PostRetrieveInteractionRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.OutputParameters["BusinessEntity"] as Entity;
        }
        public override Entity GetSecondaryEntity()
        {
            return null;
        }

        public override string McsSettingsDebugField
        {
            get { return "udo_interaction"; }
        }
        public T GetAttributeValue<T>(string field)
        {
            var primary = GetPrimaryEntity();
            if (primary != null && primary.Contains(field)) return primary.GetAttributeValue<T>(field);
            return default(T);
        }
        /// <summary>
        /// The primary execute method initiated from the plugin and the entry function to the runner class
        /// </summary>
        internal void Execute()
        {
            TracingService.Trace("top of execute - 3");
            var target = PluginExecutionContext.OutputParameters["BusinessEntity"] as Entity;
            if (target == null || !target.LogicalName.Equals("udo_interaction", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("No Target udo_interaction found.");
            }
            //Logger.WriteDebugMessage("{0}: Starting Service Request Post Update Service Request Runner", PluginExecutionContext.MessageName);
            var interactionId = Guid.Empty;

            interactionId = GetAttributeValue<Guid>("udo_interactionid");
            if (interactionId == Guid.Empty)
            {
                TracingService.Trace("NO INTERACTIONID");
                Logger.WriteDebugMessage("NO INTERACTIONID");
                return;
            }

            TracingService.Trace("INTERACTIONID:" + interactionId);
            try
            {
                var fetchXML = "<fetch>" +
            "<entity name='contact' >" +
            "<attribute name='udo_optoutvbatextemail' />" +
            "<attribute name='udo_securitypin' />" +
            "<attribute name='udo_optoutlastupdated' />" +
            "<attribute name='lastname' />" +
            "<link-entity name='udo_idproof' from='udo_veteran' to='contactid' link-type='inner' >" +
            "<link-entity name='udo_interaction' from='udo_interactionid' to='udo_interaction' link-type='inner' >" +
            "<filter type='and' >" +
            "<condition attribute='udo_interactionid' operator='eq' value='" + interactionId + "' />" +
            "</filter>" +
            "</link-entity>" +
            "</link-entity>" +
            "</entity>" +
            "</fetch>";

                TracingService.Trace("fetchXML:" + fetchXML);
                EntityCollection contacts = OrganizationService.RetrieveMultiple(new FetchExpression(fetchXML));

                if (contacts != null)
                {
                    TracingService.Trace("contacts not null");
                    if (contacts.Entities !=null)
                    {
                        TracingService.Trace("contacts.Entities not null");
                        if (contacts.Entities.Count > 0)
                        {
                            TracingService.Trace("contacts.Entities.COUNT:" + contacts.Entities.Count);
                            TracingService.Trace("got contacts");
                            if (contacts.Entities[0].Contains("udo_optoutofvbatextsemails"))
                            {
                                target["udo_optoutofvbatextsemails"] = contacts.Entities[0].GetAttributeValue<bool>("udo_optoutofvbatextsemails");
                                TracingService.Trace("udo_optoutofvbatextsemails:" + contacts.Entities[0].GetAttributeValue<bool>("udo_optoutofvbatextsemails"));
                            }
                            else
                            {
                                TracingService.Trace("udo_optoutofvbatextsemails DOES NOT EXIST");
                            }
                            if (contacts.Entities[0].Contains("udo_securitypin"))
                            {
                                target["udo_securitypin"] = contacts.Entities[0].GetAttributeValue<string>("udo_securitypin");
                                TracingService.Trace("udo_securitypin:" + contacts.Entities[0].GetAttributeValue<string>("udo_securitypin"));
                            }
                            else
                            {
                                TracingService.Trace("udo_securitypin DOES NOT EXIST");
                            }
                            PluginExecutionContext.OutputParameters["BusinessEntity"] = target;
                        }
                        else
                        {
                            TracingService.Trace("no records returned1");

                        }
                    }
                    else
                    {
                        TracingService.Trace("no records returned");

                    }
                }
                else
                {
                    TracingService.Trace("no records returned2 ");

                }

            }
            catch (InvalidPluginExecutionException ex)
            {
                TracingService.Trace("ex.m" + ex.Message);
                throw ex;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                TracingService.Trace("ex.m" + ex.Message);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
            catch (Exception ex)
            {
                TracingService.Trace("ex.m" + ex.Message);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
        }
      
    }
}
