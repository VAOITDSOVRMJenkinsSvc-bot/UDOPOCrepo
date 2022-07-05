using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.UDO.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    [Export(typeof (IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOCombinedSelectedPersonResponse)]
    [DataContract]
    public class UDOCombinedSelectedPersonResponse : MessageBase
    {
        /// <summary>
        /// For CMRe, all we want back is the URL of the cmre_person that we either found, or that we created.
        /// </summary>
        [DataMember]
        public string URL { get; set; }
        [DataMember]
        public string SSIdString { get; set; }
        [DataMember]
        public byte[] SSId { get; set; }
        [DataMember]
        public string FileNumber { get; set; }
        [DataMember]
        public string Edipi { get; set; }
        [DataMember]
        public Guid contactId { get; set; }
        [DataMember]
        public Guid idProofId { get; set; }
        [DataMember]
        public string ParticipantId { get; set; }
        [DataMember]
        public string BranchOfService { get; set; }
        [DataMember]
        public string phonenumber { get; set; }
        [DataMember]
        public string VeteranSensitivityLevel { get; set; }
        [DataMember]
        public string GenderCode { get; set; }
        [DataMember]
        public string BirthDate { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string FamilyName { get; set; }
        [DataMember]
        public string DateofBirth { get; set; }
        [DataMember]
        public string Gender { get; set; }
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string MVIMessage { get; set; }
        [DataMember]
        public PatientPerson[] Person { get; set; }

        [DataMember]
        public string RawMviExceptionMessage { get; set; }
        
    }
}
