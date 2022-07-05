using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UDO.Crm.LOB.Controllers.Claims;
using VRM.Integration.UDO.Claims.Messages;

namespace UDO.Crm.LOB.Tests.Claims
{
    [TestClass]
    public class ClaimsTests
    {
        private readonly Guid userId = Guid.NewGuid();
        private readonly string organizationName = "UDODEV";

        public ClaimsTests()
        {

        }

        [TestMethod]
        public void createClaimsTest()
        {
            UDOcreateUDOClaimsRequest request = new UDOcreateUDOClaimsRequest()
            {
                Debug = false, fileNumber = "123450", idProofId = Guid.NewGuid(),
                UserId = userId, OrganizationName = organizationName
            };

            ClaimsController c = new ClaimsController();
            var response = c.Post(request) as UDOcreateUDOClaimsResponse;
            if (response.ExceptionOccured)
            {
                var exceptionMessage = response.ExceptionMessage;
            }
        }
    }
}
