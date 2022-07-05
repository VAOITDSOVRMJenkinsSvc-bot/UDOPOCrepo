using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using UDO.LOB.Appeals.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.IDProofOrchestration.Messages;
using UDO.LOB.LegacyPayments.Messages;
using UDO.LOB.MVI.Common;
using UDO.LOB.MVI.Messages;
using IMessageBase = UDO.LOB.Core.IMessageBase;

namespace UDO.LOB.PersonSearch.Processors
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class UDOOpenIDProofRequestProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>        
        private string _PID = "";
        private string _fileNumber = "";
        private string _ssIdString = "";
        private Guid _ownerId = new Guid();
        private Guid _vetSnapId = new Guid();
        private Guid _idProofId = new Guid();
        private Guid _veteranId = new Guid();
        private string _ownerType;
        private StringBuilder _logData = new StringBuilder();
        private StringBuilder _logDataTimer = new StringBuilder();

        private enum LoadStatus
        {
            NotStarted = 752280000,
            InProgress = 752280001,
            Completed = 752280002
        };

        private LoadStatus _status = LoadStatus.NotStarted;

        //CSDev Rem 
        private Uri veisBaseUri;
        private LogSettings logSettings { get; set; }


        public IMessageBase Execute(UDOOpenIDProofRequest request)
        {
            UDOOpenIDProofResponse response = new UDOOpenIDProofResponse();
            response.MessageId = request.MessageId;
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = "UDOOpenIDProofRequestProcessor",
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            Stopwatch txnTimer = Stopwatch.StartNew();
            try
            {
                //CSDev REM
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOOpenIDProofRequestProcessor", "Top", request.Debug);
                if (request == null)
                {
                    //CSDev Rem 
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOOpenIDProofRequestProcessor"
                            , string.Format("{0} received a null message", GetType().FullName), request.Debug);
                }
                else
                {
                    //CSDev Rem
                    CRMCommonFunctions CommonFunctions = new CRMCommonFunctions();

                    #region connect to CRM
                    CrmServiceClient OrgServiceProxy = null;

                    try
                    {
                        OrgServiceProxy = ConnectionCache.GetProxy();
                    }
                    catch (Exception connectException)
                    {
                        LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOOpenIDProofRequestProcessor", connectException);


                        response.ExceptionOccurred = true;
                        return response;
                    }
                    #endregion

                    using (OrgServiceProxy)
                    {
                        ColumnSet onecol = new ColumnSet(new string[] { "udo_veteran" });
                        Entity idProof = OrgServiceProxy.Retrieve("udo_idproof", request.IDProofId, onecol);
                        _idProofId = request.IDProofId;
                        if (!idProof.Contains("udo_veteran"))
                        {
                            return response;
                        }
                        else
                        {
                            _veteranId = ((EntityReference)idProof["udo_veteran"]).Id;
                        }

                        if (!getPIDFN(idProof.Id, OrgServiceProxy))
                        {
                            return response;
                        }

                        DoAllIDProofRetrieveActions(idProof, request, OrgServiceProxy);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOOpenIDProofRequestProcessor, Execute", "Exception:" + ex.Message);
                return response;

            }
        }

        //CSDev REm
        public void DoAllIDProofRetrieveActions(Entity IDProof, UDOOpenIDProofRequest request, IOrganizationService OrgServiceProxy)
        {
            #region update statuses


            if (string.IsNullOrEmpty(_PID))
            {
                //fringe case - but if I don't have a PID, then update
                _logData.AppendLine("No PID found, updating idproof and snapshot");
                Entity idProofUpdate = new Entity();
                idProofUpdate.Id = IDProof.Id;
                idProofUpdate.LogicalName = "udo_idproof";
                idProofUpdate["udo_appealintegration"] = new OptionSetValue(752280002);
                idProofUpdate["udo_legacypaymentintegration"] = new OptionSetValue(752280001);
                idProofUpdate["udo_orchcomplete"] = new OptionSetValue(752280001);
                OrgServiceProxy.Update(idProofUpdate);

                Entity vetSnapShot = new Entity();
                vetSnapShot.LogicalName = "udo_veteransnapshot";
                vetSnapShot.Id = _vetSnapId;
                vetSnapShot["udo_pendingappeals"] = "0 pending appeal(s)";
                vetSnapShot["udo_appealscompleted"] = new OptionSetValue(752280002);
                OrgServiceProxy.Update(vetSnapShot);

            }
            else
            {

                Entity idProofUpdate = new Entity();
                idProofUpdate.Id = IDProof.Id;
                idProofUpdate.LogicalName = "udo_idproof";
                //mark it as inProgress
                // no longer doing legacy payments or appeals - doing on Demand instead and these flags drive towards us knowing if we have the data or not
                idProofUpdate["udo_orchcomplete"] = new OptionSetValue(752280001);
                OrgServiceProxy.Update(idProofUpdate);
            }
            #endregion

            //CSDev Rem
            DoOrchestration(IDProof, request);

            //CSDev Rem
            DoAppeals(IDProof, request);
            //no longer doing legacy payments until on demand- doing on Demand instead and these flags drive towards us knowing if we have the data or not
        }

        //CSDev Rem
        public void DoAppeals(Entity IDProof, UDOOpenIDProofRequest request)
        {
            try
            {
                UDOcreateUDOAppealsRelatedEntitiesMultipleRequest idReference = new UDOcreateUDOAppealsRelatedEntitiesMultipleRequest()
                {
                    RelatedEntityFieldName = "udo_idproofid",
                    RelatedEntityId = _idProofId,
                    RelatedEntityName = "udo_idproof"
                };

                UDOcreateUDOAppealsRelatedEntitiesMultipleRequest veteranReference = new UDOcreateUDOAppealsRelatedEntitiesMultipleRequest()
                {
                    RelatedEntityFieldName = "udo_veteranid",
                    RelatedEntityId = _veteranId,
                    RelatedEntityName = "contact"
                };

                UDOcreateUDOAppealsRelatedEntitiesMultipleRequest[] references = new[] { veteranReference, idReference };

                UDOcreateUDOAppealsRequest requestAppeals = new UDOcreateUDOAppealsRequest()
                {
                    MessageId = request.MessageId,
                    SSN = _fileNumber,
                    RelatedParentEntityName = "udo_idproof",
                    RelatedParentFieldName = "udo_idproofid",
                    RelatedParentId = _idProofId,
                    Debug = request.Debug,
                    LogSoap = request.LogSoap,
                    LogTiming = request.LogTiming,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    GetSnapShotData = true,
                    //CSDev REm
                    LegacyServiceHeaderInfo = new UDOHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    ownerId = _ownerId,
                    ownerType = _ownerType,
                    vetsnapshotId = _vetSnapId,
                    idProofId = _idProofId,
                    FileNumber = _fileNumber, 
                    DiagnosticsContext = request.DiagnosticsContext
                };
                //CSDev REM 
                UDOcreateUDOAppealsResponse response = WebApiUtility.SendReceive<UDOcreateUDOAppealsResponse>(requestAppeals, WebApiType.LOB);
            }
            catch(Exception ex)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOOpenIDProofRequestProcessor ", ex);
            }

        }
        //CSDev Rem
        public void DoLegacyPayments(Entity IDProof, UDOOpenIDProofRequest request)
        {
            //no longer doing this during search
            UDOcreateUDOLegacyPaymentsRelatedEntitiesMultipleRequest idReference = new UDOcreateUDOLegacyPaymentsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_idproofid",
                RelatedEntityId = _idProofId,
                RelatedEntityName = "udo_idproof"
            };

            UDOcreateUDOLegacyPaymentsRelatedEntitiesMultipleRequest veteranReference = new UDOcreateUDOLegacyPaymentsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };

            UDOcreateUDOLegacyPaymentsRelatedEntitiesMultipleRequest[] references = new[] { veteranReference, idReference };

            UDOcreateLegacyPaymentsRequest requestLP = new UDOcreateLegacyPaymentsRequest()
            {
                MessageId = request.MessageId,
                ssn = _ssIdString,
                UDOcreateUDOLegacyPaymentsRelatedEntitiesInfo = references,
                filenumber = _fileNumber,
                RelatedParentEntityName = "udo_idproof",
                RelatedParentFieldName = "udo_idproofid",
                RelatedParentId = _idProofId,
                Debug = request.Debug,
                LogSoap = request.LogSoap,
                LogTiming = request.LogTiming,
                UserId = request.UserId,
                OrganizationName = request.OrganizationName,
                //CSDev
                LegacyServiceHeaderInfo = new UDOHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                },
                ownerId = _ownerId,
                ownerType = _ownerType,
                vetsnapshotId = _vetSnapId,
                idProofId = _idProofId,
            };
            //CSDev REM 
            WebApiUtility.SendAsync(veisBaseUri, "CSDEV MESSAGE ID", requestLP, logSettings, null);
        }
        //CSDev Rem
        public void DoOrchestration(Entity IDProof, UDOOpenIDProofRequest request)
        {
            AwardsRelatedEntitiesMultipleRequest idReference = new AwardsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_idproofid",
                RelatedEntityId = _idProofId,
                RelatedEntityName = "udo_idproof"
            };

            AwardsRelatedEntitiesMultipleRequest veteranReference = new AwardsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };

            ClaimsRelatedEntitiesMultipleRequest idReference2 = new ClaimsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_idproofid",
                RelatedEntityId = _idProofId,
                RelatedEntityName = "udo_idproof"
            };

            ClaimsRelatedEntitiesMultipleRequest veteranReference2 = new ClaimsRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = _veteranId,
                RelatedEntityName = "contact"
            };


            AwardsRelatedEntitiesMultipleRequest[] awardreferences = new[] { veteranReference, idReference };
            ClaimsRelatedEntitiesMultipleRequest[] claimreferences = new[] { veteranReference2, idReference2 };

            UDOIDProofOrchestrationRequest orchRequest = new UDOIDProofOrchestrationRequest
            {             
                UDOcreateAwardsRelatedEntitiesInfo = awardreferences,
                UDOcreateClaimsRelatedEntitiesInfo = claimreferences,
                MessageId = request.MessageId,
                fileNumber = _fileNumber,
                RelatedParentEntityName = "udo_idproof",
                RelatedParentFieldName = "udo_idproofid",
                RelatedParentId = _idProofId,
                Debug = request.Debug,
                LogSoap = request.LogSoap,
                LogTiming = request.LogTiming,
                ptcpntVetId = _PID,
                UserId = request.UserId,
                OrganizationName = request.OrganizationName,
                //CSDev 
                LegacyServiceHeaderInfo = new UDOHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                },
                ownerId = _ownerId,
                ownerType = _ownerType,
                vetsnapshotId = _vetSnapId,
                idProofId = _idProofId,
                udo_ssidstring = _ssIdString,
                udo_contactId = _veteranId
            };
            //CSDev REM 
            WebApiUtility.SendAsync(orchRequest, WebApiType.LOB);
        }
        //CSDev Rem
        internal bool getPIDFN(Guid parentId, IOrganizationService OrgServiceProxy)
        {
            try
            {


                string fetch = "<fetch count='1'><entity name='contact'>" +
                            "<attribute name='udo_filenumber'/>" +
                            "<attribute name='udo_participantid'/>" +
                            "<attribute name='ownerid'/>" +
                            "<attribute name='udo_ssn'/>" +
                            "<link-entity name='udo_veteransnapshot' to='contactid' from='udo_veteranid' alias='snapshot' line-type='inner'>" +
                            "<attribute name='udo_veteransnapshotid'/>" +
                            "<link-entity name='udo_idproof' from='udo_idproofid' to='udo_idproofid' alias='idproof' line-type='inner'>" +
                            "<attribute name='udo_orchcomplete'/>" +
                            "<filter><condition attribute='udo_idproofid' operator='eq' value='" + parentId + "'/></filter>" +
                            "</link-entity></link-entity></entity></fetch>";

                EntityCollection results = OrgServiceProxy.RetrieveMultiple(new FetchExpression(fetch));
                if (results == null || results.Entities.Count == 0)
                {
                    //snapshot not done..
                    bool snapShotComplete = false;
                    for (int i = 0; i < 8; i++)
                    {
                        string snapFetch = "<fetch count='1'><entity name='udo_veteransnapshot'><attribute name='udo_veteransnapshotid'/>" +
                            "<filter><condition attribute='udo_idproofid' operator='eq' value='" + parentId + "'/></filter>" +
                            "</entity></fetch>";
                        EntityCollection snapResults = OrgServiceProxy.RetrieveMultiple(new FetchExpression(snapFetch));
                        if (snapResults != null && snapResults.Entities.Count != 0)
                        {
                            snapShotComplete = true;
                            break;
                        }
                        Thread.Sleep(500);
                    }
                    if (!snapShotComplete)
                    {
                        _logData.AppendLine("No SNAPSHOT, can't proceed");
                        return false;
                    }
                    else
                    {
                        return getPIDFN(parentId, OrgServiceProxy);
                    }
                }

                // Parse and store result
                Entity result = results.Entities[0];
                _PID = result.GetAttributeValue<string>("udo_participantid");
                _fileNumber = result.GetAttributeValue<string>("udo_filenumber");
                _ssIdString = result.GetAttributeValue<string>("udo_ssn");
                EntityReference owner = result.GetAttributeValue<EntityReference>("ownerid");


                _ownerId = owner.Id;
                _ownerType = owner.LogicalName;

                AliasedValue snapshot = result.GetAttributeValue<AliasedValue>("snapshot.udo_veteransnapshotid");
                if (snapshot != null && snapshot.Value != null)
                {
                    _vetSnapId = (Guid)snapshot.Value;
                }

                AliasedValue orchcomplete = result.GetAttributeValue<AliasedValue>("idproof.udo_orchcomplete");
                if (orchcomplete != null && orchcomplete.Value != null)
                {
                    int statusnum = ((OptionSetValue)orchcomplete.Value).Value;
                    if (statusnum >= 752280000 && statusnum <= 752280002)
                    {
                        _status = (LoadStatus)statusnum;
                    }
                    else
                    {
                        _status = LoadStatus.NotStarted;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
    }
}
