using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Lync.Utilities.Logging;

namespace CRM.EmergencyButton {
    /// <summary>
    /// DebugLogListener provides a listener that writes to the debug output
    /// </summary>    
    public class DebugLogListener : LogListener {
        /// <summary>
        /// Write a LogEntry instance
        /// </summary>
        /// <param name="logEntry"></param>
        public override void Write(LogEntry logEntry) {
            if (!Filter(logEntry)) {
                Debug.WriteLine(logEntry.ToString());
            }
        }
    }
}