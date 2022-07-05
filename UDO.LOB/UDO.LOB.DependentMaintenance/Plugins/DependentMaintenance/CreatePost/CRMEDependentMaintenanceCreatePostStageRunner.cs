using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using System.Text;
using VRM.IntegrationServicebus.AddDependent.CrmModel;

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
				TracingService.Trace($"| >> Start { this.GetType().FullName}.Execute");

				//Avoid Recursive Calls - Abort if depth is greater than 1
				if (PluginExecutionContext.Depth > 1)
                    return;

                //Create Earlybound crme_dependentmaintenance entity object
                var dependentMaintenance = GetPrimaryEntity().ToEntity<crme_dependentmaintenance>();

                CrmeSettings crmeSettings = SettingsHelper.GetSettingValues(OrganizationService, PluginExecutionContext);

                //Initiate Data Retrieval
                var dataProvider = new DependentMaintenanceDataProvider(dependentMaintenance.crme_StoredSSN, dependentMaintenance.crme_ParticipantID, this, crmeSettings);

                try
                {
                    //Map Dependents
                    dataProvider.Dependents.MapDependentInfo(OrganizationService, dependentMaintenance, OrganizationServiceContext); 
					TracingService.Trace($"| >> In { this.GetType().FullName}.Execute | After dataProvider.Dependents.MapDependentInfo()");

					//Map Marital History
					dataProvider.MaritalHistory.MapMaritalHistoryInfo(OrganizationService, dependentMaintenance, OrganizationServiceContext);
					TracingService.Trace($"| >> In { this.GetType().FullName}.Execute | After dataProvider.MaritalHistory.MapMaritalHistoryInfo()");

				}
                catch (Exception ex)
                {
                    TracingService.Trace(ex.Message);
                    throw ex;
                }
                finally
                {
					//CSDev 
					TracingService.Trace($"| >> In { this.GetType().FullName}.Execute | Finally");
					if (PluginExecutionContext.SharedVariables.ContainsKey("SoapLog"))
					{
						TracingService.Trace($"| >> In { this.GetType().FullName}.Execute | Before Fetch Soap Log 33");
						//CSDev String Builder gives searilization errors in sandboxed plugins
						//StringBuilder soapLog = (StringBuilder)PluginExecutionContext.SharedVariables["SoapLog"];
						String soapLog = (String)PluginExecutionContext.SharedVariables["SoapLog"];
						AttachSoapLog(dependentMaintenance, soapLog.ToString());
						TracingService.Trace($"| >> In { this.GetType().FullName}.Execute | Fetch Soap Log Created 33");
					}
				}

				TracingService.Trace($"| >> End { this.GetType().FullName}.Execute");

				//Logger.WriteTxnTimingMessage(String.Format("Ending :{0}", GetType()));
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                TracingService.Trace(ex.Message);

                //throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);

                SetErrorFieldValue(ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("custom"))
                {
                    TracingService.Trace(ex.Message.Substring(6));

                    //throw new InvalidPluginExecutionException(ex.Message.Substring(6));

                    SetErrorFieldValue(ex.Message);

                    return;
                }

                Logger.setMethod = "Execute";

                TracingService.Trace(ex.Message);

                //throw new InvalidPluginExecutionException(ex.Message);

                SetErrorFieldValue(ex.Message);
            }
        }

        private void AttachSoapLog(crme_dependentmaintenance dependentMaintenance, string soapLog)
        {
			TracingService.Trace($"| >> Start { this.GetType().FullName}.AttachSoapLog()");
			var bytes = Encoding.UTF8.GetBytes(soapLog);
			var annotation = new Annotation
			{
				IsDocument = true,
				Subject = "Fetch Soap Log",
				ObjectId = new EntityReference(crme_dependentmaintenance.EntityLogicalName,
							   dependentMaintenance.Id),
				ObjectTypeCode = crme_dependentmaintenance.EntityLogicalName.ToLower(),
				OwnerId = new EntityReference(SystemUser.EntityLogicalName,
							   PluginExecutionContext.InitiatingUserId),
				MimeType = @"text/xml",
				NoteText = "Fetch Soap Log",
				FileName = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), ".xml"),
				DocumentBody = Convert.ToBase64String(bytes)

			};

			//CSDev we have to use the OrgService.Create as the orgServiceContext.Add | OrgServiceContext.Update | OrgServiceContext.SaveChagnes() Pattern Doesn't work
			//Error: "The context is not currently tracking the 'annotation' entity."
			OrganizationService.Create(annotation);
			//OrganizationServiceContext.AddObject(annotation);
			//OrganizationServiceContext.UpdateObject(annotation);
			//OrganizationServiceContext.SaveChanges();
			TracingService.Trace($"| >> End { this.GetType().FullName}.AttachSoapLog()");			
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
