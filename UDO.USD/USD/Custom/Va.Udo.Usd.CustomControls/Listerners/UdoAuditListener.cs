using Microsoft.Uii.AifServices;
using Microsoft.Uii.Common.Entities;
using Microsoft.Uii.Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Va.Udo.Usd.CustomControls.Listerners
{
    public class UdoAuditListener : IAuditService
    {
        public void SaveAudit(IEnumerable<Microsoft.Uii.Common.Entities.LogData> logCache)
        {
            LogToFile(logCache);
        }

        private void LogToFile(IEnumerable<LogData> logCache)
        {
            var dateStamp = DateTime.Now.ToString("yyyy-MM-dd");
            //var filebasepath = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            //var filename = Path.Combine(filebasepath,
                //"Microsoft\\Microsoft Dynamics® 365 Unified Service Desk\\2.2.1.806\\UdoAudit-" + dateStamp + ".log");

            var filename = "C:\\temp\\UdoAudit-" + dateStamp + ".log";

            foreach (var item in logCache)
            {
                try
                {
                    File.AppendAllText(filename, item.GetLogData());
                }
                catch (Exception ex)
                {
                    Logging.Error("USD", ex.StackTrace);
                }
            }
        }
    }
}
