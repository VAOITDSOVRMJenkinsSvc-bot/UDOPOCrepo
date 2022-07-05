using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace UDO.Workflows
{
    public class GetHealthEnrollmentDetails : CodeActivity
    {

        [RequiredArgument]
        [Input("SSN")]
        public InArgument<string> SsnInput { get; set; }
        [RequiredArgument]
        [Input("IgnoreDuplicates")]
        public InArgument<bool> IgnoreDuplicates { get; set; }

        [RequiredArgument]
        [Input("FirstName")]
        public InArgument<string> FirstName { get; set; }

        [RequiredArgument]
        [Input("LastName")]
        public InArgument<string> LastName { get; set; }

        [RequiredArgument]
        [Input("DOB")]
        public InArgument<string> Dob { get; set; }

        [Output("IsDuplicate")]
        public OutArgument<bool> IsDuplicate { get; set; }

        [Output("IsSensitivityHigher")]
        public OutArgument<bool> IsSensitivityHigher { get; set; }

        [Output("ServiceConnectedPercentage")]
        public OutArgument<string> ServiceConnectedPercentage { get; set; }

        [Output("CombinedServiceConnectedPercentageEffectiveDate")]
        public OutArgument<string> CombinedServiceConnectedPercentageEffectiveDate { get; set; }

        [Output("EffectiveDate")]
        public OutArgument<DateTime> EffectiveDate { get; set; }

        [Output("EnrollmentCategory")]
        public OutArgument<string> EnrollmentCategory { get; set; }

        [Output("EnrollmentStatus")]
        public OutArgument<string> EnrollmentStatus { get; set; }

        [Output("EnrollmentDate")]
        public OutArgument<DateTime> EnrollmentDate { get; set; }


        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            //Get the Input Arguments
            string ssn = SsnInput.Get(executionContext);
            string firstName = FirstName.Get(executionContext);
            string lastName = LastName.Get(executionContext);
            string dob = Dob.Get(executionContext);
            bool ignoreDuplicates = IgnoreDuplicates.Get(executionContext);
            tracingService.Trace("Input arguments Obtained");
            tracingService.Trace("Building the query expression");
            try
            {
                string fetchXmlQuerytofindICN = $@"<fetch top='50' no-lock='true' >
              <entity name='crme_person' >
                <attribute name='crme_icn' />
                <attribute name='crme_fullname' />
                <filter type='and' >
                  <condition attribute='crme_ssn' operator='eq' value='{ssn}' />
                  <condition attribute='crme_isattended' operator='eq' value='true' />
                  <condition attribute='crme_firstname' operator='eq' value='{firstName}' />
                  <condition attribute='crme_lastname' operator='eq' value='{lastName}' />
                  <condition attribute='crme_dobstring' operator='eq' value='{dob}' />
                  <condition attribute='crme_searchtype' operator='eq' value='CombinedSearchByIdentifier' />
                </filter>
              </entity>
            </fetch>";
            string icn = string.Empty;
            tracingService.Trace("Retrieve matching crme_person");

            FetchExpression fetchExpression = new FetchExpression(fetchXmlQuerytofindICN);
            EntityCollection queryICNCollection = service.RetrieveMultiple(fetchExpression);
            if (queryICNCollection == null)
            {
                tracingService.Trace("No Matching records found");
                return;
            }
            if (queryICNCollection.Entities.Count > 1 && ignoreDuplicates == false) 
            {
                tracingService.Trace("Duplicate record found do not log. Set isDuplocate to true.");
                IsDuplicate.Set(executionContext, true);
                return;

            }
            IsDuplicate.Set(executionContext, false);
            if (queryICNCollection.Entities[0].Contains("crme_returnmessage"))
            {
                string returnMessage = queryICNCollection.Entities[0].GetAttributeValue<string>("crme_returnmessage");
                if (returnMessage.Contains("lower Sensitivity Level than CSS") || returnMessage.Contains("sensitive file - access violation"))
                {
                    tracingService.Trace("Sensitity level higher ---");
                    IsSensitivityHigher.Set(executionContext, true);
                }
            }
            IsSensitivityHigher.Set(executionContext, false);
            if (queryICNCollection.Entities[0].Contains("crme_icn"))
            {
               
                icn = queryICNCollection.Entities[0].GetAttributeValue<string>("crme_icn");
                if (string.IsNullOrEmpty(icn))
                {
                    tracingService.Trace("ICN Value is empty");
                    return;
                }
                foreach (var item in queryICNCollection.Entities[0].Attributes)
                {
                    tracingService.Trace($"These are the attributes {item.Key} and its values {item.Value} ");
                }
            }
            else
            {
                tracingService.Trace($"No Valid ICN , exiting the workflow");
                return;
            }

            tracingService.Trace("Begin API call to VEIS");
            tracingService.Trace($"THis is the value of ICN {icn}");
           
                var fetchSubscriptionIds = $@"<fetch>
                                              <entity name='mcs_setting' >
                                                <attribute name='udo_ocpapimsubscriptionkeysouth' />
                                                <attribute name='udo_ocpapimsubscriptionkeyeast' />
                                                <attribute name='udo_ocpapimsubscriptionkey' />
                                              </entity>
                                            </fetch>";

                FetchExpression fetchExp = new FetchExpression(fetchSubscriptionIds);
                EntityCollection subscriptionRes = service.RetrieveMultiple(fetchExp);

                string subscriptionKey = string.Empty;
                string subscriptionKeyE = string.Empty;
                string subscriptionKeyS = string.Empty;

                if (subscriptionRes.Entities[0].Contains("udo_ocpapimsubscriptionkey"))
                {
                    subscriptionKey = subscriptionRes.Entities[0].Attributes["udo_ocpapimsubscriptionkey"].ToString();
                }
                if (subscriptionRes.Entities[0].Contains("udo_ocpapimsubscriptionkeyeast"))
                {
                    subscriptionKeyE = subscriptionRes.Entities[0].Attributes["udo_ocpapimsubscriptionkeyeast"].ToString();
                }
                if (subscriptionRes.Entities[0].Contains("udo_ocpapimsubscriptionkeysouth"))
                {
                    subscriptionKeyS = subscriptionRes.Entities[0].Attributes["udo_ocpapimsubscriptionkeysouth"].ToString();
                }

                var fetchESREndPoint = $@"<fetch>
                                            <entity name='va_systemsettings' >
                                            <attribute name='va_description' />
                                            <filter>
                                                <condition attribute='va_name' operator='eq' value='ESREndPoint' />
                                            </filter>
                                            </entity>
                                        </fetch>";

                FetchExpression fetchESRExp = new FetchExpression(fetchESREndPoint);
                EntityCollection endPointRes = service.RetrieveMultiple(fetchESRExp);
                tracingService.Trace($"End point obtained");
                string endpoint = string.Empty;
                if (endPointRes.Entities[0].Contains("va_description"))
                {
                    endpoint = endPointRes.Entities[0].Attributes["va_description"].ToString().Replace("{0}", icn);
                    tracingService.Trace($"End point {endpoint}");
                }
                else
                {
                    tracingService.Trace("No end point to query");
                    return;
                }
                string uri = endpoint;
                HttpClient request = new HttpClient();
                request.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                request.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key-E", subscriptionKeyE);
                request.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key-S", subscriptionKeyS);
                HttpResponseMessage response = request.GetAsync(uri).GetAwaiter().GetResult();
                tracingService.Trace($"api called and response recieved {response.StatusCode}");
                if (response.IsSuccessStatusCode)
                {

                    var data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    tracingService.Trace($"This is the data --------{data}");

                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(data)))
                    {
                        //Desrialize data 
                        DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Root));
                        Root root = (Root)deserializer.ReadObject(ms);
                        if (root.ErrorOccurred == true)
                        {
                            tracingService.Trace("No data found cannot create log");
                            return;
                        }
                        string serviceConnectedPercentatge = string.Empty;
                        var combinedServiceConnectedPercentageEffectiveDate = string.Empty;
                        var effectiveDate = string.Empty;
                        var enrollmentCategory = string.Empty;
                        var enrollmentStatus = string.Empty;
                        var enrollmentDate = string.Empty;
                        DateTime effectDate = new DateTime();
                        DateTime enrollDate = new DateTime();
                        tracingService.Trace("Starting to deserialize");
                        if (root.Data.EnrollmentDeterminationInfo.ServiceConnectionAward.ServiceConnectedPercentage != null)
                        {
                            tracingService.Trace("Entering here");
                            serviceConnectedPercentatge = root.Data.EnrollmentDeterminationInfo.ServiceConnectionAward.ServiceConnectedPercentage.ToString();
                        }
                        tracingService.Trace($"service connected set ");
                        if (root.Data.EnrollmentDeterminationInfo.ServiceConnectionAward.CombinedServiceConnectedPercentageEffectiveDate != null)
                        {
                            combinedServiceConnectedPercentageEffectiveDate = root.Data.EnrollmentDeterminationInfo.ServiceConnectionAward.CombinedServiceConnectedPercentageEffectiveDate.ToString();
                        }
                      
                        if (root.Data.EnrollmentDeterminationInfo.EffectiveDate != null)
                        {
                            effectiveDate = root.Data.EnrollmentDeterminationInfo.EffectiveDate;
                            effectDate = DateTime.Parse(effectiveDate);

                        }
                        tracingService.Trace($"EffectiveDate set ");
                        if (root.Data.EnrollmentDeterminationInfo.EnrollmentCategoryName != null)
                        {
                            enrollmentCategory = root.Data.EnrollmentDeterminationInfo.EnrollmentCategoryName;
                        }
                        tracingService.Trace($"EnrollmentCategoryName set ");

                        if (root.Data.EnrollmentDeterminationInfo.EnrollmentStatus != null)
                        {
                            enrollmentStatus = root.Data.EnrollmentDeterminationInfo.EnrollmentStatus;
                        }
                        if (root.Data.EnrollmentDeterminationInfo.EnrollmentDate != null)
                        {
                            enrollmentDate = root.Data.EnrollmentDeterminationInfo.EnrollmentDate;
                            enrollDate = DateTime.Parse(enrollmentDate);
                        }

                        ServiceConnectedPercentage.Set(executionContext, serviceConnectedPercentatge);
                        CombinedServiceConnectedPercentageEffectiveDate.Set(executionContext, combinedServiceConnectedPercentageEffectiveDate);
                        EffectiveDate.Set(executionContext, effectDate);
                        EnrollmentCategory.Set(executionContext, enrollmentCategory);
                        EnrollmentStatus.Set(executionContext, enrollmentStatus);           
                        EnrollmentDate.Set(executionContext, enrollDate);
                        tracingService.Trace("Output Data set");

                    }

                }
            }
            catch (Exception ex)
            {
                tracingService.Trace(ex.Message);
                return;

            }

        }


        [DataContract()]
        public class Associations
        {
            [DataMember(IsRequired = false)]
            public object Association { get; set; }

        }
        [DataContract()]
        public class DeathRecond
        {
            [DataMember(IsRequired = false)]
            public object DataSource { get; set; }
            [DataMember(IsRequired = false)]
            public object DeathDate { get; set; }
            [DataMember(IsRequired = false)]
            public object DeathReportDate { get; set; }
            [DataMember(IsRequired = false)]
            public object FacilityReceived { get; set; }

        }
        [DataContract()]
        public class Address
        {
            [DataMember(IsRequired = false)]
            public string Line1 { get; set; }
            [DataMember(IsRequired = false)]
            public object Line2 { get; set; }
            [DataMember(IsRequired = false)]
            public object Line3 { get; set; }
            [DataMember(IsRequired = false)]
            public string City { get; set; }
            [DataMember(IsRequired = false)]
            public string State { get; set; }
            [DataMember(IsRequired = false)]
            public string ZipCode { get; set; }
            [DataMember(IsRequired = false)]
            public string ZipPlus4 { get; set; }
            [DataMember(IsRequired = false)]
            public object PostalCode { get; set; }
            [DataMember(IsRequired = false)]
            public string County { get; set; }
            [DataMember(IsRequired = false)]
            public object ProvinceCode { get; set; }
            [DataMember(IsRequired = false)]
            public string Country { get; set; }
            [DataMember(IsRequired = false)]
            public string AddressTypeCode { get; set; }
            [DataMember(IsRequired = false)]

            public string AddressChangeDateTime { get; set; }
            [DataMember(IsRequired = false)]

            public string AddressChangeDateDate { get; set; }
            [DataMember(IsRequired = false)]
            public string AddressChangeEffectiveDate { get; set; }
            [DataMember(IsRequired = false)]

            public string AddressChangeEffectiveDateDate { get; set; }
            [DataMember(IsRequired = false)]
            public string AddressChangeSite { get; set; }
            [DataMember(IsRequired = false)]
            public string AddressChangeSource { get; set; }
            [DataMember(IsRequired = false)]
            public object BadAddressReason { get; set; }
            [DataMember(IsRequired = false)]
            public List<object> ConfidentialAddressCategories { get; set; }
            [DataMember(IsRequired = false)]
            public string ContactMethodType { get; set; }
            [DataMember(IsRequired = false)]

            public string ContactMethodReportDate { get; set; }
            [DataMember(IsRequired = false)]
            public object EndDate { get; set; }
            [DataMember(IsRequired = false)]

            public string EndDateDate { get; set; }
            [DataMember(IsRequired = false)]
            public object PhoneNumber { get; set; }

        }
        [DataContract()]
        public class Addresses
        {
            public List<Address> Address { get; set; }

        }
        [DataContract()]
        public class ContactInfo
        {
            [DataMember(IsRequired = false)]
            public Addresses Addresses { get; set; }
            [DataMember(IsRequired = false)]
            public object Emails { get; set; }
            [DataMember(IsRequired = false)]
            public object Phones { get; set; }

        }
        [DataContract()]

        public class PreferredFacility
        {
            [DataMember(IsRequired = false)]

            public string AssignmentDate { get; set; }
            [DataMember(IsRequired = false)]
            public string PreferredFacilityName { get; set; }

        }
        [DataContract()]
        public class PreferredFacilities
        {
            [DataMember(IsRequired = false)]
            public List<PreferredFacility> PreferredFacility { get; set; }

        }
        [DataContract()]
        public class Demographics
        {
            [DataMember(IsRequired = false)]
            public ContactInfo ContactInfo { get; set; }
            [DataMember(IsRequired = false)]
            public object MaritalStatus { get; set; }
            [DataMember(IsRequired = false)]
            public string PreferredFacility { get; set; }
            [DataMember(IsRequired = false)]
            public PreferredFacilities PreferredFacilities { get; set; }
            [DataMember(IsRequired = false)]
            public object PreferredLanguage { get; set; }
            [DataMember(IsRequired = false)]
            public string Religion { get; set; }

        }
        [DataContract()]
        public class PrimaryEligibility
        {
            [DataMember(IsRequired = false)]

            public string EligibilityReportDate { get; set; }
            [DataMember(IsRequired = false)]
            public string Indicator { get; set; }
            [DataMember(IsRequired = false)]
            public string Type { get; set; }

        }
        [DataContract()]
        public class ServiceConnectionAward
        {
            [DataMember(IsRequired = false)]
            public object AwardDate { get; set; }
            [DataMember(IsRequired = false)]
            public string CombinedServiceConnectedPercentageEffectiveDate { get; set; }
            [DataMember(IsRequired = false)]
            public object PermanentAndTotalEffectiveDate { get; set; }
            [DataMember(IsRequired = false)]
            public string PermanentAndTotal { get; set; }
            [DataMember(IsRequired = false)]
            public object RatedDisabilities { get; set; }
            [DataMember(IsRequired = false)]

            public string ScReportDate { get; set; }
            [DataMember(IsRequired = false)]
            public string ServiceConnectedIndicator { get; set; }
            [DataMember(IsRequired = false)]
            public string ServiceConnectedPercentage { get; set; }
            [DataMember(IsRequired = false)]
            public string Unemployable { get; set; }

        }
        [DataContract()]
        public class SpecialFactors
        {
            [DataMember(IsRequired = false)]
            public string AgentOrangeInd { get; set; }
            [DataMember(IsRequired = false)]
            public string EnvContaminantsInd { get; set; }
            [DataMember(IsRequired = false)]
            public string RadiationExposureInd { get; set; }

        }
        [DataContract()]
        public class EnrollmentDeterminationInfo
        {
            [DataMember(IsRequired = false)]

            public string ApplicationDate { get; set; }
            [DataMember(IsRequired = false)]

            public string EffectiveDate { get; set; }
            [DataMember(IsRequired = false)]
            public object EligibleForMedicaid { get; set; }
            [DataMember(IsRequired = false)]
            public object EndDate { get; set; }
            [DataMember(IsRequired = false)]
            public string EnrollmentCategoryName { get; set; }
            [DataMember(IsRequired = false)]

            public string EnrollmentDate { get; set; }
            [DataMember(IsRequired = false)]
            public string EnrollmentStatus { get; set; }
            [DataMember(IsRequired = false)]
            public object OtherEligibilities { get; set; }
            [DataMember(IsRequired = false)]
            public PrimaryEligibility PrimaryEligibility { get; set; }
            [DataMember(IsRequired = false)]
            public ServiceConnectionAward ServiceConnectionAward { get; set; }
            [DataMember(IsRequired = false)]
            public SpecialFactors SpecialFactors { get; set; }
            [DataMember(IsRequired = false)]

            public string RecordCreatedDate { get; set; }
            [DataMember(IsRequired = false)]

            public string RecordModifiedDate { get; set; }
            [DataMember(IsRequired = false)]
            public object UserEnrolleeSite { get; set; }
            [DataMember(IsRequired = false)]
            public object UserEnrolleeValidThrough { get; set; }
            [DataMember(IsRequired = false)]
            public string Veteran { get; set; }

        }
        [DataContract()]
        public class EligibilityVerificationInfo
        {
            [DataMember(IsRequired = false)]
            public string EligibilityStatus { get; set; }
            [DataMember(IsRequired = false)]
            public string EligibilityStatusDate { get; set; }
            [DataMember(IsRequired = false)]
            public string VerificationMethod { get; set; }

        }
        [DataContract()]
        public class Status
        {
            [DataMember(IsRequired = false)]
            public string StatusType { get; set; }
            [DataMember(IsRequired = false)]
            public string DeterminedStatus { get; set; }
            [DataMember(IsRequired = false)]
            public string IncomeTestType { get; set; }
            [DataMember(IsRequired = false)]

            public string LastEditedDate { get; set; }
            [DataMember(IsRequired = false)]

            public string CompletedDate { get; set; }

        }
        [DataContract()]
        public class Statuses
        {
            [DataMember(IsRequired = false)]
            public List<Status> Status { get; set; }

        }
        [DataContract()]
        public class IncomeTest
        {
            [DataMember(IsRequired = false)]
            public Statuses Statuses { get; set; }

        }
        [DataContract()]
        public class FinancialsInfo
        {
            [DataMember(IsRequired = false)]
            public object FinancialStatement { get; set; }
            [DataMember(IsRequired = false)]
            public IncomeTest IncomeTest { get; set; }

        }
        [DataContract()]
        public class InsuranceList
        {
            [DataMember(IsRequired = false)]
            public object Insurance { get; set; }

        }
        [DataContract()]
        public class MilitaryServiceInfo
        {
            [DataMember(IsRequired = false)]
            public object CombatVeteranEligibilityEndDate { get; set; }
            [DataMember(IsRequired = false)]
            public object DisabilityRetirementIndicator { get; set; }
            [DataMember(IsRequired = false)]
            public object DischargeDueToDisability { get; set; }
            [DataMember(IsRequired = false)]
            public object EligibleForClassIIDental { get; set; }
            [DataMember(IsRequired = false)]
            public object MedalofHonorIndicator { get; set; }
            [DataMember(IsRequired = false)]
            public object ShadIndicator { get; set; }

        }
        [DataContract()]
        public class PrisonerOfWarInfo
        {
            [DataMember(IsRequired = false)]
            public object PowIndicator { get; set; }

        }
        [DataContract()]
        public class Relations
        {
            [DataMember(IsRequired = false)]
            public object Relation { get; set; }

        }
        [DataContract()]
        public class SensitivityInfo
        {
            [DataMember(IsRequired = false)]
            public object SensitivityChangeSource { get; set; }
            [DataMember(IsRequired = false)]
            public object SensitivityChangeSite { get; set; }
            [DataMember(IsRequired = false)]
            public string SensityFlag { get; set; }

        }
        [DataContract()]
        public class VamcInfo
        {
            [DataMember(IsRequired = false)]
            public string DfnNumber { get; set; }
            [DataMember(IsRequired = false)]
            public string FacilityNumber { get; set; }
            [DataMember(IsRequired = false)]
            public string LastVisitDate { get; set; }
            [DataMember(IsRequired = false)]
            public string VamcReportDate { get; set; }

        }
        [DataContract()]
        public class VamcData
        {
            [DataMember(IsRequired = false)]
            public List<VamcInfo> VamcInfo { get; set; }

        }
        [DataContract()]
        public class PersonInfo
        {
            public VamcData VamcData { get; set; }

        }
        [DataContract()]
        public class Data
        {
            [DataMember(IsRequired = false)]
            public Associations Associations { get; set; }
            [DataMember(IsRequired = false)]
            public DeathRecond DeathRecond { get; set; }
            [DataMember(IsRequired = false)]
            public Demographics Demographics { get; set; }
            [DataMember(IsRequired = false)]
            public EnrollmentDeterminationInfo EnrollmentDeterminationInfo { get; set; }
            [DataMember(IsRequired = false)]
            public EligibilityVerificationInfo EligibilityVerificationInfo { get; set; }
            [DataMember(IsRequired = false)]
            public FinancialsInfo FinancialsInfo { get; set; }
            [DataMember(IsRequired = false)]
            public InsuranceList InsuranceList { get; set; }
            [DataMember(IsRequired = false)]
            public MilitaryServiceInfo MilitaryServiceInfo { get; set; }
            [DataMember(IsRequired = false)]
            public PrisonerOfWarInfo PrisonerOfWarInfo { get; set; }
            [DataMember(IsRequired = false)]
            public Relations Relations { get; set; }
            [DataMember(IsRequired = false)]
            public SensitivityInfo SensitivityInfo { get; set; }
            [DataMember(IsRequired = false)]
            public PersonInfo PersonInfo { get; set; }

        }
        [DataContract()]
        public class Root
        {
            [DataMember(IsRequired = false)]
            public bool ErrorOccurred { get; set; }
            [DataMember(IsRequired = false)]
            public object ErrorMessage { get; set; }
            [DataMember(IsRequired = false)]
            public object Status { get; set; }
            [DataMember(IsRequired = false)]
            public object DebugInfo { get; set; }
            [DataMember(IsRequired = false)]
            public Data Data { get; set; }

        }


    }
}
