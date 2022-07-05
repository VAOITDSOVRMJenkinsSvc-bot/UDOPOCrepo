using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VEIS.Core.Core;

namespace VEIS.Mvi.Messages.Common
{
    /// <summary>
    /// Base class for Add Person acknowledgment classes.
    /// </summary>
    [DataContract]
    public abstract class AcknowledgementBase
    {
        private List<Exception> _exceptions;

        /// <summary>
        /// The type of acknowledgement returned from the MVI call.
        /// </summary>
        [DataMember]
        public AcknowledgementType AckType { get; set; }

        /// <summary>
        /// The Id of the request message.
        /// </summary>
        [DataMember]
        public string RequestMessageId { get; set; }

        /// <summary>
        /// The Id of the response message.
        /// </summary>
        [DataMember]
        public string ResponseMessageId { get; set; }

        /// <summary>
        /// A list containing any messages returned from MVI.
        /// </summary>
        [DataMember]
        public List<string> AcknowledgementDetailText { get; set; }

        /// <summary>
        /// True if an exception occurred during VIMT message processing; False otherwise.
        /// </summary>
        [DataMember]
        public bool ExceptionOccurred { get; set; }

        /// <summary>
        /// Any exceptions caught during the processing of the message.
        /// </summary>
        [DataMember]
        public List<Exception> Exceptions
        {
            get { return _exceptions; }
            set
            {
                ExceptionOccurred = true;
                _exceptions = value;
            }
        }
    }

    /// <summary>
    /// Base class for Add Person response messages.
    /// </summary>
    public abstract class AddPersonResponseBase : MessageBase
    {
       /// <summary>
        /// Gets or sets the string representation of the Add Person To MVI message sent to MVI.
        /// </summary>
        public string MviRequestMessage { get; set; }

        /// <summary>
        /// Gets or sets the string representation of the acknowledgement message returned from MVI.
        /// </summary>
        public string MviResponseMessage { get; set; }
    }

    /// <summary>
    /// Base class for Add Person request messages.
    /// </summary>
    public abstract class AddPersonRequestBase : MessageBase
    {
        /// <summary>
        /// The Id of the request.
        /// </summary>
        public new string MessageId
        {
            get { return base.MessageId; }
            set { base.MessageId = value; }
        }

        /// <summary>
        /// Gets or sets the name of the CRM organization making the request.
        /// </summary>
        /// <remarks>This is a required field.</remarks>
        [DataMember]
        public string OrganizationName { get; set; }

        /// <summary>
        /// Gets or sets the type of processing for this message.
        /// </summary>
        /// <remarks>This is a required field. The default is Test.</remarks>
        [DataMember]
        public ProcessingType ProcessingCode { get; set; }

        /// <summary>
        /// True to return the raw request and response message back to the caller; False otherwise.
        /// </summary>
        [DataMember]
        public bool ReturnMviMessagesInResponse { get; set; }

        /// <summary>
        /// Gets or sets the CRM user making the request.
        /// </summary>
        /// <remarks>This is a required field.</remarks>
        [DataMember]
        public CrmUser Requestor { get; set; }
    }

    /// <summary>
    /// Concrete class implementation of the AcknowledgementBase abstract class
    /// </summary>
    public class Acknowledgement : AcknowledgementBase
    {
        /// <summary>
        /// Creates an AA Acknowledgement object with required values.
        /// </summary>
        /// <param name="requestId">The message Id of the request to MVI.</param>
        /// <param name="responseId">The message Id of the response message from MVI.</param>
        /// <returns>A properly formatted Acknowledgement object.</returns>
        public static Acknowledgement CreateOkAcknowledgement(string requestId, string responseId)
        {
            return new Acknowledgement
            {
                AckType = AcknowledgementType.OK,
                RequestMessageId = requestId,
                ResponseMessageId = responseId
            };
        }

        /// <summary>
        /// Creates an AE/AR Acknowledgement object with required values.
        /// </summary>
        /// <param name="type">The type of acknowledgement.</param>
        /// <param name="requestId">The message Id of the request to MVI.</param>
        /// <param name="responseId">The message Id of the response message from MVI.</param>
        /// <param name="acknowledgementDetails">A list of messages returned from MVI.</param>
        /// <returns>A properly formatted Acknowledgement object.</returns>
        public static Acknowledgement CreateReceiverErrorAcknowledgement(AcknowledgementType type, string requestId, string responseId, List<string> acknowledgementDetails)
        {
            return new Acknowledgement
            {
                AckType = type,
                RequestMessageId = requestId,
                ResponseMessageId = responseId,
                AcknowledgementDetailText = acknowledgementDetails
            };
        }

        /// <summary>
        /// Creates an Acknowledgement object for a message which causes an exception in VIMT before it is sent.
        /// </summary>
        /// <param name="requestId">The message Id of the request to MVI.</param>
        /// <param name="exceptions">A list of exceptions generated by VIMT.</param>
        /// <returns>A properly formatted Acknowledgement object.</returns>
        public static Acknowledgement CreateVimtErrorAcknowledgement(string requestId, List<Exception> exceptions)
        {
            return new Acknowledgement
            {
                AckType = AcknowledgementType.ApplicationErrorInSender,
                RequestMessageId = requestId,
                Exceptions = exceptions
            };
        }
    }

    /// <summary>
    /// Concrete class implementation of the AddPersonResponseBase abstract class
    /// </summary>
    public class AddPersonResponse : AddPersonResponseBase
    { }

    /// <summary>
    /// Concrete class implementation of the AddPersonRequestBase abstract class
    /// </summary>
    public class AddPersonRequest : AddPersonRequestBase
    { }

    /// <summary>
    /// Encapsulates data for a person's name.
    /// </summary>
    [DataContract]
    public class PersonName
    {
        /// <summary>
        /// Gets or sets the usage of the name.
        /// </summary>
        /// <remarks>This is a required field.</remarks>
        [DataMember]
        public NameUse Use { get; set; }

        /// <summary>
        /// Gets or sets the last name of the person.
        /// </summary>
        /// <remarks>This is a required field.</remarks>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the first name of the person.
        /// </summary>
        /// <remarks>This is a required field.</remarks>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets a middle name for the person.
        /// </summary>
        [DataMember]
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets a suffix associated with the name.
        /// </summary>
        [DataMember]
        public string NameSuffix { get; set; }

        /// <summary>
        /// Create's a person name object representing the person's legal name.
        /// </summary>
        /// <param name="lastName">The last name of the person.</param>
        /// <param name="firstName">The first name of the person.</param>
        /// <param name="middleName">The middle name of the person.</param>
        /// <param name="suffix">The name suffix of the person.</param>
        /// <returns>A properly formatted person name object.</returns>
        public static PersonName CreateLegalName(string lastName, string firstName, string middleName = "", string suffix = "")
        {
            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName))
                throw new ArgumentException("Person name requires a non-null last and first name.");

            return new PersonName
            {
                Use = NameUse.Legal,
                LastName = lastName,
                FirstName = firstName,
                MiddleName = middleName,
                NameSuffix = suffix
            };
        }
    }

}
