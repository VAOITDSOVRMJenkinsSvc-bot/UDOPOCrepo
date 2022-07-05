using System;
using System.Linq;
using System.Collections;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Extensibility;
using System.Collections.Generic;
using System.Diagnostics;


namespace CRM.EmergencyButton {
    public class Lync {

        [ScriptableMember()]
        public void Execute() {
            var emercencyInfo = GetEmergencyInfo();
            Automation automation = null;

            try {
                automation = LyncClient.GetAutomation();
            } catch (AutomationServerException aEx) {
                ManageExceptions(string.Format("{0} - {1}", aEx.Reason, aEx.Message));
                return;
            } 

            // Conversation setting Dictionary.
            var modalitySettings = new Dictionary<AutomationModalitySettings, object>();
            modalitySettings.Add(AutomationModalitySettings.FirstInstantMessage, emercencyInfo.Message);
            modalitySettings.Add(AutomationModalitySettings.SendFirstInstantMessageImmediately, true);

            // Start the conversation.
            try {
                var asyncResult = automation.BeginStartConversation(
                    AutomationModalities.InstantMessage
                    , emercencyInfo.Emails()
                    , modalitySettings
                    , result => {
                        HtmlPage.Window.Invoke("onLyncExecuteReady");
                    }
                    , null);
            
            } catch (LyncClientException lyncClientEx) {
                ManageExceptions(lyncClientEx.Message);
            } 
        }

        // Gets the _emergency object from the HTML page javascript
        public EmergencyInfo GetEmergencyInfo() {
            return new EmergencyInfo() {
                Message = (string)HtmlPage.Window.GetProperty("message"),
                Recipients = (ScriptObject)HtmlPage.Window.GetProperty("recipients")
            };
        } 

        private void ManageExceptions(string message) {
            HtmlPage.Window.Invoke("onLyncErrors", message);
        }

    }
}
