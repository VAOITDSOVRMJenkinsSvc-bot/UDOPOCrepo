using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using VIMT.AppealService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Appeals.Messages;
using VRM.Integration.UDO.Common;

namespace VRM.Integration.UDO.Appeals.Processors
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
            //var request = message as createUDOAppealsRequest;
            UDOcreateUDOAppealsResponse response = new UDOcreateUDOAppealsResponse();
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;
            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

          
            OrganizationServiceProxy OrgServiceProxy;

            #region connect to CRM
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateUDOAppealsProcessor Processor, Connection Error", connectException.Message); 
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                #region Create EC Request

                // prefix = fndaplsfindAppealsRequest();
                DateTime DOB = new DateTime();
                if (!string.IsNullOrEmpty(request.DateOfBirth))
                {
                    DOB = DateTime.Parse(request.DateOfBirth);
                }
                var findAppealsRequest = new VIMTfndaplsfindAppealsRequest()
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    LegacyServiceHeaderInfo = new VIMT.AppealService.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    findappealsrequestInfo = new VIMTfndaplsfindappealsrequest
                    {
                        mcs_DateOfBirth = DOB,
                        mcs_SSN = requestSSN.ToUnsecureString(),
                        mcs_State = request.State,
                        searchfieldcityInfo = new VIMTfndaplssearchfieldcity { mcs_Partialflag = false, mcs_Value = request.City },
                        searchfieldfirstnameInfo = new VIMTfndaplssearchfieldfirstname { mcs_Partialflag = false, mcs_Value = request.FirstName },
                        searchfieldlastnameInfo = new VIMTfndaplssearchfieldlastname { mcs_Partialflag = false, mcs_Value = request.LastName }
                    }
                };
                #endregion
                //TODO(NP): Update the VIMT call to VEIS
                var findAppealsResponse = findAppealsRequest.SendReceive<VIMTfndaplsfindAppealsResponse>(MessageProcessType.Remote);
                response.ExceptionOccured = findAppealsResponse.ExceptionOccured;
                if (findAppealsResponse.VIMTfndaplsAppealIdentifierInfo != null)
                {
                    if (findAppealsResponse.VIMTfndaplsAppealIdentifierInfo.Count() == 0)
                    {
                        if (!response.ExceptionOccured)
                        {
                            if (requestSSN.StartsWith("0"))
                            {
                                #region firststrip
                                requestSSN = requestSSN.SecureSubstring(1);
                                findAppealsRequest = new VIMTfndaplsfindAppealsRequest()
                                {
                                    LogTiming = request.LogTiming,
                                    LogSoap = request.LogSoap,
                                    Debug = request.Debug,
                                    RelatedParentEntityName = request.RelatedParentEntityName,
                                    RelatedParentFieldName = request.RelatedParentFieldName,
                                    RelatedParentId = request.RelatedParentId,
                                    UserId = request.UserId,
                                    OrganizationName = request.OrganizationName,
                                    LegacyServiceHeaderInfo = new VIMT.AppealService.Messages.HeaderInfo
                                    {
                                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                    },
                                    findappealsrequestInfo = new VIMTfndaplsfindappealsrequest
                                    {
                                        mcs_DateOfBirth = DOB,
                                        mcs_SSN = requestSSN.ToUnsecureString(),
                                        mcs_State = request.State,
                                        searchfieldcityInfo = new VIMTfndaplssearchfieldcity { mcs_Partialflag = false, mcs_Value = request.City },
                                        searchfieldfirstnameInfo = new VIMTfndaplssearchfieldfirstname { mcs_Partialflag = false, mcs_Value = request.FirstName },
                                        searchfieldlastnameInfo = new VIMTfndaplssearchfieldlastname { mcs_Partialflag = false, mcs_Value = request.LastName }
                                    }
                                };

                                //TODO(NP): Update the VIMT call to VEIS
                                findAppealsResponse = findAppealsRequest.SendReceive<VIMTfndaplsfindAppealsResponse>(MessageProcessType.Local);
                                response.ExceptionOccured = findAppealsResponse.ExceptionOccured;
                                #endregion
                                if (findAppealsResponse.VIMTfndaplsAppealIdentifierInfo != null)
                                {
                                    if (findAppealsResponse.VIMTfndaplsAppealIdentifierInfo.Count() == 0)
                                    {
                                        if (!response.ExceptionOccured)
                                        {
                                            if (requestSSN.StartsWith("0"))
                                            {
                                                #region secondstrip
                                                //request.SSN = request.SSN.Substring(1);                                
                                                requestSSN = requestSSN.SecureSubstring(1);

                                                findAppealsRequest = new VIMTfndaplsfindAppealsRequest()
                                                {
                                                    LogTiming = request.LogTiming,
                                                    LogSoap = request.LogSoap,
                                                    Debug = request.Debug,
                                                    RelatedParentEntityName = request.RelatedParentEntityName,
                                                    RelatedParentFieldName = request.RelatedParentFieldName,
                                                    RelatedParentId = request.RelatedParentId,
                                                    UserId = request.UserId,
                                                    OrganizationName = request.OrganizationName,
                                                    LegacyServiceHeaderInfo = new VIMT.AppealService.Messages.HeaderInfo
                                                    {
                                                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                                    },
                                                    findappealsrequestInfo = new VIMTfndaplsfindappealsrequest
                                                    {
                                                        mcs_DateOfBirth = DOB,
                                                        mcs_SSN = requestSSN.ToUnsecureString(),
                                                        mcs_State = request.State,
                                                        searchfieldcityInfo = new VIMTfndaplssearchfieldcity { mcs_Partialflag = false, mcs_Value = request.City },
                                                        searchfieldfirstnameInfo = new VIMTfndaplssearchfieldfirstname { mcs_Partialflag = false, mcs_Value = request.FirstName },
                                                        searchfieldlastnameInfo = new VIMTfndaplssearchfieldlastname { mcs_Partialflag = false, mcs_Value = request.LastName }
                                                    }
                                                };

                                                //TODO(NP): Update the VIMT call to VEIS
                                                findAppealsResponse = findAppealsRequest.SendReceive<VIMTfndaplsfindAppealsResponse>(MessageProcessType.Local);
                                                response.ExceptionOccured = findAppealsResponse.ExceptionOccured;
                                                #endregion
                                                if (findAppealsResponse.VIMTfndaplsAppealIdentifierInfo != null)
                                                {
                                                    if (findAppealsResponse.VIMTfndaplsAppealIdentifierInfo.Count() == 0)
                                                    {
                                                        if (!response.ExceptionOccured)
                                                        {
                                                            if (requestSSN.StartsWith("0"))
                                                            {
                                                                #region laststrip
                                                                requestSSN = requestSSN.SecureSubstring(1);
                                                                //request.SSN = request.SSN.Substring(1);
                                                                findAppealsRequest = new VIMTfndaplsfindAppealsRequest()
                                                                {
                                                                    LogTiming = request.LogTiming,
                                                                    LogSoap = request.LogSoap,
                                                                    Debug = request.Debug,
                                                                    RelatedParentEntityName = request.RelatedParentEntityName,
                                                                    RelatedParentFieldName = request.RelatedParentFieldName,
                                                                    RelatedParentId = request.RelatedParentId,
                                                                    UserId = request.UserId,
                                                                    OrganizationName = request.OrganizationName,
                                                                    LegacyServiceHeaderInfo = new VIMT.AppealService.Messages.HeaderInfo
                                                                    {
                                                                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                                                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                                                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                                                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                                                    },
                                                                    findappealsrequestInfo = new VIMTfndaplsfindappealsrequest
                                                                    {
                                                                        mcs_DateOfBirth = DOB,
                                                                        mcs_SSN = requestSSN.ToUnsecureString(),
                                                                        mcs_State = request.State,
                                                                        searchfieldcityInfo = new VIMTfndaplssearchfieldcity { mcs_Partialflag = false, mcs_Value = request.City },
                                                                        searchfieldfirstnameInfo = new VIMTfndaplssearchfieldfirstname { mcs_Partialflag = false, mcs_Value = request.FirstName },
                                                                        searchfieldlastnameInfo = new VIMTfndaplssearchfieldlastname { mcs_Partialflag = false, mcs_Value = request.LastName }
                                                                    }
                                                                };

                                                                //TODO(NP): Update the VIMT call to VEIS
                                                                findAppealsResponse = findAppealsRequest.SendReceive<VIMTfndaplsfindAppealsResponse>(MessageProcessType.Local);
                                                                response.ExceptionOccured = findAppealsResponse.ExceptionOccured;
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

                if (findAppealsResponse.VIMTfndaplsAppealIdentifierInfo != null)
                {
                    var AppealIdentifier = findAppealsResponse.VIMTfndaplsAppealIdentifierInfo;

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
                        //if (!string.IsNullOrEmpty(AppealIdentifierItem.mcs_AppealStatusDescription))
                        //{
                        //    thisNewEntity["udo_statusdescription"] = AppealIdentifierItem.mcs_AppealStatusDescription;
                        //}
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
                        if (AppealIdentifierItem.mcs_NoticeOfDisagreementReceivedDate != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_noticeofdisagreementrec"] = AppealIdentifierItem.mcs_NoticeOfDisagreementReceivedDate;
                        }
                        if (AppealIdentifierItem.mcs_DecisionDate != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_decisiondate"] = AppealIdentifierItem.mcs_DecisionDate;
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
                        // TODO(NP): var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, request.UserId, request.Debug);
                        var result = new ExecuteMultipleResponse();
                        if (_debug)
                        {
                            // LogBuffer += result.LogDetail;
                            LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                        }

                        if (result.IsFaulted)
                        {
                            //LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                            // response.ExceptionMessage = result.FriendlyDetail;
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
                        // TODO(NP):  OrgServiceProxy.Update(TruncateHelper.TruncateFields(idProof, request.OrganizationName, request.UserId, request.LogTiming));
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
                    // TODO(NP): OrgServiceProxy.Update(TruncateHelper.TruncateFields(vetSnapShot, request.OrganizationName, request.UserId, request.LogTiming));
                    string logInfo = string.Format("Snapshot Updated with Appeals Data");
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Appeal Data", logInfo);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateUDOAppealsProcessor Processor, Progess:" + progressString, ExecutionException);
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
                        // TODO(NP): OrgServiceProxy.Update(TruncateHelper.TruncateFields(idProof, request.OrganizationName, request.UserId, request.LogTiming));
                    }
                }
                return response;
            }
        }

    }
}
