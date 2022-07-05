using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;


namespace UDO.LOB.MVI.Messages
{
    [DataContract]
    public class UDOHandleDupCorpRecordResponse : MessageBase
    {
        [DataMember]
        public PatientPerson[] Person { get; set; }
        [DataMember]
        public string CORPDbMessage { get; set; }
        [DataMember]
        public string UDOMessage { get; set; }
        [DataMember]
        public bool ExceptionOccurred { get; set; }
    }
}
