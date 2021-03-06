using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
/// <summary>
/// VIMT LOB Component for UDOgetVeteranIdentifiers,getVeteranIdentifiers method, Response.
/// Code Generated by IMS on: 6/5/2015 6:31:51 PM
/// Version: 2015.19.01
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace VRM.Integration.UDO.MVI.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOgetVeteranIdentifiersResponse)]
    [DataContract]
    public class UDOgetVeteranIdentifiersResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOgetVeteranIdentifiers UDOgetVeteranIdentifiersInfo { get; set; }
    }
    [DataContract]
    public class UDOgetVeteranIdentifiers
    {
        [DataMember]
        public string crme_ParticipantID { get; set; }
        [DataMember]
        public string crme_FileNumber { get; set; }
        [DataMember]
        public string crme_EDIPI { get; set; }
        [DataMember]
        public string crme_SSN { get; set; }
        [DataMember]
        public byte[] SSId { get; set; }
        [DataMember]
        public string crme_SensitivityLevel { get; set; }
        [DataMember]
        public string crme_FirstName { get; set; }
        [DataMember]
        public string crme_LastName { get; set; }
        [DataMember]
        public string crme_MiddleName { get; set; }
        [DataMember]
        public string crme_Gender { get; set; }
        [DataMember]
        public string crme_BranchOfService { get; set; }
        [DataMember]
        public string crme_PhoneNumber{ get; set; }
        [DataMember]
        public string crme_DateofDeath { get; set; }
        [DataMember]
        public string crme_SOJ { get; set; }
        [DataMember]
        public string crme_characterofdischarge { get; set; }
    }
}
