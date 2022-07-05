using System.Linq;
using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;
using VRM.IntegrationServicebus.AddDependent.CrmModel;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class CRMEDependentDeletePreStageRunner : MCSPlugins.PluginRunner
    {
        #region Constructor
        public CRMEDependentDeletePreStageRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
        #endregion

        #region Internal Methods/Properties
        internal void Execute()
        {
            try
            {
                //Avoid Recursive Calls - Abort if depth is greater than 1
                if (PluginExecutionContext.Depth > 1)
                    return;

                //Get Primary Entity
                var primaryEntityReference = GetPrimaryEntityReference();

                //Only process crme_dependent types
                if (primaryEntityReference.LogicalName != "crme_dependent")
                    return;

                Logger.WriteDebugMessage("See if status is Draft");

                //Get CrmeDependent Enttity
                CheckIfOkToDeleteBasedonStatus();

                Logger.WriteDebugMessage("Make sure this isn't a legacy dependent");

                //Allow Deletion of Non-WebService Dependents
                CheckIfOkToDelete();

                Logger.WriteDebugMessage("Delete any marital history records");

                //Delete All Marital History for Matching Dependent
                DeleteMaritalHistory();
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Logger.WriteToFile(ex.Message);

                throw new InvalidPluginExecutionException(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.WriteToFile(ex.Message);

                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        private crme_dependentmaintenance GetCrmeDependentMaint()
        {
            var col = new ColumnSet();

            col.AddColumn("crme_txnstatus");

            var crmeDependent = OrganizationService.Retrieve("crme_dependentmaintenance", 
                McsHelper.getEntRefID("crme_dependentmaintenance"), col).ToEntity<crme_dependentmaintenance>();

            return crmeDependent;
        }

        private void CheckIfOkToDeleteBasedonStatus()
        {
            try
            {
                Logger.setMethod = "CheckIfOkToCreateBasedonStatus";

                var crmDepMaint = GetCrmeDependentMaint();

                if (crmDepMaint.crme_txnStatus.Value != 935950000)
                    throw new InvalidPluginExecutionException(
                        "This record cannot be deleted because the Dependent Maintenance record it is related to has already been submitted.");
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Logger.WriteToFile(ex.Message);

                throw new InvalidPluginExecutionException(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.WriteToFile(ex.Message);

                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        private void DeleteMaritalHistory()
        {
            using (var srv = new VRMXRM(OrganizationService))
            {
                try
                {
                    var marHist =
                        srv.crme_maritalhistorySet.FirstOrDefault(
                            i => i.crme_spousessn == McsHelper.getStringValue("crme_ssn"));

                    if (marHist != null)
                    {
                        OrganizationService.Delete(crme_maritalhistory.EntityLogicalName, marHist.Id);
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    Logger.WriteToFile(ex.Message);

                    throw new InvalidPluginExecutionException(ex.Message);
                }
                catch (Exception ex)
                {
                    Logger.WriteToFile(ex.Message);

                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
        }

        private void CheckIfOkToDelete()
        {
            if (McsHelper.getBoolValue("crme_legacyrecord"))
                throw new InvalidPluginExecutionException(
                    "This record was retrieved using a Web Service.  Unable to delete the selected record.");
        }

        public override string McsSettingsDebugField
        {
            get { return "crme_dependent"; }
        }

        //For Create plugins there is no PRE image.
        //for anything else PRE images can exist on a pre-stage plugin but POST image does not.
        // For any other POST stage plugins PRE and POST exist.

        //since target only contains what was changed - the purpose of the helper is to be able to get a value of a field that may or may not have been updated.
        //For pre-stage plugins this can be difficult and require lots of coding as you want to use the target if the user updated the field and the property exists in the target - but if it
        //doesn't exist then the pre-image would hold the value.  That can be 4 lines of code, or more, for each attribute.

        //however for a post stage plugin if you want the post value then make the thisEntity and preEntity both contain the post image.
        //though not common it is possible for a post image to be different than the target.  
        //One scenario is phone number where maybe the phone number in target is 123-4567890 and the post it is 
        //(123) 456-7890.  This happens if a plugin changes the value prior to it getting to the db.

        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.PreEntityImages["pre"];
        }

        public EntityReference GetPrimaryEntityReference()
        {
            return (EntityReference)PluginExecutionContext.InputParameters["Target"];
        }

        public override Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.PreEntityImages["pre"];
        }
    }
    #endregion
}


