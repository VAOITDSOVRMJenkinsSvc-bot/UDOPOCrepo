using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UDO.LOB.UserTool.Controllers;
using UDO.LOB.UserTool.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System.Configuration;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Core;
using UDO.LOB.Extensions;

namespace UDO.LOB.UserToolApi.Tests
{
    [TestClass]
    public class SecurityControllerTest
    {
        [TestMethod]
        public void AssociateTest()
        {
            try
            {
                UDOSecurityAssocRequest request = new UDOSecurityAssocRequest
                {
                    Debug = true,
                    UserId = Guid.NewGuid(),
                    OrganizationName = "UDO",
                    MessageId = "UDOSecurityAssocRequest",
                    Relationship = null,
                    RelatedParentId = Guid.NewGuid(),
                    RelatedParentEntityName = "udo_person",
                    RelatedParentFieldName = "name",
                    LogTiming = false,
                    One = null,
                    Many = null
                };

                Uri baseUri = new Uri(ConfigurationManager.AppSettings["LobApimUri"].ToString());
                //string apiRoute = @"/api/security/associate";

                //Uri lobApi = new Uri(baseUri, apiRoute);

                //LogSettings logSettings = new LogSettings
                //{
                //    UserId = request.UserId,
                //    CallingMethod = "RTestR",
                //    Org = request.OrganizationName,
                //    ConfigFieldName = "na"
                //};

                // UDOcreateAwardsResponse response = WebApiUtility.SendReceive<UDOcreateAwardsResponse>(lobApi, createAward.MessageId, createAward, logSettings);
                var response = WebApiUtility.SendReceive<UDOSecurityAssocResponse>(request, WebApiType.LOB);
                
                Console.WriteLine(JsonHelper.Serialize<UDOSecurityAssocResponse>(response));
            }
            finally
            {

            }

        }

        [TestMethod]
        public void DisassociateTest()
        {
            try
            {
                UDOSecurityDisassocRequest request = new UDOSecurityDisassocRequest
                {
                    RelatedParentId = Guid.NewGuid(),
                    RelatedParentEntityName = "udo_person",
                    RelatedParentFieldName = "name",
                    OrganizationName = "UDO",
                    UserId = Guid.NewGuid(),
                    LogTiming = false,
                    LogSoap = false,
                    Debug = false,
                    Relationship = null,
                    One = null,
                    Many = null,
                    MessageId = "UDOSecurityDisassocRequest"
                };

                Uri baseUri = new Uri(ConfigurationManager.AppSettings["LobApimUri"].ToString());

                var response = WebApiUtility.SendReceive<UDOSecurityDisassocResponse>(request, WebApiType.LOB);

                Console.WriteLine(JsonHelper.Serialize<UDOSecurityDisassocResponse>(response));
            }
            finally
            {

            }
        }

    }
}
