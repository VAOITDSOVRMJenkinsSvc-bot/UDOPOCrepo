using MCSPlugins;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Va.Udo.Crm.Plugins.Common
{
    /// <summary>
    /// Duration update runner wraps the logic to find out the duration based on create on time and
    /// udo_endtime 
    /// </summary>
    internal class DurationUpdateRunner:PluginRunner
    {
        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DurationUpdateRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }
          
        /// <summary>
        /// Retrieve the primary entity
        /// </summary>
        /// <returns></returns>
        public override Microsoft.Xrm.Sdk.Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters.ContainsKey("Target") ? PluginExecutionContext.InputParameters["Target"] as Entity : null;
        }

        /// <summary>
        /// Get the secondary entity
        /// </summary>
        /// <returns></returns>
        public override Microsoft.Xrm.Sdk.Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.PreEntityImages.ContainsKey("PreImage") ? PluginExecutionContext.PreEntityImages["PreImage"] as Entity : null;
        }

        /// <summary>
        //  Set the debug field
        /// </summary>
        public override string McsSettingsDebugField
        {
            get { return null; }
        }

        /// <summary>
        /// Execution logic of the runner: It retrieves the start and end time from pre and target entities and computes the duration of the interaction/request.
        /// </summary>
        internal void Execute()
        {
            try
            {
                Trace("Start: DurationUpdateRunner");
                var entity = GetPrimaryEntity();
                if (entity == null)
                    throw new InvalidPluginExecutionException("Target entity is null");

                var preEntity = GetSecondaryEntity();
                if (preEntity == null)
                    throw new InvalidPluginExecutionException("Configure a pre image(alias:PreImage) with end time as attribute.");

                //Check if the pre entities are configured and restrict to udo_interaction and udo_request
                if (entity.LogicalName.Equals("udo_interaction", StringComparison.InvariantCultureIgnoreCase) ||
                    entity.LogicalName.Equals("udo_request", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (entity.Contains("udo_endtime") && preEntity.Contains("createdon"))
                    {
                        var start = preEntity.GetAttributeValue<DateTime>("createdon");
                        var end = entity.GetAttributeValue<DateTime>("udo_endtime");

                        var duration = (end - start).Duration();

                        entity["udo_duration"] = duration.TotalSeconds;
                    }
                }
            }
            catch (Exception ex)
            {
                PluginError = true;
                Trace(string.Format("{0}", ex.Message));
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
    }
}
