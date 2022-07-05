using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Desktop.Core;
using Microsoft.Uii.Desktop.SessionManager;
using System.Collections.Generic;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Microsoft.Xrm.Sdk;
using Va.Udo.Usd.CustomControls.Shared;

namespace Va.Udo.Usd.CustomControls
{
    /// <summary>
    ///     Interaction logic for SessionController.xaml
    /// </summary>
    public partial class SessionController : BaseHostedControlCommon
    {
        public SessionController()
        {
            InitializeComponent();
        }

        public SessionController(Guid appId, string name, string init)
            : base(appId, name, init)
        {
            InitializeComponent();
            //this.localSessionManager.SessionCloseEvent += localSessionManager_SessionCloseEvent;
            //this.localSessionManager.SessionShowEvent += localSessionManager_SessionShowEvent;
        }

        private bool ForceSessionClose(SessionCloseActions action = SessionCloseActions.CloseAppsAndCloseSession, List<string> globalparmstoremove = null)
        {
            return ForceSessionClose(null, action, globalparmstoremove);
        }

        private void RequestCloseSession(Guid? SessionId, bool newcall)
        {
            UpdateDataParameters("SessionController", "ValidationComplete", "0");
            UpdateDataParameters("SessionController", "CloseSessionValidations", "0");
            UpdateDataParameters("SessionController", "CloseSessionFailures", "0");
            UpdateDataParameters("SessionController", "NewCall", newcall ? "1" : "0");

            var sessionMgr = localSessionManager;
            var session = SessionId.HasValue && SessionId.Value != Guid.Empty
                ? sessionMgr.GetSession(SessionId.Value)
                : sessionMgr.ActiveSession;

            if (session == null || session.Global) return;

            FireEvent("SessionCloseRequested");
        }

        private bool ForceSessionClose(Guid? SessionId,
            SessionCloseActions action = SessionCloseActions.CloseAppsAndCloseSession,
            List<String> globalparmstoremove = null)
        {
            FireEvent("BeforeSessionClose");
            FireEvent("SessionClosing");

            //cleanup action
            if (action.HasFlag(SessionCloseActions.CloseSession))
                action |= SessionCloseActions.CloseAppsAndCloseSession;

            var sessionMgr = localSessionManager;
            var session = SessionId.HasValue && SessionId.Value != Guid.Empty
                ? sessionMgr.GetSession(SessionId.Value)
                : sessionMgr.ActiveSession;

            if (session == null || session.Global)
            {
                if (action.HasFlag(SessionCloseActions.OpenNewSession))
                {
                    FireEvent("CreateSession");
                    return true;
                }
                return false;
            }

            var agentSession = session as AgentDesktopSession;
            // if agentSession is null, the variables are not cleared.
            if (!action.HasFlag(SessionCloseActions.ClearSessionData)) agentSession = null;
            DynamicsCustomerRecord customerRecord = null;

            if (agentSession != null) customerRecord = (DynamicsCustomerRecord)agentSession.Customer.DesktopCustomer;


            var apps = session.OfType<IHostedApplication>().Where((iapp) =>
            {
                var app = iapp as HostedApplication;
                var dyn = iapp as DynamicsBaseHostedControl;
                if (app != null && app.IsGlobal) return false;
                if (dyn != null && dyn.IsGlobal) return false;
                return true;
            });

            if (session.IsWorkflowPending && session.OfType<IWorkflow>().Any(w => w.IsForced))
            {
                return false;
            }

            Task.WhenAll(apps.Select(iapp =>
                Task.Run(() =>
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        if (customerRecord != null && action.HasFlag(SessionCloseActions.ClearSessionData))
                        {
                            customerRecord.ClearReplaceableParameter(iapp.ApplicationName);
                            //iapp.Close();
                            session.AppHost.UnloadDynamicApplication(iapp);
                        }
                    });
                })
            ));



            if (localSessionManager.Count <= 1
                && customerRecord != null
                && action.HasFlag(SessionCloseActions.ClearSessionData)
                && globalparmstoremove != null)
            {
                var globalAgentSession = sessionMgr.GlobalSession as AgentDesktopSession;
                if (globalAgentSession != null)
                {
                    if (globalAgentSession.Customer != null && globalAgentSession.Customer.DesktopCustomer != null)
                    {
                        var globalCustomerRecord = globalAgentSession.Customer.DesktopCustomer as DynamicsCustomerRecord;
                        if (globalCustomerRecord != null)
                        {
                            foreach (var globalparam in globalparmstoremove)
                            {
                                globalCustomerRecord.ClearReplaceableParameter(globalparam);
                            }
                            //globalCustomerRecord.ClearReplaceableParameter("$Panel");
                        }
                    }
                }

                customerRecord.Entities.Clear();
                customerRecord.EntityResults.Clear();
                customerRecord.CapturedReplacementVariables.Clear();
                customerRecord.CtiData.Clear();
                customerRecord.Dispose();
                agentSession.Customer.DesktopCustomer = null;
            }

            if (action.HasFlag(SessionCloseActions.CloseSession))
            {
                //Is this possible?
                //if (FireEvent("CloseSession"))
                //{
                desktopAccess.CloseSession(session);
                FireEvent("AfterSessionClose");
                FireEvent("SessionClosed");
                //}
            }

            if (action.HasFlag(SessionCloseActions.OpenNewSession))
            {
                FireEvent("CreateSession");
            }

            return true;
        }

        private void UpdateDataParameters(string nodeName, string key, string value)
        {
            var lri = new LookupRequestItem
            {
                Key = key,
                Value = value
            };

            var lriList = new List<LookupRequestItem> { lri };

            var dcr =
                (DynamicsCustomerRecord)
                    ((AgentDesktopSession)localSessionManager.ActiveSession).Customer.DesktopCustomer;
            dcr.MergeReplacementParameter(nodeName, lriList, true);

            var parms = new Dictionary<string, string>();
            parms.Add("node", nodeName);
            parms.Add("key", key);

            var validationComplete = ("1" == base.GetDataParameter("SessionController", "ValidationComplete"));

            if (validationComplete)
            {
                var condition = GetDataParameter("SessionController", "ValidationCondition");
                if (!String.IsNullOrWhiteSpace(condition))
                {

                    condition = condition.Trim().Replace("{", "[").Replace("}", "]");

                    AgentDesktopSession agentSession = this.localSession as AgentDesktopSession;

                    //added nullchecks to escape null dereferencing
                    Context context = null;
                    if(agentSession != null)
                        context = agentSession.AppHost.GetContext();
                    if (context != null)
                    {
                        condition = Utility.GetContextReplacedString(condition, context, dcr, new Dictionary<string, string>());

                        var result = Javascript.EvaluateScript(condition, true);

                        var validated = (result is bool && (bool)result);

                        if (validated) FireEvent("ValidatedSessionClose");
                    }
                }
            }
            FireEvent("UpdatedDataParameter", parms);
        }

        //private string GetDataParameter(string nodeName, string key)
        //{
        //    var dcr =
        //        (DynamicsCustomerRecord)
        //            ((AgentDesktopSession)localSessionManager.ActiveSession).Customer.DesktopCustomer;

        //    if (!dcr.CapturedReplacementVariables.ContainsKey(nodeName)) return null;

        //    var node = dcr.CapturedReplacementVariables[nodeName];

        //    if (node != null && node.ContainsKey(key)) return node[key].value;

        //    return null;
        //}

        private void IncrementCounter(string nodeName, string key, string amount)
        {
            var value = GetDataParameter(nodeName, key);
            int num = 1;
            if (!String.IsNullOrEmpty(value) && int.TryParse(value, out num))
            {
                int incr = 1;
                if (!String.IsNullOrWhiteSpace(amount) && int.TryParse(amount, out incr))
                {
                    num += incr;
                }
                else
                {
                    num++;
                }
            }

            UpdateDataParameters(nodeName, key, num.ToString());
        }

        protected override void DoAction(RequestActionEventArgs args)
        {
            switch (args.Action)
            {
                case "ValidationComplete":
                    UpdateDataParameters("SessionController", "ValidationComplete", "1");
                    return;
                case "ClearCloseSessionFailures":
                    UpdateDataParameters("SessionController", "CloseSessionFailures", "0");
                    return;
                case "IncrementCloseSessionFailures":
                    var parms1 = Utility.SplitLines(args.Data, CurrentContext, localSession);
                    string value1 = string.Empty;
                    if (parms1 != null) value1 = Utility.GetAndRemoveParameter(parms1, "value");
                    IncrementCounter("SessionController", "CloseSessionFailures", value1);
                    return;
                case "ClearCloseSessionValidations":
                    UpdateDataParameters("SessionController", "CloseSessionValidations", "0");
                    return;
                case "IncrementCloseSessionValidations":
                    var parms2 = Utility.SplitLines(args.Data, CurrentContext, localSession);
                    string value2 = string.Empty;
                    if (parms2 != null) value1 = Utility.GetAndRemoveParameter(parms2, "value");
                    IncrementCounter("SessionController", "CloseSessionValidations", value2);
                    return;
                case "IncrementCounter":
                case "UpdateParameter":
                    List<KeyValuePair<string, string>> parms;
                    if (args.Data.Contains("\n") || args.Data.Contains("\r\n"))
                    {
                        parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
                    }
                    else
                    {
                        // no line breaks, so we should split on & instead.
                        var lines = args.Data.Split('&');

                        parms = new List<KeyValuePair<string, string>>();
                        foreach (var line in lines)
                        {
                            var lineitems = line.Split('=');
                            if (lineitems.Count() >= 2)
                            {
                                parms.Add(new KeyValuePair<string, string>(lineitems[0], lineitems[1]));
                            }
                        }
                    }
                    var node = Utility.GetAndRemoveParameter(parms, "node");
                    if (String.IsNullOrEmpty(node)) node = "SessionController";
                    var key = Utility.GetAndRemoveParameter(parms, "key");
                    var value = Utility.GetAndRemoveParameter(parms, "value");

                    if (args.Action == "IncrementCounter")
                    {
                        if (String.IsNullOrEmpty(value)) value = "1";
                        IncrementCounter(node, key, value);
                        return;
                    }
                    if (String.IsNullOrEmpty(value)) value = "0";
                    UpdateDataParameters(node, key, value);
                    return;
                case "RequestCloseSession":
                    RequestCloseSession(null, false); // FireEvent("SessionCloseRequested")
                    // When it's done USD (not code here) should call the CloseSession Action.
                    return;
                case "RequestNewCall":
                    RequestCloseSession(null, true);  // FireEvent("SessionCloseRequested");
                    FireEvent("NewCallRequested");
                    return;
                case "CloseSession":
                    ForceSessionClose(SessionCloseActions.CloseAppsAndCloseSession, GetParameterLines(args));
                    return;
                case "NewCall":
                    var parms4 = Utility.SplitLines(args.Data, CurrentContext, localSession);
                    //FireEvent("CreateSession");
                    ForceSessionClose(SessionCloseActions.CloseSessionAndOpenNewSession, GetParameterLines(args));
                    return;
                case "SessionCloseUpdateRequestEndTime":
                    UpdateRequestEndTime(args);
                    return;
                case "SessionCloseUpdateInteractionEndTime":
                    UpdateInteractionEndTime(args);
                    return;
            }
            base.DoAction(args);
        }

        private List<string> GetParameterLines(RequestActionEventArgs args)
        {
            var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
            if (parms == null) return null;
            var remainderlines = Utility.RemainderParameter(parms);
            if (String.IsNullOrWhiteSpace(remainderlines)) return null;
            return remainderlines.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private void UpdateRequestEndTime(RequestActionEventArgs args)
        {
            var parms5 = Utility.SplitLines(args.Data, CurrentContext, localSession);
            if (parms5 == null) return;
            var requestId = Utility.GetAndRemoveParameter(parms5, "RequestId");

            if (string.IsNullOrEmpty(requestId)) return;
            var updateEntity = new Entity("udo_request") { Id = new Guid(requestId) };
            updateEntity["udo_endtime"] = DateTime.Now;

            base.Update(updateEntity);
        }

        private void UpdateInteractionEndTime(RequestActionEventArgs args)
        {
            var parms6 = Utility.SplitLines(args.Data, CurrentContext, localSession);
            if (parms6 == null) return;
            var interactionId = Utility.GetAndRemoveParameter(parms6, "InteractionId");

            if (string.IsNullOrEmpty(interactionId)) return;

            var updateEntity = new Entity("udo_interaction") { Id = new Guid(interactionId) };
            updateEntity["udo_endtime"] = DateTime.Now;
            updateEntity["udo_status"] = false;

            base.Update(updateEntity);
        }


        [Flags]
        private enum SessionCloseActions
        {
            CloseApplications = 1 << 0,
            ClearSessionData = 1 << 1,
            CloseSession = 1 << 2,
            OpenNewSession = 1 << 3,
            CloseAppsAndCloseSession = (1 << 0) | (1 << 1) | (1 << 2),
            CloseSessionAndOpenNewSession = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3)
        }
    }
}