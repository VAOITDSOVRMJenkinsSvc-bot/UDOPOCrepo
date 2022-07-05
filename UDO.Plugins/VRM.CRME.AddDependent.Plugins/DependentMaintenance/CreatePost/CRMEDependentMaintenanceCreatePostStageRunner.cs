using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using System.Text;
using VRM.IntegrationServicebus.AddDependent.CrmModel;
using Microsoft.Xrm.Sdk.Client;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    //Purpose:  Populate Veteran Information on the Dependent Maintenance Form.
    public class CrmeDependentMaintenanceCreatePostStageRunner : MCSPlugins.PluginRunner
    {
        #region Constructor
        public CrmeDependentMaintenanceCreatePostStageRunner(IServiceProvider serviceProvider)
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

                Logger.WriteGranularTimingMessage("Before VIMT");

                CrmeSettings crmeSettings = SettingsHelper.GetSettingValues(OrganizationService, PluginExecutionContext);

                //Initiate Data Retrieval
                var dataProvider = new DependentMaintenanceDataProvider(dependentMaintenance.crme_StoredSSN, 
                    dependentMaintenance.crme_ParticipantID,
                    this,
                    crmeSettings);

                try
                {
                    //Map Dependents
                    TracingService.Trace("Mapping Dependents Now..");
                    dataProvider.Dependents.MapDependentInfo(OrganizationService,
                        dependentMaintenance,
                        OrganizationServiceContext);

                    //Map Marital History
                    TracingService.Trace("Mapping Marital Info Now..");
                    //dataProvider.MaritalHistory.MapMaritalHistoryInfo(OrganizationService,
                    //    dependentMaintenance,
                    //    OrganizationServiceContext);
                    TracingService.Trace("Done.");
                }
                catch (Exception ex)
                {
                    Logger.WriteToFile(ex.Message);
                    throw ex;
                }
                finally
                {
                    TracingService.Trace("Entered Finally");
                    TracingService.Trace("Value is : " + (string)(PluginExecutionContext.SharedVariables["SoapLog"]));
                    if (PluginExecutionContext.SharedVariables.ContainsKey("SoapLog"))
                    {
                        //TODO: Need to figure out how to handle this.
                        TracingService.Trace("about to attach soap log");
                        string soapLog = (string)PluginExecutionContext.SharedVariables["SoapLog"];
                        
                        AttachSoapLog(dependentMaintenance, soapLog);
                        TracingService.Trace("done:  attach soap log");
                    }
                }

                Logger.WriteGranularTimingMessage("After VIMT");

                Logger.WriteTxnTimingMessage(String.Format("Ending :{0}", GetType()));
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Logger.WriteToFile(ex.Message);

                //throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);

                SetErrorFieldValue(ex.Message);

                TracingService.Trace("Error Message Details: " + ex.Message);
                TracingService.Trace("Stack Trace: " + ex.StackTrace);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("custom"))
                {
                    Logger.WriteDebugMessage(ex.Message.Substring(6));

                    //throw new InvalidPluginExecutionException(ex.Message.Substring(6));

                    SetErrorFieldValue(ex.Message);

                    return;
                }

                Logger.setMethod = "Execute";

                Logger.WriteToFile(ex.Message);

                //throw new InvalidPluginExecutionException(ex.Message);

                SetErrorFieldValue(ex.Message);

                TracingService.Trace("Error Message Details: " + ex.Message);
                TracingService.Trace("Stack Trace: " + ex.StackTrace);
            }
        }

        private void AttachSoapLog(crme_dependentmaintenance dependentMaintenance, string soapLog)
        {
            var annotation = new Annotation
            {
                IsDocument = true,
                Subject = "Fetch Soap Log",
                ObjectId = new EntityReference(crme_dependentmaintenance.EntityLogicalName,
                    dependentMaintenance.Id),
                ObjectTypeCode = crme_dependentmaintenance.EntityLogicalName.ToLower(),
                OwnerId = new EntityReference(SystemUser.EntityLogicalName,
                    PluginExecutionContext.InitiatingUserId)
            };
            TracingService.Trace("calling add object");
           // OrganizationServiceContext.AddObject(annotation);

            annotation.MimeType = @"text/xml";
            annotation.NoteText = "Fetch Soap Log";
            annotation.FileName = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), ".xml");

            TracingService.Trace("Bytes");
            var bytes = Encoding.UTF8.GetBytes(soapLog);
            TracingService.Trace("document body");
            annotation.DocumentBody = Convert.ToBase64String(bytes);

            TracingService.Trace("update object");
            //  OrganizationServiceContext.UpdateObject(annotation);
            //OrganizationServiceContext.AddObject(annotation);
            TracingService.Trace("save changes");
            //OrganizationServiceContext.SaveChanges();
            OrganizationService.Create(annotation);
            TracingService.Trace("save changes end");
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
