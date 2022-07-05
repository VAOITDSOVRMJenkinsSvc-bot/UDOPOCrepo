using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using VRM.IntegrationServicebus.AddDependent.CrmModel;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public static class DependentMaintenanceMappingExtensions
    {
        public static crme_dependentmaintenance MapVeteranInfo(this Messages.GetVeteranInfoMultipleResponse vaData,
            crme_dependentmaintenance crmData,
            OrganizationServiceContext context)
        {
			crmData.crme_ZIPPostalCodeId = CreateZipEntityReferenceIfNotExists(context,
                vaData.crme_ZIP,
                vaData.crme_State,
                vaData.crme_Country);
            crmData.crme_StateProvinceId = CreateStateEntityReferenceIfNotExists(context, 
                vaData.crme_State);
            crmData.crme_CountryId = CreateCountryEntityReferenceIfNotExists(context,
                vaData.crme_Country);
            crmData.crme_VAFileNumber = vaData.crme_VAFileNumber;
            crmData.crme_StoredSSN = vaData.crme_StoredSSN;
            crmData.crme_SSN = vaData.crme_SSN;
 
            crmData.crme_ParticipantID = vaData.crme_ParticipantID;
            crmData.crme_MiddleName = vaData.crme_MiddleName;
            crmData.crme_LastName = vaData.crme_LastName;
            crmData.crme_FirstName = vaData.crme_FirstName;
            crmData.crme_Email = vaData.crme_Email;
            crmData.crme_EDIPI = vaData.crme_EDIP;
            crmData.crme_DOB = vaData.crme_DOB;
            crmData.crme_DatafromApplication = vaData.crme_DataFromApplication;
            crmData.crme_City = vaData.crme_City;
            crmData.crme_Address3 = vaData.crme_Address3;
            crmData.crme_Address2 = vaData.crme_Address2;
            crmData.crme_Address1 = vaData.crme_Address1;
            crmData.crme_ZIPPlus4 = vaData.crme_ZipPlus4;
            //crmData.crme_MaritalStatus = null; 
            crmData.ScheduledEnd = DateTime.Now + new TimeSpan(5, 0, 0, 0); //Today + 5 Days - Set Scheduled End Date So Activity Shows up in the Grid.
            crmData.Subject = string.Format("{0}, {1} - " + "Dependent Maintenance", vaData.crme_LastName, vaData.crme_FirstName);

            crmData.crme_AddressType = vaData.crme_AddressType;
            crmData.crme_AllowPOAAccess = vaData.crme_AllowPOAAccess;
            crmData.crme_AllowPOACADD = vaData.crme_AllowPOACADD;
            crmData.crme_DayTimeAreaCode = vaData.crme_DayTimeAreaCode;
            crmData.crme_NightTimeAreaCode = vaData.crme_NightTimeAreaCode;
            crmData.crme_SecondaryPhone = vaData.crme_SecondaryPhone;
            crmData.crme_PrimaryPhone = vaData.crme_PrimaryPhone;
            crmData.crme_DayTimePhone = vaData.crme_DayTimeAreaCode + vaData.crme_PrimaryPhone;
            crmData.crme_NightTimePhone = vaData.crme_NightTimeAreaCode + vaData.crme_SecondaryPhone;

            crmData.crme_Title = new OptionSetValue(DependentMaintenanceMappingExtensions.getOptionSetValue(context, vaData.crme_Title, "crme_dependentmaintenance", "crme_title")); // vaData.crme_Title;
            crmData.crme_SuffixName = vaData.crme_SuffixName;

			return crmData;
        }



        public static DateTime? CrmDateTime(this DateTime dateTime)
        {
            DateTime? nullDate = null;

            return (dateTime > DateTime.MinValue.AddDays(1) && dateTime < DateTime.MaxValue) ? dateTime : nullDate;
        }

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
            //    _TracingService.Trace(ex.Detail.Message);
            //    _logger.setModule = "execute";
            //    return 0;
            //}
            //catch (Exception ex)
            //{
            //    _logger.setService = _service;
            //    _logger.setModule = "getOptionSetValue";
            //    _TracingService.Trace(ex.Message);
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
            catch (Exception ex)
            {
                throw ex;
            }
            //catch (FaultException<OrganizationServiceFault> ex)
            //{
            //    _logger.setService = _service;
            //    _logger.setModule = "getOptionSetString";
            //    _TracingService.Trace(ex.Detail.Message);
            //    _logger.setModule = "execute";
            //    return null;
            //}
            //catch (Exception ex)
            //{
            //    _logger.setService = _service;
            //    _logger.setModule = "getOptionSetString";
            //    _TracingService.Trace(ex.Message);
            //    _logger.setModule = "execute";
            //    return null;
        }
 

        public static crme_dependent MapDependentInfo(this Messages.GetDependentInfoMultipleResponse vaData,
            crme_dependentmaintenance dependentMaintenance,
            crme_dependent crmDependent,
            OrganizationServiceContext context)
		{
			crmDependent.crme_DependentMaintenance = new EntityReference(dependentMaintenance.LogicalName, dependentMaintenance.Id);

			crmDependent.crme_LegacyRecord = true;
			crmDependent.crme_City = vaData.crme_City;
			crmDependent.crme_ZIPPostalCodeId = CreateZipEntityReferenceIfNotExists(context,
				vaData.crme_Zip,
				vaData.crme_State,
				vaData.crme_Country);
			crmDependent.crme_CountryId = CreateCountryEntityReferenceIfNotExists(context, vaData.crme_Country);
			crmDependent.crme_StateProvinceId = CreateStateEntityReferenceIfNotExists(context, vaData.crme_State);
			crmDependent.crme_SSN = vaData.crme_SSN;
			crmDependent.crme_SpouseVAFileNumber = vaData.crme_SpouseVAFileNumber;
			crmDependent.crme_SpousePrimaryPhone = vaData.crme_SpousePrimaryPhone;
			crmDependent.crme_SpouseisVeteran = vaData.crme_SpouseisVeteran;
			crmDependent.crme_MonthlyContributiontoSpouseSupport = new Money{Value = vaData.crme_MonthlyContributiontoSpouseSupport};
			crmDependent.crme_MiddleName = vaData.crme_MiddleName;
			crmDependent.crme_MarriageStateId = CreateStateEntityReferenceIfNotExists(context, vaData.crme_MarriageState);
			crmDependent.crme_MarriageCountryId = CreateCountryEntityReferenceIfNotExists(context, vaData.crme_MarriageCountry);

			//CSDev
			//crmDependent.crme_MarriageDate = vaData.crme_MarriageDate.CrmDateTime();
			crmDependent.crme_MarriageDate = ValidateDateTime(vaData.crme_MarriageDate);

			crmDependent.crme_MarriageCity = vaData.crme_MarriageCity;
			crmDependent.crme_LiveswithSpouse = vaData.crme_LiveswithSpouse;
			crmDependent.crme_LastName = vaData.crme_LastName;
			crmDependent.crme_FirstName = vaData.crme_FirstName;

			//CSDEv is this the bad string?
			//crmDependent.crme_DOB = Convert.ToDateTime(vaData.crme_DOB);
			crmDependent.crme_DOB = ValidateDateTime(vaData.crme_DOB);

			crmDependent.crme_DependentStatus = new OptionSetValue(vaData.crme_DependentStatus == 0 ? 935950000 : vaData.crme_DependentStatus);
			crmDependent.crme_DependentRelationship = new OptionSetValue(vaData.crme_DependentRelationship == 0 ? 935950000 : vaData.crme_DependentRelationship);
			crmDependent.crme_County = vaData.crme_County;
			crmDependent.crme_MaintenanceType = new OptionSetValue(935950002);

			crmDependent.crme_City = vaData.crme_City;
			crmDependent.crme_ChildSeriouslyDisabled = vaData.crme_ChildSeriouslyDisabled;
			crmDependent.crme_ChildRelationship = new OptionSetValue(vaData.crme_ChildRelationship == 0 ? 935950000 : vaData.crme_ChildRelationship);
			crmDependent.crme_ChildPrimaryPhone = vaData.crme_ChildPrimaryPhone;
			crmDependent.crme_ChildPreviouslyMarried = vaData.crme_ChildPreviouslyMarried;
			crmDependent.crme_ChildPlaceofBirthStateId = GetStateEntityReference(vaData.crme_ChildPlaceofBirthState, context);
			crmDependent.crme_ChildPlaceOfBirthCountryId = GetCountryEntityReference(vaData.crme_ChildPlaceofBirthCountry, context);
			crmDependent.crme_ChildPlaceofBirthCity = vaData.crme_ChildPlaceofBirthCity;
			crmDependent.crme_ChildLiveswithMiddleName = vaData.crme_ChildLiveswithMiddleName;
			crmDependent.crme_ChildLiveswithLastName = vaData.crme_ChildLiveswithLastName;
			crmDependent.crme_ChildLiveswithFirstName = vaData.crme_ChildLiveswithFirstName;
			crmDependent.crme_ChildLiveswithVet = vaData.crme_ChildLiveswithVet;
			crmDependent.crme_ChildAge1823InSchool = vaData.crme_ChildAge1823InSchool;
			crmDependent.crme_Address3 = vaData.crme_Address3;
			crmDependent.crme_Address2 = vaData.crme_Address2;
			crmDependent.crme_Address1 = vaData.crme_Address1;
			crmDependent.crme_name = vaData.crme_name;

			//CSdev
			//crmDependent.crme_RelationshipBeginDate = vaData.crme_RelationshipBeginDate;
			crmDependent.crme_RelationshipBeginDate = ValidateDateTime(vaData.crme_RelationshipBeginDate);

			//CSDev
			//crmDependent.crme_RelationshipEndDate = vaData.crme_RelationshipEndDate;
			crmDependent.crme_RelationshipEndDate = ValidateDateTime(vaData.crme_RelationshipEndDate);

			crmDependent.crme_AwardInd = string.IsNullOrEmpty(vaData.crme_AwardInd) != true ? vaData.crme_AwardInd.Substring(0, 1) : null;


			return crmDependent;
		}

		public static crme_dependent[] MapDependentInfo(this Messages.GetDependentInfoMultipleResponse[] vaData,
            IOrganizationService organizationService,
            crme_dependentmaintenance dependentMaintenance,
            OrganizationServiceContext context)
        {
            var dependents = new List<crme_dependent>();

            foreach (var crmDependent in 
                vaData.Select(vaDependent => vaDependent.MapDependentInfo(dependentMaintenance, 
                    new crme_dependent(),
                    context)))
            {
                organizationService.Create(crmDependent);

                dependents.Add(crmDependent);
            }

            return dependents.ToArray();
        }

        public static crme_maritalhistory MapMaritalHistoryInfo(this Messages.GetMaritalInfoMultipleResponse vaData,
            crme_dependentmaintenance dependentMaintenance,
            crme_maritalhistory crmMaritalhistory,
            OrganizationServiceContext context)
        {
            crmMaritalhistory.crme_DependentMaintenance = new EntityReference(dependentMaintenance.LogicalName, dependentMaintenance.Id);
            crmMaritalhistory.crme_StateProvinceId = GetStateEntityReference(vaData.crme_State, context);
            crmMaritalhistory.crme_spousessn = vaData.crme_SpouseSSN;
			
			//CSDev
            //crmMaritalhistory.crme_MarriageStartDate = vaData.crme_MarriageStartDate.CrmDateTime();
            crmMaritalhistory.crme_MarriageStartDate = ValidateDateTime(vaData.crme_MarriageStartDate);
			
			//CSDev
            //crmMaritalhistory.crme_MarriageEndDate = vaData.crme_MarriageEndDate.CrmDateTime();
            crmMaritalhistory.crme_MarriageEndDate = ValidateDateTime(vaData.crme_MarriageEndDate);

            crmMaritalhistory.crme_LastName = vaData.crme_LastName;
            crmMaritalhistory.crme_FirstName = vaData.crme_FirstName;
			
			//CSDev
            //crmMaritalhistory.crme_DOB = vaData.crme_DOB;
            crmMaritalhistory.crme_DOB = ValidateDateTime(vaData.crme_DOB);

            crmMaritalhistory.crme_CountryId = CreateCountryEntityReferenceIfNotExists(context, vaData.crme_Country); 
            crmMaritalhistory.crme_City = vaData.crme_City;
            crmMaritalhistory.crme_LegacyRecord = true;
            crmMaritalhistory.crme_VeteranSSN = dependentMaintenance.crme_SSN;
			
			//CSDev
            //crmMaritalhistory.crme_RelationshipBeginDate = vaData.crme_RelationshipBeginDate;
            crmMaritalhistory.crme_RelationshipBeginDate = ValidateDateTime(vaData.crme_RelationshipBeginDate);

			//CSDev
            //crmMaritalhistory.crme_RelationshipEndDate = vaData.crme_RelationshipEndDate;
            crmMaritalhistory.crme_RelationshipEndDate = ValidateDateTime(vaData.crme_RelationshipEndDate);

            crmMaritalhistory.crme_AwardInd = vaData.crme_AwardInd;

            return crmMaritalhistory;
        }

        public static crme_maritalhistory[] MapMaritalHistoryInfo(this Messages.GetMaritalInfoMultipleResponse[] vaData,
            IOrganizationService organizationService,
            crme_dependentmaintenance dependentMaintenance,
            OrganizationServiceContext context)
        {
            var spouses = new List<crme_maritalhistory>();

            foreach (var spouse in
                vaData.Select(vaSpouse => vaSpouse.MapMaritalHistoryInfo(dependentMaintenance,
                    new crme_maritalhistory(),
                    context)))
            {
                organizationService.Create(spouse);

                spouses.Add(spouse);
            }

            return spouses.ToArray();
        }

        public static void MapMaritalHistoryInfo(this crme_maritalhistory maritalHistory, crme_dependent dependent)
        {
            
            maritalHistory.crme_Dependent = new EntityReference(crme_dependent.EntityLogicalName, dependent.Id);
            maritalHistory.crme_DependentMaintenance = dependent.crme_DependentMaintenance;
            maritalHistory.crme_spousessn = dependent.crme_SSN;
            maritalHistory.crme_MarriageStartDate = dependent.crme_MarriageDate;
            maritalHistory.crme_LastName = dependent.crme_LastName;
            maritalHistory.crme_MiddleName = dependent.crme_MiddleName;
            maritalHistory.crme_FirstName = dependent.crme_FirstName;
            maritalHistory.crme_DOB = dependent.crme_DOB;
            maritalHistory.crme_CountryId = dependent.crme_MarriageCountryId;
            maritalHistory.crme_City = dependent.crme_MarriageCity;
            maritalHistory.crme_StateProvinceId = dependent.crme_MarriageStateId;
            maritalHistory.crme_startlocation = dependent.crme_marriagelocation;
            maritalHistory.crme_LegacyRecord = false;
        }

        public static
             bool IsNumber(string inputvalue)
        {
            var regex = new Regex("[^0-9]");

            return !regex.IsMatch(inputvalue);
        }

        public static EntityReference CreateZipEntityReferenceIfNotExists(OrganizationServiceContext context, 
            string zip, 
            string state, 
            string country)
        {
            if (string.IsNullOrEmpty(zip) ||
                string.IsNullOrEmpty(state) ||
                string.IsNullOrEmpty(country))
                return null;

            if (!IsNumber(zip))
                throw new Exception(string.Format("ZipCode [{0}] is not a valid value.", zip));

            EntityReference result;

            //State
            var stateEntityReference = CreateStateEntityReferenceIfNotExists(context, state);

            //Country
            var countryEntityReference = CreateCountryEntityReferenceIfNotExists(context, country);

            //Zip
            var zipEntity = GetZipEntity(context, zip);

            if (zipEntity == null)
            {
                //Create
                zipEntity = new crme_postalcodelookup
                {
                    crme_postalcode = zip,
                    crme_StateId = stateEntityReference,
                    crme_CountryId = countryEntityReference
                };

                context.AddObject(zipEntity);

                context.SaveChanges();

                result = GetZipEntityReference(zip, context);
            }
            else
            {
                //Update
                zipEntity.crme_StateId = stateEntityReference;
                zipEntity.crme_CountryId = countryEntityReference;

                context.SaveChanges();

                result = new EntityReference(crme_postalcodelookup
                    .EntityLogicalName, zipEntity.Id);
            }

            return result;
        }

        private static EntityReference CreateCountryEntityReferenceIfNotExists(OrganizationServiceContext context, string country)
        {
            if (string.IsNullOrEmpty(country))
                return null;

            EntityReference result;

            var countryEntity = GetCountryEntity(context, country);

            if (countryEntity == null)
            {
                countryEntity = new crme_countrylookup {crme_country = country};

                context.AddObject(countryEntity);

                context.SaveChanges();

                result = GetCountryEntityReference(country, context);
            }
            else
            {
                result = new EntityReference(crme_countrylookup
                    .EntityLogicalName, countryEntity.Id);
            }

            return result;
        }

        private static EntityReference CreateStateEntityReferenceIfNotExists(OrganizationServiceContext context, string state)
        {
            if (string.IsNullOrEmpty(state))
                return null;

            EntityReference result;

            var stateEntity = GetStateEntity(context, state);

            if (stateEntity == null)
            {
                stateEntity = new crme_stateorprovincelookup {crme_stateorprovince = state};

                context.AddObject(stateEntity);

                context.SaveChanges();

                result = GetStateEntityReference(state, context);
            }
            else
            {
                result = new EntityReference(crme_stateorprovincelookup
                    .EntityLogicalName, stateEntity.Id);
            }

            return result;
        }

        private static crme_countrylookup GetCountryEntity(OrganizationServiceContext context, string country)
        {
            var countryCode = (from d in context.CreateQuery<crme_countrylookup>()
                where d.crme_country == country
                select d).FirstOrDefault();

            return countryCode;
        }

        private static crme_stateorprovincelookup GetStateEntity(OrganizationServiceContext context, string state)
        {
            var stateCode = (from d in context.CreateQuery<crme_stateorprovincelookup>()
                where d.crme_stateorprovince == state
                select d).FirstOrDefault();

            return stateCode;
        }

        private static crme_postalcodelookup GetZipEntity(OrganizationServiceContext context, string zip)
        {
            var zipCode = (from d in context.CreateQuery<crme_postalcodelookup>()
                where d.crme_postalcode == zip
                select d).FirstOrDefault();

            return zipCode;
        }

        private static EntityReference GetZipEntityReference(string zip, OrganizationServiceContext context)
        {
            var zipCode = GetZipEntity(context, zip);

            return zipCode == null ? null : 
                new EntityReference(crme_postalcodelookup.EntityLogicalName, zipCode.Id);
        }

        private static EntityReference GetStateEntityReference(string state, OrganizationServiceContext context)
        {
            var stateCode = GetStateEntity(context, state);

            return stateCode == null ? null :
                new EntityReference(crme_stateorprovincelookup.EntityLogicalName, stateCode.Id);
        }

        private static EntityReference GetCountryEntityReference(string country, OrganizationServiceContext context)
        {
            var countryCode = GetCountryEntity(context, country);

            return countryCode == null ? null :
                new EntityReference(crme_countrylookup.EntityLogicalName, countryCode.Id);
        }

		private static DateTime? ValidateDateTime(string stringToParse)
		{
			DateTime tempDateTime;
			DateTime.TryParse(stringToParse, out tempDateTime);
			return tempDateTime.CrmDateTime();
		}
	}

    public class AddressEntityReferences
    {
        public EntityReference ZipEntityReference { get; set; }
        public EntityReference StateEntityReference { get; set; }
        public EntityReference CountryEntityReference { get; set; }
    }
}