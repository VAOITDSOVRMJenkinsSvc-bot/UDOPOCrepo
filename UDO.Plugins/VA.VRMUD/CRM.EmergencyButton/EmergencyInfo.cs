using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Browser;

namespace CRM.EmergencyButton {

    public class EmergencyInfo {
        public string Message { get; set; }
        public ScriptObject Recipients { get; set; }

        public List<string> Emails() {
            var emails = new List<string>();

            if (Recipients == null) return emails;

            var index = Convert.ToInt32(Recipients.GetProperty("length"));
            for (int i = 0; i < index; i++) {
                var recipient = (string) Recipients.GetProperty(i);
                if (recipient != null) {
                    emails.Add(recipient);
                }
            }

            return emails;
        } 
    }
}
