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
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOGetVeteranInfoRequest)]
    [DataContract]
    public class UDOGetVeteranInfoRequest : MessageBase
    {
        [DataMember]
        public string crme_OrganizationName { get; set; }

        [DataMember]
        public Guid crme_UserId { get; set; }

        [DataMember]
        public string crme_SSN { get; set; }
    }
}
