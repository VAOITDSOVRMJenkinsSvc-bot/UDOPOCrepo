#region Using Directives

using System;
using System.ServiceModel;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using VRM.Integration.UDO.SSRS.Messages;
using VRMRest;

#endregion

namespace Va.Udo.Crm.SSRS.Plugins
{
    internal class GenerateDocumentPostRunner : PluginRunner
    {
        private const string _vimtRestEndpointField = "crme_restendpointforvimt";
        internal bool _logSoap;
        internal bool _logTimer;
        internal bool _debug;
        internal string _uri;
        private CRMAuthTokenConfiguration _crmAuthTokenConfig;

        public GenerateDocumentPostRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logSoap = false;
            _logTimer = false;
            _debug = false;
        }

        public override string McsSettingsDebugField
        {
            get { return "udo_ssrsdebug"; }
        }

        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.PreEntityImages["Pre"];
        }

        public override Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.PreEntityImages["Pre"];
        }

        internal void Execute()
        {
            try
            {
                Trace("\r\nGetting Settings\r\n");
                getSettingValues();
                Trace("Keys Passed: " + String.Join(",", PluginExecutionContext.InputParameters.Keys) + "\r\n");


                #region Get VIMT URI

                var uri = new Uri(_uri);

                if (string.IsNullOrEmpty(_uri))
                {
                    //Logger.WriteToFile("NO URI FOUND, cannot call VIMT");
                    Trace("NO URI FOUND, cannot call VIMT");
                    return;
                }

                #endregion

                // Get target
                if (!PluginExecutionContext.InputParameters.ContainsKey("Target"))
                {
                    Trace("Target not found");
                    
                    throw new Exception("Target not found");
                }
                var target = PluginExecutionContext.InputParameters["Target"] as EntityReference;
                
                if (target == null) return;

                TracingService.Trace("Target: {0} {1}", target.LogicalName, target.Id);

                // Build Request
                var request = new UDORunCRMReportRequest
                {
                    LogSoap = _logSoap,
                    Debug = _debug,
                    LogTiming = _logTimer,
                    OrganizationName = PluginExecutionContext.OrganizationName,
                    OrganizationId = PluginExecutionContext.OrganizationId,
                    UserId = PluginExecutionContext.InitiatingUserId,
                   // MessageId = Guid.NewGuid().ToString(),
                   MessageId = PluginExecutionContext.CorrelationId.ToString()
                };

                // Get Type
                if (target.LogicalName.Equals("udo_lettergeneration", StringComparison.OrdinalIgnoreCase))
                {
                    request.udo_LetterGenerationId = target.Id;
                }
                else if (target.LogicalName.Equals("udo_servicerequest", StringComparison.OrdinalIgnoreCase))
                {
                    request.udo_ServiceRequestId = target.Id;
                }
                else if (target.LogicalName.Equals("va_fnod", StringComparison.OrdinalIgnoreCase))
                {
                    request.udo_FnodId = target.Id;
                }
                else
                {
                    return; //unknown type
                }

                //request.ownerId = PluginExecutionContext.InitiatingUserId;
                //request.ownerType = "systemuser";

                if (PluginExecutionContext.InputParameters.ContainsKey("Person"))
                {
                    var inputPerson = (EntityReference)PluginExecutionContext.InputParameters["Person"];
                    request.udo_PersonId = (inputPerson==null) ? Guid.Empty : inputPerson.Id;
                    if (!request.udo_PersonId.Equals(Guid.Empty))
                    {
                        var person = OrganizationService.Retrieve(inputPerson.LogicalName, inputPerson.Id, new ColumnSet("ownerid"));
                        if (person != null && person.Contains("ownerid"))
                        {
                            var owner = (EntityReference)person["ownerid"];
                            request.ownerType = owner.LogicalName;
                            request.ownerId = owner.Id;
                        }
                        TracingService.Trace("Person: {0}", inputPerson.Id);
                    }
                }


                if (PluginExecutionContext.InputParameters.ContainsKey("Report"))
                {
                    var inputReport = (EntityReference)PluginExecutionContext.InputParameters["Report"];
                    request.udo_ReportId = (inputReport==null) ? Guid.Empty : inputReport.Id;
                    TracingService.Trace("Report: {0}", request.udo_ReportId);
                }

                var inputSourceUrl = GetInputParameter("SourceUrl");
                TracingService.Trace("SourceURL: {0}", inputSourceUrl);
                var inputReportName = GetInputParameter("ReportName", true);
                TracingService.Trace("Report Name: {0}", inputReportName);
                
                var inputFormatType = GetInputParameter("FormatType", true);
                if (string.IsNullOrEmpty(inputFormatType)) inputFormatType = "PDF";
                TracingService.Trace("Format Type: {0}", inputFormatType);

                var inputClaimNumber = GetInputParameter("ClaimNumber", true);
                
                #region Get Report Id

                if (string.IsNullOrEmpty(inputReportName) && request.udo_ReportId==Guid.Empty)
                {
                    throw new InvalidPluginExecutionException("customSSRSReportName or ReportId must be provided");
                }

                request.udo_SSRSReportName = inputReportName;

                #endregion

                request.udo_FormatType = inputFormatType;
                request.udo_ClaimNumber = inputClaimNumber;
                // request.MessageId = Guid.NewGuid().ToString();
                request.MessageId = PluginExecutionContext.CorrelationId.ToString();
                request.udo_UploadToVBMS = (bool) PluginExecutionContext.InputParameters["UploadToVBMS"];
                request.udo_SourceUrl = inputSourceUrl;

                if (request.udo_UploadToVBMS)
                {
                    request.udo_vbmsdocumentid = Guid.NewGuid();
                    SetOutputParameter("VBMSDocument", new EntityReference("udo_vbmsdocument", request.udo_vbmsdocumentid));
                }

                var logSettings = new LogSettings
                {
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL",
                    UserId = PluginExecutionContext.InitiatingUserId,
                    callingMethod = "GenerateDocumentPostRunner"
                };


                Trace("Running Report");
                Trace("URL:" + uri.ToString());
                Logger.WriteDebugMessage($"\r\n RequestBody: { JsonHelper.Serialize(request, request.GetType())}");
                var response = Utility.SendReceive<UDORunCRMReportResponse>(uri, "UDORunCRMReportRequest", request,
                    logSettings, 0, _crmAuthTokenConfig, TracingService);
                Trace(JsonHelper.Serialize(response));

                if (response == null)
                {
                    response = new UDORunCRMReportResponse();
                    response.udo_Uploaded = false;
                    response.udo_UploadMessage = "Unable to upload report";
                }

                if (response.ExceptionOccured)
                {
                    throw new InvalidPluginExecutionException(response.ExceptionMessage);
                }
               
                var uploaded = response.udo_Uploaded;
                SetOutputParameter("Uploaded", uploaded);

                if (!String.IsNullOrEmpty(response.udo_UploadMessage))
                {
                    SetOutputParameter("UploadStatusMessage", response.udo_UploadMessage);
                }
                if (!uploaded)
                {
                    SetOutputParameter("MimeType", response.udo_MimeType);
                    SetOutputParameter("FileName", response.udo_FileName);
                    SetOutputParameter("Base64FileContents", response.udo_Base64FileContents);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                PluginError = true;
                if (!ex.Message.StartsWith("custom"))
                {
                    //Logger.WriteToFile(ex.Message);
                    Trace(ex.Message);
                    throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
                }
                throw new InvalidPluginExecutionException(ex.Message.Substring(6));
            }
            catch (Exception ex)
            {
                PluginError = true;
                if (!ex.Message.StartsWith("custom"))
                {
                    //Logger.WriteToFile(ex.Message);
                    Trace(ex.Message);
                    throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
                }
            }
            finally
            {
                Trace("Entered Finally");
                SetupLogger();
                Trace("Set up logger done.");
                ExecuteFinally();
                Trace("Exit Finally");
            }
        }

        private void SetOutputParameter(string key, object value)
        {
            if (PluginExecutionContext.OutputParameters.ContainsKey(key))
            {
                PluginExecutionContext.OutputParameters[key] = value;
                TracingService.Trace("Set Output Parameter: {0} = {1}", key, value);
                return;
            }
            PluginExecutionContext.OutputParameters.Add(key, value);
            TracingService.Trace("Set Output Parameter: {0} = {1}", key, value);
        }

        private string GetInputParameter(string inputParameterName, bool allowEmpty = false, string error = null)
        {
            if (error == null) error = string.Format("Parameter {0} is required.", inputParameterName);

            if (PluginExecutionContext.InputParameters.ContainsKey(inputParameterName))
            {
                var data = PluginExecutionContext.InputParameters[inputParameterName].ToString();
                if (!allowEmpty && string.IsNullOrEmpty(data))
                {
                    error += "  The data passed was empty.";
                    throw new Exception("custom" + error);
                }
                return data;
            }
            throw new Exception("custom" + error);
        }

        internal void getSettingValues()
        {
            _logTimer = McsSettings.GetSingleSetting<bool>("udo_appeallogtimer");
            _logSoap = McsSettings.GetSingleSetting<bool>("udo_appeallogsoap");
            _debug = McsSettings.GetSingleSetting<bool>("udo_ssrsdebug");
            
            _uri = McsSettings.GetSingleSetting<string>(_vimtRestEndpointField); ;

            string parentAppId = McsSettings.GetSingleSetting<string>("udo_oauthresourceid");
            string clientAppId = McsSettings.GetSingleSetting<string>("udo_oauthclientid");
            string clientSecret = McsSettings.GetSingleSetting<string>("udo_oauthclientsecret");
            string tenentId = McsSettings.GetSingleSetting<string>("udo_aadtenent");
            string apimsubscriptionkey = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkey");
            string apimsubscriptionkeyS = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeysouth");
            string apimsubscriptionkeyE = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeyeast");
            //Create the token from settings
            _crmAuthTokenConfig = new CRMAuthTokenConfiguration
            {
                ParentApplicationId = parentAppId,
                ClientApplicationId = clientAppId,
                ClientSecret = clientSecret,
                TenantId = tenentId,
                ApimSubscriptionKey = apimsubscriptionkey,
                ApimSubscriptionKeyS = apimsubscriptionkeyS,
                ApimSubscriptionKeyE = apimsubscriptionkeyE
            };
        }
    }
}