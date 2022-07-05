using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;
using VRM.IntegrationServicebus.AddDependent.CrmModel;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    //Purpose:  Populate Veteran Information on the Dependent Maintenance Form.
    public class CrmeSpMarHistCreateUpdateDeletePreStageRunner : MCSPlugins.PluginRunner
    {
        #region Constructor
        public CrmeSpMarHistCreateUpdateDeletePreStageRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
        #endregion

        #region Inernal Methods/Properties
        internal void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                //if called from a plugin, I already validated this
                if (PluginExecutionContext.Depth > 1)
                    return;

               //Allow Deletion of Non-WebService Dependents
                CheckIfOkToContinue();
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

        private void CheckIfOkToContinue()
        {
            try
            {
                Logger.setMethod = "CheckIfOkToContinue";

                var crmDepMaint = GetCrmeDependentMaint();


                if (crmDepMaint.crme_txnStatus.Value != 935950000)
                {
                    var errorMessage =
                        "This action cannot continue because the Dependent Maintenance record it is related to has already been submitted.";

                    switch (PluginExecutionContext.MessageName)
                    {
                        case "update":
                            errorMessage =
                                "This record cannot be updated because the Dependent Maintenance record it is related to has already been submitted.";
                            break;
                        case "create":
                            errorMessage =
                                "This record cannot be created because the Dependent Maintenance record it is related to has already been submitted.";
                            break;
                        case "delete":
                            errorMessage =
                                "This record cannot be deleted because the Dependent Maintenance record it is related to has already been submitted.";
                            break;
                    }

                    throw new InvalidPluginExecutionException(errorMessage);
                }
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
        #endregion

        #region  Runner Methods
        public override string McsSettingsDebugField
        {
            get { return "crme_spousemaritalhistory"; }
        }

        public override Entity GetPrimaryEntity()
        {
            if (PluginExecutionContext.MessageName == "Create")
            {
                return (Entity)PluginExecutionContext.InputParameters["Target"];
            }

            return PluginExecutionContext.PreEntityImages["pre"];
        }

        public override Entity GetSecondaryEntity()
        {
            return (Entity)PluginExecutionContext.InputParameters["Target"];
        }
        #endregion
    }
}
