using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
//using UDO.Model;
using System.ServiceModel;
using System.Diagnostics;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace VRM.Integration.UDO.Notes.Plugins
{
    internal class ForceDeleteNotesPostRunner : PluginRunner
    {
        public ForceDeleteNotesPostRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }
        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.PreEntityImages["Pre"] as Entity;
        }
        public override Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.PreEntityImages["Pre"] as Entity;
        }
        public override string McsSettingsDebugField
        {
            get { return "udo_notes"; }
        }
        internal void Execute(IServiceProvider serviceProvider)
        {

            try
            {
                // Allow if Depth is more than 1 (i.e. not a direct delete), but don't delete it in the subsystem.
                //if (PluginExecutionContext.RequestId.Equals(Guid.Parse())) return;

                var target = PluginExecutionContext.InputParameters["Target"] as EntityReference;
                var authKey = PluginExecutionContext.InputParameters["AuthorizationKey"] as string;
                //Logger.WriteDebugMessage("Executing ForceDelete... Target Id: {0}  AuthKey: {1}", 
                    //(target == null ? "" : target.Id.ToString()),
                    //(authKey == null ? "" : authKey));
                TracingService.Trace("Executing ForceDelete... Target Id: {0}  AuthKey: {1}",
                    (target == null ? "" : target.Id.ToString()),
                    (authKey == null ? "" : authKey));

                if (target == null || authKey == null) return;

                if (ValidKey(authKey))
                {
                    DeleteRequest dr = new DeleteRequest();
                    dr.Target = target;
                    OrganizationService.Execute(dr);
                }

            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                if (!ex.Message.StartsWith("custom"))
                {

                    Logger.WriteToFile(ex.Message);
                    throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
                }
                throw new InvalidPluginExecutionException(ex.Message.Substring(6));

            }
            catch (Exception ex)
            {
                if (!ex.Message.StartsWith("custom"))
                {

                    Logger.WriteToFile(ex.Message);
                    throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
                }
                throw new InvalidPluginExecutionException(ex.Message.Substring(6));
            }
        }

        private bool ValidKey(string authKey)
        {
            return (authKey == "udo_delete_now");
        }
    }
}