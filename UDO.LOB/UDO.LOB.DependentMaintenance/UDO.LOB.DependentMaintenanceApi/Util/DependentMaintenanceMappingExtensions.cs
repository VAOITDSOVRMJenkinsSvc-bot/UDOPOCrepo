using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
//using VRM.Integration.Servicebus.AddDependent.Messages;
using VRM.IntegrationServicebus.AddDependent.CrmModel;
using VRM.Integration.Servicebus.AddDependent.Util;
using UDO.LOB.DependentMaintenance.Messages;
using UDO.LOB.DependentMaintenance;

namespace VRM.Integration.Servicebus.AddDependent
{
    public static class DependentMaintenanceMappingExtensions
    {
        private const int _Spouse = 935950001;
        private const int _Biological = 935950000;
        private const int _StepChild = 935950001;
        private const int _Adopted = 935950002;


        private const int _College = 752280000;
        private const int _HighSchool = 752280001;
        private const int _HomeSchool = 752280002;
        private const int _PostSecondary = 752280003;

        private const int _Death = 752280001;
        private const int _Divorce = 752280000;

        private const int _Add = 935950000;
        private const int _Edit = 752280000;
        private const int _Remove= 935950001;


        public static AddDependentRequest MapAddDependentRequest(this crme_dependentmaintenance dependentMaintenance)
        {
            return dependentMaintenance.MapAddDependentRequest(null);
        }

        public static AddDependentRequest MapAddDependentRequest(this crme_dependentmaintenance dependentMaintenance,
            OrganizationServiceContext context)
        {
            var request = new AddDependentRequest
            {
                Veteran = dependentMaintenance.MapVeteranParticipant(context),
                Dependents = dependentMaintenance.MapDependents(context)
            };

            return request;
        }

        public static VeteranParticipant MapVeteranParticipant(this crme_dependentmaintenance dependentMaintenance,
            OrganizationServiceContext context)
        {
            var veteran = new VeteranParticipant
            {
                CorpParticipantId = long.Parse(dependentMaintenance.crme_ParticipantID),
                //FileNumber = dependentMaintenance.crme_StoredSSN,
                FileNumber = dependentMaintenance.crme_VAFileNumber,
                Ssn = dependentMaintenance.crme_StoredSSN,
                FirstName = dependentMaintenance.crme_FirstName,
                LastName = dependentMaintenance.crme_LastName,
                BirthDate = DateTime.Parse(dependentMaintenance.crme_DOB),
                Addresses = dependentMaintenance.MapAddresses(context),
                PhoneNumbers = dependentMaintenance.MapPhoneNumbers(),

                MiddleName = dependentMaintenance.crme_MiddleName,
                //Need to Add These Fields to CRM for Veteran
                IsVetInd = true,
                BirthCityName = "", 
                BirthStateCode = "",
                BirthCountryName = "",
                IsSeriouslyDisabled = false,
                IsScholdChild = false,
                EverMarriedInd = GetEverMarriedInd(dependentMaintenance.crme_MaritalStatus) ? "Y" : "N",
                MaritalStatus = GetMaritalStatus(dependentMaintenance.crme_MaritalStatus),
                AddresssType = dependentMaintenance.crme_AddressType,
                AllowPoaAccess = dependentMaintenance.crme_AllowPOAAccess,
                AllowPoaCadd = dependentMaintenance.crme_AllowPOACADD,
                EmailAddress = dependentMaintenance.crme_Email,
                //postalCodePlus4 = dependentMaintenance.crme_ZIPPlus4,
                SuffixName = dependentMaintenance.crme_SuffixName,
                TitleName = Utils.getOptionSetString(context, dependentMaintenance.crme_Title,"crme_dependentmaintenance","crme_title")
            };

            return veteran;
        }

        public static Address MapAddress(this crme_dependentmaintenance dependentMaintenance, 
            OrganizationServiceContext context)
        {
            var veteranAddress = new Address
            {
                EffectiveDate = DateTimeExtensions.TodayNoon,
                AddressLine1 = dependentMaintenance.crme_Address1,
                AddressLine2 = dependentMaintenance.crme_Address2,
                AddressLine3 = dependentMaintenance.crme_Address3,
                City = dependentMaintenance.crme_City,
                State = GetState(dependentMaintenance.crme_StateProvinceId, context),
                Country = GetCountry(dependentMaintenance.crme_CountryId, context),
                County = "",
                ZipCode = GetZip(dependentMaintenance.crme_ZIPPostalCodeId, context),
                ZipPlus4 = dependentMaintenance.crme_ZIPPlus4,
                //Title = dependentMaintenance.crme_Title,
                Title = Utils.getOptionSetString(context, dependentMaintenance.crme_Title, "crme_dependentmaintenance", "crme_title"),
                AddressTypeName = "Mailing",
                SharedAddressIndicator = false
            };

            return veteranAddress;
        }

        public static Address[] MapAddresses(this crme_dependentmaintenance dependentMaintenance, OrganizationServiceContext context)
        {
            var veteranAddresses = new List<Address>();

            var veteranAddress = dependentMaintenance.MapAddress(context);

            veteranAddresses.Add(veteranAddress);

            return veteranAddresses.ToArray();
        }

        public static PhoneNumber MapPrimaryPhoneNumber(this crme_dependentmaintenance dependentMaintenance)
        {
            if (string.IsNullOrEmpty(dependentMaintenance.crme_PrimaryPhone))
                return null;

            var phoneNumber = new PhoneNumber
            {
                    EffectiveDate = DateTimeExtensions.TodayNoon,
                    Number = dependentMaintenance.crme_PrimaryPhone,
                    PhoneTypeName = "Daytime",
                    AreaCode = dependentMaintenance.crme_DayTimeAreaCode
                };

            return phoneNumber;
        }

        public static PhoneNumber MapSecondaryPhoneNumber(this crme_dependentmaintenance dependentMaintenance)
        {
            if (string.IsNullOrEmpty(dependentMaintenance.crme_SecondaryPhone))
                return null;

            var phoneNumber = new PhoneNumber
            {
                EffectiveDate = DateTimeExtensions.TodayNoon,
                Number = dependentMaintenance.crme_SecondaryPhone,
                PhoneTypeName = "Nighttime",
                AreaCode = dependentMaintenance.crme_NightTimeAreaCode
                
            };

            return phoneNumber;
        }

        public static PhoneNumber[] MapPhoneNumbers(this crme_dependentmaintenance dependentMaintenance)
        {
            var veteranPhoneNumbers = new List<PhoneNumber>();

            var primaryNumber = dependentMaintenance.MapPrimaryPhoneNumber();

            var secondaryNumber = dependentMaintenance.MapSecondaryPhoneNumber();

            if(primaryNumber != null)
                veteranPhoneNumbers.Add(primaryNumber);

            if (secondaryNumber != null)
                veteranPhoneNumbers.Add(secondaryNumber);

            return veteranPhoneNumbers.ToArray();
        }

        public static DependentParticipant[] MapDependents(this crme_dependentmaintenance dependentMaintenance, OrganizationServiceContext context)
        {
            //Get Dependents
            var dependents = (from d in context.CreateQuery<crme_dependent>()
                              where d.crme_DependentMaintenance.Id == dependentMaintenance.Id &&
                              (d.crme_LegacyRecord == false || (d.crme_LegacyRecord == true && d.crme_MaintenanceType.Value == 752280000 && d.crme_DependentRelationship.Value == 935950000) ||
                              (d.crme_LegacyRecord == true && d.crme_MaintenanceType.Value == 935950001 && d.crme_DependentRelationship.Value == 935950001))
                              select d);

            //var dependents = (from d in context.CreateQuery<crme_dependent>()
            //                  where d.crme_DependentMaintenance.Id == dependentMaintenance.Id &&
            //                  d.crme_LegacyRecord == false
            //                  select d);

            //Map Request
            var result = dependents.Select(dependent => 
                dependent.MapDependentParticipant(dependentMaintenance, context)).ToList();
            
            return result.ToArray();
        }

        public static DependentParticipant MapDependentParticipant(this crme_dependent dependent, 
            crme_dependentmaintenance veteran, 
            OrganizationServiceContext context)
        {
            var fileNumber = dependent.crme_SpouseVAFileNumber;

            //if (string.IsNullOrEmpty(fileNumber))
            //    fileNumber = dependent.crme_SSN;



            var dependentParticipant = new DependentParticipant
            { 
                FileNumber = fileNumber,
                Ssn = dependent.crme_SSN,
                FirstName = dependent.crme_FirstName,
                LastName = dependent.crme_LastName,
                BirthDate = dependent.crme_DOB.GetValueOrDefault(),
                DependentRelationship = dependent.MapDependentRelationship(context),
                Addresses = dependent.MapAddresses(veteran, context),
                PhoneNumbers = dependent.MapPhoneNumbers(),

                MiddleName = dependent.crme_MiddleName,
                //TitleName = Utils.getOptionSetString(context, dependent.crme_Title.Value, "crme_dependent", "crme_title"),

                NoSssnReasonTypeCd = string.IsNullOrEmpty(dependent.crme_SSN) ? "NSAS"  : null, 
                //Utils.getOptionSetString(context, dependent.crme_NoSSNReasonType.Value, "crme_dependent", "crme_NoSSNReasonType"),
            };

            switch (dependent.crme_MaintenanceType.Value)
            {
                case _Add:
                    dependentParticipant.MaintenanceType = "Add";
                    break;
                case _Remove:
                    dependentParticipant.MaintenanceType = "Remove";
                    break;
                case _Edit:
                    dependentParticipant.MaintenanceType = "Edit";
                    break;
                
                default:
                    dependentParticipant.MaintenanceType = " ";
                    break;
            }


            dependentParticipant.DepID = dependent.Id;

            //Dependent is Child
            if (dependent.crme_DependentRelationship.Value == 935950000)
            {
                dependentParticipant.IsSeriouslyDisabled = dependent.crme_ChildSeriouslyDisabled.GetValueOrDefault();
                dependentParticipant.IsScholdChild = dependent.crme_ChildAge1823InSchool.GetValueOrDefault();
                dependentParticipant.BirthCityName = dependent.crme_ChildPlaceofBirthCity;
                dependentParticipant.BirthStateCode = GetState(dependent.crme_ChildPlaceofBirthStateId, context);
                dependentParticipant.BirthCountryName = GetCountry(dependent.crme_ChildPlaceOfBirthCountryId, context);
                dependentParticipant.IsVetInd = false;
            }
            //Dependent is Spouse
            else
            {
                dependentParticipant.IsVetInd = dependent.crme_SpouseisVeteran.GetValueOrDefault();
            }

            
            if(dependent.crme_ChildAge1823InSchool.GetValueOrDefault() == true)//school age child then get the school and student info here
            {
                dependentParticipant.SchoolCode = dependent.udo_facilitycode;
                dependentParticipant.SchoolName = dependent.udo_schoolname;
                dependentParticipant.CourseName = dependent.udo_CourseName;
                dependentParticipant.SchoolAddressLine1 = dependent.udo_schooladdressline1;
                dependentParticipant.SchoolAddressCity = dependent.udo_schooladdresscity;
                dependentParticipant.SchoolAddressState = GetState(dependent.udo_schooladdstate, context);
                dependentParticipant.SchoolAddressZip = dependent.udo_schooladdresszip;
                dependentParticipant.CourseBeginDate = dependent.udo_coursebegindate.GetValueOrDefault();
                dependentParticipant.ExpectedStartDate = dependent.udo_expectedstartdate.GetValueOrDefault();
                dependentParticipant.ExpectedGradDate = dependent.udo_expectedgraduationdate.GetValueOrDefault();
                dependentParticipant.IsAttendedLastTerm = dependent.udo_attendedlastterm.GetValueOrDefault();
                dependentParticipant.IsPaidByDEA = dependent.udo_paidbydea.GetValueOrDefault();

                if(dependent.udo_paidbydea == true)
                {
                    dependentParticipant.AgencyName = dependent.udo_agencyname;
                    dependentParticipant.PaidTuitionStartDate = dependent.udo_datepaymentsbegan.GetValueOrDefault();
                }

                if(dependent.udo_otherassests is null)
                {
                    dependentParticipant.OtherAssests = 0;
                }
                else
                {
                    dependentParticipant.OtherAssests = dependent.udo_otherassests.Value;
                }

                if (dependent.udo_realestate is null)
                {
                    dependentParticipant.RealEstate = 0;
                }
                else
                {
                    dependentParticipant.RealEstate = dependent.udo_realestate.Value;
                }

                if (dependent.udo_savings is null)
                {
                    dependentParticipant.Savings = 0;
                }
                else
                {
                    dependentParticipant.Savings = dependent.udo_savings.Value;
                }

                if (dependent.udo_securities is null)
                {
                    dependentParticipant.Securities = 0;
                }
                else
                {
                    dependentParticipant.Securities = dependent.udo_securities.Value;
                }
                

                switch (dependent.udo_fulltimestudenttypecode.Value)
                {
                    case _College:
                        dependentParticipant.FullTimeStudentTypeCode = "College";
                        break;
                    case _HighSchool:
                        dependentParticipant.FullTimeStudentTypeCode = "HighSch";
                        break;
                    case _HomeSchool:
                        dependentParticipant.FullTimeStudentTypeCode = "HomeSch";
                        break;
                    case _PostSecondary:
                        dependentParticipant.FullTimeStudentTypeCode = "POSTSCNDY";
                        break;
                    default:
                        dependentParticipant.FullTimeStudentTypeCode = " ";
                        break;

                }
                if(dependent.udo_attendedlastterm.GetValueOrDefault() == true)
                {
                    dependentParticipant.PrevSchoolCode = dependent.udo_prevfacilitycode;
                    dependentParticipant.AttendedBeginDate = dependent.udo_attendedbegindate.GetValueOrDefault();
                    dependentParticipant.AttendedEndDate = dependent.udo_attendedenddate.GetValueOrDefault();
                    dependentParticipant.AttendedSchool = dependent.udo_attendedschool;
                    dependentParticipant.AttendedSchoolAddressLine1 = dependent.udo_attendedschooladdress1;
                    dependentParticipant.AttendedSchoolAddressCity = dependent.udo_attendedschoolcity;
                    dependentParticipant.AttendedSchoolAddressState = GetState(dependent.udo_attendedschstate, context);
                    dependentParticipant.AttendedSchoolAddressZip = dependent.udo_attendedschoolzip;
                    dependentParticipant.AttendedSessionsPerWeek = dependent.udo_attendedsessionsperweek.GetValueOrDefault();
                    dependentParticipant.AttendedHoursPerWeek = dependent.udo_attendedhoursperweek.GetValueOrDefault();
                }
            }



            return dependentParticipant;
        }


        private static void MapMaritalHistory(OrganizationServiceContext context, Guid dependentId, DependentRelationship dependentRelationship)
        {
            crme_maritalhistory maritalHistory = null;

            maritalHistory = (from m in context.CreateQuery<crme_maritalhistory>()
                                   where m.crme_Dependent.Id == dependentId
                                   select m).FirstOrDefault();

            if (maritalHistory != null)
            {
                if (maritalHistory.crme_MarriageEndDate.HasValue)
                {
                    dependentRelationship.EndDate = maritalHistory.crme_MarriageEndDate;
                    dependentRelationship.MarriageTerminationCityName = maritalHistory.crme_MarriageEndCity;
                    dependentRelationship.MarriageTerminationCountryName = GetCountry(maritalHistory.crme_MarriageEndCountryId, context);
                    dependentRelationship.MarriageTerminationStateCode = GetState(maritalHistory.crme_MarriageEndStateId, context);
                    dependentRelationship.MarriageTerminationTypeCode = Utils.getOptionSetString(context, maritalHistory.crme_HowMarriageWasTerminated, "crme_maritalhistory", "crme_howmarriagewasterminated"); 
                }
            }
            
        }


        public static DependentRelationship MapDependentRelationship(this crme_dependent dependent,
            OrganizationServiceContext context)
        {
            var dependentRelationship = new DependentRelationship();

            if (dependent.crme_DependentRelationship.Value == _Spouse)
            {
                dependentRelationship.RelationshipTypeName = "Spouse";

                dependentRelationship.FamilyRelationshipTypeName = "Spouse";

                dependentRelationship.BeginDate = dependent.crme_MarriageDate.GetValueOrDefault();

                dependentRelationship.LivesWithRelatedPerson = dependent.crme_LiveswithSpouse.GetValueOrDefault();

                dependentRelationship.MarriageCityName = dependent.crme_MarriageCity;

                dependentRelationship.MarriageCountryName = GetCountry(dependent.crme_MarriageCountryId, context);

                dependentRelationship.MarriageStateCode = GetState(dependent.crme_MarriageStateId, context);

                dependentRelationship.MonthlyContributionToSpouseSupport =
                    dependent.crme_MonthlyContributiontoSpouseSupport == null
                        ? 0
                        : dependent.crme_MonthlyContributiontoSpouseSupport.Value;
                
                //Dont need to map marital info anymore as the fields are moved to dependent record now. So mapping the fields to dependent record now.
                 //MapMaritalHistory(context, dependent.Id, dependentRelationship);
               if(dependent.udo_marriageenddate != null)
                {
                    dependentRelationship.EndDate = dependent.udo_marriageenddate.GetValueOrDefault();
                    dependentRelationship.MarriageTerminationCityName = dependent.udo_marriageendcity;
                    dependentRelationship.MarriageTerminationCountryName = GetCountry(dependent.udo_marriageendcountry, context);
                    dependentRelationship.MarriageTerminationStateCode = GetState(dependent.udo_marriageendstate, context);

                    if (dependent.udo_howwasmarriageterminated != null)
                    {
                        switch (dependent.udo_howwasmarriageterminated.Value)
                        {
                            case _Death:
                                dependentRelationship.MarriageTerminationTypeCode = "Death";
                                break;
                            case _Divorce:
                                dependentRelationship.MarriageTerminationTypeCode = "Divorce";
                                break;
                            default:
                                dependentRelationship.MarriageTerminationTypeCode = " ";
                                break;
                        }
                    }

                    //After mapping the marital info, check the end date to see if this is an ex-spouse.
                    if (dependentRelationship.EndDate.HasValue)
                    {
                        dependentRelationship.FamilyRelationshipTypeName = "Ex-Spouse";
                    }
                }
            }
            else
            {
                dependentRelationship.RelationshipTypeName = "Child";

                switch (dependent.crme_ChildRelationship.Value)
                {
                    case _Biological:
                        dependentRelationship.FamilyRelationshipTypeName = "Biological";
                        break;
                    case _StepChild:
                        dependentRelationship.FamilyRelationshipTypeName = "Stepchild";
                        break;
                    case _Adopted:
                        dependentRelationship.FamilyRelationshipTypeName = "Adopted Child";
                        break;
                    default:
                        dependentRelationship.FamilyRelationshipTypeName = "NoneOfTheAbove";
                        break;
                }

                dependentRelationship.BeginDate = dependent.crme_DOB.GetValueOrDefault();

                dependentRelationship.ChildPreviouslyMarried = dependent.crme_ChildPreviouslyMarried.GetValueOrDefault();

                dependentRelationship.LivesWithRelatedPerson = dependent.crme_ChildLiveswithVet.GetValueOrDefault();
            }

            return dependentRelationship;
        }

        public static Address MapAddress(this crme_dependent dependent, 
            crme_dependentmaintenance veteran, 
            OrganizationServiceContext context)
        {
            var veteranAddress = new Address
            {
                EffectiveDate = DateTimeExtensions.TodayNoon,
                AddressLine1 = dependent.crme_Address1,
                AddressLine2 = dependent.crme_Address2,
                AddressLine3 = dependent.crme_Address3,
                City = dependent.crme_City,
                State = GetState(dependent.crme_StateProvinceId, context),
                Country = GetCountry(dependent.crme_CountryId, context),
                County = dependent.crme_County,
                ZipCode = GetZip(dependent.crme_ZIPPostalCodeId, context),
                ZipPlus4 = dependent.crme_ZipPlus4,
                AddressTypeName = "Mailing",

                SharedAddressIndicator = dependent.crme_DependentRelationship.Value == _Spouse
                    ? dependent.crme_LiveswithSpouse.GetValueOrDefault()
                    : dependent.crme_ChildLiveswithVet.GetValueOrDefault()
            };

            if (!veteranAddress.SharedAddressIndicator) 
                return veteranAddress;

            veteranAddress.AddressLine1 = veteran.crme_Address1;
            veteranAddress.AddressLine2 = veteran.crme_Address2;
            veteranAddress.AddressLine3 = veteran.crme_Address3;
            veteranAddress.City = veteran.crme_City;
            veteranAddress.State = GetState(veteran.crme_StateProvinceId, context);
            veteranAddress.Country = GetCountry(veteran.crme_CountryId, context);
            veteranAddress.County = "";
            veteranAddress.ZipCode = GetZip(veteran.crme_ZIPPostalCodeId, context);
            veteranAddress.ZipPlus4 = veteran.crme_ZIPPlus4;

            return veteranAddress;
        }

        public static Address[] MapAddresses(this crme_dependent dependent, 
            crme_dependentmaintenance veteran, 
            OrganizationServiceContext context)
        {
            var veteranAddresses = new List<Address>();

            var veteranAddress = dependent.MapAddress(veteran, context);

            veteranAddresses.Add(veteranAddress);

            return veteranAddresses.ToArray();
        }

        public static PhoneNumber MapPrimaryPhoneNumber(this crme_dependent dependent)
        {
            var phoneNumber = dependent.crme_DependentRelationship.Value == _Spouse
                ? dependent.crme_SpousePrimaryPhone
                : dependent.crme_ChildPrimaryPhone;

            if (string.IsNullOrEmpty(phoneNumber))
                return null;

            string areaCode = null;
            if (phoneNumber.Length == 10)
            {
                areaCode = phoneNumber.Substring(0, 3);
                phoneNumber = phoneNumber.Substring(3);
            }

            var result = new PhoneNumber
            {
                EffectiveDate = DateTimeExtensions.TodayNoon,
                Number = phoneNumber,
                AreaCode = areaCode,
                PhoneTypeName = "Daytime"
            };

            return result;
        }

        public static PhoneNumber[] MapPhoneNumbers(this crme_dependent dependent)
        {
            var veteranPhoneNumbers = new List<PhoneNumber>();

            var primaryNumber = dependent.MapPrimaryPhoneNumber();

            if (primaryNumber != null)
               veteranPhoneNumbers.Add(primaryNumber);
            
            return veteranPhoneNumbers.ToArray();
        }

        public static string GetZip(EntityReference entityReference, OrganizationServiceContext context)
        {
            if (entityReference == null)
                return string.Empty;

            var zipCode = (from d in context.CreateQuery<crme_postalcodelookup>()
                           where d.Id == entityReference.Id
                           select d).FirstOrDefault();

            return zipCode == null ? string.Empty : zipCode.crme_postalcode;
        }

        public static string GetState(EntityReference entityReference, OrganizationServiceContext context)
        {
            if (entityReference == null)
                return string.Empty;

            var stateCode = (from d in context.CreateQuery<crme_stateorprovincelookup>()
                             where d.Id == entityReference.Id
                             select d).FirstOrDefault();

            return stateCode == null ? string.Empty : stateCode.crme_stateorprovince;
        }

        public static string GetCountry(EntityReference entityReference, OrganizationServiceContext context)
        {
            if (entityReference == null)
                return string.Empty;

            var countryCode = (from d in context.CreateQuery<crme_countrylookup>()
                               where d.Id == entityReference.Id
                               select d).FirstOrDefault();

            return countryCode == null ? string.Empty : countryCode.crme_country;
        }

        public static string GetMaritalStatus(OptionSetValue optionSet)
        {
            if (optionSet == null)
                return string.Empty;

            switch (optionSet.Value)
            {
                case 935950000:
                    return "Married";
                case 935950001:
                    return "Widowed";
                case 935950002:
                    return "Divorced";
                case 935950003:
                    return "Separated";
                case 935950004:
                    return "Never";
                default:
                    return string.Empty;
            }
        }

        public static bool GetEverMarriedInd(OptionSetValue optionSet)
        {
            if (optionSet == null)
                return false;

            return optionSet.Value != 935950004;
        }
    }
}