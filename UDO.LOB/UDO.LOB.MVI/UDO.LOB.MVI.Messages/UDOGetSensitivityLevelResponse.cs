using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.MVI.Messages
{
    [DataContract]
    public class UDOGetSensitivityLevelResponse : MessageBase
    {

        [DataMember]
        public string SensitivityLevel { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public bool ExceptionOccured { get; set; }

    }
}
