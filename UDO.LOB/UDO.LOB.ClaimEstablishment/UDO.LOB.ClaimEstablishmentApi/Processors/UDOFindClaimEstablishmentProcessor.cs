

namespace UDO.LOB.ClaimEstablishment.Processors
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.WebServiceClient;
    using Microsoft.Xrm.Tooling.Connector;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UDO.LOB.ClaimEstablishment.Messages;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions;
    using UDO.LOB.Extensions.Logging;
    using VEIS.Core.Messages;
    using VEIS.Messages.BenefitClaimServiceV2;

    public class UDOFindClaimEstablishmentProcessor
    {
        private const string method = "UDOFindClaimEstablishmentProcessor";
        public string MachineName { get; set; }
        private string LogBuffer { get; set; }
        private string progressString = "Top of Processor";
        private bool _debug { get; set; }
        private CrmServiceClient OrgServiceProxy;

        public UDOFindClaimEstablishmentProcessor()
        {
            MachineName = System.Environment.MachineName;
        }

        public IMessageBase Execute(UDOFindClaimEstablishmentRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute"); //{JsonHelper.Serialize<UDOcreateAwardsRequest>(request)}");
            var response = new UDOFindClaimEstablishmentResponse { MessageId = request?.MessageId };
            var claimEstablishmentExceptions = new List<UDOException>();
            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
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
            TraceLogger tLogger = new TraceLogger(method, request);

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOFindClaimEstablishmentRequest>(request)}");

            _debug = request.Debug;
            LogBuffer = string.Empty;

            string progressString = "Top of Processor";

            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                var findClaim = FindBenefitClaim(OrgServiceProxy, request);
                response.ExceptionOccurred = findClaim.ExceptionOccurred;

                if (findClaim.ExceptionOccurred)
                {
                    response.ExceptionMessage = findClaim.ExceptionMessage;
                }
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }

            return response;
        }

        public UDOFindClaimEstablishmentResponse FindBenefitClaim(IOrganizationService orgServiceProxy, UDOFindClaimEstablishmentRequest request)
        {
            var common = new UDOClaimEstablishmentCommon();
            var idproof = common.GetIdProofById(orgServiceProxy, request.IdProofId);
            var response = new UDOFindClaimEstablishmentResponse { MessageId = request?.MessageId };
            var claimEstablishmentExceptions = new List<UDOException>();

            try
            {
                var findBenefitClaimRequest = new VEISfindBenefitClaimRequest()
                {
                    Debug = request.Debug,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo()
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    LogSoap = request.LogSoap,
                    LogTiming = request.LogTiming,
                    OrganizationName = request.OrganizationName,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    MessageId = request.MessageId,
                    mcs_filenumber = request.FileNumber
                };

                //LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "FindBenefitClaim > VEISfindBenefitClaimRequest",
                //    // Replaced: SerializationExtensions.SerializeToString(findBenefitClaimRequest));
                //    JsonHelper.Serialize<UDOFindClaimEstablishmentRequest>(request));

                // REM: Invoke VEIS endpoint
                // var findBenefitClaimResponse = findBenefitClaimRequest.SendReceive<VEISfindBenefitClaimResponse>(request.ProcessType);
                var findBenefitClaimResponse = WebApiUtility.SendReceive<VEISfindBenefitClaimResponse>(findBenefitClaimRequest, WebApiType.VEIS);

                if (request.LogSoap || findBenefitClaimResponse.ExceptionOccurred)
                {
                    if (findBenefitClaimResponse.SerializedSOAPRequest != null || findBenefitClaimResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findBenefitClaimResponse.SerializedSOAPRequest + findBenefitClaimResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfindBenefitClaimRequest Request/Response {requestResponse}", true);
                    }
                }

                //LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "FindBenefitClaim > VEISfindBenefitClaimResponse",
                //    // Replaced: SerializationExtensions.SerializeToString(response));
                //    JsonHelper.Serialize<UDOFindClaimEstablishmentResponse>(response));

                response.ExceptionOccurred = findBenefitClaimResponse.ExceptionOccured;
                response.ExceptionMessage = findBenefitClaimResponse.ExceptionMessage;
                response.MessageId = findBenefitClaimResponse.MessageId;

                if (findBenefitClaimResponse.ExceptionOccured == false)
                {
                    if (findBenefitClaimResponse.VEISbenefitClaimRecordBCS2Info != null)
                    {
                        if (findBenefitClaimResponse.VEISbenefitClaimRecordBCS2Info.mcs_returnCode == "SHAR 9999")
                        {
                            response.UDObenefitClaimRecordBCS2Info = common.ExtractVEISResponse(findBenefitClaimResponse.VEISbenefitClaimRecordBCS2Info);

                            if (response.UDObenefitClaimRecordBCS2Info.UDOparticipantRecordBCS2Info.UDOselectionBCS2Info.Length > 0)
                            {
                                var selectionBCS2MultipleResponse = common.GetClaimEstablishmentByVeteranId(orgServiceProxy, idproof.GetAttributeValue<EntityReference>("udo_veteran").Id);

                                foreach (var item in response.UDObenefitClaimRecordBCS2Info.UDOparticipantRecordBCS2Info.UDOselectionBCS2Info)
                                {
                                    var claimTypeCode = new Entity();
                                    if (common.GetClaimEstablishmentTypeCodebyClaimTypeCode(orgServiceProxy, item.claimTypeCode, out claimTypeCode))
                                    {
                                        var crmGuid = Guid.Empty;
                                        var rtnVal = string.Empty;
                                        var recordFound = common.CheckClaimEstablishment(item, selectionBCS2MultipleResponse, out rtnVal, out crmGuid);

                                        if (!recordFound)
                                        {
                                            if (rtnVal == "New")
                                            {
                                                var person = new Entity();

                                                if (common.GetPersonByIdProofIdandPayeeCode(orgServiceProxy, request.IdProofId, item.payeeTypeCode, out person))
                                                {

                                                    var _vet_filenumber = string.Empty;
                                                    var aliasFilenumber = person.GetAttributeValue<AliasedValue>("contact1.udo_filenumber");
                                                    if (aliasFilenumber != null && aliasFilenumber.Value != null)
                                                    {
                                                        _vet_filenumber = aliasFilenumber.Value.ToString();
                                                    }

                                                    var createClaim = new UDOInitiateClaimEstablishmentRequest();
                                                    createClaim.MessageId = request.MessageId;
                                                    createClaim.Debug = request.Debug;
                                                    createClaim.LogSoap = request.LogSoap;
                                                    createClaim.LogTiming = request.LogTiming;
                                                    createClaim.LegacyServiceHeaderInfo = request.LegacyServiceHeaderInfo;
                                                    createClaim.OrganizationName = request.OrganizationName;
                                                    createClaim.OwnerType = request.OwnerType;
                                                    createClaim.OwnerId = request.OwnerId;
                                                    // REM: Do not need this for VEIS Web Api 
                                                    //createClaim.ProcessType = request.ProcessType;
                                                    createClaim.RelatedEntities = request.RelatedEntities;
                                                    createClaim.RelatedParentEntityName = request.RelatedParentEntityName;
                                                    createClaim.RelatedParentFieldName = request.RelatedParentFieldName;
                                                    createClaim.RelatedParentId = request.RelatedParentId;
                                                    createClaim.fileNumber = _vet_filenumber;
                                                    createClaim.awardtypecode = item.programTypeCode;
                                                    createClaim.SSN = person.GetAttributeValue<string>("udo_ssn");
                                                    createClaim.FirstName = item.claimantFirstName;
                                                    createClaim.LastName = item.claimantLastName;
                                                    createClaim.ptcpntId = Convert.ToInt32(person.GetAttributeValue<string>("udo_ptcpntid"));
                                                    createClaim.vetptcpntId = Convert.ToInt32(person.GetAttributeValue<string>("udo_ptcpntid"));
                                                    createClaim.PayeeCode = item.payeeTypeCode;
                                                    createClaim.UserId = request.UserId;
                                                    createClaim.udo_payeecodeid = person.GetAttributeValue<EntityReference>("udo_payeecodeid").Id;
                                                    createClaim.udo_personid = person.Id;
                                                    createClaim.udo_idproofid = request.IdProofId;
                                                    createClaim.udo_veteranid = person.GetAttributeValue<EntityReference>("udo_veteranid").Id;
                                                    createClaim.udo_veteransnapshotid = person.GetAttributeValue<EntityReference>("udo_veteransnapshotid").Id;
                                                    createClaim.udo_interaction = request.InteractionId;

                                                    // LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "FindBenefitClaim > CreateClaimEstablishment", SerializationExtensions.SerializeToString(createClaim));

                                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, JsonHelper.Serialize<UDOInitiateClaimEstablishmentRequest>(createClaim));

                                                    crmGuid = common.CreateClaimEstablishment(createClaim, orgServiceProxy);
                                                    // LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "FindBenefitClaim > UpdateClaimEstablishmentFromFind", SerializationExtensions.SerializeToString(item));
                                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, JsonHelper.Serialize<UDOselectionBCS2MultipleResponse>(item));

                                                    common.UpdateClaimEstablishmentFromFind(orgServiceProxy, crmGuid, request.IdProofId, item);

                                                }
                                                else
                                                {
                                                    //throw new Exception("Person not found");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (rtnVal == "Update")
                                            {
                                                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, JsonHelper.Serialize<UDOselectionBCS2MultipleResponse>(item));
                                                common.UpdateClaimEstablishmentFromFind(orgServiceProxy, crmGuid, request.IdProofId, item);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }

            }
            catch (Exception executionException)
            {
                var stInfo = string.Empty;

                var st = new StackTrace(executionException, true);
                for (var i = 0; i < st.FrameCount; i++)
                {
                    var sf = st.GetFrame(i);
                    stInfo = stInfo +
                             string.Format("LOB Machine Name: {0}, Method: {1}, File: {2}, Line Number: {3}", MachineName, sf.GetMethod(), sf.GetFileName(),
                                 sf.GetFileLineNumber());
                    stInfo = stInfo + System.Environment.NewLine;
                }

                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method, executionException);

                response.ExceptionMessage = $"{method}: Exception Message: {executionException.Message} Execution Progress : {progressString}";
                response.ExceptionOccurred = true;
                response.StackTrace = stInfo;

                if (claimEstablishmentExceptions.Count > 0) response.InnerExceptions = claimEstablishmentExceptions.ToArray();
                return response;
            }

            return response;
        }

    }
}