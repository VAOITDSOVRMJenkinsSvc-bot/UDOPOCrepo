using System;
using System.Diagnostics;
using VEIS.Core.Messages;
using VEIS.Core.Processor;
using VEIS.Core.Wcf;
using VEIS.Messages.ReportServerReportExecution;
using VEIS.ReportServerReportExecution.Services;
using VEIS.ReportServerReportExecution.Services.ReportServerReportExecution;

namespace VEIS.ReportServerReportExecution.Api.Processors
{
    public class VEISLRLoadReportGetDataProcessor : EcProcessorBase
    {        /// <summary> 
             /// </summary>
             /// <param name=none></param>
             /// <returns>none</returns>
        public override VEISEcResponseBase Process(VEISEcRequestBase req)
        {
            VEISLRLoadReportResponse response = null;
            VEISLRLoadReportRequest request = null;
            Stopwatch serviceTimer = new Stopwatch();
            try
            {
                Trace("Top of Processor");
                request = (VEISLRLoadReportRequest)req;

                #region do legacy webservice calls
                string method = "LoadReportProcessor", webService = "LoadReport";

                var LoadReportResponse = new LoadReportResponse();
                Stopwatch thisTimer = Stopwatch.StartNew();

                try
                {
                    SoapLog.Current.Active = true;
                    LoadReportRequest loadReportRequest = new LoadReportRequest()
                    {
                        TrustedUserHeader = new TrustedUserHeader()
                    };
                    if (!string.IsNullOrEmpty(request.trusteduserheaderInfo.mcs_UserName)) loadReportRequest.TrustedUserHeader.UserName = request.trusteduserheaderInfo.mcs_UserName;
                    loadReportRequest.TrustedUserHeader.UserToken = request.trusteduserheaderInfo.mcs_UserToken;

                    var service = ServiceFactory.GetReportServerReportExecutionServiceReference(request.LegacyServiceHeaderInfo);

                    Trace("Before Service Call");
                    serviceTimer.Start();
                    LoadReportResponse = service.LoadReport(loadReportRequest);
                    serviceTimer.Stop();
                    Trace("After Service Call");
                }
                catch (Exception ex)
                {
                    serviceTimer.Stop();
                    if (response == null) response = new VEISLRLoadReportResponse();
                    var messageTest = ex.Message.ToLower();

                    if (messageTest.Contains("no records"))
                    {
                        return response;
                    }
                    if (!messageTest.Contains("shareexception") && !messageTest.Contains("access violation") && !messageTest.Contains("sensitive"))
                    {
                        Trace(ex.Message, LogLevel.Error);
                    }
                    return ReturnFailure(response, messageTest, SoapLog.Current);
                }

                Trace("Before Mapping");
                try
                {
                    response = new VEISLRLoadReportResponse();
                    //LoadReportResponse Mapping 
                    #region  LoadReportResponse Mapping 
                    var VEISLRexecutionInfossrsInfo = new VEISLRexecutionInfossrs();
                    var executioninfo = LoadReportResponse.executionInfo;
                    if (executioninfo != null)
                    {
                        VEISLRexecutionInfossrsInfo.mcs_HasSnapshot = executioninfo.HasSnapshot;
                        VEISLRexecutionInfossrsInfo.mcs_NeedsProcessing = executioninfo.NeedsProcessing;
                        VEISLRexecutionInfossrsInfo.mcs_AllowQueryExecution = executioninfo.AllowQueryExecution;
                        VEISLRexecutionInfossrsInfo.mcs_CredentialsRequired = executioninfo.CredentialsRequired;
                        VEISLRexecutionInfossrsInfo.mcs_ParametersRequired = executioninfo.ParametersRequired;
                        VEISLRexecutionInfossrsInfo.mcs_ExpirationDateTime = executioninfo.ExpirationDateTime;
                        VEISLRexecutionInfossrsInfo.mcs_ExecutionDateTime = executioninfo.ExecutionDateTime;
                        VEISLRexecutionInfossrsInfo.mcs_NumPages = executioninfo.NumPages;
                        VEISLRexecutionInfossrsInfo.mcs_HasDocumentMap = executioninfo.HasDocumentMap;
                        VEISLRexecutionInfossrsInfo.mcs_ExecutionID = executioninfo.ExecutionID;
                        VEISLRexecutionInfossrsInfo.mcs_ReportPath = executioninfo.ReportPath;
                        VEISLRexecutionInfossrsInfo.mcs_HistoryID = executioninfo.HistoryID;
                        VEISLRexecutionInfossrsInfo.mcs_AutoRefreshInterval = executioninfo.AutoRefreshInterval;
                        response.VEISLRexecutionInfossrsInfo = VEISLRexecutionInfossrsInfo;
                        Trace("After LoadReportResponse Mapping");

                        //passing to parentDTOName executionInfo

                        #region  - LEGACY LoadReportResponse.executionInfo.Parameters Response Mapping 
                        System.Collections.Generic.List<VEISLRParametersssrsMultipleResponse> ParametersssrsListRecord = new System.Collections.Generic.List<VEISLRParametersssrsMultipleResponse>();
                        var ReportParameter = LoadReportResponse.executionInfo.Parameters;
                        if (ReportParameter != null)
                        {
                            foreach (var Parametersssrsitem in ReportParameter)
                            {
                                var VEISLRParametersssrsRecord = new VEISLRParametersssrsMultipleResponse();
                                VEISLRParametersssrsRecord.mcs_Name = Parametersssrsitem.Name;
                                Trace("Parametersssrsitem.Name");
                                VEISLRParametersssrsRecord.mcs_TypeSpecified = Parametersssrsitem.TypeSpecified;
                                Trace("Parametersssrsitem.TypeSpecified");
                                VEISLRParametersssrsRecord.mcs_Nullable = Parametersssrsitem.Nullable;
                                Trace("Parametersssrsitem.Nullable");
                                VEISLRParametersssrsRecord.mcs_NullableSpecified = Parametersssrsitem.NullableSpecified;
                                Trace("Parametersssrsitem.NullableSpecified");
                                VEISLRParametersssrsRecord.mcs_AllowBlank = Parametersssrsitem.AllowBlank;
                                Trace("Parametersssrsitem.AllowBlank");
                                VEISLRParametersssrsRecord.mcs_AllowBlankSpecified = Parametersssrsitem.AllowBlankSpecified;
                                Trace("Parametersssrsitem.AllowBlankSpecified");
                                VEISLRParametersssrsRecord.mcs_MultiValue = Parametersssrsitem.MultiValue;
                                Trace("Parametersssrsitem.MultiValue");
                                VEISLRParametersssrsRecord.mcs_MultiValueSpecified = Parametersssrsitem.MultiValueSpecified;
                                Trace("Parametersssrsitem.MultiValueSpecified");
                                VEISLRParametersssrsRecord.mcs_QueryParameter = Parametersssrsitem.QueryParameter;
                                Trace("Parametersssrsitem.QueryParameter");
                                VEISLRParametersssrsRecord.mcs_QueryParameterSpecified = Parametersssrsitem.QueryParameterSpecified;
                                Trace("Parametersssrsitem.QueryParameterSpecified");
                                VEISLRParametersssrsRecord.mcs_Prompt = Parametersssrsitem.Prompt;
                                Trace("Parametersssrsitem.Prompt");
                                VEISLRParametersssrsRecord.mcs_PromptUser = Parametersssrsitem.PromptUser;
                                Trace("Parametersssrsitem.PromptUser");
                                VEISLRParametersssrsRecord.mcs_PromptUserSpecified = Parametersssrsitem.PromptUserSpecified;
                                Trace("Parametersssrsitem.PromptUserSpecified");
                                VEISLRParametersssrsRecord.mcs_Dependencies = Parametersssrsitem.Dependencies;
                                Trace("Parametersssrsitem.Dependencies");
                                VEISLRParametersssrsRecord.mcs_ValidValuesQueryBased = Parametersssrsitem.ValidValuesQueryBased;
                                Trace("Parametersssrsitem.ValidValuesQueryBased");
                                VEISLRParametersssrsRecord.mcs_ValidValuesQueryBasedSpecified = Parametersssrsitem.ValidValuesQueryBasedSpecified;
                                Trace("Parametersssrsitem.ValidValuesQueryBasedSpecified");
                                VEISLRParametersssrsRecord.mcs_DefaultValuesQueryBased = Parametersssrsitem.DefaultValuesQueryBased;
                                Trace("Parametersssrsitem.DefaultValuesQueryBased");
                                VEISLRParametersssrsRecord.mcs_DefaultValuesQueryBasedSpecified = Parametersssrsitem.DefaultValuesQueryBasedSpecified;
                                Trace("Parametersssrsitem.DefaultValuesQueryBasedSpecified");
                                VEISLRParametersssrsRecord.mcs_DefaultValues = Parametersssrsitem.DefaultValues;
                                Trace("Parametersssrsitem.DefaultValues");
                                VEISLRParametersssrsRecord.mcs_StateSpecified = Parametersssrsitem.StateSpecified;
                                Trace("Parametersssrsitem.StateSpecified");
                                VEISLRParametersssrsRecord.mcs_ErrorMessage = Parametersssrsitem.ErrorMessage;
                                Trace("Parametersssrsitem.ErrorMessage");
                                Trace("After LoadReportResponse.executionInfo.ParametersResponse Mapping");
                                //newItemName = Parametersssrsitem
                                //newParent_baseDTOName = VEISLRexecutionInfossrsInfo.VEISLRParametersssrsInfo
                                //writing out VEISLRexecutionInfossrsInfo.VEISLRParametersssrsInfo for :18a760dd-6627-e611-9440-0050568df261
                                //passing to ParentDTOName executionInfo.Parameters

                                #region  - LEGACY LoadReportResponse.executionInfo.Parameters.ValidValues Response Mapping 
                                System.Collections.Generic.List<VEISLRValidValuesssrsMultipleResponse> ValidValuesssrsListRecord = new System.Collections.Generic.List<VEISLRValidValuesssrsMultipleResponse>();
                                var ValidValue = Parametersssrsitem.ValidValues;
                                if (ValidValue != null)
                                {
                                    foreach (var ValidValuesssrsitem in ValidValue)
                                    {
                                        var VEISLRValidValuesssrsRecord = new VEISLRValidValuesssrsMultipleResponse();
                                        VEISLRValidValuesssrsRecord.mcs_Label = ValidValuesssrsitem.Label;
                                        Trace("ValidValuesssrsitem.Label");
                                        VEISLRValidValuesssrsRecord.mcs_Value = ValidValuesssrsitem.Value;
                                        Trace("ValidValuesssrsitem.Value");
                                        Trace("After LoadReportResponse.executionInfo.Parameters.ValidValuesResponse Mapping");
                                        //newItemName = ValidValuesssrsitem
                                        //newParent_baseDTOName = VEISLRexecutionInfossrsInfo.VEISLRParametersssrsInfo.VEISLRValidValuesssrsInfo
                                        //writing out VEISLRexecutionInfossrsInfo.VEISLRParametersssrsInfo.VEISLRValidValuesssrsInfo for :2fa760dd-6627-e611-9440-0050568df261
                                        //passing to ParentDTOName executionInfo.Parameters.ValidValues
                                        //No records existed beyond parent 
                                        ValidValuesssrsListRecord.Add(VEISLRValidValuesssrsRecord);
                                    }
                                    //parentRecordName:VEISLRParametersssrsRecord
                                    //insideofMultiple:True
                                    VEISLRParametersssrsRecord.VEISLRValidValuesssrsInfo = ValidValuesssrsListRecord.ToArray();
                                }
                                #endregion
                                ParametersssrsListRecord.Add(VEISLRParametersssrsRecord);
                            }
                            //parentRecordName:
                            //insideofMultiple:False
                            response.VEISLRexecutionInfossrsInfo.VEISLRParametersssrsInfo = ParametersssrsListRecord.ToArray();
                        }
                        #endregion

                        #region  - LEGACY LoadReportResponse.executionInfo.DataSourcePrompts Response Mapping 
                        System.Collections.Generic.List<VEISLRDataSourcePromptsssrsMultipleResponse> DataSourcePromptsssrsListRecord = new System.Collections.Generic.List<VEISLRDataSourcePromptsssrsMultipleResponse>();
                        var DataSourcePrompt = LoadReportResponse.executionInfo.DataSourcePrompts;
                        if (DataSourcePrompt != null)
                        {
                            foreach (var DataSourcePromptsssrsitem in DataSourcePrompt)
                            {
                                var VEISLRDataSourcePromptsssrsRecord = new VEISLRDataSourcePromptsssrsMultipleResponse();
                                VEISLRDataSourcePromptsssrsRecord.mcs_Name = DataSourcePromptsssrsitem.Name;
                                Trace("DataSourcePromptsssrsitem.Name");
                                VEISLRDataSourcePromptsssrsRecord.mcs_DataSourceID = DataSourcePromptsssrsitem.DataSourceID;
                                Trace("DataSourcePromptsssrsitem.DataSourceID");
                                VEISLRDataSourcePromptsssrsRecord.mcs_Prompt = DataSourcePromptsssrsitem.Prompt;
                                Trace("DataSourcePromptsssrsitem.Prompt");
                                Trace("After LoadReportResponse.executionInfo.DataSourcePromptsResponse Mapping");
                                //newItemName = DataSourcePromptsssrsitem
                                //newParent_baseDTOName = VEISLRexecutionInfossrsInfo.VEISLRDataSourcePromptsssrsInfo
                                //writing out VEISLRexecutionInfossrsInfo.VEISLRDataSourcePromptsssrsInfo for :3da760dd-6627-e611-9440-0050568df261
                                //passing to ParentDTOName executionInfo.DataSourcePrompts
                                //No records existed beyond parent 
                                DataSourcePromptsssrsListRecord.Add(VEISLRDataSourcePromptsssrsRecord);
                            }
                            //parentRecordName:
                            //insideofMultiple:False
                            response.VEISLRexecutionInfossrsInfo.VEISLRDataSourcePromptsssrsInfo = DataSourcePromptsssrsListRecord.ToArray();
                        }
                        #endregion

                        #region  - LEGACY LoadReportResponse.executionInfo.ReportPageSettings Response Mapping 
                        var VEISLRReportPageSettingsssrsInfo = new VEISLRReportPageSettingsssrs();
                        var PageSettings = LoadReportResponse.executionInfo.ReportPageSettings;
                        if (PageSettings != null)
                        {
                            Trace("After LoadReportResponse.executionInfo.ReportPageSettingsResponse Mapping");
                            //newItemName = PageSettings
                            //newParent_baseDTOName = VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo
                            //writing out VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo for :4ba760dd-6627-e611-9440-0050568df261
                            //passing to ParentDTOName executionInfo.ReportPageSettings

                            #region  - LEGACY LoadReportResponse.executionInfo.ReportPageSettings.PaperSize Response Mapping 
                            var VEISLRPaperSizessrsInfo = new VEISLRPaperSizessrs();
                            var ReportPaperSize = PageSettings.PaperSize;
                            if (ReportPaperSize != null)
                            {
                                Trace("After LoadReportResponse.executionInfo.ReportPageSettings.PaperSizeResponse Mapping");
                                //newItemName = ReportPaperSize
                                //newParent_baseDTOName = VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRPaperSizessrsInfo
                                //writing out VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRPaperSizessrsInfo for :52a760dd-6627-e611-9440-0050568df261
                                //passing to ParentDTOName executionInfo.ReportPageSettings.PaperSize

                                #region  - LEGACY LoadReportResponse.executionInfo.ReportPageSettings.PaperSize.Height Response Mapping 
                                var VEISLRHeightssrsInfo = new VEISLRHeightssrs();
                                var Double = ReportPaperSize.Height;
                                if (Double != null)
                                {
                                    Trace("After LoadReportResponse.executionInfo.ReportPageSettings.PaperSize.HeightResponse Mapping");
                                    //newItemName = Double
                                    //newParent_baseDTOName = VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRPaperSizessrsInfo.VEISLRHeightssrsInfo
                                    //writing out VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRPaperSizessrsInfo.VEISLRHeightssrsInfo for :59a760dd-6627-e611-9440-0050568df261
                                    //passing to ParentDTOName executionInfo.ReportPageSettings.PaperSize.Height
                                    //No records existed beyond parent 
                                    VEISLRPaperSizessrsInfo.VEISLRHeightssrsInfo = VEISLRHeightssrsInfo;
                                }
                                #endregion

                                #region  - LEGACY LoadReportResponse.executionInfo.ReportPageSettings.PaperSize.Width Response Mapping 
                                var VEISLRWidthssrsInfo = new VEISLRWidthssrs();
                                var Double1 = ReportPaperSize.Width;
                                if (Double1 != null)
                                {
                                    Trace("After LoadReportResponse.executionInfo.ReportPageSettings.PaperSize.WidthResponse Mapping");
                                    //newItemName = Double1
                                    //newParent_baseDTOName = VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRPaperSizessrsInfo.VEISLRWidthssrsInfo
                                    //writing out VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRPaperSizessrsInfo.VEISLRWidthssrsInfo for :60a760dd-6627-e611-9440-0050568df261
                                    //passing to ParentDTOName executionInfo.ReportPageSettings.PaperSize.Width
                                    //No records existed beyond parent 
                                    VEISLRPaperSizessrsInfo.VEISLRWidthssrsInfo = VEISLRWidthssrsInfo;
                                }
                                #endregion
                                VEISLRReportPageSettingsssrsInfo.VEISLRPaperSizessrsInfo = VEISLRPaperSizessrsInfo;
                            }
                            #endregion

                            #region  - LEGACY LoadReportResponse.executionInfo.ReportPageSettings.Margins Response Mapping 
                            var VEISLRMarginsssrsInfo = new VEISLRMarginsssrs();
                            var ReportMargins = PageSettings.Margins;
                            if (ReportMargins != null)
                            {
                                Trace("After LoadReportResponse.executionInfo.ReportPageSettings.MarginsResponse Mapping");
                                //newItemName = ReportMargins
                                //newParent_baseDTOName = VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRMarginsssrsInfo
                                //writing out VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRMarginsssrsInfo for :67a760dd-6627-e611-9440-0050568df261
                                //passing to ParentDTOName executionInfo.ReportPageSettings.Margins

                                #region  - LEGACY LoadReportResponse.executionInfo.ReportPageSettings.Margins.Top Response Mapping 
                                var VEISLRTopssrsInfo = new VEISLRTopssrs();
                                var Double2 = ReportMargins.Top;
                                if (Double2 != null)
                                {
                                    Trace("After LoadReportResponse.executionInfo.ReportPageSettings.Margins.TopResponse Mapping");
                                    //newItemName = Double2
                                    //newParent_baseDTOName = VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRMarginsssrsInfo.VEISLRTopssrsInfo
                                    //writing out VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRMarginsssrsInfo.VEISLRTopssrsInfo for :6ea760dd-6627-e611-9440-0050568df261
                                    //passing to ParentDTOName executionInfo.ReportPageSettings.Margins.Top
                                    //No records existed beyond parent 
                                    VEISLRMarginsssrsInfo.VEISLRTopssrsInfo = VEISLRTopssrsInfo;
                                }
                                #endregion

                                #region  - LEGACY LoadReportResponse.executionInfo.ReportPageSettings.Margins.Bottom Response Mapping 
                                var VEISLRBottomssrsInfo = new VEISLRBottomssrs();
                                var Double3 = ReportMargins.Bottom;
                                if (Double3 != null)
                                {
                                    Trace("After LoadReportResponse.executionInfo.ReportPageSettings.Margins.BottomResponse Mapping");
                                    //newItemName = Double3
                                    //newParent_baseDTOName = VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRMarginsssrsInfo.VEISLRBottomssrsInfo
                                    //writing out VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRMarginsssrsInfo.VEISLRBottomssrsInfo for :75a760dd-6627-e611-9440-0050568df261
                                    //passing to ParentDTOName executionInfo.ReportPageSettings.Margins.Bottom
                                    //No records existed beyond parent 
                                    VEISLRMarginsssrsInfo.VEISLRBottomssrsInfo = VEISLRBottomssrsInfo;
                                }
                                #endregion

                                #region  - LEGACY LoadReportResponse.executionInfo.ReportPageSettings.Margins.Left Response Mapping 
                                var VEISLRLeftssrsInfo = new VEISLRLeftssrs();
                                var Double4 = ReportMargins.Left;
                                if (Double4 != null)
                                {
                                    Trace("After LoadReportResponse.executionInfo.ReportPageSettings.Margins.LeftResponse Mapping");
                                    //newItemName = Double4
                                    //newParent_baseDTOName = VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRMarginsssrsInfo.VEISLRLeftssrsInfo
                                    //writing out VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRMarginsssrsInfo.VEISLRLeftssrsInfo for :7ca760dd-6627-e611-9440-0050568df261
                                    //passing to ParentDTOName executionInfo.ReportPageSettings.Margins.Left
                                    //No records existed beyond parent 
                                    VEISLRMarginsssrsInfo.VEISLRLeftssrsInfo = VEISLRLeftssrsInfo;
                                }
                                #endregion

                                #region  - LEGACY LoadReportResponse.executionInfo.ReportPageSettings.Margins.Right Response Mapping 
                                var VEISLRRightssrsInfo = new VEISLRRightssrs();
                                var Double5 = ReportMargins.Right;
                                if (Double5 != null)
                                {
                                    Trace("After LoadReportResponse.executionInfo.ReportPageSettings.Margins.RightResponse Mapping");
                                    //newItemName = Double5
                                    //newParent_baseDTOName = VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRMarginsssrsInfo.VEISLRRightssrsInfo
                                    //writing out VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo.VEISLRMarginsssrsInfo.VEISLRRightssrsInfo for :83a760dd-6627-e611-9440-0050568df261
                                    //passing to ParentDTOName executionInfo.ReportPageSettings.Margins.Right
                                    //No records existed beyond parent 
                                    VEISLRMarginsssrsInfo.VEISLRRightssrsInfo = VEISLRRightssrsInfo;
                                }
                                #endregion
                                VEISLRReportPageSettingsssrsInfo.VEISLRMarginsssrsInfo = VEISLRMarginsssrsInfo;
                            }
                            #endregion
                            response.VEISLRexecutionInfossrsInfo.VEISLRReportPageSettingsssrsInfo = VEISLRReportPageSettingsssrsInfo;
                        }
                        #endregion
                    }
                }
                #endregion
                catch (Exception ex)
                {
                    return ReturnFailure(response, ex.Message, SoapLog.Current);
                }
                #endregion

                return response;
            }

            catch (Exception ex)
            {
                return ReturnFailure(response, ex.Message, SoapLog.Current);
            }
            finally
            {
                if (response == null) response = new VEISLRLoadReportResponse();
                if (serviceTimer != null)
                {
                    if (serviceTimer.IsRunning)
                        serviceTimer.Stop();
                    response.ServiceTimer = serviceTimer.ElapsedMilliseconds;
                }
                response.SerializedSOAPRequest = SoapLog.Current?.Request;
                response.SerializedSOAPResponse = SoapLog.Current?.Response;
                CloseOutSoapLog();
            }
        }

        private VEISLRLoadReportResponse ReturnFailure(VEISLRLoadReportResponse response, string message, SoapLog log)
        {
            Trace($"EC Error occurred in findallptcpntdpositacntbyfilenumber: {message}.", LogLevel.Error);
            if (response == null)
            {
                response = new VEISLRLoadReportResponse();
            }

            response.ExceptionOccurred = true;
            response.ExceptionMessage = message;
            return response;
        }

        private static void CloseOutSoapLog()
        {
            SoapLog.Current.Active = false;
            SoapLog.Current.ClearLog();
        }
    }
}

