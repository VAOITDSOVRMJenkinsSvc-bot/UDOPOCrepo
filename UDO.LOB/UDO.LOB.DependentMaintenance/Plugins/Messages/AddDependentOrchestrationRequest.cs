using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
//using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.Servicebus.AddDependent.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.AddDependentOrchestrationRequest)]
    [DataContract]
    public class AddDependentOrchestrationRequest : MessageBase, IAddDependentOchestrationRequest
    {
        [DataMember]
        public string OrganizationName{ get; set; }

        [DataMember]
        public Guid DependentMaintenanceId { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

		[DataMember]
		public bool Debug { get; set; }
	}
}
