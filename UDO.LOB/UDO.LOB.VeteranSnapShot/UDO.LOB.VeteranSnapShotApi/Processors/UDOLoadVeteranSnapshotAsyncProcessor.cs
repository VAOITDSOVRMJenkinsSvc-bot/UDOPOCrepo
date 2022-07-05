using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using UDO.LOB.Appeals.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.IDProofOrchestration.Messages;
using UDO.LOB.VeteranSnapShot.Messages;

namespace UDO.LOB.VeteranSnapShot.Processors
{
    /// <summary>
    /// UDOLoadVeteranSnapshotAsyncProcessor
    /// </summary>
    public class UDOLoadVeteranSnapshotAsyncProcessor : TimedProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOCreateVeteranSnapshotProcessor";

        private UDOLoadVeteranSnapshotAsyncRequest Request { get; set; }

        public UDOException Execute(UDOLoadVeteranSnapshotAsyncRequest request)
        {
            var response = new UDOException();
      
            #region Check for null request message

            if (request == null)
            {
                response = Tools.Exception<UDOException>(CommonErrorMessages.ReceivedNullMessage, CurrentMethod);
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, CommonErrorMessages.ReceivedNullMessage);
                return response;
            }

            #endregion

            Request = request;

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
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }

            #endregion

            try
            {
                DoAllIDProofRetrieveActions(OrgServiceProxy);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, ex);
                response.ExceptionMessage = ex.Message;
                response.ExceptionOccurred = true;
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }

            StopTimer(request);
            return response;
        }


        public void DoAllIDProofRetrieveActions(IOrganizationService OrgServiceProxy)
        {
            #region update statuses

            if (string.IsNullOrEmpty(Request.udo_participantid))
            {
                //fringe case - but if I don't have a PID, then update
                //idproof and snapshot accordingly
                Timer.Log("No PID found, updating idproof and snapshot");
                var idProofUpdate = new Entity();
                idProofUpdate.Id = Request.udo_idproofid;
                idProofUpdate.LogicalName = "udo_idproof";
                idProofUpdate["udo_appealintegration"] = new OptionSetValue(752280002);
                idProofUpdate["udo_legacypaymentintegration"] = new OptionSetValue(752280001);
                idProofUpdate["udo_orchcomplete"] = new OptionSetValue(752280001);
                OrgServiceProxy.Update(idProofUpdate);

                var vetSnapShot = new Entity();
                vetSnapShot.LogicalName = "udo_veteransnapshot";
                vetSnapShot.Id = Request.udo_veteransnapshotid;
                vetSnapShot["udo_pendingappeals"] = "0 pending appeal(s)";
                vetSnapShot["udo_appealscompleted"] = new OptionSetValue(752280002);
                OrgServiceProxy.Update(vetSnapShot);
            }
            else
            {
                var idProofUpdate = new Entity();
                idProofUpdate.Id = Request.udo_idproofid;
                idProofUpdate.LogicalName = "udo_idproof";
                //mark it as inProgress
                // no longer doing legacy payments or appeals - doing on Demand instead and these flags drive towards us knowing if we have the data or not
                idProofUpdate["udo_orchcomplete"] = new OptionSetValue(752280001);
                OrgServiceProxy.Update(idProofUpdate);
            }

            #endregion

            DoOrchestration();
            DoAppeals();
            //no longer doing legacy payments until on demand- doing on Demand instead and these flags drive towards us knowing if we have the data or not
        }

        public void DoAppeals()
        {
            var idReference = new UDOcreateUDOAppealsRelatedEntitiesMultipleRequest
            {
                RelatedEntityFieldName = "udo_idproofid",
                RelatedEntityId = Request.udo_idproofid,
                RelatedEntityName = "udo_idproof"
            };

            var veteranReference = new UDOcreateUDOAppealsRelatedEntitiesMultipleRequest
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = Request.udo_veteranid,
                RelatedEntityName = "contact"
            };

            var references = new[] {veteranReference, idReference};

            var request = new UDOcreateUDOAppealsRequest
            {
                MessageId = Request.MessageId,
                SSN = Request.udo_filenumber,
                RelatedParentEntityName = "udo_idproof",
                RelatedParentFieldName = "udo_idproofid",
                RelatedParentId = Request.udo_idproofid,
                Debug = Request.Debug,
                LogSoap = Request.LogSoap,
                LogTiming = Request.LogTiming,
                UserId = Request.UserId,
                OrganizationName = Request.OrganizationName,
                GetSnapShotData = true,
                LegacyServiceHeaderInfo = new UDOHeaderInfo
                {
                    ApplicationName = Request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = Request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = Request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = Request.LegacyServiceHeaderInfo.StationNumber
                },
                UDOcreateUDOAppealsRelatedEntitiesInfo = references,
                ownerId = Request.OwnerId ?? Guid.Empty,
                ownerType = Request.OwnerType,
                vetsnapshotId = Request.udo_veteransnapshotid,
                idProofId = Request.udo_idproofid,
                FileNumber = Request.udo_filenumber
            };

            var response = WebApiUtility.SendReceive<UDOcreateUDOAppealsResponse>(request, WebApiType.LOB);
        }

        public void DoOrchestration()
        {
            var idReference = new AwardsRelatedEntitiesMultipleRequest
            {
                RelatedEntityFieldName = "udo_idproofid",
                RelatedEntityId = Request.udo_idproofid,
                RelatedEntityName = "udo_idproof"
            };

            var veteranReference = new AwardsRelatedEntitiesMultipleRequest
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = Request.udo_veteranid,
                RelatedEntityName = "contact"
            };

            var idReference2 = new ClaimsRelatedEntitiesMultipleRequest
            {
                RelatedEntityFieldName = "udo_idproofid",
                RelatedEntityId = Request.udo_idproofid,
                RelatedEntityName = "udo_idproof"
            };

            var veteranReference2 = new ClaimsRelatedEntitiesMultipleRequest
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = Request.udo_veteranid,
                RelatedEntityName = "contact"
            };


            var awardreferences = new[] {veteranReference, idReference};
            var claimreferences = new[] {veteranReference2, idReference2};

            var request = new UDOIDProofOrchestrationRequest
            {
                UDOcreateAwardsRelatedEntitiesInfo = awardreferences,
                UDOcreateClaimsRelatedEntitiesInfo = claimreferences,
                MessageId = Request.MessageId,
                fileNumber = Request.udo_filenumber,
                RelatedParentEntityName = "udo_idproof",
                RelatedParentFieldName = "udo_idproofid",
                RelatedParentId = Request.udo_idproofid,
                Debug = Request.Debug,
                LogSoap = Request.LogSoap,
                LogTiming = Request.LogTiming,
                ptcpntVetId = Request.udo_participantid,
                UserId = Request.UserId,
                OrganizationName = Request.OrganizationName,
                LegacyServiceHeaderInfo = new UDOHeaderInfo
                {
                    ApplicationName = Request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = Request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = Request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = Request.LegacyServiceHeaderInfo.StationNumber
                },
                ownerId = Request.OwnerId ?? Guid.Empty,
                ownerType = Request.OwnerType,
                vetsnapshotId = Request.udo_veteransnapshotid,
                idProofId = Request.udo_idproofid,
                
                udo_ssidstring = Request.ssid.ToUnsecureString(),
                udo_contactId = Request.udo_veteranid
            };

            // REM: Invoke UDO LOB Endpoint
            var response = WebApiUtility.SendReceive<UDOIDProofOrchestrationResponse>(request, WebApiType.LOB);
        }
    }
}