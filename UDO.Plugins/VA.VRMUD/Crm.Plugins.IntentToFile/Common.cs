using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using System.Net.Security;

namespace Crm.WebServices
{
    public class Common
    {
        public string SoapRequestBody;
        public string soapRequestBodyEnd = "</soapenv:Body></soapenv:Envelope>";

        public string soapRequestBody1 = //"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:q0=\"";

        public string soapRequestBody2 = "\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
            "<soapenv:Header>" +
            "<wsse:Security xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">" +
            "<wsse:UsernameToken xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">" +
            //"<wsse:Username>281GHEDR</wsse:Username>" +
            "<wsse:Username>dummy_Username</wsse:Username>" +
            "<wsse:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText\"></wsse:Password>" +
            "</wsse:UsernameToken>" +
            "<vaws:VaServiceHeaders xmlns:vaws=\"http://vbawebservices.vba.va.gov/vawss\">" +
            //"<vaws:CLIENT_MACHINE>10.224.104.174</vaws:CLIENT_MACHINE>" +
            "<vaws:CLIENT_MACHINE>dummy_CLIENT_MACHINE</vaws:CLIENT_MACHINE>" +
            //"<vaws:STN_ID>281</vaws:STN_ID>" +
            "<vaws:STN_ID>dummy_STN_ID</vaws:STN_ID>" +
            //"<vaws:applicationName>FBS</vaws:applicationName>" +
            "<vaws:applicationName>dummy_applicationName</vaws:applicationName>" +
            "</vaws:VaServiceHeaders>" +
            "</wsse:Security>" +
            "</soapenv:Header>" +
            "<soapenv:Body>";


        public string InvokeWebService(BgsConfig bgsConfig, Uri uri, string messageBody, string endpoint)
        {
            bool doUseCert = false;

            if (!string.IsNullOrEmpty(bgsConfig.ThumbPrint))
                doUseCert = true;

            string response = string.Empty;
            HttpWebRequest request = null;
            if (doUseCert)
            {
                bool isCertFound = false;
                //X509Store storeTrustedPeople = null;
                X509Store storeLocalMachine = null;
                try
                {
                    storeLocalMachine = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                    storeLocalMachine.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                    X509Certificate2Collection certificatesLocalMachine = storeLocalMachine.Certificates;
                    var matchedCertificates = storeLocalMachine.Certificates.Find(X509FindType.FindByThumbprint, bgsConfig.ThumbPrint, true);
                    if (matchedCertificates.Count > 0)
                    {
                        request = (HttpWebRequest)WebRequest.Create(uri);
                        request.ClientCertificates.Add(matchedCertificates[0]);
                        isCertFound = true;
                    }
                    storeLocalMachine.Close();

                    if (!isCertFound)
                        throw new Exception("The configured certificate was not found on the server");
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (storeLocalMachine != null)
                        storeLocalMachine.Close();
                }
            }
            else
            {
                Console.WriteLine(string.Format("SERVICE_URL: {0}", uri));

                request = (HttpWebRequest)HttpWebRequest.Create(uri);
            }

            soapRequestBody2 = soapRequestBody2.Replace("dummy_Username", bgsConfig.Username);
            soapRequestBody2 = soapRequestBody2.Replace("dummy_STN_ID", bgsConfig.StationId);
            soapRequestBody2 = soapRequestBody2.Replace("dummy_applicationName", bgsConfig.ApplicationName);
            soapRequestBody2 = soapRequestBody2.Replace("dummy_CLIENT_MACHINE", "10.224.104.174");
            //soapRequestBody2 = soapRequestBody2.Replace("dummy_password", "");

            SoapRequestBody = soapRequestBody1 + endpoint + soapRequestBody2 + messageBody + soapRequestBodyEnd;

            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";//"text/xml";
            request.Headers.Add("SOAPAction:\"\"");
            request.ContentLength = SoapRequestBody.Length;
            request.KeepAlive = false;

            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(SoapRequestBody);
                streamWriter.Close();
            }

            using (StreamReader streamReader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                while (!streamReader.EndOfStream)
                {
                    response = streamReader.ReadLine();
                }
            }

            return response;
        }
    }
}