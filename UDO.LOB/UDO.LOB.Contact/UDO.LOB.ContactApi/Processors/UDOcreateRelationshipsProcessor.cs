using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;
using UDO.LOB.Contact.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Core.Messages;
using VEIS.Messages.ClaimantService;

/// <summary>
/// UDOcreateRelationships,createRelationships method, Processor.
/// </summary>
namespace UDO.LOB.Contact.Processors
{
    class UDOcreateRelationshipsProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOcreateRelationshipsProcessor";
        
        public IMessageBase Execute(UDOcreateRelationshipsRequest request)
        {
            UDOcreateRelationshipsResponse response = new UDOcreateRelationshipsResponse { MessageId = request.MessageId };
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

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

            #region connect to CRM
            CrmServiceClient OrgServiceProxy = null;
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            var requestCollection = new OrganizationRequestCollection();

            progressString = "After CRM Connection, beginning new request.";

            try
            {
                var findAllRelationshipsRequest = new VEISfallrelfindAllRelationshipsRequest();
                findAllRelationshipsRequest.LogTiming = request.LogTiming;
                findAllRelationshipsRequest.LogSoap = request.LogSoap;
                findAllRelationshipsRequest.Debug = request.Debug;
                findAllRelationshipsRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findAllRelationshipsRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findAllRelationshipsRequest.RelatedParentId = request.RelatedParentId;
                findAllRelationshipsRequest.UserId = request.UserId;
                findAllRelationshipsRequest.OrganizationName = request.OrganizationName;

                findAllRelationshipsRequest.mcs_ptcpntid = request.ptcpntId;
                if (request.LegacyServiceHeaderInfo != null)
                {
                    findAllRelationshipsRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }
                // REM: Invoke VEIS Endpoint
                var findAllRelationshipsResponse = WebApiUtility.SendReceive<VEISfallrelfindAllRelationshipsResponse>(findAllRelationshipsRequest, WebApiType.VEIS);
                if (request.LogSoap || findAllRelationshipsResponse.ExceptionOccurred)
                {
                    if (findAllRelationshipsResponse.SerializedSOAPRequest != null || findAllRelationshipsResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findAllRelationshipsResponse.SerializedSOAPRequest + findAllRelationshipsResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfallrelfindAllRelationshipsRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VEIS EC Call";
                requestCollection = new OrganizationRequestCollection();

                response.ExceptionMessage = findAllRelationshipsResponse.ExceptionMessage;
                response.ExceptionOccured = findAllRelationshipsResponse.ExceptionOccurred;
                if (findAllRelationshipsResponse.VEISfallrelreturnInfo != null)
                {
                    var shrinq6Person = findAllRelationshipsResponse.VEISfallrelreturnInfo.VEISfallreldependentsInfo;
                    System.Collections.Generic.List<UDOcreateRelationshipsMultipleResponse> UDOcreateRelationshipsArray = new System.Collections.Generic.List<UDOcreateRelationshipsMultipleResponse>();
                    if (shrinq6Person != null)
                    {
                        foreach (var shrinq6PersonItem in shrinq6Person)
                        {
                            var responseIds = new UDOcreateRelationshipsMultipleResponse();
                            //instantiate the new Entity
                            Entity thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_relationships";
                            thisNewEntity["udo_name"] = "Relationship Summary";
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            else
                            {
                                LogHelper.LogError(request.OrganizationName, request.UserId, "Create Relationships", "No Owner");
                            }
                            if (shrinq6PersonItem.mcs_terminateReason != string.Empty)
                            {
                                thisNewEntity["udo_terminatereason"] = shrinq6PersonItem.mcs_terminateReason;
                            }
                            if (shrinq6PersonItem.mcs_ssnVerifiedInd != string.Empty)
                            {
                                thisNewEntity["udo_ssnverifiedstatus"] = shrinq6PersonItem.mcs_ssnVerifiedInd;
                            }
                            if (shrinq6PersonItem.mcs_ssn != string.Empty)
                            {
                                thisNewEntity["udo_ssn"] = shrinq6PersonItem.mcs_ssn;
                            }
                            if (!string.IsNullOrEmpty(shrinq6PersonItem.mcs_awardInd))
                            {
                                thisNewEntity["udo_awardind"] = shrinq6PersonItem.mcs_awardInd;
                            }
                            if (!string.IsNullOrEmpty(shrinq6PersonItem.mcs_relationshipEndDate) && DateTime.TryParse(dateStringFormat(shrinq6PersonItem.mcs_relationshipEndDate), out DateTime newDateTime))
                            {
                                thisNewEntity["udo_relationshipenddate"] = newDateTime;
                            }
                            if (!string.IsNullOrEmpty(shrinq6PersonItem.mcs_relationshipBeginDate) && DateTime.TryParse(dateStringFormat(shrinq6PersonItem.mcs_relationshipBeginDate), out newDateTime))
                            {
                                thisNewEntity["udo_relationshipbegindate"] = newDateTime;
                            }
                            if (shrinq6PersonItem.mcs_relationshipType != string.Empty)
                            {
                                thisNewEntity["udo_relationship"] = shrinq6PersonItem.mcs_relationshipType;
                            }
                            if (shrinq6PersonItem.mcs_ptcpntId != string.Empty)
                            {
                                thisNewEntity["udo_ptcpntid"] = shrinq6PersonItem.mcs_ptcpntId;
                            }
                            if (shrinq6PersonItem.mcs_proofOfDependecyInd != string.Empty)
                            {
                                thisNewEntity["udo_proofofdependency"] = shrinq6PersonItem.mcs_proofOfDependecyInd;
                            }
                            if (shrinq6PersonItem.mcs_poa != string.Empty)
                            {
                                thisNewEntity["udo_poa"] = shrinq6PersonItem.mcs_poa;
                            }
                            if (shrinq6PersonItem.mcs_gender != string.Empty)
                            {
                                thisNewEntity["udo_gender"] = shrinq6PersonItem.mcs_gender;
                            }
                            if (shrinq6PersonItem.mcs_fileNumber != string.Empty)
                            {
                                thisNewEntity["udo_filenumber"] = shrinq6PersonItem.mcs_fileNumber;
                            }
                            if (shrinq6PersonItem.mcs_fiduciary != string.Empty)
                            {
                                thisNewEntity["udo_fiduciary"] = shrinq6PersonItem.mcs_fiduciary;
                            }
                            if (shrinq6PersonItem.mcs_emailAddress != string.Empty)
                            {
                                thisNewEntity["udo_email"] = shrinq6PersonItem.mcs_emailAddress;
                            }
                            if (!string.IsNullOrEmpty(shrinq6PersonItem.mcs_dateOfDeath) && DateTime.TryParse(dateStringFormat(shrinq6PersonItem.mcs_dateOfDeath), out newDateTime))
                            {
                                thisNewEntity["udo_dod"] = newDateTime;
                            }
                            if (!string.IsNullOrEmpty(shrinq6PersonItem.mcs_dateOfBirth) && DateTime.TryParse(dateStringFormat(shrinq6PersonItem.mcs_dateOfBirth), out newDateTime))
                            {
                                thisNewEntity["udo_dob"] = newDateTime;
                            }
                            if (!string.IsNullOrEmpty(shrinq6PersonItem.mcs_dependentTerminateDate) && DateTime.TryParse(dateStringFormat(shrinq6PersonItem.mcs_dependentTerminateDate), out newDateTime))
                            {
                                thisNewEntity["udo_dependentterminatedate"] = newDateTime;
                            }
                            if (shrinq6PersonItem.mcs_dependentReason != string.Empty)
                            {
                                thisNewEntity["udo_dependantreason"] = shrinq6PersonItem.mcs_dependentReason;
                            }
                            if (shrinq6PersonItem.mcs_awardType != string.Empty)
                            {
                                thisNewEntity["udo_awardtype"] = shrinq6PersonItem.mcs_awardType;
                            }
                            if (!string.IsNullOrEmpty(shrinq6PersonItem.mcs_awardEndDate) && DateTime.TryParse(dateStringFormat(shrinq6PersonItem.mcs_awardEndDate), out  newDateTime))
                            {
                                thisNewEntity["udo_awardenddate"] = newDateTime;
                            }
                            if (!string.IsNullOrEmpty(shrinq6PersonItem.mcs_awardBeginDate) && DateTime.TryParse(dateStringFormat(shrinq6PersonItem.mcs_awardBeginDate), out newDateTime))
                            {
                                thisNewEntity["udo_awardbegindate"] = newDateTime;
                            }

                            var fullname = shrinq6PersonItem.mcs_firstName + " " + shrinq6PersonItem.mcs_middleName + " " + shrinq6PersonItem.mcs_lastName;
                            thisNewEntity["udo_fullname"] = fullname;

                            if (request.UDOcreateRelationshipsRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedItem in request.UDOcreateRelationshipsRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }

                            CreateRequest createData = new CreateRequest
                            {
                                Target = thisNewEntity
                            };
                            requestCollection.Add(createData);

                        }

                        if (requestCollection.Count() > 0)
                        {
                            var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);

                            if (_debug)
                            {
                                LogBuffer += result.LogDetail;
                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, LogBuffer, request.Debug);
                            }

                            if (result.IsFaulted)
                            {
                                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, result.ErrorDetail);
                                response.ExceptionMessage = result.FriendlyDetail;
                                response.ExceptionOccured = true;
                                return response;
                            }
                        }

                        #region Log Results
                        string logInfo = $"Relationship Records Created: {requestCollection?.Count}";
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, logInfo, request.Debug);
                        #endregion
                    }
                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.MessageId,request.OrganizationName, request.UserId, method, progressString);
                LogHelper.LogError(request.MessageId,request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, ExecutionException);
                response.ExceptionMessage = "Failed to Map EC data to LOB";
                response.ExceptionOccured = true;
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

        private static string dateStringFormat(string date)
        {
            if (date == null) return null;
            if (date.Length == 10) return date;
            if (date.Length < 8) return date;

            date = date.Insert(2, "/");
            date = date.Insert(5, "/");
            return date;
        }
    }
}
