using System;

using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.Servicebus.Bgs.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.GetSensitivityLevelResponse)]
    [DataContract]
    public class GetSensitivityLevelResponse : MessageBase
    {
        [DataMember]
        public string SensitivityLevel { get; set; }
    }
}
