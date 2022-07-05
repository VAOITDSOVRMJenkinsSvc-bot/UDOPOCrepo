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
    internal class PreRetrieveInteractionRunner : PluginRunner
    {

        internal bool _logSoap;
        internal bool _logTimer;
        internal bool _debug;
        internal string _uri;
        public PreRetrieveInteractionRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public override Entity GetPrimaryEntity()
        {
            return null;
        }
        public override Entity GetSecondaryEntity()
        {
            return null;
        }

        public override string McsSettingsDebugField
        {
            get { return "udo_interaction"; }
        }
        /// <summary>
        /// The primary execute method initiated from the plugin and the entry function to the runner class
        /// </summary>
        internal void Execute()
        {
            if (PluginExecutionContext.Depth > 1) return;
            var interactionId = PluginExecutionContext.PrimaryEntityId;

            TracingService.Trace("top of execute - just fields");
            var updateInteraction = false;
            var optin = false;
            var PIN = string.Empty;

            try
            {
                var fetchXML = "<fetch>" +
                        "<entity name='udo_idproof' >" +
                        "<filter type='and' >" +
                            "<condition attribute='udo_interaction' operator= 'eq' uitype='udo_interaction' value='{" + interactionId + "}' />" +
                        "</filter>" +
                        "<link-entity name='contact' from='contactid' to='udo_veteran' visible='false' link-type='outer' alias='vet' >" +
                            "<attribute name='udo_securitypin' alias='vetpin' />" +
                            "<attribute name='udo_optoutvbatextemail' alias='vetopt' />" +
                        "</link-entity >" +
                        "<link-entity name='udo_interaction' from='udo_interactionid' to='udo_interaction' visible='false' link-type='outer' alias='int' >" +
                            "<attribute name='udo_securitypin' alias='intpin'/>" +
                            "<attribute name='udo_optoutofvbatextsemails' alias='intopt' />" +
                        "</link-entity>" +
                        "</entity>" +
                        "</fetch>";

                TracingService.Trace("fetchXML:" + fetchXML);
                EntityCollection contacts = OrganizationService.RetrieveMultiple(new FetchExpression(fetchXML));

                if (contacts != null)
                {
                    TracingService.Trace("udo_idproof not null");
                    if (contacts.Entities != null)
                    {
                        TracingService.Trace("udo_idproof.Entities not null");
                        if (contacts.Entities.Count > 0)
                        {

                            TracingService.Trace("udo_idproof.Entities.COUNT:" + contacts.Entities.Count);
                            TracingService.Trace("got udo_idproof");

                            var vetpin = string.Empty;
                            var intpin = string.Empty;
                            var vetopt = false ;
                            var intopt = false;

                            foreach (var item in contacts.Entities[0].Attributes)
                            {
                                TracingService.Trace("item value:" + item.Value);
                                TracingService.Trace("item key:" + item.Key);

                                switch (item.Key)
                                {
                                    case "vetpin":
                                        vetpin = (string)((Microsoft.Xrm.Sdk.AliasedValue)item.Value).Value;
                                        break;
                                    case "vetopt":
                                        vetopt = (bool)((Microsoft.Xrm.Sdk.AliasedValue)item.Value).Value;
                                        break;
                                    case "intpin":
                                        intpin = (string)((Microsoft.Xrm.Sdk.AliasedValue)item.Value).Value;
                                        break;
                                    case "intopt":
                                        intopt = (bool)((Microsoft.Xrm.Sdk.AliasedValue)item.Value).Value;
                                        break;
                                    default:
                                        break;
                                }
                                if (item.Key != "udo_idproofid")
                                {
                                    TracingService.Trace("item keyvalue:" + ((Microsoft.Xrm.Sdk.AliasedValue)item.Value).Value);
                                }
                            }
                            if (vetopt != intopt)
                            {
                                if (!intopt)
                                {
                                    updateInteraction = true;
                                    optin = vetopt;
                                    TracingService.Trace("updating optin:" + vetopt);
                                }
                                else
                                {
                                    TracingService.Trace("INT is true");
                                }
                            }
                            else
                            {
                                TracingService.Trace("optin Match");
                            }
                            if (vetpin != intpin)
                            {
                                TracingService.Trace("pins don't match");
                                if (intpin == string.Empty)
                                {
                                    TracingService.Trace("OPT PIN EMPTY, LET'S UPDATE");
                                    updateInteraction = true;
                                    PIN = vetpin;
                                    TracingService.Trace("udo_securitypin:" + vetpin);
                                }
                                else
                                {
                                    updateInteraction = false;
                                    TracingService.Trace("optpin exists");
                                }
                            }
                            else
                            {
                                TracingService.Trace("PINS Match");
                            }
                            
                            if (updateInteraction)
                            {

                                TracingService.Trace("Need to update interaction");
                                Entity Interaction = new Entity();
                                Interaction.LogicalName = "udo_interaction";
                                Interaction.Id = interactionId;
                                Interaction["udo_optoutofvbatextsemails"] = optin;
                                Interaction["udo_securitypin"] = PIN;
                                OrganizationService.Update(Interaction);
                                TracingService.Trace("updated interaction");

                            }
                            else
                            {
                                TracingService.Trace("NO NEED to update interaction");
                            }
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
