﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using VIMT.ClaimantWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.Contact.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateFlashes,createFlashes method, Processor.
/// Code Generated by IMS on: 5/19/2015 2:33:53 PM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.Contact.Processors
{
    class UDOcreateFlashesProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateFlashesProcessor";
        private string LogBuffer { get; set; }
        private OrganizationRequestCollection requestCollection = new OrganizationRequestCollection();
        private string combinedFlashes = String.Empty;

        public IMessageBase Execute(UDOcreateFlashesRequest request)
        {
            //var request = message as createFlashesRequest;
            UDOcreateFlashesResponse response = new UDOcreateFlashesResponse();
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
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateFlashesProcessor Processor, Connection Error", connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

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

                //  Logger.Instance.Info(string.Format("Should be deleting {0} old flashes",   retrieved.Entities.Count));
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
                    var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, request.UserId, request.Debug);

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
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateFlashesRecordsProcessor Processor, Progess:" + progressString, ExecutionException);
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
                    response.ExceptionOccured = findFlashesResponse.ExceptionOccured;

                    if (findFlashesResponse.VIMTfgenpidreturnclmsInfo != null)
                    {
                        if (findFlashesResponse.VIMTfgenpidreturnclmsInfo.VIMTfgenpidflashesclmsInfo != null)
                        {
                            var flash = findFlashesResponse.VIMTfgenpidreturnclmsInfo.VIMTfgenpidflashesclmsInfo;

                            ///Map Flashes
                            MapFlashesFromPidResponse(flash, request);
                        }
                    }
                }
                else if(!string.IsNullOrEmpty(request.fileNumber))
                {
                    var findFlashesResponse = FindGeneralInfoByFileNumber(request);

                    response.ExceptionMessage = findFlashesResponse.ExceptionMessage;
                    response.ExceptionOccured = findFlashesResponse.ExceptionOccured;

                    if (findFlashesResponse.VIMTfgenFNreturnclmsInfo != null)
                    {
                        if (findFlashesResponse.VIMTfgenFNreturnclmsInfo.VIMTfgenFNflashesclmsInfo != null)
                        {
                            var flash = findFlashesResponse.VIMTfgenFNreturnclmsInfo.VIMTfgenFNflashesclmsInfo;

                            ///Map Flashes
                            MapFlashesFromFNResponse(flash, request);
                        }
                    }
                }

                progressString = "After VIMT EC Call";

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
                    var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, request.UserId, request.Debug);

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
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateFlashesProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Map EC data to LOB";
                response.ExceptionOccured = true;
                return response;
            }
        }

        private void MapFlashesFromPidResponse(VIMTfgenpidflashesclmsMultipleResponse[] flash, UDOcreateFlashesRequest request)
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
            LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Flash Records Created", logInfo);
            #endregion
        }

        private VIMTfgenpidfindGeneralInformationByPtcpntIdsResponse FindGeneralInfoByPtcpntIds(UDOcreateFlashesRequest request)
        {
            // prefix = fflfindFlashesRequest();
            var findFlashesRequest = new VIMTfgenpidfindGeneralInformationByPtcpntIdsRequest();
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
                findFlashesRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }
            //findFlashesRequest.mcs_filenumber = request.fileNumber;
            findFlashesRequest.mcs_ptcpntvetid = request.ptcpntVetId;
            findFlashesRequest.mcs_ptcpntbeneid = request.ptcpntBeneId;
            findFlashesRequest.mcs_ptpcntrecipid = request.ptpcntRecipId;
            //findFlashesRequest.mcs_awardtypecd = request.awardTypeCd;
            var findFlashesResponse = new VIMTfgenpidfindGeneralInformationByPtcpntIdsResponse();
            if (request.ECResponse == null)
            {
                // LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateFlashesProcessor ","Didn't get response passed in");
                // TODO(TN): Comment to remediate
                findFlashesResponse = findFlashesRequest.SendReceive<VIMTfgenpidfindGeneralInformationByPtcpntIdsResponse>(MessageProcessType.Local);
            }
            else
            {
                //   LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateFlashesProcessor ", "response passed in");
                findFlashesResponse = request.ECResponse;
            }

            return findFlashesResponse;
        }

        private void MapFlashesFromFNResponse(VIMTfgenFNflashesclmsMultipleResponse[] flash, UDOcreateFlashesRequest request)
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
            LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Flash Records Created", logInfo);
            #endregion
        }

        private VIMTfgenFNfindGeneralInformationByFileNumberResponse FindGeneralInfoByFileNumber(UDOcreateFlashesRequest request)
        {
            // prefix = fflfindFlashesRequest();
            var findFlashesRequest = new VIMTfgenFNfindGeneralInformationByFileNumberRequest();
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
                findFlashesRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }
            findFlashesRequest.mcs_filenumber = request.fileNumber;
            //findFlashesRequest.mcs_awardtypecd = request.awardTypeCd;
            var findFlashesResponse = new VIMTfgenFNfindGeneralInformationByFileNumberResponse();
            if (request.ECResponse == null)
            {
                // LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateFlashesProcessor ","Didn't get response passed in");
                // TODO(TN): Comment to remediate
                findFlashesResponse = findFlashesRequest.SendReceive<VIMTfgenFNfindGeneralInformationByFileNumberResponse>(MessageProcessType.Local);
            }
            //else
            //{
            //    //   LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateFlashesProcessor ", "response passed in");
            //    findFlashesResponse = request.ECResponse;
            //}

            return findFlashesResponse;
        }
    }
}