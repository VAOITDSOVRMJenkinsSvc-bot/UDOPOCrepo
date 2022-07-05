using System;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using AuthenticationType = Microsoft.Xrm.Tooling.Connector.AuthenticationType;

namespace Va.Udo.Usd.CustomControls
{
    /// <summary>
    ///     Interaction logic for GlobalKeyboardShortcut.xaml
    /// </summary>
    public partial class GlobalKeyboardShortcut : DynamicsBaseHostedControl
    {
        /// <summary>
        ///     Log writer
        /// </summary>
        private readonly TraceLogger logWriter;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicationName"></param>
        /// <param name="initXml"></param>
        public GlobalKeyboardShortcut(Guid id, string applicationName, string initXml) :
            base(id, applicationName, initXml)
        {
            InitializeComponent();
            logWriter = new TraceLogger();
        }

        protected override void DesktopReady()
        {
            base.DesktopReady();
            //RegisterGlobalShortcutKeys();
        }

        protected override void DoAction(RequestActionEventArgs args)
        {
            if (String.Compare(args.Action, "Refresh", StringComparison.OrdinalIgnoreCase) == 0)
            {
                RegisterGlobalShortcutKeys();
            }
            else if (String.Compare(args.Action, "SetFocus", StringComparison.OrdinalIgnoreCase) == 0)
            {
                var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
                var controlName = Utility.GetAndRemoveParameter(parms, "controlname");

                var mainWindow = Application.Current.MainWindow;
                var objControl = (Control) FindVisualChildByName(mainWindow, controlName);
                if (objControl != null)
                {
                    objControl.Focus();
                }
            }
            base.DoAction(args);
        }

        private void RegisterGlobalShortcutKeys()
        {
            try
            {
                // Get active global keyboard shortcut keys configurations.
                var reqKeys = new RetrieveMultipleRequest();
                var queryKeys = new QueryExpression();
                queryKeys.EntityName = "udo_globalkeyboardshortcut";
                queryKeys.ColumnSet = new ColumnSet("udo_shortcutkey", "udo_actioncall");
                queryKeys.Criteria = new FilterExpression(LogicalOperator.And);
                queryKeys.Criteria.AddCondition(new ConditionExpression("statuscode", ConditionOperator.Equal, 1));
                queryKeys.Criteria.AddCondition(new ConditionExpression("udo_actioncall", ConditionOperator.NotNull));
                queryKeys.Criteria.AddCondition(new ConditionExpression("udo_shortcutkey", ConditionOperator.NotNull));
                reqKeys.Query = queryKeys;

                RetrieveMultipleResponse resKeys = null;
                if (_client.CrmInterface.ActiveAuthenticationType == AuthenticationType.OAuth)
                    resKeys =
                        (RetrieveMultipleResponse) _client.CrmInterface.OrganizationWebProxyClient.Execute(reqKeys);
                else
                    resKeys = (RetrieveMultipleResponse) _client.CrmInterface.OrganizationServiceProxy.Execute(reqKeys);

                // Register global keyboard shortcut keys action calls as Key Binding Gestures.
                var window = Application.Current.MainWindow;
                foreach (var globalShortcutKey in resKeys.EntityCollection.Entities)
                {
                    try
                    {
                        // Obtain configured action call.
                        var usdAction = ((EntityReference) globalShortcutKey.Attributes["udo_actioncall"]).Name;

                        // Obtain configured extention keys.
                        var shortcutKey = (string) globalShortcutKey.Attributes["udo_shortcutkey"];

                        // Create keybinding and assigning action call as command parameter.
                        var keyBinding = new KeyBinding
                        {
                            Command = ActionCommands.DoActionCommand,
                            CommandParameter = "http://actioncall/" + HttpUtility.UrlEncode(usdAction)
                        };
                        var gestureSerializer = new KeyGestureValueSerializer();
                        keyBinding.Gesture = (KeyGesture) gestureSerializer.ConvertFromString(shortcutKey, null);
                        window.InputBindings.Add(keyBinding);

                        // Wite to log - the registered global keyboard shortcut keys 
                        logWriter.Log("Global Shortcut Keys : " + usdAction + " using combo " + shortcutKey,
                            TraceEventType.Information);
                    }
                    catch (Exception ex)
                    {
                        logWriter.Log(
                            string.Format(CultureInfo.CurrentCulture,
                                "{0} - Exception occured while binding Global Shortcut Key : {0}", ApplicationName,
                                ex.Message), TraceEventType.Information);
                        //MessageBox.Show(ex.Message, "Exception occured while binding Global Shortcut Key");
                    }
                }
            }
            catch (Exception ex)
            {
                logWriter.Log(
                    string.Format(CultureInfo.CurrentCulture,
                        "{0} - Exception occured while registering Global Shortcut Key : {0}", ApplicationName,
                        ex.Message), TraceEventType.Information);
                //MessageBox.Show(ex.Message, "Exception occured while retrieving and registering Global Shortcut Key");
            }
        }

        private object FindVisualChildByName(UIElement child, string name)
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
    }
}