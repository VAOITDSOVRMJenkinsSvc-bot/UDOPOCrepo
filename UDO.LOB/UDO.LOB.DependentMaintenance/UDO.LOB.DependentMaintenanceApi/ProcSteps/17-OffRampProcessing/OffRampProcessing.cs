using System;
using System.Globalization;
using CuttingEdge.Conditions;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.AddDependent;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class OffRampProcessing : FilterBase<IAddDependentRequestState>
    {
        public static string EndProductNameDefault = "130 – Phone Dependency Adjustments";
        public static string EndProductNameException = "130 – Phone Dependency Adjusts Exception";

        public override void Execute(IAddDependentRequestState msg)
        {
			//CSDEV REm
			//Logger.Instance.Debug("Calling OffRampProcessing");
			//LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "OffRampProcessing.Execute", "Calling OffRampProcessing");

			Condition.Requires(msg, "msg").IsNotNull();

            //var request = CreateDefaultBenefitClaim(msg);

            //Place Holder Service

            //ExecOffRampRules(msg, request);

            //msg.BenifitClaim = request;
        }

        private insertBenefitClaimBenefitClaimInput CreateDefaultBenefitClaim(IAddDependentRequestState msg)
        {
            var request = new insertBenefitClaimBenefitClaimInput
            {
                endProduct = "130",
                endProductCode = "130",
                firstName = msg.DependentRequestState.DependentParticipant.FirstName,
                lastName = msg.DependentRequestState.DependentParticipant.LastName,
                addressLine1 = msg.DependentRequestState.DependentParticipant.Addresses[0].AddressLine1,
                city = msg.DependentRequestState.DependentParticipant.Addresses[0].City,
                state = msg.DependentRequestState.DependentParticipant.Addresses[0].State,
                postalCode = msg.DependentRequestState.DependentParticipant.Addresses[0].ZipCode,
                dateOfClaim = DateTimeExtensions.TodayNoon.ToString(CultureInfo.InvariantCulture),
                bnftClaimId = msg.BenefitClaimId.ToString(CultureInfo.InvariantCulture)
            };

            request.endProductName = ExecOffRampRules(msg, request) ? 
                EndProductNameDefault : 
                EndProductNameException;

            if(request.endProductName == EndProductNameException)
			{
				//CSDEV REm
				//Logger.Instance.Info(string.Format("Offramp Error for Benefit Claim: {0}", msg.BenefitClaimId));
				LogHelper.LogInfo(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
					, msg.AddDependentMaintenanceRequestState.SystemUserId, "OffRampProcessing.Execute", "Calling OffRampProcessing");
			}
                

            return request;
        }

        private bool ExecOffRampRules(IAddDependentRequestState msg, insertBenefitClaimBenefitClaimInput request)
        {
            if (!ChildDependentSsnCheck(msg, request)) 
                return false;

            if (!SpouseDependentSsnCheck(msg, request))
                return false;

            if (!SpouseDependentMaritalDateOverlap(msg, request))
                return false;

            if (!IncompleteMaritalHistory(msg, request))
                return false;

            if (!IncompleteSpouseMaritalHistory(msg, request))
                return false;

            if (!AttemptingToAddDuplicateChildRecordInCorporate(msg, request))
                return false;

            if (!AttemptingToAddDuplicateSpouseRecordInCorporate(msg, request))
                return false;

            if (!ChildPreviouslyMarried(msg, request))
                return false;

            if (!ChildAgeGreaterThanEighteen(msg, request))
                return false;

            return true;
        }

        private bool ChildAgeGreaterThanEighteen(IAddDependentRequestState msg, insertBenefitClaimBenefitClaimInput request)
        {
            var dependent = msg.DependentRequestState.DependentParticipant;

            if (dependent.DependentRelationship.FamilyRelationshipTypeName != "Child")
                return true;

            return true;
        }

        private bool ChildPreviouslyMarried(IAddDependentRequestState msg, insertBenefitClaimBenefitClaimInput request)
        {
            var dependent = msg.DependentRequestState.DependentParticipant;

            if (dependent.DependentRelationship.FamilyRelationshipTypeName != "Child")
                return true;

            return true;
        }

        private bool AttemptingToAddDuplicateSpouseRecordInCorporate(IAddDependentRequestState msg, insertBenefitClaimBenefitClaimInput request)
        {
            var dependent = msg.DependentRequestState.DependentParticipant;

            if (dependent.DependentRelationship.FamilyRelationshipTypeName != "Spouse")
                return true;

            return true;
        }

        private bool AttemptingToAddDuplicateChildRecordInCorporate(IAddDependentRequestState msg, insertBenefitClaimBenefitClaimInput request)
        {
            var dependent = msg.DependentRequestState.DependentParticipant;

            if (dependent.DependentRelationship.FamilyRelationshipTypeName != "Child")
                return true;

            return true;
        }

        private bool IncompleteSpouseMaritalHistory(IAddDependentRequestState msg, insertBenefitClaimBenefitClaimInput request)
        {
            var dependent = msg.DependentRequestState.DependentParticipant;

            if (dependent.DependentRelationship.FamilyRelationshipTypeName != "Spouse")
                return true;

            return true;
        }

        private bool IncompleteMaritalHistory(IAddDependentRequestState msg, insertBenefitClaimBenefitClaimInput request)
        {
            if (msg.MaritalHistories == null)
                return true;

            foreach (var maritalHistory in msg.MaritalHistories)
            {
                if (string.IsNullOrEmpty(maritalHistory.HowMarriageWasTerminated))
                    return false;

                if (maritalHistory.MarriageEndDate == DateTime.MinValue)
                    return false;

                if (string.IsNullOrEmpty(maritalHistory.City))
                    return false;

                if (string.IsNullOrEmpty(maritalHistory.State))
                    return false;

                if (string.IsNullOrEmpty(maritalHistory.Country))
                    return false;
            }

            return true;
        }

        private bool SpouseDependentMaritalDateOverlap(IAddDependentRequestState msg, insertBenefitClaimBenefitClaimInput request)
        {
            var dependent = msg.DependentRequestState.DependentParticipant;

            if (dependent.DependentRelationship.FamilyRelationshipTypeName != "Spouse")
                return true;

            if (msg.MaritalHistories == null)
                return true;

            foreach (var maritalHistory in msg.MaritalHistories)
            {
                if (dependent.DependentRelationship.BeginDate >= maritalHistory.MarriageStartDate &&
                    dependent.DependentRelationship.BeginDate <= maritalHistory.MarriageEndDate)
                    return false;
            }

            return true;
        }

        private bool SpouseDependentSsnCheck(IAddDependentRequestState msg, insertBenefitClaimBenefitClaimInput request)
        {
            var dependent = msg.DependentRequestState.DependentParticipant;

            return (dependent.DependentRelationship.FamilyRelationshipTypeName == "Spouse") && (!string.IsNullOrEmpty(dependent.Ssn));
        }

        private bool ChildDependentSsnCheck(IAddDependentRequestState msg, insertBenefitClaimBenefitClaimInput request)
        {
            var dependent = msg.DependentRequestState.DependentParticipant;

            return (dependent.DependentRelationship.FamilyRelationshipTypeName == "Child") && (!string.IsNullOrEmpty(dependent.Ssn));
        }
    }
}