using MCSPlugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Va.Udo.Crm.Plugins.Security
{
    /// <summary>
    /// Contains the business logic to assign the interaction to the default team of the business unit.
    /// </summary>
    internal class AssignmentRunner : PluginRunner
    {
        private const string _TeamName = "PCR";
        private readonly string preImageAlias = "PreImage";
        //private readonly string postImageAlias = "PostImage";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        public AssignmentRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

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
            return (PluginExecutionContext.PreEntityImages != null && PluginExecutionContext.PreEntityImages.Contains(this.preImageAlias)) ? PluginExecutionContext.PreEntityImages[this.preImageAlias] as Entity : null;
        }

        /// <summary>
        /// Debug field
        /// </summary>
        public override string McsSettingsDebugField
        {
            get { return "udo_security"; }
        }


        /// <summary>
        /// Used for setting senitivity level
        /// Added by BG 2/17/15
        /// </summary>
        /// <param name="LookupField"></param>
        internal void Execute(string LookupField)
        {
            // Logger.WriteDebugMessage("top of security");
            var message = PluginExecutionContext.MessageName;
            TracingService.Trace("Message: " + message);

            if (!message.Equals("Create", StringComparison.InvariantCultureIgnoreCase) &&
                !message.Equals("Update", StringComparison.InvariantCultureIgnoreCase))
            {
                return; // unsupported message
            }

            // Entity variable
            Entity entity = GetPrimaryEntity();
            if (entity == null)
                throw new InvalidPluginExecutionException("Target entity is null");

            TracingService.Trace("Target entity is not null.");

            // Field variables - get contact lookup and owner from entity
            EntityReference owner = entity.GetAttributeValue<EntityReference>("ownerid");
            EntityReference lookup = entity.GetAttributeValue<EntityReference>(LookupField);

            TracingService.Trace("Lookup Field: " + LookupField);

            if (owner != null)
                TracingService.Trace("Owner: ", owner.Id);
            else
                TracingService.Trace("Owner: <null>");

            if (lookup != null)
                TracingService.Trace("Lookup: ", lookup.Id);
            else
                TracingService.Trace("Lookup: <null>");

            // if Updating, consider case where filtered attribute to contact lookup is changed
            //since Property Bag from Update entity will not contain these attributes unless changed, use the entity image if needed
            if (String.Equals(message, "Update", StringComparison.InvariantCultureIgnoreCase))
            {
                var pre = GetSecondaryEntity();
                if (owner == null) owner = pre.GetAttributeValue<EntityReference>("ownerid");
                if (lookup == null) lookup = pre.GetAttributeValue<EntityReference>(LookupField);
            }

            if (owner == null || owner.LogicalName != "team")
            {
                TracingService.Trace("Owner is not a team.");
                if (lookup == null)
                {
                    throw new InvalidPluginExecutionException("Lookup field to contact is missing.");
                }

                //Logger.WriteDebugMessage(string.Format("in AssignmentRunner code, entity Name: {0}", PluginExecutionContext.PrimaryEntityName));

                TracingService.Trace("Getting Sensitivity Team");
                
                owner = GetSensitivityTeam(lookup);
              
                if (owner != null)
                {
                    TracingService.Trace("Owner: {0}[{1}]", owner.LogicalName, owner.Id);

                    TracingService.Trace("Assigning Record");

                    AssignToTeam(entity.ToEntityReference(), owner);

                    TracingService.Trace("Assigned to team");
                }
            }
        }

        /// <summary>
        /// Added by BG 2/17/15
        /// Gets the related vet's owning team.
        /// </summary>
        /// <param name="contactLookup">EntityReference to the contact</param>
        /// <returns></returns>
        private EntityReference GetSensitivityTeam(EntityReference contactLookup)
        {
            if (contactLookup == null) return null;
            /*********************************************/
            var cols = new ColumnSet("ownerid", "udo_veteransensitivitylevel");
            var vet = base.OrganizationService.Retrieve("contact", contactLookup.Id, cols);

            // Get Owner
            if (vet==null) return null;
            
            var owner = vet.GetAttributeValue<EntityReference>("ownerid");
            // If team, return owner
            if (owner!=null && owner.LogicalName == "team") return owner;

            /*********************************************/
            // Get Sensitivity Level
            if (!vet.Contains("udo_veteransensitivitylevel"))
            {
                throw new InvalidPluginExecutionException("No udo_veteransensitivitylevel");
            }
            var sensitivitylevel = vet.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel");

            var sensitivitylevelValue = 752280000;
            if (sensitivitylevel != null) sensitivitylevelValue = sensitivitylevel.Value;

            // Make sure sensitivity level is valid
            //Logger.WriteDebugMessage("realLevel:" + sensitivitylevelValue);
            if (sensitivitylevelValue < 0)
            {
                throw new InvalidPluginExecutionException("Invalid udo_veteransensitivitylevel:" + sensitivitylevelValue);
            }

            owner = GetTeam(_TeamName, sensitivitylevelValue);

            AssignToTeam(contactLookup, owner);

            return owner;
        }

        private void AssignToTeam(EntityReference target, EntityReference team)
        {
            if (team == null || !team.LogicalName.Equals("team", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
                //throw new InvalidPluginExecutionException("Unable to assign record because assignee is null or not a team.");
            }
            AssignRequest req = new AssignRequest { Assignee = team, Target = target };

            TracingService.Trace("Executing Assign Request");
            try
            {
                var factory = (IOrganizationServiceFactory)ServiceProvider.GetService(typeof(IOrganizationServiceFactory));
                var service = factory.CreateOrganizationService(PluginExecutionContext.UserId);
                service.Execute(req);
            }
            catch (Exception ipe)
            {
                TracingService.Trace("error.....");
                TracingService.Trace("Error:{0}\r\n{1}", ipe.Message, ipe.StackTrace);
                if (ipe.InnerException != null)
                    TracingService.Trace("Internal Error: ", ipe.InnerException.Message);
                throw;
            }
            TracingService.Trace("Success");
            //Logger.WriteDebugMessage("Updated owner of {0} to {1} during assignmentrunner", target.LogicalName, team.LogicalName);
        }

        private EntityReference GetTeam(string name, int securitylevel)
        {
            var fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' count='1'>"
                      + @"<entity name='team'>"
                      + @"<attribute name='teamid' />"
                      + @"<filter type='and'>"
                      + @"<condition attribute='name' operator='eq' value='{0}' />"
                      + @"</filter>"
                      + @"<link-entity name='businessunit' from='businessunitid' to='businessunitid' alias='ad'>"
                      + @"<filter type='and'>"
                      + @"<condition attribute='udo_veteransensitivitylevel' operator='eq' value='{1}' />"
                      + @"</filter>"
                      + @"</link-entity>"
                      + @"</entity>"
                      + @"</fetch>";

            var query = String.Format(fetch, name, securitylevel);

            var fe = new FetchExpression(query);

            var result = base.OrganizationService.RetrieveMultiple(fe);

            if (result == null || result.Entities.Count == 0) return null;

            return result.Entities[0].ToEntityReference();
        }
    }
}
