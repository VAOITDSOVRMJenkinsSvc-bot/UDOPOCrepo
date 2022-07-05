using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.MVI.Messages
{

    [DataContract]
    public class UDOfindVeteranInfoByPidResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOfindVeteranInfo UDOfindVeteranInfoInfo { get; set; }
    }
}
