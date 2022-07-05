using CRM007.CRM.SDK.Core;
using CuttingEdge.Conditions;
using log4net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.CrmModel;
using VRM.Integration.Servicebus.Egain.State;

namespace VRM.Integration.Servicebus.Egain.Processor.AuthCoBrowseRequestStep.Steps
{
	public class CreateSessionStep : FilterBase<IAuthCoBrowseRequestState>
	{
		public CreateSessionStep()
		{
		}

		public override void Execute(IAuthCoBrowseRequestState msg)
		{
			Logger.get_Instance().Debug("Calling CreateSessionStep");
			ValidatorExtensions.IsNotNull<IAuthCoBrowseRequestState>(Condition.Requires<IAuthCoBrowseRequestState>(msg, "msg"));
			CrmConnectionParms connectionParms = CrmConnectionConfiguration.get_Current().GetCrmConnectionParmsByName(msg.OrgName.ToUpper());
			if (connectionParms == null)
			{
				throw new Exception(string.Format("ConnectionParms was not found for OrganizationProxy {0}", msg.OrgName));
			}
			OrganizationServiceProxy connection = CrmConnection.Connect(connectionParms, typeof(crme_chatcobrowsesessionlog).Assembly);
			ChatXrmServiceContext xrm = new ChatXrmServiceContext(connection);
			WhoAmIRequest userRequest = new WhoAmIRequest();
			WhoAmIResponse response = (WhoAmIResponse)msg.TargetOrganizationServiceProxy.Execute(userRequest);
			crme_chatcobrowsesessionlog session = xrm.CreateQuery<crme_chatcobrowsesessionlog>().FirstOrDefault<crme_chatcobrowsesessionlog>((crme_chatcobrowsesessionlog x) => x.crme_CoBrowseSessionId == msg.CoBrowseSessionId);
			if (session != null)
			{
				session.crme_CallAgentId = (msg.CallAgentId != null ? msg.CallAgentId : session.crme_CallAgentId);
				session.crme_CoBrowseSessionId = (msg.CoBrowseSessionId != null ? msg.CoBrowseSessionId : session.crme_CoBrowseSessionId);
				xrm.UpdateObject(session);
			}
			else
			{
				session = new crme_chatcobrowsesessionlog()
				{
					OwnerId = new EntityReference("systemuser", response.get_UserId()),
					crme_SessionType = new OptionSetValue()
				};
				session.crme_SessionType.set_Value(935950002);
				session.crme_CallAgentId = msg.CallAgentId;
				xrm.AddObject(session);
				Annotation annotation1 = new Annotation()
				{
					IsDocument = new bool?(true),
					Subject = "Authenticated Chat Transcript",
					ObjectId = new EntityReference("crme_chatcobrowsesessionlog", session.get_Id()),
					ObjectTypeCode = "crme_chatcobrowsesessionlog".ToLower(),
					OwnerId = new EntityReference("systemuser", response.get_UserId())
				};
				Annotation annotation = annotation1;
				xrm.AddObject(annotation);
				annotation.MimeType = "text/plain";
				annotation.NoteText = "Authenticated Chat Transcript";
				DateTime now = DateTime.Now;
				annotation.FileName = string.Concat("AuthenticatedChatTranscript-", now.ToString("yyyy-dd-M-HH-mm-ss"), ".txt");
				annotation.DocumentBody = msg.CoBrowseSessionLog;
				xrm.UpdateObject(annotation);
				xrm.SaveChanges();
			}
			xrm.SaveChanges();
		}
	}
}