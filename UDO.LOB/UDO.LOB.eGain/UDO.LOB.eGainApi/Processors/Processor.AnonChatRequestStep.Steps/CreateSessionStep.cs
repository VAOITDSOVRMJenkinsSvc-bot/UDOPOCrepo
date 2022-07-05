//using CRM007.CRM.SDK.Core;
using CuttingEdge.Conditions;
//using log4net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
//using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain;
//using VRM.Integration.Servicebus.Egain.CrmModel;

namespace UDO.LOB.Egain.Processor
{
    public class CreateSessionStep : FilterBase<IAnonChatRequestState>
    {
        public CreateSessionStep()
        {
        }

        public override void Execute(IAnonChatRequestState msg)
        {
            // Logger.get_Instance().Debug("Calling CreateSessionStep");

            LogHelper.LogInfo(msg.OrgName, false, Guid.Empty, "VRM.Integration.Servicebus.Egain.Processor.AnonChatRequestStep.Steps", "Calling CreateSessionStep");
            ValidatorExtensions.IsNotNull<IAnonChatRequestState>(Condition.Requires<IAnonChatRequestState>(msg, "msg"));


            /*
            CrmConnectionParms connectionParms = CrmConnectionConfiguration.get_Current().GetCrmConnectionParmsByName(msg.OrgName.ToUpper());
			if (connectionParms == null)
			{
				throw new Exception(string.Format("ConnectionParms was not found for OrganizationProxy {0}", msg.OrgName));
			}
			OrganizationServiceProxy connection = CrmConnection.Connect(connectionParms, typeof(crme_chatcobrowsesessionlog).Assembly);
			WhoAmIRequest userRequest = new WhoAmIRequest();
			WhoAmIResponse response = (WhoAmIResponse)msg.TargetOrganizationServiceProxy.Execute(userRequest);
			ChatXrmServiceContext xrm = new ChatXrmServiceContext(connection);
			crme_chatcobrowsesessionlog session = xrm.CreateQuery<crme_chatcobrowsesessionlog>().FirstOrDefault<crme_chatcobrowsesessionlog>((crme_chatcobrowsesessionlog x) => x.crme_ChatSessionId == msg.ChatSessionId);
			if (session != null)
			{
				session.crme_CallAgentId = (msg.CallAgentId != null ? msg.CallAgentId : session.crme_CallAgentId);
				session.crme_ChatSessionId = (msg.ChatSessionId != null ? msg.ChatSessionId : session.crme_ChatSessionId);
				xrm.UpdateObject(session);
			}
			else
			{
				session = new crme_chatcobrowsesessionlog()
				{
					OwnerId = new EntityReference("systemuser", response.get_UserId()),
					crme_SessionType = new OptionSetValue()
				};
				session.crme_SessionType.set_Value(935950000);
				session.crme_CallAgentId = msg.CallAgentId;
				session.crme_ChatSessionId = msg.ChatSessionId;
				xrm.AddObject(session);
			}
			xrm.SaveChanges();
            */

            // Create Crm Connection
            OrganizationWebProxyClient organizationWebProxy = CrmConnection.Connect<OrganizationWebProxyClient>();

            WhoAmIRequest userRequest = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)msg.TargetOrganizationServiceProxy.Execute(userRequest);

            // Find the crme_chatcobrowsesessionlog with crme_ChatSessionId that matches the input Chat Session Id
            QueryExpression query = new QueryExpression("crme_chatcobrowsesessionlog")
            {
                ColumnSet = new ColumnSet(new string[] { "crme_callagentid", "crme_chatsessionid", "crme_chatcobrowsesessionlogid" })
            };

            ConditionExpression conditionChatSessionId = new ConditionExpression("crme_chatsessionid", ConditionOperator.Equal, msg.ChatSessionId);
            query.Criteria.AddCondition(conditionChatSessionId);

            EntityCollection entityCollection = organizationWebProxy.RetrieveMultiple(query);
            // If it exist, update the crme_chatcobrowsesessionlog with Caller Agent Id and Chat Session Id
            if (entityCollection != null && entityCollection.Entities.Count > 0)
            {
                Entity session = entityCollection.Entities[0];
                session["crme_callagentid"] = msg.CallAgentId ?? msg.CallAgentId;
                session["crme_chatsessionid"] = msg.ChatSessionId ?? msg.ChatSessionId;

                organizationWebProxy.Update(session);

            }
            // If the crme_chatcobrowsesessionlog  does not exist, create it.
            else
            {
                Entity session = new Entity("crme_chatcobrowsesessionlog");

                session.Attributes["ownerid"] = new EntityReference("systemuser", response.UserId);
                session.Attributes["crme_sessiontype"] = new OptionSetValue(935950000);
                session.Attributes["crme_callagentid"] = msg.CallAgentId;
                session.Attributes["crme_chatsessionid"] = msg.ChatSessionId;

                organizationWebProxy.Create(session);

            }
        }
    }


}