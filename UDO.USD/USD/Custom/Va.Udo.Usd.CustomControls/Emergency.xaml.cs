using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Va.Udo.Usd.CustomControls.Shared;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using System.Linq;

using Microsoft.Lync.Model.Extensibility;

namespace Va.Udo.Usd.CustomControls
{

    public partial class Emergency : BaseHostedControlCommon
    {
        Automation automation;

        string[] _RemoteUsersUri = null;
        string _message = null;

        private readonly TraceLogger _logWriter;

        public Emergency(Guid id, string applicationName, string initXml) :
            base(id, applicationName, initXml)
        {
            InitializeComponent();
            _logWriter = new TraceLogger();
        }

        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            _logWriter.Log("Entering DoAction");
            if (string.Compare(args.Action, "EmergencyButton-SendLink", StringComparison.OrdinalIgnoreCase) == 0)
            {
                var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
                _message = Utility.GetAndRemoveParameter(parms, "message");
                var recipients = Utility.GetAndRemoveParameter(parms, "recipients");
                Initiate(recipients, args);
            }
        }

        public void Initiate(string recipients, Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            try
            {
                //Start the conversation
                base.UpdateContext("EmergencyProcess", "Processing Step", "Initiate");

                automation = LyncClient.GetAutomation();
                
                base.UpdateContext("EmergencyProcess", "RAW Recipents", recipients.ToString());
                string[] participants = recipients.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                participants = participants.Distinct().ToArray();

                _RemoteUsersUri = participants;
                base.UpdateContext("EmergencyProcess", "Ptcpnts After Distinct()", string.Join("," , participants));

                CreateConversation();
            }
            catch (LyncClientException lyncClientException)
            {
                base.UpdateSessionContext(localSessionManager.GlobalSession, "elements", "recipients", recipients);


                var exp = ExceptionManager.ReportException(lyncClientException);
                base.UpdateContext("Emergency", "ExceptionMethod", "Initiate-Lync Client Exception");
                base.UpdateContext("Emergency", "ExceptionReport", exp);

                args.ActionReturnValue = "Failure";
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    base.UpdateSessionContext(localSessionManager.GlobalSession, "elements", "recipients", recipients);


                    var exp = ExceptionManager.ReportException(systemException);
                    base.UpdateContext("Emergency", "ExceptionMethod", "Initiate-Lync Exception");
                    base.UpdateContext("Emergency", "ExceptionReport", exp);

                    args.ActionReturnValue = "Failure";
                }
                else
                {
                    base.UpdateSessionContext(localSessionManager.GlobalSession, "elements", "recipients", recipients);

                    var exp = ExceptionManager.ReportException(systemException);
                    base.UpdateContext("Emergency", "ExceptionMethod", "Initiate-Unknown System Exception");
                    base.UpdateContext("Emergency", "ExceptionReport", exp);

                    args.ActionReturnValue = "Failure";
                }
            }
            catch (Exception ex)
            {
                var exp = ExceptionManager.ReportException(ex);
                base.UpdateContext("Emergency", "ExceptionMethod", "Initiate-Unhandled Exception");
                base.UpdateContext("Emergency", "ExceptionReport", exp);

                args.ActionReturnValue = "Failure";
            }
        }

        private void CreateConversation()
        {
            base.UpdateContext("EmergencyProcess", "Processing Step", "CreateConversation");

            //base.UpdateContext("EmergencyProcess", "Message-CreateConversation", _message);

            try
            {
                Dictionary<AutomationModalitySettings, object> conversationSettings = new Dictionary<AutomationModalitySettings, object>();
                conversationSettings.Add(AutomationModalitySettings.Subject, "UDO Initiated Emergency");

                //Make first call to ensure conversation is open and active. 
                //We are not able to send a message to multiple participants on conversation start so must do after
               var ar1 = automation.BeginStartConversation(AutomationModalities.InstantMessage, _RemoteUsersUri, null,
                                                  null, null);
                //End Initial Start Conversation and get the conversationwindow 
               var conversationWindow = automation.EndStartConversation(ar1);

                //Get the conversation from the window
               var conversation = conversationWindow.Conversation;

                //Send the message to the conversation
               var sendMessage = ((InstantMessageModality)conversation.Modalities[ModalityTypes.InstantMessage]).BeginSendMessage(
                       _message
                       , null
                       , null);

               ((InstantMessageModality)conversation.Modalities[ModalityTypes.InstantMessage]).EndSendMessage(sendMessage);
            }
            catch (LyncClientException lyncClientException)
            {
                var exp = ExceptionManager.ReportException(lyncClientException);
                base.UpdateContext("Emergency", "ExceptionMethod", "CreateConversation");
                base.UpdateContext("Emergency", "ExceptionReport", exp);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    var exp = ExceptionManager.ReportException(systemException);
                    base.UpdateContext("Emergency", "ExceptionMethod", "CreateConversation");
                    base.UpdateContext("Emergency", "ExceptionReport", exp);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    var exp = ExceptionManager.ReportException(systemException);
                    base.UpdateContext("Emergency", "ExceptionMethod", "CreateConversation");
                    base.UpdateContext("Emergency", "ExceptionReport", exp);
                }
            }
            catch (Exception ex)
            {
                var exp = ExceptionManager.ReportException(ex);
                base.UpdateContext("Emergency", "ExceptionMethod", "CreateConversation");
                base.UpdateContext("Emergency", "ExceptionReport", exp);
            }
        }


        #region OLD CODE

        //private void waitCreate()
        //{
        //    try
        //    {
        //        int recursion = 0;
        //        while (recursion < 1000)
        //        {
        //            if (_received) break;
        //            Thread.Sleep(10);
        //            recursion++;
        //        }
        //        //if (!_received) base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Timeout", "Timeout waiting for Conversation Create."); ;
        //    }
        //    catch(Exception ex)
        //    {
        //        //base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Unknown Exception during waitCreate", ex.Message);
        //        //base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Unknown Exception during waitCreate", ex.InnerException.ToString());


        //        var exp = ExceptionManager.ReportException(ex);
        //        base.UpdateContext("Emergency", "ExceptionMethod", "waitCreate");
        //        base.UpdateContext("Emergency", "ExceptionReport", exp);
        //    }
        //}

        //private void waitPtcpnt()
        //{
        //    try
        //    {
        //        int recursion = 0;
        //        while (recursion < 1000)
        //        {
        //            if (_added) break;
        //            Thread.Sleep(10);
        //            recursion++;
        //        }
        //        //if (!_added) base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Timeout", "Timeout waiting for Add Ptcpnts."); ;
        //    }
        //    catch (Exception ex)
        //    {
        //        //base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Unknown Exception during waitPtcpnt", ex.Message);
        //        //base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Unknown Exception during waitPtcpnt", ex.InnerException.ToString());


        //        var exp = ExceptionManager.ReportException(ex);
        //        base.UpdateContext("Emergency", "ExceptionMethod", "waitPtcpnt");
        //        base.UpdateContext("Emergency", "ExceptionReport", exp);
        //    }
        //}


        //private async void conversationManager_ConversationAdded(object sender, Microsoft.Lync.Model.Conversation.ConversationManagerEventArgs e)
        //{
        //    try
        //    {

        //        //base.UpdateContext("EmergencyProcess", "Processing Step", "Conversation Created");
        //       // e.Conversation.ParticipantAdded += new EventHandler<Microsoft.Lync.Model.Conversation.ParticipantCollectionChangedEventArgs>(Conversation_ParticipantAdded);
        //        e.Conversation.ParticipantAdded += Conversation_ParticipantAdded;
        //        e.Conversation.StateChanged += Conversation_StateChanged;

        //        _totalPtcpnts = _RemoteUsersUri.Count();

        //        base.UpdateContext("EmergencyProcess", "Total Number of Partcpnts Before Distinct", _totalPtcpnts.ToString());
        //        base.UpdateContext("EmergencyProcess", "USER LIST AFTER CONVERSATION ADD", _RemoteUsersUri.ToString());
        //        _RemoteUsersUri = _RemoteUsersUri.Distinct().ToArray();

        //        base.UpdateContext("EmergencyProcess", "Total Number of Partcpnts After Distinct", _totalPtcpnts.ToString());
        //        base.UpdateContext("EmergencyProcess", "RAW USER LIST AFTER DISTINCT", _RemoteUsersUri.ToString());




        //        foreach (string remoteUser in _RemoteUsersUri)
        //        {
        //            base.UpdateContext("EmergencyProcess", "Processing Step", string.Format("Adding: {0}", remoteUser));

        //            e.Conversation.AddParticipant(client.ContactManager.GetContactByUri(remoteUser));
        //        }
        //        await SafeDispatcher.BeginInvoke(new Action(() =>
        //        {
        //            //base.UpdateContext("EmergencyProcess", "Processing Step", "Waiting for Participants");
        //            waitPtcpnt();
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        var exp = ExceptionManager.ReportException(ex);
        //        base.UpdateContext("Emergency", "ExceptionMethod", "conversationManager_ConversationAdded");
        //        base.UpdateContext("Emergency", "ExceptionReport", exp);
        //    }
        //}

        //private void Conversation_StateChanged(object sender, Microsoft.Lync.Model.Conversation.ConversationStateChangedEventArgs e)
        //{            
        //    Microsoft.Lync.Model.Conversation.Conversation conversation = (Microsoft.Lync.Model.Conversation.Conversation)sender;
        //    base.UpdateContext("EmergencyProcess", "State Changed New State: ", e.NewState.ToString());

        //    if (e.NewState != Microsoft.Lync.Model.Conversation.ConversationState.Active)
        //    {
        //        base.UpdateContext("EmergencyProcess", "State Changed", "New State not Active");

        //        conversation.End();
        //        client.ConversationManager.Conversations.Remove(conversation);
        //    }
        //}

        //private void Conversation_ParticipantAdded(object sender, Microsoft.Lync.Model.Conversation.ParticipantCollectionChangedEventArgs e)
        //{
        //    try
        //    {

        //        //base.UpdateContext("EmergencyProcess", "Processing Step", string.Format("Participant Added: {0}", _Ptcpnts));
        //        //base.UpdateContext("EmergencyProcess", "# participants", _Ptcpnts.ToString());
        //        //base.UpdateContext("EmergencyProcess", "total participants", _totalPtcpnts.ToString());
        //        if (e.Participant.IsSelf == false)
        //        {
        //            _Ptcpnts++;
        //            if (_totalPtcpnts == _Ptcpnts)
        //            {
        //                base.UpdateContext("EmergencyProcess", "Processing Step", "All Ptcpnts added");
        //                //Microsoft.Lync.Model.Conversation.Conversation conversation = (Microsoft.Lync.Model.Conversation.Conversation)sender;

        //                //Microsoft.Lync.Model.Conversation.InstantMessageModality imModality = (Microsoft.Lync.Model.Conversation.InstantMessageModality)conversation.Modalities[Microsoft.Lync.Model.Conversation.ModalityTypes.InstantMessage];
        //                //IDictionary<Microsoft.Lync.Model.Conversation.InstantMessageContentType, string> formattedMessage = this.Generate2013FormattedMessage(_message);

        //                try
        //                {
        //                    //base.UpdateContext("EmergencyProcess", "Processing Step", "Sending Msg");
        //                    //base.UpdateContext("EmergencyProcess", "Can Invoke", imModality.CanInvoke(Microsoft.Lync.Model.Conversation.ModalityAction.SendInstantMessage).ToString());
        //                    //base.UpdateContext("EmergencyProcess", "formattedMessage", formattedMessage.ToString());
        //                    //base.UpdateContext("EmergencyProcess", "Message", _message.ToString());

        //                    //if (imModality.CanInvoke(Microsoft.Lync.Model.Conversation.ModalityAction.SendInstantMessage))
        //                    //{
        //                    //    IAsyncResult asyncResult = imModality.BeginSendMessage(
        //                    //        formattedMessage,
        //                    //        SendMessageCallback,
        //                    //        imModality);
        //                    //}

        //                    SendMessage(_message);
        //                }
        //                catch (Microsoft.Lync.Model.LyncClientException ex)
        //                {
        //                    //base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Lync Client Exception", ex.Message);
        //                    //base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Lync Client Exception", ex.InnerException.ToString());

        //                    var exp = ExceptionManager.ReportException(ex);
        //                    base.UpdateContext("Emergency", "ExceptionMethod", "Conversation_ParticipantAdded");
        //                    base.UpdateContext("Emergency", "ExceptionReport", exp);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Unknown Exception during PtcpntAdded", ex.Message);
        //        //base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Unknown Exception during PtcpntAdded", ex.InnerException.ToString());


        //        var exp = ExceptionManager.ReportException(ex);
        //        base.UpdateContext("Emergency", "ExceptionMethod", "Conversation_ParticipantAdded");
        //        base.UpdateContext("Emergency", "ExceptionReport", exp);

        //    }
        //}

        //private void SendMessage(string messageToSend)
        //{
        //    try
        //    {
        //        IDictionary<Microsoft.Lync.Model.Conversation.InstantMessageContentType, string> textMessage = new Dictionary<Microsoft.Lync.Model.Conversation.InstantMessageContentType, string>();
        //        textMessage.Add(Microsoft.Lync.Model.Conversation.InstantMessageContentType.PlainText, messageToSend);
        //        if (((Microsoft.Lync.Model.Conversation.InstantMessageModality)_conversation.Modalities[Microsoft.Lync.Model.Conversation.ModalityTypes.InstantMessage]).CanInvoke(Microsoft.Lync.Model.Conversation.ModalityAction.SendInstantMessage))
        //        {
        //            ((Microsoft.Lync.Model.Conversation.InstantMessageModality)_conversation.Modalities[Microsoft.Lync.Model.Conversation.ModalityTypes.InstantMessage]).BeginSendMessage(
        //                textMessage
        //                , SendMessageCallback
        //                , textMessage);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var exp = ExceptionManager.ReportException(ex);
        //        base.UpdateContext("Emergency", "ExceptionMethod", "SendMessage");
        //        base.UpdateContext("Emergency", "ExceptionReport", exp);
        //    }
        //}

        //private void SendMessageCallback(IAsyncResult ar)
        //{
        //    //Microsoft.Lync.Model.Conversation.InstantMessageModality imModality = (Microsoft.Lync.Model.Conversation.InstantMessageModality)ar.AsyncState;

        //    if (ar.IsCompleted == true)
        //    {
        //        try
        //        {
        //            ((Microsoft.Lync.Model.Conversation.InstantMessageModality)_conversation.Modalities[Microsoft.Lync.Model.Conversation.ModalityTypes.InstantMessage]).EndSendMessage(ar);
        //            //imModality.EndSendMessage(ar);
        //        }
        //        catch (Microsoft.Lync.Model.LyncClientException lce)
        //        {
        //            //base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Lync Client Exception", lce.Message);
        //            //base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Lync Client Exception", lce.InnerException.ToString());

        //            var exp = ExceptionManager.ReportException(lce);
        //            base.UpdateContext("Emergency", "ExceptionMethod", "Lync Client Exception");
        //            base.UpdateContext("Emergency", "ExceptionReport", exp);
        //        }
        //        catch (Exception ex)
        //        {

        //            var exp = ExceptionManager.ReportException(ex);
        //            base.UpdateContext("Emergency", "ExceptionMethod", "Unknown Exception");
        //            base.UpdateContext("Emergency", "ExceptionReport", exp);
        //            // base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Unknown Exception", ex.Message);
        //        }
        //    }

        //}


        //private IDictionary<Microsoft.Lync.Model.Conversation.InstantMessageContentType, string> Generate2013FormattedMessage(string message)
        //{
        //    try
        //    {
        //        string formattedMessage = string.Empty;
        //        IDictionary<Microsoft.Lync.Model.Conversation.InstantMessageContentType, string> textMessage = new Dictionary<Microsoft.Lync.Model.Conversation.InstantMessageContentType, string>();

        //        formattedMessage = message;
        //        textMessage.Add(Microsoft.Lync.Model.Conversation.InstantMessageContentType.PlainText, formattedMessage);

        //        return textMessage;
        //    }
        //    catch (Exception ex)
        //    {
        //        //base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Unknown Exception", ex.Message);
        //        //base.UpdateSessionContext(localSessionManager.GlobalSession, "ExceptionMessage", "Unknown Exception", ex.InnerException.ToString());


        //        var exp = ExceptionManager.ReportException(ex);
        //        base.UpdateContext("Emergency", "ExceptionMethod", "Generate2013FormattedMessage");
        //        base.UpdateContext("Emergency", "ExceptionReport", exp);

        //        return new Dictionary<Microsoft.Lync.Model.Conversation.InstantMessageContentType, string>();
        //    }
        //}
        #endregion OLD CODE

        private bool IsLyncException(SystemException ex)
        {
            return
                ex is NotImplementedException ||
                ex is ArgumentException ||
                ex is NullReferenceException ||
                ex is NotSupportedException ||
                ex is ArgumentOutOfRangeException ||
                ex is IndexOutOfRangeException ||
                ex is InvalidOperationException ||
                ex is TypeLoadException ||
                ex is TypeInitializationException ||
                ex is InvalidCastException;
        }

    }
}
