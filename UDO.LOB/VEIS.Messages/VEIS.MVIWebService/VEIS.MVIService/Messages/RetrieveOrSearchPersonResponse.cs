
using System.Runtime.Serialization;
using VEIS.Core.Core;

namespace VEIS.Mvi.Messages
{

    [DataContract]
    public class RetrieveOrSearchPersonResponse : MessageBase
    {
        [DataMember]
        public PatientPerson[] Person { get; set; }

        [DataMember]
        public bool ExceptionOccured { get; set; }

        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Sets or gets the actual exception from MVI.
        /// </summary>
        [DataMember]
        public string RawMviExceptionMessage { get; set; }

        /// <summary>
        /// Sets or get the MVI Acknowlegment details that contain MVIerror codes and texts.
        /// </summary>
        [DataMember]
        public Acknowledgement Acknowledgement { get; set; }

        [DataMember]
        public QueryAcknowledgement QueryAcknowledgement { get; set; }

        [DataMember]
        public string OrganizationName { get; set; }

        //[DataMember]
        //public MessageProcessType FetchMessageProcessType { get; set; }
    }
}
