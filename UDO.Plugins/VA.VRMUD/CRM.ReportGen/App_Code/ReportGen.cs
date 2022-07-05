using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Services;
using System.ServiceModel.Description;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using VRM.EmailReport;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]

public class ReportGen : System.Web.Services.WebService
{
    private string allowedUsers = String.Empty;
    private bool debug = false;
    private bool validateUser = false;
    private string traceFile = @"C:\temp\trace.txt";
    Dictionary<String, ErrorSuppressionInfo> errorSuppressionNodes = new Dictionary<string, ErrorSuppressionInfo>();

    public ReportGen()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
        string doDebug = ConfigurationManager.AppSettings["Debug"];
        debug = (doDebug == "1");

        string trFile = ConfigurationManager.AppSettings["TraceFile"];
        if (!String.IsNullOrEmpty(trFile)) traceFile = trFile;
        /*
        string doValidate = ConfigurationManager.AppSettings["ValidateUser"];
        validateUser = (doValidate == "1");

        allowedUsers = ConfigurationManager.AppSettings["AllowedUsers"];

        string suppressionNodes = ConfigurationManager.AppSettings["ErrorSuppressionNodes"];
        if (!String.IsNullOrEmpty(suppressionNodes))
        {
            string[] errSuppressionNodes = suppressionNodes.Split(new char[] { ';' });
            if (errSuppressionNodes != null && errSuppressionNodes.Length > 0)
            {
                foreach (String node in errSuppressionNodes)
                {
                    string[] info = node.Split(new char[] { '|' });
                    errorSuppressionNodes.Add(info[0], new ErrorSuppressionInfo() { WSKeyNode = info[0], ReplacementString = info[1] });
                }
            }
        }
         * */
    }

    [WebMethod]
    public string IsAlive(string message, string message2)
    {
        return "Service says " + message + "; " + message2;
    }

    private void Trace(string s)
    {
        System.Diagnostics.Debug.WriteLine(s);
        if (!debug || String.IsNullOrEmpty(traceFile)) return;
        try
        {
            File.AppendAllText(traceFile, s + Environment.NewLine);
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("Error writing to trace file " + traceFile + ": " + e.Message);
        }
    }

    [WebMethod]
    public string DownloadReport(string serviceRequestId, string action, string dispSubType)
    {
        string reportNames = "";
        System.Text.StringBuilder sb = new StringBuilder();
        String sharedFolder = ConfigurationManager.AppSettings["SharedFolder"];
        String sharedFolderURI = ConfigurationManager.AppSettings["SharedFolderURI"];
        if (!sharedFolderURI.EndsWith("/")) sharedFolderURI += "/";
        //serviceRequestId = "582939FA-D037-E111-A3DA-005056883635"; action = "0820";

        try {

            validateSharedFolder(sharedFolder);

            string ActionReportFullName = ConfigurationManager.AppSettings[action];
            string ActionReportParameterName = ConfigurationManager.AppSettings[action + "-ParameterName"];

            string InformalClaims = ConfigurationManager.AppSettings["0820ab-10"];
            string InformalClaimsParameterName = ConfigurationManager.AppSettings["0820ab-10-ParameterName"];

            ReportExecution reportService = new ReportExecution();
            Dictionary<string, string> data = new Dictionary<string, string>();
            EntityData sr = new EntityData("va_servicerequest", new Guid(serviceRequestId), data);

            if (ActionReportFullName != null)
            {
                byte[] actionReport = reportService.ExecuteReport(ActionReportFullName, ActionReportParameterName, sr.guid.ToString());
                sr.AddReport(ActionReportFullName, actionReport); // FIX REPORT ACCESSOR
            }
            if (dispSubType.ToUpper() == "CLAIM:INFORMAL - AB-10 LETTER")
            {
                byte[] informalClaimsReport = reportService.ExecuteReport(InformalClaims, InformalClaimsParameterName, sr.guid.ToString());
                sr.AddReport(InformalClaims, informalClaimsReport);
            }

            foreach (var report in sr.reports)
            {
                if (report.Item1 != null && report.Item2 != null)
                {
                    //validate that Item1 does not traverse back up from the shared directory - per Fortify Scan results
                    if (report.Item1.Contains(".."))
                    {
                        throw new Exception(String.Format("Exception caught in DAC.DownloadReport(): {0} {1}", "Bad report.Item1 format: " + report.Item1, String.Empty));
                    }

                    string fn = report.Item1 + "_" + serviceRequestId + ".pdf";
                    string body = Convert.ToBase64String(report.Item2);
                    string fileName = System.IO.Path.Combine(new string[] { sharedFolder, fn });
                    //string fileNameURI = System.IO.Path.Combine(new string[] { sharedFolderURI, fn });
                    string fileNameURI = sharedFolderURI + fn;

                    File.WriteAllBytes(fileName, report.Item2);

                    sb.AppendFormat("{0},", fileNameURI);
                    //using (FileStream stream = new FileStream(fileName, FileMode.Create))
                    //{
                    //    using (BinaryWriter writer = new BinaryWriter(stream))
                    //    {
                    //        writer.Write(body);
                    //        writer.Close();
                    //    }
                    //}

                }
                else
                {
                    Trace("\n ...Report List for SR Id: " + serviceRequestId + " has null values. Check report, connection, and Error logs.\n");
                }
            }

            Trace("\n.........done\n");

        }
        catch (System.ServiceModel.Security.MessageSecurityException e)
        {
            Trace(e.Message);
            return String.Format("MessageSecurityException: {0}", e.Message);
        }
        catch (System.ServiceModel.FaultException ex)
        {
            Trace(ex.Message);
            return String.Format("FaultException: {0}", ex.Message);
        }
        catch (Exception ex)
        {
            Trace(ex.Message);
            return String.Format("Exception: {0}", ex.Message);
        }
        reportNames = sb.ToString();
        if (reportNames.Length > 0)
        {
            reportNames = reportNames.Substring(0, reportNames.Length - 1);
        }
        return reportNames;
    }

    [WebMethod]
    public string DownloadReportMultipleParameters(string recordId, string reportName, string parameters)
    {
        System.Text.StringBuilder sb = new StringBuilder();
        String sharedFolder = ConfigurationManager.AppSettings["SharedFolder"];
        String sharedFolderURI = ConfigurationManager.AppSettings["SharedFolderURI"];
        if (!sharedFolderURI.EndsWith("/")) sharedFolderURI += "/";
        
        try
        {
            validateSharedFolder(sharedFolder);

            //validate that reportName does not traverse back up from the shared directory - per Fortify Scan results
            if (reportName.Contains(".."))
            {
                throw new Exception(String.Format("Exception caught in DAC.DownloadReportMultipleParameters(): {0} {1}", "Bad reportName format: " + reportName, String.Empty));
            }


            ReportExecution reportService = new ReportExecution();
            byte[] generatedReport = null;

            if (reportName != null)
            {
                generatedReport = reportService.ExecuteReportMultipleParameters(reportName,parameters);
            }
            
            if(generatedReport!=null)
            {
                    string fn = reportName + "_" + recordId + ".pdf";
                    string body = Convert.ToBase64String(generatedReport);
                    string fileName = System.IO.Path.Combine(new string[] { sharedFolder, fn });
                    string fileNameURI = sharedFolderURI + fn;

                    File.WriteAllBytes(fileName, generatedReport);

                    sb.AppendFormat("{0},", fileNameURI);
            }

            Trace("\n.........done\n");

        }
        catch (System.ServiceModel.Security.MessageSecurityException e)
        {
            Trace(e.Message);
            return String.Format("MessageSecurityException: {0}", e.Message);
        }
        catch (System.ServiceModel.FaultException ex)
        {
            Trace(ex.Message);
            return String.Format("FaultException: {0}", ex.Message);
        }
        catch (Exception ex)
        {
            Trace(ex.Message);
            return String.Format("Exception: {0}", ex.Message);
        }
        String reportNames = sb.ToString();
        if (reportNames.Length > 0)
        {
            reportNames = reportNames.Substring(0, reportNames.Length - 1);
        }
        return reportNames;
    }

    /// <summary>
    /// To satisfy Fortify secure code analysis, we need to perform validation on the shared path from the config file.
    /// Since the paths could be different in different environments, we are just validating that the END of the path is either PDF or VVA
    /// That will satisfy all current paths in current environments.
    /// </summary>
    /// <param name="folderPath">the sharedfolder value read from the config file</param>
    private void validateSharedFolder(string folderPath)
    {
        if ((!folderPath.EndsWith("PDF", StringComparison.InvariantCultureIgnoreCase)) && (!folderPath.EndsWith("VVA", StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new Exception("Error validating shared folder on the server. Path must end in 'PDF'");
        }
    }

/*
    [WebMethod]
    public string DownloadReport2(string serviceRequestId, string action)
    {
        string reportNames = "";
        System.Text.StringBuilder sb = new StringBuilder();
        String sharedFolder = ConfigurationManager.AppSettings["SharedFolder"];

        try
        {
            CrmConnect conn = new CrmConnect();
            var orgService = conn.ConnectToCRM();
            CrmRequestService CrmRequest = new CrmRequestService(orgService);
            ConditionExpression condition = new ConditionExpression("va_servicerequestid", ConditionOperator.Equal, serviceRequestId);
            List<EntityData> servicerequests = CrmRequest.RetrieveServiceRequests(condition);
            Trace(String.Format("...............done.  {0} queued SR(s) found\n", servicerequests != null ? servicerequests.Count : 0));

            Trace("Retrieving Associated Phone Calls...");
            foreach (var sr in servicerequests)
            {
                if (sr.data["va_originatingcallid"] != null && sr.data["va_originatingcallid"] != "")
                {
                    var pcData = CrmRequest.RetrievePhoneCallInfo(new Guid(sr.data["va_originatingcallid"]));
                    if (pcData == null) { continue; }
                    foreach (var entry in pcData)
                    {
                        sr.data.Add(entry.Key, entry.Value);
                    }
                }
            }
            Trace(".................done\n");

            Logger logger = new Logger(orgService);

            Trace("Executing Reports...........");
            ReportExecution reportService = new ReportExecution(ref logger);
            foreach (var sr in servicerequests)
            {
                string ActionReportFullName = ConfigurationManager.AppSettings[action];
                string ActionReportParameterName = ConfigurationManager.AppSettings[action + "-ParameterName"];

                string InformalClaims = ConfigurationManager.AppSettings["0820ab-10"];
                string InformalClaimsParameterName = ConfigurationManager.AppSettings["0820ab-10-ParameterName"];

                if (ActionReportFullName != null)
                {
                    byte[] actionReport = reportService.ExecuteReport(ActionReportFullName, ActionReportParameterName, sr.guid.ToString());
                    sr.AddReport(ActionReportFullName, actionReport); // FIX REPORT ACCESSOR
                }
                if (sr.data.ContainsKey("va_disposition") && sr.data.ContainsKey("va_dispositionsubtype") &&
                    sr.data["va_disposition"] == "Claim" && sr.data["va_dispositionsubtype"] == "Claim:Informal - AB-10 Letter")
                {
                    byte[] informalClaimsReport = reportService.ExecuteReport(InformalClaims, InformalClaimsParameterName, sr.guid.ToString());
                    sr.AddReport(InformalClaims, informalClaimsReport);
                }

                foreach (var report in sr.reports)
                {
                    if (report.Item1 != null && report.Item2 != null)
                    {
                        string fn = ActionReportFullName + "_" + serviceRequestId + ".pdf";
                        string body = Convert.ToBase64String(report.Item2);
                        string[] arr = { sharedFolder, fn };
                        string fileName = System.IO.Path.Combine(arr);

                        File.WriteAllBytes(fileName, report.Item2);

                        sb.AppendFormat("{0},", fileName);
                        //using (FileStream stream = new FileStream(fileName, FileMode.Create))
                        //{
                        //    using (BinaryWriter writer = new BinaryWriter(stream))
                        //    {
                        //        writer.Write(body);
                        //        writer.Close();
                        //    }
                        //}

                    }
                    else
                    {
                        Trace("\n ...Report List for SR Id: " + serviceRequestId + " has null values. Check report, connection, and Error logs.\n");
                    }
                }
            }

            Trace("\n.........done\n");

        }
        catch (System.ServiceModel.FaultException ex)
        {
            Trace(ex.Message);
            return String.Format("<S:Fault><ns2:exception><message>" + "Error while downloading a report: {0}</message></ns2:exception></S:Fault>", ex.Message);
        }
        catch (Exception ex)
        {
            Trace(ex.Message);
            return String.Format("<S:Fault><ns2:exception><message>" + "Error while downloading a report: {0}</message></ns2:exception></S:Fault>", ex.Message);
        }
        string result = sb.ToString();
        if (result.Length > 0)
        {
            result = result.Substring(0, result.Length - 1);
        }
        return result;
    }
 * */
}

public class ErrorSuppressionInfo
{
    public String WSKeyNode { get; set; }
    public String ReplacementString { get; set; }
}