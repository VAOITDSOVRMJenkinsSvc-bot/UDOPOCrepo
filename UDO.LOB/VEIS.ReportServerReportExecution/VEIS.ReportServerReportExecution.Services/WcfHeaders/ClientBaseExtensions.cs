using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client; 
using ServiceEndpoint = System.ServiceModel.Description.ServiceEndpoint; 

namespace VEIS.ReportServerReportExecution.Services
{
    public static class ClientBaseExtensions
    {
        public static string HttpPostFile(string uri, string fileName)
        {
            var text = System.IO.File.ReadAllText(fileName);

            return HttpPost(uri, text);
        }

        public static string HttpPost(string uri, string parameters)
        {
            var req = System.Net.WebRequest.Create(uri);

            req.ContentType = "text/xml";

            req.Method = "POST";

            var bytes = System.Text.Encoding.ASCII.GetBytes(parameters);

            req.ContentLength = bytes.Length;

            var os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);
            os.Close();

            var resp = req.GetResponse();

            var sr = new System.IO.StreamReader(resp.GetResponseStream());

            return sr.ReadToEnd().Trim();
        }
    }
}