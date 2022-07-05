using Microsoft.Uii.Common.Logging;
using Microsoft.Uii.Common.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;

namespace Va.Udo.Usd.CustomControls.Listerners
{
    public class UdoDiagnosticListener : ILogging
    {
        private string filename;

        public UdoDiagnosticListener()
        {
            var dateStamp = DateTime.Now.ToString("yyyy-MM-dd");
            //var filebasepath = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            //filename = Path.Combine(filebasepath,
                //"Microsoft\\Microsoft Dynamics® 365 Unified Service Desk\\2.2.1.806\\UdoDiagnostics-" + dateStamp + ".log");

            filename = "C:\\temp\\UdoDiagnostics-" + dateStamp + ".log";
        }

        public void Error(string applicationName, string message, string advanced)
        {
            File.AppendAllText(filename, @"\nError is logged\n" + "\nApplication Name:\n" + applicationName + "\nMessage:\n" + message + "\nAdvanced:\n" + advanced);
        }

        public void Information(string applicationName, string message)
        {
            File.AppendAllText(filename, "\nInformation is logged\n" + "\nApplication Name:\n" + applicationName + "\nMessage:\n" + message);
        }

        public void Initialize(string name, System.Collections.Specialized.NameValueCollection configValue)
        {
            //Not needed
        }

        public bool ShowErrors { get; set; }

        public string Tag { get; set; }

        public void Trace(string applicationName, string message)
        {
            File.AppendAllText(filename, "\nVerbose is logged\n" + "\nApplication Name:\n" + applicationName + "\nMessage:\n" + message);
        }

        public void Warn(string applicationName, string message)
        {
            File.AppendAllText(filename, "\nWarning is logged\n" + "\nApplication Name:\n" + applicationName + "\nMessage:\n" + message);
        }



    }
}
