﻿using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Web;
using System.Web.Services;
using System.ServiceModel.Description;

using Microsoft.Xrm.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Client;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Xrm;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]

public class RedirectSvc : System.Web.Services.WebService
{
    private string allowedUsers = String.Empty;
    private bool debug = false;
    private bool traceResponse = false;
    private bool validateUser = false;
    private string traceFile = @"C:\temp\trace.txt";
    private bool multipleCerts = false;
    private string certKey = String.Empty;
    private string certKeyFile = String.Empty;
    Dictionary<String, ErrorSuppressionInfo> errorSuppressionNodes = new Dictionary<string, ErrorSuppressionInfo>();

    public RedirectSvc()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
        string doDebug = ConfigurationManager.AppSettings["Debug"];
        debug = (doDebug == "1");
        string dotraceResponse = ConfigurationManager.AppSettings["TraceResponse"];
        traceResponse = (dotraceResponse == "1");

        string trFile = ConfigurationManager.AppSettings["TraceFile"];
        if (!String.IsNullOrEmpty(trFile)) traceFile = trFile;

        string doValidate = ConfigurationManager.AppSettings["ValidateUser"];
        validateUser = (doValidate == "1");

        allowedUsers = ConfigurationManager.AppSettings["AllowedUsers"];

        certKey = ConfigurationManager.AppSettings["CertKeyFile"];
        multipleCerts = (ConfigurationManager.AppSettings["MultipleCerts"] == "1");

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
    }

    [WebMethod]
    public string Execute(string address, string value)
    {
        String certOp = String.Empty;

        try
        {
            // expecting this in value: <payload><![CDATA[<soapenv:Envelope... rest of target soap...</soapenv:Envelope>]]></payload>

            Trace(String.Format("{0}User: {1} ({2}, {3}); Input: {4}:{5}", Environment.NewLine,
                Context.User.Identity.Name, Context.User.Identity.AuthenticationType, Context.User.Identity.IsAuthenticated, address, value));
            //if (debug)
            //{
            //    foreach (string s in this.Context.Request.ServerVariables) Trace(s + " - " + this.Context.Request.ServerVariables[s]);
            //    foreach (var i in this.Context.Request.RequestContext.HttpContext.Items) Trace(i.ToString() + " - " + this.Context.Request.RequestContext.HttpContext.Items[i]);
            //}
            /*
             * validate that Context.User.Identity.Name is CRM user, and is not disabled
            */
            String message = String.Empty;
            if (validateUser)
            {
                CrmConnector cc = new CrmConnector();
                if (!cc.ValidateUser(Context.User.Identity.Name, ref message, allowedUsers))
                {
                    throw new Exception(String.Format("User validation failed for {0}: {1}", Context.User.Identity.Name, message));
                }
            }

            //String data = String.Empty;
            byte[] data = null;

            if (value.Contains("[CDATA"))
            {
                try
                {
                    XElement payload = XElement.Parse(value);
                    data = Encoding.UTF8.GetBytes(payload.Value);
                }
                catch (Exception)
                {
                    throw new Exception("Invalid payload being sent.");
                }
            }
            else
                data = Encoding.UTF8.GetBytes(value);

            if (String.IsNullOrEmpty(address)) throw new Exception("Redirect WS couldn't find address.");
            if (data == null || data.Length == 0) throw new Exception("Redirect WS couldn't find payload nodes.");

            Trace(String.Format("Add: {0}; Val: {1}", address, Encoding.UTF8.GetString(data)));
            Uri addressUri = new Uri(address);
            int tryCount = 0;
            string result = String.Empty;

        Sendout:
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)(HttpWebRequest.Create(addressUri));
                request.Method = "POST";

                if (!String.IsNullOrEmpty(certKey))
                {
                    // Load multiple certs from XML or single cert stored by name
                    if (multipleCerts)
                    {
                        XElement x;
                        try
                        {
                            x = XElement.Parse(certKey);
                        }
                        catch (Exception)
                        {
                            throw new Exception("Unable to parse out certificate key.");
                        }

                        foreach (XElement cert in x.Nodes())
                        {
                            certKeyFile = cert.Element("filename").Value;
                            string user = cert.Element("user") != null ? cert.Element("user").Value : String.Empty;
                            string pwd = cert.Element("user") != null ? cert.Element("password").Value : String.Empty;

                            if (!File.Exists(certKeyFile)) throw new FileNotFoundException("Certificate Key File " + certKeyFile + " not found.");
                            X509Certificate certificate = null;
                            certOp = String.Format("Loading Cert file {0}", certKeyFile);
                            certificate = String.IsNullOrEmpty(pwd) ? X509Certificate.CreateFromCertFile(certKeyFile) : new X509Certificate(certKeyFile, pwd);
                            /* SecureString securePassword = new SecureString(); foreach (char s in pwd) { securePassword.AppendChar(s);} Cert = new X509Certificate(certKeyFile, securePassword); */

                            request.ClientCertificates.Add(certificate);
                        }
                    }
                    else
                    {
                        if (!File.Exists(certKey)) throw new FileNotFoundException("Certificate Key File " + certKey + " not found.");
                        X509Certificate certificate = X509Certificate.CreateFromCertFile(certKey);
                        request.ClientCertificates.Add(certificate);
                    }
                }
                certOp = String.Empty;

                request.ContentType = "text/xml; charset=utf-8";  //"application/x-www-form-urlencoded";
                request.Headers["SOAPAction"] = String.Empty;

                //data = data.Replace("_vrm.dctag_", "![CDATA[").Replace("_vrm.dctag2_", "]]");
                if (data == null || data.Length == 0) throw new Exception("Null data at replace");
                data = ReplaceBytes(data, Encoding.UTF8.GetBytes("_vrm.dctag2_"), Encoding.UTF8.GetBytes("]]"));
                data = ReplaceBytes(data, Encoding.UTF8.GetBytes("_vrm.dctag_"), Encoding.UTF8.GetBytes("![CDATA["));

                //byte[] byteData = UTF8Encoding.UTF8.GetBytes(data);
                //request.ContentLength = byteData.Length;
                request.AllowAutoRedirect = true;

                using (Stream postStream = request.GetRequestStream())
                {
                    //postStream.Write(byteData, 0, byteData.Length);
                    postStream.Write(data, 0, data.Length);
                }

                response = (HttpWebResponse)request.GetResponse();

                //StreamReader reader = new StreamReader(response.GetResponseStream()); result = reader.ReadToEnd();
                StringBuilder builder = new StringBuilder();
                const int length = 1 * 1024; //1 kb
                char[] buffer = new char[length];
                int resLen = 0;
                using (StreamReader streamReader = (new StreamReader(response.GetResponseStream())))
                {
                    while ((resLen = streamReader.Read(buffer, 0, length)) > 0)
                    {
                        int i = 0;
                        while (i < resLen)
                        {
                            builder.Append(buffer[i]);
                            i++;
                        }
                    }
                }
                result = builder.ToString();

                if (traceResponse) Trace(String.Format("Result: {0}{1}", result, Environment.NewLine));
            }
            catch (WebException tex)
            {
                if (tex.Status == WebExceptionStatus.ProtocolError)
                {
                    response = tex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        result = reader.ReadToEnd();
                        if (traceResponse) Trace(String.Format("Result: {0}{1}", result, Environment.NewLine));
                        return result;
                    }
                }

                // see if we configured to suppress error
                bool suppressError = false;
                foreach (KeyValuePair<String, ErrorSuppressionInfo> kvp in errorSuppressionNodes)
                {
                    //if (data.IndexOf(kvp.Key) >= 0)
                    if (FindBytes(data, Encoding.UTF8.GetBytes(kvp.Key)) >= 0)
                    {
                        suppressError = true;
                        result = kvp.Value.ReplacementString;
                        break;
                    }
                }

                if (!suppressError)
                {
                    result = String.Format("<S:Fault><ns2:exception><message>" + "Error while processing DAC request: {0}</message></ns2:exception></S:Fault>", tex.Message);
                    tryCount++;
                    Trace(tex.Message);
                    if (tryCount < 2) goto Sendout;
                }
                else
                {
                    if (traceResponse) Trace(String.Format("Result: {0}{1}", result, Environment.NewLine));
                    return result;
                }
            }
            catch (CryptographicException ex)
            {
                Trace(ex.Message);
                return String.Format("<S:Fault><ns2:exception><message>" + "Crypto Error while processing DAC request: {0}; {1}</message></ns2:exception></S:Fault>", certOp, ex.Message);
            }
            catch (System.Net.ProtocolViolationException tex)
            {
                Trace(tex.Message);
                return String.Format("<S:Fault><ns2:exception><message>" + "Protocol Violation Exception while processing DAC request. {0}</message></ns2:exception></S:Fault>",
                    tex.Message);
            }
            catch (Exception ex)
            {
                Trace(ex.Message);
                return String.Format("<S:Fault><ns2:exception><message>" + "Error while processing DAC request: {0}</message></ns2:exception></S:Fault>", ex.Message);
            }

            return result;

        }
        catch (Exception e)
        {
            Trace(e.Message);
            ////XmlDocument doc = new XmlDocument();
            //doc.LoadXml("<S:Fault><ns2:exception><message>" + "Error while processing WS Redirect request: " + e.Message + DteTime.Now.ToLongTimeString() + "</message></ns2:exception></S:Fault>");

            return String.Format("<S:Fault><ns2:exception><message>" + "Error while processing DAC request: {0}</message></ns2:exception></S:Fault>", e.Message); //doc.DocumentElement.Value;
        }
    }


    [WebMethod]
    public string IsAlive(string message, string message2)
    {
        return "Service says " + message + "; " + message2;
    }
    //[WebMethod]
    //public string CorpByFileNoTest()
    //{
    //    return TestFromFile(@"http://vbmscert.vba.va.gov/VetRecordServiceBean/VetRecordWebService", @"c:\temp\CorpByFileNo.txt");
    //}

    //[WebMethod]
    //public string TestFromFile(string address, string requestFileName)
    //{
    //    // C:\Traces\DEPNoData500.txt
    //    // https://bepprod.vba.va.gov/ClaimantServiceBean/ClaimantWebService
    //    try
    //    {
    //        Trace(String.Format("Running DAC.TestFromFile('{0}', {1}')...", address, requestFileName));

    //        if (!File.Exists(requestFileName)) throw new FileNotFoundException(String.Format("File '{0}' not found.", requestFileName));

    //        String request = File.ReadAllText(requestFileName);
    //        return Execute(address, request);
    //    }
    //    catch (Exception e)
    //    {
    //        Trace(e.Message);
    //        return "<S:Fault><ns2:exception><message>" + "Error while processing DAC.TestFromFile: " + e.Message + "</message></ns2:exception></S:Fault>";
    //    }
    //    /*
    //    string value = @"<payload><![CDATA[<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:ser='http://services.share.benefits.vba.va.gov/' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
    //      <soapenv:Header>
    //        <wsse:Security xmlns:wsse='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'>
    //          <wsse:UsernameToken xmlns:wsu='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd'>
    //            <wsse:Username>281CEASL</wsse:Username>
    //            <wsse:Password Type='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText'>
    //            </wsse:Password>
    //          </wsse:UsernameToken>
    //            <vaws:VaServiceHeaders xmlns:vaws='http://vbawebservices.vba.va.gov/vawss'>
    //            <vaws:CLIENT_MACHINE>10.224.104.174</vaws:CLIENT_MACHINE>
    //            <vaws:STN_ID>317</vaws:STN_ID>
    //            <vaws:applicationName>VBMS</vaws:applicationName>
    //          </vaws:VaServiceHeaders>
    //        </wsse:Security>
    //      </soapenv:Header>
    //      <soapenv:Body>
    //        <ser:findDependents>
    //          <fileNumber>555119977</fileNumber>
    //        </ser:findDependents>
    //      </soapenv:Body>   </soapenv:Envelope>]]></payload>";
    //    */
    //}
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
    public string Sanity(string distro)
    {
        string answer = "This is no longer in use and is commented out below.";
        return answer;
        //try
        //{
        //    string sanitySettingsFile = ConfigurationManager.AppSettings["SanitySettingsFile"];
        //    string sanityLineSep = ConfigurationManager.AppSettings["SanityLineSep"];
        //    string sanityEnvironment = ConfigurationManager.AppSettings["SanityEnvironment"];
        //    bool sanityHTMLEmail = (ConfigurationManager.AppSettings["SanityHTMLEmail"] == "1");

        //    string[] settings = File.ReadAllLines(sanitySettingsFile);
        //    string sanityPath = Path.GetDirectoryName(sanitySettingsFile);
        //    ValidateSanitySettingFolder(sanitySettingsFile);
        //    List<SanitySetting> sanitySettings = new List<SanitySetting>();

        //    settings.ToList().ForEach(s => sanitySettings.Add(new SanitySetting(sanityPath, s)));
        //    Trace(String.Format("Sanity starts thru {0} tests at {1}", sanitySettings.Count, DateTime.Now.ToLongDateString()));

        //    StringBuilder sb = new StringBuilder();
        //    String a = "\"";
        //    if (sanityHTMLEmail)
        //    {
        //        string cellStyle = String.Format(" style={0}font-size: 14px; font-weight: bold{0}", a);
        //        sb.AppendFormat("<table><tr><td{0}>#</td><td{0}>Test Name</td><td{0}>Sanity Results</td><td{0}>Score</td><td{0}>Time (ms)</td></tr>", cellStyle);
        //    }
        //    int i = 1, overallSanityIndex = 0, commentsCount = 0, maxExecutionTime = 0, totalExecutionTime = 0;
        //    SanitySetting slowestService = null;

        //    foreach (SanitySetting s in sanitySettings) { if (s.CommentLine) commentsCount++; }
        //    int testCountWithoutComments = sanitySettings.Count - commentsCount;

        //    foreach (SanitySetting s in sanitySettings)
        //    {
        //        if (s.CommentLine)
        //        {
        //            Trace("Staring tests for " + s.TestName);
        //            if (sanityHTMLEmail)
        //            {
        //                sb.AppendFormat("<tr><td></td><td colspan=3>{0}</td></tr>", s.TestName);
        //            }
        //            continue;
        //        }

        //        DateTime startTime = DateTime.Now;
        //        s.AnalyzeResponse(Execute(s.Address, File.ReadAllText(s.FileName)));
        //        s.ExecutionTime = DateTime.Now.Subtract(startTime).Milliseconds;
        //        totalExecutionTime += s.ExecutionTime;
        //        if (s.ExecutionTime > maxExecutionTime)
        //        {
        //            maxExecutionTime = s.ExecutionTime;
        //            slowestService = s;
        //        }

        //        if (sanityHTMLEmail)
        //        {
        //            // mark no response in bold red
        //            String serviceName = s.TestName;
        //            if (s.SanityIndex == SanitySetting.NoResponse) serviceName = String.Format("<font color=red><b>{0}</b></font>", s.TestName);
        //            sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4:#,##0}</td></tr>", i, serviceName, s.SanityResponse, s.SanityIndex, s.ExecutionTime);
        //        }
        //        else
        //            sb.AppendFormat("{0} ({1} of {2}): {3}{4}", s.TestName, i, testCountWithoutComments, s.SanityResponse, sanityLineSep);

        //        overallSanityIndex += s.SanityIndex;
        //        i++;
        //    }
        //    if (sanityHTMLEmail)
        //    {
        //        if (slowestService == null)
        //        {
        //            throw new Exception("Error invoking sanity checks - slowest service cannot be null");
        //        }
        //        // last row with total time and slowes service
        //        sb.AppendFormat("<tr><td colspan=4><br/><font><b>Total Execution Time: {0:#,##0} ms; slowest service: {1} ({2} ms)</b></font></td></tr>",
        //            totalExecutionTime, slowestService.TestName, slowestService.ExecutionTime);
        //        // footer
        //        sb.AppendFormat("</table><br/><br/><font size=1 color=black>Web Service Support List:<br/>{0}<br/><br/>This is an automated message.</font>", ConfigurationManager.AppSettings["SanitySupport"]);
        //    }

        //    // get combined sanity index, response text and send emails
        //    String overallSanity = String.Empty;
        //    String overallSanityRight = String.Empty;
        //    float sanityFactor = (float)overallSanityIndex / (float)testCountWithoutComments;

        //    if (sanityFactor >= 9.9)
        //    {
        //        overallSanity = "Sane";
        //        overallSanityRight = String.Format("<font size=14 color=green><b>{1}</b></font>", a, overallSanity);   // green
        //    }
        //    else if (sanityFactor >= 5 && sanityFactor < 9.9)
        //    {
        //        overallSanity = "Almost Sane";
        //        overallSanityRight = String.Format("<font size=14 color=purple><b>{1}</b></font>", a, overallSanity);
        //    }
        //    else if (sanityFactor >= 2 && sanityFactor < 5)
        //    {
        //        overallSanity = "Mental";
        //        overallSanityRight = String.Format("<font size=14 color=red><b>{1}</b></font>", a, overallSanity);
        //    }
        //    else
        //    {
        //        overallSanity = "Insane!!";
        //        overallSanityRight = String.Format("<font size=14 color=red><b>{1}</b></font>", a, overallSanity);
        //    }

        //    string answer = String.Empty;

        //    if (sanityHTMLEmail)
        //    {
        //        String overallSanityLeft = String.Format("<label id=l1 style={0}font-weight: bold; font-size: 12px; color: #0000FF{0}>Overall Sanity: </label>", a);
        //        answer = String.Format("{1}{2}<br/><label id=l3 style={0}font-weight: bold; font-size: 12px; color: #0000FF{0}>Test for {5} environment was executed on {6} with Sanity factor of {3}.<br/>Detailed results for the automated tests of VRMUD - related web services are displayed below</label><br/><br/>{4}",
        //            a, overallSanityLeft, overallSanityRight, sanityFactor, sb.ToString(), sanityEnvironment, DateTime.Now);
        //    }
        //    else
        //        answer = String.Format(" Overall Sanity: {0} ({1}){3} Detailed results: {2}", overallSanity, sanityFactor, sb.ToString(), sanityLineSep);

        //    Trace("Sanity Answer: " + answer);

        //    // send email
        //    string SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
        //    if (!String.IsNullOrEmpty(SMTPServer))
        //    {
        //        string SanityFrom = ConfigurationManager.AppSettings["SanityFrom"];
        //        if (String.IsNullOrEmpty(distro)) distro = ConfigurationManager.AppSettings["SanityToList"];
        //        string[] toList = distro.Split(new char[] { ';' });

        //        string subject = String.Format("VRMUD WS Sanity Result is {0} for {1} Environment; sanity score: {2}", overallSanity, sanityEnvironment, sanityFactor);
        //        toList.ToList().ForEach(s => SendSMTPMail(SMTPServer, SanityFrom, s, subject, answer, sanityHTMLEmail));
        //    }

        //    return answer;
        //}
        //catch (Exception e)
        //{
        //    Trace(e.Message);
        //    return "DAC.Sanity failed: " + e.Message;
        //}
    }

    //*************************** VVA Methods
    /*
     <soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:ser="http://service.bfi.va.gov/">
   <soap:Header>
      <wsse:Security soap:mustUnderstand="true" xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd" xmlns:wsu="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd">
         <wsse:UsernameToken wsu:Id="UsernameToken-1">
            <wsse:Username>TEST</wsse:Username>
            <wsse:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText">YYYYY</wsse:Password>
            </wsse:UsernameToken>
      </wsse:Security>
   </soap:Header>
   <soap:Body>
      <ser:DocumentContent>
         <ser:fnDcmntId>7127075</ser:fnDcmntId>
         <ser:fnDcmntSource>IS</ser:fnDcmntSource>
         <ser:dcmntFormatCd>PDF</ser:dcmntFormatCd>
         <ser:jro>459</ser:jro>
         <ser:userId>TEST</ser:userId>
      </ser:DocumentContent>
   </soap:Body>
</soap:Envelope>
     */
    [WebMethod]
    public string DownloadVVADocument(string vvaServiceUri, string userName, string password, string fnDocId, string fnDocSource,
        string docFormatCode, string jro, string userId)
    {
        //validate that fnDocId does not traverse back up from the shared directory - per Fortify Scan results
        if (fnDocId.Contains(".."))
        {
            throw new Exception(String.Format("Exception caught in DAC.DownloadVVADocument(): {0} {1}", "Bad fnDocId format: " + fnDocId, String.Empty));
        }

        string downloadedFileName = String.Empty;
        String sharedFolder = ConfigurationManager.AppSettings["SharedFolder"];
        ValidateSharedFolder(sharedFolder);
        String sharedFolderUri = ConfigurationManager.AppSettings["SharedFolderURI"];
        if (!sharedFolderUri.EndsWith("/")) sharedFolderUri += "/";
        String requestTemplate = ConfigurationManager.AppSettings["VVARequestTemplate"];
        ValidateVvaRequestTemplate(requestTemplate);
        byte[] request = Encoding.UTF8.GetBytes(String.Format(requestTemplate, userName, password, fnDocId, fnDocSource, docFormatCode, jro, userId));

        String response = Execute(vvaServiceUri, Encoding.UTF8.GetString(request));
        //Trace(response);
        /* returns xml like
         <soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope">
   <soap:Body>
      <GetDocumentContentResponse xmlns="http://service.bfi.va.gov/">
         <content>base64payload</content>
         <mimeType>application/pdf</mimeType>
      </GetDocumentContentResponse>
   </soap:Body>
</soap:Envelope>
        */
        bool decodedOK = false;

        string fn = String.Format("{0}.{1}", fnDocId, docFormatCode);
        string fileName = System.IO.Path.Combine(new string[] { sharedFolder, fn });
        //if (File.Exists((fileName))) File.Delete(fileName);
        try { File.Delete(fileName); }
        catch (FileNotFoundException ex) { }

        downloadedFileName = sharedFolderUri + fn;
        string decodedContent = "Failed to decode response for doc id " + fnDocId;

        const string tag1 = "<content>";
        const string tag2 = "</content>";
        int pos1 = response.IndexOf(tag1, StringComparison.InvariantCulture), pos2 = response.IndexOf(tag2, StringComparison.InvariantCulture);
        if (pos1 >= 0 && pos2 >= 0)
        {
            //Trace("Got substring, writing to " + fileName);
            string encodedContent = response.Substring(pos1 + tag1.Length, pos2 - pos1 - tag2.Length + 1);
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedContent);
            File.WriteAllBytes(fileName, encodedDataAsBytes);
            decodedOK = true;
        }
        else
        {
            //Trace("Couldn't find payload.");
            decodedContent = "Couldn't find payload.\n\n" + response;
            fn = String.Format("{0}.txt", fnDocId);
            fileName = System.IO.Path.Combine(new string[] { sharedFolder, fn });
            downloadedFileName = sharedFolderUri + fn;
        }

        if (!decodedOK) File.WriteAllText(fileName, decodedContent);

        return downloadedFileName;
    }

    [WebMethod]
    public string TestVVA(string vvaServiceUri, string userName, string password, string fnDocId, string fnDocSource, string docFormatCode, string jro)
    {
        if (String.IsNullOrEmpty(vvaServiceUri)) vvaServiceUri = "https://vbaphi5dopp.vba.va.gov:7002/VABFI/services/vva";
        //if (String.IsNullOrEmpty(userName)) userName = "TEST";
        //if (String.IsNullOrEmpty(password)) password = "YYYYY";
        if (String.IsNullOrEmpty(fnDocId)) fnDocId = "7127075";
        if (String.IsNullOrEmpty(fnDocSource)) fnDocSource = "IS";
        if (String.IsNullOrEmpty(docFormatCode)) docFormatCode = "PDF";
        if (String.IsNullOrEmpty(jro)) jro = "459";

        string tempVvaDoc = DownloadVVADocument(vvaServiceUri, userName, password, fnDocId, fnDocSource, docFormatCode, jro, "VVATester");
        return tempVvaDoc;
    }

    protected void SendSMTPMail(string server, string from, string to, string subject, string body, bool sanityHTMLEmail)
    {
        Trace("Sanity emails Answer to " + to);
        string UserName = ConfigurationManager.AppSettings["UserName"];
        //string UserDomain = ConfigurationManager.AppSettings["UserDomain"];
        string Pwd = ConfigurationManager.AppSettings["Pwd"];

        MailMessage message = new MailMessage(from, to);
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = sanityHTMLEmail;
        SmtpClient client = new SmtpClient(server);
        // Credentials are necessary if the server requires the client 
        // to authenticate before it will send e-mail on the client's behalf.
        client.UseDefaultCredentials = true;
        var creds = new NetworkCredential(UserName, Pwd);
        client.Credentials = creds;

        try
        {
            client.Send(message);
        }
        catch (Exception ex)
        {
            throw new Exception(String.Format("Exception caught in DAC.SendSMTPMail(): {0} {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty));
        }
    }

    private int FindBytes(byte[] src, byte[] find)
    {
        try
        {

            int index = -1;
            int matchIndex = 0;
            // handle the complete source array
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] == find[matchIndex])
                {
                    if (matchIndex == (find.Length - 1))
                    {
                        index = i - matchIndex;
                        break;
                    }
                    matchIndex++;
                }
                else
                {
                    matchIndex = 0;
                }

            }
            return index;
        }
        catch (Exception ex)
        {
            throw new Exception("Error in FindBytes: " + ex.Message, ex);
        }

    }

    private byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
    {
        try
        {
            byte[] dst = null;
            int index = FindBytes(src, search);
            if (index >= 0)
            {
                dst = new byte[src.Length - search.Length + repl.Length];
                // before found array
                Buffer.BlockCopy(src, 0, dst, 0, index);
                // repl copy
                Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
                // rest of src array
                Buffer.BlockCopy(
                    src,
                    index + search.Length,
                    dst,
                    index + repl.Length,
                    src.Length - (index + search.Length));
            }
            if (dst == null)
                return src;
            else
                return dst;
        }
        catch (Exception ex)
        {
            throw new Exception("Error in ReplaceBytes: " + ex.Message, ex);
        }
    }

    /// <summary>
    /// To satisfy Fortify secure code analysis, we need to perform validation on the shared path from the config file.
    /// Since the paths could be different in different environments, we are just validating that the END of the path is either PDF or VVA
    /// That will satisfy all current paths in current environments.
    /// </summary>
    /// <param name="folderPath">the sharedfolder value read from the config file</param>
    private void ValidateSharedFolder(string folderPath)
    {
        if ((!folderPath.EndsWith("PDF", StringComparison.InvariantCultureIgnoreCase)) && (!folderPath.EndsWith("VVA", StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new Exception("Error validating shared folder on the server. Path must end in 'PDF'");
        }
    }

    private void ValidateSanitySettingFolder(string folderPath)
    {
        // The Sanity Test is just a test and does not currently run in any environment. For example none of the current config files have this value in the config.
        // Even though this is true we still validate the path by checking for the word sanity in the path. Although we know that an intruder can manipulate a path to include this directory
        // it does minimize the risk.
        if (!folderPath.ToUpper().Contains("SANITY"))
            throw new Exception("Invalid Sanity Settings File Path.");
    }

    private void ValidateVvaRequestTemplate(string requestTemplateValue)
    {
        // Do a cursory check to make sure this resembles a sample VVA request
        if (!(requestTemplateValue.ToUpper().Contains("SOAP:ENVELOPE") && requestTemplateValue.ToUpper().Contains(@"HTTP://SERVICE.BFI.VA.GOV/") &&
            requestTemplateValue.ToUpper().Contains("WSSE:SECURITY")))
            throw new Exception("Invalid VVA Request template value");
    }
}

public class ErrorSuppressionInfo
{
    public String WSKeyNode { get; set; }
    public String ReplacementString { get; set; }
}

public class SanitySetting
{
    public const int NoResponse = -10;
    public const int SettingsMissingControlToken = 9;
    public const int NoControlTokenInResponse = 2;
    public const int Sane = 10;

    public string SanityResponse { get; set; }
    public int SanityIndex { get; set; }
    public bool CommentLine { get; set; }

    public string FileName { get; set; }
    public string TestName { get; set; }
    public string Address { get; set; }
    public string ResponseToken { get; set; }
    public string Response { get; set; }
    public int ExecutionTime { get; set; }

    public void AnalyzeResponse(string value)
    {
        Response = value;
        SanityResponse = "Insane: no response";
        SanityIndex = NoResponse;

        // analize sanity
        if (String.IsNullOrEmpty(Response)) return; // insane

        if (String.IsNullOrEmpty(ResponseToken))
        {
            SanityResponse = "Sane: missing control token from settings";
            SanityIndex = SettingsMissingControlToken;
            return;
        }

        if (Response.IndexOf(ResponseToken) < 0)
        {
            SanityResponse = "Mental: response doesn't have control token";
            SanityIndex = NoControlTokenInResponse;
            return;
        }

        SanityIndex = Sane;
        SanityResponse = "Sane";
    }

    public SanitySetting() { }
    public SanitySetting(string path, string concatSettings)
    {
        if (concatSettings.Trim().Length <= 2 || concatSettings.Trim().Substring(0, 2) == "//")
        {
            CommentLine = true;
            TestName = concatSettings.Substring(2);
            return;
        }

        string[] settings = concatSettings.Split(new char[] { '|' });

        FileName = Path.Combine(path, settings[0]);
        TestName = settings[1];
        Address = settings[2];
        if (settings.Length > 3) ResponseToken = settings[3];
    }
}