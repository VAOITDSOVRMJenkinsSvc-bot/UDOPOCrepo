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
    public class VEISSEPSetExecutionParametersGetDataProcessor : EcProcessorBase
    {
        /// <summary> 
        /// </summary>
        /// <param name=none></param>
        /// <returns>none</returns>
        public override VEISEcResponseBase Process(VEISEcRequestBase req)
        {
            VEISSEPSetExecutionParametersResponse response = null;
            VEISSEPSetExecutionParametersRequest request = null;
            Stopwatch serviceTimer = new Stopwatch();
            try
            {
                Trace("Top of Processor");
                request = (VEISSEPSetExecutionParametersRequest)req;
                #region do legacy webservice calls
                string method = "SetExecutionParametersProcessor", webService = "SetExecutionParameters";

                var SetExecutionParametersResponse = new SetExecutionParametersResponse();
                Stopwatch thisTimer = Stopwatch.StartNew();

                try
                {
                    SoapLog.Current.Active = true;
                    SetExecutionParametersRequest setExecutionParametersRequest = new SetExecutionParametersRequest()
                    { 
                        ParameterLanguage = request.mcs_parameterlanguage 
                    };
                    setExecutionParametersRequest.ExecutionHeader = new ExecutionHeader();
                    if (!string.IsNullOrEmpty(request.executionheaderInfo.mcs_ExecutionID)) setExecutionParametersRequest.ExecutionHeader.ExecutionID = request.executionheaderInfo.mcs_ExecutionID;


                    setExecutionParametersRequest.TrustedUserHeader = new TrustedUserHeader();
                    if (!string.IsNullOrEmpty(request.trusteduserheaderInfo.mcs_UserName)) setExecutionParametersRequest.TrustedUserHeader.UserName = request.trusteduserheaderInfo.mcs_UserName;


                    setExecutionParametersRequest.TrustedUserHeader.UserToken = request.trusteduserheaderInfo.mcs_UserToken;

                    var service = ServiceFactory.GetReportServerReportExecutionServiceReference(request.LegacyServiceHeaderInfo);


                    Trace("Before Service Call");
                    serviceTimer.Start();
                    SetExecutionParametersResponse = service.SetExecutionParameters(setExecutionParametersRequest);
                    serviceTimer.Stop();
                    Trace("After Service Call");
                }
                catch (Exception ex)
                {
                    serviceTimer.Stop();
                    if (response == null) response = new VEISSEPSetExecutionParametersResponse();
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
                    response = new VEISSEPSetExecutionParametersResponse();
                    //SetExecutionParametersResponse Mapping 
                    #region  SetExecutionParametersResponse Mapping 
                    var VEISSEPexecutionInfossrsInfo = new VEISSEPexecutionInfossrs();
                    var executioninfo = SetExecutionParametersResponse.executionInfo;
                    if (executioninfo != null)
                    {
                        VEISSEPexecutionInfossrsInfo.mcs_HasSnapshot = executioninfo.HasSnapshot;
                        VEISSEPexecutionInfossrsInfo.mcs_NeedsProcessing = executioninfo.NeedsProcessing;
                        VEISSEPexecutionInfossrsInfo.mcs_AllowQueryExecution = executioninfo.AllowQueryExecution;
                        VEISSEPexecutionInfossrsInfo.mcs_CredentialsRequired = executioninfo.CredentialsRequired;
                        VEISSEPexecutionInfossrsInfo.mcs_ParametersRequired = executioninfo.ParametersRequired;
                        VEISSEPexecutionInfossrsInfo.mcs_ExpirationDateTime = executioninfo.ExpirationDateTime;
                        VEISSEPexecutionInfossrsInfo.mcs_ExecutionDateTime = executioninfo.ExecutionDateTime;
                        VEISSEPexecutionInfossrsInfo.mcs_NumPages = executioninfo.NumPages;
                        VEISSEPexecutionInfossrsInfo.mcs_HasDocumentMap = executioninfo.HasDocumentMap;
                        VEISSEPexecutionInfossrsInfo.mcs_ExecutionID = executioninfo.ExecutionID;
                        VEISSEPexecutionInfossrsInfo.mcs_ReportPath = executioninfo.ReportPath;
                        VEISSEPexecutionInfossrsInfo.mcs_HistoryID = executioninfo.HistoryID;
                        VEISSEPexecutionInfossrsInfo.mcs_AutoRefreshInterval = executioninfo.AutoRefreshInterval;
                        response.VEISSEPexecutionInfossrsInfo = VEISSEPexecutionInfossrsInfo;
                        Trace("After SetExecutionParametersResponse Mapping");

                        //passing to parentDTOName executionInfo

                        #region  - LEGACY SetExecutionParametersResponse.executionInfo.Parameters Response Mapping 
                        System.Collections.Generic.List<VEISSEPParametersssrsMultipleResponse> ParametersssrsListRecord = new System.Collections.Generic.List<VEISSEPParametersssrsMultipleResponse>();
                        var ReportParameter = SetExecutionParametersResponse.executionInfo.Parameters;
                        if (ReportParameter != null)
                        {
                            foreach (var Parametersssrsitem in ReportParameter)
                            {
                                var VEISSEPParametersssrsRecord = new VEISSEPParametersssrsMultipleResponse();
                                VEISSEPParametersssrsRecord.mcs_Name = Parametersssrsitem.Name;
                                Trace("Parametersssrsitem.Name");
                                VEISSEPParametersssrsRecord.mcs_TypeSpecified = Parametersssrsitem.TypeSpecified;
                                Trace("Parametersssrsitem.TypeSpecified");
                                VEISSEPParametersssrsRecord.mcs_Nullable = Parametersssrsitem.Nullable;
                                Trace("Parametersssrsitem.Nullable");
                                VEISSEPParametersssrsRecord.mcs_NullableSpecified = Parametersssrsitem.NullableSpecified;
                                Trace("Parametersssrsitem.NullableSpecified");
                                VEISSEPParametersssrsRecord.mcs_AllowBlank = Parametersssrsitem.AllowBlank;
                                Trace("Parametersssrsitem.AllowBlank");
                                VEISSEPParametersssrsRecord.mcs_AllowBlankSpecified = Parametersssrsitem.AllowBlankSpecified;
                                Trace("Parametersssrsitem.AllowBlankSpecified");
                                VEISSEPParametersssrsRecord.mcs_MultiValue = Parametersssrsitem.MultiValue;
                                Trace("Parametersssrsitem.MultiValue");
                                VEISSEPParametersssrsRecord.mcs_MultiValueSpecified = Parametersssrsitem.MultiValueSpecified;
                                Trace("Parametersssrsitem.MultiValueSpecified");
                                VEISSEPParametersssrsRecord.mcs_QueryParameter = Parametersssrsitem.QueryParameter;
                                Trace("Parametersssrsitem.QueryParameter");
                                VEISSEPParametersssrsRecord.mcs_QueryParameterSpecified = Parametersssrsitem.QueryParameterSpecified;
                                Trace("Parametersssrsitem.QueryParameterSpecified");
                                VEISSEPParametersssrsRecord.mcs_Prompt = Parametersssrsitem.Prompt;
                                Trace("Parametersssrsitem.Prompt");
                                VEISSEPParametersssrsRecord.mcs_PromptUser = Parametersssrsitem.PromptUser;
                                Trace("Parametersssrsitem.PromptUser");
                                VEISSEPParametersssrsRecord.mcs_PromptUserSpecified = Parametersssrsitem.PromptUserSpecified;
                                Trace("Parametersssrsitem.PromptUserSpecified");
                                VEISSEPParametersssrsRecord.mcs_Dependencies = Parametersssrsitem.Dependencies;
                                Trace("Parametersssrsitem.Dependencies");
                                VEISSEPParametersssrsRecord.mcs_ValidValuesQueryBased = Parametersssrsitem.ValidValuesQueryBased;
                                Trace("Parametersssrsitem.ValidValuesQueryBased");
                                VEISSEPParametersssrsRecord.mcs_ValidValuesQueryBasedSpecified = Parametersssrsitem.ValidValuesQueryBasedSpecified;
                                Trace("Parametersssrsitem.ValidValuesQueryBasedSpecified");
                                VEISSEPParametersssrsRecord.mcs_DefaultValuesQueryBased = Parametersssrsitem.DefaultValuesQueryBased;
                                Trace("Parametersssrsitem.DefaultValuesQueryBased");
                                VEISSEPParametersssrsRecord.mcs_DefaultValuesQueryBasedSpecified = Parametersssrsitem.DefaultValuesQueryBasedSpecified;
                                Trace("Parametersssrsitem.DefaultValuesQueryBasedSpecified");
                                VEISSEPParametersssrsRecord.mcs_DefaultValues = Parametersssrsitem.DefaultValues;
                                Trace("Parametersssrsitem.DefaultValues");
                                VEISSEPParametersssrsRecord.mcs_StateSpecified = Parametersssrsitem.StateSpecified;
                                Trace("Parametersssrsitem.StateSpecified");
                                VEISSEPParametersssrsRecord.mcs_ErrorMessage = Parametersssrsitem.ErrorMessage;
                                Trace("Parametersssrsitem.ErrorMessage");
                                Trace("After SetExecutionParametersResponse.executionInfo.ParametersResponse Mapping");
                                //newItemName = Parametersssrsitem
                                //newParent_baseDTOName = VEISSEPexecutionInfossrsInfo.VEISSEPParametersssrsInfo
                                //writing out VEISSEPexecutionInfossrsInfo.VEISSEPParametersssrsInfo for :b387d7ec-6927-e611-9440-0050568df261
                                //passing to ParentDTOName executionInfo.Parameters

                                #region  - LEGACY SetExecutionParametersResponse.executionInfo.Parameters.ValidValues Response Mapping 
                                System.Collections.Generic.List<VEISSEPValidValuesssrsMultipleResponse> ValidValuesssrsListRecord = new System.Collections.Generic.List<VEISSEPValidValuesssrsMultipleResponse>();
                                var ValidValue = Parametersssrsitem.ValidValues;
                                if (ValidValue != null)
                                {
                                    foreach (var ValidValuesssrsitem in ValidValue)
                                    {
                                        var VEISSEPValidValuesssrsRecord = new VEISSEPValidValuesssrsMultipleResponse();
                                        VEISSEPValidValuesssrsRecord.mcs_Label = ValidValuesssrsitem.Label;
                                        Trace("ValidValuesssrsitem.Label");
                                        VEISSEPValidValuesssrsRecord.mcs_Value = ValidValuesssrsitem.Value;
                                        Trace("ValidValuesssrsitem.Value");
                                        Trace("After SetExecutionParametersResponse.executionInfo.Parameters.ValidValuesResponse Mapping");
                                        //newItemName = ValidValuesssrsitem
                                        //newParent_baseDTOName = VEISSEPexecutionInfossrsInfo.VEISSEPParametersssrsInfo.VEISSEPValidValuesssrsInfo
                                        //writing out VEISSEPexecutionInfossrsInfo.VEISSEPParametersssrsInfo.VEISSEPValidValuesssrsInfo for :ca87d7ec-6927-e611-9440-0050568df261
                                        //passing to ParentDTOName executionInfo.Parameters.ValidValues
                                        //No records existed beyond parent 
                                        ValidValuesssrsListRecord.Add(VEISSEPValidValuesssrsRecord);
                                    }
                                    //parentRecordName:VEISSEPParametersssrsRecord
                                    //insideofMultiple:True
                                    VEISSEPParametersssrsRecord.VEISSEPValidValuesssrsInfo = ValidValuesssrsListRecord.ToArray();
                                }
                                #endregion
                                ParametersssrsListRecord.Add(VEISSEPParametersssrsRecord);
                            }
                            //parentRecordName:
                            //insideofMultiple:False
                            response.VEISSEPexecutionInfossrsInfo.VEISSEPParametersssrsInfo = ParametersssrsListRecord.ToArray();
                        }
                        #endregion

                        #region  - LEGACY SetExecutionParametersResponse.executionInfo.DataSourcePrompts Response Mapping 
                        System.Collections.Generic.List<VEISSEPDataSourcePromptsssrsMultipleResponse> DataSourcePromptsssrsListRecord = new System.Collections.Generic.List<VEISSEPDataSourcePromptsssrsMultipleResponse>();
                        var DataSourcePrompt = SetExecutionParametersResponse.executionInfo.DataSourcePrompts;
                        if (DataSourcePrompt != null)
                        {
                            foreach (var DataSourcePromptsssrsitem in DataSourcePrompt)
                            {
                                var VEISSEPDataSourcePromptsssrsRecord = new VEISSEPDataSourcePromptsssrsMultipleResponse();
                                VEISSEPDataSourcePromptsssrsRecord.mcs_Name = DataSourcePromptsssrsitem.Name;
                                Trace("DataSourcePromptsssrsitem.Name");
                                VEISSEPDataSourcePromptsssrsRecord.mcs_DataSourceID = DataSourcePromptsssrsitem.DataSourceID;
                                Trace("DataSourcePromptsssrsitem.DataSourceID");
                                VEISSEPDataSourcePromptsssrsRecord.mcs_Prompt = DataSourcePromptsssrsitem.Prompt;
                                Trace("DataSourcePromptsssrsitem.Prompt");
                                Trace("After SetExecutionParametersResponse.executionInfo.DataSourcePromptsResponse Mapping");
                                //newItemName = DataSourcePromptsssrsitem
                                //newParent_baseDTOName = VEISSEPexecutionInfossrsInfo.VEISSEPDataSourcePromptsssrsInfo
                                //writing out VEISSEPexecutionInfossrsInfo.VEISSEPDataSourcePromptsssrsInfo for :d887d7ec-6927-e611-9440-0050568df261
                                //passing to ParentDTOName executionInfo.DataSourcePrompts
                                //No records existed beyond parent 
                                DataSourcePromptsssrsListRecord.Add(VEISSEPDataSourcePromptsssrsRecord);
                            }
                            //parentRecordName:
                            //insideofMultiple:False
                            response.VEISSEPexecutionInfossrsInfo.VEISSEPDataSourcePromptsssrsInfo = DataSourcePromptsssrsListRecord.ToArray();
                        }
                        #endregion

                        #region  - LEGACY SetExecutionParametersResponse.executionInfo.ReportPageSettings Response Mapping 
                        var VEISSEPReportPageSettingsssrsInfo = new VEISSEPReportPageSettingsssrs();
                        var PageSettings = SetExecutionParametersResponse.executionInfo.ReportPageSettings;
                        if (PageSettings != null)
                        {
                            Trace("After SetExecutionParametersResponse.executionInfo.ReportPageSettingsResponse Mapping");
                            //newItemName = PageSettings
                            //newParent_baseDTOName = VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo
                            //writing out VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo for :e687d7ec-6927-e611-9440-0050568df261
                            //passing to ParentDTOName executionInfo.ReportPageSettings

                            #region  - LEGACY SetExecutionParametersResponse.executionInfo.ReportPageSettings.PaperSize Response Mapping 
                            var VEISSEPPaperSizessrsInfo = new VEISSEPPaperSizessrs();
                            var ReportPaperSize = PageSettings.PaperSize;
                            if (ReportPaperSize != null)
                            {
                                Trace("After SetExecutionParametersResponse.executionInfo.ReportPageSettings.PaperSizeResponse Mapping");
                                //newItemName = ReportPaperSize
                                //newParent_baseDTOName = VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPPaperSizessrsInfo
                                //writing out VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPPaperSizessrsInfo for :ed87d7ec-6927-e611-9440-0050568df261
                                //passing to ParentDTOName executionInfo.ReportPageSettings.PaperSize

                                #region  - LEGACY SetExecutionParametersResponse.executionInfo.ReportPageSettings.PaperSize.Height Response Mapping 
                                var VEISSEPHeightssrsInfo = new VEISSEPHeightssrs();
                                var Double = ReportPaperSize.Height;
                                if (Double != null)
                                {
                                    Trace("After SetExecutionParametersResponse.executionInfo.ReportPageSettings.PaperSize.HeightResponse Mapping");
                                    //newItemName = Double
                                    //newParent_baseDTOName = VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPPaperSizessrsInfo.VEISSEPHeightssrsInfo
                                    //writing out VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPPaperSizessrsInfo.VEISSEPHeightssrsInfo for :f487d7ec-6927-e611-9440-0050568df261
                                    //passing to ParentDTOName executionInfo.ReportPageSettings.PaperSize.Height
                                    //No records existed beyond parent 
                                    VEISSEPPaperSizessrsInfo.VEISSEPHeightssrsInfo = VEISSEPHeightssrsInfo;
                                }
                                #endregion

                                #region  - LEGACY SetExecutionParametersResponse.executionInfo.ReportPageSettings.PaperSize.Width Response Mapping 
                                var VEISSEPWidthssrsInfo = new VEISSEPWidthssrs();
                                var Double1 = ReportPaperSize.Width;
                                if (Double1 != null)
                                {
                                    Trace("After SetExecutionParametersResponse.executionInfo.ReportPageSettings.PaperSize.WidthResponse Mapping");
                                    //newItemName = Double1
                                    //newParent_baseDTOName = VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPPaperSizessrsInfo.VEISSEPWidthssrsInfo
                                    //writing out VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPPaperSizessrsInfo.VEISSEPWidthssrsInfo for :fb87d7ec-6927-e611-9440-0050568df261
                                    //passing to ParentDTOName executionInfo.ReportPageSettings.PaperSize.Width
                                    //No records existed beyond parent 
                                    VEISSEPPaperSizessrsInfo.VEISSEPWidthssrsInfo = VEISSEPWidthssrsInfo;
                                }
                                #endregion
                                VEISSEPReportPageSettingsssrsInfo.VEISSEPPaperSizessrsInfo = VEISSEPPaperSizessrsInfo;
                            }
                            #endregion

                            #region  - LEGACY SetExecutionParametersResponse.executionInfo.ReportPageSettings.Margins Response Mapping 
                            var VEISSEPMarginsssrsInfo = new VEISSEPMarginsssrs();
                            var ReportMargins = PageSettings.Margins;
                            if (ReportMargins != null)
                            {
                                Trace("After SetExecutionParametersResponse.executionInfo.ReportPageSettings.MarginsResponse Mapping");
                                //newItemName = ReportMargins
                                //newParent_baseDTOName = VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPMarginsssrsInfo
                                //writing out VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPMarginsssrsInfo for :0288d7ec-6927-e611-9440-0050568df261
                                //passing to ParentDTOName executionInfo.ReportPageSettings.Margins

                                #region  - LEGACY SetExecutionParametersResponse.executionInfo.ReportPageSettings.Margins.Top Response Mapping 
                                var VEISSEPTopssrsInfo = new VEISSEPTopssrs();
                                var Double2 = ReportMargins.Top;
                                if (Double2 != null)
                                {
                                    Trace("After SetExecutionParametersResponse.executionInfo.ReportPageSettings.Margins.TopResponse Mapping");
                                    //newItemName = Double2
                                    //newParent_baseDTOName = VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPMarginsssrsInfo.VEISSEPTopssrsInfo
                                    //writing out VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPMarginsssrsInfo.VEISSEPTopssrsInfo for :0988d7ec-6927-e611-9440-0050568df261
                                    //passing to ParentDTOName executionInfo.ReportPageSettings.Margins.Top
                                    //No records existed beyond parent 
                                    VEISSEPMarginsssrsInfo.VEISSEPTopssrsInfo = VEISSEPTopssrsInfo;
                                }
                                #endregion

                                #region  - LEGACY SetExecutionParametersResponse.executionInfo.ReportPageSettings.Margins.Bottom Response Mapping 
                                var VEISSEPBottomssrsInfo = new VEISSEPBottomssrs();
                                var Double3 = ReportMargins.Bottom;
                                if (Double3 != null)
                                {
                                    Trace("After SetExecutionParametersResponse.executionInfo.ReportPageSettings.Margins.BottomResponse Mapping");
                                    //newItemName = Double3
                                    //newParent_baseDTOName = VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPMarginsssrsInfo.VEISSEPBottomssrsInfo
                                    //writing out VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPMarginsssrsInfo.VEISSEPBottomssrsInfo for :1088d7ec-6927-e611-9440-0050568df261
                                    //passing to ParentDTOName executionInfo.ReportPageSettings.Margins.Bottom
                                    //No records existed beyond parent 
                                    VEISSEPMarginsssrsInfo.VEISSEPBottomssrsInfo = VEISSEPBottomssrsInfo;
                                }
                                #endregion

                                #region  - LEGACY SetExecutionParametersResponse.executionInfo.ReportPageSettings.Margins.Left Response Mapping 
                                var VEISSEPLeftssrsInfo = new VEISSEPLeftssrs();
                                var Double4 = ReportMargins.Left;
                                if (Double4 != null)
                                {
                                    Trace("After SetExecutionParametersResponse.executionInfo.ReportPageSettings.Margins.LeftResponse Mapping");
                                    //newItemName = Double4
                                    //newParent_baseDTOName = VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPMarginsssrsInfo.VEISSEPLeftssrsInfo
                                    //writing out VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPMarginsssrsInfo.VEISSEPLeftssrsInfo for :1788d7ec-6927-e611-9440-0050568df261
                                    //passing to ParentDTOName executionInfo.ReportPageSettings.Margins.Left
                                    //No records existed beyond parent 
                                    VEISSEPMarginsssrsInfo.VEISSEPLeftssrsInfo = VEISSEPLeftssrsInfo;
                                }
                                #endregion

                                #region  - LEGACY SetExecutionParametersResponse.executionInfo.ReportPageSettings.Margins.Right Response Mapping 
                                var VEISSEPRightssrsInfo = new VEISSEPRightssrs();
                                var Double5 = ReportMargins.Right;
                                if (Double5 != null)
                                {
                                    Trace("After SetExecutionParametersResponse.executionInfo.ReportPageSettings.Margins.RightResponse Mapping");
                                    //newItemName = Double5
                                    //newParent_baseDTOName = VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPMarginsssrsInfo.VEISSEPRightssrsInfo
                                    //writing out VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo.VEISSEPMarginsssrsInfo.VEISSEPRightssrsInfo for :1e88d7ec-6927-e611-9440-0050568df261
                                    //passing to ParentDTOName executionInfo.ReportPageSettings.Margins.Right
                                    //No records existed beyond parent 
                                    VEISSEPMarginsssrsInfo.VEISSEPRightssrsInfo = VEISSEPRightssrsInfo;
                                }
                                #endregion
                                VEISSEPReportPageSettingsssrsInfo.VEISSEPMarginsssrsInfo = VEISSEPMarginsssrsInfo;
                            }
                            #endregion
                            response.VEISSEPexecutionInfossrsInfo.VEISSEPReportPageSettingsssrsInfo = VEISSEPReportPageSettingsssrsInfo;
                        }
                        #endregion
                    }
                    #endregion
                }
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
                if (response == null) response = new VEISSEPSetExecutionParametersResponse();
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

        private VEISSEPSetExecutionParametersResponse ReturnFailure(VEISSEPSetExecutionParametersResponse response, string message, SoapLog log)
        {
            Trace($"EC Error occurred in findallptcpntdpositacntbyfilenumber: {message}.", LogLevel.Error);
            if (response == null)
            {
                response = new VEISSEPSetExecutionParametersResponse();
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

