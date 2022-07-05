using Microsoft.Crm.Sdk.Messages;
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
/// VIMT LOB Component for UDOcreateDependents,createDependents method, Processor.
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.Contact.Processors
{
    class UDOcreateDependentsProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOcreateDependentsProcessor";
        
        public IMessageBase Execute(UDOcreateDependentsRequest request)
        {
            UDOcreateDependentsResponse response = new UDOcreateDependentsResponse { MessageId = request?.MessageId };
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = $"{method} Called with no message";
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

            progressString = "Records removed, beginning new request:" + request.MessageId;
            LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Dependent Records Deleted", progressString);
            try
            {
                var findDependentsRequest = new VEISfedpfindDependentsRequest();
                findDependentsRequest.LogTiming = request.LogTiming;
                findDependentsRequest.LogSoap = request.LogSoap;
                findDependentsRequest.Debug = request.Debug;
                findDependentsRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findDependentsRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findDependentsRequest.RelatedParentId = request.RelatedParentId;
                findDependentsRequest.UserId = request.UserId;
                findDependentsRequest.OrganizationName = request.OrganizationName;

                findDependentsRequest.mcs_filenumber = request.fileNumber;
                if (request.LegacyServiceHeaderInfo != null)
                {
                    findDependentsRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }
                // REM: Invoke VEIS Endpoint
                var findDependentsResponse = WebApiUtility.SendReceive<VEISfedpfindDependentsResponse>(findDependentsRequest, WebApiType.VEIS);
                if (request.LogSoap || findDependentsResponse.ExceptionOccurred)
                {
                    if (findDependentsResponse.SerializedSOAPRequest != null || findDependentsResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findDependentsResponse.SerializedSOAPRequest + findDependentsResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfedpfindDependentsRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VEIS EC Call";

                response.ExceptionMessage = findDependentsResponse.ExceptionMessage;
                response.ExceptionOccured = findDependentsResponse.ExceptionOccurred;
                // Replaced: VIMTfedpreturnclmsInfo = VEISfedpreturnInfo
                if (findDependentsResponse.VEISfedpreturnInfo != null)
                {
                    var shrinq3Person = findDependentsResponse.VEISfedpreturnInfo.VEISfedppersonsInfo;
                    System.Collections.Generic.List<UDOcreateDependentsMultipleResponse> UDOcreateDependentsArray = new System.Collections.Generic.List<UDOcreateDependentsMultipleResponse>();
                    if (shrinq3Person != null)
                    {
                        foreach (var shrinq3PersonItem in shrinq3Person)
                        {
                            var responseIds = new UDOcreateDependentsMultipleResponse();
                            //instantiate the new Entity
                            Entity thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_dependant";
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            else
                            {
                                LogHelper.LogError(request.OrganizationName, request.UserId, method, "Create Dependents - No Owner");
                            }

                            thisNewEntity["udo_name"] = "Dependent Summary";
                            if (shrinq3PersonItem.mcs_ssnVerifyStatus != string.Empty)
                            {
                                thisNewEntity["udo_ssnverifiedstatus"] = shrinq3PersonItem.mcs_ssnVerifyStatus;
                            }
                            if (shrinq3PersonItem.mcs_ssn != string.Empty)
                            {
                                thisNewEntity["udo_ssn"] = shrinq3PersonItem.mcs_ssn;
                            }
                            if (shrinq3PersonItem.mcs_relationship != string.Empty)
                            {
                                thisNewEntity["udo_relationship"] = shrinq3PersonItem.mcs_relationship;
                            }
                            if (shrinq3PersonItem.mcs_relatedToVet != string.Empty)
                            {
                                thisNewEntity["udo_relatedtovet"] = shrinq3PersonItem.mcs_relatedToVet;
                            }
                            if (shrinq3PersonItem.mcs_ptcpntId != string.Empty)
                            {
                                thisNewEntity["udo_ptcpntid"] = shrinq3PersonItem.mcs_ptcpntId;
                            }
                            if (shrinq3PersonItem.mcs_proofOfDependency != string.Empty)
                            {
                                thisNewEntity["udo_proofofdependency"] = shrinq3PersonItem.mcs_proofOfDependency;
                            }
                            if (shrinq3PersonItem.mcs_middleName != string.Empty)
                            {
                                thisNewEntity["udo_middle"] = shrinq3PersonItem.mcs_middleName;
                            }
                            if (shrinq3PersonItem.mcs_lastName != string.Empty)
                            {
                                thisNewEntity["udo_last"] = shrinq3PersonItem.mcs_lastName;
                            }
                            if (shrinq3PersonItem.mcs_gender != string.Empty)
                            {
                                thisNewEntity["udo_gender"] = shrinq3PersonItem.mcs_gender;
                            }
                            if (shrinq3PersonItem.mcs_firstName != string.Empty)
                            {
                                thisNewEntity["udo_first"] = shrinq3PersonItem.mcs_firstName;
                            }
                            if (!string.IsNullOrEmpty(shrinq3PersonItem.mcs_dateOfBirth) && DateTime.TryParse(shrinq3PersonItem.mcs_dateOfBirth, out DateTime newDateTime))
                            {
                                thisNewEntity["udo_dob"] = newDateTime;
                            }
                            if (request.UDOcreateDependentsRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedItem in request.UDOcreateDependentsRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }

                            string name = string.Empty;

                            if (shrinq3PersonItem.mcs_firstName != string.Empty)
                            {
                                name = shrinq3PersonItem.mcs_firstName;
                            }
                            if (shrinq3PersonItem.mcs_middleName != string.Empty)
                            {
                                name = name + " " + shrinq3PersonItem.mcs_middleName;
                            }
                            if (shrinq3PersonItem.mcs_lastName != string.Empty)
                            {
                                name = name + " " + shrinq3PersonItem.mcs_lastName;
                            }
                            thisNewEntity["udo_name"] = name;
                            if (request.VeteranId != Guid.Empty)
                            {
                                thisNewEntity["udo_veteranid"] = new EntityReference("contact", request.VeteranId);
                            }

                            var existingDependent = RetrieveExistingDependent(thisNewEntity, OrgServiceProxy);

                            if (existingDependent == null)
                            {

                                CreateRequest createData = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createData);
                            }
                            else
                            {
                                var updatedEntity = UpdateTarget(existingDependent, thisNewEntity);
                                if (updatedEntity != null)
                                {
                                    UpdateRequest updateData = new UpdateRequest
                                    {
                                        Target = updatedEntity
                                    };
                                    requestCollection.Add(updateData);

                                    if (updatedEntity.Contains("ownerid"))
                                    {
                                        // TODO: REM: AssignRequest to be performed in the UpdateRequest
                                        //      Reference added to Microsoft.Crm.Sdk.Messages but this should be removed
                                        AssignRequest assignData = new AssignRequest();
                                        assignData.Assignee = updatedEntity.GetAttributeValue<EntityReference>("ownerid");
                                        assignData.Target = updatedEntity.ToEntityReference();

                                        requestCollection.Add(assignData);
                                    }
                                }
                            }

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

                        string logInfo = string.Format("Dependent Records Created/Updated: {0}", requestCollection.Count());
                        LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, method, logInfo);
                    }
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateDependentsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = $" {method}: Failed to Map EC data to LOB";
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

        private Entity UpdateTarget(Entity target, Entity source)
        {
            var updatedDependent = new Entity("udo_dependant");

            foreach (var attribute in source.Attributes)
            {
                var attName = attribute.Key;
                var sourceValue = attribute.Value;

                if (target.Contains(attName))
                {
                    var targetAttValue = target[attName];

                    if (sourceValue != null && sourceValue.GetType().Equals(typeof(DateTime)))
                    {
                        var sourceDate = Convert.ToDateTime(sourceValue);
                        var targetDate = Convert.ToDateTime(targetAttValue);

                        if (sourceDate == null || targetDate == null)
                        {
                            updatedDependent.Attributes.Add(attName, sourceValue);
                            continue;
                        }

                        var result = DateTime.Compare(sourceDate.Date, targetDate.Date);

                        if (result != 0)
                            updatedDependent.Attributes.Add(attName, sourceValue);
                    }
                    else if (!targetAttValue.Equals(sourceValue))
                        updatedDependent.Attributes.Add(attName, sourceValue);
                }
                else if (sourceValue != null)
                    updatedDependent.Attributes.Add(attName, sourceValue);
            }

            if (updatedDependent.Attributes.Count > 0)
            {
                updatedDependent.Id = target.Id;
                return updatedDependent;
            }
            else
                return null;
            
        }

        private Entity RetrieveExistingDependent(Entity thisEntity, IOrganizationService service)
        {
            var veteranRef = thisEntity.GetAttributeValue<EntityReference>("udo_veteranid");
            var dependentRef = thisEntity.GetAttributeValue<EntityReference>("udo_relateddependentid");

            var searchField = string.Empty;
            var searchId = Guid.Empty;

            if (dependentRef != null)
            {
                searchField = "udo_relateddependentid";
                searchId = dependentRef.Id;
            }
            else
            {
                searchField = "udo_veteranid";
                searchId = veteranRef.Id;
            }

            var fetchDependent = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                  <entity name='udo_dependant'>
                                                    <attribute name='udo_dependantid' />
                                                    <attribute name='udo_name' />
                                                    <attribute name='udo_ssnverifiedstatus' />
                                                    <attribute name='udo_ssn' />
                                                    <attribute name='udo_relationship' />
                                                    <attribute name='udo_relatedtovet' />
                                                    <attribute name='udo_ptcpntid' />
                                                    <attribute name='udo_proofofdependency' />
                                                    <attribute name='udo_middle' />
                                                    <attribute name='udo_last' />
                                                    <attribute name='udo_gender' />
                                                    <attribute name='udo_first' />
                                                    <attribute name='udo_dob' />
                                                    <attribute name='ownerid' />
                                                    <order attribute='udo_name' descending='false' />
                                                    <filter type='and'>
                                                      <condition attribute='udo_ptcpntid' operator='eq' value='{1}' />
                                                      <condition attribute='{0}' operator='eq' value='{2}' />
                                                    </filter>
                                                  </entity>
                                                </fetch>", searchField, thisEntity["udo_ptcpntid"], searchId);

            var response = service.RetrieveMultiple(new FetchExpression(fetchDependent));

            if (response.Entities.Count > 0)
            {
                //We have more than 1 dependant with same participant id. Extras should be cleaned. 
                if (response.Entities.Count > 1)
                {
                    //Pick one
                    var returnedRecord = response.Entities.FirstOrDefault();

                    foreach (var entity in response.Entities)
                    {
                        if (entity.Id != returnedRecord.Id)
                            service.Delete("udo_dependant", entity.Id);
                    }

                    return returnedRecord;
                }

                return response.Entities.FirstOrDefault();
            }
            else
                return null;
        }
    }
}