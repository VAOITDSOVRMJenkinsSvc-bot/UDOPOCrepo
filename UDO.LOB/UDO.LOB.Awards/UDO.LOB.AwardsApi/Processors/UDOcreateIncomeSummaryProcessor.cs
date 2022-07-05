using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using UDO.LOB.Awards.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Messages.ClaimantService;

namespace UDO.LOB.Awards.Processors
{
    internal class UDOcreateIncomeSummaryProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateIncomeSummaryProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateIncomeSummaryRequest request)
        {
            //var request = message as createIncomeSummaryRequest;
            UDOcreateIncomeSummaryResponse response = new UDOcreateIncomeSummaryResponse { MessageId = request.MessageId };
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
            string progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;
            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            #region connect to CRM
            CrmServiceClient OrgServiceProxy;
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion
            tLogger.LogEvent("Connect to CRM", "001");
            progressString = "After Connection";

            try
            {
                var findIncomeExpenseRequest = new VEISfincexpfindIncomeExpenseRequest();
                findIncomeExpenseRequest.LogTiming = request.LogTiming;
                findIncomeExpenseRequest.LogSoap = request.LogSoap;
                findIncomeExpenseRequest.Debug = request.Debug;
                findIncomeExpenseRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findIncomeExpenseRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findIncomeExpenseRequest.RelatedParentId = request.RelatedParentId;
                findIncomeExpenseRequest.UserId = request.UserId;
                findIncomeExpenseRequest.OrganizationName = request.OrganizationName;

                findIncomeExpenseRequest.mcs_ptcpntvetid = request.ptcpntVetId;
                findIncomeExpenseRequest.mcs_ptcpntbeneid = request.ptcpntBeneId;
                if (request.LegacyServiceHeaderInfo != null)
                {
                    findIncomeExpenseRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                progressString = "Before VEISfincexpfindIncomeExpense EC Call";

                //REM: WebApi Call tp VEIS
                var findIncomeExpenseResponse = WebApiUtility.SendReceive<VEISfincexpfindIncomeExpenseResponse>(findIncomeExpenseRequest, WebApiType.VEIS);
                if (request.LogSoap || findIncomeExpenseResponse.ExceptionOccurred)
                {
                    if (findIncomeExpenseResponse.SerializedSOAPRequest != null || findIncomeExpenseResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findIncomeExpenseResponse.SerializedSOAPRequest + findIncomeExpenseResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfincexpfindIncomeExpenseRequest Request/Response {requestResponse}", true);
                    }
                }

                tLogger.LogEvent("Web Service Call VEISfincexpfindIncomeExpense", "002");
                progressString = "After VEISfincexpfindIncomeExpense EC Call";

                response.ExceptionMessage = findIncomeExpenseResponse.ExceptionMessage;
                response.ExceptionOccured = findIncomeExpenseResponse.ExceptionOccurred;

                int incSumCount = 0;
                int incCount = 0;
                int expCount = 0;
                OrganizationRequestCollection requestCollection = new OrganizationRequestCollection();

                if (findIncomeExpenseResponse.VEISfincexpreturnInfo.VEISfincexpincomeSummaryRecordsInfo != null)
                {
                    var incomeSummaryRecord = findIncomeExpenseResponse.VEISfincexpreturnInfo.VEISfincexpincomeSummaryRecordsInfo;
                    foreach (var incomeSummaryRecordItem in incomeSummaryRecord)
                    {
                        UDOcreateIncomeSummaryMultipleResponse responseIds = new UDOcreateIncomeSummaryMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_incomesummary";
                        thisNewEntity["udo_name"] = "Income Summary";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }

                        if (!String.IsNullOrEmpty(incomeSummaryRecordItem.mcs_awardTypeCd))
                        {
                            thisNewEntity["udo_awardtype"] = incomeSummaryRecordItem.mcs_awardTypeCd;
                        }
                        if (!String.IsNullOrEmpty(incomeSummaryRecordItem.mcs_ptcpntBeneId))
                        {
                            thisNewEntity["udo_beneptcpntid"] = incomeSummaryRecordItem.mcs_ptcpntBeneId;
                        }
                        if (!String.IsNullOrEmpty(incomeSummaryRecordItem.mcs_effectiveDate))
                        {
                            DateTime newDateTime;
                            if (DateTime.TryParse(dateStringFormat(incomeSummaryRecordItem.mcs_effectiveDate), out newDateTime))
                            {
                                thisNewEntity["udo_effectivedate"] = newDateTime;
                            }
                        }
                        if (!String.IsNullOrEmpty(incomeSummaryRecordItem.mcs_familyIncomeInd))
                        {
                            thisNewEntity["udo_familyincome"] = incomeSummaryRecordItem.mcs_familyIncomeInd;
                        }
                        if (!String.IsNullOrEmpty(incomeSummaryRecordItem.mcs_familyNetWorthAmount))
                        {
                            thisNewEntity["udo_familynetworth"] = incomeSummaryRecordItem.mcs_familyNetWorthAmount;
                        }
                        if (!String.IsNullOrEmpty(incomeSummaryRecordItem.mcs_ivap))
                        {
                            thisNewEntity["udo_ivap"] = incomeSummaryRecordItem.mcs_ivap;
                        }
                        if (!String.IsNullOrEmpty(incomeSummaryRecordItem.mcs_ivmAdjustmentInd))
                        {
                            thisNewEntity["udo_ivmadjustment"] = incomeSummaryRecordItem.mcs_ivmAdjustmentInd;
                        }
                        if (!String.IsNullOrEmpty(incomeSummaryRecordItem.mcs_netWorthInd))
                        {
                            thisNewEntity["udo_networth"] = incomeSummaryRecordItem.mcs_netWorthInd;
                        }
                        if (!String.IsNullOrEmpty(incomeSummaryRecordItem.mcs_numberOfExpenseRecords))
                        {
                            thisNewEntity["udo_ofexpenserecords"] = incomeSummaryRecordItem.mcs_numberOfExpenseRecords;
                        }
                        if (!String.IsNullOrEmpty(incomeSummaryRecordItem.mcs_numberOfIncomeRecords))
                        {
                            thisNewEntity["udo_ofincomerecords"] = incomeSummaryRecordItem.mcs_numberOfIncomeRecords;
                        }
                        if (!String.IsNullOrEmpty(incomeSummaryRecordItem.mcs_potentialFraudInd))
                        {
                            thisNewEntity["udo_potentialfraud"] = incomeSummaryRecordItem.mcs_potentialFraudInd;
                        }
                        //if (!String.IsNullOrEmpty(incomeSummaryRecordItem.mcs_ptcpntVetId))
                        //{
                        //    thisNewEntity["udo_vetptcpntid"] = incomeSummaryRecordItem.mcs_ptcpntVetId;
                        //}
                        if (request.UDOcreateIncomeSummaryRelatedEntitiesInfo != null)
                        {
                            foreach (UDOcreateIncomeSummaryRelatedEntitiesMultipleRequest relatedItem in request.UDOcreateIncomeSummaryRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }

                        Guid newSummaryId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(request.MessageId, thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                        incSumCount += 1;


                        #region do income records for this summary
                        if (incomeSummaryRecordItem.VEISfincexpincomeRecordsInfo != null)
                        {
                            var incomeRecord = incomeSummaryRecordItem.VEISfincexpincomeRecordsInfo;
                            foreach (var incomeRecordItem in incomeRecord)
                            {
                                //instantiate the new Entity
                                Entity incomeEntity = new Entity();
                                incomeEntity.LogicalName = "udo_incomeexpense";
                                if (!String.IsNullOrEmpty(incomeRecordItem.mcs_annualAmount))
                                {
                                    incomeEntity["udo_annualamount"] = moneyStringFormat(incomeRecordItem.mcs_annualAmount);
                                }
                                if (!String.IsNullOrEmpty(incomeRecordItem.mcs_exclusionAmount))
                                {
                                    incomeEntity["udo_exclusionamount"] = moneyStringFormat(incomeRecordItem.mcs_exclusionAmount);
                                }
                                if (!String.IsNullOrEmpty(incomeRecordItem.mcs_exclusionTypeName))
                                {
                                    incomeEntity["udo_exclusiontype"] = incomeRecordItem.mcs_exclusionTypeName;
                                }
                                if (!String.IsNullOrEmpty(incomeRecordItem.mcs_firstName))
                                {
                                    incomeEntity["udo_firstname"] = incomeRecordItem.mcs_firstName;
                                }
                                if (!String.IsNullOrEmpty(incomeRecordItem.mcs_incomeTypeName))
                                {
                                    incomeEntity["udo_incometype"] = incomeRecordItem.mcs_incomeTypeName;
                                }
                                if (!String.IsNullOrEmpty(incomeRecordItem.mcs_lastName))
                                {
                                    incomeEntity["udo_lastname"] = incomeRecordItem.mcs_lastName;
                                }
                                if (!String.IsNullOrEmpty(incomeRecordItem.mcs_lastName))
                                {
                                    incomeEntity["udo_middlename"] = incomeRecordItem.mcs_lastName;
                                }

                                incomeEntity["udo_incomesummaryid"] = new EntityReference("udo_incomesummary", newSummaryId);
                                //add contact
                                if (request.UDOcreateIncomeSummaryRelatedEntitiesInfo != null)
                                {
                                    foreach (UDOcreateIncomeSummaryRelatedEntitiesMultipleRequest relatedItem in request.UDOcreateIncomeSummaryRelatedEntitiesInfo)
                                    {
                                        if (string.Compare(relatedItem.RelatedEntityName, "contact") == 0)
                                        {
                                            incomeEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        }
                                    }
                                }
                                //add owner
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    incomeEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }


                                incomeEntity["udo_typeofrecord"] = false;

                                CreateRequest createincomeEntityData = new CreateRequest
                                {
                                    Target = incomeEntity
                                };
                                requestCollection.Add(createincomeEntityData);
                                incCount += 1;
                            }
                        }
                        #endregion

                        #region do Expense records for this Summary
                        if (incomeSummaryRecordItem.VEISfincexpexpenseRecordsInfo != null)
                        {
                            var expenseRecord = incomeSummaryRecordItem.VEISfincexpexpenseRecordsInfo;
                            foreach (var expenseRecordItem in expenseRecord)
                            {

                                //instantiate the new Entity
                                Entity expenseEntity = new Entity();
                                expenseEntity.LogicalName = "udo_incomeexpense";

                                expenseEntity["udo_incomesummaryid"] = new EntityReference("udo_incomesummary", newSummaryId);
                                if (!String.IsNullOrEmpty(expenseRecordItem.mcs_annualAmount))
                                {
                                    expenseEntity["udo_annualamount"] = moneyStringFormat(expenseRecordItem.mcs_annualAmount);
                                }
                                if (!String.IsNullOrEmpty(expenseRecordItem.mcs_typeName))
                                {
                                    expenseEntity["udo_type"] = expenseRecordItem.mcs_typeName;
                                }
                                //add contact
                                if (request.UDOcreateIncomeSummaryRelatedEntitiesInfo != null)
                                {
                                    foreach (UDOcreateIncomeSummaryRelatedEntitiesMultipleRequest relatedItem in request.UDOcreateIncomeSummaryRelatedEntitiesInfo)
                                    {
                                        if (string.Compare(relatedItem.RelatedEntityName, "contact") == 0)
                                        {
                                            expenseEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        }
                                    }
                                }
                                // Set owner
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    expenseEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }

                                expenseEntity["udo_typeofrecord"] = true;

                                CreateRequest createexpenseEntityData = new CreateRequest
                                {
                                    Target = expenseEntity
                                };
                                requestCollection.Add(createexpenseEntityData);
                                expCount += 1;
                            }
                        }

                        #endregion

                    }
                }

                #region Create records

                if (incSumCount > 0)
                {

                    var result = new ExecuteMultipleHelperResponse();
                    result.IsFaulted = false;
                    result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);
                    tLogger.LogEvent("Execute Multiple Awards / Income and Expense", "003");
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

                string logInfo = string.Format("Income Summary Records Created: {0}, Income Records Created: {1}, Expense Records Created: {2} ", incSumCount, incCount, expCount);
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "Summary Records Created", logInfo, request.Debug);
                #endregion

                //added to generated code
                if (request.AwardId != null)
                {
                    Entity parent = new Entity();
                    parent.Id = request.AwardId;
                    parent.LogicalName = "udo_award";
                    parent["udo_incomesummarycomplete"] = true;

                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateIncomeSummaryProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Award Income Summary Data";

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
        private static string moneyStringFormat(string thisField)
        {
            string returnField = "";
            try
            {
                Double newValue = 0;
                if (Double.TryParse(thisField, out newValue))
                {
                    returnField = string.Format("{0:C}", newValue);
                }
                else
                {
                    returnField = "$0.00";
                }
            }
            catch (Exception ex)
            {
                returnField = ex.Message;
            }
            return returnField;

        }
    }
}