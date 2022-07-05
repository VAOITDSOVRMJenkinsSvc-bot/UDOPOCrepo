using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using MessageBox = System.Windows.MessageBox;

namespace Va.Udo.Usd.CustomControls
{
    /// <summary>
    ///     This hosted control is used to host VIP hosting entities which have a high probability of containing
    ///     unsaved fields.
    /// </summary>
    public abstract partial class FormsBrowserHost : DynamicsBaseHostedControl
    {
        /// <summary>
        ///     Url to navigate to about blank
        /// </summary>
        private const string aboutBlankUri = "about:blank";

        /// <summary>
        ///     Forms web browser
        /// </summary>
        private readonly WebBrowser webBrowser;

        /// <summary>
        ///     Browser Host Constructor
        /// </summary>
        /// <param name="id">Hosted Application Id</param>
        /// <param name="appName">Hosted Application Name</param>
        /// <param name="initXml">Hosted Application Initialization Xml</param>
        public FormsBrowserHost(Guid id, string appName, string initXml)
            : base(id, appName, initXml)
        {
            InitializeComponent();
            webBrowser = formsHost.Child as WebBrowser;
        }

        /// <summary>
        ///     Calls the appropriate implementation of the action passed into the hosted application.
        /// </summary>
        /// <param name="args">Contains the action arguments</param>
        protected override void DoAction(RequestActionEventArgs args)
        {
            switch (args.Action)
            {
                case "default":
                    NavigateToUrl();
                    break;
                case "Navigate":
                    NavigateAction(args);
                    break;
                case "Close":
                    CloseControl();
                    break;
                default:
                    NotImplementedMessage();
                    break;
            }
            base.DoAction(args);
        }

        /// <summary>
        ///     Navigates to the Url passed as the parameter.
        /// </summary>
        /// <param name="uri">Base Url. The default value is about:blank</param>
        private void NavigateToUrl(string uriString = aboutBlankUri)
        {
            try
            {
                uriString = Utility.EnsureQualifiedUrl(uriString);
                var uri = new Uri(uriString);
                if (webBrowser != null)
                {
                    webBrowser.Navigate(uri);
                }
            }
            catch (UriFormatException)
            {
                MessageBox.Show("Incorrect url format: " + uriString);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Url cannot be null");
            }
        }

        /// <summary>
        ///     Implements the navigate action. This method takes the data parameters and retrieves the url from the action data.
        /// </summary>
        /// <param name="args">Action data</param>
        private void NavigateAction(RequestActionEventArgs args)
        {
            try
            {
                var parameters = Utility.SplitLines(args.Data, CurrentContext, localSessionManager);
                var url = Utility.GetAndRemoveParameter(parameters, "url");
                NavigateToUrl(url);
            }
            catch
            {
                MessageBox.Show("Failed to interpret data passed for navigate action.");
            }
        }

        /// <summary>
        ///     Close the control.
        /// </summary>
        private void CloseControl()
        {
            Close();
        }

        /// <summary>
        ///     Error message which is diplayed for any action.
        /// </summary>
        private void NotImplementedMessage()
        {
            MessageBox.Show("This action is not implemented in this control.");
        }

        /// <summary>
        ///     Disposes the browser control before calling the dispose in the base class
        /// </summary>
        public override void Close()
        {
            try
            {
                if (!webBrowser.IsDisposed)
                {
                    Debug.WriteLine("UDO LOG: Start web browser dispose.");
                    webBrowser.Dispose();
                    Debug.WriteLine("UDO LOG: End web browser dispose.");
                }
            }
            catch
            {
                Debug.WriteLine("UDO LOG: Exception during browser dispose.");
            }
            base.Close();
        }
    }
}