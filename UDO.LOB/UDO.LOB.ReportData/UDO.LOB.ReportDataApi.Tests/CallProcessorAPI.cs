using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml;
using UDO.LOB.Extensions;
using UDO.LOB.ReportData.Processors;
using UDO.LOB.ReportData.Messages;
using UDO.LOB.Core;

namespace UDO.LOB.ReportDataApi.Tests
{
    [TestClass]
    public class CallProcessorAPI
    {

        private IOrganizationService OrgServiceProxy;
        private OrganizationWebProxyClient webProxyClient;
        private List<string> fetchFields;
        private List<string> sqlFields;

   
        [TestMethod]
        public void CallAPITest()
        {

            UDOpopulateReportDataRequest request = new UDOpopulateReportDataRequest();
            request.Debug = true;
            request.MessageId = Guid.NewGuid().ToString();
            request.udo_lettergenerationid = new Guid("116CF5C5-7FE1-E511-9438-0050568DF261");
            request.OrganizationName = "CRMEDEV";
            request.UserId = new Guid("0a90c445-88a2-e511-9418-00155d14f3d0");

            UDOpopulateReportDataProcessor processor = new UDOpopulateReportDataProcessor();
            string requestBody = JsonHelper.Serialize<UDOpopulateReportDataRequest>(request);
            processor.Execute(request);
        }
    }
}
