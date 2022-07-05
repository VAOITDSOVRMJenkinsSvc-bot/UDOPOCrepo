using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Bgs.Messages;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Egain.CrmModel;
//using VRM.Integration.Servicebus.Egain.UdoCrmModel;
//using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace VRM.Integration.Servicebus.Egain
{
	public class Util
	{
		public const string HIGHEST_SENSITIVITY = "8";

		public const string LOWEST_SENSITIVITY = "0";

		public const int SENSITIVITYREASON_STATUSFOUND = 935950000;

		public const int SENSITIVITYREASON_ACCESSVIOLATION = 935950001;

		public const int SENSITIVITYREASON_BGSEXCEPTION = 935950002;

		public const int SENSITIVITYREASON_NOTFOUND = 935950003;

		public Util()
		{
		}

		public static string GetSensitivityLevel(string organizationName, Guid userId, string ssn, long? participantId)
		{
			string str;
			string retVal = null;
			if ((!string.IsNullOrEmpty(ssn) ? true : participantId.HasValue))
			{
				GetSensitivityLevelRequest getSensitivityLevelRequest = new GetSensitivityLevelRequest();
				getSensitivityLevelRequest.set_crme_OrganizationName(organizationName);
				getSensitivityLevelRequest.set_crme_UserId(userId);
				getSensitivityLevelRequest.set_crme_SSN(ssn);
				getSensitivityLevelRequest.set_crme_ParticipantId(participantId);
				GetSensitivityLevelRequest request = getSensitivityLevelRequest;
				try
				{
                    // TODO: Should this be replaced with a LOB call?
					GetSensitivityLevelResponse response = MessagingSenderWcf.SendReceive<GetSensitivityLevelResponse>(request, 0);
					if (!string.IsNullOrEmpty(response.get_SensitivityLevel()))
					{
						retVal = response.get_SensitivityLevel();
					}
				}
				catch (Exception exception)
				{
					Exception ex = exception;
					LogHelper.LogError(organizationName, userId, "crme_chatcobrowsetiming", "VRM.Integration.Servicebus.Egain.Util.GetSensitivityLevel", exception);
					if (!ex.Message.Contains("Sensitive File - Access Violation"))
					{
						throw ex;
					}

					throw new AccessViolationException(ex.Message, ex);
				}
				str = retVal;
			}
			else
			{
				str = retVal;
			}
			return str;
		}

        /* Remediated to below method
		public static Guid? GetSystemUserId(VRM.Integration.Servicebus.Egain.CrmModel.ChatXrmServiceContext chatXrm, string username)
		{
			Guid? retVal = null;
			var user = (
				from u in chatXrm.SystemUserSet
				where u.DomainName == username
				select new { SystemUserId = u.SystemUserId }).FirstOrDefault();
			if (user != null)
			{
				retVal = user.SystemUserId;
			}

			return retVal;

		}
        */

        public static Guid? GetSystemUserId(IOrganizationService service, string username)
        {
            Guid? retVal = null;
            
            QueryByAttribute query = new QueryByAttribute("systemuser");
            query.AddAttributeValue("domainname", username);
            query.ColumnSet = new ColumnSet(new string[] { "systemuserid" });

            EntityCollection users = service.RetrieveMultiple(query);
            if(users.Entities.Count > 0)
            {
                retVal = users.Entities[0].GetAttributeValue<Guid>("systemuserid");
            }

            return retVal;

        }

        /*
        public static Guid? GetSystemUserIdIfUdoChatUser(VRM.Integration.Servicebus.Egain.UdoCrmModel.ChatXrmServiceContext chatXrm, string username)
		{
			Guid? systemUserId;
			bool flag;
			var user = (
				from u in chatXrm.SystemUserSet
				where u.DomainName == username
				select new { udo_IsUdoChatUser = u.udo_IsUdoChatUser, SystemUserId = u.SystemUserId }).FirstOrDefault();
			if (user == null)
			{
				flag = true;
			}
			else
			{
				bool? udoIsUdoChatUser = user.udo_IsUdoChatUser;
				flag = (!udoIsUdoChatUser.GetValueOrDefault() ? 0 : (int)udoIsUdoChatUser.HasValue) == 0;
			}
			if (flag)
			{
				systemUserId = null;
			}
			else
			{
				systemUserId = user.SystemUserId;
			}
			return systemUserId;
		}
        */

        public static Guid? GetSystemUserIdIfUdoChatUser(IOrganizationService service, string username)
        {
            Guid? systemUserId = null;

            QueryByAttribute query = new QueryByAttribute("systemuser");
            query.AddAttributeValue("domainname", username);
            query.ColumnSet = new ColumnSet(new string[] { "udo_isudochatuser", "systemuserid" });

            EntityCollection users = service.RetrieveMultiple(query);
            if (users.Entities.Count > 0)
            {

                if (users.Entities[0].GetAttributeValue<bool>("udo_isudochatuser"))
                {
                    systemUserId = users.Entities[0].GetAttributeValue<Guid>("systemuserid");
                }
            }
            
            return systemUserId;
        }
    }
}