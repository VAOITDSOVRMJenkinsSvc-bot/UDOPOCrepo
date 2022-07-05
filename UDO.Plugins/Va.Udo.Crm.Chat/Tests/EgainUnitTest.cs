using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VRM.Integration.Servicebus.Egain.Messages.Messages;
using VRM.Integration.Servicebus.Core;
using Microsoft.Xrm.Sdk.Client;
using CRM007.CRM.SDK.Core;
using System.Linq;
using VRM.Integration.Servicebus.Egain.CrmModel;
using System.Net;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace VRM.Integration.Servicebus.Egain.UnitTest
{
    [TestClass]
    public class EgainUnitTest
    {
       string _endPoint = "https://CRMDAC.CRMUD.dev.CRM.VRM.vba.VA.GOV:8085";
        //string _endPoint = "https://CRMDAC.CRMUD.dev.CRM.VRM.vba.VA.GOV:8082";
        //string _endPoint = "https://qacrmdac.np.crm.vrm.vba.va.gov:8082";
        //string _endPoint = "https://crmdac.xrm.va.gov:8085";

        private static OrganizationServiceContext GetOrganizationServiceContext(string org)
        {


            var parms = CrmConnectionConfiguration.Current.GetCrmConnectionParmsByName(org);

            if (parms == null)
            {
                throw new System.NullReferenceException(string.Concat("Could not find the OrgName ", org, ". Ensure the OrgName exists in the VIMT.config CrmConnectionParms section."));
            }

            //Connect to CRM
            var connection = CrmConnection.Connect(parms);
            connection.Authenticate();
            //msg.TargetOrganizationService = connection;
            OrganizationServiceProxy serviceProxy = connection;

            //Create a context - used for rest of orchestration
            serviceProxy.EnableProxyTypes(typeof(crme_chatcobrowsesessionlog).Assembly);
            var context = new OrganizationServiceContext(connection);
            return context;
        }

        #region UdoUnitTests

        [TestMethod]
        public void AuthChatRequest_VrmUdoDevAuthChatRequest()
        {
            //This VIMT request will create the record if it doesn't exist, or update the record if ChatSessionId does exist
            //This marks the chat record as complete
            //If VsoOrgId is specified, VIMT will set the SSN to an empty string on the chat session record
            AuthChatRequest request = new AuthChatRequest()
            {
                //CallAgentId = @"CRM\CrmTest",
                CallAgentId = @"dev03.vbarsecr@va.gov",
                Category = "Category",
                ChatSessionId = "998877665544332217",
                ChatSessionLog = "<html><body><h2>ChatSessionLog</h2></body></html>",
                Edipi = "",
                MessageId = "MessageId",
                OrgName = "UDODEV",
                ParticipantId = "",
                Resolution = "Resolution",
                Ssn = "796083300",
                VsoOrgId = ""
            };

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Remote);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_VrmUdoDevCRMLaunchUrlRequest()
        {
            //This VIMT request will create a record if it doesn't exist, or read a record if the ChatSessionId does exist.
            //The return is the CrmLaunchUrl field, which should have been updated by the Chat plugin for UDO to include at least the InteractionId
            CrmLaunchUrlRequest request = new CrmLaunchUrlRequest()
            {
                CallAgentId = @"vrmcloud\bdrake",
                //ChatSessionId = Guid.NewGuid().ToString().Substring(0, 20),
                ChatSessionId = "998877665544332211",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "UDODEV",
                ParticipantId = "",
                Ssn = "123456789",
                VsoOrgId = ""

            };

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Remote);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);

            Assert.IsTrue(response.CrmLaunchUrl.Contains("InteractionId"));
        }

        #endregion UdoUnitTests

        #region UnitTestsFromCRMe

        #region AuthChat


        private static AuthChatRequest GetDefaultAuthChatRequest()
        {
            AuthChatRequest request = new AuthChatRequest()
            {
                CallAgentId = @"vrmcloud\CRMtestuser1",
                Category = "Category",
                ChatSessionId = Guid.NewGuid().ToString().Substring(0, 20),
                ChatSessionLog = "<html><body><h2>ChatSessionLog</h2></body></html>",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "CRMEDEV",
                ParticipantId = "600043193",
                Resolution = "Resolution",
                Ssn = "796127587",
                VsoOrgId = null
            };

            return request;
        }

        [TestMethod]
        public void GetSystemUserId()
        {
            OrganizationServiceContext serviceContext = GetOrganizationServiceContext("CRMEDEV");


            Guid? systemUserId = Util.GetSystemUserId(serviceContext, @"vrmcloud\crmtestuser1");

            Assert.IsTrue(systemUserId != null, "SystemUserId null");

        }

        [TestMethod]
        public void GetSystemUserIdCaseCheck()
        {
            OrganizationServiceContext serviceContext = GetOrganizationServiceContext("CRMEDEV");


            Guid? systemUserId = Util.GetSystemUserId(serviceContext, @"Vrmcloud\Crmtestuser1");

            Assert.IsTrue(systemUserId != null, "SystemUserId null");

        }

        [TestMethod]
        public void AuthChatRequest_TestCrmRecordOwnershipCreate()
        {
            AuthChatRequest request = GetDefaultAuthChatRequest();

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);

            Assert.Inconclusive("Must Check Chat Log Record in CRM");
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_TestCrmRecordOwnershipCreate2()
        {
            CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();
            request.CallAgentId = "donald.hurley@va.gov";

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);

            Assert.Inconclusive("Must Check Chat Log Record in CRM");
        }
        [TestMethod]
        public void AuthChatRequest_TestCrmRecordOwnershipUpdate()
        {
            AuthChatRequest request = GetDefaultAuthChatRequest();

            //Create Record
            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            //Update Record with new user.
            request.CallAgentId = @"vrmcloud\crmtestuser2";
            response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);

            Assert.Inconclusive("Must Check Chat Log Record in CRM");
        }

        [TestMethod]
        public void AuthChatRequest_Processor()
        {

            AuthChatRequest request = GetDefaultAuthChatRequest();
            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);

        }

        #region ChatLog
        [TestMethod]
        public void AuthChatRequest_ProcessorLogEmpty()
        {

            AuthChatRequest request = GetDefaultAuthChatRequest();

            request.ChatSessionLog = "";

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);
            Assert.Inconclusive("You must check the CRM record for this test");
        }

        [TestMethod]
        public void AuthChatRequest_ProcessorLogNull()
        {

            AuthChatRequest request = GetDefaultAuthChatRequest();

            request.ChatSessionLog = null;

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);
            Assert.Inconclusive("You must check the CRM record for this test");

        }
        #endregion

        #region OrgName
        [TestMethod]
        public void AuthChatRequest_ProcessorOrgNameError()
        {

            AuthChatRequest request = new AuthChatRequest()
            {
                CallAgentId = "CallAgentId",
                Category = "Category",
                ChatSessionId = "ChatSessionId",
                ChatSessionLog = "<html><body><h2>ChatSessionLog</h2></body></html>",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "CRMEDEV_Error",
                ParticipantId = "ParticipantId",
                Resolution = "Resolution",
                Ssn = "Ssn",
                VsoOrgId = "VsoOrgId"
            };

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode != 0);

        }

        [TestMethod]
        public void AuthChatRequest_ProcessorOrgNameNull()
        {

            AuthChatRequest request = new AuthChatRequest()
            {
                CallAgentId = "CallAgentId",
                Category = "Category",
                ChatSessionId = "ChatSessionId",
                ChatSessionLog = "ChatSessionLog",
                Edipi = "Edipi",
                MessageId = "MessageId",
                //OrgName = "CRMEDEV",
                ParticipantId = "ParticipantId",
                Resolution = "Resolution",
                Ssn = "Ssn",
                VsoOrgId = "VsoOrgId"
            };

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode != 0);
            Assert.IsTrue(response.ErrorMessage.Contains("OrgName Not Specified"));
        }

        [TestMethod]
        public void AuthChatRequest_ProcessorOrgNameEmpty()
        {

            AuthChatRequest request = new AuthChatRequest()
            {
                CallAgentId = "CallAgentId",
                Category = "Category",
                ChatSessionId = "ChatSessionId",
                ChatSessionLog = "ChatSessionLog",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "",
                ParticipantId = "ParticipantId",
                Resolution = "Resolution",
                Ssn = "Ssn",
                VsoOrgId = "VsoOrgId"
            };

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode != 0);
            Assert.IsTrue(response.ErrorMessage.Contains("OrgName Not Specified"));
        }
        #endregion

        #region CallAgentId
        [TestMethod]
        public void AuthChatRequest_ProcessorCallAgentIdEmpty()
        {

            AuthChatRequest request = new AuthChatRequest()
            {
                CallAgentId = "",
                Category = "Category",
                ChatSessionId = "ChatSessionId",
                ChatSessionLog = "ChatSessionLog",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "CRMEDEV",
                ParticipantId = "ParticipantId",
                Resolution = "Resolution",
                Ssn = "Ssn",
                VsoOrgId = "VsoOrgId"
            };

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode != 0);
            Assert.IsTrue(response.ErrorMessage.Contains("CallAgentId Not Specified"));
        }

        [TestMethod]
        public void AuthChatRequest_ProcessorCallAgentIdNull()
        {

            AuthChatRequest request = new AuthChatRequest()
            {
                //CallAgentId = "CallAgentId",
                Category = "Category",
                ChatSessionId = "ChatSessionId",
                ChatSessionLog = "ChatSessionLog",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "CRMEDEV",
                ParticipantId = "ParticipantId",
                Resolution = "Resolution",
                Ssn = "Ssn",
                VsoOrgId = "VsoOrgId"
            };

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode != 0);
            Assert.IsTrue(response.ErrorMessage.Contains("CallAgentId Not Specified"));
        }

        [TestMethod]
        public void AuthChatRequest_ProcessorCallAgentIdInvalid()
        {

            AuthChatRequest request = GetDefaultAuthChatRequest();
            request.CallAgentId = "UserIdInvalid";
            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 1);
            Assert.IsTrue(response.ErrorMessage.Contains(string.Format("CallAgentId {0} not found", request.CallAgentId)));
        }

        #endregion

        #region ChatSessionId
        [TestMethod]
        public void AuthChatRequest_ProcessorChatSessionIdEmpty()
        {

            AuthChatRequest request = new AuthChatRequest()
            {
                CallAgentId = "CallAgentId",
                Category = "Category",
                ChatSessionId = "",
                ChatSessionLog = "ChatSessionLog",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "CRMEDEV",
                ParticipantId = "ParticipantId",
                Resolution = "Resolution",
                Ssn = "Ssn",
                VsoOrgId = "VsoOrgId"
            };

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode != 0);
            Assert.IsTrue(response.ErrorMessage.Contains("ChatSessionId Not Specified"));
        }

        [TestMethod]
        public void AuthChatRequest_ProcessorChatSessionIdNull()
        {

            AuthChatRequest request = new AuthChatRequest()
            {
                CallAgentId = "CallAgentId",
                Category = "Category",
                //ChatSessionId = "",
                ChatSessionLog = "ChatSessionLog",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "CRMEDEV",
                ParticipantId = "ParticipantId",
                Resolution = "Resolution",
                Ssn = "Ssn",
                VsoOrgId = "VsoOrgId"
            };

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode != 0);
            Assert.IsTrue(response.ErrorMessage.Contains("ChatSessionId Not Specified"));
        }
        #endregion

        #region ParticipantId
        [TestMethod]
        public void AuthChatRequest_MessageParticipantIdSet()
        {
            AuthChatRequest request = new AuthChatRequest();
            request.ParticipantId = "12345";

            Assert.IsTrue(request.ParticipantId.Equals("12345"));

        }

        [TestMethod]
        public void AuthChatRequest_StateParticipantIdSet()
        {
            AuthChatRequestState state = new AuthChatRequestState("agentId",
                "category",
                "sessionId",
                "sessionLog",
                "edipi",
                "orgname",
                "resolution",
                "ssn",
                "VsoOrgId",
                "12345");

            Assert.IsTrue(state.ParticipantId.Equals("12345"));
        }

        #region Edipi

        [TestMethod]
        public void AuthChatRequest_MessageEdipiSet()
        {
            AuthChatRequest request = new AuthChatRequest();
            request.Edipi = "12345";

            Assert.IsTrue(request.Edipi.Equals("12345"));

        }

        [TestMethod]
        public void AuthChatRequest_StateEdipiSet()
        {
            AuthChatRequestState state = new AuthChatRequestState("agentId",
                "category",
                "sessionId",
                "sessionLog",
                "edipi",
                "orgname",
                "resolution",
                "ssn",
                "VsoOrgId",
                "12345");

            Assert.IsTrue(state.Edipi.Equals("edipi"));
        }

        #endregion

        #region SSN
        #endregion

        #region Required Field Validation
        [TestMethod]
        public void AuthChatRequest_ProcessorRequiredIdentityFieldsNull()
        {

            AuthChatRequest request = new AuthChatRequest()
            {
                CallAgentId = "CallAgentId",
                Category = "Category",
                ChatSessionId = "ChatSessionId",
                ChatSessionLog = "ChatSessionLog",
                //Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "CRMEDEV",
                //ParticipantId = "ParticipantId",
                Resolution = "Resolution",
                //Ssn = "Ssn",
                //VsoOrgId = "VsoOrgId"
            };

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode != 0);
            Assert.IsTrue(response.ErrorMessage.Contains("ParticipantId/EDIPI/SSN/VSOOrgId Not Specified"));
        }

        [TestMethod]
        public void AuthChatRequest_ProcessorRequiredIdentityFieldsEmpty()
        {

            AuthChatRequest request = new AuthChatRequest()
            {
                CallAgentId = "CallAgentId",
                Category = "Category",
                ChatSessionId = "ChatSessionId",
                ChatSessionLog = "ChatSessionLog",
                Edipi = "",
                MessageId = "MessageId",
                OrgName = "VRMUDODEV",
                ParticipantId = "",
                Resolution = "Resolution",
                Ssn = "",
                VsoOrgId = ""
            };

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode != 0);
            Assert.IsTrue(response.ErrorMessage.Contains("ParticipantId/EDIPI/SSN/VSOOrgId Not Specified"));

        }


        [TestCategory("AuthChat"), TestMethod]
        public void AuthChatRequest_ProcessorParticipanEmpty()
        {

            AuthChatRequest request = GetDefaultAuthChatRequest();
            request.ParticipantId = "";

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);
            //Assert.IsTrue(response.ErrorMessage.Equals("ParticipantId/EDIPI/SSN Not Specified"));

        }

        [TestMethod]
        public void AuthChatRequest_ProcessorRequiredIdentityEdipiSpecified()
        {
            //Edipi Specified
            AuthChatRequest request = GetDefaultAuthChatRequest();
            request.Edipi = "Edipi";

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);

        }

        [TestMethod]
        public void AuthChatRequest_ProcessorRequiredIdentityParticipantIdSpecified()
        {
            AuthChatRequest request = GetDefaultAuthChatRequest();

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);

        }

        [TestMethod]
        public void AuthChatRequest_ProcessorRequiredIdentitySSNSpecified()
        {
            AuthChatRequest request = GetDefaultAuthChatRequest();

            AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);

        }


        #endregion
        #endregion

        #endregion

        #region CRMUrlRequest

        private static CrmLaunchUrlRequest GetDefaultCrmLaunchUrlRequest()
        {
            CrmLaunchUrlRequest request = new CrmLaunchUrlRequest()
            {
                ChatSessionId = "Dons TEst", //Guid.NewGuid().ToString().Substring(0, 20), //"ChatSessionId",
                //ChatSessionId = Guid.NewGuid().ToString().Substring(0, 20), //"ChatSessionId",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "CRMEDEV",
                ParticipantId = "600043193",
                Ssn = "796127587",
                VsoOrgId = "",
                CallAgentId = @"vrmcloud\crmtestuser1",

            };

            return request;

        }

        [TestMethod]
        public void CRMLaunchUrlRequest_TestCrmRecordOwnershipCreate()
        {
            CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);

            Assert.Inconclusive("Must Check Chat Log Record in CRM");
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_TestCrmRecordOwnershipUpdate()
        {
            CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            request.CallAgentId = @"vrmcloud\crmtestuser2";
            response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);

            Assert.Inconclusive("Must Check Chat Log Record in CRM");
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_TestCrmRecordOwnershipInvalidUser()
        {
            CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();
            request.CallAgentId = "BadUser";
            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 1);
            Assert.IsTrue(response.ErrorMessage.Contains(string.Format("CallAgentId {0} not found", request.CallAgentId)));


            //Assert.Inconclusive("Must Check Chat Log Record in CRM");
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_Processor()
        {

            CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsFalse(string.IsNullOrEmpty(response.CrmLaunchUrl));
            Assert.IsTrue(response.ErrorCode == 0);
        }



        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorVsoOrgIdEmpty()
        {
            CrmLaunchUrlRequest request = new CrmLaunchUrlRequest()
            {
                VsoOrgId = "VsoOrgId"
            };

            Assert.IsTrue(request.VsoOrgId.Equals("VsoOrgId"), "VsoOrgId not equal");
           
        }
        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorVsoOrgIdNull()
        {

            CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();


            request.VsoOrgId = null;

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorVsoOrgEmpty()
        {

            CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();


            request.VsoOrgId = "";

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);
        }


        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorCallAgentIdSet()
        {
            CrmLaunchUrlRequest request = new CrmLaunchUrlRequest()
            {
                CallAgentId = "CallAgentId"
            };

            Assert.IsTrue(request.CallAgentId.Equals("CallAgentId"), "CallAgentId not equal");

        }


        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorParticipantIdNull()
        {

            CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();


            request.ParticipantId = null;

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorParticipantIdEmpty()
        {

            CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();
            request.ParticipantId = "";

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);
        }


        [TestMethod] 
        public void CRMLaunchUrlRequest_ProcessorSsnNull()
        {
            CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();
 

            request.Ssn = null;

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorSsnEmpty()
        {

            CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();


            request.Ssn = "";

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorEdipiNull()
        {

            CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();

            request.Edipi = null;

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorEdipiEmpty()
        {
            
            CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();

            request.Edipi = "";

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 0);
        }


        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorRequiredIdentityFieldsEmpty()
        {

            CrmLaunchUrlRequest request = new CrmLaunchUrlRequest()
            {
                ChatSessionId = "ChatSessionId",
                Edipi = "",
                MessageId = "MessageId",
                OrgName = "CRMEDEV",
                ParticipantId = "",
                Ssn = "",
                CallAgentId = "CallAgentId",
                VsoOrgId = ""


            };

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 1);
            Assert.IsTrue(response.ErrorMessage.Contains("ParticipantId/EDIPI/SSN/VsoOrgId Not Specified"));
        }


        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorRequiredIdentityFieldsNull()
        {

            CrmLaunchUrlRequest request = new CrmLaunchUrlRequest()
            {
                ChatSessionId = "ChatSessionId",
                MessageId = "MessageId",
                OrgName = "CRMEDEV",
                CallAgentId = "CallAgentId",
                //VsoOrgId = "VsoOrgId"

            };

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 1);
            Assert.IsTrue(response.ErrorMessage.Contains("ParticipantId/EDIPI/SSN/VsoOrgId Not Specified"));
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorChatSessionIdEmpty()
        {

            CrmLaunchUrlRequest request = new CrmLaunchUrlRequest()
            {
                ChatSessionId = "",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "CRMEDEV",
                ParticipantId = "ParticipantId",
                Ssn = "Ssn",

            };

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 1);
            Assert.IsTrue(response.ErrorMessage.Contains("ChatSessionId Not Specified"));
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorChatSessionIdNull()
        {

            CrmLaunchUrlRequest request = new CrmLaunchUrlRequest()
            {
                //ChatSessionId = "",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "CRMEDEV",
                ParticipantId = "ParticipantId",
                Ssn = "Ssn",

            };

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 1);
            Assert.IsTrue(response.ErrorMessage.Contains("ChatSessionId Not Specified"));
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorCallAgentIdEmpty()
        {

            CrmLaunchUrlRequest request = new CrmLaunchUrlRequest()
            {
                ChatSessionId = "ChatSessionId",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "CRMEDEV",
                ParticipantId = "ParticipantId",
                Ssn = "Ssn",
                CallAgentId = ""

            };

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 1);
            Assert.IsTrue(response.ErrorMessage.Contains("CallAgentId Not Specified"));
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorCallAgentIdInvalid()
        {

            CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();
            request.CallAgentId = "UserIdInvalid";
            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 1);
            Assert.IsTrue(response.ErrorMessage.Contains(string.Format("CallAgentId {0} not found", request.CallAgentId)));
        }


        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorCallAgentIdNull()
        {

            CrmLaunchUrlRequest request = new CrmLaunchUrlRequest()
            {
                ChatSessionId = "ChatSessionId",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "CRMEDEV",
                ParticipantId = "ParticipantId",
                Ssn = "Ssn",
                //CallAgentId = ""

            };

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 1);
            Assert.IsTrue(response.ErrorMessage.Contains("CallAgentId Not Specified"));
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorOrgNameError()
        {

            CrmLaunchUrlRequest request = new CrmLaunchUrlRequest()
            {
                ChatSessionId = "ChatSessionId",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "CRMEDEV_Error",
                ParticipantId = "ParticipantId",
                Ssn = "Ssn",

            };

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 1);
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorOrgNameEmpty()
        {

            CrmLaunchUrlRequest request = new CrmLaunchUrlRequest()
            {
                ChatSessionId = "ChatSessionId",
                Edipi = "Edipi",
                MessageId = "MessageId",
                OrgName = "",
                ParticipantId = "ParticipantId",
                Ssn = "Ssn",

            };

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 1);
            Assert.IsTrue(response.ErrorMessage.Contains("OrgName Not Specified"));
        }

        [TestMethod]
        public void CRMLaunchUrlRequest_ProcessorOrgNameNull()
        {

            CrmLaunchUrlRequest request = new CrmLaunchUrlRequest()
            {
                ChatSessionId = "ChatSessionId",
                Edipi = "Edipi",
                MessageId = "MessageId",
                //OrgName = "",
                ParticipantId = "ParticipantId",
                Ssn = "Ssn",

            };

            CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.ErrorCode == 1);
            Assert.IsTrue(response.ErrorMessage.Contains("OrgName Not Specified"));
        }
        #endregion

        #region Util.GetSensitivityLevel
        //[TestMethod]
        //public void GetSensitivityLevelBySsn()
        //{
        //    string crme_OrganizationName = "CRMEDEV";
        //    Guid crme_UserId = Guid.Parse("{38045b35-d411-e411-ae3f-00155d14d952}");

        //    string sensitivityLevel = Util.GetSensitivityLevel(crme_OrganizationName, crme_UserId, "796127587", null);

        //    Assert.IsTrue(sensitivityLevel.Equals("0"));
        //}

        //[TestMethod]
        ////[ExpectedException(typeof(Exception))]
        //public void GetSensitivityLevelBySsnInvalid()
        //{
        //    string crme_OrganizationName = "CRMEDEV";
        //    Guid crme_UserId = Guid.Parse("{38045b35-d411-e411-ae3f-00155d14d952}");

        //    string sensitivityLevel = Util.GetSensitivityLevel(crme_OrganizationName, crme_UserId, "00000000", null);
        //    Assert.IsTrue(string.IsNullOrEmpty(sensitivityLevel));

        //}

        //[TestMethod]
        //public void GetSensitivityLevelByParticipantId()
        //{
        //    string crme_OrganizationName = "CRMEDEV";
        //    Guid crme_UserId = Guid.Parse("{38045b35-d411-e411-ae3f-00155d14d952}");

        //    string sensitivityLevel = Util.GetSensitivityLevel(crme_OrganizationName, crme_UserId, null, 600043193L);

        //    Assert.IsTrue(sensitivityLevel.Equals("0"));
        //}

        //[TestMethod]
        ////[ExpectedException(typeof(Exception))]
        //public void GetSensitivityLevelByParticipantIdInvalid()
        //{
        //    string crme_OrganizationName = "CRMEDEV";
        //    Guid crme_UserId = Guid.Parse("{38045b35-d411-e411-ae3f-00155d14d952}");

        //    string sensitivityLevel = Util.GetSensitivityLevel(crme_OrganizationName, crme_UserId, null, long.MaxValue);

        //    Assert.IsTrue(string.IsNullOrEmpty(sensitivityLevel));

        //}

        //[TestMethod]
        ////[ExpectedException(typeof(Exception))]
        //public void GetSensitivityLevelSsnAmdParticipantIdNull()
        //{
        //    string crme_OrganizationName = "CRMEDEV";
        //    Guid crme_UserId = Guid.Parse("{38045b35-d411-e411-ae3f-00155d14d952}");

        //    string sensitivityLevel = Util.GetSensitivityLevel(crme_OrganizationName, crme_UserId, null, null);
        //    Assert.IsTrue(string.IsNullOrEmpty(sensitivityLevel));
        //    //Assert.IsTrue(sensitivityLevel.Equals(Util.HIGHEST_SENSITIVITY));

        //}


        //[TestMethod]
        //[ExpectedException(typeof(AccessViolationException))]
        //public void GetSensitivityLevelSsnHighSensitivity()
        //{
        //    string crme_OrganizationName = "CRMEDEV";
        //    Guid crme_UserId = Guid.Parse("{38045b35-d411-e411-ae3f-00155d14d952}");

        //    string sensitivityLevel = Util.GetSensitivityLevel(crme_OrganizationName, crme_UserId, "796380117", null);

        //}

        //[TestMethod]
        //[ExpectedException(typeof(Exception))]
        //public void GetSensitivityLevelForceError()
        //{
        //    string crme_OrganizationName = "CRMEDEV";
        //    Guid crme_UserId = Guid.Parse("{38045b35-d411-e411-ae3f-00155d14d952}");

        //    string sensitivityLevel = Util.GetSensitivityLevel(crme_OrganizationName, crme_UserId, "0", null);

        //}

        //[TestMethod]
        //public void CrmLaunchGetSensitivityLevel_NoVetIdPassed()
        //{
        //    CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();
        //    request.VsoOrgId = null;
        //    request.Ssn = null;
        //    request.ParticipantId = null;

        //    CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);



        //    var context = GetOrganizationServiceContext(request.OrgName);



        //    crme_chatcobrowsesessionlog actual = context.CreateQuery<crme_chatcobrowsesessionlog>().FirstOrDefault<crme_chatcobrowsesessionlog>(a => a.crme_ChatSessionId.Equals(request.ChatSessionId));
        //    Assert.IsTrue(actual.crme_SensitivityReason.Value == Util.SENSITIVITYREASON_NOTFOUND);
        //    Assert.IsTrue(actual.crme_VeteranSensitivityLevel.Equals(Util.LOWEST_SENSITIVITY));
        //}

        //[TestMethod]
        //public void CrmLaunchGetSensitivityLevel_AccessViolation()
        //{
        //    CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();
        //    request.VsoOrgId = null;
        //    request.Ssn = "796380117";
        //    request.ParticipantId = null;

        //    CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

        //    Assert.IsTrue(response.ErrorCode == 1);
        //    Assert.IsTrue(response.ErrorMessage.Contains("Sensitive File - Access Violation"));

        //}

        //[TestMethod]
        //public void CrmLaunchGetSensitivityLevel_BGSException()
        //{
        //    CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();
        //    request.VsoOrgId = null;
        //    request.Ssn = "00";
        //    request.ParticipantId = null;

        //    CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

        //    var context = GetOrganizationServiceContext(request.OrgName);

        //    crme_chatcobrowsesessionlog actual = context.CreateQuery<crme_chatcobrowsesessionlog>().FirstOrDefault<crme_chatcobrowsesessionlog>(a => a.crme_ChatSessionId.Equals(request.ChatSessionId));
        //    Assert.IsTrue(actual.crme_SensitivityReason.Value == Util.SENSITIVITYREASON_BGSEXCEPTION);
        //    Assert.IsTrue(actual.crme_VeteranSensitivityLevel.Equals(Util.LOWEST_SENSITIVITY));
        //}

        //[TestMethod]
        //public void CrmLaunchGetSensitivityLevel_CouldDetermine()
        //{
        //    CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();

        //    CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);

        //    var context = GetOrganizationServiceContext(request.OrgName);

        //    crme_chatcobrowsesessionlog actual = context.CreateQuery<crme_chatcobrowsesessionlog>().FirstOrDefault<crme_chatcobrowsesessionlog>(a => a.crme_ChatSessionId.Equals(request.ChatSessionId));
        //    Assert.IsTrue(actual.crme_SensitivityReason.Value == Util.SENSITIVITYREASON_STATUSFOUND);
        //    Assert.IsTrue(actual.crme_VeteranSensitivityLevel.Equals("0"));
        //}

        //[TestMethod]
        //public void AuthChatGetSensitivityNoVetId()
        //{

        //    AuthChatRequest request = GetDefaultAuthChatRequest();
        //    request.Ssn = null;
        //    request.ParticipantId = null;


        //    AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

        //    Assert.IsTrue(response.ErrorCode == 0);
        //    var context = GetOrganizationServiceContext(request.OrgName);

        //    crme_chatcobrowsesessionlog actual = context.CreateQuery<crme_chatcobrowsesessionlog>().FirstOrDefault<crme_chatcobrowsesessionlog>(a => a.crme_ChatSessionId.Equals(request.ChatSessionId));
        //    Assert.IsTrue(actual.crme_SensitivityReason.Value == Util.SENSITIVITYREASON_NOTFOUND);
        //    Assert.IsTrue(actual.crme_VeteranSensitivityLevel.Equals(Util.HIGHEST_SENSITIVITY));

        //    }


        //[TestMethod]
        //public void AuthChatGetSensitivity_AccessValidation()
        //{

        //    AuthChatRequest request = GetDefaultAuthChatRequest();
        //    request.Ssn = "796380117";
        //    request.ParticipantId = null;


        //    AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

        //    Assert.IsTrue(response.ErrorCode == 0);
        //    var context = GetOrganizationServiceContext(request.OrgName);

        //    crme_chatcobrowsesessionlog actual = context.CreateQuery<crme_chatcobrowsesessionlog>().FirstOrDefault<crme_chatcobrowsesessionlog>(a => a.crme_ChatSessionId.Equals(request.ChatSessionId));
        //    Assert.IsTrue(actual.crme_SensitivityReason.Value == Util.SENSITIVITYREASON_ACCESSVIOLATION);
        //    Assert.IsTrue(actual.crme_VeteranSensitivityLevel.Equals(Util.HIGHEST_SENSITIVITY));

        //}


        //[TestMethod]
        //public void AuthChatGetSensitivity_BGSExeption()
        //{

        //    AuthChatRequest request = GetDefaultAuthChatRequest();
        //    request.Ssn = "00";
        //    request.ParticipantId = null;


        //    AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

        //    Assert.IsTrue(response.ErrorCode == 0);
        //    var context = GetOrganizationServiceContext(request.OrgName);

        //    crme_chatcobrowsesessionlog actual = context.CreateQuery<crme_chatcobrowsesessionlog>().FirstOrDefault<crme_chatcobrowsesessionlog>(a => a.crme_ChatSessionId.Equals(request.ChatSessionId));
        //    Assert.IsTrue(actual.crme_SensitivityReason.Value == Util.SENSITIVITYREASON_BGSEXCEPTION);
        //    Assert.IsTrue(actual.crme_VeteranSensitivityLevel.Equals(Util.HIGHEST_SENSITIVITY));

        //}

        //[TestMethod]
        //public void AuthChatGetSensitivity_CouldDetermine()
        //{

        //    AuthChatRequest request = GetDefaultAuthChatRequest();

        //    AuthChatResponse response = request.SendReceive<AuthChatResponse>(MessageProcessType.Local);

        //    Assert.IsTrue(response.ErrorCode == 0);
        //    var context = GetOrganizationServiceContext(request.OrgName);

        //    crme_chatcobrowsesessionlog actual = context.CreateQuery<crme_chatcobrowsesessionlog>().FirstOrDefault<crme_chatcobrowsesessionlog>(a => a.crme_ChatSessionId.Equals(request.ChatSessionId));
        //    Assert.IsTrue(actual.crme_SensitivityReason.Value == Util.SENSITIVITYREASON_STATUSFOUND);
        //    Assert.IsTrue(actual.crme_VeteranSensitivityLevel.Equals("0"));

        //}

        //[TestMethod]
        //public void SensitivityDetermineTransitionToNotDeterined()
        //{

        //    CrmLaunchUrlRequest request = GetDefaultCrmLaunchUrlRequest();
        //    request.Ssn = "796131753";

        //    CrmLaunchUrlResponse response = request.SendReceive<CrmLaunchUrlResponse>(MessageProcessType.Local);


        //    //Don't supply any identifiers so that we can ensure the sensitivity stays the same.
        //    AuthChatRequest aRequest = new AuthChatRequest();
        //    aRequest.OrgName = request.OrgName;
        //    aRequest.ChatSessionId = request.ChatSessionId;
        //    aRequest.VsoOrgId = "test";
        //    aRequest.CallAgentId = request.CallAgentId;

        //    AuthChatResponse aResponse = aRequest.SendReceive<AuthChatResponse>(MessageProcessType.Local);

        //    Assert.IsTrue(aResponse.ErrorCode == 0);
        //    var context = GetOrganizationServiceContext(request.OrgName);
        //    crme_chatcobrowsesessionlog actual = context.CreateQuery<crme_chatcobrowsesessionlog>().FirstOrDefault<crme_chatcobrowsesessionlog>(a => a.crme_ChatSessionId.Equals(request.ChatSessionId));
        //    Assert.IsTrue(actual.crme_SensitivityReason.Value == Util.SENSITIVITYREASON_NOTFOUND);
        //    Assert.IsTrue(actual.crme_VeteranSensitivityLevel.Equals("6"));





        //}

        #endregion

        #region Hide Unused methods
        //[TestMethod]
        //public void SendReceive1()
        //{
        //    //Uri uri = new Uri("https://crmdac.xrm.va.gov:8085/Servicebus/Rest/SendReceive?messageId=Egain%23CrmLaunchUrlRequest&messageType=text%2Fjson&message=eyJDaGF0U2Vzc2lvbklkIjoiY2hhdHNlc3Npb25pZCIsIkVkaXBpIjoiZWRpcGkiLCJPcmdOYW1lIjoiQ1JNRURFViIsIlBhcnRpY2lwYW50SWQiOiJQYXJ0aWNpcGFudElkIiwiU3NuIjoiU3NuIiwiQ2FsbEFnZW50SWQiOiJkaHVybGV5QHZhLmdvdiIsIlZzb09yZ0lkIjoiVnNvT3JnSWQiLCJNZXNzYWdlSWQiOiI4MWFlNmNmYy1hYzU0LWMxMWMtMTA4MC0yYzYxNzI4YzgyYmUifQ%3D%3D&isQueued=false&toJSONString=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.stringify(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D&parseJSON=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.parse(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D");
        //    //Uri uri = new Uri("http://localhost:8085/Servicebus/Rest/SendReceive?messageId=Egain%23CrmLaunchUrlRequest&messageType=text%2Fjson&message=eyJDaGF0U2Vzc2lvbklkIjoiY2hhdHNlc3Npb25pZCIsIkVkaXBpIjoiZWRpcGkiLCJPcmdOYW1lIjoiQ1JNRURFViIsIlBhcnRpY2lwYW50SWQiOiJQYXJ0aWNpcGFudElkIiwiU3NuIjoiU3NuIiwiQ2FsbEFnZW50SWQiOiJkaHVybGV5QHZhLmdvdiIsIlZzb09yZ0lkIjoiVnNvT3JnSWQiLCJNZXNzYWdlSWQiOiI4MWFlNmNmYy1hYzU0LWMxMWMtMTA4MC0yYzYxNzI4YzgyYmUifQ%3D%3D&isQueued=false&toJSONString=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.stringify(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D&parseJSON=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.parse(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D");

        //    Uri uri = new Uri("http://localhost:8085/Servicebus/Rest/SendReceive?messageId=Egain%23CrmLaunchUrlRequest&messageType=text%2Fjson&message=eyJDaGF0U2Vzc2lvbklkIjoiMjAxNTAyMTAxNDM0IiwiRWRpcGkiOiIiLCJPcmdOYW1lIjoiQ1JNRURFViIsIlBhcnRpY2lwYW50SWQiOiIiLCJTc24iOiI3OTYxMjc1ODciLCJDYWxsQWdlbnRJZCI6ImRodXJsZXlAdmEuZ292IiwiVnNvT3JnSWQiOiIiLCJNZXNzYWdlSWQiOiI3YjlhZDQ1Yi1kOTMwLTQ2YmYtNGY1Yy0wYzkxYjA1NzM3NTcifQ%3D%3D&isQueued=false&toJSONString=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.stringify(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D&parseJSON=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.parse(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D");

        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        //    request.ContentType = "application/json";
        //    request.Method = "POST";
        //    request.ContentLength = 0;


        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //    string responseValue = null;

        //    using (var responseStream = response.GetResponseStream())
        //    {
        //        if (responseStream != null)
        //        {
        //            using (var reader = new StreamReader(responseStream))
        //            {
        //                //xmlDoc.Load(reader);
        //                responseValue = reader.ReadToEnd();
        //            }
        //        }
        //    }


        //    int start = responseValue.IndexOf("<Message>") + 9;
        //    int end = responseValue.IndexOf("</Message>");


        //    string message = responseValue.Substring(start, end - start);

        //    byte[] b = Convert.FromBase64String(message);
        //    UTF8Encoding enc = new UTF8Encoding();
        //    string mess = enc.GetString(b);



        //}

        //[TestMethod]
        //public void SendReceiveCrmLaunchUrl()
        //{
        //    //Uri uri = new Uri("https://crmdac.xrm.va.gov:8085/Servicebus/Rest/SendReceive?messageId=Egain%23CrmLaunchUrlRequest&messageType=text%2Fjson&message=eyJDaGF0U2Vzc2lvbklkIjoiY2hhdHNlc3Npb25pZCIsIkVkaXBpIjoiZWRpcGkiLCJPcmdOYW1lIjoiQ1JNRURFViIsIlBhcnRpY2lwYW50SWQiOiJQYXJ0aWNpcGFudElkIiwiU3NuIjoiU3NuIiwiQ2FsbEFnZW50SWQiOiJkaHVybGV5QHZhLmdvdiIsIlZzb09yZ0lkIjoiVnNvT3JnSWQiLCJNZXNzYWdlSWQiOiI4MWFlNmNmYy1hYzU0LWMxMWMtMTA4MC0yYzYxNzI4YzgyYmUifQ%3D%3D&isQueued=false&toJSONString=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.stringify(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D&parseJSON=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.parse(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D");
        //    Uri uri = new Uri("http://localhost:8085/Servicebus/Rest/SendReceive?messageId=Egain%23CrmLaunchUrlRequest&messageType=text%2Fjson&isQueued=false&toJSONString=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.stringify(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D&parseJSON=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.parse(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D");

            
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        //    request.ContentType = "application/json";
        //    request.Method = "POST";

        //    CrmLaunchUrlRequest crmLaunchUrlRequest = new CrmLaunchUrlRequest() { CallAgentId = @"vrmcloud\crmtestuser1", OrgName = "CRMEDEV", ChatSessionId = Guid.NewGuid().ToString().Substring(0, 20), Ssn = "796127587" };

        //    using (Stream reqStream = request.GetRequestStream())
        //    {
        //        SerializeObjectToStream(crmLaunchUrlRequest, reqStream);
        //    }
            
          
           

        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    string responseValue = null;

        //    using (var responseStream = response.GetResponseStream())
        //    {
        //        if (responseStream != null)
        //        {
        //            using (var reader = new StreamReader(responseStream))
        //            {
        //                //xmlDoc.Load(reader);
        //                responseValue = reader.ReadToEnd();
        //            }
        //        }
        //    }


        //    int start = responseValue.IndexOf("<Message>") + 9;
        //    int end = responseValue.IndexOf("</Message>");


        //    string message = responseValue.Substring(start, end - start);

        //    byte[] b = Convert.FromBase64String(message);
        //    UTF8Encoding enc = new UTF8Encoding();
        //    string mess = enc.GetString(b);



        //}

        //[TestMethod]
        //public async System.Threading.Tasks.Task SendReceiveCrmLaunchUrl2()
        //{
        //    //Uri uri = new Uri("https://crmdac.xrm.va.gov:8085/Servicebus/Rest/SendReceive?messageId=Egain%23CrmLaunchUrlRequest&messageType=text%2Fjson&message=eyJDaGF0U2Vzc2lvbklkIjoiY2hhdHNlc3Npb25pZCIsIkVkaXBpIjoiZWRpcGkiLCJPcmdOYW1lIjoiQ1JNRURFViIsIlBhcnRpY2lwYW50SWQiOiJQYXJ0aWNpcGFudElkIiwiU3NuIjoiU3NuIiwiQ2FsbEFnZW50SWQiOiJkaHVybGV5QHZhLmdvdiIsIlZzb09yZ0lkIjoiVnNvT3JnSWQiLCJNZXNzYWdlSWQiOiI4MWFlNmNmYy1hYzU0LWMxMWMtMTA4MC0yYzYxNzI4YzgyYmUifQ%3D%3D&isQueued=false&toJSONString=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.stringify(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D&parseJSON=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.parse(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D");
        //    Uri uri = new Uri("http://localhost:8085/Servicebus/Rest/SendReceive?messageId=Egain%23CrmLaunchUrlRequest&messageType=text%2Fjson&isQueued=false&toJSONString=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.stringify(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D&parseJSON=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.parse(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D");

        //    CrmLaunchUrlRequest crmLaunchUrlRequest = new CrmLaunchUrlRequest() { CallAgentId = @"vrmcloud\crmtestuser1", OrgName = "CRMEDEV", ChatSessionId = Guid.NewGuid().ToString().Substring(0, 20), Ssn = "796127587" };



        //    string message = await CallRestEndPoint(uri, crmLaunchUrlRequest);

        //    CrmLaunchUrlResponse crmLaunchResponse = new CrmLaunchUrlResponse();

        //    crmLaunchResponse = DeserializeResponse<CrmLaunchUrlResponse>(message);

        //    Assert.IsTrue(crmLaunchResponse.ErrorCode == 0);

        //}
        #endregion Unuse

        #region MessagePayloadTests
        [TestMethod]
        public async System.Threading.Tasks.Task SendReceiveAuthChatRequest5MB()
        {
            Uri uri = new Uri(_endPoint + "/Servicebus/Rest/SendReceive?messageId=Egain%23AuthChatRequest&messageType=text%2Fjson&isQueued=false");
            //Uri uri = new Uri("https://CRMDAC.CRMUD.dev.CRM.VRM.vba.VA.GOV:8085/Servicebus/Rest/SendReceive?messageId=Egain%23AuthChatRequest&messageType=text%2Fjson&isQueued=false");
        //https://CRMDAC.CRMUD.dev.CRM.VRM.vba.VA.GOV:8085/Servicebus
            AuthChatRequest authChatRequest = new AuthChatRequest() { CallAgentId = @"scott.orr3@va.gov", OrgName = "CRMEDEV", ChatSessionId = Guid.NewGuid().ToString().Substring(0, 20), Ssn = "796127587" };
            //AuthChatRequest authChatRequest = new AuthChatRequest() { CallAgentId = @"scott.orr3@va.gov", OrgName = "INTVRM", ChatSessionId = Guid.NewGuid().ToString().Substring(0, 20), Ssn = "438769663" };

            //AuthChatRequest authChatRequest = new AuthChatRequest() { CallAgentId = @"scott.orr3@va.gov", OrgName = "VRM", ChatSessionId = Guid.NewGuid().ToString().Substring(0, 20), Ssn = "242239702" };
            int messageSize = 5000000;

            authChatRequest.ChatSessionLog = BuildLogMessage(messageSize).ToString();


            string message = await CallRestEndPoint(uri, authChatRequest);

            AuthChatResponse authChatResponse = new AuthChatResponse();

            authChatResponse = DeserializeResponse<AuthChatResponse>(message);

            Assert.IsTrue(authChatResponse.ErrorCode == 0);

        }

        [TestMethod]
        public async System.Threading.Tasks.Task SendReceiveAuthChatRequest6MB()
        {
            //Uri uri = new Uri("http://localhost:8085/Servicebus/Rest/SendReceive?messageId=Egain%23AuthChatRequest&messageType=text%2Fjson&isQueued=false");
            Uri uri = new Uri(_endPoint + "/Servicebus/Rest/SendReceive?messageId=Egain%23AuthChatRequest&messageType=text%2Fjson&isQueued=false");
 
            AuthChatRequest authChatRequest = new AuthChatRequest() { CallAgentId = @"scott.orr3@va.gov", OrgName = "INTVRM", ChatSessionId = Guid.NewGuid().ToString().Substring(0, 20), Ssn = "796127587" };

            int messageSize = 6000000;

            authChatRequest.ChatSessionLog = BuildLogMessage(messageSize).ToString();


            string message = await CallRestEndPoint(uri, authChatRequest);

            AuthChatResponse authChatResponse = new AuthChatResponse();

            authChatResponse = DeserializeResponse<AuthChatResponse>(message);

            Assert.IsTrue(authChatResponse.ErrorCode == 1);

        }
        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async System.Threading.Tasks.Task SendReceiveAuthChatRequestInvalidMessageId()
        {
            Uri uri = new Uri(_endPoint + "/Servicebus/Rest/SendReceive?messageId=InvalidMessageId&messageType=text%2Fjson&isQueued=false");

            AuthChatRequest authChatRequest = new AuthChatRequest();


            string message = await CallRestEndPoint(uri, authChatRequest);

            AuthChatResponse authChatResponse = new AuthChatResponse();

            authChatResponse = DeserializeResponse<AuthChatResponse>(message);

            Assert.IsTrue(authChatResponse.ErrorCode == 1);

        }

        [TestMethod]
        public async System.Threading.Tasks.Task SendOneWayPass6MB()
        {
            Uri uri = new Uri(_endPoint + "/Servicebus/Rest/Send?messageId=TestMessages%23OneWayPassTest&messageType=text%2Fjson&isQueued=false");

            OneWayPassTest oneWayPassTest = new OneWayPassTest() { FirstName = "testfirst", Ssn = "123456789" };

            int messageSize = 6000000;

            oneWayPassTest.LastName = BuildLogMessage(messageSize).ToString();


            string message = await CallRestEndPoint(uri, oneWayPassTest);

            //AuthChatResponse authChatResponse = new AuthChatResponse();

            //authChatResponse = DeserializeResponse<AuthChatResponse>(message);

            //Assert.IsTrue(authChatResponse.ErrorCode == 1);

        }


        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async System.Threading.Tasks.Task SendReceiveAuthChatRequestInvalidMessageType()
        {
            Uri uri = new Uri(_endPoint + "/Servicebus/Rest/SendReceive?messageId=Egain%23AuthChatRequest&messageType=text%2Fdon&isQueued=false");

            AuthChatRequest authChatRequest = new AuthChatRequest() { CallAgentId = @"vrmcloud\crmtestuser1", OrgName = "CRMEDEV", ChatSessionId = Guid.NewGuid().ToString().Substring(0, 20), Ssn = "796127587" };


            authChatRequest.ChatSessionLog = "Test";


            string message = await CallRestEndPoint(uri, authChatRequest);

            AuthChatResponse authChatResponse = new AuthChatResponse();

            authChatResponse = DeserializeResponse<AuthChatResponse>(message);

            Assert.IsTrue(authChatResponse.ErrorCode == 1);

        }

        [TestMethod]
        public async System.Threading.Tasks.Task SendReceiveAuthChatRequestMissingParameter()
        {
            Uri uri = new Uri(_endPoint + "/Servicebus/Rest/SendReceive?messageId=Egain%23AuthChatRequest&messageType=text%2Fjson");

            AuthChatRequest authChatRequest = new AuthChatRequest() { CallAgentId = @"vrmcloud\crmtestuser1", OrgName = "CRMEDEV", ChatSessionId = Guid.NewGuid().ToString().Substring(0, 20), Ssn = "796127587" };


            authChatRequest.ChatSessionLog = "Test";


            string message = await CallRestEndPoint(uri, authChatRequest);

            AuthChatResponse authChatResponse = new AuthChatResponse();

            authChatResponse = DeserializeResponse<AuthChatResponse>(message);

            Assert.IsTrue(authChatResponse.ErrorCode == 1);

        }

        //[TestMethod]
        //public async System.Threading.Tasks.Task SendOneWayPassInvalidMessageId()
        //{
        //    Uri uri = new Uri(_endPoint + "/Servicebus/Rest/Send?messageId=InvalidMessageId&messageType=text%2Fjson&isQueued=false");

        //    OneWayPassTest oneWayPassTest = new OneWayPassTest() { FirstName = "testfirst", Ssn = "123456789" };

        //    int messageSize = 6000000;

        //    oneWayPassTest.LastName = BuildLogMessage(messageSize).ToString();


        //    string message = await CallRestEndPoint(uri, oneWayPassTest);

        //    //AuthChatResponse authChatResponse = new AuthChatResponse();

        //    //authChatResponse = DeserializeResponse<AuthChatResponse>(message);

        //    //Assert.IsTrue(authChatResponse.ErrorCode == 1);

        //}

        [TestMethod]
        public async System.Threading.Tasks.Task TestCompleteRecordAlreadyComplete()
        {
            Uri uri = new Uri(_endPoint + "/Servicebus/Rest/SendReceive?messageId=Egain%23AuthChatRequest&messageType=text%2Fjson&isQueued=false");
            //Uri uri = new Uri("https://CRMDAC.CRMUD.dev.CRM.VRM.vba.VA.GOV:8085/Servicebus/Rest/SendReceive?messageId=Egain%23AuthChatRequest&messageType=text%2Fjson&isQueued=false");
            //https://CRMDAC.CRMUD.dev.CRM.VRM.vba.VA.GOV:8085/Servicebus
            AuthChatRequest authChatRequest = new AuthChatRequest() { CallAgentId = @"vrmcloud\crmtestuser1", OrgName = "CRMEDEV", ChatSessionId = "300005df-742e-4ec6-8", Ssn = "796127587" };

            int messageSize = 10;

            authChatRequest.ChatSessionLog = BuildLogMessage(messageSize).ToString();


            string message = await CallRestEndPoint(uri, authChatRequest);

            AuthChatResponse authChatResponse = new AuthChatResponse();

            authChatResponse = DeserializeResponse<AuthChatResponse>(message);

            Assert.IsTrue(authChatResponse.ErrorCode == 0);

        }

        private static StringBuilder BuildLogMessage(int messageSize)
        {
            StringBuilder sb = new StringBuilder(messageSize);
            for (int i = 0; i < messageSize; i++)
            {
                sb.Append("i");
            }

            return sb;
        }

        private static T DeserializeResponse<T>(string message)
        {
            T retObj;
            byte[] b = Convert.FromBase64String(message);
            UTF8Encoding enc = new UTF8Encoding();
            string mess = enc.GetString(b);

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(enc.GetBytes(mess), 0, mess.Length);

                ms.Position = 0;

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

                retObj = (T)ser.ReadObject(ms);
            }
            return retObj;
        }

        private static async Task<string> CallRestEndPoint(Uri uri, object obj)
        {
            HttpClient client = new HttpClient();

            string message = string.Empty; ;

            //StreamContent sc = new StreamContent(
            using (MemoryStream memStream = ObjectToJSonStream(obj))
            {
                string responseValue;

                using (StreamContent sc = new StreamContent(memStream))
                {

                    HttpResponseMessage response = await client.PostAsync(uri, sc);
                    response.EnsureSuccessStatusCode();
                    responseValue = await response.Content.ReadAsStringAsync();
                }

                int start = responseValue.IndexOf("<Message>") + 9;
                int end = responseValue.IndexOf("</Message>");

                if (end != -1)
                {

                    message = responseValue.Substring(start, end - start);
                }
            }
            return message;
        }

        private static MemoryStream ObjectToJSonStream(object obj)
        {
            MemoryStream memStream = new MemoryStream();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());

            ser.WriteObject(memStream, obj);

            memStream.Position = 0;
            return memStream;
        }


        private static void SerializeObjectToStream(object o, Stream reqStream)
        {
            using (MemoryStream stream1 = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(o.GetType());

                ser.WriteObject(stream1, o);

                stream1.Position = 0;

                stream1.CopyTo(reqStream);
            }
        }


        [TestMethod]
        public void SendReceiveAuthChat()
        {
            //Uri uri = new Uri("https://crmdac.xrm.va.gov:8085/Servicebus/Rest/SendReceive?messageId=Egain%23CrmLaunchUrlRequest&messageType=text%2Fjson&message=eyJDaGF0U2Vzc2lvbklkIjoiY2hhdHNlc3Npb25pZCIsIkVkaXBpIjoiZWRpcGkiLCJPcmdOYW1lIjoiQ1JNRURFViIsIlBhcnRpY2lwYW50SWQiOiJQYXJ0aWNpcGFudElkIiwiU3NuIjoiU3NuIiwiQ2FsbEFnZW50SWQiOiJkaHVybGV5QHZhLmdvdiIsIlZzb09yZ0lkIjoiVnNvT3JnSWQiLCJNZXNzYWdlSWQiOiI4MWFlNmNmYy1hYzU0LWMxMWMtMTA4MC0yYzYxNzI4YzgyYmUifQ%3D%3D&isQueued=false&toJSONString=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.stringify(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D&parseJSON=function%20(filter)%20%7B%0D%0A%20%20%20%20%20%20%20%20%20%20%20%20return%20JSON.parse(this%2C%20filter)%3B%0D%0A%20%20%20%20%20%20%20%20%7D");
            //Uri uri = new Uri("http://localhost:8085/Servicebus/Rest/SendReceive?messageId=Egain%23AuthChatRequest&messageType=text%2Fjson&isQueued=false");
            //Uri uri = new Uri("http://ipv4.fiddler:8085/Servicebus/Rest/SendReceive?messageId=Egain%23AuthChatRequest&messageType=text%2Fjson&isQueued=false");
            Uri uri = new Uri(_endPoint + "/Servicebus/Rest/SendReceive?messageId=Egain%23AuthChatRequest&messageType=text%2Fjson&isQueued=false");




            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = "POST";

            AuthChatRequest authChatRequest = new AuthChatRequest() { CallAgentId = @"vrmcloud\crmtestuser1", OrgName = "CRMEDEV", ChatSessionId = Guid.NewGuid().ToString().Substring(0, 20), Ssn = "796127587" };

            int length = 100;
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append("i");
            }
            authChatRequest.ChatSessionLog = sb.ToString();

            using (Stream reqStream = request.GetRequestStream())
            {
                SerializeObjectToStream(authChatRequest, reqStream);
            }




            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseValue = null;

            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        //xmlDoc.Load(reader);
                        responseValue = reader.ReadToEnd();
                    }
                }
            }


            int start = responseValue.IndexOf("<Message>") + 9;
            int end = responseValue.IndexOf("</Message>");


            string message = responseValue.Substring(start, end - start);

            byte[] b = Convert.FromBase64String(message);
            UTF8Encoding enc = new UTF8Encoding();
            string mess = enc.GetString(b);



        }


    }
        #endregion MessagePayloadTests

        #endregion UnitTestsFromCRMe
}
