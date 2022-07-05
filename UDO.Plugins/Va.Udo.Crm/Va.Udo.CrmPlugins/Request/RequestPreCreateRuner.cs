using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Va.Udo.Crm.Plugins.Request
{
    /// <summary>
    /// Runs on the pre create of a request
    /// </summary>
    internal class RequestPreCreateRuner : PluginRunner
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        internal RequestPreCreateRuner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// Retrieves the primary entity
        /// </summary>
        /// <returns>Primary entity</returns>
        public override Microsoft.Xrm.Sdk.Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters.ContainsKey("Target") ? PluginExecutionContext.InputParameters["Target"] as Entity : null;
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
            get { return ""; }
        }


        /// <summary>
        /// Sets the repeat call field if the veteran has called for the same request type, subtype in the last two weeks
        /// </summary>
        internal void Execute()
        {
           try
            {
                Trace("Start: RequestPreCreateRuner");
                Trace("Getting Pre-Entity.");
                var entity = GetPrimaryEntity();
                if (entity == null)
                    throw new InvalidPluginExecutionException("Target entity is null");

                Trace("Target is not null.");
                if (!string.Equals(entity.LogicalName, "udo_request", StringComparison.InvariantCultureIgnoreCase))
                    throw new InvalidPluginExecutionException("This plugin is not configured for udo_request.");

                entity["udo_isrepeatcall"] = HasRelatedRequest(entity);
                //throw new Exception("blah blah");
            }
            finally
            {
                Trace("Entered Finally");
                SetupLogger();
                Trace("Set up logger done.");
                ExecuteFinally();
                Trace("Exit Finally");
            }
        }

        /// <summary>
        /// Gets the related requests created last week.
        /// </summary>
        /// <param name="entity">request entity</param>
        /// <returns></returns>
        private bool HasRelatedRequest(Entity entity)
        {
            Trace("Start: HasRelatedRequest");
            //If the veteran, type and subtype fields are not set during create return no match found.
            var vet = entity.GetAttributeValue<EntityReference>("udo_veteran");
            var type = entity.GetAttributeValue<EntityReference>("udo_type");
            var subtype = entity.GetAttributeValue<EntityReference>("udo_subtype");

            if (vet == null || type == null || subtype == null) return false;

            Trace("Building Filter");
            var filter = new FilterExpression()
                           {
                               Conditions =
                               {
                                    new ConditionExpression("udo_type",ConditionOperator.Equal,type.Id),
                                    new ConditionExpression("udo_subtype",ConditionOperator.Equal,subtype.Id),
                                    new ConditionExpression("udo_veteran",ConditionOperator.Equal,vet.Id),
                               }
                           };

            Trace("Getting Related Request Window.");
            var xweeks = McsSettings.GetSingleSetting<int>("udo_relatedrequestwindow");  // use settings cache
            Trace("Related Request Window: {0}", xweeks);
            if (xweeks == 0) return false; // or set to 2??

            filter.AddCondition("createdon", ConditionOperator.LastXWeeks, xweeks);

            var regarding = entity.GetAttributeValue<EntityReference>("udo_regarding");
            if (regarding != null)
            {
                filter.AddCondition("udo_regarding", ConditionOperator.Equal, regarding.Id);
            }
            
            EntityCollection result = null;
            try
            {
                var expression = new QueryExpression("udo_request")
                {
                    TopCount = 1,
                    NoLock = true,
                    ColumnSet = new ColumnSet("udo_requestid"),
                    Criteria = { Filters = { filter } }
                };
                Trace("Built Query Expression");
                result = base.OrganizationService.RetrieveMultiple(expression);
                Trace("Retrieve Multiple Executed");
            }
            catch (Exception ex)
            {
                PluginError = true;
                //Trace the error
                //base.TracingService.Trace(string.Format("{0}", ex.Message));
                Trace(string.Format("{0}", ex.Message));
                //Do not break the application flow
                return false;
            }
            return (result != null && result.Entities != null && result.Entities.Count > 0);
        }
    }
}
