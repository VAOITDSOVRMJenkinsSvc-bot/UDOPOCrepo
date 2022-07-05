using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using wsReportExecution2005;
using System.ServiceModel;
using System.Net;
using System.ServiceModel.Description;

namespace VRM.EmailReport
{
    class ReportExecution
    {
        private string baseAddress { get; set; }
        private Uri EndpointAddress { get; set; }
        private String DBName { get; set; }

        private String UserName { get; set; }
        private String Pwd { get; set; }     


        public ReportExecution()
        {
            DBName = ConfigurationManager.AppSettings["DBName"];
            baseAddress = ConfigurationManager.AppSettings["ReportServer"];
            EndpointAddress = new Uri(baseAddress + "/ReportServer/ReportExecution2005.asmx");

            UserName = ConfigurationManager.AppSettings["UserName"];
            Pwd = ConfigurationManager.AppSettings["Pwd"];
        }

        public byte[] ExecuteReport(string ReportName, string ParameterName, string ParameterValue)
        {
            string extension, mimeType, encoding;
            string reportPath = "/" + DBName + "/" + ReportName;
            Guid srId = new Guid(ParameterValue);

            using (ReportExecutionServiceSoapClient rs = new ReportExecutionServiceSoapClient("ReportExecutionServiceSoap", new EndpointAddress(EndpointAddress)))
            {
                UserName = ConfigurationManager.AppSettings["UserName"];
                Pwd = ConfigurationManager.AppSettings["Pwd"];
                String Domain = ConfigurationManager.AppSettings["Domain"];
                if (!String.IsNullOrEmpty(UserName))
                    rs.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential(UserName, Pwd, Domain);
                else
                    rs.ClientCredentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;

                rs.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

                byte[] result = null;
                string historyID = null;
                string devInfo = @"<DeviceInfo><Toolbar>False</Toolbar></DeviceInfo>";
                Warning[] warnings;
                string[] streamIds;

                ParameterValue[] executionParams = new ParameterValue[1];
                executionParams[0] = new ParameterValue();
                executionParams[0].Label = ParameterName; //"ServiceRequestGUID";
                executionParams[0].Name = ParameterName; //"ServiceRequestGUID";
                executionParams[0].Value = ParameterValue;

                ExecutionInfo execInfo = null;
                ServerInfoHeader serverInfoHeader = null;
                TrustedUserHeader trustedUserHeader = null;

                try
                {
                    
                    ExecutionHeader execHeader = rs.LoadReport(trustedUserHeader, reportPath, historyID, out serverInfoHeader, out execInfo);
                    rs.SetExecutionParameters(execHeader, trustedUserHeader, executionParams, "en-us", out execInfo);
                    rs.Render(execHeader, trustedUserHeader, "PDF", devInfo,
                        out result, out extension, out mimeType, out encoding, out warnings, out streamIds);
                }
                catch (System.ServiceModel.Security.MessageSecurityException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    throw e;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    throw e;
                }

                return result;
            }
        }

        public byte[] ExecuteReportMultipleParameters(string ReportName, string Parameters)
        {
            string extension, mimeType, encoding;
            string reportPath = "/" + DBName + "/" + ReportName;


            using (ReportExecutionServiceSoapClient rs = new ReportExecutionServiceSoapClient("ReportExecutionServiceSoap", new EndpointAddress(EndpointAddress)))
            {
                UserName = ConfigurationManager.AppSettings["UserName"];
                Pwd = ConfigurationManager.AppSettings["Pwd"];
                String Domain = ConfigurationManager.AppSettings["Domain"];
                if (!String.IsNullOrEmpty(UserName))
                    rs.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential(UserName, Pwd, Domain);
                else
                    rs.ClientCredentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;

                rs.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

                byte[] result = null;
                string historyID = null;
                string devInfo = @"<DeviceInfo><Toolbar>False</Toolbar></DeviceInfo>";
                Warning[] warnings;
                string[] streamIds;

                string[] myParameters = Parameters.Split(new char[] { '|' });

                ParameterValue[] executionParams = new ParameterValue[myParameters.Length];
                
                for (int i = 0; i < myParameters.Length; i++)
                {
                    string[] currentParam = myParameters[i].Split(new char[] { ':' });
                    
                    executionParams[i] = new ParameterValue();
                    executionParams[i].Label = currentParam[0]; //"ServiceRequestGUID";
                    executionParams[i].Name = currentParam[0]; //"ServiceRequestGUID";
                    executionParams[i].Value = currentParam[1];
                }

                ExecutionInfo execInfo = null;
                ServerInfoHeader serverInfoHeader = null;
                TrustedUserHeader trustedUserHeader = null;

                try
                {

                    ExecutionHeader execHeader = rs.LoadReport(trustedUserHeader, reportPath, historyID, out serverInfoHeader, out execInfo);
                    rs.SetExecutionParameters(execHeader, trustedUserHeader, executionParams, "en-us", out execInfo);
                    rs.Render(execHeader, trustedUserHeader, "PDF", devInfo,
                        out result, out extension, out mimeType, out encoding, out warnings, out streamIds);
                }
                catch (System.ServiceModel.Security.MessageSecurityException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    throw e;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    throw e;
                }

                return result;
            }
        }

    }
}
