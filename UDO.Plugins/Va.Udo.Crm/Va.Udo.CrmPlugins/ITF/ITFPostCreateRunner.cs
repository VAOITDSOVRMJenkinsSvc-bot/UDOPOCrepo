using MCSPlugins;
using Microsoft.Xrm.Sdk;
using System;

namespace Va.Udo.Crm.Plugins.ITF
{
    public class ITFPostCreateRunner : PluginRunner
    {
        public ITFPostCreateRunner(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        /// <summary>
        /// Retrieves the primary entity
        /// </summary>
        /// <returns>Primary entity</returns>
        public override Microsoft.Xrm.Sdk.Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        /// <summary>
        /// Retrieves the secondary entity
        /// </summary>
        /// <returns></returns>
        public override Microsoft.Xrm.Sdk.Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.PreEntityImages["pre"] as Entity;
        }

        /// <summary>
        /// Debug field
        /// </summary>
        public override string McsSettingsDebugField
        {
            get { return "udo_itf"; }
        }

        public void Execute()
        {
            var entity = GetPrimaryEntity();

            if (entity == null || !entity.LogicalName.Equals("va_intenttofile", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var req = new OrganizationRequest("udo_SubmitITF");
            req["ParentEntityReference"] = entity.ToEntityReference();
            req["Target"] = entity.ToEntityReference();
            
            var response = OrganizationService.Execute(req);
        }
    }
}
