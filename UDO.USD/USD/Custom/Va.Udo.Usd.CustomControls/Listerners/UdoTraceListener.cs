using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SHDocVw;

namespace Va.Udo.Usd.CustomControls.Listerners
{
    public class UdoTraceListener : TraceListener
    {
        private string filename;

        public UdoTraceListener()
        {
           
            var dateStamp = DateTime.Now.ToString("yyyy-MM-dd");
            //var filebasepath = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            //filename = Path.Combine(filebasepath,
                //"Microsoft\\Microsoft Dynamics® 365 Unified Service Desk\\2.2.1.806\\UdoTraces-" + dateStamp + ".log");

            filename = "C:\\temp\\UdoTraces-" + dateStamp + ".log";
        }

        public override void Write(string message)
        {
            File.AppendAllText(filename, message);
        }

        public override void WriteLine(string message)
        {
            File.AppendAllText(filename, message);
        }
    }

}
