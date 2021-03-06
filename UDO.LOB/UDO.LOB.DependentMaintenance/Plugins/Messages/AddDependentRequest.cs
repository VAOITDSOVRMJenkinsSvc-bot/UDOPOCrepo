using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
//using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.Servicebus.AddDependent.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.AddDependentRequest)]
    [DataContract]
    public class AddDependentRequest : MessageBase, IAddDependentRequest
    {
        [DataMember]
        public VeteranParticipant Veteran { get; set; }

        [DataMember]
        public DependentParticipant[] Dependents { get; set; }

        [DataMember]
        public MaritalHistory[] MaritalHistories { get; set; }

        [DataMember]
        public string UserName { get; set; }
    }
}
