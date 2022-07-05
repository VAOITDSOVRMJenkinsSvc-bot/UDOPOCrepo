using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRM.IntegrationServicebus.AddDependent.CrmModel;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class DependentMaintenanceHelper
    {

        public static bool ValidateHasDependents(OrganizationServiceContext context, Entity primary, Entity secondary)
        {
            bool valid = false;
            var id = primary.Id;
            var dependendentMaintenance = secondary.ToEntity<crme_dependentmaintenance>();
            var hasDependents = (from d in context.CreateQuery<crme_dependent>()
                                 where (d.crme_DependentMaintenance.Id == id) &&
                                       (d.crme_LegacyRecord == false)
                                 select d).ToList().Count > 0;

            if (!hasDependents)
            {
                if (primary.Contains("crme_hiddenerrormessage"))
                {
                    primary["crme_hiddenerrormessage"] = "Add at least one dependent before submitting.";
                }
                valid = false;
            }
            else
            {
                valid = true;
            }

            return valid;
            //throw new InvalidPluginExecutionException(
            //    "Add at least one dependent before submitting.");
        }

    }
}
