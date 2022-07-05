using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Globalization;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.VeteranSnapShot.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.ClaimantService;

namespace UDO.LOB.VeteranSnapShot.Processors
{
    class UDOcreateUDOVeteranSnapShotProcessor
    {
        private string LogBuffer { get; set; }

        private const string method = "UDOcreateUDOVeteranSnapShotProcessor";

        public IMessageBase Execute(UDOcreateUDOVeteranSnapShotRequest request)
        {
            UDOcreateUDOVeteranSnapShotResponse response = new UDOcreateUDOVeteranSnapShotResponse { MessageId = request?.MessageId };

            var progressString = "1. Top of Processor";
            LogBuffer = string.Empty;
            // _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
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
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = $"{method}: Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }

            #endregion

            progressString += " >> 2. After CRM Connection";

            try
            {
                var colsSet = new ColumnSet("ownerid", "udo_characterofdischarge", "udo_dateofdeath", "udo_soj");
                Entity SnapShot = OrgServiceProxy.Retrieve("udo_veteransnapshot", request.udo_veteransnapshotid, colsSet);

                EntityReference Owner = (EntityReference)SnapShot["ownerid"];

                var requestCollection = new OrganizationRequestCollection();
                //RC feature_3023 
                //added update to contact to avoid repeat call in contactLOB
                Entity thisContact = new Entity();
                thisContact.LogicalName = "contact";
                thisContact.Id = request.udo_veteranid;
                
                #region grab POAFID

                //if this doesn't contain anything, don't go asking for it!
                if (!string.IsNullOrEmpty(request.fileNumber))
                {
                    #region findPOA work
                    var findAllFiduciaryPoaRequest = new VEISafidpoafindAllFiduciaryPoaRequest();
                    findAllFiduciaryPoaRequest.MessageId = request.MessageId;
                    findAllFiduciaryPoaRequest.LogTiming = request.LogTiming;
                    findAllFiduciaryPoaRequest.LogSoap = request.LogSoap;
                    findAllFiduciaryPoaRequest.Debug = request.Debug;
                    findAllFiduciaryPoaRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                    findAllFiduciaryPoaRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                    findAllFiduciaryPoaRequest.RelatedParentId = request.RelatedParentId;
                    findAllFiduciaryPoaRequest.UserId = request.UserId;
                    findAllFiduciaryPoaRequest.OrganizationName = request.OrganizationName;

                    //non standard fields
                    findAllFiduciaryPoaRequest.mcs_filenumber = request.fileNumber;
                    if (request.LegacyServiceHeaderInfo != null)
                    {
                        findAllFiduciaryPoaRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                    }
                    // REM: Invoke VEIS Endpoint
                    var findAllFiduciaryPoaResponse = WebApiUtility.SendReceive<VEISafidpoafindAllFiduciaryPoaResponse>(findAllFiduciaryPoaRequest, WebApiType.VEIS);
                    if (request.LogSoap || findAllFiduciaryPoaResponse.ExceptionOccurred)
                    {
                        if (findAllFiduciaryPoaResponse.SerializedSOAPRequest != null || findAllFiduciaryPoaResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = findAllFiduciaryPoaResponse.SerializedSOAPRequest + findAllFiduciaryPoaResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISafidpoafindAllFiduciaryPoaRequest Request/Response {requestResponse}", true);
                        }
                    }

                    progressString += ">> After VEIS EC Call";


                    if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo != null)
                    {
                        if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentFiduciaryInfo != null)
                        {
                            #region map current FID data

                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentFiduciaryInfo.mcs_personOrgName))
                            {
                                var fidInfo = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentFiduciaryInfo;

                                DateTime fidEndDate = DateTime.Today.AddDays(5);

                                if (!String.IsNullOrEmpty(fidInfo.mcs_endDate))
                                    fidEndDate = DateTime.ParseExact(fidInfo.mcs_endDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                                var currentDate = DateTime.Today;

                                if (!String.IsNullOrEmpty(fidInfo.mcs_personOrgName) && fidEndDate > currentDate)
                                {
                                    SnapShot["udo_cfidpersonorgname"] = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentFiduciaryInfo.mcs_personOrgName;
                                }
                            }
                            //RC feature_3023 
                            //added update to contact to avoid repeat call in contactLOB
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentFiduciaryInfo.mcs_personOrgName))
                            {
                                thisContact["udo_fiduciaryappointed"] = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentFiduciaryInfo.mcs_personOrgName.TrimWhiteSpace();
                            }

                            #endregion
                        }
                        else
                        {
                            progressString += " >> 3. FID NOT FOUND in findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentFiduciaryInfo";
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $"Progress: {progressString}", request.Debug);
                        }
                        if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo != null)
                        {
                            #region map current POA data

                            var poaInfo = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo;

                            if (string.IsNullOrEmpty(poaInfo.mcs_endDate) && !string.IsNullOrEmpty(poaInfo.mcs_personOrgName))
                            {
                                SnapShot["udo_poa"] = poaInfo.mcs_personOrgName;
                                //RC feature_3023 
                                //added update to contact to avoid repeat call in contactLOB
                                thisContact["udo_poa"] = poaInfo.mcs_personOrgName;
                            }

                            #endregion
                        }
                        else
                        {
                            progressString += " >> 4. POA NOT FOUND in findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo";
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $" {progressString}", request.Debug);
                        }
                    }
                    else
                    {
                        progressString += " >> 4. fidpoarecords NOT FOUND in findAllFiduciaryPoaResponse.";
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $" {progressString}", request.Debug);
                    }

                    #endregion
                }

                #endregion

                //since we have them, let's update contact from vetsnapshot
                if (SnapShot.Attributes.Contains("udo_characterofdischarge"))
                {
                    thisContact["udo_charactorofdischarge"] = SnapShot["udo_characterofdischarge"];
                }
                if (SnapShot.Attributes.Contains("udo_dateofdeath"))
                {
                    thisContact["udo_dateofdeath"] = SnapShot["udo_dateofdeath"];
                }
                if (SnapShot.Attributes.Contains("udo_soj"))
                {
                    thisContact["udo_folderlocation"] = SnapShot["udo_soj"];
                }
                OrgServiceProxy.Update(SnapShot);
                OrgServiceProxy.Update(thisContact);
                progressString += " updated snapshot and contact";
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $" {progressString}", request.Debug);

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, progressString);
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, ExecutionException);
                response.ExceptionMessage = "Failed to Process Veteran Snapshot - " + ExecutionException.Message;
                response.ExceptionOccurred = true;
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

        private static CreateRequest FlashToCreateRequest(UDOcreateUDOVeteranSnapShotRequest request, EntityReference Owner, string flashText)
        {
            var entity = new Entity("udo_flash");
            entity["ownerid"] = Owner;
            entity["udo_name"] = flashText.Trim();
            entity["udo_veteranid"] = new EntityReference("contact", request.udo_veteranid);

            var createRequest = new CreateRequest { Target = entity };
            return createRequest;
        }
    }
}