using MCSPlugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
namespace Va.Udo.Crm.Plugins.Person
{
    public class PersonFiduciaryExistsRunner : PluginRunner
    {
        public PersonFiduciaryExistsRunner(IServiceProvider serviceProvider)
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
            get { return "udo_contact"; }
        }

        public void Execute()
        {

            var entity = GetPrimaryEntity();

            if (entity == null || !entity.LogicalName.Equals("udo_person", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
            
            var req = new OrganizationRequest("udo_GetFiduciaryExists");
            req["ParentEntityReference"] = entity.ToEntityReference();  

            var response = OrganizationService.Execute(req);
        }
    }
}
