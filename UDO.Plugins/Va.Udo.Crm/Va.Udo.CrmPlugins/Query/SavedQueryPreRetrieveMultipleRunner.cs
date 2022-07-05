using MCSPlugins;
using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System.Threading;

namespace Va.Udo.Crm.Plugins
{
    /// <summary>
    /// Contains the business logic to assign the interaction to the default team of the business unit.
    /// </summary>
    internal class SavedQueryPreRetrieveMultipleRunner : PluginRunner
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        public SavedQueryPreRetrieveMultipleRunner(IServiceProvider serviceProvider)
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
            return PluginExecutionContext.InputParameters["Target"] as Entity;
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
        internal void Execute()
        {

            if (PluginExecutionContext.InputParameters == null) return;

            if (PluginExecutionContext.InputParameters.Contains("Query") &&
                PluginExecutionContext.InputParameters["Query"] is QueryExpression)
            {
                /*only apply this action if the query is for 'views' or saved queries*/
                QueryExpression qe = (QueryExpression)PluginExecutionContext.InputParameters["Query"];

                if (qe.EntityName == "savedquery" && qe.Criteria != null && qe.Criteria.Conditions != null)
                {
                    // The query is modified to hide views that start with Hidden or LookupOnly in the View Name
                    qe.Criteria.AddCondition("name", ConditionOperator.NotLike, "Hidden%");
                    qe.Criteria.AddCondition("name", ConditionOperator.NotLike, "Lookup Only%");

                    PluginExecutionContext.InputParameters["Query"] = qe;
                }
            }
        }
    }
}
