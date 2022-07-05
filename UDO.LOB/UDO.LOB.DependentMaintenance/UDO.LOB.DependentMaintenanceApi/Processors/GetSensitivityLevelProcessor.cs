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
//using IMessageBase = VRM.Integration.Servicebus.Core.IMessageBase;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//using VRM.Integration.Servicebus.Core;
using UDO.LOB.DependentMaintenance.Messages;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.AddDependent.Util;
using UDO.LOB.Core;

//CSdev
//namespace VRM.Integration.Servicebus.Bgs
namespace UDO.LOB.DependentMaintenance.Processors
{
    public class GetSensitivityLevelProcessor
    {
        public IMessageBase Execute(GetSensitivityLevelRequest message)
        {
			//CSDev
			//Logger.Instance.Debug("Calling GetSensitivityLevelProcessor");
			LogHelper.LogInfo("Calling GetSensitivityLevelProcessor"); 


			DateTime methodStartTime, wsStartTime;
            string method = "GetSensitivityLevelProcessor", webService = "Utils.GetSensitivityLevel";

			//CSDev
			//Guid methodLoggingId  = LogHelper.StartTiming(message.crme_OrganizationName, Utils.ConfigFieldName, message.crme_UserId,
                //Guid.Empty, null, null, method, null, out methodStartTime);

			var crmConnection = ConnectToCrmHelper.ConnectToCrm(message.crme_OrganizationName);

            var response = new GetSensitivityLevelResponse();

			//CSDev
			//Guid wsLoggingId = LogHelper.StartTiming(message.crme_OrganizationName, Utils.ConfigFieldName, message.crme_UserId,
				//Guid.Empty, null, null, method, webService, out wsStartTime);

			response.SensitivityLevel = Utils.GetSensitivityLevel(message.crme_OrganizationName, crmConnection, message.crme_UserId, message.crme_SSN, message.crme_ParticipantId);

			//CSDev
			//LogHelper.EndTiming(wsLoggingId, message.crme_OrganizationName, Utils.ConfigFieldName, message.crme_UserId, wsStartTime);


            return response;
        }
    }
}