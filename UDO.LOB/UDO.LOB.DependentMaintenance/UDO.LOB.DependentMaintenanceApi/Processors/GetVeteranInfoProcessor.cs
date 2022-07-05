using System;
using System.Collections.Generic;
//using CRM007.CRM.SDK.Core;
using CuttingEdge.Conditions;
//using VRM.Integration.Servicebus.Bgs.Messages;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Bgs.Services.VetRecordWebServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.ClaimantWebServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.AddressWebServiceReference;
//using VRM.Integration.Servicebus.CRM.SDK.Core;
//HERE 
//using IMessageBase = VRM.Integration.Servicebus.Core.IMessageBase;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//CSDEV NEW Comment 
//using VRM.Integration.Servicebus.Core;

using UDO.LOB.DependentMaintenance.Messages;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.AddDependent.Util;
using UDO.LOB.Core;
using VEIS.Core.Wcf;

//CSdev
//namespace VRM.Integration.Servicebus.Bgs
namespace UDO.LOB.DependentMaintenance.Processors
{
    public class GetVeteranInfoProcessor
    {
        public IMessageBase Execute(GetVeteranInfoRequest message)
        {
			//CSDEV 
			//Logger.Instance.Debug("Calling GetVeteranInfoProcessor");
			//LogHelper.LogInfo("Calling GetVeteranInfoProcessor | Execute");
			//LogHelper.LogDebug(message.crme_OrganizationName, msg.debug, message.crme_UserId, $"{ this.GetType().FullName}"
			//	, $"| DDD VVV Start {this.GetType().FullName}.Execute | TESTING DEBUG | Debug Value: " + message.crme_debug.ToString());

			LogHelper.LogDebug(message.crme_OrganizationName, message.crme_debug, message.crme_UserId, $"{ this.GetType().FullName}"
				, $"| DDD VVV Start {this.GetType().FullName}.Execute");

			//CSDEv Re Add SoapLogs
			SoapLog.Current.Active = true;

			DateTime methodStartTime, wsStartTime;
            string method = "GetVeteranInfoProcessor", webService = "findVeteran";

			//CSdev
			//Guid methodLoggingId  = LogHelper.StartTiming(message.crme_OrganizationName, Utils.ConfigFieldName, message.crme_UserId,
                //Guid.Empty, null, null, method, null, out methodStartTime);

            var response = new GetVeteranInfoResponse();

            var veterans = new List<GetVeteranInfoMultipleResponse>();

            var veteranRecordInput = new veteranRecordInput
            {
                ssn = message.crme_SSN
            };
       
            var request = new findVeteranRequest(veteranRecordInput);
            
            Condition.Requires(message.crme_OrganizationName, "message.crme_OrganizationName").IsNotNullOrEmpty();

            var crmConnection = ConnectToCrmHelper.ConnectToCrm(message.crme_OrganizationName);

            var service = BgsServiceFactory.GetVetRecordService(message.crme_OrganizationName,
                crmConnection,
                message.crme_UserId);

			//CSdev
			//Guid wsLoggingId = LogHelper.StartTiming(message.crme_OrganizationName, Utils.ConfigFieldName, message.crme_UserId,
                //Guid.Empty, null, null, method, webService, out wsStartTime);

            var veteran = service.findVeteran(request);

			//CSdev
			//LogHelper.EndTiming(wsLoggingId, message.crme_OrganizationName, Utils.ConfigFieldName, message.crme_UserId, wsStartTime);

            Condition.Requires(veteran, "veteran").IsNotNull();

            var veteranInfo = new GetVeteranInfoMultipleResponse();

            shrinq2Person vetCorpRecord = veteran.@return.vetCorpRecord;

            if (String.IsNullOrEmpty(veteran.@return.vetCorpRecord.ssn))
            {
				//If no SSN search by Filenumber
                var veteranFileNumberRequest = new findVeteranByFileNumberRequest(message.crme_SSN);

                webService  = "findVeteranByFileNumber";
				//CSdev
				//wsLoggingId = LogHelper.StartTiming(message.crme_OrganizationName, Utils.ConfigFieldName, message.crme_UserId,
					//Guid.Empty, null, null, method, webService, out wsStartTime);

				var veteranFileNumberResponse = service.findVeteranByFileNumber(veteranFileNumberRequest);

				//CSdev
				//LogHelper.EndTiming(wsLoggingId, message.crme_OrganizationName, Utils.ConfigFieldName, message.crme_UserId, wsStartTime);


                Condition.Requires(veteranFileNumberResponse, "veteranFileNumberResponse").IsNotNull();

				//CSDev IF Check because the return value from findbyfilenumber is null sometimes
				Condition.Requires(veteranFileNumberResponse.@return.vetCorpRecord, "veteranFileNumberResponse.@return.vetCorpRecord").IsNotNull();
				vetCorpRecord = veteranFileNumberResponse.@return.vetCorpRecord;
				
            }
            veteranInfo.crme_ZIP = vetCorpRecord.zipCode;
            veteranInfo.crme_VAFileNumber = vetCorpRecord.fileNumber;
            veteranInfo.crme_StoredSSN = vetCorpRecord.ssn;
     
            veteranInfo.crme_SSN = vetCorpRecord.ssn;

            //veteranInfo.crme_NightTimeAreaCode = vetCorpRecord.areaNumberTwo;
            //veteranInfo.crme_SecondaryPhone = vetCorpRecord.phoneNumberTwo;

            //veteranInfo.crme_DayTimeAreaCode = vetCorpRecord.areaNumberOne;
            //veteranInfo.crme_PrimaryPhone = vetCorpRecord.phoneNumberOne;

            veteranInfo.crme_ParticipantID = vetCorpRecord.ptcpntId;
            veteranInfo.crme_MiddleName = vetCorpRecord.middleName;
            veteranInfo.crme_LastName = vetCorpRecord.lastName;
            veteranInfo.crme_FirstName = vetCorpRecord.firstName;
            veteranInfo.crme_Email = vetCorpRecord.emailAddress;
            veteranInfo.crme_EDIP = ""; //Missing Value
            veteranInfo.crme_DOB = vetCorpRecord.dateOfBirth;
            veteranInfo.crme_DataFromApplication = true;
    
            //veteranInfo.crme_ZipPlus4 = vetCorpRecord.??
            veteranInfo.crme_SuffixName = vetCorpRecord.suffixName;
            veteranInfo.crme_Title = vetCorpRecord.salutationName;

            if ((!string.IsNullOrEmpty(vetCorpRecord.phoneTypeNameOne)) && (vetCorpRecord.phoneTypeNameOne.Equals("Daytime", StringComparison.InvariantCultureIgnoreCase)))
            {
                veteranInfo.crme_DayTimeAreaCode = vetCorpRecord.areaNumberOne;
                veteranInfo.crme_PrimaryPhone = vetCorpRecord.phoneNumberOne;
            }
            else if ((!string.IsNullOrEmpty(vetCorpRecord.phoneTypeNameTwo)) && (vetCorpRecord.phoneTypeNameTwo.Equals("Daytime", StringComparison.InvariantCultureIgnoreCase)))
            {
                veteranInfo.crme_DayTimeAreaCode = vetCorpRecord.areaNumberTwo;
                veteranInfo.crme_PrimaryPhone = vetCorpRecord.phoneNumberTwo;
            }

            if ((!string.IsNullOrEmpty(vetCorpRecord.phoneTypeNameOne)) && (vetCorpRecord.phoneTypeNameOne.Equals("Nighttime", StringComparison.InvariantCultureIgnoreCase)))
            {
                veteranInfo.crme_NightTimeAreaCode = vetCorpRecord.areaNumberOne;
                veteranInfo.crme_SecondaryPhone = vetCorpRecord.phoneNumberOne;
            }
            else if ((!string.IsNullOrEmpty(vetCorpRecord.phoneTypeNameTwo)) && (vetCorpRecord.phoneTypeNameTwo.Equals("Nighttime", StringComparison.InvariantCultureIgnoreCase)))
            {
                veteranInfo.crme_NightTimeAreaCode = vetCorpRecord.areaNumberTwo;
                veteranInfo.crme_SecondaryPhone = vetCorpRecord.phoneNumberTwo;
            }


            //Get Mailing address
            var addrService = BgsServiceFactory.GetAddressWebServiceReference(message.crme_OrganizationName, crmConnection, message.crme_UserId);

            findPtcpntAddrsRequest addrReq = new findPtcpntAddrsRequest { ptcpntId = long.Parse(veteranInfo.crme_ParticipantID), ptcpntAddrsTypeNm = "Mailing" };

            webService = "findPtcpntAddrs";

			//CSdev
			//wsLoggingId = LogHelper.StartTiming(message.crme_OrganizationName, Utils.ConfigFieldName, message.crme_UserId, Guid.Empty, null, null, method, webService, out wsStartTime);
            
            findPtcpntAddrsResponse addResp = addrService.findPtcpntAddrs(addrReq);

			//CSdev
			//LogHelper.EndTiming(wsLoggingId, message.crme_OrganizationName, Utils.ConfigFieldName, message.crme_UserId, wsStartTime);

            veteranInfo.crme_Country = addResp.@return.cntryNm;
            veteranInfo.crme_City = addResp.@return.cityNm;
            veteranInfo.crme_Address1 = addResp.@return.addrsOneTxt;
            veteranInfo.crme_Address2 = addResp.@return.addrsTwoTxt;
            veteranInfo.crme_Address3 = addResp.@return.addrsThreeTxt;
            veteranInfo.crme_AddressType = "Mailing";
            veteranInfo.crme_State = addResp.@return.postalCd;
            veteranInfo.crme_ZIP = addResp.@return.zipPrefixNbr;
            veteranInfo.crme_ZipPlus4 = addResp.@return.zipFirstSuffixNbr;


            var poaService = BgsServiceFactory.GetClaimantWebServiceReference(message.crme_OrganizationName, crmConnection, message.crme_UserId);

            findPOARequest poaRequest = new findPOARequest() { fileNumber = veteranInfo.crme_VAFileNumber };

            webService = "findPOA";

			//CSdev
			//wsLoggingId = LogHelper.StartTiming(message.crme_OrganizationName, Utils.ConfigFieldName, message.crme_UserId,
				//sGuid.Empty, null, null, method, webService, out wsStartTime);
            
            findPOAResponse poaResponse = poaService.findPOA(poaRequest);

			//CSdev
			//sLogHelper.EndTiming(wsLoggingId, message.crme_OrganizationName, Utils.ConfigFieldName, message.crme_UserId, wsStartTime);

            if (poaResponse.@return != null)
            {
                if (!String.IsNullOrEmpty(poaResponse.@return.authznPoaAccessInd))
                {
                    veteranInfo.crme_AllowPOAAccess = poaResponse.@return.authznPoaAccessInd;
                }

                if (!String.IsNullOrEmpty(poaResponse.@return.authznChangeClmantAddrsInd))
                {
                    veteranInfo.crme_AllowPOACADD = poaResponse.@return.authznChangeClmantAddrsInd;
                }
            }


            veterans.Add(veteranInfo);

            response.GetVeteranInfo = veterans.ToArray();

			//CSDEv ReAdding Soap Logs 
			response.SoapLog = SoapLog.Current.Log;
			SoapLog.Current.Active = false;
			SoapLog.Current.ClearLog();

			//CSdev
			//sLogHelper.EndTiming(methodLoggingId, message.crme_OrganizationName, Utils.ConfigFieldName, message.crme_UserId, methodStartTime);

			LogHelper.LogDebug(message.crme_OrganizationName, message.crme_debug, message.crme_UserId, $"{ this.GetType().FullName}"
				, $"| DDD VVV END {this.GetType().FullName}.Execute");

			return response;
        }
    }
}