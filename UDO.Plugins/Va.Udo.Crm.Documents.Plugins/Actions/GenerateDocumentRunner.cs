/// <summary>
/// Va.Udo.Crm.Documents.Plugins - GenerateDocumentRunner
/// </summary>
namespace Va.Udo.Crm.Documents.Plugins
{
    using MCSPlugins;
    using Microsoft.Xrm.Sdk;
    using System;
    using VRMRest;

    internal class GenerateDocumentRunner : UDOActionRunner
    {
        private const string FAILURE_MESSAGE = "{0} Error on Generate Document. CorrelationId: {1}";

        public GenerateDocumentRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_contactlogtimer";
            _logSoapField = "udo_contactlogsoap";
            _debugField = "crme_dependentmaintenance";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_contacttimeout";
            _validEntities = new string[] { "crme_dependentmaintenance", "crme_dependent" };
        }
        public override void DoAction()
        {
            try
            {
                _method = "DoAction";
                GetSettingValues();

                #region Setup Log Settings

                LogSettings logSettings = new LogSettings()
                {
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL",
                    UserId = PluginExecutionContext.InitiatingUserId,
                    callingMethod = Logger.setModule
                };

                #endregion

                #region Read Input Parameters and Get Target
                if (!PluginExecutionContext.InputParameters.ContainsKey("Target"))
                {
                    tracer.Trace("Target not found");
                    Trace("Target not found");
                    throw new InvalidPluginExecutionException("Target not found");
                }

                EntityReference parentReference = PluginExecutionContext.InputParameters["ParentEntityReference"] as EntityReference;
                EntityReference attachmentEntityReference = PluginExecutionContext.InputParameters["AttachmentEntityReference"] as EntityReference;
                string documentTemplate = PluginExecutionContext.InputParameters["DocumentTemplate"].ToString();
                bool uploadAttachment = PluginExecutionContext.InputParameters["UploadAttachment"] != null ? bool.Parse(PluginExecutionContext.InputParameters["UploadAttachment"].ToString()) : true;
                bool convertToPDF = PluginExecutionContext.InputParameters["ConvertToPdf"] != null ? bool.Parse(PluginExecutionContext.InputParameters["ConvertToPdf"].ToString()) : true;
                string filenameSuffix = PluginExecutionContext.InputParameters["FilenameSuffix"].ToString();

                #endregion
                tracer.Trace($"Parent :: Id: {Parent.Id}, Entity: {Parent.LogicalName}");
                var request = new VEISDocGenRequest();
                if ((parentReference != null && parentReference.Id != null) && (attachmentEntityReference != null && attachmentEntityReference.Id != null))
                {
                    //Build the request
                    request = new VEISDocGenRequest()
                    {
                        MessageId = PluginExecutionContext.CorrelationId.ToString(),
                        LogSoap = _logSoap,
                        Debug = _debug,
                        LogTiming = _logTimer,
                        OrganizationName = PluginExecutionContext.OrganizationName,
                        UserId = PluginExecutionContext.InitiatingUserId,
                        ConvertToPdf = convertToPDF,
                        UploadAttachment = uploadAttachment,
                        DocumentTemplate = documentTemplate,
                        PrimaryEntityId = PluginExecutionContext.PrimaryEntityId,
                        PrimaryEntityName = PluginExecutionContext.PrimaryEntityName,
                        RelatedParentId = parentReference.Id,
                        RelatedParentEntityName = parentReference.LogicalName,
                        AttachmentEntityId = attachmentEntityReference.Id,
                        AttachmentEntityName = attachmentEntityReference.LogicalName
                    };
                } 
                else
                {
                    var msg = "Parent Entity Reference and/or Attachment Entity Reference are null";
                    _responseMessage = String.Format(FAILURE_MESSAGE, msg, PluginExecutionContext.CorrelationId.ToString());
                    ExceptionOccurred = true;
                    tracer.Trace($" ## ExceptionMessage: {msg}");
                    Trace($" ## ExceptionMessage: {msg}");
                    Logger.WriteException(new Exception($" ## ExceptionMessage: {msg}"));
                    return;
                }

                // Invoke the DocGen Api
                var response = Utility.SendReceive<VEISDocGenResponse>(_uri, "VEISDocGenRequest",
                                    request, logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);

                if (response.ExceptionOccured)
                {
                    _responseMessage = String.Format(FAILURE_MESSAGE, response.ExceptionMessage, PluginExecutionContext.CorrelationId.ToString());
                    ExceptionOccurred = true;
                    tracer.Trace($" ## ExceptionMessage: {response.ExceptionMessage}");
                    Trace($" ## ExceptionMessage: {response.ExceptionMessage}");
                    Logger.WriteException(new Exception($" ## ExceptionMessage: {response.ExceptionMessage}"));
                    return;
                }

                string fileExtension = string.Empty;
                string timestampFormat = "yyyy-dd-MM-HH-mm-ss";
                string filenameFormat = documentTemplate.Replace(" ", "") + "_{0}_{1}{2}";

                if (convertToPDF == true)
                {
                    fileExtension = ".pdf";
                    SetOutputParameter("MimeType", "application/pdf");
                }
                else
                {
                    fileExtension = ".doc";
                    SetOutputParameter("MimeType", "application/word");
                }

                string fileName = string.Format(filenameFormat, filenameSuffix.Replace(" ", "_"), DateTime.Now.ToString(timestampFormat), fileExtension);
                tracer.Trace(String.Format("DocGen filename: {0}", fileName));
                Trace(String.Format("DocGen filename: {0}", fileName));

                SetOutputParameter("FileName", fileName);
                SetOutputParameter("Base64FileContents", response.WordBytesasBase64);
                PluginExecutionContext.OutputParameters["AttachmentId"] = new EntityReference("annotation", response.AttachmentId);

            }

            catch (Exception ex)
            {
                PluginError = true;
                Trace(string.Format("{0}", ex.Message));
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
    }
}
