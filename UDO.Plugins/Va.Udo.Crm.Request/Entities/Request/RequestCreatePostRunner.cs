using System;
using MCSPlugins;
using System.Linq;
//using VRM.Integration.UDO.VeteranSnapShot.Messages;
using VRMRest;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using System.ServiceModel;
using System.Diagnostics;
using System.Threading;
using System.Text;

namespace Va.Udo.Crm.Request.Plugins
{
    internal class RequestCreatePostRunner : MCSPlugins.PluginRunner
    {
        public RequestCreatePostRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }
        #region Globals and inherrited
        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        public override Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        public override string McsSettingsDebugField
        {
            get { return "udo_vetsnapshot"; }
        }


        #endregion

        internal void Execute()
        {
            Entity target = GetPrimaryEntity();
            Entity image = PluginExecutionContext.PostEntityImages["PostCreateImage"];

            if (target.LogicalName != "udo_request" || PluginExecutionContext.MessageName != "Create" || !target.Contains("udo_veteran"))
            { return; }

            try
            {
                Entity contact = new Entity("contact");
                contact.Id = target.GetAttributeValue<EntityReference>("udo_veteran").Id;
                contact["udo_lastcalldatetime"] = target.GetAttributeValue<DateTime>("createdon");
                contact["udo_lastcalltype"] = image.GetAttributeValue<EntityReference>("udo_type");
                contact["udo_lastcallsubtype"] = image.GetAttributeValue<EntityReference>("udo_subtype");
                OrganizationService.Update(contact);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                //Logger.WriteToFile(ex.Message);
                TracingService.Trace(ex.Message);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
            catch (Exception ex)
            {
                //Logger.WriteToFile(ex.Message);
                TracingService.Trace(ex.Message);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
            finally
            {
                TracingService.Trace("Entered Finally");
                SetupLogger();
                TracingService.Trace("Set up logger done.");
                ExecuteFinally();
                TracingService.Trace("Exit Finally");
            }
        }
    }
}