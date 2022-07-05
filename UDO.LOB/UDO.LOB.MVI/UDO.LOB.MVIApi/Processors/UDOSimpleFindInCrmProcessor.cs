using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Common;
using UDO.LOB.MVI.Messages;
using UDO.LOB.MVI.Processors;
using UDO.LOB.PersonSearch.Processors;
using VRM.Integration.UDO.MVI.Faux_Processors;
using VEIS.Messages.VeteranWebService;
using MVIMessages = VEIS.Mvi.Messages;

namespace UDO.LOB.MVI.Processors
{
    public class UDOSimpleFindInCrmProcessor
    {
        private const string method = "UDOSelectedPersonResponse";

        public IMessageBase Execute(UDOSelectedPersonRequest request)
        {
            UDOSelectedPersonResponse response = new UDOSelectedPersonResponse();
            Stopwatch txnTimer = Stopwatch.StartNew();
            Stopwatch EntireTimer = Stopwatch.StartNew();
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            try
            {
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOSimpleFindInCrmProcessor", "Top", request.Debug);

                if (request == null)
                {
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, $"{GetType().FullName}.Execute",
                         $"<< Exit from {GetType().FullName}. Recieved a null {request.GetType().Name} message request or request.Person.");

                }
                else
                {
                    var processor = new FindinCRMProcessor();
                    response = (UDOSelectedPersonResponse)processor.Execute(request);
                    txnTimer.Stop();
                    txnTimer.Restart();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "#Error in UDOSimpleFindInCrmProcessor.Execute ", ex);
                response.ExceptionOccured = true;
                response.Message = ex.Message;
                return response;
            }
            EntireTimer.Stop();

            return response;
        }

    }
}