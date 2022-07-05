using System;
using System.Collections.Generic;
using System.Diagnostics;
using UDO.LOB.Core;
using UDO.LOB.eMIS.Messages;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Messages;
using VEIS.Mvi.Messages;
using VRM.Integration.UDO.MVI.Faux_Processors;
using MVIMessages = VEIS.Mvi.Messages;

namespace UDO.LOB.MVI.Processors
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class UDOSelectedPersonProcessor
    {
        // REM: New variables
        private const string method = "UDOSelectedPersonResponse";

        private Uri veisBaseUri;
        private LogSettings logSettings { get; set; }
        /// <summary>
        /// Execute the UDOSelectedPersonRequest message
        /// </summary>
        /// <param name="request"></param>
        /// <returns>UDOSelectedPersonResponse</returns>
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
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOSelectedPersonProcessor", "Top", request.Debug);

                if (request == null)
                {
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, $"{GetType().FullName}.Execute",
                         $"<< Exit from {GetType().FullName}. Recieved a null {request.GetType().Name} message request or request.Person.");

                }
                else
                {
                    switch (request.RecordSource)
                    {

                        case "MVI":
                            #region - do the real MVI search - MVI handles the whole attended versus unattended thing
                            var processor = new FindinCRMProcessor();
                            //new for UDO is the orchestration- - bazsed on ICN
                            if (request.ICN == null)
                            {
                                #region NO EDIPI processing
                                if (request.Edipi == null)
                                {
                                    var selectedPersonRequest = !string.IsNullOrEmpty(request.RawValueFromMvi) ?
                                        // if searching using the raw value from the original MVI search
                                        new MVIMessages.SelectedPersonRequest(request.RawValueFromMvi, request.OrganizationName,
                                            request.UserFirstName, request.UserLastName,
                                            request.UserId, request.MessageId, request.RecordSource) :
                                            !string.IsNullOrEmpty(request.Edipi) ?
                                                // if searching using EDIPI
                                                new MVIMessages.SelectedPersonRequest(request.Edipi, request.UserId,
                                                    request.UserFirstName, request.UserLastName, request.OrganizationName,
                                                    request.MessageId, request.RecordSource) :
                                                // if searching using PatienId
                                                new MVIMessages.SelectedPersonRequest(request.PatientSearchIdentifier,
                                                    request.UserFirstName, request.UserLastName, request.UserId,
                                                    request.OrganizationName, request.MessageId, request.RecordSource);

                                    selectedPersonRequest.MessageId = request.MessageId;
                                    var mviResponse = WebApiUtility.SendReceive<MVIMessages.CorrespondingIdsResponse>(selectedPersonRequest, WebApiType.VEIS);
                                    if (request.LogSoap || mviResponse.ExceptionOccurred)
                                    {
                                        if (mviResponse.SerializedSOAPRequest != null || mviResponse.SerializedSOAPResponse != null)
                                        {
                                            var requestResponse = mviResponse.SerializedSOAPRequest + mviResponse.SerializedSOAPResponse;
                                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"SelectedPersonRequest Request/Response {requestResponse}", true);
                                        }
                                    }

                                    txnTimer.Stop();
                                    txnTimer.Restart();

                                    response = (UDOSelectedPersonResponse)processor.Execute(mviResponse, request);
                                }
                                #endregion
                                else
                                {
                                    #region Have EDIPI
                                    var mviOrchestrationresponse = new MVIMessages.RetrieveOrSearchPersonResponse();
                                    if (!request.ByPassMVI)
                                    {
                                        //most UDO users will come in here - as they don't have EDIPI so we do this
                                        var mviOrchestrationRequest = new MVIMessages.RetrieveWithOrchestrationRequest(request.Edipi, request.UserId, request.UserFirstName, request.UserLastName, request.OrganizationName);

                                        //REM: VEIS WebApi
                                        mviOrchestrationRequest.MessageId = request.MessageId;
                                        mviOrchestrationresponse = WebApiUtility.SendReceive<MVIMessages.RetrieveOrSearchPersonResponse>(mviOrchestrationRequest, WebApiType.VEIS);
                                        if (request.LogSoap || mviOrchestrationresponse.ExceptionOccurred)
                                        {
                                            if (mviOrchestrationresponse.SerializedSOAPRequest != null || mviOrchestrationresponse.SerializedSOAPResponse != null)
                                            {
                                                var requestResponse = mviOrchestrationresponse.SerializedSOAPRequest + mviOrchestrationresponse.SerializedSOAPResponse;
                                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"RetrieveWithOrchestrationRequest Request/Response {requestResponse}", true);
                                            }
                                        }
                                        txnTimer.Stop();
                                        txnTimer.Restart();

                                        //This adds the CORPDB stuff to the record.
                                        if (!mviOrchestrationresponse.ExceptionOccured)
                                        {
                                            MVIMessages.CorrespondingIdsResponse newRequest = new MVIMessages.CorrespondingIdsResponse();
                                            newRequest.CorrespondingIdList = mviOrchestrationresponse.Person[0].CorrespondingIdList;
                                            newRequest.OrganizationName = request.OrganizationName;
                                            newRequest.UserId = request.UserId;

                                            response = (UDOSelectedPersonResponse)processor.Execute(newRequest, request);
                                            txnTimer.Stop();
                                            txnTimer.Restart();
                                        }
                                    }
                                    else
                                    {
                                        MVIMessages.CorrespondingIdsResponse newRequest = new MVIMessages.CorrespondingIdsResponse();
                                        List<UnattendedSearchRequest> corIdList = new List<UnattendedSearchRequest>();
                                        foreach (var corId in request.CorrespondingIdList)
                                        {
                                            corIdList.Add(new UnattendedSearchRequest
                                            {
                                                AssigningAuthority = corId.AssigningAuthority,
                                                AssigningFacility = corId.AssigningFacility,
                                                AuthorityOid = corId.AuthorityOid,
                                                IdentifierType = corId.IdentifierType,
                                                OrganizationName = corId.OrganizationName,
                                                PatientIdentifier = corId.PatientIdentifier,
                                                RawValueFromMvi = corId.RawValueFromMvi,
                                                UserFirstName = corId.UserFirstName,
                                                UserLastName = corId.UserLastName,
                                                UserId = corId.UserId
                                            });
                                        }
                                        newRequest.CorrespondingIdList = corIdList.ToArray();
                                        newRequest.OrganizationName = request.OrganizationName;
                                        newRequest.UserId = request.UserId;
                                        newRequest.MessageId = request.MessageId;

                                        response = (UDOSelectedPersonResponse)processor.Execute(newRequest, request);
                                        txnTimer.Stop();
                                        txnTimer.Restart();
                                    }

                                    #region Get Rank
                                    //CSDev REM: Sourced from E:\*\SourceCode\MCS UDO M\UDO.LOB\UDO.Crm.LOB\Messages\eMIS
                                    try
                                    {
                                        var miRequest = new UDOgetMilitaryInformationRequest()
                                        { 
                                            Debug = request.Debug,
                                            LogSoap = request.LogSoap,
                                            LogTiming = request.LogTiming,
                                            MessageId = Guid.NewGuid().ToString(),
                                            OrganizationName = request.OrganizationName,
                                            UserId = request.UserId,
                                            udo_MostRecentServiceOnly = true,
                                            udo_EDIPI = request.Edipi,
                                            udo_ICN = request.ICN
                                        };

                                        // REM: Invoke eMIS LOB endpoint
                                        var miResponse = WebApiUtility.SendReceive<UDOgetMilitaryInformationResponse>(miRequest, WebApiType.LOB);

                                        if (miResponse != null)
                                        {
                                            var serviceInfo = miResponse.udo_MostRecentService;
                                            if (serviceInfo != null)
                                            {
                                                response.Rank = serviceInfo.RankName;
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {

                                    }

                                    #endregion
                                    #endregion
                                }
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(request.Edipi) && request.IsAttended == true)
                                {
                                    var ochestrationRequest = new MVIMessages.RetrieveWithOrchestrationRequest(request.ICN, request.UserFirstName, request.UserLastName, request.OrganizationName, request.UserId);

                                    //REM: Invoke VEIS WebApi
                                    //CSDev Updated to New Overloaded Method
                                    ochestrationRequest.MessageId = request.MessageId;
                                     var orchestrationResponse = WebApiUtility.SendReceive<MVIMessages.RetrieveOrSearchPersonResponse>(ochestrationRequest, WebApiType.VEIS);
                                    if (request.LogSoap || orchestrationResponse.ExceptionOccurred)
                                    {
                                        if (orchestrationResponse.SerializedSOAPRequest != null || orchestrationResponse.SerializedSOAPResponse != null)
                                        {
                                            var requestResponse = orchestrationResponse.SerializedSOAPRequest + orchestrationResponse.SerializedSOAPResponse;
                                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"RetrieveWithOrchestrationRequest Request/Response {requestResponse}", true);
                                        }
                                    }

                                    txnTimer.Stop();
                                    txnTimer.Restart();
                                    if (orchestrationResponse != null && orchestrationResponse.Message != null && orchestrationResponse.Message.Contains("Cannot open connection"))
                                        response.Message = "MVI connection failure";
                                    else
                                        response.Message = orchestrationResponse.Message;
                                    response.ExceptionOccured = orchestrationResponse.ExceptionOccured;
                                    if (orchestrationResponse != null && (orchestrationResponse.RawMviExceptionMessage == null || orchestrationResponse.RawMviExceptionMessage.Trim() == ""))
                                        response.RawMviExceptionMessage = orchestrationResponse.Message;
                                    else
                                        response.RawMviExceptionMessage = orchestrationResponse.RawMviExceptionMessage;

                                    if (!orchestrationResponse.ExceptionOccured)
                                    {
                                        MVIMessages.CorrespondingIdsResponse newRequest = new MVIMessages.CorrespondingIdsResponse();
                                        newRequest.MessageId = request.MessageId;
                                        newRequest.CorrespondingIdList = orchestrationResponse.Person[0].CorrespondingIdList;
                                        newRequest.OrganizationName = request.OrganizationName;
                                        newRequest.UserId = request.UserId;

                                        if (!String.IsNullOrEmpty(orchestrationResponse.Person[0].SocialSecurityNumber) && orchestrationResponse.Person[0].SocialSecurityNumber != "UNK")
                                            newRequest.SocialSecurityNumber = orchestrationResponse.Person[0].SocialSecurityNumber;
                                        if (!String.IsNullOrEmpty(orchestrationResponse.Person[0].EdiPi) && orchestrationResponse.Person[0].EdiPi != "UNK")
                                            newRequest.Edipi = orchestrationResponse.Person[0].EdiPi;

                                        if (request.AlreadySearchedCorp == true)
                                        {
                                            if (response.Edipi == null && (orchestrationResponse.Person[0].EdiPi != null && orchestrationResponse.Person[0].EdiPi.Trim() != ""))
                                                response.Edipi = orchestrationResponse.Person[0].EdiPi;
                                            break;
                                        }
                                        //TODO: CSDEV FIX
                                        response = (UDOSelectedPersonResponse)processor.Execute(newRequest, request);
                                        txnTimer.Stop();
                                        txnTimer.Restart();
                                    }
                                }
                                else
                                {
                                    MVIMessages.CorrespondingIdsResponse newRequest = new MVIMessages.CorrespondingIdsResponse();
                                    //where do we get CorrespondingIds of not from Orchestration?
                                    List<UnattendedSearchRequest> corIdList = new List<UnattendedSearchRequest>();
                                    foreach (var corId in request.CorrespondingIdList)
                                    {
                                        corIdList.Add(new UnattendedSearchRequest
                                        {
                                            AssigningAuthority = corId.AssigningAuthority,
                                            AssigningFacility = corId.AssigningFacility,
                                            AuthorityOid = corId.AuthorityOid,
                                            IdentifierType = corId.IdentifierType,
                                            OrganizationName = corId.OrganizationName,
                                            PatientIdentifier = corId.PatientIdentifier,
                                            RawValueFromMvi = corId.RawValueFromMvi,
                                            UserFirstName = corId.UserFirstName,
                                            UserLastName = corId.UserLastName,
                                            UserId = corId.UserId
                                        });
                                    }
                                    newRequest.CorrespondingIdList = corIdList.ToArray();
                                    newRequest.OrganizationName = request.OrganizationName;
                                    newRequest.UserId = request.UserId;
                                    newRequest.SocialSecurityNumber = request.SSIdString;
                                    newRequest.Edipi = request.Edipi;

                                    response = (UDOSelectedPersonResponse)processor.Execute(newRequest, request);
                                    txnTimer.Stop();
                                    txnTimer.Restart();
                                }
                            }

                            break;
                        #endregion

                        default:
                            //for UDO, in every other type of response we would have SSN, or EDIPI or some other identifier we have saved in CRM -
                            //which is enough to go to CRM to select the contact
                            //BLUF there is no need to go anywhere to get those identifiers.

                            processor = new FindinCRMProcessor();
                            response = (UDOSelectedPersonResponse)processor.Execute(request);
                            txnTimer.Stop();
                            txnTimer.Restart();
                            break;
                    }
                }
                EntireTimer.Stop();

                return response;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "#Error in UDOSelectedPersonProcessor.Execute ", ex);
                response.ExceptionOccured = true;
                response.Message = ex.Message;
                return response;
            }
        }
    }
}
