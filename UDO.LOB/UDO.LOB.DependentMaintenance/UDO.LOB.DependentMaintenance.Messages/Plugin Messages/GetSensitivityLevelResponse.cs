using System;

using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
//using VRM.Integration.Servicebus.Core;

//CSDEv
//namespace VRM.Integration.Servicebus.Bgs.Messages
namespace UDO.LOB.DependentMaintenance.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.GetBGSSensitivityLevelResponse)]
    [DataContract]
    public class GetSensitivityLevelResponse : MessageBase
    {
        [DataMember]
        public string SensitivityLevel { get; set; }
    }
}
