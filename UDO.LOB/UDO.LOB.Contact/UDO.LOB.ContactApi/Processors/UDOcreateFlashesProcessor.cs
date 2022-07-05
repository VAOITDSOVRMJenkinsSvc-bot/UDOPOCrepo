using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
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
/// VIMT LOB Component for UDOcreateFlashes,createFlashes method, Processor.
/// </summary>

namespace UDO.LOB.Contact.Processors
{
    class UDOcreateFlashesProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOcreateFlashesProcessor";
        
        private OrganizationRequestCollection requestCollection = new OrganizationRequestCollection();
        private string combinedFlashes = String.Empty;

        public IMessageBase Execute(UDOcreateFlashesRequest request)
        {
            //var request = message as createFlashesRequest;
            UDOcreateFlashesResponse response = new UDOcreateFlashesResponse();
            response.MessageId = request.MessageId;

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

            using (OrgServiceProxy)
            { 
                progressString = "After Connection, deleting data";
                #region delete existingt
                try
                {

                    QueryByAttribute querybyexpression = new QueryByAttribute("udo_flash");
                    querybyexpression.ColumnSet = new ColumnSet("udo_flashid");

                    if (request.DependentId == Guid.Empty)
                    {
                        querybyexpression.Attributes.AddRange("udo_veteranid");

                        //  Value of queried attribute to return    
                        querybyexpression.Values.AddRange(request.VeteranId);
                    }
                    else
                    {
                        querybyexpression.Attributes.AddRange("udo_dependentid");

                        //  Value of queried attribute to return    
                        querybyexpression.Values.AddRange(request.DependentId);
                    }
                    //  Query passed to the service proxy    
                    EntityCollection retrieved = OrgServiceProxy.RetrieveMultiple(querybyexpression);

                    //  Iterate through returned collection   
                    var deleteCount = 0;
                    var requestCollection = new OrganizationRequestCollection();

                    foreach (var c in retrieved.Entities)
                    {
                        DeleteRequest deleteRequest = new DeleteRequest
                        {
                            Target = new EntityReference("udo_flash", c.Id)
                        };
                        requestCollection.Add(deleteRequest);
                        deleteCount += 1;
                    }

                    if (deleteCount > 0)
                    {
                        var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);

                        if (_debug)
                        {
                            LogBuffer += result.LogDetail;
                            LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                        }

                        if (result.IsFaulted)
                        {
                            LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                            response.ExceptionMessage = result.FriendlyDetail;
                            response.ExceptionOccured = true;
                            return response;
                        }
                        requestCollection.Clear();
                    }
                }
                catch (Exception ExecutionException)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateFlashesRecordsProcessor Processor, Progess:" + progressString, ExecutionException);
                    response.ExceptionMessage = "Failed to Delete Old Flashes";
                    response.ExceptionOccured = true;
                    return response;
                }
                #endregion

                progressString = "Records removed, beginning new request";

                try
                {
                    if (!string.IsNullOrEmpty(request.ptcpntVetId))
                    {
                        var findFlashesResponse = FindGeneralInfoByPtcpntIds(request);

                        response.ExceptionMessage = findFlashesResponse.ExceptionMessage;
                        response.ExceptionOccured = findFlashesResponse.ExceptionOccurred;

                        if (findFlashesResponse.VEISfgenpidreturnInfo != null)
                        {
                            if (findFlashesResponse.VEISfgenpidreturnInfo.VEISfgenpidflashesInfo != null)
                            {
                                var flash = findFlashesResponse.VEISfgenpidreturnInfo.VEISfgenpidflashesInfo;

                                ///Map Flashes
                                MapFlashesFromPidResponse(flash, request);
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(request.fileNumber))
                    {
                        var findFlashesResponse = FindGeneralInfoByFileNumber(request);

                        response.ExceptionMessage = findFlashesResponse.ExceptionMessage;
                        response.ExceptionOccured = findFlashesResponse.ExceptionOccurred;

                        // Replaced: VIMTfgenFNreturnclmsInfo = VEISfgenFNreturnInfo
                        if (findFlashesResponse.VEISfgenFNreturnInfo != null)
                        {
                            if (findFlashesResponse.VEISfgenFNreturnInfo.VEISfgenFNflashesInfo != null)
                            {
                                var flash = findFlashesResponse.VEISfgenFNreturnInfo.VEISfgenFNflashesInfo;

                                ///Map Flashes
                                MapFlashesFromFNResponse(flash, request);
                            }
                        }
                    }

                    progressString = "After VEIS EC Call";

                    if (request.VeteranId != null)
                    {
                        Entity thisContact = new Entity("contact");
                        thisContact.Id = request.VeteranId;
                        thisContact["udo_flashes"] = combinedFlashes;
                        UpdateRequest updateVet = new UpdateRequest
                        {
                            Target = thisContact
                        };
                        requestCollection.Add(updateVet);
                    }
                    if (request.VeteranSnapShotId != null)
                    {
                        Entity thisSnapShot = new Entity("udo_veteransnapshot");
                        thisSnapShot.Id = request.VeteranSnapShotId;
                        thisSnapShot["udo_flashes"] = combinedFlashes;
                        thisSnapShot["udo_flashescomplete"] = true;
                        UpdateRequest updateVetSnap = new UpdateRequest
                        {
                            Target = thisSnapShot
                        };
                        requestCollection.Add(updateVetSnap);
                    }

                    if (requestCollection.Count() > 0)
                    {
                        var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);

                        if (_debug)
                        {
                            LogBuffer += result.LogDetail;
                            LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                        }

                        if (result.IsFaulted)
                        {
                            LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                            response.ExceptionMessage = result.FriendlyDetail;
                            response.ExceptionOccured = true;
                            return response;
                        }
                    }
                    response.combinedFlashes = combinedFlashes;

                    return response;
                }
                catch (Exception ExecutionException)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateFlashesProcessor Processor, Progess:" + progressString, ExecutionException);
                    response.ExceptionMessage = "Failed to Map EC data to LOB";
                    response.ExceptionOccured = true;
                    return response;
                }
            } 
        }

        private void MapFlashesFromPidResponse(VEISfgenpidflashesMultipleResponse[] flash, UDOcreateFlashesRequest request)
        {
            var numOfFlashes = 0;

            foreach (var flashItem in flash)
            {
                if (numOfFlashes > 0)
                {
                    combinedFlashes += " : " + flashItem.mcs_flashName.Trim();
                }
                else
                {
                    combinedFlashes = flashItem.mcs_flashName.Trim();
                }
                numOfFlashes += 1;
                var responseIds = new UDOcreateFlashesMultipleResponse();
                //instantiate the new Entity
                Entity thisNewEntity = new Entity();
                thisNewEntity.LogicalName = "udo_flash";
                if (request.ownerId != System.Guid.Empty)
                {
                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                }
                else
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, "Create Flashes", "No Owner");
                }

                if (flashItem.mcs_flashName != string.Empty)
                {
                    thisNewEntity["udo_name"] = flashItem.mcs_flashName;
                }

                if (request.UDOcreateFlashesRelatedEntitiesInfo != null)
                {
                    foreach (var relatedItem in request.UDOcreateFlashesRelatedEntitiesInfo)
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

            #region Log Results
            string logInfo = string.Format("Flash Records Created: {0}", requestCollection.Count());
            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, logInfo);
            #endregion
        }

        private VEISfgenpidfindGeneralInformationByPtcpntIdsResponse FindGeneralInfoByPtcpntIds(UDOcreateFlashesRequest request)
        {
            var findFlashesRequest = new VEISfgenpidfindGeneralInformationByPtcpntIdsRequest();
            findFlashesRequest.MessageId = request.MessageId;
            findFlashesRequest.LogTiming = request.LogTiming;
            findFlashesRequest.LogSoap = request.LogSoap;
            findFlashesRequest.Debug = request.Debug;
            findFlashesRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findFlashesRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findFlashesRequest.RelatedParentId = request.RelatedParentId;
            findFlashesRequest.UserId = request.UserId;
            findFlashesRequest.OrganizationName = request.OrganizationName;
            if (request.LegacyServiceHeaderInfo != null)
            {
                findFlashesRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }

            // TODO: VEIS Dependency
            findFlashesRequest.mcs_ptcpntvetid = request.ptcpntVetId;
            findFlashesRequest.mcs_ptcpntbeneid = request.ptcpntBeneId;
            findFlashesRequest.mcs_ptpcntrecipid = request.ptpcntRecipId;
            findFlashesRequest.mcs_awardtypecd = request.awardTypeCd;
            var findFlashesResponse = new VEISfgenpidfindGeneralInformationByPtcpntIdsResponse();
            if (request.ECResponse == null)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateFlashesProcessor ","Didn't get response passed in");
                // REM: Invoke VEIS Endpoint
                findFlashesResponse = WebApiUtility.SendReceive<VEISfgenpidfindGeneralInformationByPtcpntIdsResponse>(findFlashesRequest, WebApiType.VEIS);
                if (request.LogSoap || findFlashesResponse.ExceptionOccurred)
                {
                    if (findFlashesResponse.SerializedSOAPRequest != null || findFlashesResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findFlashesResponse.SerializedSOAPRequest + findFlashesResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfgenpidfindGeneralInformationByPtcpntIdsRequest Request/Response {requestResponse}", true);
                    }
                }
            }
            else
            {
                findFlashesResponse = request.ECResponse;
            }

            return findFlashesResponse;
        }

        private void MapFlashesFromFNResponse(VEISfgenFNflashesMultipleResponse[] flash, UDOcreateFlashesRequest request)
        {
            var numOfFlashes = 0;

            foreach (var flashItem in flash)
            {
                if (numOfFlashes > 0)
                {
                    combinedFlashes += " : " + flashItem.mcs_flashName.Trim();
                }
                else
                {
                    combinedFlashes = flashItem.mcs_flashName.Trim();
                }
                numOfFlashes += 1;
                var responseIds = new UDOcreateFlashesMultipleResponse();
                //instantiate the new Entity
                Entity thisNewEntity = new Entity();
                thisNewEntity.LogicalName = "udo_flash";
                if (request.ownerId != System.Guid.Empty)
                {
                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                }
                else
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, method, "Create Flashes - No Owner");
                }

                if (flashItem.mcs_flashName != string.Empty)
                {
                    thisNewEntity["udo_name"] = flashItem.mcs_flashName;
                }

                if (request.UDOcreateFlashesRelatedEntitiesInfo != null)
                {
                    foreach (var relatedItem in request.UDOcreateFlashesRelatedEntitiesInfo)
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

            #region Log Results
            string logInfo = string.Format("Flash Records Created: {0}", requestCollection.Count());
            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, logInfo);
            #endregion
        }

        private VEISfgenFNfindGeneralInformationByFileNumberResponse FindGeneralInfoByFileNumber(UDOcreateFlashesRequest request)
        {
            var findFlashesRequest = new VEISfgenFNfindGeneralInformationByFileNumberRequest();
            findFlashesRequest.MessageId = request.MessageId;
            findFlashesRequest.LogTiming = request.LogTiming;
            findFlashesRequest.LogSoap = request.LogSoap;
            findFlashesRequest.Debug = request.Debug;
            findFlashesRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findFlashesRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findFlashesRequest.RelatedParentId = request.RelatedParentId;
            findFlashesRequest.UserId = request.UserId;
            findFlashesRequest.OrganizationName = request.OrganizationName;
            if (request.LegacyServiceHeaderInfo != null)
            {
                findFlashesRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }
            findFlashesRequest.mcs_filenumber = request.fileNumber;
            var findFlashesResponse = new VEISfgenFNfindGeneralInformationByFileNumberResponse();
            if (request.ECResponse == null)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateFlashesProcessor ","Didn't get response passed in");
                // REM: Invoke VEIS Endpoint
                findFlashesResponse = WebApiUtility.SendReceive<VEISfgenFNfindGeneralInformationByFileNumberResponse>(findFlashesRequest, WebApiType.VEIS);

                if (request.LogSoap || findFlashesResponse.ExceptionOccurred)
                {
                    if (findFlashesResponse.SerializedSOAPRequest != null || findFlashesResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findFlashesResponse.SerializedSOAPRequest + findFlashesResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfgenFNfindGeneralInformationByFileNumberRequest Request/Response {requestResponse}", true);
                    }
                }
            }

            return findFlashesResponse;
        }
    }
}