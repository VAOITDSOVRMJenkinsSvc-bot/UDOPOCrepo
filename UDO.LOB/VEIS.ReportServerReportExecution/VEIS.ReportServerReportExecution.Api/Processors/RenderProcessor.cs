using System;
using System.Diagnostics;
using System.Collections.Generic;
using VEIS.Core.Core;
using VEIS.Core.Messages;
using VEIS.Core.Processor;
using VEIS.Core.Wcf;
using VEIS.Messages.ReportServerReportExecution; 
using VEIS.ReportServerReportExecution.Services;
using VEIS.ReportServerReportExecution.Services.ReportServerReportExecution;

namespace VEIS.ReportServerReportExecution.Api.Processors
{
    public class VEISrdrRenderGetDataProcessor : EcProcessorBase
    {
        /// <summary> 
        /// </summary>
        /// <param name=none></param>
        /// <returns>none</returns>
        public override VEISEcResponseBase Process(VEISEcRequestBase req)
        {
            VEISrdrRenderResponse response = null;
            VEISrdrRenderRequest request = null;
            Stopwatch serviceTimer = new Stopwatch();
            try
            {
                Trace("Top of Processor");
                request = (VEISrdrRenderRequest)req;
                 
                #region do legacy webservice calls
                string method = "RenderProcessor", webService = "Render";

                var RenderResponse = new RenderResponse();
                Stopwatch thisTimer = Stopwatch.StartNew();

                try
                {
                    SoapLog.Current.Active = true;
                     RenderRequest renderRequest = new RenderRequest();
                    renderRequest.ExecutionHeader  = new ExecutionHeader();
                    if (!string.IsNullOrEmpty(request.executionheaderInfo.mcs_ExecutionID)) renderRequest.ExecutionHeader.ExecutionID = request.executionheaderInfo.mcs_ExecutionID;
                    var TrustedUserHeaderData = new TrustedUserHeader();
                    if (!string.IsNullOrEmpty(request.trusteduserheaderInfo.mcs_UserName)) TrustedUserHeaderData.UserName = request.trusteduserheaderInfo.mcs_UserName;
                    TrustedUserHeaderData.UserToken = request.trusteduserheaderInfo.mcs_UserToken;
 
						var service = ServiceFactory.GetReportServerReportExecutionServiceReference(request.LegacyServiceHeaderInfo);



                    Trace("Before Service Call");
                    serviceTimer.Start();
                    RenderResponse = service.Render(renderRequest);
                    serviceTimer.Stop();
                    Trace("After Service Call");
                }
                catch (Exception ex)
                {
                    serviceTimer.Stop();
                    if (response == null) response = new VEISrdrRenderResponse();
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
                    response = new VEISrdrRenderResponse();
                    //RenderResponse Mapping 
                    #region  RenderResponse Mapping 
                    System.Collections.Generic.List<VEISrdrStreamIdsssrsMultipleResponse> StreamIdsssrsRecord = new System.Collections.Generic.List<VEISrdrStreamIdsssrsMultipleResponse>();
                    VEISrdrStreamIdsssrsMultipleResponse[] veisrdrStreamIdsssrsMultipleResponse = response.VEISrdrStreamIdsssrsInfo;
                    if (veisrdrStreamIdsssrsMultipleResponse != null)
                    {
                        foreach (var VEISrdrStreamIdsssrsitem in veisrdrStreamIdsssrsMultipleResponse)
                        {
                            var VEISrdrStreamIdsssrsRecord = new VEISrdrStreamIdsssrsMultipleResponse();
                            //passing to parentDTOName StreamIds
                            //No records existed beyond parent 
                            StreamIdsssrsRecord.Add(VEISrdrStreamIdsssrsRecord);
                        }
                        response.VEISrdrStreamIdsssrsInfo = StreamIdsssrsRecord.ToArray();
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
                if (response == null) response = new VEISrdrRenderResponse();
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

        private VEISrdrRenderResponse ReturnFailure(VEISrdrRenderResponse response, string message, SoapLog log)
        {
            Trace($"EC Error occurred in findallptcpntdpositacntbyfilenumber: {message}.", LogLevel.Error);
            if (response == null)
            {
                response = new VEISrdrRenderResponse();
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

