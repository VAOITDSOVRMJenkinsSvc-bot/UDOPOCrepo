using System.Collections.Generic;
using CuttingEdge.Conditions;
//using VRM.Integration.Servicebus.Bgs.Messages;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.CRM.SDK.Core;
using VRM.Integration.Servicebus.Bgs.Services.ClaimantWebServiceReference;
using System.Linq;
using UDO.LOB.DependentMaintenance;
using VRM.Integration.Servicebus.AddDependent.Util;
using UDO.LOB.DependentMaintenance.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using VEIS.Core.Wcf;

//CSdev
//namespace VRM.Integration.Servicebus.Bgs
namespace UDO.LOB.DependentMaintenance.Processors
{
    public class GetMaritalInfoProcessor
    {
        public IMessageBase Execute(GetMaritalInfoRequest message)
        {
			//CSDEv 
			//Logger.Instance.Debug("Calling GetMaritalInfoProcessor");
			LogHelper.LogInfo("Calling GetMaritalInfoProcessor");

			//CSDEv Readding Soap Logs 
			VEIS.Core.Wcf.SoapLog.Current.Active = true;

            var response = new GetMaritalInfoResponse();

            var maritalInfos = new List<GetMaritalInfoMultipleResponse>();

            string participantId;

            Condition.Requires(message.crme_OrganizationName, "message.crme_OrganizationName").IsNotNullOrEmpty();

            var crmConnection = ConnectToCrmHelper.ConnectToCrm(message.crme_OrganizationName);

            if (string.IsNullOrEmpty(message.crme_ParticipantId))
            {
                Condition.Requires(message.crme_SSN, "message.crme_SSN").IsNotNullOrEmpty();

                participantId = Utils.GetVeteranParticipantId(message.crme_OrganizationName,
                    message.crme_SSN,
                    crmConnection, 
                    message.crme_UserId);
            }
            else
            {
                participantId = message.crme_ParticipantId;
            }

            Condition.Requires(participantId, "participantId").IsNotEmpty();

            shrinq6Person[] persons = Utils.GetAllRelationships(message.crme_OrganizationName,
                                                                participantId,
                                                                crmConnection,
                                                                message.crme_UserId);

            if (persons != null)
            {
                foreach (shrinq6Person person in persons.Where(p => p.relationshipType != null && p.relationshipType.Equals("Spouse")))
                {
                    var maritalInfo = Utils.MapShrink6PersonToGetMaritalInfoMultipleResponse(person);
                    maritalInfos.Add(maritalInfo);
                }
            }
            response.GetMaritalInfo = maritalInfos.ToArray();

			//CSDev REAdding soap logs 
			response.SoapLog = VEIS.Core.Wcf.SoapLog.Current.Log;
			VEIS.Core.Wcf.SoapLog.Current.Active = false;
			VEIS.Core.Wcf.SoapLog.Current.ClearLog();

			return response;
        }
    }
}