using System;
using System.Globalization;
using System.ServiceModel.Channels;
using CuttingEdge.Conditions;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Bgs.Services;
using System.Linq;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Services;
using UDO.LOB.Core;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class InsertBenefitClaimInformation : FilterBase<IAddDependentRequestState>
    {
        // WSCR 1601:
        //  Update the endProductName from ‘130 - Automated Dependency 686c’ to ‘130 - Phone Dependency Adjustment’
        //  private const string _EndProductName = "130 - Automated Dependency 686c";
        private const string _EndProductName = "130 - Phone Dependency Adjustment";

        // WSCR 1601:
        //  change the end product code to 130PDA
        //  private const string _EndProductCode = "130DPNDCYAUT";
        private const string _EndProductCode = "130PDA";

        private const string _AddressType = "Residence";
        private const string _Payee = "00";
        private const string _PreDischargeIndicator = "N";
        private const string _BenefitClaimType = "1";

        private IAddDependentRequestState _Msg;

        public override void Execute(IAddDependentRequestState msg)
        {
            _Msg = msg;

			//CSDev REm 
			//Logger.Instance.Debug("Calling InsertBenefitClaimInformation");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "InsertBenefitClaimInformation.Execute", "Calling InsertBenefitClaimInformation");


			DateTime methodStartTime, wsStartTime;
            string method = "InsertBenefitClaimInformation", webService = "insertBenefitClaim";

			
			Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

			Condition.Requires(msg, "msg").IsNotNull();

            Condition.Requires(msg.ProcRequestState, "msg.ProcRequestState").IsNotNull();
            Condition.Requires(msg.ProcRequestState.VnpProcId, "msg.ProcRequestState.VnpProcId").IsGreaterThan(0);
            Condition.Ensures(msg.NextAvailBnftClaimIncrement, "msg.NextAvailBnftClaimIncrement").IsNotNullOrEmpty();
            Condition.Requires(BgsSecurityConfiguration.Current.ApplicationId,
               "BgsSecurityConfiguration.Current.ApplicationId").IsNotEmpty();
            Condition.Requires(BgsSecurityConfiguration.Current.UserName,
                "BgsSecurityConfiguration.Current.UserName").IsNotEmpty();

            var input = new insertBenefitClaimBenefitClaimInput
            {
                fileNumber = msg.VeteranRequestState.VeteranParticipant.FileNumber,
                dateOfClaim = PaddedDateString(msg.ProcRequestState.CreatedDate),
                ssn = msg.VeteranRequestState.VeteranParticipant.Ssn,
                //claimantSsn = msg.VeteranRequestState.VeteranParticipant.Ssn,
                benefitClaimType = _BenefitClaimType,
                endProduct = msg.NextAvailBnftClaimIncrement,
                titleName = msg.VeteranRequestState.VeteranParticipant.TitleName,
                firstName = msg.VeteranRequestState.VeteranParticipant.FirstName,
                middleName = msg.VeteranRequestState.VeteranParticipant.MiddleName,
                lastName = msg.VeteranRequestState.VeteranParticipant.LastName,
               
                addressType = null, //Domestic Address
                addressLine1 = msg.VeteranRequestState.VeteranParticipant.Addresses[0].AddressLine1,
                addressLine2 = msg.VeteranRequestState.VeteranParticipant.Addresses[0].AddressLine2,
                addressLine3 = msg.VeteranRequestState.VeteranParticipant.Addresses[0].AddressLine3,
                city = msg.VeteranRequestState.VeteranParticipant.Addresses[0].City,
                state = msg.VeteranRequestState.VeteranParticipant.Addresses[0].State,
                postalCode = msg.VeteranRequestState.VeteranParticipant.Addresses[0].ZipCode,
                postalCodePlus4 = msg.VeteranRequestState.VeteranParticipant.Addresses[0].ZipPlus4,
                country = msg.VeteranRequestState.VeteranParticipant.Addresses[0].Country,

                soj = msg.StationOfJurisdiction,
                sectionUnitNo = msg.StationOfJurisdiction,
                endProductName = _EndProductName,
                endProductCode = _EndProductCode,
                payee = _Payee,
                preDischargeIndicator = _PreDischargeIndicator,
                allowPoaAccess = msg.VeteranRequestState.VeteranParticipant.AllowPoaAccess,
                allowPoaCadd = msg.VeteranRequestState.VeteranParticipant.AllowPoaCadd,
                emailAddress = msg.VeteranRequestState.VeteranParticipant.EmailAddress,
                suffixName = msg.VeteranRequestState.VeteranParticipant.SuffixName,
                beneficiaryDateOfBirth = PaddedDateString(msg.VeteranRequestState.VeteranParticipant.BirthDate),
                //submtrApplcnTypeCd = AddDependentConfiguration.GetCurrent(msg.AddDependentMaintenanceRequestState.Context).SubmtrApplcnTypeCd,
                //submtrRoleTypeCd = AddDependentConfiguration.GetCurrent(msg.AddDependentMaintenanceRequestState.Context).SubmtrRoleTypeCd
                
            };


            var dayTimeNumber =  msg.VeteranRequestState.VeteranParticipant.PhoneNumbers.Where(p => p.PhoneTypeName == "Daytime").FirstOrDefault();
            if(dayTimeNumber != null) {
                input.dayTimePhoneNumber =dayTimeNumber.Number;
                input.dayTimeAreaCode = dayTimeNumber.AreaCode;
            }

            var nightTimeNumber = msg.VeteranRequestState.VeteranParticipant.PhoneNumbers.Where(p => p.PhoneTypeName == "Nighttime").FirstOrDefault();
            if (nightTimeNumber != null)
            {
                input.nightTimePhoneNumber = nightTimeNumber.Number;
                input.nightTimeAreaCode = nightTimeNumber.AreaCode;
            }

            Condition.Requires(msg.AddDependentMaintenanceRequestState,
                "msg.AddDependentMaintenanceRequestState").IsNotNull();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationName,
                    "msg.AddDependentMaintenanceRequestState.OrganizationName").IsNotNullOrEmpty();
            Condition.Requires(msg.AddDependentMaintenanceRequestState.OrganizationService,
                "msg.AddDependentMaintenanceRequestState.OrganizationService").IsNotNull();

            var service = BgsServiceFactory.GetBenefitClaimService(msg.AddDependentMaintenanceRequestState.OrganizationName,
                msg.AddDependentMaintenanceRequestState.OrganizationService,
                msg.AddDependentMaintenanceRequestState.SystemUserId,
                OnReplyCallBack);

			
			Guid wsLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
				msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, webService, out wsStartTime);

			service.insertBenefitClaim(new insertBenefitClaimRequest(input));

			
			LogHelper.EndTiming(wsLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, wsStartTime);

			
			LogHelper.EndTiming(methodLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTime);

			LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
				, msg.AddDependentMaintenanceRequestState.SystemUserId, $"{ this.GetType().FullName}"
							, $"| RRR End w/ No Request / Response {this.GetType().FullName}.Execute");
		}


        //Using SOAP Message Reply Extraction here.
        //The XSD from the VA is severly jacked up. Using the object model for the response is not feasible.
        //This is a work-around
        public void OnReplyCallBack(ref Message reply, object correlationState)
        {
            Condition.Requires(_Msg, "_Msg").IsNotNull();

            var xmlDoc = reply.ToXmlDocument();

            var elements = xmlDoc.GetElementsByTagName("benefitClaimID");

            if (elements.Count == 0)
                throw new Exception(string.Format("BenefitClaimId was not returned from InsertBenefitClaimInformation. {0}", VEIS.Core.Wcf.SoapLog.GetFormattedXml(xmlDoc)));

            var benefitClaimId = elements[0].InnerText;

            Condition.Requires(benefitClaimId, "benefitClaimId").IsNotNullOrEmpty();

            _Msg.BenefitClaimId = long.Parse(benefitClaimId);

            //Remember to reset the messages. Messages can only be read once.
            reply = xmlDoc.ToMessage(reply.Version);
        }

        public string PaddedDateString(DateTime dateTime)
        {
            var year = dateTime.Date.Year.ToString(CultureInfo.InvariantCulture);
            var day = dateTime.Date.Day.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
            var month = dateTime.Date.Month.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');

            return string.Format("{0}/{1}/{2}", month, day, year);
        }
    }
}