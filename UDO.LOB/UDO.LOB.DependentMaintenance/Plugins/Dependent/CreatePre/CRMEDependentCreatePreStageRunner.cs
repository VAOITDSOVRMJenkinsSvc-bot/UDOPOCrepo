using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;
using VRM.IntegrationServicebus.AddDependent.CrmModel;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    //Purpose:  Populate Veteran Information on the Dependent Maintenance Form.
    public class CrmeDependentCreatePreStageRunner : MCSPlugins.PluginRunner
    {
        #region Constructor
        public CrmeDependentCreatePreStageRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
        #endregion

        #region Inernal Methods/Properties
        internal void Execute(IServiceProvider serviceProvider)
        {
            try
            {
				TracingService.Trace($"| >> Start { this.GetType().FullName}.Execute");
				//Avoid Recursive Calls - Abort if depth is greater than 1
				if (PluginExecutionContext.Depth > 1)
                    return;
           
                TracingService.Trace("proceeding");

                //Get Primary Entity
                var primaryEntity = GetPrimaryEntity();

                //Only process crme_dependent types
                if (primaryEntity.LogicalName != "crme_dependent")
                    return;

                TracingService.Trace("Can I create?");

                CheckIfOkToCreate();

                TracingService.Trace("Guess so!");

				TracingService.Trace($"| >> End { this.GetType().FullName}.Execute");
			}
            catch (FaultException<OrganizationServiceFault> ex)
            {
                TracingService.Trace(ex.Message);

                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("custom"))
                {
                    TracingService.Trace(ex.Message.Substring(6));

                    throw new InvalidPluginExecutionException(ex.Message.Substring(6));
                }

                Logger.setMethod = "Execute";

                TracingService.Trace(ex.Message);

                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        private crme_dependentmaintenance GetCrmeDependentMaint()
        {
            var col = new ColumnSet();

            col.AddColumn("crme_txnstatus");

            var crmeDependent = OrganizationService.Retrieve("crme_dependentmaintenance", McsHelper.getEntRefID("crme_dependentmaintenance"), col).ToEntity<crme_dependentmaintenance>();

            return crmeDependent;
        }

        private void CheckIfOkToCreate()
        {
            try
            {
                Logger.setMethod = "CheckIfOkToCreate";

                var crmDepMaint = GetCrmeDependentMaint();

                if (crmDepMaint.crme_txnStatus.Value != 935950000)
                    throw new InvalidPluginExecutionException(
                         "This record cannot be created because the Dependent Maintenance record it is related to has already been submitted.");
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                TracingService.Trace(ex.Message);

                throw new InvalidPluginExecutionException(ex.Message);
            }
            catch (Exception ex)
            {
                TracingService.Trace(ex.Message);

                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        private static void SetDefaultValues(crme_dependent dependent)
        {
            /*dependent.crme_MaintenanceType.Value =935950000;
            dependent.crme_DependentRelationship.Value = 935950001;
            dependent.crme_SSN = "555-55-5555";
            dependent.crme_FirstName = "David";
            dependent.crme_LastName = "Foley";
            dependent.crme_MonthlyContributiontoSpouseSupport.Value = 0;
            dependent.crme_MarriageDate = DateTime.Now;
            dependent.crme_MarriageCountry = "USA";
            dependent.crme_MarriageCity = "Frisco";
            dependent.crme_MarriageState = "Tx";
            dependent.crme_DOB = DateTime.Parse("10/16/1965");*/
        }

        #endregion

        #region  Runner Methods
        public override string McsSettingsDebugField
        {
            get { return "crme_dependent"; }
        }

        public override Entity GetPrimaryEntity()
        {
            return (Entity)PluginExecutionContext.InputParameters["Target"];
        }

        public override Entity GetSecondaryEntity()
        {
            return (Entity)PluginExecutionContext.InputParameters["Target"];
        }
        #endregion
    }
}
