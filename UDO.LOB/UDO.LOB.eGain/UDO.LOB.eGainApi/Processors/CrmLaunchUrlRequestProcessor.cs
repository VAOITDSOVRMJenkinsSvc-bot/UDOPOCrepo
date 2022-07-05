using log4net;
using System;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.Messages.Messages;
using VRM.Integration.Servicebus.Egain.Processor.CrmLaunchUrlRequestStep.Steps;
using VRM.Integration.Servicebus.Egain.State;
using VRM.Integration.Servicebus.Logging.CRM.Util;

namespace VRM.Integration.Servicebus.Egain
{
	public class CrmLaunchUrlRequestProcessor
	{
		public CrmLaunchUrlRequestProcessor()
		{
		}

		public IMessageBase Execute(CrmLaunchUrlRequest request)
		{
			CrmLaunchUrlResponse response = new CrmLaunchUrlResponse()
			{
				ErrorCode = 0
			};
			try
			{
				if ((!string.IsNullOrEmpty(request.ParticipantId) || !string.IsNullOrEmpty(request.Edipi) || !string.IsNullOrEmpty(request.Ssn) ? false : string.IsNullOrEmpty(request.VsoOrgId)))
				{
					throw new ArgumentException("ParticipantId/EDIPI/SSN/VsoOrgId Not Specified");
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
				CrmLaunchUrlRequestState msg = new CrmLaunchUrlRequestState(request);
				try
				{
					(new Pipeline<CrmLaunchUrlRequestState>()).Register(new ConnectToTargetCrmStep()).Register(new CreateSessionStep()).Execute(msg);
					CrmLaunchUrlResponse crmLaunchUrlResponse = new CrmLaunchUrlResponse()
					{
						CrmLaunchUrl = msg.CrmLaunchUrl,
						IsTargetUserUdoChatUser = msg.IsTargetUserUdoChatUser
					};
					response = crmLaunchUrlResponse;
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
				Exception ex2 = new Exception(string.Format("ChatSessionId: {0} : CallAgentId {1} : {2}", request.ChatSessionId, request.CallAgentId, ex.Message), ex);
				response.ErrorCode = 1;
				response.ErrorMessage = ex2.Message;
				LogHelper.LogError(request.OrgName, "crme_chatcobrowsetiming", Guid.Empty, "CrmLaunchUrlRequestProcessor.Execute", ex2);
				Logger.get_Instance().Debug(ex2.ToString());
			}
			return response;
		}
	}
}