using System.Linq;
using Microsoft.Xrm.Sdk;
using System;
using VRM.IntegrationServicebus.AddDependent.CrmModel;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    //Purpose:  Populate Veteran Information on the Dependent Maintenance Form.
    public class crmeDependentMaintenanceUpdatePreStageRunner : MCSPlugins.PluginRunner
    {
        private const long _Draft = 935950000;
        private const long _Cancelled = 935950004;

        private const long _Failure = 935950002;
        private const long _Success = 935950003;
        #region Constructor
        public crmeDependentMaintenanceUpdatePreStageRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
        #endregion

        #region Inernal Methods/Properties
        internal void Execute(IServiceProvider serviceProvider)
        {
			TracingService.Trace($"| >> Start { this.GetType().FullName}.Execute");

			//Avoid Recursive Calls - Abort if depth is greater than 1
			if (PluginExecutionContext.Depth > 1)
                return;

            if (!GetSecondaryEntity().Attributes.Contains("crme_txnstatus"))
            {
                TracingService.Trace("Status equal to null - must be save");
                return;
            }
            //If Forcing an update, reset the crme_forceUpdate field to false and write record.
            //if(CheckForForceUpdate()) 
            //    return;
           
            //Cancel done in Workflow
            if (ChecForCancel())
                return;
           
            //Fail/success done in VIMT
            if (ChecForFailureorSuccess())
                return;

            CheckTransactionStatus();

            ValidateVeteran();

            ValidateHasDependents();

            ValidateMaritalInfo();

			//End the timing for the plugin
			//Logger.WriteTxnTimingMessage(String.Format("Ending :{0}", GetType()));

			TracingService.Trace($"| >> End { this.GetType().FullName}.Execute");
		}
        private bool ChecForFailureorSuccess()
        {
            var dependendentMaintenance = GetSecondaryEntity().ToEntity<crme_dependentmaintenance>();

            if (dependendentMaintenance.crme_txnStatus != null)
            {
                if (dependendentMaintenance.crme_txnStatus.Value == _Failure ||
                    dependendentMaintenance.crme_txnStatus.Value == _Success)
                    return true;
            }
            return false;
        }
        private bool ChecForCancel()
        {
            var dependendentMaintenance = GetSecondaryEntity().ToEntity<crme_dependentmaintenance>();

            if(dependendentMaintenance.crme_txnStatus == null)
                return false;

            return dependendentMaintenance.crme_txnStatus.Value == _Cancelled;
        }

        private bool CheckForForceUpdate()
        {
            bool retVal = false;

      
            //Entity thisEntity = GetSecondaryEntity();
            //if (thisEntity.Attributes.Contains("crme_forceupdate"))
            //{
            //    return true;
            //}

            //return false;
            var dependendentMaintenance = GetSecondaryEntity().ToEntity<crme_dependentmaintenance>();

            if (!dependendentMaintenance.crme_ForceUpdate.HasValue)
                return false;

            if (dependendentMaintenance.crme_ForceUpdate.Value)
            {
                retVal = true;
                //dependendentMaintenance.crme_ForceUpdate = false;
            }
            return retVal;
        }

        private void ValidateHasDependents()
        {
            var id = GetPrimaryEntity().Id;
            var dependendentMaintenance = GetSecondaryEntity().ToEntity<crme_dependentmaintenance>();
            var hasDependents = (from d in OrganizationServiceContext.CreateQuery<crme_dependent>()
                where (d.crme_DependentMaintenance.Id == id) &&
                      (d.crme_LegacyRecord == false)
                select d).ToList().Count > 0;

            if(!hasDependents)
                throw new InvalidPluginExecutionException(
                    "Add at least one dependent before submitting.");
        }

        private void ValidateMaritalInfo()
        {
            var id = GetPrimaryEntity().Id;

            var openMarriages = (from m in OrganizationServiceContext.CreateQuery<crme_maritalhistory>()
                                 join d in OrganizationServiceContext.CreateQuery<crme_dependent>() on m.crme_Dependent.Id equals d.Id
                                 where (m.crme_DependentMaintenance.Id == id) &&
                                 (m.crme_MarriageEndDate == null)
                                 select m.Id).ToList();
            if (openMarriages.Count > 1)
            {
                throw new InvalidPluginExecutionException("Veteran cannot have more than one Marital History record without an End Date");
            }

        }
        
        private void ValidateVeteran()
        {
            var veteran = GetPrimaryEntity().ToEntity<crme_dependentmaintenance>();

            if(string.IsNullOrEmpty(veteran.crme_Address1))
                throw new InvalidPluginExecutionException(
                    "The Veteran Adddress1 field is required.");

            if (string.IsNullOrEmpty(veteran.crme_City))
                throw new InvalidPluginExecutionException(
                    "The Veteran City field is required.");

            if (veteran.crme_StateProvinceId == null)
                throw new InvalidPluginExecutionException(
                    "The Veteran State field is required.");

            if (veteran.crme_ZIPPostalCodeId == null)
                throw new InvalidPluginExecutionException(
                    "The Veteran Zip Code field is required.");

            if (veteran.crme_CountryId == null)
                throw new InvalidPluginExecutionException(
                    "The Veteran Country field is required.");
        }

        private int GetTransactionStatus()
        {
            var optionSetValue = GetPrimaryEntity().GetAttributeValue<OptionSetValue>("crme_txnstatus");

            return optionSetValue == null ? 0 : optionSetValue.Value;
        }

        private void CheckTransactionStatus()
        {
            
            if (GetTransactionStatus() != _Draft)
                throw new InvalidPluginExecutionException(
                    "This record cannot be updated because it has already been submitted.");
        }
        #endregion

        #region  _veteranRetrievePostStageRunner Methods
        public override string McsSettingsDebugField
        {
            get { return "crme_dependentmaintenance"; }
        }

        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.PreEntityImages["pre"];
        }

        public override Entity GetSecondaryEntity()
        {
            return (Entity)PluginExecutionContext.InputParameters["Target"];
        }
        #endregion
    }
}