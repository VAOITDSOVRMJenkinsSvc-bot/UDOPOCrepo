using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;
using System.Security;
using UDO.LOB.Appeals.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Core.Messages;
using VEIS.Messages.AppealService;

namespace UDO.LOB.Appeals.Processors
{
    class UDOcreateUDOAppealsProcessor
    {
        private bool _debug { get; set; }

        private const string method = "UDOcreateUDOAppealsProcessor";

        private string LogBuffer { get; set; }

        private SecureString requestSSN;

        public IMessageBase Execute(UDOcreateUDOAppealsRequest request)
        {
            requestSSN = request.SSN.ToSecureString();
            UDOcreateUDOAppealsResponse response = new UDOcreateUDOAppealsResponse();
            response.MessageId = request.MessageId;
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA",
                    IdProof = request.idProofId
                };
            }
            TraceLogger tLogger = new TraceLogger(method, request);
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            #region connect to CRM
            CrmServiceClient OrgServiceProxy = null;

            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateUDOAppealsProcessor Processor, Connection Error", connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion


            try
            {
                #region Create EC Request

                DateTime DOB = new DateTime();
                if (!string.IsNullOrEmpty(request.DateOfBirth))
                {
                    DOB = DateTime.Parse(request.DateOfBirth);
                }
                var findAppealsRequest = new VEISfndaplsfindAppealsRequest()
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    VEISfndaplsReqfindAppealCriteriaInfo = new VEISfndaplsReqfindAppealCriteria
                    {
                        mcs_DateOfBirth = DOB.ToString(),
                        mcs_SSN = requestSSN.ToUnsecureString(),
                        mcs_State = request.State,
                        VIMTfndaplsReqCityInfo = new VIMTfndaplsReqCity { mcs_Partialflag = false, mcs_Value = request.City },
                        VIMTfndaplsReqFirstNameInfo = new VIMTfndaplsReqFirstName { mcs_Partialflag = false, mcs_Value = request.FirstName },
                        VIMTfndaplsReqLastNameInfo = new VIMTfndaplsReqLastName { mcs_Partialflag = false, mcs_Value = request.LastName }
                    }
                };
                #endregion
                // REM: Invoke VEIS Endpoint
                var findAppealsResponse = WebApiUtility.SendReceive<VEISfndaplsfindAppealsResponse>(findAppealsRequest, WebApiType.VEIS);
                if (request.LogSoap || findAppealsResponse.ExceptionOccurred)
                {
                    if (findAppealsResponse.SerializedSOAPRequest != null || findAppealsResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findAppealsResponse.SerializedSOAPRequest + findAppealsResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfndaplsfindAppealsRequest Request/Response {requestResponse}", true);
                    }
                }

                tLogger.LogEvent($"Web Service Call VEISfndaplsfindAppealsResponse (First). Exception Occured: {findAppealsResponse.ExceptionOccurred.ToString()} ", "002");
                response.ExceptionOccured = findAppealsResponse.ExceptionOccurred;

                if (findAppealsResponse.VEISfndaplsAppealIdentifierInfo != null)
                {
                    if (findAppealsResponse.VEISfndaplsAppealIdentifierInfo.Count() == 0)
                    {
                        if (!response.ExceptionOccured)
                        {
                            if (requestSSN.StartsWith("0"))
                            {
                                #region firststrip
                                requestSSN = requestSSN.SecureSubstring(1);
                                findAppealsRequest = new VEISfndaplsfindAppealsRequest()
                                {
                                    MessageId = request.MessageId,
                                    LogTiming = request.LogTiming,
                                    LogSoap = request.LogSoap,
                                    Debug = request.Debug,
                                    RelatedParentEntityName = request.RelatedParentEntityName,
                                    RelatedParentFieldName = request.RelatedParentFieldName,
                                    RelatedParentId = request.RelatedParentId,
                                    UserId = request.UserId,
                                    OrganizationName = request.OrganizationName,
                                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                                    {
                                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                    },
                                    VEISfndaplsReqfindAppealCriteriaInfo = new VEISfndaplsReqfindAppealCriteria
                                    {
                                        mcs_DateOfBirth = DOB.ToString(),
                                        mcs_SSN = requestSSN.ToUnsecureString(),
                                        mcs_State = request.State,
                                        VIMTfndaplsReqCityInfo = new VIMTfndaplsReqCity { mcs_Partialflag = false, mcs_Value = request.City },
                                        VIMTfndaplsReqFirstNameInfo = new VIMTfndaplsReqFirstName { mcs_Partialflag = false, mcs_Value = request.FirstName },
                                        VIMTfndaplsReqLastNameInfo = new VIMTfndaplsReqLastName { mcs_Partialflag = false, mcs_Value = request.LastName }
                                    }
                                };

                                // REM: Invoke VEIS endpoint
                                findAppealsResponse = WebApiUtility.SendReceive<VEISfndaplsfindAppealsResponse>(findAppealsRequest, WebApiType.VEIS);
                                if (request.LogSoap || findAppealsResponse.ExceptionOccurred)
                                {
                                    if (findAppealsResponse.SerializedSOAPRequest != null || findAppealsResponse.SerializedSOAPResponse != null)
                                    {
                                        var requestResponse = findAppealsResponse.SerializedSOAPRequest + findAppealsResponse.SerializedSOAPResponse;
                                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfndaplsfindAppealsRequest Request/Response {requestResponse}", true);
                                    }
                                }

                                tLogger.LogEvent($"Web Service Call VEISfndaplsfindAppealsResponse (Second). Exception Occured: {findAppealsResponse.ExceptionOccurred.ToString()} ", "003");
                                response.ExceptionOccured = findAppealsResponse.ExceptionOccurred;
                                #endregion
                                if (findAppealsResponse.VEISfndaplsAppealIdentifierInfo != null)
                                {
                                    if (findAppealsResponse.VEISfndaplsAppealIdentifierInfo.Count() == 0)
                                    {
                                        if (!response.ExceptionOccured)
                                        {
                                            if (requestSSN.StartsWith("0"))
                                            {
                                                #region secondstrip                               
                                                requestSSN = requestSSN.SecureSubstring(1);

                                                findAppealsRequest = new VEISfndaplsfindAppealsRequest()
                                                {
                                                    MessageId = request.MessageId,
                                                    LogTiming = request.LogTiming,
                                                    LogSoap = request.LogSoap,
                                                    Debug = request.Debug,
                                                    RelatedParentEntityName = request.RelatedParentEntityName,
                                                    RelatedParentFieldName = request.RelatedParentFieldName,
                                                    RelatedParentId = request.RelatedParentId,
                                                    UserId = request.UserId,
                                                    OrganizationName = request.OrganizationName,
                                                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                                                    {
                                                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                                    },
                                                    VEISfndaplsReqfindAppealCriteriaInfo = new VEISfndaplsReqfindAppealCriteria
                                                    {
                                                        mcs_DateOfBirth = DOB.ToString(),
                                                        mcs_SSN = requestSSN.ToUnsecureString(),
                                                        mcs_State = request.State,
                                                        VIMTfndaplsReqCityInfo = new VIMTfndaplsReqCity { mcs_Partialflag = false, mcs_Value = request.City },
                                                        VIMTfndaplsReqFirstNameInfo = new VIMTfndaplsReqFirstName { mcs_Partialflag = false, mcs_Value = request.FirstName },
                                                        VIMTfndaplsReqLastNameInfo = new VIMTfndaplsReqLastName { mcs_Partialflag = false, mcs_Value = request.LastName }
                                                    }
                                                };

                                                // REM: Invoke VEIS Endpoint
                                                findAppealsResponse = WebApiUtility.SendReceive<VEISfndaplsfindAppealsResponse>(findAppealsRequest, WebApiType.VEIS);
                                                if (request.LogSoap || findAppealsResponse.ExceptionOccurred)
                                                {
                                                    if (findAppealsResponse.SerializedSOAPRequest != null || findAppealsResponse.SerializedSOAPResponse != null)
                                                    {
                                                        var requestResponse = findAppealsResponse.SerializedSOAPRequest + findAppealsResponse.SerializedSOAPResponse;
                                                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfndaplsfindAppealsRequest Request/Response {requestResponse}", true);
                                                    }
                                                }

                                                tLogger.LogEvent($"Web Service Call VEISfndaplsfindAppealsResponse (Third). Exception Occured: {findAppealsResponse.ExceptionOccurred.ToString()} ", "004");
                                                response.ExceptionOccured = findAppealsResponse.ExceptionOccurred;
                                                #endregion
                                                if (findAppealsResponse.VEISfndaplsAppealIdentifierInfo != null)
                                                {
                                                    if (findAppealsResponse.VEISfndaplsAppealIdentifierInfo.Count() == 0)
                                                    {
                                                        if (!response.ExceptionOccured)
                                                        {
                                                            if (requestSSN.StartsWith("0"))
                                                            {
                                                                #region laststrip
                                                                requestSSN = requestSSN.SecureSubstring(1);
                                                                findAppealsRequest = new VEISfndaplsfindAppealsRequest()
                                                                {
                                                                    MessageId = request.MessageId,
                                                                    LogTiming = request.LogTiming,
                                                                    LogSoap = request.LogSoap,
                                                                    Debug = request.Debug,
                                                                    RelatedParentEntityName = request.RelatedParentEntityName,
                                                                    RelatedParentFieldName = request.RelatedParentFieldName,
                                                                    RelatedParentId = request.RelatedParentId,
                                                                    UserId = request.UserId,
                                                                    OrganizationName = request.OrganizationName,
                                                                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                                                                    {
                                                                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                                                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                                                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                                                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                                                    },
                                                                    VEISfndaplsReqfindAppealCriteriaInfo = new VEISfndaplsReqfindAppealCriteria
                                                                    {
                                                                        mcs_DateOfBirth = DOB.ToString(),
                                                                        mcs_SSN = requestSSN.ToUnsecureString(),
                                                                        mcs_State = request.State,
                                                                        VIMTfndaplsReqCityInfo = new VIMTfndaplsReqCity { mcs_Partialflag = false, mcs_Value = request.City },
                                                                        VIMTfndaplsReqFirstNameInfo = new VIMTfndaplsReqFirstName { mcs_Partialflag = false, mcs_Value = request.FirstName },
                                                                        VIMTfndaplsReqLastNameInfo = new VIMTfndaplsReqLastName { mcs_Partialflag = false, mcs_Value = request.LastName }
                                                                    }
                                                                };

                                                                // REM: Invoke VEIS endpoint
                                                                findAppealsResponse = WebApiUtility.SendReceive<VEISfndaplsfindAppealsResponse>(findAppealsRequest, WebApiType.VEIS);
                                                                if (request.LogSoap || findAppealsResponse.ExceptionOccurred)
                                                                {
                                                                    if (findAppealsResponse.SerializedSOAPRequest != null || findAppealsResponse.SerializedSOAPResponse != null)
                                                                    {
                                                                        var requestResponse = findAppealsResponse.SerializedSOAPRequest + findAppealsResponse.SerializedSOAPResponse;
                                                                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfndaplsfindAppealsRequest Request/Response {requestResponse}", true);
                                                                    }
                                                                }

                                                                tLogger.LogEvent($"Web Service Call VEISfndaplsfindAppealsResponse (Fourth). Exception Occured: {findAppealsResponse.ExceptionOccurred.ToString()} ", "005");
                                                                response.ExceptionOccured = findAppealsResponse.ExceptionOccurred;
                                                                #endregion
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #region Map Response to Fields
                var appealsCnt = 0;
                var pendingAppealsCnt = 0;
                var requestCollection = new OrganizationRequestCollection();

                if (findAppealsResponse.VEISfndaplsAppealIdentifierInfo != null)
                {
                    var AppealIdentifier = findAppealsResponse.VEISfndaplsAppealIdentifierInfo;

                    foreach (var AppealIdentifierItem in AppealIdentifier)
                    {
                        var thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_appeal";
                        thisNewEntity["udo_name"] = "Appeal Summary";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        appealsCnt += 1;
                        if (!string.IsNullOrEmpty(AppealIdentifierItem.mcs_AppealStatusCode))
                        {
                            thisNewEntity["udo_statuscode"] = AppealIdentifierItem.mcs_AppealStatusCode;
                            switch (AppealIdentifierItem.mcs_AppealStatusCode)
                            {
                                case "PEN":
                                    pendingAppealsCnt += 1;
                                    thisNewEntity["udo_statusdescription"] = AppealIdentifierItem.mcs_AppealStatusDescription;
                                    break;
                                case "ACT":
                                    pendingAppealsCnt += 1;
                                    thisNewEntity["udo_statusdescription"] = "Active (Case at BVA)";
                                    break;
                                case "ADV":
                                    pendingAppealsCnt += 1;
                                    thisNewEntity["udo_statusdescription"] = "Advanced (NOD Appeal Filed and/or on Docket—Case in RO)";
                                    break;
                                case "REM":
                                    pendingAppealsCnt += 1;
                                    thisNewEntity["udo_statusdescription"] = "Remand (case has been Remanded to VBA)";
                                    break;
                                case "CAV":
                                    pendingAppealsCnt += 1;
                                    thisNewEntity["udo_statusdescription"] = "CAVC (U.S. Court of Appeals for Veterans Claims Action pending -case in transit to BVA)";
                                    break;
                                case "HIS":
                                    thisNewEntity["udo_statusdescription"] = "History (BVA action is complete and appeal is closed)";
                                    break;
                                default:
                                    break;

                            }
                        }
                        if (!string.IsNullOrEmpty(AppealIdentifierItem.mcs_RegionOfficeDescription))
                        {
                            thisNewEntity["udo_regionofficedescription"] = AppealIdentifierItem.mcs_RegionOfficeDescription;
                        }
                        if (!string.IsNullOrEmpty(AppealIdentifierItem.mcs_RegionOfficeCode))
                        {
                            thisNewEntity["udo_regionofficecode"] = AppealIdentifierItem.mcs_RegionOfficeCode;
                        }
                        if (!string.IsNullOrEmpty(AppealIdentifierItem.mcs_NoticeOfDisagreementReceivedDate))
                        {
                            DateTime newDateTime;
                            if (DateTime.TryParse(AppealIdentifierItem.mcs_NoticeOfDisagreementReceivedDate, out newDateTime))
                            {
                                thisNewEntity["udo_noticeofdisagreementrec"] = newDateTime;
                            }
                        }
                        if (!string.IsNullOrEmpty(AppealIdentifierItem.mcs_DecisionDate))
                        {
                            DateTime newDateTime;
                            if (DateTime.TryParse(AppealIdentifierItem.mcs_DecisionDate, out newDateTime))
                            {
                                thisNewEntity["udo_decisiondate"] = newDateTime;
                            }
                        }
                        if (!string.IsNullOrEmpty(AppealIdentifierItem.mcs_ActionTypeDescription))
                        {
                            thisNewEntity["udo_actiontypedescription"] = AppealIdentifierItem.mcs_ActionTypeDescription;
                        }
                        if (!string.IsNullOrEmpty(AppealIdentifierItem.mcs_ActionTypeCode))
                        {
                            thisNewEntity["udo_actiontypecode"] = AppealIdentifierItem.mcs_ActionTypeCode;
                        }
                        var applName = AppealIdentifierItem.mcs_LastName;
                        if (!string.IsNullOrEmpty(AppealIdentifierItem.mcs_MiddleInitial))
                        {
                            applName += ", " + AppealIdentifierItem.mcs_MiddleInitial;
                        }
                        if (!string.IsNullOrEmpty(AppealIdentifierItem.mcs_FirstName))
                        {
                            applName += ", " + AppealIdentifierItem.mcs_FirstName;
                        }

                        thisNewEntity["udo_name"] = applName;

                        if (!string.IsNullOrEmpty(AppealIdentifierItem.mcs_AppealKey))
                        {
                            thisNewEntity["udo_appealkey"] = AppealIdentifierItem.mcs_AppealKey;
                        }
                        if (request.UDOcreateUDOAppealsRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateUDOAppealsRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createAppeals = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createAppeals);
                    }
                    #endregion
                }

                #region Create records
                if (!request.GetSnapShotData)
                {
                    if (appealsCnt > 0)
                    {
                        var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);
                        if (_debug)
                        {
                            LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                        }

                        if (result.IsFaulted)
                        {
                            response.ExceptionOccured = true;
                            return response;
                        }
                    }

                    string logInfo = string.Format("Appeal Records Created: {0}", appealsCnt);
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Appeal Records Created", logInfo);
                    #endregion
                    if (request.idProofId != Guid.Empty)
                    {
                        var idProof = new Entity();
                        idProof.Id = request.idProofId;
                        idProof.LogicalName = "udo_idproof";
                        idProof["udo_appealintegration"] = new OptionSetValue(752280002);
                        OrgServiceProxy.Update(idProof);
                    }
                    else
                    {
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateUDOAppealsProcessor Processor", "no idProofId found");
                    }
                }
                if (request.vetsnapshotId != Guid.Empty)
                {
                    Entity vetSnapShot = new Entity();
                    vetSnapShot.LogicalName = "udo_veteransnapshot";
                    vetSnapShot.Id = request.vetsnapshotId;
                    vetSnapShot["udo_pendingappeals"] = pendingAppealsCnt.ToString() + " pending appeal(s)";
                    vetSnapShot["udo_appealscompleted"] = new OptionSetValue(752280002);
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, vetSnapShot, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    string logInfo = string.Format("Snapshot Updated with Appeals Data");
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Appeal Data", logInfo);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateUDOAppealsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Appeals Data";
                response.ExceptionOccured = true;
                if (!request.GetSnapShotData)
                {
                    if (request.idProofId != Guid.Empty)
                    {
                        var idProof = new Entity();
                        idProof.Id = request.idProofId;
                        idProof.LogicalName = "udo_idproof";
                        idProof["udo_appealintegration"] = new OptionSetValue(752280003);
                        OrgServiceProxy.Update(idProof);
                    }
                }
                return response;
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }
        }
    }
}