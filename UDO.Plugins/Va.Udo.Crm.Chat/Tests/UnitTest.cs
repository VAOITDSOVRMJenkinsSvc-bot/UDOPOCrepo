using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VRM.Integration.Servicebus.Egain.UnitTest.ServicebusServiceReference;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Egain.Messages;
using VRM.Integration.Servicebus.Egain.State;
using VRM.Integration.Servicebus.Egain.Messages.Messages;

namespace VRM.Integration.Servicebus.Egain.UnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestCrmUrlRequestStep()
        {
            //Processor.CrmUrlRequestStep.Steps.CrmUrlRequestStep connection = new Processor.CrmUrlRequestStep.Steps.CrmUrlRequestStep();
            //CrmUrlRequestState state = new CrmUrlRequestState("12345", "CRME", "1234567890", "Session Log Test...");

            //connection.Execute(state);
        }

        [TestMethod]
        public void TestVimtCrmUrlRequestStep()
        {
            //var request = new CrmUrlRequest();
            //var response = request.SendReceive<CrmUrlResponse>();
        }

        [TestMethod]
        public void TestConnectVimtSend()
        {
            var message = new OneWayPassTest();
            message.Send();
        }

        [TestMethod]
        public void TwoWayPassTest()
        {
            var message = new TwoWayPassTest();
            var response = message.SendReceive<TwoWayPassResponseTest>();
            Assert.AreEqual(message.MessageId, response.MessageId);
        }

        [TestMethod]
        public void AnonChatRequestTest()
        {
            var message = new AnonChatRequest();
            message.OrgName = "CRME";
            message.ChatSessionId = "1234567890";
            message.ChatSessionLog = "Chat session logs...";

            message.Send();
        }

        [TestMethod]
        public void AnonChatRequest() 
        { 
            //AnonChatRequestHandler handler = new AnonChatRequestHandler();
            //var message = new AnonChatRequest()
            //{
            //    CallAgentId = "SWOFFORD",
            //    MessageId = "1234567890",
            //    ChatSessionLog = "This is a test...",
            //    ChatSessionId = "1234567890",
            //    OrgName = "CRMUD",
            //    VeteranId = "123121234",
            //    VsoAgentId = "SWOFFORD",
            //    VsoId = "12345",
            //    LaunchUrl = "http://www.google.com"
            //};

            //handler.Handle(message);
        }

        // AuthChatRequest
        // SessionId doesnt extis
        public void AuthChatRequestCreate() 
        {
            // Step - ConnectToCrme

            // Step - GetCrmeConfiguration

            // Step - Create Session
            //AuthChatRequestHandler handler = new AuthChatRequestHandler();
            //var message = new AuthChatRequest()
            //{
            //    CallAgentId = "SWOFFORD",
            //    MessageId = "1234567890",
            //    ChatSessionLog = "This is a test...",
            //    ChatSessionId = "1234567890",
            //    OrgName = "CRMUD",
            //    VeteranId = "123121234",
            //    VsoAgentId = "SWOFFORD",
            //    VsoId = "12345",
            //    LaunchUrl = "http://www.google.com"
            //};

            //handler.Handle(message);
        }

        // SessionId update for chat and cobrowse
        public void AuthChatRequestUpdate() 
        { 
        
        }

        // AuthCoBrowseRequest
        // SessionId doesnt extis
        public void AuthCoBrowseRequestCreate() 
        { 
        
        }
        // SessionId update for cobrowse
        public void AuthCoBrowseRequestUpdate() 
        { 
        
        }

        // AuthChatCoBrowseRequest
        // SessionId doesnt extis
        public void AuthChatCoBrowseRequestCreate() 
        { 
        
        }
        // SessionId update for chat and cobrowse
        public void AuthChatCoBrowseRequestUpdate() 
        { 
        
        }

        // select * from ChatCoBrowseSessionLog where ChatCoBrowseSessionLog.ChatSessionId = chatsessionid 
        // and ChatCoBrowseSessionLog.CoBrowseSessionId = cobrowseid


        public void CrmUrlRequest() 
        {
            // Return Url to 
            // Incoming message
            // Does Chat SessionId exist (never cobrowse)
            // Create record if not exits
            // Read sessionId
            // Return launch url field on record
        }

    }
}
