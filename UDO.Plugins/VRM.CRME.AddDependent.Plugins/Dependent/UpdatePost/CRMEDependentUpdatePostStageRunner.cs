using System.Linq;
using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;
using VRM.IntegrationServicebus.AddDependent.CrmModel;


namespace VRM.CRME.Plugin.DependentMaintenance
{
    //Purpose:  Populate Veteran Information on the Dependent Maintenance Form.
    public class CRMEDependentUpdatePostStageRunner : MCSPlugins.PluginRunner
    {
        #region Constructor
        public CRMEDependentUpdatePostStageRunner(IServiceProvider serviceProvider)
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

                //Get Primary Entity - this has the changes
                var primaryEntity = GetPrimaryEntity().ToEntity<crme_dependent>();

                //Only process crme_dependent types
                if (primaryEntity.LogicalName != "crme_dependent")
                    return;

                //Create Earlybound crme_dependent entity object
                var dependent = GetSecondaryEntity().ToEntity<crme_dependent>();

                //if it isn't a spouse, get out
                if (dependent.crme_DependentRelationship.Value != 935950001) return;

                //Update Marital History Properties
               // UpdateMaritalHistory(primaryEntity);
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

        private void UpdateMaritalHistory(crme_dependent dependent)
        {
            try
            {
                QueryByAttribute querybyattribute = new QueryByAttribute("crme_maritalhistory");
                querybyattribute.ColumnSet = new ColumnSet("crme_dependent");
                //  Attribute to query.
                querybyattribute.Attributes.AddRange("crme_dependent");

                //  Value of queried attribute to return.
                querybyattribute.Values.AddRange(dependent.Id);

                //  Query passed to service proxy.
                EntityCollection retrieved = OrganizationService.RetrieveMultiple(querybyattribute);

                if (retrieved.Entities.Count > 0)
                {
                    var maritalId = retrieved.Entities[0].Id;
                    var maritalHistory = new Entity();
                    maritalHistory.LogicalName = "crme_maritalhistory";
                    maritalHistory.Id = maritalId;
                    if (dependent.crme_SSN != null)
                    {
                        maritalHistory["crme_spousessn"] = dependent.crme_SSN;
                    }

                    if (dependent.crme_MarriageDate != null)
                    {
                        maritalHistory["crme_marriagestartdate"] = dependent.crme_MarriageDate;
                    }
                    if (dependent.crme_LastName != null)
                    {
                        maritalHistory["crme_lastname"] = dependent.crme_LastName;
                    }
                    if (dependent.crme_MiddleName != null)
                    {
                        maritalHistory["crme_middlename"] = dependent.crme_MiddleName;
                    }
                    if (dependent.crme_FirstName != null)
                    {
                        maritalHistory["crme_firstname"] = dependent.crme_FirstName;
                    }
                    if (dependent.crme_DOB != null)
                    {
                        maritalHistory["crme_dob"] = dependent.crme_DOB;
                    }
                    if (dependent.crme_MarriageCountryId != null)
                    {
                        maritalHistory["crme_countryid"] = dependent.crme_MarriageCountryId;
                    }
                    if (dependent.crme_MarriageCity != null)
                    {
                        maritalHistory["crme_city"] = dependent.crme_MarriageCity;
                    }
                    if (dependent.crme_MarriageStateId != null)
                    {
                        maritalHistory["crme_stateprovinceid"] = dependent.crme_MarriageStateId;
                    }
                    if (dependent.crme_marriagelocation != null)
                    {
                        maritalHistory["crme_startlocation"] = dependent.crme_marriagelocation;
                    }

                    OrganizationService.Update(maritalHistory);



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
            return (Entity)PluginExecutionContext.PreEntityImages["Pre"];
        }
        #endregion
    }
}
