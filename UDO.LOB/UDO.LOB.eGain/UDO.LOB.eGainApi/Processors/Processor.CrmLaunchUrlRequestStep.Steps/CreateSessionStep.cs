using CRM007.CRM.SDK.Core;
using CuttingEdge.Conditions;
using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain;
using VRM.Integration.Servicebus.Egain.CrmModel;
using VRM.Integration.Servicebus.Egain.Messages.Messages;
using VRM.Integration.Servicebus.Egain.State;
using VRM.Integration.Servicebus.Egain.UdoCrmModel;

namespace VRM.Integration.Servicebus.Egain.Processor.CrmLaunchUrlRequestStep.Steps
{
	public class CreateSessionStep : FilterBase<ICrmLaunchUrlRequestState>
	{
		public CreateSessionStep()
		{
		}

		public override void Execute(ICrmLaunchUrlRequestState msg)
		{
			bool isUdoOrg;
			OrganizationServiceContext context;
			Exception ex;
			Guid? nullable;
			Guid empty;
			bool flag;
			bool flag1;
			CrmConnectionParms connectionParms = null;
			OrganizationServiceProxy connection = null;
			OrganizationServiceProxy connectionUdo = null;
			VRM.Integration.Servicebus.Egain.UdoCrmModel.ChatXrmServiceContext xrmUdo = null;
			VRM.Integration.Servicebus.Egain.CrmModel.ChatXrmServiceContext xrm = null;
			Guid? callerId = null;
			long? partId = null;
			long tempId = (long)0;
			if (long.TryParse(msg.Request.ParticipantId, out tempId))
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
					Logger.get_Instance().Debug("Calling CrmLaunchUrlRequestStep");
					ValidatorExtensions.IsNotNull<ICrmLaunchUrlRequestState>(Condition.Requires<ICrmLaunchUrlRequestState>(msg, "msg"));
					if (!msg.Request.OrgName.ToUpper().Contains("VRM"))
					{
						isUdoOrg = true;
						chatOrg = msg.Request.OrgName.ToUpper();
						connectionParms = CrmConnectionConfiguration.get_Current().GetCrmConnectionParmsByName(chatOrg);
						connectionUdo = CrmConnection.Connect(connectionParms, typeof(VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog).Assembly);
						xrmUdo = new VRM.Integration.Servicebus.Egain.UdoCrmModel.ChatXrmServiceContext(connectionUdo);
						callerId = Util.GetSystemUserIdIfUdoChatUser(xrmUdo, msg.Request.CallAgentId);
						connectionUdo.Authenticate();
						msg.TargetOrganizationService = connectionUdo;
						msg.TargetOrganizationServiceProxy = connectionUdo;
						context = new OrganizationServiceContext(connectionUdo);
						msg.TargetOrganizationServiceContext = context;
						if (!callerId.HasValue)
						{
							flag = true;
						}
						else
						{
							nullable = callerId;
							empty = Guid.Empty;
							flag = (!nullable.HasValue ? 1 : (int)(nullable.GetValueOrDefault() != empty)) == 0;
						}
						if (flag)
						{
							throw new ApplicationException(string.Format("CallAgentId {0} is either not found in {1} or is not marked as a UDO Chat User", msg.Request.CallAgentId, chatOrg));
						}
						isUdoOrg = true;
						msg.TargetOrganizationServiceProxy.set_CallerId(callerId.Value);
					}
					else
					{
						UdoChatSettingsConfiguration udoChatSettings = new UdoChatSettingsConfiguration(msg.Request.OrgName.ToUpper());
						chatOrg = ((udoChatSettings == null ? true : udoChatSettings.Current.UdoChatOrg == null) ? msg.Request.OrgName.ToUpper() : udoChatSettings.Current.UdoChatOrg);
						connectionParms = CrmConnectionConfiguration.get_Current().GetCrmConnectionParmsByName(chatOrg);
						connectionUdo = CrmConnection.Connect(connectionParms, typeof(VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog).Assembly);
						xrmUdo = new VRM.Integration.Servicebus.Egain.UdoCrmModel.ChatXrmServiceContext(connectionUdo);
						Guid? systemUserIdIfUdoChatUser = Util.GetSystemUserIdIfUdoChatUser(xrmUdo, msg.Request.CallAgentId);
						if (!systemUserIdIfUdoChatUser.HasValue)
						{
							flag1 = true;
						}
						else
						{
							nullable = systemUserIdIfUdoChatUser;
							empty = Guid.Empty;
							flag1 = (!nullable.HasValue ? 1 : (int)(nullable.GetValueOrDefault() != empty)) == 0;
						}
						if (flag1)
						{
							isUdoOrg = false;
							xrmUdo.Dispose();
							if (connectionUdo != null)
							{
								connectionUdo.Dispose();
							}
							chatOrg = msg.Request.OrgName.ToUpper();
							connectionParms = CrmConnectionConfiguration.get_Current().GetCrmConnectionParmsByName(chatOrg);
							connection = CrmConnection.Connect(connectionParms, typeof(VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowsesessionlog).Assembly);
							xrm = new VRM.Integration.Servicebus.Egain.CrmModel.ChatXrmServiceContext(connection);
							callerId = Util.GetSystemUserId(xrm, msg.Request.CallAgentId);
							connection.Authenticate();
							msg.TargetOrganizationService = connection;
							msg.TargetOrganizationServiceProxy = connection;
							context = new OrganizationServiceContext(connection);
							msg.TargetOrganizationServiceContext = context;
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
						throw new Exception(string.Format("CallAgentId {0} not found", msg.Request.CallAgentId));
					}
					bool isNewRecord = true;
					switch (isUdoOrg)
					{
						case false:
						{
							sensitivityLevel = "0";
							sensitivityLevelReason = 935950003;
							try
							{
								sensitivityLevel = Util.GetSensitivityLevel(chatOrg, callerId.Value, msg.Request.Ssn, partId);
								if (string.IsNullOrEmpty(sensitivityLevel))
								{
									sensitivityLevel = "0";
								}
								else
								{
									sensitivityLevelReason = 935950000;
								}
							}
							catch (AccessViolationException accessViolationException)
							{
								throw accessViolationException;
							}
							catch (Exception exception)
							{
								ex = exception;
								sensitivityLevelReason = 935950002;
							}
							if (xrm != null)
							{
								VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowsesessionlog existingSessionTest = xrm.CreateQuery<VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowsesessionlog>().FirstOrDefault<VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowsesessionlog>((VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowsesessionlog x) => x.crme_ChatSessionId == msg.Request.ChatSessionId);
								VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowsesessionlog existingSession = new VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowsesessionlog();
								if (existingSessionTest != null)
								{
									isNewRecord = false;
									existingSession.set_Id(existingSessionTest.get_Id());
									xrm.Detach(existingSessionTest);
									xrm.Attach(existingSession);
								}
								existingSession.crme_ChatSessionId = msg.Request.ChatSessionId;
								existingSession.crme_EDIPI = msg.Request.Edipi;
								existingSession.crme_ParticipantId = msg.Request.ParticipantId;
								existingSession.crme_SSN = msg.Request.Ssn;
								existingSession.crme_CallAgentId = msg.Request.CallAgentId;
								existingSession.crme_VSOOrgId = msg.Request.VsoOrgId;
								existingSession.crme_VeteranSensitivityLevel = sensitivityLevel;
								existingSession.crme_SensitivityReason = new OptionSetValue(sensitivityLevelReason);
								existingSession.crme_SessionType = new OptionSetValue();
								if (!(!string.IsNullOrEmpty(msg.Request.Edipi) || !string.IsNullOrEmpty(msg.Request.Ssn) ? true : !string.IsNullOrEmpty(msg.Request.ParticipantId)))
								{
									existingSession.crme_SessionType.set_Value(935950000);
								}
								else if ((!string.IsNullOrEmpty(msg.Request.Edipi) || !string.IsNullOrEmpty(msg.Request.Ssn) ? true : !string.IsNullOrEmpty(msg.Request.ParticipantId)))
								{
									existingSession.crme_SessionType.set_Value(935950001);
								}
								if (!isNewRecord)
								{
									xrm.UpdateObject(existingSession);
								}
								else
								{
									xrm.AddObject(existingSession);
								}
								xrm.SaveChanges();
								var existingSessionQuery = (
									from x in xrm.crme_chatcobrowsesessionlogSet
									where x.crme_ChatSessionId == msg.Request.ChatSessionId
									select new { crme_CallAgentId = x.crme_CallAgentId, crme_ChatCompleted = x.crme_ChatCompleted, crme_ChatSessionId = x.crme_ChatSessionId, crme_EDIPI = x.crme_EDIPI, crme_LaunchUrl = x.crme_LaunchUrl, crme_SessionType = x.crme_SessionType }).FirstOrDefault();
								if (existingSessionQuery != null)
								{
									if ((string.IsNullOrEmpty(existingSessionQuery.crme_LaunchUrl) ? true : existingSessionQuery.crme_SessionType.get_Value() == 935950000))
									{
										if (xrm != null)
										{
											VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowseconfig config = xrm.CreateQuery<VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowseconfig>().FirstOrDefault<VRM.Integration.Servicebus.Egain.CrmModel.crme_chatcobrowseconfig>();
											if (config != null)
											{
												msg.CrmLaunchUrl = config.crme_DefaultURL;
												return;
											}
										}
										else
										{
											return;
										}
									}
									msg.CrmLaunchUrl = existingSessionQuery.crme_LaunchUrl;
									msg.IsTargetUserUdoChatUser = isUdoOrg;
									Logger.get_Instance().Debug(string.Concat("Returning url: ", msg.CrmLaunchUrl));
								}
								break;
							}
							else
							{
								return;
							}
						}
						case true:
						{
							VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog existingSessionTestUdo = xrmUdo.CreateQuery<VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog>().FirstOrDefault<VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog>((VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog x) => x.crme_ChatSessionId == msg.Request.ChatSessionId);
							VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog existingSessionUdo = new VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowsesessionlog();
							if (existingSessionTestUdo != null)
							{
								isNewRecord = false;
								existingSessionUdo.set_Id(existingSessionTestUdo.get_Id());
								xrmUdo.Detach(existingSessionTestUdo);
								xrmUdo.Attach(existingSessionUdo);
							}
							existingSessionUdo.crme_ChatSessionId = msg.Request.ChatSessionId;
							existingSessionUdo.crme_EDIPI = msg.Request.Edipi;
							existingSessionUdo.crme_ParticipantId = msg.Request.ParticipantId;
							existingSessionUdo.crme_SSN = msg.Request.Ssn;
							existingSessionUdo.crme_CallAgentId = msg.Request.CallAgentId;
							existingSessionUdo.crme_VSOOrgId = msg.Request.VsoOrgId;
							existingSessionUdo.crme_SessionType = new OptionSetValue();
							if (!(!string.IsNullOrEmpty(msg.Request.Edipi) || !string.IsNullOrEmpty(msg.Request.Ssn) ? true : !string.IsNullOrEmpty(msg.Request.ParticipantId)))
							{
								existingSessionUdo.crme_SessionType.set_Value(935950000);
							}
							else if ((!string.IsNullOrEmpty(msg.Request.Edipi) || !string.IsNullOrEmpty(msg.Request.Ssn) ? true : !string.IsNullOrEmpty(msg.Request.ParticipantId)))
							{
								existingSessionUdo.crme_SessionType.set_Value(935950001);
							}
							if (!isNewRecord)
							{
								xrmUdo.UpdateObject(existingSessionUdo);
							}
							else
							{
								xrmUdo.AddObject(existingSessionUdo);
							}
							xrmUdo.SaveChanges();
							var existingSessionQueryUdo = (
								from x in xrmUdo.crme_chatcobrowsesessionlogSet
								where x.crme_ChatSessionId == msg.Request.ChatSessionId
								select new { crme_CallAgentId = x.crme_CallAgentId, crme_ChatCompleted = x.crme_ChatCompleted, crme_ChatSessionId = x.crme_ChatSessionId, crme_EDIPI = x.crme_EDIPI, crme_LaunchUrl = x.crme_LaunchUrl, crme_SessionType = x.crme_SessionType }).FirstOrDefault();
							if (existingSessionQueryUdo != null)
							{
								if ((string.IsNullOrEmpty(existingSessionQueryUdo.crme_LaunchUrl) ? true : existingSessionQueryUdo.crme_SessionType.get_Value() == 935950000))
								{
									VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowseconfig config = xrmUdo.CreateQuery<VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowseconfig>().FirstOrDefault<VRM.Integration.Servicebus.Egain.UdoCrmModel.crme_chatcobrowseconfig>();
									if (config != null)
									{
										msg.CrmLaunchUrl = config.crme_DefaultURL;
										return;
									}
								}
								msg.CrmLaunchUrl = existingSessionQueryUdo.crme_LaunchUrl;
								msg.IsTargetUserUdoChatUser = isUdoOrg;
								Logger.get_Instance().Debug(string.Concat("Returning url: ", msg.CrmLaunchUrl));
							}
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
					ex = exception1;
					Logger.get_Instance().Debug(ex.ToString());
					for (Exception iex = ex.InnerException; iex != null; iex = iex.InnerException)
					{
						Logger.get_Instance().Debug(iex.ToString());
					}
					throw;
				}
				return;
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
				Logger.get_Instance().Debug("CrmLaunchUrlRequest::Exiting ConnectToTargetCrmStep");
			}
		}
	}
}