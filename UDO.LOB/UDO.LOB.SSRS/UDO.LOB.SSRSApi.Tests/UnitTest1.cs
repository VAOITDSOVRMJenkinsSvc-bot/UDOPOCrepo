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
using UDO.LOB.SSRS.Processors;
using UDO.LOB.SSRS.Messages;
using UDO.LOB.Core;

namespace UDO.LOB.SSRSApi.Tests
{
    [TestClass]

    public class CallProcessorAPI
    {

        private IOrganizationService OrgServiceProxy;
        private OrganizationWebProxyClient webProxyClient;
        private List<string> fetchFields;
        private List<string> sqlFields;

        [TestMethod]
        public void CallAPITestsr()
        {

            UDOpopulateReportDataRequest request = new UDOpopulateReportDataRequest();
            request.Debug = true;
            request.MessageId = Guid.NewGuid().ToString();
            request.udo_servicerequestid = new Guid("695f0693-9f93-e911-a97b-001dd800951b");
            request.OrganizationName = "CRMEDEV";
            request.UserId = new Guid("0a90c445-88a2-e511-9418-00155d14f3d0");

            UDOPopulateReportDataProcessor processor = new UDOPopulateReportDataProcessor();
            string requestBody = JsonHelper.Serialize<UDOpopulateReportDataRequest>(request);
            processor.Execute(request);

            // 695f0693-9f93-e911-a97b-001dd800951b
        }
        [TestMethod]
        public void CallAPITest()
        {

            UDOpopulateReportDataRequest request = new UDOpopulateReportDataRequest();
            request.Debug = true;
            request.MessageId = Guid.NewGuid().ToString();
            request.udo_lettergenerationid = new Guid("76A59C53-9DB3-E911-A998-001DD8308047");
            request.OrganizationName = "CRMEDEV";
            request.UserId = new Guid("0a90c445-88a2-e511-9418-00155d14f3d0");

            UDOPopulateReportDataProcessor processor = new UDOPopulateReportDataProcessor();
            string requestBody = JsonHelper.Serialize<UDOpopulateReportDataRequest>(request);
            processor.Execute(request);

           // 695f0693 - 9f93 - e911 - a97b - 001dd800951b
        }
        [TestMethod]
        public void getDataTest()
        {

            UDORunCRMReportRequest request = new UDORunCRMReportRequest();
            request.Debug = true;
            request.MessageId = Guid.NewGuid().ToString();
            //request.udo_LetterGenerationId = new Guid("B02B6051-D886-E911-A97A-001DD800951B");

        
    





            request.udo_LetterGenerationId = new Guid("50bdcace-a2b3-e911-a998-001dd8308047");
            
            request.OrganizationName = "CRMEDEV";
           // request.UserId = new Guid("0a90c445-88a2-e511-9418-00155d14f3d0");
            request.udo_SSRSReportName = "Blank Letter";
            request.udo_FormatType = "pdf";
            //request.udo_PersonInfo = new PersonInfo
           // {
            //    udo_FirstName = "Rich",
           //     udo_LastName = "Carr"
         //   };
            request.udo_UploadToVBMS = false;

          
            UDORunCRMReportProcessor processor = new UDORunCRMReportProcessor();
            string requestBody = JsonHelper.Serialize<UDORunCRMReportRequest>(request);
            var thisresult = processor.Execute(request);

        }
        [TestMethod]
        public void getDataTestSR()
        {

            UDORunCRMReportRequest request = new UDORunCRMReportRequest();
            request.Debug = true;
            request.MessageId = Guid.NewGuid().ToString();
            //request.udo_LetterGenerationId = new Guid("B02B6051-D886-E911-A97A-001DD800951B");
            //request.udo_SSRSReportName = "Blank Letter - UDO";
          //  [1:56 PM]
      //  Rich Carr(FEDERAL-CRM)
    



            request.udo_ServiceRequestId = new Guid("50bdcace-a2b3-e911-a998-001dd8308047");
            request.udo_SSRSReportName = "Disability Breakdown Letter - UDO";

            
            //int
            //request.udo_LetterGenerationId = new Guid("7f0cce45-3887-e611-946a-0050568d63d9");

            // request.udo_ServiceRequestId = new Guid("695f0693-9f93-e911-a97b-001dd800951b");

            request.OrganizationName = "CRMEDEV";
            // request.UserId = new Guid("0a90c445-88a2-e511-9418-00155d14f3d0");
           
            request.udo_FormatType = "PDF";
            //request.udo_PersonInfo = new PersonInfo
            // {
            //    udo_FirstName = "Rich",
            //     udo_LastName = "Carr"
            //   };
            request.udo_UploadToVBMS = false;


            UDORunCRMReportProcessor processor = new UDORunCRMReportProcessor();
            string requestBody = JsonHelper.Serialize<UDORunCRMReportRequest>(request);
            var thisresult = processor.Execute(request);

        }
    }
}
