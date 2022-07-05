using System;
using System.Collections.Generic;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Desktop.SessionManager;
using Va.Udo.Usd.CustomControls.Shared;

namespace Va.Udo.Usd.CustomControls
{
    /// <summary>
    /// Interaction logic for RoleSecurity.xaml
    /// </summary>
    public partial class CtiProcessor : BaseHostedControlCommon
    {
        private readonly TraceLogger _logWriter;
        private string _datanodename = "CTI";
        private string _processingstep;

        public CtiProcessor(Guid id, string applicationName, string initXml)
            : base(id, applicationName, initXml)
        {
            InitializeComponent();
            _logWriter = new TraceLogger();
        }

        protected override void DesktopReady()
        {
            base.DesktopReady();
        }

        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            _processingstep = string.Format("{0} > started", args.Action);

            try
            {
                if (string.Compare(args.Action, "CheckChatActiveSessions", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    CheckChatActiveSessions(args);
                }
                else if (string.Compare(args.Action, "SetChatOnActiveSession", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    SetChatOnActiveSession(args);
                }
                else if (string.Compare(args.Action, "SetActiveSession", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    SetActiveSession(args);
                }
                else if (string.Compare(args.Action, "CloseIEWindowForLocalHost", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    CloseIEWindowForLocalHost(args);
                }
                else if (string.Compare(args.Action, "CheckSessionCount", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    CheckSessionCount(args);
                }
                else if (string.Compare(args.Action, "GetSessionIdBasedOnChatSessionLogId", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    GetSessionIdBasedOnChatSessionLogId(args);
                }

                base.DoAction(args);
                base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "Processing Step", _processingstep);
            }
            catch (Exception ex)
            {
                base.UpdateContext(_datanodename, "Processing Step", _processingstep);
                var exp = ExceptionManager.ReportException(ex);

                base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "ErrorOccurred", "Y");
                base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "ErrorOccurredIn", args.Action);
                base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "ExceptionMessage", ex.Message);
                base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "ExceptionReport", exp);
            }
        }

        private void CheckChatActiveSessions(RequestActionEventArgs args)
        {
            var rcdFound = false;
            var sessionId = "";
            var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
            var datanodename = Utility.GetAndRemoveParameter(parms, "DataNodeName");
            if (!string.IsNullOrEmpty(datanodename))
            {
                _datanodename = datanodename;
            }
            var interactionId = Utility.GetAndRemoveParameter(parms, "InteractionId");
            var chatSessionId = Utility.GetAndRemoveParameter(parms, "ChatSessionId");

            if (localSessionManager.Count > 0)
            {
                foreach (var sess in localSessionManager)
                {
                    _processingstep = "CheckChatActiveSessions get localsession";
                    var currSess = (AgentDesktopSession)sess;
                    if (!currSess.Global)
                    {
                        _processingstep = "CheckChatActiveSessions ChatSessionId - " + chatSessionId;
                        if (!string.IsNullOrEmpty(chatSessionId))
                        {
                            _processingstep = "CheckChatActiveSessions CtiCallRefIdChat - " + currSess.CtiCallRefIdChat;
                            if (currSess.CtiCallRefIdChat == new Guid(chatSessionId))
                            {
                                rcdFound = true;
                                sessionId = currSess.SessionId.ToString();
                                break;
                            }
                        }
                    }
                }
            }

            if (!rcdFound)
            {
                base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "ChatExists", "False");
                base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "SessionId", "");
                args.ActionReturnValue = "False";
            }
            else
            {
                base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "ChatExists", "True");
                base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "SessionId", sessionId);
                args.ActionReturnValue = "True";
            }

            base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "Processing Step", _processingstep);
            base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "ChatSessionId", chatSessionId);
            base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "InteractionId", interactionId);

        }

        private void SetChatOnActiveSession(RequestActionEventArgs args)
        {
            var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
            var datanodename = Utility.GetAndRemoveParameter(parms, "DataNodeName");
            if (!string.IsNullOrEmpty(datanodename))
            {
                _datanodename = datanodename;
            }
            var interactionId = Utility.GetAndRemoveParameter(parms, "InteractionId");
            var chatSessionId = Utility.GetAndRemoveParameter(parms, "ChatSessionId");

            if (localSessionManager.Count > 0)
            {
                foreach (var sess in localSessionManager)
                {
                    _processingstep = "SetChatOnActiveSession get localsession";
                    var currSess = (AgentDesktopSession)sess;

                    if (!currSess.Global)
                    {
                        if (currSess.SessionId == localSessionManager.ActiveSession.SessionId)
                        {
                            if (chatSessionId != null)
                            {
                                base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "ChatSet", "True");
                                currSess.CtiCallRefIdChat = new Guid(chatSessionId);
                                currSess.CtiCallType = "CHAT";
                                base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "Processing Step", _processingstep);
                                break;
                            }
                        }
                    }
                }
            }

            base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "ChatSessionId", chatSessionId);
            base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "InteractionId", interactionId);

        }

        private void SetActiveSession(RequestActionEventArgs args)
        {
            var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
            var datanodename = Utility.GetAndRemoveParameter(parms, "DataNodeName");
            if (!string.IsNullOrEmpty(datanodename))
            {
                _datanodename = datanodename;
            }
            var sessionId = Utility.GetAndRemoveParameter(parms, "SessionId");

            if (localSessionManager.ActiveSession.SessionId != new Guid(sessionId))
            {
                localSessionManager.SetActiveSession(new Guid(sessionId));

                var actionParms = new Dictionary<string, string>()
                {
                    {"SessionId", sessionId}
                };

                FireEvent("SwitchSession", actionParms);
            }
        }

        private void CloseIEWindowForLocalHost(RequestActionEventArgs args)
        {
            var closed = "N";
            // Get all IEXPLORE processes.
            System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName("IEXPLORE");
            foreach (System.Diagnostics.Process proc in procs)
            {
                // Look for localhost:5000 title.
                if (proc.MainWindowTitle.IndexOf("localhost:5000") > -1)
                {
                    if (proc.CloseMainWindow())
                        closed = "Y";
                }
            }
            base.UpdateSessionContext(localSessionManager.GlobalSession, "CTI", "ClosedBrowser", closed);
            args.ActionReturnValue = closed;
        }

        private void CheckSessionCount(RequestActionEventArgs args)
        {
            var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
            var datanodename = Utility.GetAndRemoveParameter(parms, "DataNodeName");
            if (!string.IsNullOrEmpty(datanodename))
            {
                _datanodename = datanodename;
            }

            var chatSessionId = Utility.GetAndRemoveParameter(parms, "ChatSessionLogId");
            var nonchatmaxnumberofsessionsStr = Utility.GetAndRemoveParameter(parms, "NonChatMaxNumberOfSessions");
            var maxnumberofsessionsStr = Utility.GetAndRemoveParameter(parms, "MaxNumberOfSessions");

            var nonchatmaxnumberofsessions = 0;
            if (!int.TryParse(nonchatmaxnumberofsessionsStr, out nonchatmaxnumberofsessions))
            {
                throw new Exception("Invalid Non Chat Max Number of Sessions value");
            }

            var maxnumberofsessions = 0;
            if (!int.TryParse(maxnumberofsessionsStr, out maxnumberofsessions))
            {
                throw new Exception("Invalid Max Number of Sessions value");
            }

            if (string.IsNullOrEmpty(chatSessionId))
            {
                var sessionFound = false;
                string sessionId;
                var mycount = RetrieveSessionCount(string.Empty, out sessionFound, out sessionId);

                if (mycount >= nonchatmaxnumberofsessions)
                {
                    base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "ChatExists", "False");
                    base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "IsSessionValid", "Invalid");
                    args.ActionReturnValue = "Invalid";
                }
                else
                {
                    base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "ChatExists", "False");
                    base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "IsSessionValid", "Valid");
                    args.ActionReturnValue = "Valid";
                }
            }
            else
            {
                var sessionFound = false;
                string sessionId;
                var mycount = RetrieveSessionCount(chatSessionId, out sessionFound, out sessionId);

                if (sessionFound)
                {
                    base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "ChatExists", "True");
                    base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "IsSessionValid", "Valid");
                    args.ActionReturnValue = "Valid";
                }
                else
                {
                    if (mycount >= maxnumberofsessions)
                    {
                        base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "ChatExists", "False");
                        base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "IsSessionValid", "Invalid");
                        args.ActionReturnValue = "Invalid";
                    }
                    else
                    {
                        base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "ChatExists", "False");
                        base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "IsSessionValid", "Valid");
                        args.ActionReturnValue = "Valid";
                    }
                }
            }
        }

        private void GetSessionIdBasedOnChatSessionLogId(RequestActionEventArgs args)
        {
            var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
            var datanodename = Utility.GetAndRemoveParameter(parms, "DataNodeName");
            if (!string.IsNullOrEmpty(datanodename))
            {
                _datanodename = datanodename;
            }

            var chatSessionId = Utility.GetAndRemoveParameter(parms, "ChatSessionLogId");

            if (!string.IsNullOrEmpty(chatSessionId))
            {

                var sessionFound = false;
                string sessionId;
                var mycount = RetrieveSessionCount(chatSessionId, out sessionFound, out sessionId);

                if (sessionFound)
                {
                    base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "SessionId", sessionId);
                    args.ActionReturnValue = sessionId;
                }
                else
                {
                    base.UpdateSessionContext(localSessionManager.GlobalSession, _datanodename, "SessionId", string.Empty);
                    args.ActionReturnValue = string.Empty;
                }
            }
        }

        private int RetrieveSessionCount(string chatSessionId, out bool sessionIdFound, out string sessionId)
        {
            var sessCount = 0;
            sessionIdFound = false;
            sessionId = string.Empty;

            if (localSessionManager.Count <= 0) return sessCount;
            foreach (var sess in localSessionManager)
            {
                var currSess = (AgentDesktopSession)sess;
                if (currSess.Global) continue;
                sessCount++;
                if (string.IsNullOrEmpty(chatSessionId)) continue;
                if (currSess.CtiCallRefIdChat == new Guid(chatSessionId))
                {
                    sessionIdFound = true;
                    sessionId = currSess.SessionId.ToString();
                }
            }

            return sessCount;
        }

    }
}