using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Globalization;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.VeteranSnapShot.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.ClaimantService;

namespace UDO.LOB.VeteranSnapShot.Processors
{
    /// <summary>
    /// UDOCreateVeteranSnapshotProcessor
    /// </summary>
    public class UDOCreateVeteranSnapshotProcessor : TimedProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOCreateVeteranSnapshotProcessor";

        public IMessageBase Execute(UDOCreateVeteranSnapshotRequest request)
        {
            var response = new UDOCreateVeteranSnapshotResponse();
            response.MessageId = request.MessageId;

            #region Check for null request message

            if (request == null)
            {
                response = Tools.Exception<UDOCreateVeteranSnapshotResponse>(CommonErrorMessages.ReceivedNullMessage, CurrentMethod);
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, CommonErrorMessages.ReceivedNullMessage);
                return response;
            }

            #endregion

            #region Set Method and Start Timer
            Timer.Restart();
            MethodHistory = new Stack<string>();
            LogStartOfMethod(method);
            #endregion

            #region connect to CRM
            CrmServiceClient OrgServiceProxy = null;

            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }

            #endregion

            var entity = MapRequestToVeteranSnapshot(request);

            GetPOAFID(request, entity);

            Guid id;
            using (OrgServiceProxy)
            {
                id = OrgServiceProxy.Create(TruncateHelper.TruncateFields(request.MessageId, entity, request.OrganizationName, request.UserId,
                    request.LogTiming, OrgServiceProxy));
            }

            response.udo_veteransnapshotId = id;

            StopTimer(request);

            return response;
        }

        private void GetPOAFID(UDOCreateVeteranSnapshotRequest request, Entity entity)
        {
            if (string.IsNullOrEmpty(request.udo_filenumber)) return;

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
            findAllFiduciaryPoaRequest.mcs_filenumber = request.udo_filenumber;
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
            Timer.MarkStart("VEISafidpoafindAllFiduciaryPoaRequest");
            var findAllFiduciaryPoaResponse = WebApiUtility.SendReceive<VEISafidpoafindAllFiduciaryPoaResponse>(findAllFiduciaryPoaRequest, WebApiType.VEIS);
            if (request.LogSoap || findAllFiduciaryPoaResponse.ExceptionOccurred)
            {
                if (findAllFiduciaryPoaResponse.SerializedSOAPRequest != null || findAllFiduciaryPoaResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findAllFiduciaryPoaResponse.SerializedSOAPRequest + findAllFiduciaryPoaResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISafidpoafindAllFiduciaryPoaRequest Request/Response {requestResponse}", true);
                }
            }

            Timer.MarkStop("VEISafidpoafindAllFiduciaryPoaRequest");

            if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo != null)
            {
                if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentFiduciaryInfo != null)
                {
                    #region map current FID data

                    if (
                        !string.IsNullOrEmpty(
                            findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentFiduciaryInfo
                                .mcs_personOrgName))
                    {
                        var fidInfo = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentFiduciaryInfo;

                        DateTime fidEndDate = DateTime.Today.AddDays(5);

                        if (!String.IsNullOrEmpty(fidInfo.mcs_endDate))
                            fidEndDate = DateTime.ParseExact(fidInfo.mcs_endDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                        var currentDate = DateTime.Today;

                        if (!String.IsNullOrEmpty(fidInfo.mcs_personOrgName) && fidEndDate > currentDate)
                        {

                            entity["udo_cfidpersonorgname"] =
                                findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentFiduciaryInfo
                                    .mcs_personOrgName;
                        }
                    }

                    #endregion
                }

                if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo != null)
                {
                    #region map current POA data

                    var poaInfo = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo;

                    if (string.IsNullOrEmpty(poaInfo.mcs_endDate) && !string.IsNullOrEmpty(poaInfo.mcs_personOrgName))
                    {
                        entity["udo_poa"] = poaInfo.mcs_personOrgName;
                    }

                    #endregion
                }
            }

            #endregion
        }

        private static Entity MapRequestToVeteranSnapshot(UDOCreateVeteranSnapshotRequest request)
        {
            var entity = new Entity { LogicalName = "udo_veteransnapshot" };
            entity["udo_veteranid"] = new EntityReference("contact", request.udo_veteranid);
            entity["udo_name"] = request.udo_name;
            entity["udo_idproofid"] = new EntityReference("udo_idproof", request.udo_idproofid);
            entity["udo_phonenumber"] = request.udo_phonenumber;
            entity["udo_firstname"] = request.udo_firstname;
            entity["udo_lastname"] = request.udo_lastname;
            entity["udo_ssn"] = request.ssid.ToUnsecureString();
            entity["udo_characterofdischarge"] = request.udo_characterofdischarge;
            entity["udo_dateofdeath"] = request.udo_dateofdeath;
            entity["udo_soj"] = request.udo_soj;
            entity["udo_branchofservice"] = request.udo_branchofservice;
            entity["udo_rank"] = request.udo_rank;
            entity["udo_filenumber"] = request.udo_filenumber;
            entity["udo_gender"] = request.udo_gender;
            entity["udo_birthdatestring"] = request.udo_birthdatestring;
            entity["udo_participantid"] = request.udo_participantid;
            if (request.OwnerId.HasValue)
            {
                entity["ownerid"] = new EntityReference(request.OwnerType, request.OwnerId.Value);
            }
            return entity;
        }
    }
}