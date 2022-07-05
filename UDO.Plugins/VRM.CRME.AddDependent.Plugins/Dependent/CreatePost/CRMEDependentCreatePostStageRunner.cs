using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using VA.UDO.Plugins.Helpers;
using VRM.IntegrationServicebus.AddDependent.CrmModel;


namespace VRM.CRME.Plugin.DependentMaintenance
{
    //Purpose:  Populate Veteran Information on the Dependent Maintenance Form.
    public class CrmeDependentCreatePostStageRunner : MCSPlugins.PluginRunner
    {
        #region Constructor
        public CrmeDependentCreatePostStageRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
        #endregion

        #region Inernal Methods/Properties
        internal void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                //Avoid Recursive Calls - Abort if depth is greater than 1
                if (PluginExecutionContext.Depth > 1)
                    return;

                //Get Primary Entity
                var primaryEntity = GetPrimaryEntity();
                
                //Only process crme_dependent types
                if (primaryEntity.LogicalName != "crme_dependent")
                    return;

                //Create Earlybound crme_dependent entity object
                var dependent = GetPrimaryEntity().ToEntity<crme_dependent>();

                //Processing Guard Conditions
                if (dependent == null ||
                    dependent.crme_DependentRelationship == null ||
                    dependent.crme_MaintenanceType == null ||
                    dependent.crme_MaintenanceType.Value != 935950000 ||
                    dependent.crme_DependentRelationship.Value != 935950001)
                    return;

                //Assign Marital History Properties
                //CreateNewMaritalHistory(dependent);

            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Logger.WriteToFile(ex.Message);

                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("custom"))
                {
                    Logger.WriteDebugMessage(ex.Message.Substring(6));

                    throw new InvalidPluginExecutionException(ex.Message.Substring(6));
                }

                Logger.setMethod = "Execute";

                Logger.WriteToFile(ex.Message);

                throw new InvalidPluginExecutionException(ex.Message);
            }            
        }

        private void CreateNewMaritalHistory(crme_dependent dependent)
        {
            var crmMaritalHistory = new crme_maritalhistory();

            crmMaritalHistory.MapMaritalHistoryInfo(dependent);

            OrganizationService.Create(crmMaritalHistory);
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
