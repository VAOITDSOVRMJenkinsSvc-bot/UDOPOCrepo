using CuttingEdge.Conditions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDO.LOB.DependentMaintenance.Messages;
using UDO.LOB.Extensions.Logging;
//using VRM.Integration.Servicebus.Bgs.Messages;
using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Bgs.Services.AddressWebServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.ClaimantWebServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.PersonWebServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.PhoneWebServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.VetRecordWebServiceReference;

namespace VRM.Integration.Servicebus.AddDependent.Util
{
    public class Utils
    {
		public const int SpouseRelationship = 935950001;
		public const int ChildRelationship = 935950000;

		//public static int getOptionSetValue(IOrganizationService service, string optionSetString, string entityName, string attributeName)
		public static int getOptionSetValue(OrganizationServiceContext service, string optionSetString, string entityName, string attributeName)
        {
            try
            {
                int returnInt = 0;
                RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest();
                attributeRequest.EntityLogicalName = entityName;
                attributeRequest.LogicalName = attributeName;
                // Retrieve only the currently published changes, ignoring the changes that have
                // not been published.
                attributeRequest.RetrieveAsIfPublished = false;

                RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)service.Execute(attributeRequest);

                // Access the retrieved attribute.
                PicklistAttributeMetadata retrievedAttributeMetadata = (PicklistAttributeMetadata)attributeResponse.AttributeMetadata;
                for (int i = 0; i < retrievedAttributeMetadata.OptionSet.Options.Count; i++)
                {
                    if (retrievedAttributeMetadata.OptionSet.Options[i].Label.LocalizedLabels[0].Label == optionSetString)
                    {
                        returnInt = retrievedAttributeMetadata.OptionSet.Options[i].Value.Value;
                        break;
                    }

                }
                return returnInt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //catch (FaultException<OrganizationServiceFault> ex)
            //{
            //    _logger.setService = _service;
            //    _logger.setModule = "getOptionSetValue";
            //    _logger.WriteToFile(ex.Detail.Message);
            //    _logger.setModule = "execute";
            //    return 0;
            //}
            //catch (Exception ex)
            //{
            //    _logger.setService = _service;
            //    _logger.setModule = "getOptionSetValue";
            //    _logger.WriteToFile(ex.Message);
            //    _logger.setModule = "execute";
            //    return 0;
            //}
        }

        public static string getOptionSetString(OrganizationServiceContext service, OptionSetValue optionSetValue, string entityName, string attributeName)
        {
            try
            {

                string optionSetString = string.Empty;
                if (optionSetValue == null) return null;

                RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest();
                attributeRequest.EntityLogicalName = entityName;
                attributeRequest.LogicalName = attributeName;
                // Retrieve only the currently published changes, ignoring the changes that have
                // not been published.
                attributeRequest.RetrieveAsIfPublished = true;

                RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)service.Execute(attributeRequest);

                // Access the retrieved attribute.
                PicklistAttributeMetadata retrievedAttributeMetadata = (PicklistAttributeMetadata)attributeResponse.AttributeMetadata;
                for (int i = 0; i < retrievedAttributeMetadata.OptionSet.Options.Count; i++)
                {
                    if (retrievedAttributeMetadata.OptionSet.Options[i].Value == optionSetValue.Value)
                    {
                        optionSetString = retrievedAttributeMetadata.OptionSet.Options[i].Label.LocalizedLabels[0].Label;
                        break;
                    }

                }
                return optionSetString;
            }
            catch (Exception ex) {
                throw ex;
            }
            //catch (FaultException<OrganizationServiceFault> ex)
            //{
            //    _logger.setService = _service;
            //    _logger.setModule = "getOptionSetString";
            //    _logger.WriteToFile(ex.Detail.Message);
            //    _logger.setModule = "execute";
            //    return null;
            //}
            //catch (Exception ex)
            //{
            //    _logger.setService = _service;
            //    _logger.setModule = "getOptionSetString";
            //    _logger.WriteToFile(ex.Message);
            //    _logger.setModule = "execute";
            //    return null;
            }

		public static string GetSensitivityLevel(string organizationName, IOrganizationService organizationService, Guid userId, string ssn, long? participantId)
		{
			shrinq2Person corpRecord = null;
			if (!string.IsNullOrEmpty(ssn))
			{
				//CSDev
				//corpRecord = Bgs.Utils.GetCorporateRecordByFileNumber(organizationName, organizationService, userId, ssn);
				corpRecord = GetCorporateRecordByFileNumber(organizationName, organizationService, userId, ssn);

				//The service will return a record with nulls if a record is not found,  Check key values and null out record if not there.
				if (string.IsNullOrEmpty(corpRecord.fileNumber) && string.IsNullOrEmpty(corpRecord.ssn)) corpRecord = null;
			}

			if (corpRecord == null && participantId != null && participantId > 0)
			{
				//CSDev
				//veteranRecord vetRecord = Bgs.Utils.GetVeteranByParticipantId(organizationName, organizationService, userId, (long)participantId);
				veteranRecord vetRecord = GetVeteranByParticipantId(organizationName, organizationService, userId, (long)participantId);

				corpRecord = vetRecord.vetCorpRecord;
				//The service will return a record with nulls if a record is not found,  Check key values and null out record if not there.
				if (string.IsNullOrEmpty(corpRecord.fileNumber) && string.IsNullOrEmpty(corpRecord.ssn)) corpRecord = null;
			}
			string sensitivityLevel = null;

			if (corpRecord != null)
			{
				sensitivityLevel = corpRecord.sensitiveLevelOfRecord;
			}

			return sensitivityLevel;
		}

		public static shrinq2Person GetCorporateRecordByFileNumber(string organizationName, IOrganizationService organizationService, Guid userId, string ssn)
		{
			var veteranWebService = BgsServiceFactory.GetVetRecordService(organizationName, organizationService, userId);

			findCorporateRecordByFileNumberRequest request = new findCorporateRecordByFileNumberRequest(ssn);

			findCorporateRecordByFileNumberResponse response = veteranWebService.findCorporateRecordByFileNumber(request);

			return response.@return;

		}

		public static veteranRecord GetVeteranByParticipantId(string organizationName, IOrganizationService organizationService, Guid userId, long participantId)
		{
			var veteranWebService = BgsServiceFactory.GetVetRecordService(organizationName, organizationService, userId);

			findVeteranByPtcpntIdRequest request = new findVeteranByPtcpntIdRequest(participantId);

			findVeteranByPtcpntIdResponse response = veteranWebService.findVeteranByPtcpntId(request);

			return response.@return;
		}

		public static string GetVeteranParticipantId(string organizationName, string ssn, IOrganizationService organizationService, Guid userId)
		{

			DateTime methodStartTime, wsStartTime;
			string method = "GetVeteranParticipantId", webService = "findVeteran";

			//CSDev
			//Guid methodLoggingId = LogHelper.StartTiming(organizationName, ConfigFieldName, userId,
				//Guid.Empty, null, null, method, null, out methodStartTime);

			var veteranRecordInput = new veteranRecordInput
			{
				ssn = ssn
			};

			var request = new findVeteranRequest(veteranRecordInput);

			var veteranService = BgsServiceFactory.GetVetRecordService(organizationName,
				organizationService,
				userId);

			//CSDev
			//Guid wsLoggingId = LogHelper.StartTiming(organizationName, ConfigFieldName, userId,
				  //Guid.Empty, null, null, method, webService, out wsStartTime);

			var veteran = veteranService.findVeteran(request);

			//CSDev
			//LogHelper.EndTiming(wsLoggingId, organizationName, ConfigFieldName, userId, wsStartTime);

			Condition.Requires(veteran, "veteran").IsNotNull();

			var participantId = veteran.@return.vetCorpRecord.ptcpntId;

			if (!string.IsNullOrEmpty(participantId))
			{
				//CSDev
				//LogHelper.EndTiming(methodLoggingId, organizationName, ConfigFieldName, userId, methodStartTime);

				return participantId;
			}

			var veteranFileNumberRequest = new findVeteranByFileNumberRequest(ssn);

			webService = "findVeteranByFileNumber";

			//CSDev
			//wsLoggingId = LogHelper.StartTiming(organizationName, ConfigFieldName, userId,
				  //Guid.Empty, null, null, method, webService, out wsStartTime);

			var veteranFileNumberResponse = veteranService.findVeteranByFileNumber(veteranFileNumberRequest);

			//CSDev
			//LogHelper.EndTiming(wsLoggingId, organizationName, ConfigFieldName, userId, wsStartTime);

			Condition.Requires(veteranFileNumberResponse, "veteranFileNumberResponse").IsNotNull();

			participantId = veteranFileNumberResponse.@return.vetCorpRecord.ptcpntId;

			//CSDev
			//LogHelper.EndTiming(methodLoggingId, organizationName, ConfigFieldName, userId, methodStartTime);

			return participantId;
		}
        ////rajul testing
        //public static shrinq3Person[] findDependentsByPtcpntId(string organizationName, string participantId, IOrganizationService organizationService, Guid userId)
        //{

        //    shrinq3Person[] returnVal = null;

        //    var claimantWebService = BgsServiceFactory.GetClaimantWebServiceReference(organizationName, organizationService, userId);

        //    findDependentsByPtcpntIdRequest request = new findDependentsByPtcpntIdRequest(participantId);

        //    findDependentsByPtcpntIdResponse response = claimantWebService.findDependentsByPtcpntId(request);

        //    if (response.@return != null && response.@return.persons != null)
        //    {
        //        returnVal = response.@return.persons;
        //    }

        //    return returnVal;
        //}
        ////end rajul testing
        public static shrinq6Person[] GetAllRelationships(string organizationName, string participantId, IOrganizationService organizationService, Guid userId)
		{

			shrinq6Person[] returnVal = null;

			var claimantWebService = BgsServiceFactory.GetClaimantWebServiceReference(organizationName, organizationService, userId);

			findAllRelationshipsRequest request = new findAllRelationshipsRequest(participantId);

			findAllRelationshipsResponse response = claimantWebService.findAllRelationships(request);

			if (response.@return != null && response.@return.dependents != null)
			{
				returnVal = response.@return.dependents;
			}

			return returnVal;
		}

        //rajul 
        public static personDTO GetBirthInformation(string organizationName, long participantId, IOrganizationService organizationService, Guid userId)
        {
            personDTO returnVal = null;
            var personWebService = BgsServiceFactory.GetPersonWebService(organizationName, organizationService, userId);
            findPersonByPtcpntIdRequest request = new findPersonByPtcpntIdRequest(participantId);
            findPersonByPtcpntIdResponse response = personWebService.findPersonByPtcpntId(request);

            if (response.PersonDTO != null)
            {
                returnVal = response.PersonDTO;
            }
            return returnVal;
        }

        public static Bgs.Services.AddressWebServiceReference.ptcpntAddrsDTO GetAddressInformation(string organizationName, long participantId, IOrganizationService organizationService, Guid userId)
        {
            Bgs.Services.AddressWebServiceReference.ptcpntAddrsDTO returnVal = null;

            var addressWebService = BgsServiceFactory.GetAddressWebServiceReference(organizationName, organizationService, userId);
            string addressType = "Mailing";
            findPtcpntAddrsRequest request = new findPtcpntAddrsRequest(participantId, addressType);
            findPtcpntAddrsResponse response = addressWebService.findPtcpntAddrs(request);

            if (response.@return != null)
            {
                returnVal = response.@return;
            }

            return returnVal;

        }

        public static Bgs.Services.PhoneWebServiceReference.ptcpntPhoneDTO[] GetPhoneInformation(string organizationName, long participantId, IOrganizationService organizationService, Guid userId)
        {
            Bgs.Services.PhoneWebServiceReference.ptcpntPhoneDTO[] returnVal = null;

            var phoneWebService = BgsServiceFactory.GetPhoneWebServiceReference(organizationName, organizationService, userId);

            findAllPtcpntPhoneByPtcpntIdRequest request = new findAllPtcpntPhoneByPtcpntIdRequest(participantId);
            findAllPtcpntPhoneByPtcpntIdResponse response = phoneWebService.findAllPtcpntPhoneByPtcpntId(request);

            if (response.@return != null)
            {
                returnVal = response.@return;
            }

            return returnVal;

        }
        //end rajul

        public static GetMaritalInfoMultipleResponse MapShrink6PersonToGetMaritalInfoMultipleResponse(shrinq6Person person)
		{
			GetMaritalInfoMultipleResponse response = new GetMaritalInfoMultipleResponse();

			response.crme_SpouseSSN = person.ssn;
			response.crme_LastName = person.lastName;
			response.crme_FirstName = person.firstName;
			response.crme_DOB = person.dateOfBirth;
			response.crme_Country = "";
			response.crme_City = "";
			response.crme_State = "";
			response.crme_RelationshipBeginDate = person.relationshipBeginDate;
			response.crme_RelationshipEndDate = person.relationshipEndDate;
			response.crme_AwardInd = person.awardInd;
            
			return response;
		}

		public static DateTime? ConvertDateMMddyyyy(string date)
		{
			DateTime dt;
			DateTime? retDate = null;

			if (DateTime.TryParseExact(date, "MMddyyyy", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt))
			{
				retDate = dt;
			}

			return retDate;
		}

		public static GetDependentInfoMultipleResponse MapShrink6PersonToGetDependentInfoMultipleResponse(shrinq6Person person)
		{
			GetDependentInfoMultipleResponse response = new GetDependentInfoMultipleResponse();


			//Were not in the FindAllRelationships method
			//crme_SuffixName = person.suffixNm,
			//crme_TermnlDigitNbr = person.termnlDigitNbr

			response.crme_SSN = person.ssn;
			response.crme_LastName = person.lastName;
			response.crme_FirstName = person.firstName;
			response.crme_MiddleName = person.middleName;
			response.crme_DependentRelationship =
				Utils.DependentRelationshipConverter(person.relationshipType);
			response.crme_RelationshipBeginDate = person.relationshipBeginDate;
			response.crme_RelationshipEndDate = person.relationshipEndDate;
			response.crme_DOB = person.dateOfBirth;
			response.crme_AwardInd = person.awardInd;
          
            

			return response;
		}

		public static int DependentRelationshipConverter(string relationship)
		{
			return relationship.ToUpper().Trim() == "SPOUSE" ? SpouseRelationship : ChildRelationship;
		}
	}
}
