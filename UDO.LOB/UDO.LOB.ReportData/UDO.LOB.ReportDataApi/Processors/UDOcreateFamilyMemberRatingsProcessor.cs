﻿using CRM007.CRM.SDK.Core;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using System;
using System.Collections.Generic;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Ratings.Messages;
using VRM.Integration.Servicebus.Logging.CRM.Util;
 using Logger = VRM.Integration.Servicebus.Core.Logger;
	/// <summary>
	/// VIMT LOB Component for UDOUDOcreateFamilyMemberRatings,UDOcreateFamilyMemberRatings method, Processor.
	/// Code Generated by IMS on: 6/12/2015 1:55:52 PM
	/// Version: 2015.06.02
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
using VIMT.RatingWebService.Messages;
using VRM.Integration.Common;
namespace VRM.Integration.UDO.Ratings.Processors
{
	class UDOUDOcreateFamilyMemberRatingsProcessor 
	{
		public IMessageBase Execute(UDOcreateFamilyMemberRatingsRequest request)
		{
			//var request = message as UDOcreateFamilyMemberRatingsRequest;
			UDOcreateFamilyMemberRatingsResponse response = new UDOcreateFamilyMemberRatingsResponse();
			var progressString = "Top of Processor";

			if (request == null)
			{
				response.ExceptionMessage = "Called with no message";
				response.ExceptionOccured = true;
				return response;
			}

			Logger.Instance.Info(string.Format("Message Id:{0}, Type={2}, Recieved diagnostics message: {1}",
			request.MessageId,
			request.MessageId,
			GetType().FullName));

			OrganizationServiceProxy OrgServiceProxy;

			#region connect to CRM
			try
			{
				var CommonFunctions = new CRMCommonFunctions();

				OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

			}
			catch (Exception connectException)
			 {
				LogHelper.LogError(request.OrganizationName, "mcs_UDOcreateFamilyMemberRatings", request.UserId, "UDOUDOcreateFamilyMemberRatingsProcessor Processor, Progess:" + progressString, connectException);
				response.ExceptionMessage = "Failed to get CRMConnection";
				response.ExceptionOccured = true;
				return response;
			}
			#endregion

			progressString = "After Connection";

			try
			{
				// prefix = fnrtngdtfindRatingDataRequest();
				var findRatingDataRequest = new  VIMTfnrtngdtfindRatingDataRequest();
				findRatingDataRequest.LogTiming = request.LogTiming;
				findRatingDataRequest.LogSoap = request.LogSoap;
				findRatingDataRequest.Debug = request.Debug;
				findRatingDataRequest.RelatedParentEntityName = request.RelatedParentEntityName;
				findRatingDataRequest.RelatedParentFieldName = request.RelatedParentFieldName;
				findRatingDataRequest.RelatedParentId = request.RelatedParentId;
				findRatingDataRequest.UserId = request.UserId;
				findRatingDataRequest.OrganizationName = request.OrganizationName;
				
				findRatingDataRequest.mcs_filenumber = request.fileNumber;
				
				var findRatingDataResponse = findRatingDataRequest.SendReceive<VIMTfnrtngdtfindRatingDataResponse>(MessageProcessType.Local);
				progressString = "After VIMT EC Call";
				
				response.ExceptionMessage =findRatingDataResponse.ExceptionMessage;
				response.ExceptionOccured = findRatingDataResponse.ExceptionOccured;
				if (findRatingDataResponse.!=null) {
						var responseIds = new UDOUDOcreateFamilyMemberRatingsMultipleResponse();
						//instantiate the new Entity
						Entity thisNewEntity = new Entity();
						thisNewEntity.LogicalName = "";
					if (findRatingDataResponse..mcs_beginDate!=string.Empty)
					{
						thisNewEntity["udo_begindate"]= findRatingDataResponse..mcs_beginDate;
					}
					if (findRatingDataResponse..mcs_decisionTypeName!=string.Empty)
					{
						thisNewEntity["udo_decision"]= findRatingDataResponse..mcs_decisionTypeName;
					}
					if (findRatingDataResponse..mcs_disabilityTypeName!=string.Empty)
					{
						thisNewEntity["udo_disability"]= findRatingDataResponse..mcs_disabilityTypeName;
					}
					if (findRatingDataResponse..mcs_endDate!=string.Empty)
					{
						thisNewEntity["udo_enddate"]= findRatingDataResponse..mcs_endDate;
					}
					if (findRatingDataResponse..mcs_familyMemberName!=string.Empty)
					{
						thisNewEntity["udo_familymembername"]= findRatingDataResponse..mcs_familyMemberName;
					}
					if (findRatingDataResponse..mcs_ratingDate!=string.Empty)
					{
						thisNewEntity["udo_ratingdate"]= findRatingDataResponse..mcs_ratingDate;
					}
					if (request.UDOUDOcreateFamilyMemberRatingsRelatedEntitiesInfo!=null){
						foreach (var relatedItem in request.UDOUDOcreateFamilyMemberRatingsRelatedEntitiesInfo)
						{
						    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
						}
					}
					responseIds.newUDOUDOcreateFamilyMemberRatingsId = OrgServiceProxy.Create(thisNewEntity);
					UDOUDOcreateFamilyMemberRatingsArray.Add(responseIds);
				}
				response.UDOUDOcreateFamilyMemberRatingsInfo =UDOUDOcreateFamilyMemberRatingsArray.ToArray();
				
			//added to generated code
			if (request.udo_ratingId != System.Guid.Empty)
			{
				var parent = new Entity();
				parent.Id = request.udo_ratingId;
				parent.LogicalName = "udo_rating";
				parent["udo_familymemberratingcomplete"] = true;
				 parent["udo_familymemberratingmessage"] = "";
				OrgServiceProxy.Update(parent);
			}
				return response;
			}
			catch (Exception connectException)
			 {
				LogHelper.LogError(request.OrganizationName, "udo_familymemberrating", request.UserId, "UDOUDOcreateFamilyMemberRatingsProcessor Processor, Progess:" + progressString, connectException);
				response.ExceptionMessage = "Failed to Map EC data to LOB";
				response.ExceptionOccured = true;
			if (request.udo_ratingId != System.Guid.Empty)
			{
				var parent = new Entity();
				parent.Id = request.udo_ratingId;
				parent.LogicalName = "udo_rating";
				 parent["udo_familymemberratingmessage"] = "";
				OrgServiceProxy.Update(parent);
			}
				return response;
			}
		}
	}
}
