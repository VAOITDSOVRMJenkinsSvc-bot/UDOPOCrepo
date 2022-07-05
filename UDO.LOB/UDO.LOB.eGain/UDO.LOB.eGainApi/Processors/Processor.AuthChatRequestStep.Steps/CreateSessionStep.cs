using CRM007.CRM.SDK.Core;
using CuttingEdge.Conditions;
using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain;
using VRM.Integration.Servicebus.Egain.CrmModel;
using VRM.Integration.Servicebus.Egain.State;
using VRM.Integration.Servicebus.Egain.UdoCrmModel;

namespace VRM.Integration.Servicebus.Egain.Processor.AuthChatRequestStep.Steps
{
	public class CreateSessionStep : FilterBase<IAuthChatRequestState>
	{
		public CreateSessionStep()
		{
		}

		public override void Execute(IAuthChatRequestState msg)
		{
			bool isUdoOrg;
			OrganizationServiceContext context;
			DateTime now;
			bool flag;
			CrmConnectionParms connectionParms = null;
			OrganizationServiceProxy connection = null;
			OrganizationServiceProxy connectionUdo = null;
			VRM.Integration.Servicebus.Egain.CrmModel.ChatXrmServiceContext xrm = null;
			VRM.Integration.Servicebus.Egain.UdoCrmModel.ChatXrmServiceContext xrmUdo = null;
			Guid? callerId = null;
			long? partId = null;
			long tempId = (long)0;
			if (long.TryParse(msg.ParticipantId, out tempId))
			{
				partId = new long?(tempId);
			}
			string sensitivityLevel = null;
			int sensitivityLevelReason = 935950003;
			string chatOrg = null;
			try
			{
				try
				{
					Logger.get_Instance().Debug("AuthChatRequestStep::Calling CreateSessionStep");
					ValidatorExtensions.IsNotNull<IAuthChatRequestState>(Condition.Requires<IAuthChatRequestState>(msg, "msg"));
					if (!msg.OrgName.ToUpper().Contains("VRM"))
					{
						isUdoOrg = true;
						chatOrg = msg.OrgName.ToUpper();
						connectionParms = CrmConnectionConfiguration.get_Current().GetCrmConnectionParmsByName(chatOrg);
						connectionUdo = CrmConnection.Connect(connectionParms, typeof(VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog).Assembly);
						xrmUdo = new VRM.Integration.Servicebus.Egain.UdoCrmModel.ChatXrmServiceContext(connectionUdo);
						connectionUdo.Authenticate();
						msg.TargetOrganizationService = connectionUdo;
						msg.TargetOrganizationServiceProxy = connectionUdo;
						context = new OrganizationServiceContext(connectionUdo);
						msg.TargetOrganizationServiceContext = context;
						callerId = Util.GetSystemUserIdIfUdoChatUser(xrmUdo, msg.CallAgentId);
						if (callerId.HasValue)
						{
							msg.TargetOrganizationServiceProxy.set_CallerId(callerId.Value);
						}
					}
					else
					{
						UdoChatSettingsConfiguration udoChatSettings = new UdoChatSettingsConfiguration(msg.OrgName.ToUpper());
						chatOrg = ((udoChatSettings == null ? true : udoChatSettings.Current.UdoChatOrg == null) ? msg.OrgName.ToUpper() : udoChatSettings.Current.UdoChatOrg);
						connectionParms = CrmConnectionConfiguration.get_Current().GetCrmConnectionParmsByName(chatOrg);
						connectionUdo = CrmConnection.Connect(connectionParms, typeof(VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog).Assembly);
						xrmUdo = new VRM.Integration.Servicebus.Egain.UdoCrmModel.ChatXrmServiceContext(connectionUdo);
						Guid? systemUserIdIfUdoChatUser = Util.GetSystemUserIdIfUdoChatUser(xrmUdo, msg.CallAgentId);
						if (!systemUserIdIfUdoChatUser.HasValue)
						{
							flag = true;
						}
						else
						{
							Guid? nullable = systemUserIdIfUdoChatUser;
							Guid empty = Guid.Empty;
							flag = (!nullable.HasValue ? 1 : (int)(nullable.GetValueOrDefault() != empty)) == 0;
						}
						if (flag)
						{
							isUdoOrg = false;
							xrmUdo.Dispose();
							connectionUdo.Dispose();
							chatOrg = msg.OrgName.ToUpper();
							connectionParms = CrmConnectionConfiguration.get_Current().GetCrmConnectionParmsByName(chatOrg);
							connection = CrmConnection.Connect(connectionParms, typeof(VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowsesessionlog).Assembly);
							xrm = new VRM.Integration.Servicebus.Egain.CrmModel.ChatXrmServiceContext(connection);
							connection.Authenticate();
							msg.TargetOrganizationService = connection;
							msg.TargetOrganizationServiceProxy = connection;
							context = new OrganizationServiceContext(connection);
							msg.TargetOrganizationServiceContext = context;
							callerId = Util.GetSystemUserId(xrm, msg.CallAgentId);
							if (callerId.HasValue)
							{
								msg.TargetOrganizationServiceProxy.set_CallerId(callerId.Value);
							}
						}
						else
						{
							isUdoOrg = true;
							connectionUdo.Authenticate();
							msg.TargetOrganizationService = connectionUdo;
							msg.TargetOrganizationServiceProxy = connectionUdo;
							context = new OrganizationServiceContext(connectionUdo);
							msg.TargetOrganizationServiceContext = context;
							msg.TargetOrganizationServiceProxy.set_CallerId(systemUserIdIfUdoChatUser.Value);
							callerId = systemUserIdIfUdoChatUser;
						}
					}
					if (connectionParms == null)
					{
						throw new Exception(string.Format("ConnectionParms was not found for OrganizationProxy {0}", chatOrg));
					}
					if (!callerId.HasValue)
					{
						throw new Exception(string.Format("CallAgentId {0} not found", msg.CallAgentId));
					}
					switch (isUdoOrg)
					{
						case false:
						{
							string previousSensitivity = null;
							if (xrm != null)
							{
								VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowsesessionlog session = xrm.CreateQuery<VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowsesessionlog>().FirstOrDefault<VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowsesessionlog>((VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowsesessionlog x) => x.crme_ChatSessionId == msg.ChatSessionId);
								if ((session == null || session.crme_SensitivityReason == null ? false : session.crme_SensitivityReason.get_Value() == 935950000))
								{
									previousSensitivity = session.crme_VeteranSensitivityLevel;
								}
								sensitivityLevel = previousSensitivity ?? "8";
								sensitivityLevelReason = 935950003;
								try
								{
									sensitivityLevel = Util.GetSensitivityLevel(msg.OrgName, callerId.Value, msg.Ssn, partId);
									if (string.IsNullOrEmpty(sensitivityLevel))
									{
										sensitivityLevel = previousSensitivity ?? "8";
									}
									else
									{
										sensitivityLevelReason = 935950000;
									}
								}
								catch (AccessViolationException accessViolationException)
								{
									sensitivityLevelReason = 935950001;
								}
								catch (Exception exception)
								{
									sensitivityLevelReason = 935950002;
								}
								if (session != null)
								{
									session.crme_CallAgentId = (msg.CallAgentId != null ? msg.CallAgentId : session.crme_CallAgentId);
									session.crme_Category = (msg.Category != null ? msg.Category : session.crme_Category);
									session.crme_ChatSessionId = (msg.ChatSessionId != null ? msg.ChatSessionId : session.crme_ChatSessionId);
									session.crme_EDIPI = (msg.Edipi != null ? msg.Edipi : session.crme_EDIPI);
									session.crme_Resolution = (msg.Resolution != null ? msg.Resolution : session.crme_Resolution);
									session.crme_SSN = (msg.Ssn != null ? msg.Ssn : session.crme_SSN);
									session.crme_ParticipantId = (msg.ParticipantId != null ? msg.ParticipantId : session.crme_ParticipantId);
									session.crme_VSOOrgId = (msg.VsoOrgId != null ? msg.VsoOrgId : session.crme_VSOOrgId);
									session.crme_VeteranSensitivityLevel = sensitivityLevel;
									session.crme_SensitivityReason = new OptionSetValue(sensitivityLevelReason);
									session.crme_ChatCompleted = new bool?(true);
									xrm.UpdateObject(session);
								}
								else
								{
									session = new VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowsesessionlog()
									{
										crme_SessionType = new OptionSetValue()
									};
									session.crme_SessionType.set_Value(935950001);
									session.crme_CallAgentId = msg.CallAgentId;
									session.crme_Category = msg.Category;
									session.crme_ChatSessionId = msg.ChatSessionId;
									session.crme_EDIPI = msg.Edipi;
									session.crme_Resolution = msg.Resolution;
									session.crme_SSN = msg.Ssn;
									session.crme_VSOOrgId = msg.VsoOrgId;
									session.crme_ParticipantId = msg.ParticipantId;
									session.OwnerId = new EntityReference("systemuser", callerId.Value);
									session.crme_VeteranSensitivityLevel = sensitivityLevel;
									session.crme_SensitivityReason = new OptionSetValue(sensitivityLevelReason);
									session.crme_ChatCompleted = new bool?(true);
									xrm.AddObject(session);
								}
								xrm.SaveChanges();
								if (string.IsNullOrEmpty(msg.ChatSessionLog))
								{
									msg.ChatSessionLog = "<html><body>No Chat Transcript Available.</body></html>";
								}
								VRM.Integration.Servicebus.Egain.CrmModel.Annotation annotation1 = new VRM.Integration.Servicebus.Egain.CrmModel.Annotation()
								{
									IsDocument = new bool?(true),
									Subject = "Authenticated Chat Transcript",
									ObjectId = new EntityReference("crme_chatcobrowsesessionlog", session.get_Id()),
									ObjectTypeCode = "crme_chatcobrowsesessionlog".ToLower(),
									OwnerId = new EntityReference("systemuser", callerId.Value)
								};
								VRM.Integration.Servicebus.Egain.CrmModel.Annotation annotation = annotation1;
								xrm.AddObject(annotation);
								annotation.MimeType = "text/html";
								annotation.NoteText = "Authenticated Chat Transcript";
								string chatSessionId = msg.ChatSessionId;
								now = DateTime.Now;
								annotation.FileName = string.Format("AuthenticatedChatTranscript-{0}-{1}.{2}", chatSessionId, now.ToString("yyyy-MM-dd-HH-mm-ss"), "html");
								annotation.DocumentBody = Convert.ToBase64String(Encoding.UTF8.GetBytes(msg.ChatSessionLog));
								xrm.UpdateObject(annotation);
								xrm.SaveChanges();
								msg.IsTargetUserUdoChatUser = isUdoOrg;
								break;
							}
							else
							{
								return;
							}
						}
						case true:
						{
							VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog sessionUdo = xrmUdo.CreateQuery<VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog>().FirstOrDefault<VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog>((VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog x) => x.crme_ChatSessionId == msg.ChatSessionId);
							if (sessionUdo != null)
							{
								sessionUdo.crme_CallAgentId = (msg.CallAgentId != null ? msg.CallAgentId : sessionUdo.crme_CallAgentId);
								sessionUdo.crme_Category = (msg.Category != null ? msg.Category : sessionUdo.crme_Category);
								sessionUdo.crme_ChatSessionId = (msg.ChatSessionId != null ? msg.ChatSessionId : sessionUdo.crme_ChatSessionId);
								sessionUdo.crme_EDIPI = (msg.Edipi != null ? msg.Edipi : sessionUdo.crme_EDIPI);
								sessionUdo.crme_Resolution = (msg.Resolution != null ? msg.Resolution : sessionUdo.crme_Resolution);
								sessionUdo.crme_SSN = (msg.Ssn != null ? msg.Ssn : sessionUdo.crme_SSN);
								sessionUdo.crme_ParticipantId = (msg.ParticipantId != null ? msg.ParticipantId : sessionUdo.crme_ParticipantId);
								sessionUdo.crme_VSOOrgId = (msg.VsoOrgId != null ? msg.VsoOrgId : sessionUdo.crme_VSOOrgId);
								sessionUdo.crme_ChatCompleted = new bool?(true);
								xrmUdo.UpdateObject(sessionUdo);
							}
							else
							{
								sessionUdo = new VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog()
								{
									crme_SessionType = new OptionSetValue()
								};
								sessionUdo.crme_SessionType.set_Value(935950001);
								sessionUdo.crme_CallAgentId = msg.CallAgentId;
								sessionUdo.crme_Category = msg.Category;
								sessionUdo.crme_ChatSessionId = msg.ChatSessionId;
								sessionUdo.crme_EDIPI = msg.Edipi;
								sessionUdo.crme_Resolution = msg.Resolution;
								sessionUdo.crme_SSN = msg.Ssn;
								sessionUdo.crme_VSOOrgId = msg.VsoOrgId;
								sessionUdo.crme_ParticipantId = msg.ParticipantId;
								sessionUdo.OwnerId = new EntityReference("systemuser", callerId.Value);
								sessionUdo.crme_ChatCompleted = new bool?(true);
								xrmUdo.AddObject(sessionUdo);
							}
							xrmUdo.SaveChanges();
							if (string.IsNullOrEmpty(msg.ChatSessionLog))
							{
								msg.ChatSessionLog = "<html><body>No Chat Transcript Available.</body></html>";
							}
							VRM.Integration.Servicebus.Egain.UdoCrmModel.Annotation annotation2 = new VRM.Integration.Servicebus.Egain.UdoCrmModel.Annotation()
							{
								IsDocument = new bool?(true),
								Subject = "Authenticated Chat Transcript",
								ObjectId = new EntityReference("crme_chatcobrowsesessionlog", sessionUdo.get_Id()),
								ObjectTypeCode = "crme_chatcobrowsesessionlog".ToLower(),
								OwnerId = new EntityReference("systemuser", callerId.Value)
							};
							VRM.Integration.Servicebus.Egain.UdoCrmModel.Annotation annotationUdo = annotation2;
							xrmUdo.AddObject(annotationUdo);
							annotationUdo.MimeType = "text/html";
							annotationUdo.NoteText = "Authenticated Chat Transcript";
							string str = msg.ChatSessionId;
							now = DateTime.Now;
							annotationUdo.FileName = string.Format("AuthenticatedChatTranscript-{0}-{1}.{2}", str, now.ToString("yyyy-MM-dd-HH-mm-ss"), "html");
							annotationUdo.DocumentBody = Convert.ToBase64String(Encoding.UTF8.GetBytes(msg.ChatSessionLog));
							xrmUdo.UpdateObject(annotationUdo);
							xrmUdo.SaveChanges();
							msg.IsTargetUserUdoChatUser = isUdoOrg;
							break;
						}
						default:
						{
							throw new ApplicationException("Unable to determine whether Chat user is migrated to UDO org yet");
						}
					}
				}
				catch (Exception exception1)
				{
					Logger.get_Instance().Debug(exception1.ToString());
					throw;
				}
			}
			finally
			{
				if (xrmUdo != null)
				{
					xrmUdo.Dispose();
				}
				if (connectionUdo != null)
				{
					connectionUdo.Dispose();
				}
				if (xrm != null)
				{
					xrm.Dispose();
				}
				if (connection != null)
				{
					connection.Dispose();
				}
				Logger.get_Instance().Debug("AuthChatRequestStep::Exiting CreateSessionStep");
			}
		}
	}
}