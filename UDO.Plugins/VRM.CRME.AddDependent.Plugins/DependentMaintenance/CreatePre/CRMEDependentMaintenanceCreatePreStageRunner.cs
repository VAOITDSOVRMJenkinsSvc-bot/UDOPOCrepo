using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Client;
using VRM.IntegrationServicebus.AddDependent.CrmModel;


namespace VRM.CRME.Plugin.DependentMaintenance
{
    //Purpose:  Populate Veteran Information on the Dependent Maintenance Form.
    public class CrmeCreatePreDependentMaintenanceRunner : MCSPlugins.PluginRunner
    {
        #region Constructor
        public CrmeCreatePreDependentMaintenanceRunner(IServiceProvider serviceProvider)
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

                //Create Earlybound crme_dependentmaintenance entity object
                var dependentMaintenance = GetPrimaryEntity().ToEntity<crme_dependentmaintenance>();
                ////existing code to grab the crme_claimdate
                //dependentMaintenance.crme_ClaimDate = DateTime.Today;

                Logger.WriteGranularTimingMessage("Before VIMT");


                CrmeSettings crmeSettings = SettingsHelper.GetSettingValues(OrganizationService, PluginExecutionContext);

               

                //Initiate Data Retrieval
                var dataProvider = new DependentMaintenanceDataProvider(dependentMaintenance.crme_StoredSSN,
                    dependentMaintenance.crme_ParticipantID,
                    this,
                    crmeSettings);

                
                
                dataProvider.Veteran.MapVeteranInfo(dependentMaintenance, OrganizationServiceContext);

                Logger.WriteGranularTimingMessage("After VIMT");

                Logger.WriteTxnTimingMessage(String.Format("Ending :{0}", GetType()));
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Logger.WriteToFile(ex.Message);
                SetErrorFieldValue(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("custom"))
                {
                    Logger.WriteDebugMessage(ex.Message.Substring(6));
                    SetErrorFieldValue(ex.Message);
                    return;
                }

                Logger.setMethod = "Execute";
                Logger.WriteToFile(ex.Message);
                SetErrorFieldValue(ex.Message);
            }
        }

        
        private void SetErrorFieldValue(string value)
        {
            GetPrimaryEntity()["crme_txnstatus"] = new OptionSetValue(935950002);
            GetPrimaryEntity()["crme_hiddenerrormessage"] = value;
        }
        #endregion

        #region  _veteranRetrievePostStageRunner Methods
        public override string McsSettingsDebugField
        {
            get { return "crme_dependentmaintenance"; }
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
