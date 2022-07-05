using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.MVI.Messages
{
    /// <summary>
    /// UDOGetVeteranInfoRequest
    /// </summary>
    /// <remarks>
    /// </remarks>
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
