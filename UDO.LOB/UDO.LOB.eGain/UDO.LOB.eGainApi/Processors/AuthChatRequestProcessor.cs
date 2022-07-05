using log4net;
using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.Messages.Messages;
using VRM.Integration.Servicebus.Egain.Processor.AuthChatRequestStep.Steps;
using VRM.Integration.Servicebus.Egain.State;
using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace VRM.Integration.Servicebus.Egain
{
	public class AuthChatRequestProcessor
	{
		public AuthChatRequestProcessor()
		{
		}

		public IMessageBase Execute(AuthChatRequest request)
		{
			AuthChatResponse response = new AuthChatResponse()
			{
				ErrorCode = 0
			};
			try
			{
				if ((!string.IsNullOrEmpty(request.ParticipantId) || !string.IsNullOrEmpty(request.Edipi) || !string.IsNullOrEmpty(request.Ssn) ? false : string.IsNullOrEmpty(request.VsoOrgId)))
				{
					throw new ArgumentException("ParticipantId/EDIPI/SSN/VSOOrgId Not Specified");
				}
				if (string.IsNullOrEmpty(request.ChatSessionId))
				{
					throw new ArgumentException("ChatSessionId Not Specified");
				}
				if (string.IsNullOrEmpty(request.OrgName))
				{
					throw new ArgumentException("OrgName Not Specified");
				}
				if (string.IsNullOrEmpty(request.CallAgentId))
				{
					throw new ArgumentException("CallAgentId Not Specified");
				}
				if (!string.IsNullOrEmpty(request.VsoOrgId))
				{
					request.Ssn = null;
				}
				AuthChatRequestState msg = new AuthChatRequestState(request.CallAgentId, request.Category, request.ChatSessionId, request.ChatSessionLog, request.Edipi, request.OrgName, request.Resolution, request.Ssn, request.VsoOrgId, request.ParticipantId);
				try
				{
					(new Pipeline<IAuthChatRequestState>()).Register(new ConnectToTargetCrmStep()).Register(new CreateSessionStep()).Execute(msg);
					response = new AuthChatResponse()
					{
						IsTargetUserUdoChatUser = msg.IsTargetUserUdoChatUser
					};
				}
				finally
				{
					if (msg != null)
					{
						((IDisposable)msg).Dispose();
					}
				}
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				Exception ex2 = null;
				string traceText = null;
				if (ex.InnerException != null)
				{
					if (ex.InnerException.InnerException != null)
					{
						traceText = ((FaultException<OrganizationServiceFault>)ex.InnerException.InnerException).Detail.get_TraceText();
					}
				}
				if (string.IsNullOrEmpty(traceText))
				{
					ex2 = new Exception(string.Format("ChatSessionId: {0} : CallAgentId {1} : {2}", request.ChatSessionId, request.CallAgentId, ex.Message), ex);
				}
				else
				{
					object[] chatSessionId = new object[] { request.ChatSessionId, request.CallAgentId, ex.Message, traceText };
					ex2 = new Exception(string.Format("ChatSessionId: {0} : CallAgentId {1} : {2} \r\nTraceText: {3}", chatSessionId), ex);
				}
				response.ErrorCode = 1;
				response.ErrorMessage = ex2.Message;
				LogHelper.LogError(request.OrgName, "crme_chatcobrowsetiming", Guid.Empty, "AuthChatRequestProcessor.Execute", ex2);
				Logger.get_Instance().Debug(ex2.ToString());
			}
			return response;
		}
	}
}