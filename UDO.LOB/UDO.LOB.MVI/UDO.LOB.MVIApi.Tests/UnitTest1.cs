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
using UDO.LOB.MVI.Messages;
using UDO.LOB.MVI.Processors;
using UDO.LOB.Core;


namespace UDO.LOB.MVIApi.Tests
{
    [TestClass]
    public class UnitTest1
    {

        private IOrganizationService OrgServiceProxy;
        private OrganizationWebProxyClient webProxyClient;
        private List<string> fetchFields;
        private List<string> sqlFields;
        [TestMethod]
        public void TestMethod1()
        {
//            "UserId": "00000000-0000-0000-0000-000000000000",
//  "LogTiming": true,
//  "LogSoap": true,
//  "noAddPerson": false,
//  "Debug": true,
//  "userSL": 7,
//  "MVICheck": true,
//  "BypassMvi": false,
//  "LegacyServiceHeaderInfo": {
//                "StationNumber": "385,
//    "LoginName": "281tuser06",
//    "ApplicationName": "CRMDU",
//    "ClientMachine": "10.0.0.1"
//  },
//  "SSIdString": "796288429",
//  "BirthDate": "01121923",
//  "IsAttended": true,
//  "MessageId": "string"
//}


        UDOCombinedPersonSearchRequest request = new UDOCombinedPersonSearchRequest();
            request.Debug = true;
            request.MessageId = Guid.NewGuid().ToString();
            request.SSIdString = "796288429";
            request.BirthDate = "01121923";
            request.OrganizationName = "CRMEDEV";
            request.IsAttended = true;
            request.LegacyServiceHeaderInfo = new UDOHeaderInfo
            {
                StationNumber = "385",
                LoginName = "281tuser06",
                ApplicationName = "CRMDU",
                ClientMachine = "10.0.0.1"
            };
            request.UserId = new Guid("0a90c445-88a2-e511-9418-00155d14f3d0");
            request.userSL = 7;
            request.MVICheck = true;
            request.Debug = true;
            request.BypassMvi = false;

            UDOCombinedPersonSearchProcessor processor = new UDOCombinedPersonSearchProcessor();
        
            processor.Execute(request);
        }
        [TestMethod]
        public void TestMethodedipi()
        {
            //            "UserId": "00000000-0000-0000-0000-000000000000",
            //  "LogTiming": true,
            //  "LogSoap": true,
            //  "noAddPerson": false,
            //  "Debug": true,
            //  "userSL": 7,
            //  "MVICheck": true,
            //  "BypassMvi": false,
            //  "LegacyServiceHeaderInfo": {
            //                "StationNumber": "385,
            //    "LoginName": "281tuser06",
            //    "ApplicationName": "CRMDU",
            //    "ClientMachine": "10.0.0.1"
            //  },
            //  "SSIdString": "796288429",
            //  "BirthDate": "01121923",
            //  "IsAttended": true,
            //  "MessageId": "string"
            //}


            UDOCombinedPersonSearchRequest request = new UDOCombinedPersonSearchRequest();
            request.Debug = true;
            request.MessageId = Guid.NewGuid().ToString();
            request.Edipi = "101026241";
            request.OrganizationName = "CRMEDEV";
            request.IsAttended = true;
            request.LegacyServiceHeaderInfo = new UDOHeaderInfo
            {
                StationNumber = "385",
                LoginName = "281tuser06",
                ApplicationName = "CRMDU",
                ClientMachine = "10.0.0.1"
            };
            request.UserId = new Guid("0a90c445-88a2-e511-9418-00155d14f3d0");
            request.userSL = 7;
            request.MVICheck = true;
            request.Debug = true;
            request.BypassMvi = false;

            UDOCombinedPersonSearchProcessor processor = new UDOCombinedPersonSearchProcessor();

            processor.Execute(request);
        }
    }
}
