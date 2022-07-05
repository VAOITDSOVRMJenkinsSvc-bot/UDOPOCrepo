using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using UDO.LOB.Awards.Messages;
using UDO.LOB.Core;

namespace UDO.LOB.AwardsApi.Tests
{
    [TestClass]
    public class AwardsApiTests
    {
        private readonly bool testLocal;
        private readonly bool testAzure;
        private readonly int iterations;

        public AwardsApiTests()
        {
            // Set local/Azure values
            testLocal = ConfigurationManager.AppSettings["TestLocal"].Equals("true");

            if (!testLocal)
            {
                testAzure = ConfigurationManager.AppSettings["TestAzure"].Equals("true");
            }
            else
            {
                testAzure = false;
            }

            // Set iterations
            iterations = int.Parse(ConfigurationManager.AppSettings["Iterations"]);
        }

        [TestMethod]
        public void CreateAwardsTest()
        {

        }

        [TestMethod]
        public void CreateAwardLinesTest()
        {

        }

        [TestMethod]
        public void CreateAwardsSyncOrchTest()
        {
            // Setup
            var request = new UDOcreateAwardsSyncOrchRequest()
            {
                DiagnosticsConfiguration = null,
                DiagnosticsContext = null,
                MessageId = "00000000-0000-0000-0000-000000000000",
                RelatedParentEntityName = "",
                RelatedParentFieldName = "",
                RelatedParentId = Guid.Empty,
                Debug = ConfigurationManager.AppSettings["Debug"].Equals("true"),
                LegacyServiceHeaderInfo = new UDOHeaderInfo()
                {
                    ApplicationName = ConfigurationManager.AppSettings["ApplicationName"],
                    ClientMachine = ConfigurationManager.AppSettings["ClientMachine"],
                    LoginName = ConfigurationManager.AppSettings["LoginName"],
                    StationNumber = ConfigurationManager.AppSettings["StationNumber"]
                },
                LogSoap = ConfigurationManager.AppSettings["LogSoap"].Equals("true"),
                LogTiming = ConfigurationManager.AppSettings["LogTiming"].Equals("true"),
                OrganizationName = ConfigurationManager.AppSettings["OrganizationName"],
                UserId = new Guid(ConfigurationManager.AppSettings["UserId"]),
                idProofId = Guid.Empty,
                vetsnapshotId = Guid.Empty,
                ownerId = Guid.Empty,
                ownerType = "",
                udo_contactId = Guid.Empty,
                udo_dependentId = Guid.Empty,
                fileNumber = "",
                ptcpntVetId = "",
                ptcpntBeneId = "",
                ptcpntRecipId = "",
                awardTypeCd = "",
                UDOcreateAwardsRelatedEntitiesInfo = new UDOcreateAwardsRelatedEntitiesMultipleRequest[]
                {
                        new UDOcreateAwardsRelatedEntitiesMultipleRequest()
                        {
                            RelatedEntityFieldName = "",
                            RelatedEntityId = Guid.Empty,
                            RelatedEntityName = ""
                        }
                },
                udo_ssn = ""
            };

            bool didOneFail = false;


            // Action
            if (this.testLocal)
            {
                var controller = new Controllers.AwardsController();
                didOneFail = (controller.CreateAwardsSyncOrch(request) as UDOcreateAwardsResponse).ExceptionOccured;
            }
            else if (this.testAzure)
            {
                bool[] responseArray = new bool[iterations];

                for (int x = 0; x < responseArray.Length; x++)
                {
                    responseArray[x] = WebApiUtility.SendReceive<UDOcreateAwardsResponse>(request, WebApiType.LOB).ExceptionOccured;
                }

                foreach (bool responseBool in responseArray)
                {
                    if (responseBool)
                    {
                        didOneFail = true;
                    }
                }
            }
            else
            {
                throw new Exception("The testLocal and testAzure app.config keys are both set to false.");
            }


            // Assertion
            Assert.AreEqual(false, didOneFail);
        }

        [TestMethod]
        public void CreateClothingAllowanceTest()
        {

        }

        [TestMethod]
        public void CreateEVRTest()
        {

        }

        [TestMethod]
        public void CreateIncomeSummaryTest()
        {

        }

        [TestMethod]
        public void CreateDeductionsTest()
        {

        }

        [TestMethod]
        public void CreateDiariesTest()
        {

        }

        [TestMethod]
        public void CreateProceedsTest()
        {

        }

        [TestMethod]
        public void CreateReceivablesTest()
        {

        }

        [TestMethod]
        public void RetrieveAwardsTest()
        {

        }
    }
}
