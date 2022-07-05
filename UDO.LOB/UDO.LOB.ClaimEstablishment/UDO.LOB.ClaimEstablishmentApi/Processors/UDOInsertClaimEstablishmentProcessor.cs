using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xrm.Sdk.Query;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//using VRM.Integration.UDO.ClaimEstablishment.Messages;
//using VRM.Integration.UDO.Common;
//using VRM.Integration.UDO.Common.Messages;
//using VIMT.BenefitClaimServiceV2.Messages;

//using VRM.Integration.UDO.ClaimEstablishment.Helper;

using VEIS.Messages.BenefitClaimServiceV2;
using UDO.LOB.ClaimEstablishment.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using System.Collections.Specialized;
using UDO.LOB.Extensions.Configuration;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;

namespace UDO.LOB.ClaimEstablishment.Processors
{
    public class UDOInsertClaimEstablishmentProcessor
    {
        private bool _debug { get; set; }
        private string method = "UDOInsertClaimEstablishmentProcessor";
        string progressString = "Top of Processor";
        private string LogBuffer { get; set; }
        private CrmServiceClient OrgServiceProxy;
        public IMessageBase Execute(UDOInsertClaimEstablishmentRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute");

            UDOInsertClaimEstablishmentResponse response = new UDOInsertClaimEstablishmentResponse();
            var claimEstablishmentExceptions = new List<UDOException>();
            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
                return response;
            }
            response.MessageId = request.MessageId;
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

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOInsertClaimEstablishmentRequest>(request)}");

            _debug = request.Debug;
            LogBuffer = string.Empty;

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
                var entity = OrgServiceProxy.Retrieve("udo_claimestablishment", request.ClaimEstablishmentId,
                    new ColumnSet(true));

                var claimInserted = InsertBenefitClaim(request, entity, OrgServiceProxy);
                response.ClaimEstablishmentId = request.ClaimEstablishmentId;
                response.ExceptionOccurred = claimInserted.ExceptionOccurred;

                if (claimInserted.ExceptionOccurred)
                {
                    response.ExceptionMessage = claimInserted.ExceptionMessage;
                }
                else
                {
                    response.UDObenefitClaimRecordBCS2Info = claimInserted.UDObenefitClaimRecordBCS2Info;
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

        private UDOInsertClaimEstablishmentResponse InsertBenefitClaim(UDOInsertClaimEstablishmentRequest request, Entity entity, IOrganizationService organizationService)
        {
            var common = new UDOClaimEstablishmentCommon();
            var response = new UDOInsertClaimEstablishmentResponse();
            var claimEstablishmentExceptions = new List<UDOException>();
            var responseExtractor = new UDOClaimEstablishmentCommon();

            try
            {

                #region Validate Request Message

                if (request == null)
                {
                    claimEstablishmentExceptions.Add(new UDOException()
                    {
                        ExceptionMessage = "Called with no message",
                        ExceptionOccurred = true,
                        ExceptionCategory = "Person"
                    });

                    response.ExceptionMessage = "Called with no message";
                    response.ExceptionOccurred = true;
                    response.InnerExceptions = claimEstablishmentExceptions.ToArray();
                    return response;
                }

                #endregion

                #region Validate Claim Establishment Record

                if (!entity.Contains("udo_filenumber"))
                {
                    claimEstablishmentExceptions.Add(new UDOException()
                    {
                        ExceptionMessage = "File Number is missing",
                        ExceptionOccurred = true,
                        ExceptionCategory = "Claim Establishment"
                    });
                }

                if (!entity.Contains("udo_firstname"))
                {
                    claimEstablishmentExceptions.Add(new UDOException()
                    {
                        ExceptionMessage = "First Name is missing",
                        ExceptionOccurred = true,
                        ExceptionCategory = "Claim Establishment"
                    });
                }

                if (!entity.Contains("udo_lastname"))
                {
                    claimEstablishmentExceptions.Add(new UDOException()
                    {
                        ExceptionMessage = "Last Name is missing",
                        ExceptionOccurred = true,
                        ExceptionCategory = "Claim Establishment"
                    });
                }

                if (!entity.Contains("udo_endproduct"))
                {
                    claimEstablishmentExceptions.Add(new UDOException()
                    {
                        ExceptionMessage = "End Product is missing",
                        ExceptionOccurred = true,
                        ExceptionCategory = "Claim Establishment"
                    });
                }

                if (!entity.Contains("udo_ssn"))
                {
                    claimEstablishmentExceptions.Add(new UDOException()
                    {
                        ExceptionMessage = "SSN is missing",
                        ExceptionOccurred = true,
                        ExceptionCategory = "Claim Establishment"
                    });
                }

                if (!entity.Contains("udo_dateofclaim"))
                {
                    claimEstablishmentExceptions.Add(new UDOException()
                    {
                        ExceptionMessage = "Date of Claim is missing",
                        ExceptionOccurred = true,
                        ExceptionCategory = "Claim Establishment"
                    });
                }

                if (!entity.Contains("udo_sectionunitno"))
                {
                    claimEstablishmentExceptions.Add(new UDOException()
                    {
                        ExceptionMessage = "Section Unit No is missing",
                        ExceptionOccurred = true,
                        ExceptionCategory = "Claim Establishment"
                    });
                }

                if (!entity.Contains("udo_benefitclaimtype"))
                {
                    claimEstablishmentExceptions.Add(new UDOException()
                    {
                        ExceptionMessage = "Benefit Claim Type is missing",
                        ExceptionOccurred = true,
                        ExceptionCategory = "Claim Establishment"
                    });
                }

                if (!entity.Contains("udo_payeecodeid"))
                {
                    claimEstablishmentExceptions.Add(new UDOException()
                    {
                        ExceptionMessage = "Payee Code Id No is missing",
                        ExceptionOccurred = true,
                        ExceptionCategory = "Claim Establishment"
                    });
                }

                if (claimEstablishmentExceptions.Count > 0)
                {
                    response.ExceptionMessage = "Missing key information from Claim Establishment";
                    response.ExceptionOccurred = true;
                    response.InnerExceptions = claimEstablishmentExceptions.ToArray();
                    return response;
                }

                #endregion

                var typecode = organizationService.Retrieve("udo_claimestablishmenttypecode", entity.GetAttributeValue<EntityReference>("udo_endproduct").Id, new ColumnSet(true));
                progressString = "retrieved udo_claimestablishmenttypecode";
                var payeecode = organizationService.Retrieve("udo_claimestablishmentpayeecode", entity.GetAttributeValue<EntityReference>("udo_payeecodeid").Id, new ColumnSet(true));
                progressString = "retrieved udo_payeecode";

                var reqBenefitClaimInput = new VEISReqbenefitClaimInputBCS2();

                if (entity.Contains("udo_addresstype"))
                {

                    var addressType = entity.GetAttributeValue<OptionSetValue>("udo_addresstype");

                    switch (addressType.Value)
                    {
                        case 1:

                            reqBenefitClaimInput.mcs_addressType = "";
                            reqBenefitClaimInput.mcs_addressLine1 = common.RetrieveValueFromEntityField(entity, "udo_addressline1");
                            reqBenefitClaimInput.mcs_addressLine2 = common.RetrieveValueFromEntityField(entity, "udo_addressline2");
                            reqBenefitClaimInput.mcs_addressLine3 = common.RetrieveValueFromEntityField(entity, "udo_addressline3");
                            reqBenefitClaimInput.mcs_city = common.RetrieveValueFromEntityField(entity, "udo_city");
                            reqBenefitClaimInput.mcs_state = common.RetrieveValueFromEntityField(entity, "udo_state");
                            reqBenefitClaimInput.mcs_postalCode = common.RetrieveValueFromEntityField(entity, "udo_postalcode");
                            reqBenefitClaimInput.mcs_country = common.RetrieveValueFromEntityField(entity, "udo_country");

                            break;

                        case 2:

                            reqBenefitClaimInput.mcs_addressType = "OVR";
                            reqBenefitClaimInput.mcs_addressLine1 = common.RetrieveValueFromEntityField(entity, "udo_addressline1");
                            reqBenefitClaimInput.mcs_addressLine2 = common.RetrieveValueFromEntityField(entity, "udo_addressline2");
                            reqBenefitClaimInput.mcs_addressLine3 = common.RetrieveValueFromEntityField(entity, "udo_addressline3");
                            reqBenefitClaimInput.mcs_mltyPostOfficeTypeCd = common.RetrieveValueFromEntityField(entity, "udo_city");
                            reqBenefitClaimInput.mcs_mltyPostalTypeCd = common.RetrieveValueFromEntityField(entity, "udo_state");
                            reqBenefitClaimInput.mcs_postalCode = common.RetrieveValueFromEntityField(entity, "udo_postalcode");
                            reqBenefitClaimInput.mcs_country = common.RetrieveValueFromEntityField(entity, "udo_country");

                            break;

                        case 3:

                            reqBenefitClaimInput.mcs_addressType = "INT";
                            reqBenefitClaimInput.mcs_addressLine1 = common.RetrieveValueFromEntityField(entity, "udo_addressline1");
                            reqBenefitClaimInput.mcs_addressLine2 = common.RetrieveValueFromEntityField(entity, "udo_addressline2");
                            reqBenefitClaimInput.mcs_addressLine3 = common.RetrieveValueFromEntityField(entity, "udo_addressline3");
                            reqBenefitClaimInput.mcs_city = common.RetrieveValueFromEntityField(entity, "udo_city");
                            reqBenefitClaimInput.mcs_state = common.RetrieveValueFromEntityField(entity, "udo_state");
                            reqBenefitClaimInput.mcs_foreignMailCode = common.RetrieveValueFromEntityField(entity, "udo_postalcode");
                            reqBenefitClaimInput.mcs_country = common.RetrieveValueFromEntityField(entity, "udo_country");
                            break;

                    }
                }
                else
                {
                    reqBenefitClaimInput.mcs_addressType = "";

                    reqBenefitClaimInput.mcs_addressLine1 = common.RetrieveValueFromEntityField(entity, "udo_addressline1");
                    reqBenefitClaimInput.mcs_addressLine2 = common.RetrieveValueFromEntityField(entity, "udo_addressline2");
                    reqBenefitClaimInput.mcs_addressLine3 = common.RetrieveValueFromEntityField(entity, "udo_addressline3");
                    reqBenefitClaimInput.mcs_city = common.RetrieveValueFromEntityField(entity, "udo_city");
                    reqBenefitClaimInput.mcs_state = common.RetrieveValueFromEntityField(entity, "udo_state");
                    reqBenefitClaimInput.mcs_postalCode = common.RetrieveValueFromEntityField(entity, "udo_postalcode");
                    reqBenefitClaimInput.mcs_country = common.RetrieveValueFromEntityField(entity, "udo_country");
                }

                if (entity.Attributes.Contains("udo_benefitclaimtype"))
                {
                    reqBenefitClaimInput.mcs_benefitClaimType = common.RetrieveOptionSetValueFromEntityField(organizationService, entity, "udo_benefitclaimtype").ToString();
                }

                reqBenefitClaimInput.mcs_claimantSsn = common.RetrieveValueFromEntityField(entity, "udo_ssn");

                if (entity.Attributes.Contains("udo_dateofclaim"))
                {
                    reqBenefitClaimInput.mcs_dateOfClaim = entity.GetAttributeValue<DateTime>("udo_dateofclaim").ToString("MM/dd/yyyy");
                }

                reqBenefitClaimInput.mcs_disposition = "M";

                if (typecode.Attributes.Contains("udo_typecode"))
                {
                    var tmpTypeCode = typecode.GetAttributeValue<string>("udo_typecode").ToString();
                    reqBenefitClaimInput.mcs_endProduct = tmpTypeCode.Substring(0, 3);
                    reqBenefitClaimInput.mcs_endProductCode = tmpTypeCode;
                };

                reqBenefitClaimInput.mcs_endProductName = common.RetrieveValueFromEntityField(typecode, "udo_name");
                reqBenefitClaimInput.mcs_fileNumber = common.RetrieveValueFromEntityField(entity, "udo_filenumber");
                reqBenefitClaimInput.mcs_firstName = common.RetrieveValueFromEntityField(entity, "udo_firstname");
                reqBenefitClaimInput.mcs_folderWithClaim = "N";
                reqBenefitClaimInput.mcs_groupOneValidatedInd = "N";
                reqBenefitClaimInput.mcs_gulfWarRegistryPermit = common.RetrieveValueFromEntityField(entity, "udo_persiangulfwarserviceindicator");
                reqBenefitClaimInput.mcs_homelessIndicator = common.RetrieveValueFromEntityField(entity, "udo_homelessindicator");
                reqBenefitClaimInput.mcs_lastName = common.RetrieveValueFromEntityField(entity, "udo_lastname");
                reqBenefitClaimInput.mcs_middleName = common.RetrieveValueFromEntityField(entity, "udo_middlename");
                reqBenefitClaimInput.mcs_payee = common.RetrieveValueFromEntityField(payeecode, "udo_payeecode");


                if (entity.Attributes.Contains("udo_powdays"))
                {
                    reqBenefitClaimInput.mcs_powNumberOfDays = common.RetrieveValueFromEntityField(entity, "udo_powdays");
                }
                else
                {
                    reqBenefitClaimInput.mcs_powNumberOfDays = "0";
                }

                reqBenefitClaimInput.mcs_preDischargeIndicator = "N";
                reqBenefitClaimInput.mcs_ptcpntIdClaimant = common.RetrieveValueFromEntityField(entity, "udo_participantid");
                reqBenefitClaimInput.mcs_sectionUnitNo = common.RetrieveValueFromEntityField(entity, "udo_sectionunitno");
                reqBenefitClaimInput.mcs_soj = common.RetrieveValueFromEntityField(entity, "udo_soj");
                reqBenefitClaimInput.mcs_ssn = common.RetrieveValueFromEntityField(entity, "udo_ssn");

                reqBenefitClaimInput.mcs_submtrApplcnTypeCd = "VBMS";
                reqBenefitClaimInput.mcs_submtrRoleTypeCd = "VBA";
                reqBenefitClaimInput.mcs_bypassIndicator = "N";

                progressString = "creating VIMTinsertBenefitClaimRequest";
                var insertBenefitClaimRequest = new VEISinsertBenefitClaimRequest();

                insertBenefitClaimRequest.Debug = request.Debug;
                progressString = "insertBenefitClaimRequest.Debug";

                insertBenefitClaimRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo();
                progressString = "insertBenefitClaimRequest.LegacyServiceHeaderInfo";
                insertBenefitClaimRequest.LegacyServiceHeaderInfo.ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName;
                progressString = "insertBenefitClaimRequest.LegacyServiceHeaderInfo.ApplicationName";
                insertBenefitClaimRequest.LegacyServiceHeaderInfo.ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine;
                progressString = "insertBenefitClaimRequest.LegacyServiceHeaderInfo.ClientMachine";
                insertBenefitClaimRequest.LegacyServiceHeaderInfo.LoginName = request.LegacyServiceHeaderInfo.LoginName;
                progressString = "insertBenefitClaimRequest.LegacyServiceHeaderInfo.LoginName";
                insertBenefitClaimRequest.LegacyServiceHeaderInfo.StationNumber = request.LegacyServiceHeaderInfo.StationNumber;
                progressString = "insertBenefitClaimRequest.LegacyServiceHeaderInfo.StationNumber";

                insertBenefitClaimRequest.LogSoap = request.LogSoap;
                progressString = "insertBenefitClaimRequest.LegacyServiceHeaderInfo";
                insertBenefitClaimRequest.LogTiming = request.LogTiming;
                progressString = "insertBenefitClaimRequest.LogTiming";
                insertBenefitClaimRequest.OrganizationName = request.OrganizationName;
                progressString = "insertBenefitClaimRequest.OrganizationName";
                insertBenefitClaimRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                progressString = "insertBenefitClaimRequest.RelatedParentEntityName";
                insertBenefitClaimRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                progressString = "insertBenefitClaimRequest.RelatedParentFieldName";
                insertBenefitClaimRequest.RelatedParentId = request.RelatedParentId;
                progressString = "insertBenefitClaimRequest.RelatedParentId";
                insertBenefitClaimRequest.UserId = request.UserId;
                progressString = "insertBenefitClaimRequest.UserId";
                insertBenefitClaimRequest.MessageId = request.MessageId;
                progressString = "insertBenefitClaimRequest.MessageId";
                insertBenefitClaimRequest.VEISReqbenefitClaimInputBCS2Info = reqBenefitClaimInput;
                progressString = "insertBenefitClaimRequest.VIMTReqbenefitClaimInputBCS2Info";

                progressString = "calling VIMTinsertBenefitClaimRequest";

                // REM: Invoke VEIS WebApi
                // var insertBenefitClaimResponse = insertBenefitClaimRequest.SendReceive<VIMTinsertBenefitClaimResponse>(request.ProcessType);

                var insertBenefitClaimResponse = WebApiUtility.SendReceive<VEISinsertBenefitClaimResponse>(insertBenefitClaimRequest, WebApiType.VEIS);

                if (request.LogSoap || insertBenefitClaimResponse.ExceptionOccurred)
                {
                    if (insertBenefitClaimResponse.SerializedSOAPRequest != null || insertBenefitClaimResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = insertBenefitClaimResponse.SerializedSOAPRequest + insertBenefitClaimResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISinsertBenefitClaimRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "returned VEISinsertBenefitClaimRequest";

                response.MessageId = request.MessageId;
                response.ExceptionOccurred = insertBenefitClaimResponse.ExceptionOccurred;
                response.ExceptionMessage = insertBenefitClaimResponse.ExceptionMessage;

                if (insertBenefitClaimResponse.ExceptionOccurred == false)
                {
                    if (insertBenefitClaimResponse.VEISbenefitClaimRecordBCS2Info != null)
                    {
                        try
                        {
                            response.UDObenefitClaimRecordBCS2Info = responseExtractor.ExtractVEISResponse(insertBenefitClaimResponse.VEISbenefitClaimRecordBCS2Info);
                        }
                        catch (Exception e)
                        {
                            var stInfo = string.Empty;

                            var st = new StackTrace(e, true);
                            for (var i = 0; i < st.FrameCount; i++)
                            {
                                var sf = st.GetFrame(i);
                                stInfo = stInfo +
                                         string.Format("LOB Machine Name: {0}, Method: {1}, File: {2}, Line Number: {3}", System.Environment.MachineName, sf.GetMethod(), sf.GetFileName(),
                                             sf.GetFileLineNumber());
                                stInfo = stInfo + System.Environment.NewLine;
                            }
                        }

                        responseExtractor.UpdateCrmClaimEstablishment(insertBenefitClaimResponse.ExceptionOccured, insertBenefitClaimResponse.ExceptionMessage, request.ClaimEstablishmentId, response.UDObenefitClaimRecordBCS2Info, ClaimEstablishmentStatus.Inserted, organizationService);

                    }
                }
                else
                {
                    #region record Exception

                    var claimEntity = new Entity("udo_claimestablishment")
                    {
                        Id = request.ClaimEstablishmentId
                    };

                    claimEntity["udo_exceptionoccurred"] = response.ExceptionOccurred;
                    claimEntity["udo_exceptionmessage"] = response.ExceptionMessage;

                    organizationService.Update(claimEntity);

                    #endregion
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
                             string.Format("LOB Machine Name: {0}, Method: {1}, File: {2}, Line Number: {3}", System.Environment.MachineName, sf.GetMethod(), sf.GetFileName(),
                                 sf.GetFileLineNumber());
                    stInfo = stInfo + System.Environment.NewLine;
                }

                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method, executionException);

                response.ExceptionMessage = $"{method}: Exception Message: {executionException.Message} Execution Progress : {progressString}";
                response.ExceptionOccurred = true;
                response.StackTrace = stInfo;

                #region record Exception

                var claimEntity = new Entity("udo_claimestablishment")
                {
                    Id = request.ClaimEstablishmentId
                };

                claimEntity["udo_exceptionoccurred"] = response.ExceptionOccurred;
                claimEntity["udo_exceptionmessage"] = response.ExceptionMessage;

                organizationService.Update(claimEntity);

                #endregion

                if (claimEstablishmentExceptions.Count > 0) response.InnerExceptions = claimEstablishmentExceptions.ToArray();

                return response;
            }

            return response;
        }
    }
}