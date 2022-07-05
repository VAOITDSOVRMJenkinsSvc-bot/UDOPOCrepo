using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.Uii.Desktop.SessionManager;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Controls.Styles;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Va.Udo.Usd.CustomControls.Listerners;
using AuthenticationType = Microsoft.Xrm.Tooling.Connector.AuthenticationType;
using Microsoft.Uii.Csr;
using Microsoft.Win32;
using System.Management;

namespace Va.Udo.Usd.CustomControls.Shared
{
    public abstract class BaseHostedControlCommon : DynamicsBaseHostedControl
    {
      
        public delegate void ActionHandler(RequestActionEventArgs args);
        Dictionary<string, ActionHandler> registeredActions = new Dictionary<string, ActionHandler>();

        private static readonly ColumnSet UiiOptionColumnSet = new ColumnSet(
            "uii_name",
            "uii_value");

        private readonly TraceLogger _logWriter;

        protected BaseHostedControlCommon()
        {
            _logWriter = new TraceLogger();
            _logWriter.RefreshListeners(new List<TraceSourceSetting>());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicationName"></param>
        /// <param name="initXml"></param>
        protected BaseHostedControlCommon(Guid id, string applicationName, string initXml) :
            base(id, applicationName, initXml)
        {
            _logWriter = new TraceLogger("BaseHostedControlCommon");
            
        }

        protected override void SessionCreatedEvent(Session session)
        {
            base.SessionCreatedEvent(session);
        }

        protected override bool SessionCloseEvent(Session session)
        {
            return base.SessionCloseEvent(session);
        }       

        protected void RegisterAction(string name, ActionHandler handler)
        {
            string lowername = name.ToLower();
            lock (registeredActions)
            {
                if (registeredActions.ContainsKey(lowername))
                    registeredActions.Remove(lowername);
                registeredActions.Add(lowername, handler);
            }
        }

        protected override void DoAction(RequestActionEventArgs args)
        {

            //string action = args.Action.ToLower();
            //lock (registeredActions)
            //{
            //    if (registeredActions.ContainsKey(action))
            //    {
            //        registeredActions[action].Invoke(args);
            //        return;
            //    }
            //}

            //base.DoAction(args);

            var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);

            if (string.Compare(args.Action, "UpdateContext", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region UpdateContext

                var dataNodeName = Utility.GetAndRemoveParameter(parms, "datanodename");
                var key = Utility.GetAndRemoveParameter(parms, "key");
                var value = Utility.GetAndRemoveParameter(parms, "value");
                var strs = new Dictionary<string, string>();

                foreach (var parm in parms)
                {
                    strs.Add(parm.Key, parm.Value);
                }

                if (!string.IsNullOrEmpty(key))
                {
                    UpdateSessionContext(localSessionManager.ActiveSession, dataNodeName, key, value);
                }

                if (strs.Count > 0)
                {
                    UpdateSessionContext(localSessionManager.ActiveSession, dataNodeName, strs);
                }

                #endregion
            }
            if (string.Compare(args.Action, "UpdateGlobalSessionContext", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region UpdateGlobalSessionContext

                var dataNodeName = Utility.GetAndRemoveParameter(parms, "datanodename");
                var key = Utility.GetAndRemoveParameter(parms, "key");
                var value = Utility.GetAndRemoveParameter(parms, "value");
                var strs = new Dictionary<string, string>();

                foreach (var parm in parms)
                {
                    strs.Add(parm.Key, parm.Value);
                }

                if (!string.IsNullOrEmpty(key))
                {
                    UpdateSessionContext(localSessionManager.GlobalSession, dataNodeName, key, value);
                    args.ActionReturnValue = value;
                }

                if (strs.Count > 0)
                {
                    UpdateSessionContext(localSessionManager.GlobalSession, dataNodeName, strs);
                }

                #endregion
            }
            if (string.Compare(args.Action, "ClearGlobalSessionContext", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region ClearGlobalSessionContext
                var dataNodeName = Utility.GetAndRemoveParameter(parms, "datanodename");

                ClearSessionContext(localSessionManager.GlobalSession, dataNodeName);
                #endregion
            }
            else if (string.Compare(args.Action, "GetUrlParameterValue", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region Get URL Parameter Value
                var url = Utility.GetAndRemoveParameter(parms, "url");
                var parm = Utility.GetAndRemoveParameter(parms, "parm");
                var dataNodeName = Utility.GetAndRemoveParameter(parms, "datanodename");
                var key = Utility.GetAndRemoveParameter(parms, "key");

                var returnValue = GetUrlParameterValue(url, parm);
                UpdateContext(dataNodeName, key, returnValue);
                #endregion
            }
            else if (string.Compare(args.Action, "SetState", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region SetState
                var logicalName = Utility.GetAndRemoveParameter(parms, "logicalName");
                var id = Utility.GetAndRemoveParameter(parms, "id");
                var stateCode = Utility.GetAndRemoveParameter(parms, "stateCode");
                var statusCode = Utility.GetAndRemoveParameter(parms, "statusCode");

                var entityRef = new EntityReference(logicalName, new Guid(id));
                var ss = new SetStateRequest()
                {
                    EntityMoniker = entityRef,
                    State = new OptionSetValue(int.Parse(stateCode)),
                    Status = new OptionSetValue(int.Parse(statusCode))
                };

                try
                {
                    SetState(ss);
                }
                catch (Exception ex)
                {
                    var datanodename = "setstateerror";
                    _logWriter.Log(ex);
                    UpdateContext(datanodename, "ErrorOccurredIn", "SetState");
                    UpdateContext(datanodename, "ExceptionMessage", ex.Message);
                }
                #endregion
            }
            else if (string.Compare(args.Action, "AssignTeamOwnerFromVeteranField", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region Assign Team Owner Veteran Field
                var lookupLogicalName = Utility.GetAndRemoveParameter(parms, "LogicalName");
                var lookupId = Utility.GetAndRemoveParameter(parms, "Id");
                var lookupField = Utility.GetAndRemoveParameter(parms, "VeteranField");
                var dataNodeName = Utility.GetAndRemoveParameter(parms, "datanodename");

                UpdateContext(dataNodeName, "LogicalName", "");
                UpdateContext(dataNodeName, "Id", "");
                UpdateContext(dataNodeName, "Name", "");
                var owningTeam = AssignTeamOwnerFromVeteranField(lookupLogicalName, new Guid(lookupId), lookupField, dataNodeName);
                UpdateContext(dataNodeName, "LogicalName", owningTeam.LogicalName);
                UpdateContext(dataNodeName, "Id", owningTeam.Id.ToString());
                UpdateContext(dataNodeName, "Name", owningTeam.Name);
                #endregion
            }
            else if (string.Compare(args.Action, "AssignTeamOwnerFromSensitivityLevel", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region Assign Team Owner From Sensitivity Level
                var lookupLogicalName = Utility.GetAndRemoveParameter(parms, "LogicalName");
                var lookupId = Utility.GetAndRemoveParameter(parms, "Id");
                var sensitivtyLevel = Utility.GetAndRemoveParameter(parms, "SensitivityLevel");
                var dataNodeName = Utility.GetAndRemoveParameter(parms, "datanodename");

                UpdateContext(dataNodeName, "LogicalName", "");
                UpdateContext(dataNodeName, "Id", "");
                UpdateContext(dataNodeName, "SensitivityLevel", "");
                var sl = int.Parse(sensitivtyLevel);
                var owningTeam = AssignTeamOwnerFromSensitivityLevel(lookupLogicalName, new Guid(lookupId), sl, dataNodeName);
                UpdateContext(dataNodeName, "LogicalName", owningTeam.LogicalName);
                UpdateContext(dataNodeName, "Id", owningTeam.Id.ToString());
                UpdateContext(dataNodeName, "SensitivityLevel", sensitivtyLevel);
                #endregion
            }
            else if (String.Compare(args.Action, "SetFocus", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region SetFocus

                var controlName = Utility.GetAndRemoveParameter(parms, "controlname");

                var mainWindow = Application.Current.MainWindow;
                var objControl = (Control)FindVisualChildByName(mainWindow, controlName);
                if (objControl != null)
                {
                    objControl.Focus();
                }

                #endregion
            }
            else if (String.Compare(args.Action, "GetOrgSystemValue", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region Get Org System Value

                var orgSystemValue = Utility.GetAndRemoveParameter(parms, "OrgSystemValue");
                var dataNodeName = Utility.GetAndRemoveParameter(parms, "datanodename");

                UpdateContext(dataNodeName, orgSystemValue, GetOrgSystemValue(orgSystemValue));
                #endregion
            }
            else if (String.Compare(args.Action, "GetSpecialFolder", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region Get Special Folder

                var specialFolder = Utility.GetAndRemoveParameter(parms, "SpecialFolder");
                var dataNodeName = Utility.GetAndRemoveParameter(parms, "datanodename");

                UpdateContext(dataNodeName, specialFolder, GetSpecialFolder(specialFolder));
                #endregion
            }
            else if (string.Compare(args.Action, "LogMessage", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region Log Message

                var log = new UdoDiagnosticListener();
                var traceEventType = TraceEventType.Information;

                var appName = Utility.GetAndRemoveParameter(parms, "ApplicationName");
                var message = Utility.GetAndRemoveParameter(parms, "Message");
                var stringTraceEventType = Utility.GetAndRemoveParameter(parms, "TraceEventType");
                var advance = Utility.GetAndRemoveParameter(parms, "Advance");


                //traceEventType = TraceEventType.TryParse(stringTraceEventType,)

                switch (stringTraceEventType)
                {
                    case "Critical":
                        traceEventType = TraceEventType.Critical;
                        log.Error(appName, message, advance);
                        break;
                    case "Error":
                        traceEventType = TraceEventType.Error;
                        log.Error(appName, message, advance);
                        break;
                    case "Warning":
                        traceEventType = TraceEventType.Warning;
                        log.Warn(appName, message);
                        break;
                    case "Information":
                        traceEventType = TraceEventType.Information;
                        log.Information(appName, message);
                        break;
                    case "Verbose":
                        traceEventType = TraceEventType.Verbose;
                        log.Information(appName, message);
                        break;
                    case "Start":
                        traceEventType = TraceEventType.Start;
                        break;
                    case "Stop":
                        traceEventType = TraceEventType.Stop;
                        break;
                    case "Suspend":
                        traceEventType = TraceEventType.Suspend;
                        break;
                    case "Resume":
                        traceEventType = TraceEventType.Resume;
                        break;
                    case "Transfer":
                        traceEventType = TraceEventType.Transfer;
                        break;
                    default:
                        traceEventType = TraceEventType.Information;
                        log.Information(appName, message);
                        break;
                }

                _logWriter.Log(message, traceEventType);

                #endregion
            }
            else if (string.Compare(args.Action, "SetReturnValue", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region Set Return Value

                var text = Utility.GetAndRemoveParameter(parms, "text");
                args.ActionReturnValue = text;

                #endregion
            }
            else if (string.Compare(args.Action, "DetermineFocus", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region Determine Focus

                DetermineFocus(args);

                #endregion
            }
            else if (string.Compare(args.Action, "ParseCrmUrlQuery", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region Parse CRM Url

                var datanodename = Utility.GetAndRemoveParameter(parms, "datanodename");
                var isGlobal = Utility.GetAndRemoveParameter(parms, "isglobal") == "Y" ? true : false;
                var subjecturl = Utility.GetAndRemoveParameter(parms, "subjecturl");

                ParseCrmUrlQuery(datanodename, new Uri(subjecturl).Query, isGlobal);

                #endregion
            }
            else if (string.Compare(args.Action, "RetrieveUiiOptionByName", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region Retrieve UII Option by Name

                var name = Utility.GetAndRemoveParameter(parms, "name");
                var uiiRecord = RetrieveUiiOptionByName(name);

                if (uiiRecord != null)
                    args.ActionReturnValue = uiiRecord.GetAttributeValue<string>("uii_value");
                else
                    throw new Exception("Uii Option Record By Name Not Found");

                #endregion
            }
            else if (string.Compare(args.Action, "GetSessionCount", StringComparison.OrdinalIgnoreCase) == 0)
            {
                GetSessionCount(args);
            }
            else
            {
                base.DoAction(args);
            }
        }

        public string GetOrgSystemValue(string orgValue)
        {
            var value = string.Empty;

            //value = _client.CrmInterface.CrmConnectOrgUriActual.Segments
            switch (orgValue)
            {
                case "OrgName":
                    value = _client.CrmInterface.ConnectedOrgFriendlyName;
                    break;
                case "Port":
                    value = _client.CrmInterface.CrmConnectOrgUriActual.Port.ToString();
                    break;
                case "AbsolutePath":
                    value = _client.CrmInterface.CrmConnectOrgUriActual.AbsolutePath;
                    break;
                case "AbsoluteUri":
                    value = _client.CrmInterface.CrmConnectOrgUriActual.AbsoluteUri;
                    break;
                case "Authority":
                    value = _client.CrmInterface.CrmConnectOrgUriActual.Authority;
                    break;
                case "DnsSafeHost":
                    value = _client.CrmInterface.CrmConnectOrgUriActual.DnsSafeHost;
                    break;
                case "Fragment":
                    value = _client.CrmInterface.CrmConnectOrgUriActual.Fragment;
                    break;
                case "Host":
                    value = _client.CrmInterface.CrmConnectOrgUriActual.Host;
                    break;
                case "LocalPath":
                    value = _client.CrmInterface.CrmConnectOrgUriActual.LocalPath;
                    break;
                case "PathAndQuery":
                    value = _client.CrmInterface.CrmConnectOrgUriActual.PathAndQuery;
                    break;
                case "OriginalString":
                    value = _client.CrmInterface.CrmConnectOrgUriActual.OriginalString;
                    break;
                case "Query":
                    value = _client.CrmInterface.CrmConnectOrgUriActual.Query;
                    break;
                case "Scheme":
                    value = _client.CrmInterface.CrmConnectOrgUriActual.Scheme;
                    break;
                case "Segments":
                    var values = _client.CrmInterface.CrmConnectOrgUriActual.Segments;
                    value = string.Join(", ", values);
                    break;
                default:
                    break;
            }
            
            return value;
        }

        public object FindVisualChildByName(UIElement child, string name)
        {
            if (child is DependencyObject)
            {
                var controlName = child.GetValue(NameProperty) as string;
                if (controlName == name)
                    return child;
            }
            var fe = child as FrameworkElement;
            if (fe != null)
            {
                try
                {
                    fe.ApplyTemplate();
                }
                catch
                {
                    // ignored
                }
                var childrenCount = VisualTreeHelper.GetChildrenCount(fe);
                for (var i = 0; i < childrenCount; i++)
                {
                    var child1 = VisualTreeHelper.GetChild(child, i);
                    var obj = FindVisualChildByName(child1 as UIElement, name);
                    if (obj != null)
                        return obj;
                }
            }
            var c = child as ContentControl;
            if (c != null)
            {
                var obj = FindVisualChildByName(c.Content as UIElement, name);
                if (obj != null)
                    return obj;
            }
            var ic = child as ItemsControl;
            if (ic != null)
            {
                foreach (var elem in ic.Items)
                {
                    if (elem is UIElement)
                    {
                        var obj = FindVisualChildByName(elem as UIElement, name);
                        if (obj != null)
                            return obj;
                    }
                }
            }
            return null;
        }

        public SetStateResponse SetState(SetStateRequest setStateRequest)
        {
            return (SetStateResponse)Execute(setStateRequest);
        }

        public string GetDataParameter(string nodeName, string key)
        {
            var dcr =
                (DynamicsCustomerRecord)
                    ((AgentDesktopSession)localSessionManager.ActiveSession).Customer.DesktopCustomer;

            if (!dcr.CapturedReplacementVariables.ContainsKey(nodeName)) return null;

            var node = dcr.CapturedReplacementVariables[nodeName];

            if (node != null && node.ContainsKey(key)) return node[key].value;

            return null;
        }

        public string GetDataParameter(Session session, string nodeName, string key)
        {
            var dcr =
                (DynamicsCustomerRecord)
                    ((AgentDesktopSession)session).Customer.DesktopCustomer;

            if (!dcr.CapturedReplacementVariables.ContainsKey(nodeName)) return null;

            var node = dcr.CapturedReplacementVariables[nodeName];

            if (node != null && node.ContainsKey(key)) return node[key].value;

            return null;
        }

        public string GetUrlParameterValue(string url, string section)
        {
            var returnValue = "";
            var decodedUrl = HttpUtility.UrlDecode(url);

            if (decodedUrl != null)
            {
                decodedUrl = decodedUrl.Replace("%7b", "{");
                decodedUrl = decodedUrl.Replace("%7d", "}");


                var pArray = decodedUrl.Split('&');
                foreach (var t in pArray)
                {
                    var keyValue = t.Split('=');
                    if (keyValue[0] == section)
                    {
                        returnValue = keyValue[1];
                    }
                }
            }

            returnValue = returnValue.Replace("{", "");
            returnValue = returnValue.Replace("}", "");

            return returnValue;
        }

        public void UpdateContext(string dataNodeName, string key, string value)
        {
            UpdateSessionContext(localSessionManager.ActiveSession, dataNodeName, key, value);
        }

        public void UpdateSessionContext(Session session, string dataNodeName, string key, string value)
        {
            var lri = new LookupRequestItem
            {
                Key = key,
                Value = value
            };

            var lriList = new List<LookupRequestItem> { lri };

            var dcr =
                (DynamicsCustomerRecord)
                    ((AgentDesktopSession)session).Customer.DesktopCustomer;

            dcr.MergeReplacementParameter(dataNodeName, lriList, true);

        }

        public void UpdateSessionContext(Session session, string dataNodeName, Dictionary<string, string> parms)
        {

            var lriList = new List<LookupRequestItem>();
            var dcr = (DynamicsCustomerRecord)((AgentDesktopSession)session).Customer.DesktopCustomer;

            foreach (var parm in parms)
            {
                var lri = new LookupRequestItem
                {
                    Key = parm.Key,
                    Value = parm.Value
                };

                lriList.Add(lri);
            }

            dcr.MergeReplacementParameter(dataNodeName, lriList, true);

        }

        public void UpdateContext(string dataNodeName, string key, string type, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }


            var data = new Dictionary<string, CRMApplicationData>();
            data.Add(key, new CRMApplicationData { value = value, type = type, name = key });

            var dcr =
                (DynamicsCustomerRecord)
                    ((AgentDesktopSession)localSessionManager.ActiveSession).Customer.DesktopCustomer;
            dcr.MergeReplacementParameter(dataNodeName, data, true);


        }

        public void UpdateContextFormat(string dataNodeName, string key, string format, object[] args)
        {
            var lri = new LookupRequestItem
            {
                Key = key,
                Value = string.Format(format, args)
            };

            var lriList = new List<LookupRequestItem> { lri };

            var dcr =
                (DynamicsCustomerRecord)
                    ((AgentDesktopSession)localSessionManager.ActiveSession).Customer.DesktopCustomer;
            dcr.MergeReplacementParameter(dataNodeName, lriList, true);

        }

        public void ClearSessionContext(Session session, string dataNodeName)
        {
            var dcr =
                (DynamicsCustomerRecord)
                    ((AgentDesktopSession)session).Customer.DesktopCustomer;
            dcr.ClearReplaceableParameter(dataNodeName);

        }

        public EntityReference AssignTeamOwnerFromVeteranField(string lookupLogicalName, Guid lookupId, string lookupField, string dataNodeName)
        {
            EntityReference owningTeam = null;

            var cols = new ColumnSet(new string[] { lookupField });
            var lookupEntity = Retrieve(lookupLogicalName, lookupId, cols);

            if (lookupEntity != null)
            {
                var contactId = lookupEntity.GetAttributeValue<EntityReference>(lookupField);

                if (contactId != null)
                {
                    var vetcols = new ColumnSet(new string[] { "ownerid", "udo_veteransensitivitylevel" });
                    var vet = Retrieve(contactId.LogicalName, contactId.Id, vetcols);

                    var vetTeam = vet.GetAttributeValue<EntityReference>("ownerid");

                    if (vetTeam.LogicalName != "team")
                    {
                        var realLevel = vet.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel").Value;
                        owningTeam = AssignTeamOwnerFromSensitivityLevel(lookupLogicalName, lookupId, realLevel,
                            dataNodeName);
                    }
                }
            }

            return owningTeam;
        }

        public EntityReference AssignTeamOwnerFromSensitivityLevel(string lookupLogicalName, Guid lookupId, int sensitivtyLevel, string dataNodeName)
        {
            EntityReference owningTeam = null;

            var busUnit = GetBusinessUnitBySensitivityLevel(sensitivtyLevel);
            var defaultTeam = GetDefaultTeamForBusinessUnit(busUnit.Id);

            if (defaultTeam != null)
            {
                var vetTeam = new EntityReference("team", defaultTeam.Id);
                var req = new AssignRequest()
                {
                    Assignee = vetTeam,
                    Target = new EntityReference(lookupLogicalName, lookupId)
                };

                var res = (AssignResponse)Execute(req); // Execute the AssignRequest

                owningTeam = vetTeam;
            }

            return owningTeam;
        }

        public Entity GetBusinessUnitBySensitivityLevel(int level)
        {

            var expression = new QueryExpression()
            {
                EntityName = "businessunit",
                Criteria =
                {
                    Filters = 
                    {
                        new FilterExpression()
                        {
                            Conditions = 
                            { 
                                new ConditionExpression("udo_veteransensitivitylevel", ConditionOperator.Equal, level)
                            }
                        }
                    }
                }
            };

            var results = _client.CrmInterface.ActiveAuthenticationType ==
                      Microsoft.Xrm.Tooling.Connector.AuthenticationType.OAuth ?
                      _client.CrmInterface.OrganizationWebProxyClient.RetrieveMultiple(expression) :
                      _client.CrmInterface.OrganizationServiceProxy.RetrieveMultiple(expression);

            return results.Entities.Count > 0 ? results.Entities[0] : null;
        }

        public Entity GetDefaultTeamForBusinessUnit(Guid buId)
        {
            QueryExpression expression = new QueryExpression()
            {
                EntityName = "team",
                Criteria =
                {
                    Filters = 
                    {
                        
                        new FilterExpression()
                        {
                          Conditions = 
                            { 
                                
                                new ConditionExpression("name", ConditionOperator.Equal, "PCR"),
                                new ConditionExpression("businessunitid", ConditionOperator.Equal, buId)
                            }
                        }
                    }
                }
            };

            var results = RetrieveMultiple(expression);
            return results.Entities.Count > 0 ? results.Entities[0] : null;
        }

        #region CRUD Methods

        //public object GetValueFromEntity(string id, string logicalname, string fieldValue)
        //{
        //    string fetchXml = string.Format("<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\">\r\n                              <entity name=\"{0}\">\r\n                               <attribute name=\"{2}\" />\r\n                               <filter type=\"and\">\r\n                                 <condition attribute=\"{0}id\" operator=\"eq\" uitype=\"{0}\" value=\"{1}\" />\r\n                                </filter>\r\n                              </entity>\r\n                              </fetch>", logicalname, id, fieldValue);
        //    string pageCookie = "";
        //    bool isMoreRecords = false;
        //    CrmServiceClient crmInterface = this._client.get_CrmInterface();
        //    Guid guid = new Guid();
        //    EntityCollection result = crmInterface.GetEntityDataByFetchSearchEC(fetchXml, 1, 1, pageCookie, ref pageCookie, ref isMoreRecords, guid);
        //    if ((result == null && this._client.CrmInterface.LastCrmException != null))
        //    {
        //        this.diagLogger.Log(this._client.get_CrmInterface().get_LastCrmException());
        //        throw this._client.get_CrmInterface().get_LastCrmException();
        //    }
        //    var c = result.Entities.FirstOrDefault<Entity>();

        //    return c.Item(fieldValue);
        //}



        public Entity Retrieve(string entityName, Guid id)
        {
            return Retrieve(entityName, id, new ColumnSet(true));
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            var entity = _client.CrmInterface.ActiveAuthenticationType == AuthenticationType.OAuth ?
                _client.CrmInterface.OrganizationWebProxyClient.Retrieve(entityName, id, columnSet) :
                _client.CrmInterface.OrganizationServiceProxy.Retrieve(entityName, id, columnSet);
            return entity;
        }

        public EntityCollection RetrieveMultiple(QueryExpression queryExpression)
        {
            var results = _client.CrmInterface.ActiveAuthenticationType ==
                Microsoft.Xrm.Tooling.Connector.AuthenticationType.OAuth ?
                _client.CrmInterface.OrganizationWebProxyClient.RetrieveMultiple(queryExpression) :
                _client.CrmInterface.OrganizationServiceProxy.RetrieveMultiple(queryExpression);

            return results;
        }

        public EntityCollection RetrieveMultiple(FetchExpression fetchExpression)
        {
            var results = _client.CrmInterface.ActiveAuthenticationType ==
                Microsoft.Xrm.Tooling.Connector.AuthenticationType.OAuth ?
                _client.CrmInterface.OrganizationWebProxyClient.RetrieveMultiple(fetchExpression) :
                _client.CrmInterface.OrganizationServiceProxy.RetrieveMultiple(fetchExpression);

            return results;
        }

        public RetrieveMultipleResponse RetrieveMultiple(RetrieveMultipleRequest retrieveMultipleRequest)
        {
            return (RetrieveMultipleResponse)Execute(retrieveMultipleRequest);
        }

        public RetrieveAllEntitiesResponse RetrieveAllEntities(RetrieveAllEntitiesRequest retrieveAllEntitiesRequest)
        {
            return (RetrieveAllEntitiesResponse)Execute(retrieveAllEntitiesRequest);
        }

        public void Update(Entity updateEntity)
        {
            if (_client.CrmInterface.ActiveAuthenticationType ==
                Microsoft.Xrm.Tooling.Connector.AuthenticationType.OAuth)
                _client.CrmInterface.OrganizationWebProxyClient.Update(updateEntity);
            else
                _client.CrmInterface.OrganizationServiceProxy.Update(updateEntity);

        }

        public Guid Create(Entity createEntity)
        {
            Guid newGuid;
            if (_client.CrmInterface.ActiveAuthenticationType ==
Microsoft.Xrm.Tooling.Connector.AuthenticationType.OAuth)
                newGuid = _client.CrmInterface.OrganizationWebProxyClient.Create(createEntity);
            else
                newGuid = _client.CrmInterface.OrganizationServiceProxy.Create(createEntity);

            return newGuid;
        }

        public Entity RetrieveUiiOptionByName(string name)
        {
            #region Retrieve UII Option By Name

            var qe = new QueryExpression
            {
                TopCount = 1,
                EntityName = "uii_option",
                ColumnSet = UiiOptionColumnSet
            };

            var fe1 = new FilterExpression(LogicalOperator.And);
            fe1.AddCondition("uii_name", ConditionOperator.Equal, name);
            qe.Criteria.AddFilter(fe1);

            var uiiOption = RetrieveMultiple(qe);

            return uiiOption.Entities.Count == 0 ? null : uiiOption.Entities[0];

            #endregion
        }

        #endregion

        #region QueueItem Methods

        public PickFromQueueResponse PickFromQueue(PickFromQueueRequest pickFromQueue)
        {
            return (PickFromQueueResponse)Execute(pickFromQueue);
        }

        public ReleaseToQueueResponse ReleaseFromQueue(ReleaseToQueueRequest releaseToQueueRequest)
        {
            return (ReleaseToQueueResponse)Execute(releaseToQueueRequest);
        }

        public RemoveFromQueueResponse RemoveFromQueue(RemoveFromQueueRequest removeFromQueueRequest)
        {
            return (RemoveFromQueueResponse)Execute(removeFromQueueRequest);
        }

        #endregion

        #region Private Base Methods

        public OrganizationResponse Execute(OrganizationRequest organizationRequest)
        {
            OrganizationResponse organizationResponse = null;
            organizationResponse = _client.CrmInterface.ActiveAuthenticationType == AuthenticationType.OAuth ? _client.CrmInterface.OrganizationWebProxyClient.Execute(organizationRequest) : _client.CrmInterface.OrganizationServiceProxy.Execute(organizationRequest);
            return organizationResponse;
        }

        #endregion

        public string GetSpecialFolder(string specialFolder)
        {
            string folderPath = string.Empty;

            switch (specialFolder)
            {
                case "AppDomainBaseDirectory":
                    folderPath = AppDomain.CurrentDomain.BaseDirectory;
                    break;
                case "CurrentDirectory":
                    folderPath = Environment.CurrentDirectory;
                    break;
                case "Downloads":
                    folderPath = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                    folderPath = Path.Combine(folderPath, "Downloads");
                    break;
                case "LocalApplicationData":
                    folderPath = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                    break;
                case "Temp":
                    folderPath = Path.GetTempPath();
                    break;
                case "GetRandomFileName":
                    folderPath = Path.GetRandomFileName();
                    break;
                case "GetTempFileName":
                    folderPath = Path.GetTempFileName();
                    break;
                case "GetUSDCustomizationPath":

                    var absoluteUri = _client.CrmInterface.CrmConnectOrgUriActual.AbsoluteUri;
                    var orgUniqueName = _client.CrmInterface.ConnectedOrgUniqueName;
                    var userID = _client.CrmInterface.GetMyCrmUserId();

                    // Commented out due to issue with USD 4.1. 
                    // Need all users to upgrade to USD 4.1 before we can upgrade this line to use new assembly with new parameters
                    //var po = new Microsoft.Crm.UnifiedServiceDesk.CommonUtility.UserProfileManager.ProfileOperations(absoluteUri, orgUniqueName, userID, "", false);
                    ////po.UsdCustomizationFilesPath
                    //folderPath = po.UsdCustomizationFilesPath;

                    var po = new Microsoft.Crm.UnifiedServiceDesk.CommonUtility.UserProfileManager.ProfileOperations(absoluteUri, orgUniqueName, userID, "", false, string.Empty, false);
                    //po.UsdCustomizationFilesPath
                    folderPath = po.UsdCustomizationFilesPath;

                    break;
                default:

                    try
                    {
                        if (!Directory.Exists(specialFolder))
                        {
                            Directory.CreateDirectory(specialFolder);
                        }
                    }
                    catch (Exception e)
                    {
                        //Nothing
                    }

                    folderPath = specialFolder;

                    break;
            }

            return folderPath;
        }

        public DispatcherPriority GetDispatchPrioroity(string dispatch)
        {
            DispatcherPriority rtnDispatch;

            switch (dispatch)
            {
                case "ApplicationIdle":
                    rtnDispatch = DispatcherPriority.ApplicationIdle;
                    break;
                case "Background":
                    rtnDispatch = DispatcherPriority.Background;
                    break;
                case "ContextIdle":
                    rtnDispatch = DispatcherPriority.ContextIdle;
                    break;
                case "DataBind":
                    rtnDispatch = DispatcherPriority.DataBind;
                    break;
                case "Inactive":
                    rtnDispatch = DispatcherPriority.Inactive;
                    break;
                case "Input":
                    rtnDispatch = DispatcherPriority.Input;
                    break;
                case "Invalid":
                    rtnDispatch = DispatcherPriority.Invalid;
                    break;
                case "Loaded":
                    rtnDispatch = DispatcherPriority.Loaded;
                    break;
                case "Normal":
                    rtnDispatch = DispatcherPriority.Normal;
                    break;
                case "Render":
                    rtnDispatch = DispatcherPriority.Render;
                    break;
                case "Send":
                    rtnDispatch = DispatcherPriority.Send;
                    break;
                case "SystemIdle":
                    rtnDispatch = DispatcherPriority.SystemIdle;
                    break;
                default:
                    rtnDispatch = DispatcherPriority.Normal;
                    break;
            }

            return rtnDispatch;
        }

        protected override void SafeDispatcherUnhandledExceptionHandler(object sender, SafeDispatcherUnhandledExceptionEventArgs ex)
        {
            base.SafeDispatcherUnhandledExceptionHandler(sender, ex);
        }

        private void CheckChatActiveSessions(RequestActionEventArgs args)
        {

            var _datanodename = "CTI";
            var _processingstep = "next step";

            try
            {
                //var rcdFound = false;
                //var sessionId = "";
                //var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
                //var interactionId = Utility.GetAndRemoveParameter(parms, "InteractionId");
                //var chatSessionId = Utility.GetAndRemoveParameter(parms, "ChatSessionId");

                //[[Interaction.Id]]

                //foreach (var sess in localSessionManager)
                //{
                //    _processingstep = "CheckChatActiveSessions get localsession";
                //    var currSess = (AgentDesktopSession)sess;

                //    _processingstep = "CheckChatActiveSessions ChatSessionId - " + chatSessionId;
                //    if (!string.IsNullOrEmpty(chatSessionId))
                //    {
                //        _processingstep = "CheckChatActiveSessions CtiCallRefIdChat - " + currSess.CtiCallRefIdChat;
                //        if (currSess.CtiCallRefIdChat == new Guid(chatSessionId))
                //        {
                //            rcdFound = true;
                //            sessionId = currSess.SessionId.ToString();
                //        }
                //    }
                //}

                UpdateContext(_datanodename, "Processing Step", _processingstep);
            }
            catch (Exception ex)
            {
                UpdateContext(_datanodename, "Processing Step", _processingstep);
                var exp = ExceptionManager.ReportException(ex);

                UpdateContext(_datanodename, "ErrorOccurred", "Y");
                UpdateContext(_datanodename, "ErrorOccurredIn", args.Action);
                UpdateContext(_datanodename, "ExceptionMessage", ex.Message);
                UpdateContext(_datanodename, "ExceptionReport", exp);
            }

        }

        private void ParseCrmUrlQuery(string datanodename, string crmurl, bool isGlobal)
        {
            var strs = new Dictionary<string, string>();
            var nvc = HttpUtility.ParseQueryString(crmurl);

            foreach (string key in nvc)
            {
                string mykey = key;

                if (mykey.Contains("?"))
                {
                    var split = key.Split('?');
                    mykey = split[1];
                }

                if (mykey == "extraqs" || mykey == "uri")
                    ParseCrmUrlQuery(datanodename, nvc[key], isGlobal);
                else
                {
                    var value = nvc[key];

                    if (key == "id" || key == "rskey" || key == "_CreateFromId")
                    {
                        value = value.Replace("{", "");
                        value = value.Replace("}", "");
                    }
                    strs.Add(mykey, value);
                }
            }

            if (strs.Count > 0)
            {
                if (isGlobal)
                    UpdateSessionContext(localSessionManager.GlobalSession, datanodename, strs);
                else
                    UpdateSessionContext(localSessionManager.ActiveSession, datanodename, strs);
            }
        }

        private void DetermineFocus(RequestActionEventArgs args)
        {

            var fe = FocusManager.GetFocusedElement(System.Windows.Application.Current.MainWindow);

            if (fe == null)
            {
                args.ActionReturnValue = "Focus not found";
            }
            else if (!(fe is UIElement))
            {
                args.ActionReturnValue = string.Concat("Type = ", fe.GetType());
            }
            else
            {
                var element = (FrameworkElement)fe;
                args.ActionReturnValue = element.Name.ToString();
            }
        }

        private void GetSessionCount(RequestActionEventArgs args)
        {
            var sessCount = 0;
            var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
            var dataNodeName = Utility.GetAndRemoveParameter(parms, "datanodename");
            var key = Utility.GetAndRemoveParameter(parms, "key");
            var global = Utility.GetAndRemoveParameter(parms, "global");

            // Get the Session Count
            if (localSessionManager.Count > 0)
            {
                foreach (var sess in localSessionManager)
                {
                    var currSess = (AgentDesktopSession)sess;
                    if (!currSess.Global)
                    {
                        sessCount++;
                    }
                }
            }

            if (!string.IsNullOrEmpty(dataNodeName) && !string.IsNullOrEmpty(key))
            {
                UpdateSessionContext(
                    global == "Y" ? localSessionManager.GlobalSession : localSessionManager.ActiveSession, dataNodeName,
                    key, sessCount.ToString());
            }

            args.ActionReturnValue = sessCount.ToString();
        }
        
        public string GetIEVersion()
        {
            string key = @"Software\Microsoft\Internet Explorer";
            RegistryKey dkey = Registry.LocalMachine.OpenSubKey(key, false);
            string data = dkey.GetValue("Version").ToString();
            return data;
        }

    }
}
