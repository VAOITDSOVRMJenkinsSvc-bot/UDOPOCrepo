using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;

namespace Va.Udo.Usd.CustomControls
{
    /// <summary>
    ///     This hosted control is used to host VIP hosting entities which have a high probability of containing
    ///     unsaved fields.
    /// </summary>
    public partial class BrowserHost : DynamicsBaseHostedControl
    {
        /// <summary>
        ///     Url to navigate to about blank
        /// </summary>
        private const string aboutBlankUri = "about:blank";

        private bool sessionCloseInitiated;

        /// <summary>
        ///     Browser Host Constructor
        /// </summary>
        /// <param name="id">Hosted Application Id</param>
        /// <param name="appName">Hosted Application Name</param>
        /// <param name="initXml">Hosted Application Initialization Xml</param>
        public BrowserHost(Guid id, string appName, string initXml)
            : base(id, appName, initXml)
        {
            InitializeComponent();
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
                case "InitiateSessionClose":
                    InitiateSessionClose2();
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


        private void InitiateSessionClose2()
        {
            try
            {
                sessionCloseInitiated = true;
                webBrowser.Navigated += webBrowser_Navigated;
                webBrowser.Navigate(aboutBlankUri);
            }
            catch
            {
                webBrowser.Navigated -= webBrowser_Navigated;
            }
        }

        private void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            if (string.Compare(e.Uri.AbsoluteUri, "about:blank", true
                ) == 0)
            {
                if (sessionCloseInitiated)
                {
                    webBrowser.Navigated -= webBrowser_Navigated;
                    FireEvent("ReadyToClose", new Dictionary<string, string>());
                }
            }
        }

        /// <summary>
        ///     Disposes the browser control before calling the dispose in the base class
        /// </summary>
        public override void Close()
        {
            try
            {
                webBrowser.Dispose();
            }
            catch
            {
                //TODO: Implement logging
            }
            base.Close();
        }
    }
}