﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using VIMT.BenefitClaimService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Claims.Messages;
using VRM.Integration.UDO.Common;
using Logger = VRM.Integration.Servicebus.Core.Logger;

/// <summary>
/// VIMT LOB Component for UDOcreateUDOLifecycles,createUDOLifecycles method, Processor.
/// Code Generated by IMS on: 5/29/2015 3:11:20 PM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.Claims.Processors
{
    class UDOcreateUDOLifecyclesProcessor
    {
        	private bool _debug { get; set; }
            private const string method = "UDOcreateUDOLifecyclesProcessor";
            string pclrPcanExplanation = null;
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateUDOLifecyclesRequest request)
        {
            //var request = message as createUDOLifecyclesRequest;
            UDOcreateUDOLifecyclesResponse response = new UDOcreateUDOLifecyclesResponse();
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
              LogHelper.LogError(request.OrganizationName, request.UserId, string.Format("{0} Processor, Connection Error", method), connectException.Message);
              response.ExceptionMessage = "Failed to get CRMConnection";
              response.ExceptionOccured = true;
              return response;
          }
          #endregion

          try
          {
              var findBenefitClaimDetailRequest = new VIMTfbendtlfindBenefitClaimDetailRequest();
              findBenefitClaimDetailRequest.Debug = request.Debug;
              findBenefitClaimDetailRequest.LogSoap = request.LogSoap;
              findBenefitClaimDetailRequest.LogTiming = request.LogTiming;
              findBenefitClaimDetailRequest.RelatedParentEntityName = request.RelatedParentEntityName;
              findBenefitClaimDetailRequest.RelatedParentFieldName = request.RelatedParentFieldName;
              findBenefitClaimDetailRequest.RelatedParentId = request.RelatedParentId;
              findBenefitClaimDetailRequest.UserId = request.UserId;
              findBenefitClaimDetailRequest.OrganizationName = request.OrganizationName;
              findBenefitClaimDetailRequest.LegacyServiceHeaderInfo = new VIMT.BenefitClaimService.Messages.HeaderInfo
              {
                  ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                  ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                  LoginName = request.LegacyServiceHeaderInfo.LoginName,

                  StationNumber = request.LegacyServiceHeaderInfo.StationNumber
              };
              //non standard fields
              findBenefitClaimDetailRequest.mcs_benefitclaimid = request.claimId.ToString();
              Logger.Instance.Info("Looking for benefitclaimdetail for claim:" + request.claimId.ToString());

                // TODO (TN): comment to remediate
                var findBenefitClaimDetailResponse = new VIMTfbendtlfindBenefitClaimDetailResponse();
                // var findBenefitClaimDetailResponse = findBenefitClaimDetailRequest.SendReceive<VIMTfbendtlfindBenefitClaimDetailResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";

              response.ExceptionMessage = findBenefitClaimDetailResponse.ExceptionMessage;
              response.ExceptionOccured = findBenefitClaimDetailResponse.ExceptionOccured;

              #region Create Lifecycles for each Claim
              //RC - eric - while you could do it here, you are delaying the creation of all claims.  I would move this down below just like you have evidence and other things
              //so you loop through the ID"s you need, but after claims are created.  The reason is because the claims is the first grid the user will see.
              if (findBenefitClaimDetailResponse.VIMTfbendtlbenefitClaimRecordbclmInfo.VIMTfbendtllifeCycleRecordbclmInfo.VIMTfbendtllifeCycleRecordsbclmInfo != null)
              {
                  var shrinqbcLifeCycle = findBenefitClaimDetailResponse.VIMTfbendtlbenefitClaimRecordbclmInfo.VIMTfbendtllifeCycleRecordbclmInfo.VIMTfbendtllifeCycleRecordsbclmInfo;

                  if (shrinqbcLifeCycle != null)
                  {
                      Logger.Instance.Info("Found lifecycles to process");
                      System.Collections.Generic.List<UDOcreateUDOLifecyclesMultipleResponse> UDOcreateUDOLifecyclesArray = new System.Collections.Generic.List<UDOcreateUDOLifecyclesMultipleResponse>();



                      progressString = "After Connection";
                      var responseIds = new UDOcreateUDOLifecyclesMultipleResponse();
                      var requestCollection = new OrganizationRequestCollection();
                      var lifeCycleCount = 0;
                      DateTime latestLifecycle = new DateTime(1);
                      #region create lifecycle records
                      foreach (var shrinqbcLifeCycleItem in shrinqbcLifeCycle)
                      {
                          var thisLifecycle = new Entity { LogicalName = "udo_lifecycle" };
                         
                          if (request.ownerId != System.Guid.Empty)
                          {
                              thisLifecycle["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                          }
                        
                          thisLifecycle["udo_lifecycle"] = "Life Cycle Summary";

                          if (shrinqbcLifeCycleItem.mcs_lifeCycleStatusTypeName != string.Empty)
                          {
                              thisLifecycle["udo_status"] = shrinqbcLifeCycleItem.mcs_lifeCycleStatusTypeName;
                          }
                          if (shrinqbcLifeCycleItem.mcs_statusReasonTypeName != string.Empty)
                          {
                              thisLifecycle["udo_pcanpclrreason"] = shrinqbcLifeCycleItem.mcs_statusReasonTypeName;
                          }
                          if (shrinqbcLifeCycleItem.mcs_stationofJurisdiction != string.Empty)
                          {
                              thisLifecycle["udo_claimstation"] = shrinqbcLifeCycleItem.mcs_stationofJurisdiction;
                          }
                          if (shrinqbcLifeCycleItem.mcs_changedDate != string.Empty)
                          {
                              DateTime newDateTime;
                              if (DateTime.TryParse(shrinqbcLifeCycleItem.mcs_changedDate, out newDateTime))
                              {
                                  thisLifecycle["udo_changedate"] = newDateTime;
                              }
                          }
                          if (shrinqbcLifeCycleItem.mcs_actionStationNumber != string.Empty)
                          {
                              thisLifecycle["udo_actionstation"] = shrinqbcLifeCycleItem.mcs_actionStationNumber;
                          }
                          if (shrinqbcLifeCycleItem.mcs_actionFirstName != string.Empty && shrinqbcLifeCycleItem.mcs_actionLastName != string.Empty && shrinqbcLifeCycleItem.mcs_actionMiddleName != string.Empty)
                          {
                              thisLifecycle["udo_actionperson"] = shrinqbcLifeCycleItem.mcs_actionLastName + ", " + shrinqbcLifeCycleItem.mcs_actionFirstName + " , " + shrinqbcLifeCycleItem.mcs_actionMiddleName;
                          }
                          if (shrinqbcLifeCycleItem.mcs_actionFirstName != string.Empty && shrinqbcLifeCycleItem.mcs_actionLastName != string.Empty)
                          {
                              thisLifecycle["udo_actionperson"] = shrinqbcLifeCycleItem.mcs_actionLastName + ", " + shrinqbcLifeCycleItem.mcs_actionFirstName ;
                          }
                          if (shrinqbcLifeCycleItem.mcs_benefitClaimID != string.Empty && shrinqbcLifeCycleItem.mcs_lifeCycleStatusTypeName != string.Empty)
                          {
                              thisLifecycle["udo_lifecycle"] = shrinqbcLifeCycleItem.mcs_benefitClaimID + " - " +
                                                                  shrinqbcLifeCycleItem.mcs_lifeCycleStatusTypeName;
                          }
                          if (shrinqbcLifeCycleItem.mcs_reasonText != string.Empty)
                          {
                              thisLifecycle["udo_explanation"] = shrinqbcLifeCycleItem.mcs_reasonText;
                              if (DateTime.Compare(DateTime.Parse(shrinqbcLifeCycleItem.mcs_changedDate), latestLifecycle) > 0)
                              {
                                  latestLifecycle = DateTime.Parse(shrinqbcLifeCycleItem.mcs_changedDate);
                                  pclrPcanExplanation = shrinqbcLifeCycleItem.mcs_reasonText;
                              }
                          }

                          if (request.UDOcreateUDOLifecyclesRelatedEntitiesInfo != null)
                          {
                              foreach (var relatedItem in request.UDOcreateUDOLifecyclesRelatedEntitiesInfo)
                              {
                                  thisLifecycle[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                              }
                          }
                          CreateRequest createExamData = new CreateRequest
                          {
                              Target = thisLifecycle
                          };
                          requestCollection.Add(createExamData);
                          lifeCycleCount += 1;

                         
                      }
                      #endregion

                      if (lifeCycleCount > 0)
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

                      string logInfo = string.Format("Number of LifeCycles Created: {0}", lifeCycleCount);
                      LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "LifeCycle Records Created", logInfo);
                  }
              }
              #endregion

              //added to generated code
              if (request.udo_claimId != System.Guid.Empty)
              {
                  var parent = new Entity();
                  parent.Id = request.udo_claimId;
                  parent.LogicalName = "udo_claim";
                  parent["udo_lifecyclecomplete"] = true;
                  if (pclrPcanExplanation != null)
                  { parent["udo_pclrpcanexplanation"] = pclrPcanExplanation; }
                  OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
              }
              return response;
          }
          catch (Exception ExecutionException)
          {
              LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
              response.ExceptionMessage = "Failed to process UDO Lifecycle data";
              response.ExceptionOccured = true;
              return response;
          }
        }
    }
}